// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Common
// *文件名称：WebHelper.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：IIS监控
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using RestSharp;
using KMSLotterySystemFront.Model;


namespace KMSLotterySystemFront.Common
{
    public class WebHelper
    {
        #region 1)本方法用于获取客户端的IP地址 yhr20100316
        /// <summary>
        /// 本方法用于获取客户端的IP地址
        /// </summary>
        /// <param name="ctx">HttpContext上下文对象</param>
        /// <returns></returns>
        public static string GetClientIP(HttpContext ctx)
        {
            NameValueCollection coll = ctx.Request.ServerVariables;
            //负载均衡下获取
            string IP = coll.Get("HTTP_X_FORWARDED_FOR");
            if (IP == null)
                //非负载
                return coll.Get("REMOTE_ADDR");
            return IP;
        }
        #endregion



        #region 1)本方法用于获取客户端的IP地址 wzp20161019
        /// <summary>
        /// 本方法用于获取客户端的IP地址 (type:HTTP_X_FORWARDED_FOR)
        /// </summary>
        /// <param name="ctx">HttpContext上下文对象</param>
        /// <returns></returns>
        public static string GetClientIP2(HttpContext ctx)
        {
            NameValueCollection coll = ctx.Request.ServerVariables;
            //string IP = coll.Get("HTTP_X_FORWARDED_FOR");
            ////取得通过代理服务器访问网络的客户的真实IP            
            //if (IP == null)
            //    IP = coll.Get("REMOTE_ADDR");
            ////如果不是通过代理服务器访问，取得其IP            
            return coll.Get("HTTP_X_FORWARDED_FOR");
        }
        #endregion

        #region 2)IIS运行监测
        /// <summary>
        /// IIS运行监测
        /// </summary>
        /// <param name="webUrl">检测地址</param>
        /// <returns></returns>
        public static Boolean CheckServerIsRun(string webUrl)
        {
            Boolean bRet = false;
            try
            {
                WebRequest request = WebRequest.Create(webUrl);
                HttpWebResponse reponse = (HttpWebResponse)request.GetResponse();
                System.IO.Stream stream = reponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string strRet = reader.ReadToEnd();
                stream.Close();
                if (reponse.StatusCode.ToString() == "OK" && strRet == "Hello World\r\n")
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("内部服务器错误") && ex.Message.Contains("500"))
                {
                    bRet = false;
                }
            }
            return bRet;
        }
        #endregion

        #region 5) GPS解析

        /// <summary>
        /// GPS解析公共接口
        /// </summary>
        /// <param name="Version">版本号</param>
        /// <param name="Longitude">经度</param>
        /// <param name="Latitude">纬度</param>
        /// <param name="zinfo">高度</param>
        public static bool GetGPSInfo(string Version, string Longitude, string Latitude, string zinfo, out GpsInterfaceEntity gpsinfo)
        {
            gpsinfo = null;
            bool bRet = false;

            DateTime dtAmStart = DateTime.Now.ToUniversalTime();

            try
            {

                var client = new RestClient(SysConfig.Instance.GpsApiUrl);
                var request = new RestRequest(Method.POST);
                //request.AddHeader("content-type", "application/x-www-form-urlencoded");
                //request.AddHeader("postman-token", "5780b8bc-b1f9-e9e2-2393-86233266991a");
                //request.AddHeader("v", Version);//指定容器版本号
                //request.AddHeader("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssffff"));//暂未使用,建议填写
                ////request.AddHeader("access_token", "xxx"); //暂未使用,可不填
                //request.AddHeader("app_key", SysConfig.Instance.GpsApiAppKey);// 必须,固定为此值
                //request.AddHeader("Longitude ", Longitude); // 接口参数
                //request.AddHeader("Latitude ", Latitude);// 接口参数
                ////request.AddParameter("application/x-www-form-urlencoded", "Longitude=117.2109760000&Latitude=31.8583220000", ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);

                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("postman-token", "5780b8bc-b1f9-e9e2-2393-86233266991a");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("app_key", SysConfig.Instance.GpsApiAppKey);
                request.AddParameter("Longitude", Longitude);
                request.AddParameter("Latitude", Latitude);
                //request.AddParameter("application/x-www-form-urlencoded", "Longitude=117.2109760000&Latitude=31.8583220000", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                DateTime dtNightEnd = DateTime.Now.ToUniversalTime();
                System.TimeSpan ts = dtNightEnd.Subtract(dtAmStart);

                KMSLotterySystemFront.Logger.AppLog.Write("解析经纬度 [Longitude:" + Longitude + "] [Latitude:" + Latitude + "] 时间 T" + ts.Seconds.ToString() + "-" + ts.Milliseconds.ToString(), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);



                string ResponseContent = response.Content;

                KMSLotterySystemFront.Logger.AppLog.Write(" [Longitude:" + Longitude + "] [Latitude:" + Latitude + "] API解析接口返回" + ResponseContent, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                if (!string.IsNullOrEmpty(ResponseContent))
                {
                    try
                    {
                        gpsinfo = JsonHelper.DeserializeJsonToObject<GpsInterfaceEntity>(ResponseContent);
                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("解析GPS返回数据异常" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        throw ex;
                    }
                }
                else
                {
                    KMSLotterySystemFront.Logger.AppLog.Write("解析GPS数据返回为空 [Longitude:" + Longitude + "] [Latitude:" + Latitude + "] 时间 T" + ts.Seconds.ToString() + "-" + ts.Milliseconds.ToString(), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                }

                if (gpsinfo != null)
                {
                    if (gpsinfo.IsSuccess.ToLower().Equals("true"))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex1)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("WebHelper.GetGPSInfo OUT[Error] " + " ---" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return bRet;
            }
            return bRet;
        }
        #endregion
    }
}
