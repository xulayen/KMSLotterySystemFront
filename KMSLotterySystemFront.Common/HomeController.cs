using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.Security;
using KMSLotterySystemFront.Model;
 

namespace KMSLotterySystemFront.Common
{
    public class HomeController
    {
        public string SendRedPack(SendRedPackModel model, string SendURL)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.ReceiverOpenId))
                {
                    return HttpHelper.SendRequest(SendURL, HttpMethod.Post, EntityToDic(model));
                }

                return null;
            }
            catch (Exception ex)
            {
                string exout = "LotteryService.GetRedEnvelope：" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace;
                Logger.AppLog.Write(exout, Logger.AppLog.LogMessageType.Fatal);
                return null;
            }

        }


        public static Dictionary<string, object> EntityToDic<T>(T entity)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>();

            Type type = entity.GetType();
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo i in ps)
            {
                dicParam.Add(i.Name, i.GetValue(entity, null) ?? "");
            }
            return dicParam;
        }

    }
}
