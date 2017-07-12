using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace KMSLotterySystemFront
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            try
            {
                //在应用程序启动时运行的代码
                KMSLotterySystemFront.Logger.AppLog.Init();
                KMSLotterySystemFront.Logger.AppLog.Write("KMS抽奖...", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                bool bRet = KMSLotterySystemFront.Common.SysConfig.Instance.InitApplicationResultByDBConfig();
                if (bRet) KMSLotterySystemFront.Logger.AppLog.Write("InitApplicationResultByDBConfig load success……", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                bRet = KMSLotterySystemFront.Common.SysConfig.Instance.InitLotteryConfig();
                if (bRet) KMSLotterySystemFront.Logger.AppLog.Write("InitLotteryConfig load success……", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("Application_Start" + ex.Source + "--" + ex.StackTrace + "--" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
            Logger.AppLog.Write(string.Format("时间:{0},错误消息:{1},地址：{2}", DateTime.Now.ToString(), Server.GetLastError().InnerException != null ? Server.GetLastError().InnerException.Message : "", Request.Url.AbsolutePath), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。

        }

    }
}
