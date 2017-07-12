using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront._public.Token
{
    /// <summary>
    /// token 的摘要说明
    /// </summary>
    public class token : IHttpHandler, IRequiresSessionState
    {
        #region 操作枚举
        /// <summary>
        /// 枚举类型
        /// </summary>
        public enum TOKEN_CreateActionType : int
        {
            /// <summary>
            /// 创建TOKEN
            /// </summary>
            CreateToken = 0
        }
        #endregion

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            TOKEN_CreateActionType type = default(TOKEN_CreateActionType);
            string action = context.Request.Form["action"] ?? "0";
            if (string.IsNullOrEmpty(action)) action = "0";
            type = (TOKEN_CreateActionType)Enum.Parse(typeof(TOKEN_CreateActionType), action);
            string result = "";
            switch (type)
            {
                case TOKEN_CreateActionType.CreateToken:
                    result = CreateToken(context);
                    break;
            }
            context.Response.Write(result);
        }

        private string CreateToken(HttpContext context)
        {
            string result = null;
            try
            {
                string AllowCreateTokenFactory = ConfigurationManager.AppSettings["AllowCreateTokenFactory"] ?? "";
                string facid = context.Request.Form["facid"] ?? "";
                if (AllowCreateTokenFactory.Split(',').ToList().Contains(facid) && !string.IsNullOrEmpty(facid))
                {
                    string t = Utility.randString(32);
                    context.Session["TOKEN_" + facid] = t;
                    Utility.SetCookie("TOKEN_" + facid, t);
                    result = "{\"Code\":\"001\",\"Result\":\"设置成功！\",\"Token\":\"" + t + "\"}";
                }
                else
                {
                    result = "{\"Code\":\"-10001\",\"Result\":\"没有权限创建TOKEN!\",\"Token\":\"\"}";
                }

            }
            catch (Exception ex)
            {
                result = "{\"Code\":\"-10000\",\"Result\":\"" + ex.Message + " -- FACID未设置！\",\"Token\":\"\"}";
            }
            return result;

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}