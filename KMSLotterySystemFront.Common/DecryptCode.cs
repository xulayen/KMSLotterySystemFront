// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Common
// *文件名称：RegexExpress.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：数码相关解析
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using ENCRYPTLib;
using System.Collections;
using System.Data;
using KMSLotterySystemFront.DBUtility;

namespace KMSLotterySystemFront.Common
{
    #region CodeInfo
    /// <summary>
    /// 数码的原始生码信息
    /// </summary>
    public struct CodeCreateInfo
    {
        /// <summary>
        /// 厂家ID
        /// </summary>
        public String FacID;
        /// <summary>
        /// CCN生码产品ID
        /// </summary>
        public String ProID;
        /// <summary>
        /// 数码所在表名
        /// </summary>
        public String TableName;
        /// <summary>
        /// 生码日期
        /// </summary>
        public DateTime CreateDate;
        /// <summary>
        /// 生产线号
        /// </summary>
        public String Line;
        /// <summary>
        /// 班组号
        /// </summary>
        public String GroupID;
        /// <summary>
        /// 随机数码
        /// </summary>
        public String RoundCode;
    }

    /// <summary>
    /// 数码附属信息
    /// </summary>
    public struct CodeAttachedInfo
    {
        /// <summary>
        /// 数码是否存在
        /// </summary>
        public Boolean CodeIsExist;
        /// <summary>
        /// 数码是否被查询过
        /// </summary>
        public Boolean IsQueryed;
        /// <summary>
        /// 数码被查询的次数
        /// </summary>
        public Int32 QueryNum;
        /// <summary>
        /// 数码首次查询时间
        /// </summary>
        public DateTime FirstQueryDate;
        /// <summary>
        /// 允许最大查询次数
        /// </summary>
        public Int32 AllowQueryMaxNum;
        /// <summary>
        /// 是否逻辑变色
        /// </summary>
        public Boolean IsLogicColor;
        /// <summary>
        /// 逻辑变色读报位数
        /// </summary>
        public String LogicColorReadInfo;
        /// <summary>
        /// 逻辑变色
        /// </summary>
        public String LogicColor;
        /// <summary>
        /// 数码对应产品编号信息(由生产信息指定)
        /// </summary>
        public String ProID;
        /// <summary>
        /// 销售区域
        /// </summary>
        public String SaleArea;
        /// <summary>
        /// 数码虚拟厂家编号
        /// </summary>
        public String FacID;
        /// <summary>
        /// CCN生码产品ID
        /// </summary>
        public String CreateCodeProID;
        /// <summary>
        /// 起始厂家编号
        /// </summary>
        public String OldFacID;
        /// <summary>
        /// 防伪正确查询回复
        /// </summary>
        public String ResultS;
        /// <summary>
        /// 防伪错误查询回复
        /// </summary>
        public String ResultD;
        /// <summary>
        /// 防伪重复查询回复
        /// </summary>
        public String ResultLD;
        /// <summary>
        /// 未知作用
        /// </summary>
        public String ResultLS;
        /// <summary>
        /// 数码过期首次回复
        /// </summary>
        public String ExpiredReplyFirst;
        /// <summary>
        /// 数码过期重复回复
        /// </summary>
        public String ExpiredReplyRepeat;
        /// <summary>
        /// 数码超过最大查询次数
        /// </summary>
        public String Visit2;
        /// <summary>
        /// 数码对应的厂家名
        /// </summary>
        public String FactoryName;
        /// <summary>
        /// 数码对应的产品名
        /// </summary>
        public String ProductName;
        /// <summary>
        /// 数码有效期(天)
        /// </summary>
        public int CodeEffectiveNum;
        /// <summary>
        /// 数码已存在天数
        /// </summary>
        public int CodeExistedDay;

        /// <summary>
        /// 产品激活标志(P表是否激活)
        /// </summary>
        public string Activation_Flag;

        /// <summary>
        /// 产品是否启用激活(厂家产品表)
        /// </summary>
        public Boolean Is_Activation_Flag;

