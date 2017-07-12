
using System;
using System.Web;
using System.Web.Security;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Web.Script.Serialization;
using System.Linq;
using System.Reflection;

namespace KMSLotterySystemFront.Common
{
    /// <summary>
    /// 基础类处理
    /// </summary>
    public sealed class BaseFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFunction"/> class.
        /// </summary>
        public BaseFunction()
        {
        }

        #region MD5加密
        /// <summary>
        /// MD5加  update 2014-03-19 wumiao
        /// </summary>
        /// <param name="originalString">需加密字符</param>
        /// <param name="codeLength">需加密长度</param>
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

        #region 本方法用于获取客户端的IP地址
        /// <summary>
        /// 本方法用于获取客户端的IP地址
        /// </summary>
        /// <param name="httpContext">HttpContext上下文对象</param>
        /// <returns></returns>
        public static String GetClientIP(HttpContext httpContext)
        {
            NameValueCollection collection = httpContext.Request.ServerVariables;
            String ip = collection.Get("HTTP_X_FORWARDED_FOR");
            //取得通过代理服务器访问网络的客户的真实IP            
            if (ip == null)
                ip = collection.Get("REMOTE_ADDR");
            //如果不是通过代理服务器访问，取得其IP            
            return ip;
        }
        #endregion

        #region 获取客户端的IP地址
        /// <summary>
        /// 本方法用于获取客户端的IP地址
        /// </summary>
        /// <returns></returns>
        public static String GetClientIP()
        {
            NameValueCollection collection = HttpContext.Current.Request.ServerVariables;

            String ip = collection.Get("HTTP_X_FORWARDED_FOR");
            //取得通过代理服务器访问网络的客户的真实IP            
            if (ip == null)
                ip = collection.Get("REMOTE_ADDR");
            //如果不是通过代理服务器访问，取得其IP            
            return ip;
        }
        #endregion

        #region 本方法用于重设密码
        /// <summary>
        /// 本方法用于重设密码
        /// </summary>
        /// <param name="length">重设密码的长度</param>
        /// <returns></returns>
        public static String ResetPassword(Int32 length)
        {
            return CreateRandomCode(length);
        }
        #endregion

