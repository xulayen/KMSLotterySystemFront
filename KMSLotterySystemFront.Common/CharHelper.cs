// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Common
// *文件名称：CharHelper.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：数据转换
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Common
{
    public class CharHelper
    {
        private List<char> listQuanJiao = new List<char>();
        private List<char> listBanJiao = new List<char>();

        #region 0)Constructor
        public CharHelper()
        {
            listQuanJiao.Add('０');
            listQuanJiao.Add('１');
            listQuanJiao.Add('２');
            listQuanJiao.Add('３');
            listQuanJiao.Add('４');
            listQuanJiao.Add('５');
            listQuanJiao.Add('６');
            listQuanJiao.Add('７');
            listQuanJiao.Add('８');
            listQuanJiao.Add('９');
            listQuanJiao.Add('Ａ');
            listQuanJiao.Add('Ｂ');
            listQuanJiao.Add('Ｃ');
            listQuanJiao.Add('Ｄ');
            listQuanJiao.Add('Ｅ');
            listQuanJiao.Add('Ｆ');
            listQuanJiao.Add('Ｇ');
            listQuanJiao.Add('Ｈ');
            listQuanJiao.Add('Ｉ');
            listQuanJiao.Add('Ｊ');
            listQuanJiao.Add('Ｋ');
            listQuanJiao.Add('Ｌ');
            listQuanJiao.Add('Ｍ');
            listQuanJiao.Add('Ｎ');
            listQuanJiao.Add('Ｏ');
            listQuanJiao.Add('Ｐ');
            listQuanJiao.Add('Ｑ');
            listQuanJiao.Add('Ｒ');
            listQuanJiao.Add('Ｓ');
            listQuanJiao.Add('Ｔ');
            listQuanJiao.Add('Ｕ');
            listQuanJiao.Add('Ｖ');
            listQuanJiao.Add('Ｗ');
            listQuanJiao.Add('Ｘ');
            listQuanJiao.Add('Ｙ');
            listQuanJiao.Add('Ｚ');
            listQuanJiao.Add('ａ');
            listQuanJiao.Add('ｂ');
            listQuanJiao.Add('ｃ');
            listQuanJiao.Add('ｄ');
            listQuanJiao.Add('ｅ');
            listQuanJiao.Add('ｆ');
            listQuanJiao.Add('ｇ');
            listQuanJiao.Add('ｈ');
            listQuanJiao.Add('ｉ');
            listQuanJiao.Add('ｊ');
            listQuanJiao.Add('ｋ');
            listQuanJiao.Add('ｌ');
            listQuanJiao.Add('ｍ');
            listQuanJiao.Add('ｎ');
            listQuanJiao.Add('ｏ');
            listQuanJiao.Add('ｐ');
            listQuanJiao.Add('ｑ');
            listQuanJiao.Add('ｒ');
            listQuanJiao.Add('ｓ');
            listQuanJiao.Add('ｔ');
            listQuanJiao.Add('ｕ');
            listQuanJiao.Add('ｖ');
            listQuanJiao.Add('ｗ');
            listQuanJiao.Add('ｘ');
            listQuanJiao.Add('ｙ');
            listQuanJiao.Add('ｚ');
            ///////////////////////////////////

            listBanJiao.Add('0');
            listBanJiao.Add('1');
            listBanJiao.Add('2');
            listBanJiao.Add('3');
            listBanJiao.Add('4');
            listBanJiao.Add('5');
            listBanJiao.Add('6');
            listBanJiao.Add('7');
            listBanJiao.Add('8');
            listBanJiao.Add('9');

            listBanJiao.Add('A');
            listBanJiao.Add('B');
            listBanJiao.Add('C');
            listBanJiao.Add('D');
            listBanJiao.Add('E');
            listBanJiao.Add('F');
            listBanJiao.Add('G');
            listBanJiao.Add('H');
            listBanJiao.Add('I');
            listBanJiao.Add('J');
            listBanJiao.Add('K');
            listBanJiao.Add('L');
            listBanJiao.Add('M');
            listBanJiao.Add('N');
            listBanJiao.Add('O');
            listBanJiao.Add('P');
            listBanJiao.Add('Q');
            listBanJiao.Add('R');
            listBanJiao.Add('S');
            listBanJiao.Add('T');
            listBanJiao.Add('U');
            listBanJiao.Add('V');
            listBanJiao.Add('W');
            listBanJiao.Add('X');
            listBanJiao.Add('Y');
            listBanJiao.Add('Z');
            listBanJiao.Add('a');
            listBanJiao.Add('b');
            listBanJiao.Add('c');
            listBanJiao.Add('d');
            listBanJiao.Add('e');
            listBanJiao.Add('f');
            listBanJiao.Add('g');
            listBanJiao.Add('h');
            listBanJiao.Add('i');
            listBanJiao.Add('j');
            listBanJiao.Add('k');
            listBanJiao.Add('l');
            listBanJiao.Add('m');
            listBanJiao.Add('n');
            listBanJiao.Add('o');
            listBanJiao.Add('p');
            listBanJiao.Add('q');
            listBanJiao.Add('r');
            listBanJiao.Add('s');
            listBanJiao.Add('t');
            listBanJiao.Add('u');
            listBanJiao.Add('v');
            listBanJiao.Add('w');
            listBanJiao.Add('x');
            listBanJiao.Add('y');
            listBanJiao.Add('z');
        }
        #endregion

        #region 1)全角转半角
        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="src">转换字符</param>
        /// <returns></returns>
        public string ReplaceQuanJiaoToBanJiao(String src)
        {
            //将全角的字符编号替换为半角的
            CharEnumerator charEnum = src.GetEnumerator();
            StringBuilder strDest = new StringBuilder();
            while (charEnum.MoveNext())
            {
                char c = charEnum.Current;
                if (listQuanJiao.Contains(c))
                {
                    int index = 0;
                    foreach (char cQ in listQuanJiao)
                    {
                        if (cQ == c)
                        {
                            break;
                        }
                        index++;
                    }
                    char cNew = listBanJiao[index];
                    strDest.Append(cNew);
                }
                else
                {
                    strDest.Append(c);
                }
            }
            return strDest.ToString();
        }
        #endregion

        #region 2)特殊字符过滤
        /// <summary>
        /// 特殊字符过滤(\0,，)
        /// </summary>
        /// <param name="src">字符串</param>
        /// <returns></returns>
        public string FilterSpecialChar(string src)
        {
            string strRet = "";
            strRet = src.Replace("\0", "");//yhr 2009-01-12
            strRet = strRet.Replace("，", ",").Trim();
            //strRet = strRet.Replace(" ", "").Trim();

            //del by guojun at 20111101 据张工说第一列可以为空，因此下面操作无意义。
            //string[] strArr = strRet.Split(',');

            //StringBuilder strContent = new StringBuilder();
            //foreach (string str in strArr)
            //{
            //    if (strContent.Length == 0)
            //    {
            //        strContent.Append(str.Trim());
            //    }
            //    else
            //    {
            //        strContent.Append(","+str.Trim());
            //    }
            //}
            return strRet.ToString();
        }
        #endregion

        #region 3)空字符过滤
        /// <summary>
        /// 空字符过滤
        /// </summary>
        /// <param name="src">字符串</param>
        /// <returns></returns>
        public string FilterSpace(string src)
        {
            string strRet = "";
            strRet = src.Replace(" ", "").Trim();
            return strRet;
        }
        #endregion

        #region 4)将时间字符串转换为系统认定的标准时间字符格式(yyyyMMddHHmmss)
        /// <summary>
        /// 将时间字符串转换为系统认定的标准格式(yyyyMMddHHmmss)
        /// </summary>
        /// <param name="srcDateTime">时间字符串</param>
        /// <returns></returns>
        public string ConvertStandardFormater(string srcDateTime)
        {
            string strRet = "";
            strRet = srcDateTime.Replace(" ", "").Trim();
            strRet = strRet.Replace("-", "").Trim();
            strRet = strRet.Replace(":", "").Trim();
            strRet = strRet.PadRight(14, '0');
            return strRet;
        }
        #endregion

        #region 5)数据格式转化
        /// <summary>
        /// 数据格式转化
        /// </summary>
        /// <param name="srcContent">源数据</param>
        /// <param name="srcIndex">源数据索引</param>
        /// <param name="destIndex">目标数据索引</param>
        /// <returns></returns>
        public string FormatString(string srcContent, int srcIndex, int destIndex)
        {
            StringBuilder strRet = new StringBuilder();
            string[] strArr = srcContent.Split(',');

            if (strArr[srcIndex].IndexOf('-') > 0 && strArr[srcIndex].IndexOf(':') > 0)
            {
                string temp = strArr[srcIndex];
                strArr[srcIndex] = strArr[destIndex];
                strArr[destIndex] = temp;
                int nIndex = 0;
                foreach (string str in strArr)
                {
                    if (nIndex == 0)
                    {
                        strRet.Append(str);
                    }
                    else
                    {
                        strRet.Append("," + str);
                    }
                    nIndex++;
                }
            }
            else
            {
                strRet.Append(srcContent);
            }
            return strRet.ToString();
        }
        #endregion

        private int rep = 0;

        /// <summary>
        /// 生成随机数字字符串
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成的数字字符串</returns>
        public string GenerateCheckCodeNum(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }

        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成的字母字符串</returns>
        public string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        public string getGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
