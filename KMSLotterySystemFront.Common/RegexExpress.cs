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
// *创建标识：正则表达式验证类
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace KMSLotterySystemFront.Common
{
    /// <summary>
    /// 验证表达式 验证
    /// </summary>
    public sealed class RegexExpress
    {
        /// <summary>
        /// 手机类型
        /// </summary>
        public enum MobileType
        {
            /// <summary>
            /// 移动
            /// </summary>
            YiDong,
            /// <summary>
            /// 联通
            /// </summary>
            LianTong,
            /// <summary>
            /// 电信
            /// </summary>
            DianXin
        }


        public enum LotteryType
        {
            //类别（1:话费、2：红包、3：流量、4：虚拟券、5：实物礼品、6：旅游）

            /// <summary>
            /// 话费
            /// </summary>
            [Description("1")]
            lotteryHH = 1,

            /// <summary>
            /// 红包
            /// </summary>
            [Description("2")]
            lotteryHB = 2,

            /// <summary>
            /// 流量
            /// </summary>
            [Description("3")]
            lotteryLL = 3,

            /// <summary>
            /// 虚拟券
            /// </summary>
            [Description("4")]
            lotteryXNQ = 4,

            /// <summary>
            /// 实物礼品
            /// </summary>
            [Description("5")]
            lotterySW = 5,

            /// <summary>
            /// 旅游
            /// </summary>
            [Description("6")]
            lotteryLY = 6,

        }



        public const String REGEXP_IS_YiDong_Mobile = @"^((((13[5-9]{1})|(15[0,1,2,7,8,9]{1})|(188)){1}\d{1})|((134[0-8]{1}){1})){1}\d{7}$";
        //| 130 | 131 | 132 | 155 | 156 | 186
        public const String REGEXP_IS_LianTong_Mobile = @"^((13[0-2]{1})|(15[5,6,8]{1})|186)\d{8}$";
        /// <summary>
        /// 电子邮件校验常量 
        /// </summary>
        public const String REGEXP_IS_VALID_EMAIL = @"^\w+((-\w+)|(\.\w+))*\@\w+((\.|-)\w+)*\.\w+$";
        /// <summary>
        /// 网址校验常量
        /// </summary>
        public const String REGEXP_IS_VALID_URL = @"^http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        /// <summary>
        /// 邮编校验常量 
        /// </summary>
        public const String REGEXP_IS_VALID_ZIP = @"\d{6}";
        /// <summary>
        /// 身份证校验常量 
        /// </summary>
        public const String REGEXP_IS_VALID_SSN = @"\d{18}|\d{15}";
        /// <summary>
        /// 整数校验常量 
        /// </summary>
        public const String REGEXP_IS_VALID_INT = @"^\d{1,}$";
        /// <summary>
        /// 数值校验常量
        /// </summary>
        public const String REGEXP_IS_VALID_DEMICAL = @"^-?(0|\d+)(\.\d+)?$";
        //使用正则表达式 (判断YYYY-MM-DD或YYYY-M-D格式)
        // public const String REGEXP_IS_VALID_DEMICAL = @"^((((1[6-9]|[2-9]"d)"d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]"d|3[01]))|(((1[6-9]|[2-9]"d)"d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]"d|30))|(((1[6-9]|[2-9]"d)"d{2})-0?2-(0?[1-9]|1"d|2[0-8]))|(((1[6-9]|[2-9]"d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";

        /// <summary>
        /// 日期校验常量
        /// </summary>
        public const String REGEXP_IS_VALID_DATE = @"^(?:(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\/|-|\.)(?:0?2\1(?:29))$)|(?:(?:1[6-9]|[2-9]\d)?\d{2})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8]))$";

        /// <summary>
        /// 严格的日期验证包括格式和逻辑上的合法性
        /// </summary>
        public const String REGEXP_IS_VALID_FULLDATE = @"^((((19|20)(([02468][048])|([13579][26]))-02-29))|((20[0-9][0-9])|(19[0-9][0-9]))-((((0[1-9])|(1[0-2]))-((0[1-9])|(1\d)|(2[0-8])))|((((0[13578])|(1[02]))-31)|(((01,3-9])|(1[0-2]))-(29|30)))))$Matches: [2002-01-31], [1997-04-30], [2004-01-01] [ More Details]  
No-Matches: [2002-01-32], [2003-02-29], [04-01-01]";

        /// <summary>
        /// 判断YYYY-MM-DD这种格式
        /// </summary>
        public const String REGEXP_IS_VALID_DATEYYYYMMDD = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";

        /// <summary>
        /// 加了时间验证的
        /// </summary>
        public const String REGEXP_IS_VALID_DATETIME = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";

        /// <summary>
        /// 验证只能输入汉字、字母(半角)和数字(半角)
        /// </summary>
        private const String REGEXP_CHAR_IS_BANJIAO = @"^[A-Za-z\u4e00-\u9fa5][\w\u4e00-\u9fa5]*$";

        /// <summary>
        /// 时间格式验证：20100919135325
        /// </summary>
        private const String REGEXP_TIME_NUMBERFORMAT = @"^\d{14}$";

        /// <summary>
        /// 时间格式验证：2010-09-19 13:53:25
        /// </summary>
        private const String REGEXP_TIME_STRINGFORMAT = @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$";



        #region 验证邮箱
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsEmail(String source)
        {
            //return Regex.IsMatch(source, @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase);
            return Regex.IsMatch(source, @"^[A-Za-z0-9](([_\.\-]{0,}[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// HasEmail
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean HasEmail(String source)
        {
            return Regex.IsMatch(source, @"[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证网址
        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="source">WebURL</param>
        /// <returns></returns>
        public static Boolean IsUrl(String source)
        {
            return Regex.IsMatch(source, @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static Boolean HasUrl(String source)
        {
            return Regex.IsMatch(source, @"(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证日期
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="source">验证日期</param>
        /// <returns></returns>
        public static Boolean IsDateTime(String source)
        {
            try
            {
                DateTime time = Convert.ToDateTime(source);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 验证IP地址
        public static Boolean IsValidIP(string strIP)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(strIP, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                string[] ip_ = strIP.Split('.');
                if (ip_.Length == 4 || ip_.Length == 6)
                {
                    if (System.Int32.Parse(ip_[0]) < 256 && System.Int32.Parse(ip_[1]) < 256 & System.Int32.Parse(ip_[2]) < 256 & System.Int32.Parse(ip_[3]) < 256) return true;
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        #endregion

        #region 验证手机号
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="source">验证手机号</param>
        /// <returns></returns>
        public static Boolean IsMobile(String source)
        {
            return Regex.IsMatch(source, @"^(1[34578][0-9]|147|176|177|178)\d{8}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="source">验证手机号</param>
        /// <returns></returns>
        public static Boolean IsMobileNew(String source)
        {
            return Regex.IsMatch(source, @"^(1[34578][0-9]|147|176|177|178)\d{8}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="source">手机号</param>
        /// <returns></returns>
        public static Boolean HasMobile(String source)
        {
            return Regex.IsMatch(source, @"1[358]\d{9}", RegexOptions.IgnoreCase);
        }

        public static Boolean IsMobile_Send(String source)
        {
            return Regex.IsMatch(source, @"^(1[3578][0-9]|147)\d{8}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证IP
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="source">IP</param>
        /// <returns></returns>
        public static Boolean IsIP(String source)
        {
            return Regex.IsMatch(source, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="source">IP</param>
        /// <returns></returns>
        public static Boolean HasIP(String source)
        {
            return Regex.IsMatch(source, @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证身份证是否有效
        /// <summary>
        /// 验证身份证是否有效
        /// </summary>
        /// <param name="Id">身份证</param>
        /// <returns></returns>
        public static Boolean IsIDCard(String Id)
        {
            if (Id.Length == 18)
            {
                Boolean check = IsIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                Boolean check = IsIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// IDCard18
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Boolean IsIDCard18(String Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            String address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            String birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            String[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            String[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            Int32 sum = 0;
            for (Int32 i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            Int32 y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// IDCard15
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Boolean IsIDCard15(String Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            String address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            String birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion

        #region 是不是Int型的
        /// <summary>
        /// 是不是Int型的
        /// </summary>
        /// <param name="source">数字</param>
        /// <returns></returns>
        public static Boolean IsInt(String source)
        {
            Regex regex = new Regex(@"^(-){0,1}\d+$");
            if (regex.Match(source).Success)
            {
                if ((long.Parse(source) > 0x7fffffffL) || (long.Parse(source) < -2147483648L))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region 是不是数值型
        /// <summary>
        /// 是不是数值型
        /// </summary>
        /// <param name="source">数值</param>
        /// <returns></returns>
        public static Boolean IsDecimal(String source)
        {
            Regex regex = new Regex(@"^\d{0,8}.\d{0,2}$");
            if (regex.Match(source).Success)
            {
                if ((double.Parse(source) > 0x7fffffffL) || (double.Parse(source) < -2147483648L))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region 看字符串的长度是不是在限定数之间 一个中文为两个字符
        /// <summary>
        /// 看字符串的长度是不是在限定数之间 一个中文为两个字符
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="begin">大于等于</param>
        /// <param name="end">小于等于</param>
        /// <returns></returns>
        public static Boolean IsLengthStr(String source, Int32 begin, Int32 end)
        {
            Int32 length = Regex.Replace(source, @"[^\x00-\xff]", "OK").Length;
            if ((length <= begin) && (length >= end))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 是不是中国电话，格式010-85849685
        /// <summary>
        /// 是不是中国电话，格式010-85849685
        /// </summary>
        /// <param name="source">格式010-85849685</param>
        /// <returns></returns>
        public static Boolean IsTel(String source)
        {
            return Regex.IsMatch(source, @"^\d{3,4}-?\d{6,8}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 邮政编码 6个数字
        /// <summary>
        /// 邮政编码 6个数字
        /// </summary>
        /// <param name="source">邮政编码</param>
        /// <returns></returns>
        public static Boolean IsPostCode(String source)
        {
            return Regex.IsMatch(source, @"^\d{6}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 是不是中文
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
        /// 是不是中文
        /// </summary>
        /// <param name="source">待检测字符</param>
        /// <returns></returns>
        public static Boolean hasChinese(String source)
        {
            return Regex.IsMatch(source, @"[\u4e00-\u9fa5]+", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证是不是正常字符 字母，数字，下划线的组合
        /// <summary>
        /// 验证是不是正常字符 字母，数字，下划线的组合
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static Boolean IsNormalChar(String source)
        {
            return Regex.IsMatch(source, @"[\w\d_]+", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证是不是16位防伪数码
        /// <summary>
        /// 验证是不是16位防伪数码
        /// </summary>
        /// <param name="source">防伪数码</param>
        /// <returns></returns>
        public static Boolean IsDigitCode16(String source)
        {
            if ((Regex.IsMatch(source, @"^\d{16}$", RegexOptions.IgnoreCase)) && (16 == source.Length))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region 验证是不是13位69码
        /// <summary>
        /// 验证是不是13位69码
        /// </summary>
        /// <param name="source">13位69码</param>
        /// <returns></returns>
        public static Boolean IsDigitCode13(String source)
        {
            if ((Regex.IsMatch(source, @"^\d{13}$", RegexOptions.IgnoreCase)) && (13 == source.Length))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region 验证是不是13位69码
        /// <summary>
        /// 验证是不是http开头的二维码
        /// </summary>
        /// <param name="source">http开头的二维码</param>
        /// <returns></returns>
        public static Boolean IsQRCode(String source)
        {
            if (source.StartsWith("http://"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 验证是不是数码
        /// <summary>
        /// 验证是不是数码
        /// </summary>
        /// <param name="source">数码</param>
        /// <returns></returns>
        public static Boolean IsDigitCode(String source)
        {
            Regex r = new Regex("^\\d"); // 定义一个Regex对象实例
            Match m = r.Match(source); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion


        /////////////////////////////////////////
        #region  验证电子邮件
        /// <summary>
        /// 验证电子邮件
        /// </summary>
        /// <param name="fieldName">电子邮件</param>
        /// <returns></returns>
        public static Boolean IsValidEMail(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_EMAIL); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证网址
        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="fieldName">网址</param>
        /// <returns></returns>
        public static Boolean IsValidURL(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_URL); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证邮编
        /// <summary>
        /// 验证邮编
        /// </summary>
        /// <param name="fieldName">邮编</param>
        /// <returns></returns>
        public static Boolean IsValidZip(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_ZIP); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证身份证
        /// <summary>
        /// 验证身份证
        /// </summary>
        /// <param name="fieldName">身份证</param>
        /// <returns></returns>
        public static Boolean IsValidSSN(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_SSN); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证整数
        /// <summary>
        /// 验证整数
        /// </summary>
        /// <param name="fieldName">整数</param>
        /// <returns></returns>
        public static Boolean IsValidINT(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_INT); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证数值
        /// <summary>
        /// 验证数值
        /// </summary>
        /// <param name="fieldName">fieldName</param>
        /// <returns></returns>
        public static Boolean IsValidDEMICAL(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_DEMICAL); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证日期
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="fieldName">日期</param>
        /// <returns></returns>
        public static Boolean IsValidDATE(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_DATE); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  严格的日期验证包括格式和逻辑上的合法性
        /// <summary>
        /// 严格的日期验证包括格式和逻辑上的合法性
        /// </summary>
        /// <param name="fieldName">日期验证</param>
        /// <returns></returns>
        public static Boolean IsValidFULLDATE(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_FULLDATE); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 验证日期 YYYY-MM-DD这种格式
        /// <summary>
        /// 验证日期 YYYY-MM-DD这种格式
        /// </summary>
        /// <param name="fieldName">YYYY-MM-DD</param>
        /// <returns></returns>
        public static Boolean IsValidDATEYYYYMMDD(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_DATEYYYYMMDD); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  验证时间
        /// <summary>
        /// 验证时间
        /// </summary>
        /// <param name="fieldName">时间</param>
        /// <returns></returns>
        public static Boolean IsValidDATETIME(String fieldName)
        {
            Regex r = new Regex(REGEXP_IS_VALID_DATETIME); // 定义一个Regex对象实例
            Match m = r.Match(fieldName); // 在字符串中匹配
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 检测手机类型 yhr20100305
        /// <summary>
        /// 检测手机类型 yhr20100305
        /// </summary>
        /// <param name="mobile">手机</param>
        /// <returns></returns>
        public static MobileType CheckMobileType(string mobile)
        {
            Regex r = new Regex(REGEXP_IS_YiDong_Mobile);
            Match m = r.Match(mobile);
            if (m.Success)
            {
                return MobileType.YiDong;
            }
            else
            {
                r = new Regex(REGEXP_IS_LianTong_Mobile);
                m = r.Match(mobile);
                if (m.Success)
                {
                    return MobileType.LianTong;
                }
            }
            return MobileType.DianXin;
        }
        #endregion

        #region 检测参数是否为半角字符
        /// <summary>
        /// 检测参数是否为半角字符
        /// </summary>
        /// <param name="src">字符串参数</param>
        /// <returns></returns>
        public static Boolean CharIsBanJiao(string src)
        {
            Regex r = new Regex(REGEXP_CHAR_IS_BANJIAO);
            Match m = r.Match(src);
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 检测检测时间格式是否为20100919135625
        /// <summary>
        /// 检测检测时间格式是否为20100919135625
        /// </summary>
        /// <param name="src">字符串参数</param>
        /// <returns></returns>
        public static Boolean CheckTimeFormatIsNumber(string src)
        {
            Regex r = new Regex(REGEXP_TIME_NUMBERFORMAT);
            Match m = r.Match(src);
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 检测检测时间格式是否为2010-09-19 13:56:25
        /// <summary>
        /// 检测检测时间格式是否为2010-09-19 13:56:25
        /// </summary>
        /// <param name="src">字符串参数</param>
        /// <returns></returns>
        public static Boolean CheckTimeFormatIsString(string src)
        {
            Regex r = new Regex(REGEXP_TIME_STRINGFORMAT);
            Match m = r.Match(src);
            if (m.Success)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 验证是不是正确有效的经纬度

        /// <summary>
        /// 验证是不是正确有效的经纬度
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsGPS(string source)
        {
            bool bRet = false;
            try
            {
                if (!string.IsNullOrEmpty(source))
                {
                    if (source.Contains("|"))
                    {
                        string[] gpslist = source.Split('|');

                        if (IsLongitude(gpslist[0]) && IsLatitude(gpslist[1]))
                        {
                            bRet = true;
                        }
                    }
                }
            }
            catch
            {
                return bRet;
            }
            return bRet;
        }



        /// <summary>  
        /// 验证经度  
        /// </summary>  
        /// <param name="input">待验证的字符串</param>  
        /// <returns>是否匹配</returns>  
        public static bool IsLongitude(string input)
        {
            ////范围为-180～180，小数位数必须是1到5位  
            //string pattern = @"^[-\+]?((1[0-7]\d{1}|0?\d{1,2})\.\d{1,5}|180\.0{1,5})$";
            //return IsMatch(input, pattern);
            float lon;
            if (float.TryParse(input, out lon) && lon >= -180 && lon <= 180)
                return true;
            else
                return false;
        }

        /// <summary>  
        /// 验证纬度  
        /// </summary>  
        /// <param name="input">待验证的字符串</param>  
        /// <returns>是否匹配</returns>  
        public static bool IsLatitude(string input)
        {
            ////范围为-90～90，小数位数必须是1到5位  
            //string pattern = @"^[-\+]?([0-8]?\d{1}\.\d{1,5}|90\.0{1,5})$";  
            //return IsMatch(input, pattern);  
            float lat;
            if (float.TryParse(input, out lat) && lat >= -90 && lat <= 90)
                return true;
            else
                return false;
        }


        #endregion

        #region 检验是否是微信粉丝号
        /// <summary>
        /// 检验是否是微信粉丝号 （目前微信号长度为28位）
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static bool CheckIsOpenid(string openid)
        {
            bool bRet = true;

            if (string.IsNullOrEmpty(openid))
            {
                bRet = false;
                return bRet;
            }
            if (openid.Length > 28)
            {
                bRet = false;
                return bRet;
            }
            return bRet;
        }


        #endregion

        #region 手机号码运营商判断
        /// <summary>
        /// 检测手机类型 yhr20100305
        /// </summary>
        /// <param name="mobile">手机</param>
        /// <returns></returns>
        public static MobileType CheckNewMobileType(string mobile)
        {

            if (!string.IsNullOrEmpty(mobile) && mobile.Length == 11)
            {
                mobile = mobile.Substring(0, 3);

                if ("134、135、136、137、138、139、147、150、151、152、157、158、159、182、183、184、187、188".Contains(mobile))
                {
                    return MobileType.YiDong;
                }
                else if ("130、131、132、145、155、156、185、186".Contains(mobile))
                {
                    return MobileType.LianTong;
                }
                else if ("133、153、180、181、189".Contains(mobile))
                {
                    return MobileType.DianXin;
                }
                else
                {
                    return MobileType.DianXin;
                }
            }
            return MobileType.DianXin;
        }
        #endregion



    }
}