        #region 本方法用于产生随机验证码
        /// <summary>
        /// 本方法用于产生随机验证码
        /// </summary>
        /// <param name="codelength">随机码长度</param>
        /// <returns></returns>
        public static String CreateRandomCode(Int32 codelength)
        {
            String allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
            //string allChar = "0,1,2,3,4,5,6,7,8,9" ;
            String[] allCharArray = allChar.Split(',');
            String randomCode = "";
            Int32 temp = -1;

            Random rand = new Random();
            for (Int32 i = 0; i < codelength; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                Int32 t = rand.Next(35);
                //int t = rand.Next(10);
                if (temp == t)
                {
                    return CreateRandomCode(codelength);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
        #endregion

        #region  全码换成半码
        /// <summary>
        /// 全码换成半码
        /// </summary>
        /// <param name="originalCode">数码</param>
        /// <returns></returns>
        public static String ReplaceQuanjiao(String originalCode)
        {
            originalCode = originalCode.Replace("０", "0");
            originalCode = originalCode.Replace("１", "1");
            originalCode = originalCode.Replace("２", "2");
            originalCode = originalCode.Replace("３", "3");
            originalCode = originalCode.Replace("４", "4");
            originalCode = originalCode.Replace("５", "5");
            originalCode = originalCode.Replace("６", "6");
            originalCode = originalCode.Replace("７", "7");
            originalCode = originalCode.Replace("８", "8");
            originalCode = originalCode.Replace("９", "9");
            return originalCode;
        }
        #endregion

        #region  Null字符替换掉
        /// <summary>
        /// Null字符替换掉
        /// </summary>
        /// <param name="stringCode"></param>
        /// <returns></returns>
        public static String ReplaceNullString(String stringCode)
        {
            return stringCode.Replace("Null", "");
        }
        #endregion

        #region  检查字符串是否为空 返回ture 为不为空 返回flase则为空
        /// <summary>
        /// 检查字符串是否为空  返回ture 为不为空 返回flase则为空
        /// </summary>
        /// <param name="stringCode"></param>
        /// <returns>返回ture 为不为空 返回flase则为空</returns>
        public static Boolean StringNotEmpty(String stringCode)
        {
            if ((stringCode == null) || (stringCode.Trim() == "") || (stringCode.Trim() == "Null") || (stringCode.Trim() == "null") || (stringCode.Trim() == string.Empty))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Base64加密解密
        public static string Base64ToString(string data)
        {
            try
            {
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
            }
            catch
            {
                return "";
            }
        }
        public static string StringToBase64(string sourceString)
        {
            try
            {
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sourceString)).Replace("+", "%2B");
            }
            catch
            {
                return "";
            }
        }
        #endregion

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

        #region Method for ExportXLS //导出XLS格式的文件
        /// <summary>
        /// 导出XLS格式的文件
        /// </summary>
        /// <param name="response">输出请求</param>
        /// <param name="strFileName">输入的文件名</param>
        /// <param name="ditCaption">标题列表</param>
        /// <history>2011.09.09 zhoujie created</history>
        public static bool Export_XLS(System.Web.HttpResponse response, string strDefaultName, DataTable dt, Dictionary<string, string> ditCaption, out string strState)
        {
            bool bflag = false;
            const string EXTEND_CSV = ".xls";
            try
            {
                if (null == dt || dt.Rows.Count < 1)
                {
                    strState = "数据集合为空";
                    return bflag;
                }
                if (null == strDefaultName || strDefaultName.Trim().Length < 1)
                {
                    strState = "导出的默认文件名不可为空！";
                    return bflag;
                }
                string sw = Export_Table(dt, ditCaption);

                response.Buffer = true;
                response.AppendHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(strDefaultName + EXTEND_CSV, System.Text.Encoding.UTF8));
                response.ContentType = "application/ms-excel";
                response.HeaderEncoding = System.Text.Encoding.UTF8;
                response.ContentEncoding = System.Text.Encoding.UTF8;
                response.Write(sw);
                response.Flush();
                //response.End();
                strState = "文件导出成功！";
            }
            catch (Exception ex)
            {
                strState = "文件导出发生异常:" + ex.Message;

            }
            return bflag;
        }

        public static string Export_Table(DataTable dtResult, Dictionary<string, string> ditCaption)
        {
            StringBuilder sbData = new StringBuilder();
            string cellStyle = string.Empty;
            //foreach (DataTable tb in ds.Tables)
            {
                sbData.Append("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\">");
                //data += tb.TableName + "\n";
                sbData.Append("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
                //combine caption
                sbData.Append("<tr style=\"font-weight: bold; white-space: nowrap;\">");
                foreach (KeyValuePair<string, string> item in ditCaption)
                {
                    sbData.Append("<td style=\"background-color:Silver;\">" + GB2312ToUTF8(item.Value) + "</td>");
                }
                sbData.Append("</tr>");

                //combine data
                foreach (DataRow row in dtResult.Rows)
                {
                    sbData.Append("<tr>");
                    cellStyle = string.Empty;

                    foreach (KeyValuePair<string, string> item in ditCaption)
                    {
                        sbData.Append("<td style=\"vnd.ms-excel.numberformat:@;" + cellStyle + "\">" + GB2312ToUTF8(row[item.Key].ToString()) + "</td>");
                    }
                    sbData.Append("</tr>");
                }
                sbData.Append("</table>");
            }
            sbData.Append("</body></html>");
            return sbData.ToString();
        }

        public static string GB2312ToUTF8(string str)
        {
            try
            {
                Encoding uft8 = Encoding.GetEncoding(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] temp = gb2312.GetBytes(str);
                byte[] temp1 = Encoding.Convert(gb2312, uft8, temp);
                string result = uft8.GetString(temp1);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static string UTF8ToGB2312(string str)
        {
            try
            {
                Encoding utf8 = Encoding.GetEncoding(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");//Encoding.Default ,936   
                byte[] temp = utf8.GetBytes(str);
                byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
                string result = gb2312.GetString(temp1);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion

        #region MessageBox
        public static void MessageShow(System.Web.UI.Page page, string strMsg)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", string.Format("<script language=\"javascript\" defer>alert('{0}');</script>", strMsg).ToString());
        }
        #endregion

        /// <summary> 
        /// 反回JSON数据到前台 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>JSON字符串</returns> 
        public static string DataTable2Json(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jsonBuilder.Append("{");

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        jsonBuilder.Append("\"");

                        jsonBuilder.Append(dt.Columns[j].ColumnName);

                        jsonBuilder.Append("\":\"");

                        jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\""));

                        jsonBuilder.Append("\",");

                    }

                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);

                    jsonBuilder.Append("},");

                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }



