using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Common
{
    public class InviteHandleResult
    {

        private string mobile = "";//手机号
        private string error_code = "";//错误码
        private string error_msg = "";//错误描述

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get { return mobile; }
            set { mobile = value; }
        }


        /// <summary>
        /// /错误码
        /// </summary>
        public string Error_code
        {
            get { return error_code; }
            set { error_code = value; }
        }
      
        /// <summary>
        /// 错误描述
        /// </summary>
        public string Error_msg
        {
            get { return error_msg; }
            set { error_msg = value; }
        }
       

      

    }
}
