using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KMSLotterySystemFront.Common
{
    public class ResonseMessage
    {
        private string resultTemp = "\"state\":\"{0}\",\"data\":{1},\"sysCode\":\"{2}\",\"msg\":\"{3}\",\"desc\":\"{4}\",\"line\":\"{5}\"";
        private string _state = "0";
        private object _data = "";
        private string _syscode = "00000";
        private string _msg = "失败";

        /// <summary>
        /// 【0：失败】 【1：成功】 【-1：异常】
        /// </summary>
        public string state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }


        public object data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public string sysCode
        {
            get
            {
                return _syscode;
            }
            set
            {
                _syscode = value;
            }
        }

        public string msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        public string desc
        {
            get;
            set;
        }

        public string line
        {
            get;
            set;
        }

        public virtual string ToJson()
        {
            if (this.data != null && this.data.GetType().ToString() == "System.Data.DataTable")
            {
                this.data = BaseFunction.DataTable2Json((this.data as DataTable));
                StringBuilder sb = new StringBuilder("{");
                sb.Append(string.Format(resultTemp, this.state, this.data, this.sysCode, this.msg, this.desc, this.line));
                sb.Append("}");
                return sb.ToString();
            }
            else
            {
                return BaseFunction.ToJson(this);
            }


        }
    }
}
