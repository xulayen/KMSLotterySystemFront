using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class RegisterConfig
    {
        public RegisterConfig() { }

        private string _decollator;
        private string _keyword;
        private string _fieldlist;
        private string _tablename;
        private string _facid;

        /// <summary>
        /// 注册信息分割符
        /// </summary>
        public string Decollator
        {
            get { return _decollator; }
            set { _decollator = value; }
        }
       
        //注册信息关键字
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }
        
        /// <summary>
        /// 注册信息存储配置
        /// </summary>
        public string Fieldlist
        {
            get { return _fieldlist; }
            set { _fieldlist = value; }
        }
       
        /// <summary>
        /// 配置表名
        /// </summary>
        public string Tablename
        {
            get { return _tablename; }
            set { _tablename = value; }
        }
        
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string Facid
        {
            get { return _facid; }
            set { _facid = value; }
        }


    }
}
