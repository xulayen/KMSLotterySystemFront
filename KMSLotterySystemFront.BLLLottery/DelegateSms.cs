using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;

namespace KMSLotterySystemFront.BLLLottery
{
    public class DelegateSms
    {
        #region 发送短信委托
        public delegate void DelegateSendSMS(string facid, string message, string mobile, string pfid, string businessType);

        #region 新短信发送
        /// <summary>
        /// 新短信发送
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="message">需要发送内容</param>
        /// <param name="mobile">需要下发的手机</param>
        /// <param name="pfid">平台ID</param>
        /// <param name="businessType">平台ID</param>
        /// <param name="result">输出:短信发送结果</param>
        public static void NewSendSms(string facid, string message, string mobile, string pfid, string businessType)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(mobile))
                {
                    using (NewCcnSendSms.CCNSMSServiceSoapClient NewSmsSend = new NewCcnSendSms.CCNSMSServiceSoapClient())
                    {
                        NewCcnSendSms.ArrayOfString mobilelist = new NewCcnSendSms.ArrayOfString();
                        mobilelist.Add(mobile);

                        string sendUserName = ConfigurationManager.AppSettings["NewCcnSendSmsUserId"].ToString();
                        string sendUserPwd = ConfigurationManager.AppSettings["NewCcnSendSmsUserPwd"].ToString();

                        try
                        {
                            if (!string.IsNullOrEmpty(sendUserName) && !string.IsNullOrEmpty(sendUserPwd))
                            {


                                Logger.AppLog.Write("提交发送短信时异常或者超时传入参数：[facid:" + facid + "][sendUserName:" + sendUserName + "][sendUserPwd:" + sendUserPwd + "][businessType:" + businessType + "][mobilelist:" + mobilelist + "][message:" + message + "][pfid:" + pfid + "]", Logger.AppLog.LogMessageType.Info);

                                NewSmsSend.SendMTMsgHasProgram(facid, sendUserName, sendUserPwd, businessType, mobilelist, message, "86", "8", pfid, "", "", "", out result);

                                Logger.AppLog.Write("提交发送短信时异常或者超时传入参数：[facid:" + facid + "][sendUserName:" + sendUserName + "][sendUserPwd:" + sendUserPwd + "][businessType:" + businessType + "][mobilelist:" + mobilelist + "][message:" + message + "][pfid:" + pfid + "][result:" + result + "]", Logger.AppLog.LogMessageType.Info);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.AppLog.Write("提交发送短信时异常或者超时--" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, Logger.AppLog.LogMessageType.Fatal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("SendMessageWebService:NewSendSms 异常" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, Logger.AppLog.LogMessageType.Fatal);
            }

        }
        #endregion

        public static void PrintComeplete(IAsyncResult result)
        {
            (result.AsyncState as DelegateSendSMS).EndInvoke(result);
        }

        #endregion

        #region 发送红包委托

        public delegate bool DeledateSendRed(HttpContext current, string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num, out string systemState, out string msgresult);

        public static bool SendRed(HttpContext current, string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num, out string systemState, out string msgresult)
        {
            WxPay.WxPayServiceSoapClient wxpay = new WxPay.WxPayServiceSoapClient();
            return wxpay.RedPackSendInt(facId, ccnactivityid, openId, code, money, hbtype, total_num, out systemState, out msgresult);
        }


        public static void DeledateSendRedComeplete(IAsyncResult result)
        {
            string msgresult, systemState;
            (result.AsyncState as DeledateSendRed).EndInvoke(out systemState, out  msgresult, result);

            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-DelegateSms-DeledateSendRedComeplete   OUT [systemState:" + systemState + "] [msgresult:" + msgresult + "] ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
        }
        #endregion


    }
}
