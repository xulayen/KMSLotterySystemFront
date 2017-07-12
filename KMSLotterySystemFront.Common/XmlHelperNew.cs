using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace KMSLotterySystemFront.Common
{
    /// <summary>
    /// Xml操作帮助类
    /// </summary>
    public class XmlHelperNew
    {
        #region 初始化
        const string VERSION = "1.0"; //默认xml版本
        const string ENCODING = "utf-8";//默认xml编码
        private XmlHelperNew()
        {

        }
        #endregion


        public static string GetXmlDocument(string filename)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                //加载文档
                document.Load(filename);
                return document.InnerXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region 创建XML文件和XML操作

        /// <summary>
        /// 创建XML文件
        /// </summary>
        /// <param name="fileName">创建文件的完全限定名(包含路径)</param>
        /// <param name="rootNodeName">根节点名称</param>
        /// <param name="encoding">xml文档编码 (默认:utf-8)</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool CreateXmlDocument(string oldxml, string rootNodeName, string encoding, out string xml)
        {
            xml = string.Empty;
            bool success = false;
            try
            {
                XmlDocument document = new XmlDocument();
                XmlDeclaration declaration = document.CreateXmlDeclaration(VERSION, encoding, "yes");
                XmlNode root = document.CreateElement(rootNodeName);
                document.AppendChild(declaration);
                document.AppendChild(root);

                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
            }
            return success;
        }
        /// <summary>
        /// 创建XML文件
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="rootNodeName">根节点名称</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool CreateXmlDocument(string oldxml, string rootNodeName, out string xml)
        {
            xml = string.Empty;
            return CreateXmlDocument(oldxml, rootNodeName, ENCODING, out xml);
        }

        /// <summary>
        /// 创建一个子节点
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询父节点的XPath路径</param>
        /// <param name="xmlNodeName">创建的节点名称</param>
        /// <param name="innerXml">创建的节点内xml文字</param>
        /// <param name="attributes">需要创建的属性字典(为NULL,则不创建属性)</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool CreateXmlNode(string oldxml, string xPath, string xmlNodeName, string innerXml, Dictionary<string, string> attributes, out string xml)
        {
            xml = string.Empty;
            bool success = false;

            XmlDocument document = new XmlDocument();
            try
            {
                //加载文档

                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));

                XmlNode node = document.SelectSingleNode(xPath);
                if (node != null)
                {
                    XmlElement element = document.CreateElement(xmlNodeName);
                    if (innerXml != null) { element.InnerXml = innerXml; }

                    //添加属性
                    if (attributes != null && attributes.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> attribute in attributes)
                        {
                            if ((!string.IsNullOrEmpty(attribute.Key)) && (!string.IsNullOrEmpty(attribute.Value)))
                            {
                                XmlAttribute attr = document.CreateAttribute(attribute.Key);
                                attr.Value = attribute.Value;

                                element.Attributes.Append(attr);
                            }
                        }
                    }

                    node.AppendChild(element);
                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw ex;
            }

            return success;
        }

        /// <summary>
        /// 创建或修改一个子节点
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询父节点的XPath路径</param>
        /// <param name="xmlNodeName">创建的节点名称</param>
        /// <param name="innerXml">创建的节点内xml文字</param>
        /// <param name="attributes">需要创建的属性字典(为NULL,则不创建属性)</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool CreateOrUpdateXmlNode(string oldxml, string xPath, string xmlNodeName, string innerXml, Dictionary<string, string> attributes, out string xml)
        {
            xml = string.Empty;
            bool success = false;
            bool exsit = false;//标示是否已经存在该节点
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);
                if (node != null)
                {
                    exsit = true;
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Name.ToLower() == xmlNodeName.ToLower())
                        {
                            //存在 执行更新
                            if (innerXml != null) { node.InnerXml = innerXml; }
                            //添加属性
                            if (attributes != null && attributes.Count > 0)
                            {
                                foreach (KeyValuePair<string, string> attribute in attributes)
                                {
                                    if ((!string.IsNullOrEmpty(attribute.Key)) && (!string.IsNullOrEmpty(attribute.Value)))
                                    {
                                        if (item.Attributes[attribute.Key] != null)
                                        {
                                            //存在该属性，更新
                                            item.Attributes[attribute.Key].Value = attribute.Value;
                                        }
                                        else
                                        {
                                            //不存在该属性，新增
                                            XmlAttribute nodeAttribute = document.CreateAttribute(attribute.Key);
                                            nodeAttribute.Value = attribute.Value;
                                            item.Attributes.Append(nodeAttribute);
                                        }
                                    }
                                }
                            }
                            exsit = true;
                            break;
                        }
                    }
                    if (!exsit)
                    {
                        //不存在，执行添加
                        //TODO:: success??
                        success = CreateXmlNode(oldxml, xPath, xmlNodeName, innerXml, attributes, out xml);
                    }
                    else
                    {
                        xml = document.InnerXml;
                    }
                }

                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw ex;

            }
            return success;
        }

        public static bool UpdateXmlNode(string oldxml, string xPath, Dictionary<string, string> attributes, out string xml)
        {
            xml = string.Empty;
            bool success = false;
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode item = document.SelectSingleNode(xPath);
                if (item != null)
                {
                    //添加属性
                    if (attributes != null && attributes.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> attribute in attributes)
                        {
                            if ((!string.IsNullOrEmpty(attribute.Key)) && (!string.IsNullOrEmpty(attribute.Value)))
                            {
                                if (item.Attributes[attribute.Key] != null)
                                {
                                    //存在该属性，更新
                                    item.Attributes[attribute.Key].Value = attribute.Value;
                                }
                                else
                                {
                                    //不存在该属性，新增
                                    XmlAttribute nodeAttribute = document.CreateAttribute(attribute.Key);
                                    nodeAttribute.Value = attribute.Value;
                                    item.Attributes.Append(nodeAttribute);
                                }
                            }
                        }
                    }
                }
                xml = document.InnerXml;
                success = true;

            }
            catch (XmlException ex)
            {
                success = false;
                throw ex;

            }
            return success;
        }

        /// <summary>
        /// 创建或修改一个属性
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询该属性节点的XPath路径</param>
        /// <param name="name">创建或修改的属性名称</param>
        /// <param name="value">创建或修改的属性值</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool CreateOrUpdateAttribute(string oldxml, string xPath, string name, string value, out string xml)
        {
            xml = string.Empty;
            bool success = false;
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);

                if (node != null)
                {
                    XmlAttributeCollection attributes = node.Attributes;
                    if (attributes != null)
                    {
                        if (attributes[name] != null)
                        {
                            //存在属性 更新
                            attributes[name].Value = value;
                        }
                        else
                        {
                            //不存在属性 创建
                            XmlAttribute atr = document.CreateAttribute(name);
                            atr.Value = value;
                            node.Attributes.Append(atr);
                        }
                    }
                    else
                    {
                        //不存在属性 创建
                        XmlAttribute atr = document.CreateAttribute(name);
                        atr.Value = value;
                        node.Attributes.Append(atr);
                    }
                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw ex;
            }

            return success;
        }

        //创建或更新多个属性
        public static bool CreateOrUpdateAttribute(string oldxml, string xPath, Dictionary<string, string> attrs, out string xml)
        {
            xml = string.Empty;
            bool success = false;
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);

                if (node != null)
                {
                    XmlAttributeCollection attributes = node.Attributes;
                    if (attributes != null)
                    {
                        foreach (var attr in attrs)
                        {
                            if (attributes[attr.Key] != null)
                            {
                                //存在属性 更新
                                attributes[attr.Key].Value = attr.Value;
                            }
                            else
                            {
                                //不存在属性 创建
                                XmlAttribute atr = document.CreateAttribute(attr.Key);
                                atr.Value = attr.Value;
                                node.Attributes.Append(atr);
                            }
                        }
                    }
                    else
                    {
                        foreach (var attr in attrs)
                        {
                            XmlAttribute atr = document.CreateAttribute(attr.Key);
                            atr.Value = attr.Value;
                            node.Attributes.Append(atr);
                        }
                    }
                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw ex;
            }

            return success;
        }

        //创建或更新多个属性
        public static bool CreateOrUpdateAttribute(string oldxml, string xPath, Dictionary<string, string> attrs, string InnerText, out string xml)
        {
            xml = string.Empty;
            bool success = false;
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);

                if (node != null)
                {
                    XmlAttributeCollection attributes = node.Attributes;
                    if (attributes != null)
                    {
                        foreach (var attr in attrs)
                        {
                            if (attributes[attr.Key] != null)
                            {
                                //存在属性 更新
                                attributes[attr.Key].Value = attr.Value;
                            }
                            else
                            {
                                //不存在属性 创建
                                XmlAttribute atr = document.CreateAttribute(attr.Key);
                                atr.Value = attr.Value;
                                node.Attributes.Append(atr);
                            }
                        }
                    }
                    else
                    {
                        foreach (var attr in attrs)
                        {
                            XmlAttribute atr = document.CreateAttribute(attr.Key);
                            atr.Value = attr.Value;
                            node.Attributes.Append(atr);
                        }
                    }
                    if (!string.IsNullOrEmpty(InnerText))
                    {
                        node.InnerText = InnerText;
                    }
                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw ex;
            }

            return success;
        }

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询该节点的XPath路径</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool DeleteXmlNode(string oldxml, string xPath, out string xml)
        {
            xml = string.Empty;
            bool success = true;
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);
                if (node != null)
                {
                    //删除节点
                    node.ParentNode.RemoveChild(node);
                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw;
            }
            return success;
        }
        /// <summary>
        /// 批量删除节点
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询节点集合的XPath路径</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool BatchDeleteXmlNode(string oldxml, string xPath, out string xml)
        {
            xml = string.Empty;
            bool success = true;
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNodeList nodeList = document.SelectNodes(xPath);
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        //删除节点
                        node.ParentNode.RemoveChild(node);
                    }

                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw;
            }
            return success;
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询该属性节点的XPath路径</param>
        /// <param name="name">属性名称</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static bool DeleteAttribute(string oldxml, string xPath, string attributeName, out string xml)
        {
            xml = string.Empty;
            bool success = true;
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNodeList nodeList = document.SelectNodes(xPath);
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes != null && node.Attributes.Count > 0 && node.Attributes[attributeName] != null)
                        {
                            //删除该属性
                            node.Attributes.Remove(node.Attributes[attributeName]);
                        }
                    }

                }
                xml = document.InnerXml;
                success = true;
            }
            catch (XmlException ex)
            {
                success = false;
                throw;
            }
            return success;
        }

        /// <summary>
        /// 获取一个节点
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询该节点的XPath路径</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static XmlNode GetXmlNode(string oldxml, string xPath)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);
                return node;
            }
            catch (XmlException ex)
            {
                return null;
            }
        }

        #region 用户中心菜单处理


        /// <summary>
        ///  在节点项里面查找--根据节点的属性名称和属性值得到第一个符合的节点
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="xpath"></param>
        /// <param name="attrText"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNodeByAttribute(string oldxml, string xpath, string attrText, string attrValue)
        {
            XmlNodeList list = GetXmlNodeList(oldxml, "/root/items/item");
            XmlNode result = null;
            foreach (XmlNode temp in list)
            {
                string[] tempStrSZ = attrValue.Split(new char[] { '?' });
                string tempAttrValue = attrValue;
                if (temp.Attributes["matchParameter"] != null && temp.Attributes["matchParameter"].Value == "true")
                {
                    if (tempStrSZ.Length > 1 && !string.IsNullOrEmpty(tempStrSZ[1]) && attrValue.ToLower().Contains("pageindex"))
                    {
                        int charIndex = attrValue.LastIndexOf("&");
                        tempAttrValue = attrValue.Substring(0, charIndex);
                    }
                }
                else
                {
                    if (tempStrSZ.Length > 0 && !string.IsNullOrEmpty(tempStrSZ[0]))
                    {
                        tempAttrValue = tempStrSZ[0];
                    }
                }
                if (temp.Attributes[attrText] != null && temp.Attributes[attrText].Value.Equals(tempAttrValue, StringComparison.OrdinalIgnoreCase))
                {
                    result = temp;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 在节点树里面查找--根据节点的属性名称和属性值得到第一个符合的节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="attrText"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static XmlNode GetTreeXmlNodeByAttribute(XmlNodeList list, string attrText, string attrValue)
        {
            XmlNode result = null;
            foreach (XmlNode temp in list)
            {
                if (temp.Attributes != null && temp.Attributes[attrText] != null)
                {
                    if (temp.Attributes[attrText].Value == attrValue)
                    {
                        result = temp;
                        break;
                    }
                    else
                    {
                        result = GetTreeXmlNodeByAttribute(temp.ChildNodes, attrText, attrValue);
                    }
                }
                if (result != null)
                    break;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取多个节点
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询多个节点的XPath路径</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static XmlNodeList GetXmlNodeList(string oldxml, string xPath)
        {
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNodeList nodeList = document.SelectNodes(xPath);
                return nodeList;
            }
            catch (XmlException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="fileName">XML文件的完全限定路径(包含路径)</param>
        /// <param name="xPath">查询该属性节点的XPath路径</param>
        /// <returns>成功返回True,失败返回Fasle</returns>
        public static XmlAttribute GetXmlAttribute(string oldxml, string xPath, string name)
        {
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("utf-8").GetBytes(oldxml)));
                XmlNode node = document.SelectSingleNode(xPath);
                if (node != null)
                {
                    if (node.Attributes != null && node.Attributes.Count > 0 && node.Attributes[name] != null)
                    {
                        return node.Attributes[name];
                    }
                }
                return null;
            }
            catch (XmlException ex)
            {
                return null;
            }
        }
        #endregion
    }
}
