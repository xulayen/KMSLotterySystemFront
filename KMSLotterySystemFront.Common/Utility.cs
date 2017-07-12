using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.ComponentModel;
using System.Reflection;
using System.Net;

namespace KMSLotterySystemFront.Common
{

    public enum Channel
    {
        /// <summary>
        /// 网站
        /// </summary>
        W,

        /// <summary>
        /// Wap
        /// </summary>
        M,

        /// <summary>
        /// 电话
        /// </summary>
        T,

        /// <summary>
        /// 短信
        /// </summary>
        S,

        /// <summary>
        /// 中商App
        /// </summary>
        A,

        /// <summary>
        /// 微信查询
        /// </summary>
        X,

    }

    public enum Language
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        Chinese = 1,

        /// <summary>
        /// 英文
        /// </summary>
        English = 2,
    }

    public enum TraceType
    {
        US,
        LD,
        UA,
        S,
        L

    }

    public static class Utility
    {
        /// <summary>
        /// 读取clob类型并转换为文本
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="col">第几个字段</param>
        /// <returns></returns>
        public static string ClobToText(IDataReader reader, int col)
        {
            //读取BLOB字段中的内空并转换为字符串
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            byte[] buffer = new byte[1000];
            int j, i = 0;
            do
            {
                j = (int)reader.GetBytes(col, i, buffer, 0, buffer.Length);
                sb.Append(System.Text.Encoding.Unicode.GetString(buffer, 0, j));
                i += j;

            } while (j == buffer.Length);
            reader.Dispose();
            return sb.ToString();
        }


        /// <summary>
        /// 字符串的字节长度 
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串的字节长度</returns>
        public static int TrueLength(string str)
        {
            int intlenTotal = 0;
            int intn = str.Length;
            string strWord = "";
            int intAsc;
            for (int i = 0; i < intn; i++)
            {
                strWord = str.Substring(i, 1);
                intAsc = Convert.ToChar(strWord);
                if (intAsc < 0 || intAsc > 127)
                    intlenTotal = intlenTotal + 2;
                else
                    intlenTotal = intlenTotal + 1;
            }

            return intlenTotal;
        }

        /// <summary> 转半角的函数(DBC case) </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 获取完整跳转Url
        /// </summary>
        /// <param name="strPostUrl">Redirect page</param>
        /// <returns>完成Url</returns>
        public static string Get_PostUrl(string strPostUrl)
        {
            string strWWW = HttpContext.Current.Request.Url.Host;   //Domain name
            int intPort = HttpContext.Current.Request.Url.Port; //Port
            string strApplicationPath = HttpContext.Current.Request.ApplicationPath.TrimStart('/');  //Virtual directory
            StringBuilder strbuUrl = new StringBuilder();
            strbuUrl.Append("http://" + strWWW);
            if (intPort > 0)
            {
                strbuUrl.Append(":" + intPort + "/");
            }
            if (strApplicationPath.Length > 0)
            {
                strbuUrl.Append(strApplicationPath + "/");
            }
            if (strPostUrl.Length > 0)
            {
                strbuUrl.Append(strPostUrl);
            }
            return strbuUrl.ToString();
        }

        /// <summary>
        /// verify keyword
        /// </summary>
        /// <param name="strString">input string</param>
        /// <returns>whether format is correct</returns>
        public static bool Check_KeyWord(string strString)
        {
            string pattern = @"select|insert|delete|from|count\(|drop table|update|truncate|asc\
                                (|mid\(|char\(|xp_cmdshell|exec   master|netlocalgroup administrators|:|net user|""|or|and|)";
            if (Regex.IsMatch(strString, pattern, RegexOptions.IgnoreCase))
                return true;
            else
                return false;

        }

        #region Filter string
        /// <summary>
        /// Filter string
        /// </summary>
        /// <param name="strtext">Need to filter the word string</param>
        /// <returns>Filtered word string</returns>
        /// <history>2011.8.30 dean created</history>
        public static string Input_Filter(string strtext)
        {
            strtext = strtext.Trim();
            if (string.IsNullOrEmpty(strtext))
                return string.Empty;
            strtext = Regex.Replace(strtext, "[ \\s]{2,}", " "); //Two or more spaces
            strtext = Regex.Replace(strtext, "(<[b|B][r|R]/*>)+|(<[p|P](.| \\n)*?>)", "\n"); //<br>
            strtext = Regex.Replace(strtext, "( \\s*&[n|N][b|B][s|S][p|P];\\s*)+", " "); //&nbsp;
            strtext = Regex.Replace(strtext, "<(.| \\n)*?>", string.Empty); //other tag
            strtext = strtext.Replace("'", "''");
            return strtext;
        }
        #endregion


        #region Encrypt & Decrypt

        public static string _strQueryStringKey = "ccnyesno"; //DES encrypt Key 

        /// <summary>
        /// Encrypt URL
        /// </summary>
        /// <param name="strQueryString">String to be encrypted</param>
        /// <returns>Return encrypted string</returns>
        /// <history>2011.9.8 dean created</history>
        public static string EncryptQueryString(string strQueryString)
        {
            return Encrypt(strQueryString, _strQueryStringKey);
        }
        /// <summary>
        ///  Decrypt URL
        /// </summary>
        /// <param name="strQueryString">String to be decrypted</param>
        /// <returns>Return decrypted string</returns>
        /// <history>2011.9.8 dean created</history>
        public static string DecryptQueryString(string strQueryString)
        {
            return Decrypt(strQueryString, _strQueryStringKey);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strToEncrypt">需要的加密字符串</param>
        /// <param name="strKey">密钥</param>
        /// <returns>返回 加密字符串</returns>
        /// <history>2011.9.8 dean created</history>
        public static string Encrypt(string strToEncrypt, string strKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();   //put string into byte array   

            byte[] inputByteArray = Encoding.Default.GetBytes(strToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(strKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strToDecrypt">需要的解密字符串</param>
        /// <param name="strKey">密钥</param>
        /// <returns>返回 解密字符串</returns>
        /// <history>2011.9.8 dean created</history>
        public static string Decrypt(string strToDecrypt, string strKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputByteArray = new byte[strToDecrypt.Length / 2];
                for (int x = 0; x < strToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(strToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(strKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                StringBuilder ret = new StringBuilder();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {

                return "";
            }
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="originalString">需加密字符</param>
        /// <param name="codeLength">需加密长度(16~32)</param>
        /// <returns></returns>
        public static String MD5(String originalString, Int32 codeLength)
        {
            string tempPassword = "00000000000000000000000000000000";
            tempPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(originalString, "MD5").ToLower();
            if (codeLength > 32 || codeLength < 16)
                codeLength = 32;
            tempPassword = tempPassword.ToUpper().Substring(0, codeLength);

            string moveTemp = tempPassword.Substring(3, 5);
            tempPassword = tempPassword.Replace(moveTemp, "").Insert(7, moveTemp);

            return tempPassword;
        }
        #endregion

        #region 获取随机长度字符串
        /// <summary>
        /// 获取随机长度字符串
        /// </summary>
        /// <param name="length">长度5~32</param>
        /// <returns></returns>
        public static String DigitCode(int length)
        {
            string result = "";
            if (5 <= length && length <= 32)
                result = Guid.NewGuid().ToString().Replace("-", "").Substring(0, length - 1);

            return result;
        }
        #endregion

        #region 获取随机长度字符串
        /// <summary>
        /// 获取随机长度字符串
        /// </summary>
        /// <param name="length">长度5~32</param>
        /// <returns></returns>
        public static String DigitCode2(int length)
        {

            StringBuilder sb = new StringBuilder();

            //if (5 <= length && length <= 32)
            //    result = Guid.NewGuid().ToString().Replace("-", "").Substring(0, length - 1);

            char[] character = { '0', '1', '2', '3', '4', '5', '6', '8', '9' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < length; i++)
            {
                sb.Append(character[rnd.Next(character.Length)]);
            }
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// 验证Email地址
        /// </summary>
        /// <param name="strEmail">Email地址字符串</param>
        /// <returns>TRUE：正确的EMAIL地址FALSE：无效的EMAIL地址</returns>
        public static Boolean IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="strMobile"></param>
        /// <returns></returns>
        public static Boolean IsMobile(string strMobile)
        {
            return Regex.IsMatch(strMobile, @"^(13[0-9]|15[0-9]|18[0-9]||17[0-9])\d{8}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证数码是否为纯数字
        /// </summary>
        /// <param name="strCode">数码</param>
        /// <returns></returns>
        public static Boolean IsUrl(string strCode)
        {
            return Regex.IsMatch(strCode, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        }

        /// <summary>
        /// 网址验证
        /// </summary>
        /// <param name="strCode">数码</param>
        /// <returns></returns>
        public static Boolean IsCode(string strCode)
        {
            return Regex.IsMatch(strCode, @"^[0-9]*$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是不是中文
        /// </summary>
        /// <param name="source">待检测字符</param>
        /// <returns></returns>
        public static Boolean IsChinese(String source)
        {
            return Regex.IsMatch(source, @"^[\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证是不是正常字符 字母，数字，下划线的组合
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static Boolean IsNormalChar(String source)
        {
            return Regex.IsMatch(source, @"[\w\d_]+", RegexOptions.IgnoreCase);
        }

        public static Boolean IsPasswordChar(String source)
        {
            return IsNormalChar(source) && !Regex.IsMatch(source, @"[ ]+", RegexOptions.IgnoreCase);
        }




        #region 检查用户信息是否符合 用户的规则
        /// <summary>
        /// 检查用户信息是否符合 用户的规则
        /// </summary>
        /// <param name="rule">用户规则</param>
        /// <param name="userinfo">用户信息</param>
        /// <returns></returns>
        public static bool CheckUserRule(string rule, Dictionary<string, Dictionary<string, string>> userinfo)
        {

            bool flag = false, bRet = false;
            string provnice = "";
            string city = "";
            //存储逻辑判断结果的二维数组
            Dictionary<string, string> Resultlist = new Dictionary<string, string>();

            if (rule != null && rule != "")
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(rule);
                if (xdoc != null)
                {
                    XmlNodeList RuleLogicList = xdoc.SelectNodes("Rules/RuleLogic");
                    if (RuleLogicList != null && RuleLogicList.Count > 0)
                    {
                        try
                        {
                            foreach (XmlNode rulelogic in RuleLogicList)
                            {
                                string ruleLogicTemp = rulelogic.Attributes["logic"].Value.Trim(); //获取运算符 and 或者 or
                                //<item id="" name="MOBILE" typename="手机" column="MOBILE" dbtype="String" cssclass="" label="手机号码:" width="120" controlType="TextBox" operatortype="1" ctype="=">13127751581</item>
                                #region 循环数据判断
                                foreach (XmlNode item in rulelogic.ChildNodes)
                                {
                                    string itemvalue = ""; //比对值
                                    if (item.InnerText != null)
                                        itemvalue = item.InnerText.ToString().Trim();
                                    if (itemvalue == "") continue;

                                    string UserType = ""; //检测项
                                    if (item.Attributes["name"] != null)
                                        UserType = item.Attributes["name"].Value.Trim();

                                    Dictionary<string, string> useritem = null; //获取要比对的数据，这里是注册信息

                                    if (UserType == "Area")
                                    {
                                        Dictionary<string, string> provtmp = null;
                                        userinfo.TryGetValue(item.Attributes["columname"].Value.Trim(), out provtmp);
                                        if (provtmp != null)
                                        {
                                            KeyValuePair<string, string> pls = provtmp.ElementAt(0);
                                            provnice = pls.Value;
                                        }

                                        Dictionary<string, string> citytmp = null;
                                        userinfo.TryGetValue(item.Attributes["columcityname"].Value.Trim(), out citytmp);
                                        if (citytmp != null)
                                        {
                                            KeyValuePair<string, string> cls = citytmp.ElementAt(0);
                                            city = cls.Value;
                                        }


                                    }
                                    else
                                        if (UserType == "registerDateTime")
                                        {
                                            userinfo.TryGetValue(UserType.ToUpper(), out useritem);
                                            if (useritem == null)
                                                userinfo.TryGetValue("CREATEDATE", out useritem);
                                        }
                                        else
                                        {
                                            userinfo.TryGetValue(UserType.ToUpper(), out useritem);
                                        }

                                    string ctype = ""; //检查条件
                                    if (item.Attributes["ctype"] != null)
                                    {
                                        ctype = item.Attributes["ctype"].Value.Trim();
                                        ctype = ctype.Replace("&gt;", ">");
                                        ctype = ctype.Replace("&lt;", "<");
                                    }

                                    #region 检查项
                                    string userval = string.Empty;
                                    switch (UserType)
                                    {
                                        case "Area":
                                            if (provnice != "")
                                            {
                                                if (itemvalue != "")
                                                {
                                                    flag = LogicalComparison(itemvalue, provnice, "like");
                                                    if (flag)
                                                    {
                                                        if (city != "")
                                                            flag = LogicalComparison(itemvalue, city, "like");
                                                    }
                                                }
                                                else
                                                    flag = false;
                                            }
                                            else if (provnice == "" && city != "")
                                            {
                                                if (itemvalue != "")
                                                {
                                                    flag = LogicalComparison(itemvalue, city, "like");
                                                }
                                                else
                                                    flag = false;
                                            }
                                            else
                                                flag = false;
                                            break;
                                        case "registerDateTime":
                                            flag = LogicalComparisonDate(DateTime.Now.ToString("yyyy-MM-dd"), itemvalue, ctype);
                                            if (useritem != null && useritem.Count > 0)
                                            {
                                                foreach (KeyValuePair<string, string> useritemval in useritem)
                                                    userval = useritemval.Value;
                                                flag = LogicalComparisonDate(DateTime.Parse(userval).ToString("yyyy-MM-dd"), itemvalue, ctype);
                                            }
                                            break;
                                        default:

                                            if (useritem != null && useritem.Count > 0)
                                                foreach (KeyValuePair<string, string> useritemval in useritem)
                                                    userval = useritemval.Value;
                                            flag = LogicalComparison(userval, itemvalue, ctype);
                                            break;
                                    }


                                    #endregion

                                    if (!flag)
                                    {
                                        break;
                                    }
                                }
                                #endregion
                                //添加验证结果集
                                Resultlist.Add(Guid.NewGuid().ToString().Replace("-", ""), ruleLogicTemp + ":" + flag.ToString().ToLower());
                            }


                        }
                        catch (Exception exp)
                        {
                            bRet = false;
                            throw exp;
                        }

                    }
                    else
                        bRet = false;
                }
                else
                    bRet = false;
            }
            else
                bRet = false;

            //判断Resultlist二维数组结果是否通过
            if (Resultlist.Count > 0)
            {
                string andValueFalse = "and:false";
                string andValueTrue = "and:true";
                string orValue = "or:true";
                if (Resultlist.ContainsValue(orValue))
                {
                    //只要Or存在true 就通过
                    bRet = true;
                }
                else
                {
                    if (Resultlist.ContainsValue(andValueFalse))
                    {
                        //只要and存在flase就不通过
                        bRet = false;
                    }
                    else
                    {
                        if (Resultlist.ContainsValue(andValueTrue))
                        {
                            //and不存在flase，存在true 就通过
                            bRet = true;
                        }
                        else
                        {
                            //and不存在flase，不存在true，Or存在false 就不通过
                            bRet = false;
                        }
                    }
                }
            }
            else
                bRet = false;
            return bRet;
        }
        #endregion



        #region Method for LogicalComparison //逻辑判断
        /// <summary>
        /// 逻辑判断
        /// </summary>
        /// <param name="newItemValue"></param>
        /// <param name="oldItemValue"></param>
        /// <param name="itemOperator"></param>
        /// <returns></returns>
        public static bool LogicalComparison(string newItemValue, string oldItemValue, string itemOperator)
        {
            bool bResult = false;
            try
            {
                if (!string.IsNullOrEmpty(newItemValue) && !string.IsNullOrEmpty(oldItemValue) && !string.IsNullOrEmpty(itemOperator))
                {
                    switch (itemOperator)
                    {
                        case "like":
                            if (newItemValue.Contains(oldItemValue))
                                bResult = true;
                            break;
                        case "<>":
                            if (!newItemValue.Equals(oldItemValue))
                                bResult = true;
                            break;
                        case "!=":
                            if (oldItemValue.IndexOf(',') >= 0)
                            {
                                string[] olds = oldItemValue.Split(new char[] { ',' });
                                bool bRet = true;
                                foreach (string item in olds)
                                {
                                    if (!newItemValue.Contains(item))
                                        bRet = true;
                                    else
                                        bRet = false;
                                    if (!bRet)
                                    {
                                        bResult = false;
                                        break;
                                    }
                                    else
                                    {
                                        bResult = true;
                                    }
                                }
                            }
                            else if (!newItemValue.Equals(oldItemValue))
                                bResult = true;
                            break;
                        case "=":
                            if (oldItemValue.IndexOf(',') >= 0)
                            {
                                string[] olds = oldItemValue.Split(new char[] { ',' });
                                foreach (string item in olds)
                                {
                                    if (newItemValue.Contains(item))
                                    {
                                        bResult = true;
                                    }
                                }
                            }
                            else if (newItemValue.Equals(oldItemValue))
                                bResult = true;
                            break;
                        case "<=":
                            if (Convert.ToInt32(newItemValue) <= Convert.ToInt32(oldItemValue))
                                bResult = true;
                            break;
                        case "<":
                            if (Convert.ToInt32(newItemValue) < Convert.ToInt32(oldItemValue))
                                bResult = true;
                            break;
                        case ">=":
                            if (Convert.ToInt32(newItemValue) >= Convert.ToInt32(oldItemValue))
                                bResult = true;
                            break;
                        case ">":
                            if (Convert.ToInt32(newItemValue) > Convert.ToInt32(oldItemValue))
                                bResult = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (string.IsNullOrEmpty(oldItemValue))
            {
                bResult = true;
            }
            return bResult;
        }
        #endregion

        #region Method for LogicalComparison //逻辑判断
        /// <summary>
        /// 逻辑判断
        /// </summary>
        /// <param name="newItemValue"></param>
        /// <param name="oldItemValue"></param>
        /// <param name="itemOperator"></param>
        /// <returns></returns>
        public static bool LogicalComparisonDate(string newItemValue, string oldItemValue, string itemOperator)
        {
            bool bResult = false;
            try
            {
                if (!string.IsNullOrEmpty(newItemValue) && !string.IsNullOrEmpty(oldItemValue) && !string.IsNullOrEmpty(itemOperator))
                {
                    switch (itemOperator)
                    {
                        case "like":
                            if (newItemValue.Contains(oldItemValue))
                                bResult = true;
                            break;
                        case "<>":
                            if (!newItemValue.Equals(oldItemValue))
                                bResult = true;
                            break;
                        case "!=":
                            if (!newItemValue.Equals(oldItemValue))
                                bResult = true;
                            break;
                        case "=":
                            if (newItemValue.Equals(oldItemValue))
                                bResult = true;
                            break;
                        case "<=":
                            if (Convert.ToDateTime(newItemValue) <= Convert.ToDateTime(oldItemValue))
                                bResult = true;
                            break;
                        case "<":
                            if (Convert.ToDateTime(newItemValue) < Convert.ToDateTime(oldItemValue))
                                bResult = true;
                            break;
                        case ">=":
                            if (Convert.ToDateTime(newItemValue) >= Convert.ToDateTime(oldItemValue))
                                bResult = true;
                            break;
                        case ">":
                            if (Convert.ToDateTime(newItemValue) > Convert.ToDateTime(oldItemValue))
                                bResult = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bResult;
        }
        #endregion



        public static DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构  
            foreach (DataRow row in rows)
                tmp.Rows.Add(row.ItemArray);  // 将DataRow添加到DataTable中  
            return tmp;
        }



        #region 检测权限参数是否为空
        /// <summary>
        /// 检测权限参数是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckParamIsEmpty(params Object[] str)
        {
            foreach (string s in str)
            {
                if (string.IsNullOrEmpty(s))
                    return false;
            }
            return true;
        }
        #endregion

        #region 检测权限参数是否包含关键字
        /// <summary>
        /// 检测权限参数是否包含关键字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckParamKey(params Object[] str)
        {
            foreach (string s in str)
            {
                string pattern = @"select|insert|delete|from|count\(|drop table|update|truncate|asc\
                                (|mid\(|char\(|xp_cmdshell|exec   master|netlocalgroup administrators|:|net user|""|or|and|)";
                if (Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }
        #endregion

        #region 获取微信发放红包答复

        /// <summary>
        /// 获取微信发放红包答复1
        /// </summary>
        /// <param name="status"></param>
        /// <param name="openid"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string GetWxPayResult(string status, string openid, string money)
        {

            return "{" + string.Format("\"status\":\"{0}\"", status) + "," + string.Format("\"openid\":\"{0}\"", openid) + "," + string.Format("\"money\":\"{0}\"", money) + "}";

        }

        /// <summary>
        /// 获取微信发放红包答复2
        /// </summary>
        /// <param name="status"></param>
        /// <param name="openid"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string GetWxPayResult(string status)
        {
            return "{" + string.Format("\"Status\":\"{0}\"", status) + "}";
        }
        #endregion

        #region DES解密
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
        /// <returns></returns>
        public static string DESDecrypt(string data, string key, string iv)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }



        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <returns></returns>
        public static string DESDecrypt(string data)
        {
            string DefaultDecrypt = "";
            try
            {
                DefaultDecrypt = ConfigurationManager.AppSettings["DefaultDecrypt"].ToString();

                Logger.AppLog.Write("DefaultDecrypt:" + DefaultDecrypt, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(DefaultDecrypt);
                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(DefaultDecrypt);

                byte[] byEnc;
                try
                {
                    byEnc = Convert.FromBase64String(data);
                }
                catch
                {
                    return null;
                }

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DefaultDecrypt:" + DefaultDecrypt + " error:" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
            }

            return "";
        }
        #endregion


        /// <summary>
        /// 各种对象序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Serialize<T>(this T t)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                return jss.Serialize(t);
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="originalString">需加密字符</param>
        /// <param name="codeLength">需加密长度(16~32)</param>
        /// <returns></returns>
        public static String MD5(this String originalString)
        {
            MD5CryptoServiceProvider md5Pro = new MD5CryptoServiceProvider();
            byte[] theSrc = Encoding.UTF8.GetBytes(originalString);
            byte[] theResTypes = md5Pro.ComputeHash(theSrc);
            return Convert.ToBase64String(theResTypes);
        }
        #endregion

        #region 字典序排序
        /// <summary>
        /// 字典序排序
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ValueSort(params string[] val)
        {
            List<string> list = new List<string>();
            foreach (string str in val)
            {
                list.Add(str);
            }
            list.Sort();
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in list)
            {
                builder.Append(str2);
            }
            return builder.ToString();
        }
        #endregion


        public static void SetCookie(string key, string value, int expiresMonth)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Value = value;
            if (expiresMonth < 1) { expiresMonth = 1; }
            cookie.Expires = DateTime.Now.AddMonths(expiresMonth);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void SetCookie(string key, string value)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Value = value;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string getCookie(string key)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[key].Value.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void clearCookie(string key)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Value = string.Empty;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        #region 生成随机字符串
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <returns></returns>
        public static string randString(int length)
        {
            string str = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";//62个字符
            Random r = new Random();
            string result = string.Empty;

            //生成一个8位长的随机字符，具体长度可以自己更改
            for (int i = 0; i < length; i++)
            {
                int m = r.Next(0, 62);//这里下界是0，随机数可以取到，上界应该是62，因为随机数取不到上界，也就是最大62，符合我们的题意
                string s = str.Substring(m, 1);
                result += s;
            }

            return result;
        }


        #endregion




        public static bool Check_DigitCode(string strString)
        {
            string pattern = @"\d{16}";
            if (Regex.IsMatch(strString, pattern, RegexOptions.IgnoreCase))
                return true;
            else
                return false;

        }

        #region 获取枚举描述
        public static string GetEnumDescription(this Enum e)
        {
            FieldInfo enumInfo = e.GetType().GetField(e.ToString());
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])enumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
            {
                return EnumAttributes[0].Description;
            }
            return e.ToString();
        }
        #endregion


        #region HTTP请求
        public static string HttpGet(string postDataStr, string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion

        public static string HttpPost(string paramData, string postUrl)
        {
            string ret = string.Empty;
            try
            {
                Encoding dataEncode = dataEncode = Encoding.UTF8;
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

    }



    /// <summary>
    /// 流量包描述（流量包大小,金额）
    /// </summary>
    public static class PackCodeFlow
    {



        /// <summary>
        /// 移动流量包枚举
        /// </summary>
        public enum 移动
        {
            [Description("10,3")]
            Mb10 = 100010,
            [Description("30,5")]
            Mb30 = 100030,
            [Description("70,10")]
            Mb70 = 100070,
            [Description("100,10")]
            Mb100 = 100100,
            [Description("150,20")]
            Mb150 = 100150,
            [Description("300,20")]
            Mb300 = 100300,
            [Description("500,30")]
            Mb500 = 100500,
            [Description("1024,50")]
            Mb1024 = 101024,
            [Description("2048,70")]
            Mb2048 = 102048,
            [Description("3072,100")]
            Mb3072 = 103072,
            [Description("4096,130")]
            Mb4096 = 104096,
            [Description("6144,180")]
            Mb6144 = 106144,
            [Description("11264,280")]
            Mb11264 = 111264
        }

        /// <summary>
        /// 联通流量包枚举
        /// </summary>
        public enum 联通
        {
            [Description("20,3")]
            Mb20 = 100020,
            [Description("50,6")]
            Mb50 = 100050,
            [Description("100,10")]
            Mb100 = 100100,
            [Description("200,15")]
            Mb200 = 100200,
            [Description("500,30")]
            Mb500 = 100500,
            [Description("1024,100")]//是半年包，暂无
            Mb1024 = 101024
        }

        /// <summary>
        /// 全网流量包枚举
        /// </summary>
        public enum 电信
        {
            [Description("5,1")]
            Mb5 = 100005,
            [Description("10,2")]
            Mb10 = 100010,
            [Description("30,5")]
            Mb30 = 100030,
            [Description("50,7")]
            Mb50 = 100050,
            [Description("100,10")]
            Mb100 = 100100,
            [Description("200,15")]
            Mb200 = 100200,
            [Description("500,30")]
            Mb500 = 100500,
            [Description("1024,50")]
            Mb1024 = 101024
        }



    }
}