        /// <summary> 
        /// 反回JSON数据到前台 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>JSON字符串</returns> 
        public static string DataTable2Json(DataTable dt, string name)
        {
            StringBuilder jsonBuilder = new StringBuilder();



            jsonBuilder.Append("\"" + name + "\":[");
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jsonBuilder.Append("{");

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        jsonBuilder.Append("\"");

                        jsonBuilder.Append(dt.Columns[j].ColumnName);

                        jsonBuilder.Append("\":\"");

                        jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\""));

                        jsonBuilder.Append("\",");

                    }

                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);

                    jsonBuilder.Append("},");

                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }



            jsonBuilder.Append("]");

            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            string strJson = ToJson(obj, null);
            string p = @"\\/Date\((\d+)\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            strJson = reg.Replace(strJson, matchEvaluator);
            return strJson;
        }

        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            //DateTime dt = new DateTime(1970, 1, 1);
            //dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            //dt = dt.ToLocalTime();
            //result = dt.ToString(FormateStr);
            result = m.Groups[1].Value;
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonConverters"></param>
        /// <returns></returns>
        public static string ToJson(object obj, IEnumerable<JavaScriptConverter> jsonConverters)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (jsonConverters != null) serializer.RegisterConverters(jsonConverters ?? new JavaScriptConverter[0]);
            return serializer.Serialize(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static TEntity ToObject<TEntity>(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<TEntity>(json);
        }


        ///获取一个类指定的属性值
        /// object对象
        /// field 属性名称
        public static object GetPropertyValue(object info, string field)
        {
            if (info == null) return null;
            Type t = info.GetType();
            IEnumerable property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (p.Name.ToLower().Equals(field.ToLower()))
                {
                    return p.GetValue(info, null);
                }
            }
            return "";
        }

        ///
        /// dataset转json
        /// 
        public static string GetJsonByDataset(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            {
                //如果查询到的数据为空则返回标记ok:false
                return "{\"ok\":false}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,");
            foreach (DataTable dt in ds.Tables)
            {
                sb.Append(string.Format("\"{0}\":[", dt.TableName));

                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("{");
                    for (int i = 0; i < dr.Table.Columns.Count; i++)
                    {
                        sb.AppendFormat("\"{0}\":\"{1}\",", dr.Table.Columns[i].ColumnName.Replace("\"", "\\\"").Replace("\'", "\\\'"), ObjToStr(dr[i]).Replace("\"", "\\\"").Replace("\'", "\\\'")).Replace(Convert.ToString((char)13), "\\r\\n").Replace(Convert.ToString((char)10), "\\r\\n");
                    }
                    sb.Remove(sb.ToString().LastIndexOf(','), 1);
                    sb.Append("},");
                }

                sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append("],");
            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// 将object转换成为string
        /// </summary>
        /// <param name="ob">obj对象</param>
        /// <returns></returns>
        public static string ObjToStr(object ob)
        {
            if (ob == null)
            {
                return string.Empty;
            }
            else
                return ob.ToString();
        }
    }
}
