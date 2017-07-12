using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using KMSLotterySystemFront.Error;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront
{
    public class BaseAshx : IHttpHandler, IRequiresSessionState
    {

        public bool IsReusable
        {
            get { return false; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            string facid = ConfigurationManager.AppSettings["FACID"];

            string action = context.Request["action"] ?? "";
            string token = context.Request["token"] ?? "";
            if (string.IsNullOrEmpty(token))
            {
                throw new CustomerException("非法提交", CustomerSystemState.N非法提交);
            }

            object ServerToken = context.Session["TOKEN_" + facid];
            if (ServerToken == null)
            {
                ServerToken = KMSLotterySystemFront.Common.Utility.getCookie("TOKEN_" + facid);
            }

            if (ServerToken == null)
            {
                throw new CustomerException("非法提交", CustomerSystemState.N非法提交);
            }

            if ((string)ServerToken != token)
            {
                throw new CustomerException("非法提交", CustomerSystemState.N非法提交);
            }
        }


        public void ClearTokenSession()
        {
            string facid = ConfigurationManager.AppSettings["FACID"];
            HttpContext.Current.Session["TOKEN_" + facid] = null;
            KMSLotterySystemFront.Common.Utility.clearCookie("TOKEN_" + facid);
        }




    }
}