        /// <summary>
        /// 当产品需要激活而未激活时的返回信息
        /// </summary>
        public string ResultUA;

        /// <summary>
        /// 生产工厂名称
        /// </summary>
        public string FacName;

        /// <summary>
        /// 产品批次 2016年7月4日 20:07:58 徐磊添加
        /// </summary>
        public string BatchID;


        /// <summary>
        /// 产品生产批次 2017年1月11日 15:03:32 徐磊添加
        /// </summary>
        public string Batch;
    }

    /// <summary>
    /// 数码查询方式
    /// </summary>
    public enum CodeQueryType
    {
        W,
        S,
        T
    }

    /// <summary>
    /// 逻辑变色颜色名称
    /// </summary>
    public enum LogicColorEN
    {
        Black,
        Red,
        Green,
        Blue
    }

    #endregion

    public class DecryptCode
    {
        public static Hashtable hashColor = new Hashtable();

        static DecryptCode()
        {
            hashColor.Add("Black", "#221E1F");
            hashColor.Add("Red", "#EC008C");
            hashColor.Add("Green", "#00A650");
            hashColor.Add("Blue", "#00AEEF");
        }

        #region 1)本方法用于根据防伪码得到厂家产品编号
        /// <summary>
        /// 本方法用于根据防伪码得到厂家产品编号
        /// </summary>
        /// <param name="strSpcode">16位防伪码</param>
        /// <returns></returns>
        public static string GetFacProIDByCode(string strSpcode)
        {
            string facproID = null;
            string xulie = null;
            string code = null;
            try
            {
                if (strSpcode.Length == 16)
                {
                    EncryptImplClass objEncrytLib = new EncryptImplClass();
                    facproID = objEncrytLib.Decode(strSpcode);
                    facproID = objEncrytLib.Decode(facproID);
                    facproID = objEncrytLib.DataDecompress(facproID, out xulie);
                    if (facproID.StartsWith("0") || facproID.StartsWith("9"))
                    {
                        xulie = facproID.Substring(4, 2) + xulie.Substring(0, 2) + xulie.Substring(3, 1) + xulie.Substring(6, 1);
                        code = objEncrytLib.DataMapDate(xulie);
                        facproID = facproID.Substring(0, 2) + code.Substring(8, 2) + facproID.Substring(2, 2);
                    }
                    else
                    {
                        facproID = facproID.Substring(0, 4);
                    }
                }
                else
                {
                    return null;
                }
                return facproID;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 2)本方法用于获取防伪码包含的原始生码信息
        /// <summary>
        /// 本方法用于获取防伪码包含的原始生码信息
        /// </summary>
        /// <param name="strSpcode">16位防伪码</param>
        /// <returns></returns>
        public static CodeCreateInfo GetCodeInfo(string strSpcode)
        {
            CodeCreateInfo objCodeInfo = new CodeCreateInfo();
            string facproID = null;
            string xulie = null;
            string code = null;
            try
            {
                if (strSpcode.Length == 16)
                {
                    EncryptImplClass objEncrytLib = new EncryptImplClass();
                    facproID = objEncrytLib.Decode(strSpcode);
                    facproID = objEncrytLib.Decode(facproID);
                    facproID = objEncrytLib.DataDecompress(facproID, out xulie);
                    xulie = facproID.Substring(4, 2) + xulie.Substring(0, 2) + xulie.Substring(3, 1) + xulie.Substring(6, 1);
                    code = objEncrytLib.DataMapDate(xulie);
                    //组合各字段信息
                    objCodeInfo.FacID = facproID.Substring(0, 2);
                    //objCodeInfo.CreateDate = code.Substring(0, 8);
                    objCodeInfo.CreateDate = DateTime.Parse(code.Substring(0, 4) + "-" + code.Substring(4, 2) + "-" + code.Substring(6, 2));
                    objCodeInfo.Line = code.Substring(8, 2);
                    objCodeInfo.GroupID = code.Substring(10, 1);
                    objCodeInfo.ProID = facproID.Substring(2, 2);

                    if (facproID.StartsWith("0"))
                    {
                        objCodeInfo.FacID = objCodeInfo.FacID + objCodeInfo.Line + objCodeInfo.ProID.Substring(0, 1);
                        //objCodeInfo.CreateDate = code.Substring(0, 8);
                        objCodeInfo.Line = "";
                        objCodeInfo.GroupID = code.Substring(10, 1);
                        objCodeInfo.ProID = objCodeInfo.ProID.Substring(objCodeInfo.ProID.Length - 1, 1);
                    }
                    else if (facproID.StartsWith("9"))
                    {
                        objCodeInfo.FacID = objCodeInfo.FacID + objCodeInfo.Line;
                        //objCodeInfo.CreateDate = code.Substring(0, 8);
                        objCodeInfo.Line = "";
                        objCodeInfo.GroupID = code.Substring(10, 1);
                        objCodeInfo.ProID = facproID.Substring(2, 2);
                    }
                    objCodeInfo.TableName = "T_INFOCODE_P" + objCodeInfo.FacID + objCodeInfo.ProID;
                }
                return objCodeInfo;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("数码解析异常:" + ex.Message);
            }
        }
        #endregion

        #region 3)获取数码变色后的HTML值
        /// <summary>
        /// 获取数码变色后的HTML值
        /// </summary>
        /// <param name="code">16位数码</param>
        /// <param name="queryType">防伪查询方式(W,S,T)</param>
        /// <param name="colorReplyConfig">数码需要显示颜色的字符配置(从后往前)T[4],W[16],S[4]</param>
        /// <param name="colorInfo">数码颜色的信息(绿色:绿色:蓝色:绿色:蓝色:绿色:黑色:红色:黑色:红色:红色:蓝色:黑色:黑色:绿色:蓝色)</param>
        /// <returns></returns>
        public static String GetCodeColorToHtml(String code, CodeQueryType queryType, String colorReplyConfig, String colorInfo)
        {
            string codeRet = "";
            string[] colorRC = colorReplyConfig.Split(',');
            String[] colorArr = colorInfo.Split(':');
            CharHelper charHelper = new CharHelper();


            int codeReplyLen = 0;
            foreach (String config in colorRC)
            {
                if (config.StartsWith(queryType.ToString()))
                {
                    string colorReply = charHelper.ReplaceQuanJiaoToBanJiao(config);
                    colorReply = colorReply.Replace(queryType.ToString(), "").Replace("[", "").Replace("]", "");
                    if (colorReply != "")
                    {
                        codeReplyLen = int.Parse(colorReply);
                    }
                }
            }
            StringBuilder strRetHtml = new StringBuilder("</br><table><tr>");
            StringBuilder strColorhtml = new StringBuilder();
            StringBuilder strCodehtml = new StringBuilder(code.Substring(0, code.Length - codeReplyLen));
            String strCodeReply = code.Substring(code.Length - codeReplyLen, codeReplyLen);
            String color = "";
            for (int i = code.Length - codeReplyLen; i < code.Length; i++)
            {
                color = "";
                color = colorArr[i];
                switch (color)
                {
                    case "黑色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Black.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Black.ToString()].ToString() + "\">" + color + "</font></td>");
                        break;
                    case "绿色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Green.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Green.ToString()].ToString() + "\">" + color + "</font></td>");
                        break;
                    case "蓝色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Blue.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Blue.ToString()].ToString() + "\">" + color + "</font></td>");
                        break;
                    case "红色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Red.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Red.ToString()].ToString() + "\">" + color + "</font></td>");
                        break;
                }
            }
            strRetHtml.Append(strCodehtml + "</tr><tr align=\"center\">");
            strRetHtml.Append(strColorhtml + "</tr></table>");
            codeRet = strRetHtml.ToString();
            return codeRet;
        }

        /// <summary>
        /// 获取数码变色后的HTML值
        /// </summary>
        /// <param name="code">16位数码</param>
        /// <param name="queryType">防伪查询方式(W,S,T)</param>
        /// <param name="colorReplyConfig">数码需要显示颜色的字符配置(从后往前)T[4],W[16],S[4]</param>
        /// <param name="colorInfo">数码颜色的信息(绿色:绿色:蓝色:绿色:蓝色:绿色:黑色:红色:黑色:红色:红色:蓝色:黑色:黑色:绿色:蓝色)</param>
        /// <param name="channel">防伪查询通道类型</param>
        /// <returns></returns>
        public static String GetCodeColorToHtml(String code, CodeQueryType queryType, String colorReplyConfig, String colorInfo, string channel)
        {
            string codeRet = "";
            string[] colorRC = colorReplyConfig.Split(',');
            String[] colorArr = colorInfo.Split(':');
            CharHelper charHelper = new CharHelper();


            int codeReplyLen = 0;
            foreach (String config in colorRC)
            {
                if (config.StartsWith(queryType.ToString()))
                {
                    string colorReply = charHelper.ReplaceQuanJiaoToBanJiao(config);
                    colorReply = colorReply.Replace(queryType.ToString(), "").Replace("[", "").Replace("]", "");
                    if (colorReply != "")
                    {
                        codeReplyLen = int.Parse(colorReply);
                    }
                }
            }
            StringBuilder strRetHtml = new StringBuilder("</br><table><tr>");
            StringBuilder strColorhtml = new StringBuilder();
            StringBuilder strCodehtml = new StringBuilder(code.Substring(0, code.Length - codeReplyLen));
            String strCodeReply = code.Substring(code.Length - codeReplyLen, codeReplyLen);
            String color = "";
            for (int i = code.Length - codeReplyLen; i < code.Length; i++)
            {
                color = "";
                color = colorArr[i];

                if (i == 8)
                {
                    if (channel.ToString().Equals("X") || channel.ToString().Equals("M"))
                    {
                        strCodehtml.Append("</tr><tr align=\"center\">");
                        strColorhtml.Append("</tr><tr><td>&nbsp;</td></tr><tr align=\"center\">");

                        strRetHtml.Append(strCodehtml);
                        strRetHtml.Append(strColorhtml);

                        strCodehtml = new StringBuilder();
                        strColorhtml = new StringBuilder();

                    }
                }
                switch (color)
                {
                    case "黑色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Black.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Black.ToString()].ToString() + "\">" + ReplaceColorName(channel.ToString(), color) + "</font></td>");
                        break;
                    case "绿色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Green.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Green.ToString()].ToString() + "\">" + ReplaceColorName(channel.ToString(), color) + "</font></td>");
                        break;
                    case "蓝色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Blue.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Blue.ToString()].ToString() + "\">" + ReplaceColorName(channel.ToString(), color) + "</font></td>");
                        break;
                    case "红色":
                        strCodehtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Red.ToString()].ToString() + "\">" + code.Substring(i, 1) + "</font></td>");
                        strColorhtml.Append("<td align=\"center\"><font color=\"" + hashColor[LogicColorEN.Red.ToString()].ToString() + "\">" + ReplaceColorName(channel.ToString(), color) + "</font></td>");
                        break;
                }


            }
            strRetHtml.Append(strCodehtml + "</tr><tr align=\"center\">");
            strRetHtml.Append(strColorhtml + "</tr></table>");
            codeRet = strRetHtml.ToString();
            return codeRet;
        }

        /// <summary>
        /// WAP和微信替换颜色
        /// </summary>
        /// <param name="channel">通道类型</param>
        /// <param name="colorname">颜色名称</param>
        /// <returns></returns>
        public static string ReplaceColorName(string channel, string colorname)
        {
            if (channel.ToUpper().Equals("M") || channel.ToUpper().Equals("X"))
            {
                return colorname.Replace("色", "");
            }
            return colorname;
        }
        #endregion

        #region 4)判断是否进行二次加密
        /// <summary>
        /// 判断是否进行二次加
        /// </summary>
        /// <param name="_pid">产品编码</param>
        /// <returns></returns>
        public static bool CheckEncypt(string _pid)
        {

            string sql = "select f.is_security from t_fac_product f where f.factoryid||f.productid='{0}'";
            bool isfalg = false;
            try
            {
                if (!string.IsNullOrEmpty(_pid))
                {
                    DataBase dataBase = new DataBase();
                    object oRet = dataBase.ExecuteScalar(CommandType.Text, string.Format(sql, _pid), null);
                    if (oRet != null)
                    {
                        isfalg = oRet.ToString() == "1" ? true : false;
                    }
                }
            }
            catch (Exception)
            {
                return isfalg;
            }
            return isfalg;
        }
        #endregion

        #region 4)判断是否进行二次加密
        /// <summary>
        /// 判断佳能数码的产品类型
        /// </summary>
        /// <param name="_fid">厂家id</param>
        /// <param name="pid">产品id</param>
        /// <returns></returns>
        public static bool CheckProductTypeConon(string _fid, string pid)
        {

            string sql = "select f.type from t_fac_product f where f.factoryid ='{0}' and f.productid='{1}'";
            bool isfalg = false;
            try
            {
                if (!string.IsNullOrEmpty(_fid) && !string.IsNullOrEmpty(pid))
                {
                    DataBase dataBase = new DataBase();
                    object oRet = dataBase.ExecuteScalar(CommandType.Text, string.Format(sql, _fid, pid), null);
                    if (oRet != null)
                    {
                        isfalg = oRet.ToString() == "0" ? true : false;
                    }
                }
            }
            catch (Exception)
            {
                return isfalg;
            }
            return isfalg;
        }
        #endregion

        ///// <summary>
        /////  补丁 (检查数码是否是威刚重复数码)
        ///// </summary>
        ///// <param name="_code">查询的数码</param>
        ///// <returns></returns>
        //public static bool CheckRepeatBarcode(string _code)
        //{
        //    string sql = "select r.barcode from T_ADATA_RepeatBarcode r where r.barcode='{0}'";
        //    bool isfalg = false;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(_code))
        //        {
        //            DataBase dataBase = new DataBase();
        //            object oRet = dataBase.ExecuteScalar(CommandType.Text, string.Format(sql, _code), null);
        //            if (oRet != null)
        //            {
        //                isfalg = oRet.ToString() == _code ? true : false;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return isfalg;
        //    }
        //    return isfalg;
        //}

        #region 5)判断是否需要经过二次加密和获取二次加密的数码
        /// <summary>
        /// 判断是否需要经过二次加密和获取二次加密的数码
        /// </summary>
        /// <param name="strSpcode">16位防伪码</param>
        /// <param name="outcode">二次加密的数码</param>
        /// <returns>是否需要二次加密</returns>
        public static bool GetAllEncypt(CodeCreateInfo objCodeInfo, string strSpcode, out string outcode)
        {
            bool isencypt = false;
            outcode = "";
            try
            {
                if (strSpcode.Length == 16)
                {
                    EncryptImplClass objEncrytLib = new EncryptImplClass();

                    isencypt = CheckEncypt(objCodeInfo.FacID + objCodeInfo.ProID);
                    if (isencypt)
                    {
                        outcode = objEncrytLib.NewEncrypt(strSpcode);
                    }
                }
                return isencypt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("数码解析异常:" + ex.Message);
            }

        }

        /// <summary>
        /// 使用注册组件替换颜色
        /// </summary>
        /// <param name="_color">颜色</param>
        /// <returns></returns>
        public static string RegColor(string _color)
        {
            string strcolor = string.Empty;
            try
            {

                EncryptImplClass objEncrytLib = new EncryptImplClass();
                if (!string.IsNullOrEmpty(_color))
                {
                    strcolor = objEncrytLib.LogicColor(_color);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("数码解析异常:" + ex.Message);
            }
            return strcolor;
        }
        #endregion

    }
}
