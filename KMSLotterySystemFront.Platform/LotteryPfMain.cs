using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InquiryCompatibleIntegration.ComCompatibleInterface;
using KMSLotterySystemFront.Logger;
using InquiryFramework.Core;
using CompatibleToolkit;
using System.Collections;
using KMSLotterySystemFront.Model;

namespace KMSLotterySystemFront.Platform
{
    public class LotteryPfMain
    {
        /// <summary>
        /// 新平台防伪查询
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="ip">消费者查询IP地址</param>
        /// <param name="code">消费者查询数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="country">查询国别</param>
        /// <param name="channeltype">查询渠道日志记录</param>
        /// <param name="iscustomer">是否为客服查询</param>
        /// <param name="reply">输出:防伪查询答复</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="isPfCode">输出:是否为新平台数码 (0:老平台 1:新平台)</param>
        /// <returns></returns>
        public bool PfMainExecuteInquiry(string directoryName, string ip, string code, string channel, string language, string iscustomer, string country, string channeltype, out string reply, out string systemState, out string isPfCode)
        {
            bool bRet = false;
            reply = "";
            systemState = "000";
            isPfCode = "0";
            try
            {
                #region 组织传递参数
                //组织传递参数
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_ISINTEGRATED, "1");

                // //任意Any = 0, //防伪DAC = 1,//积分 SGM = 2,//溯源TRACE = 3
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_BUSINESSTYPE, "1");
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, directoryName);

                //厂家编号
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (!directoryName.Equals("00000"))
                    {
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FACTORYID, directoryName);
                    }
                }
                //IP地址
                if (!string.IsNullOrEmpty(ip))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);
                }
                //查询数码
                if (!string.IsNullOrEmpty(code))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CODE, code);
                }
                //是否为客服查询
                if (!string.IsNullOrEmpty(iscustomer))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_ISCALLEDFROMINSIDE, iscustomer);
                }

                //查询渠道
                if (!string.IsNullOrEmpty(channel))
                {
                    switch (channel)
                    {
                        case "W":
                            channel = "10";
                            break;
                        case "M":
                            channel = "20";
                            break;
                        case "A":
                            channel = "21";
                            break;
                        default:
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CHANNELTYPE, channel);
                }
                //查询语言
                if (!string.IsNullOrEmpty(language))
                {
                    switch (language)
                    {
                        case "1":
                            language = "zh-cn";
                            break;
                        case "2":
                            language = "en-US";
                            break;
                        default:
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_LANGAUGETYPE, language);
                }

                //查询国别
                if (!string.IsNullOrEmpty(country))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F11", country);
                }
                //查询渠道日志记录
                if (!string.IsNullOrEmpty(channeltype))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F12", channeltype);
                }

                #endregion

                try
                {
                    Dictionary<string, object> result = CompatibleHelper.SendInquiry(dict);

                    if (result.ContainsKey("INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"))
                    {
                        //01-新平台，05-老平台
                        if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("01"))
                            isPfCode = "1";
                        else if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("05"))
                            isPfCode = "0";
                    }
                    else
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("新平台解码未返回平台标志，请联系管理员", AppLog.LogMessageType.Error);
                    }

                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"))//答复
                    {
                        reply = result["INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"].ToString();
                    }
                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCODE"))//系统执行状态
                    {
                        systemState = result["INQUIRYCONTEXT_VARIABLE_REPLYCODE"].ToString();
                    }


                    KMSLotterySystemFront.Logger.AppLog.Write(directoryName + "新平台返回信息:是否为新平台数码:" + isPfCode + "--reply:" + reply + "--systemState:" + systemState, AppLog.LogMessageType.Error);

                    if (!string.IsNullOrEmpty(reply) && !string.IsNullOrEmpty(systemState))
                    {
                        bRet = true;
                    }

                    return bRet;
                }
                catch (Exception ex1)
                {
                    KMSLotterySystemFront.Logger.AppLog.Write("PfMainExecuteInquiry 新平台查询异常" + "--" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, AppLog.LogMessageType.Error);
                    return bRet;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("PfMainExecuteInquiry 新平台加载DLL 异常" + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, AppLog.LogMessageType.Error);
                return bRet;
            }
        }

        /// <summary>
        /// 新平台防伪查询
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="ip">消费者查询IP地址</param>
        /// <param name="code">消费者查询数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="iscustomer">是否为客服查询</param>
        /// <param name="other">区分查询渠道不同答复</param>
        /// <param name="msgHash">其他信息集合</param>
        /// <param name="cityCode">门店所属区域编码</param>
        /// <param name="queryID">防伪查询记录请求ID</param>
        /// <param name="reply">输出:防伪查询答复</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="isPfCode">输出:是否为新平台数码 (0:老平台 1:新平台)</param>
        /// <returns></returns>
        public bool PfMainExecuteInquiry(string directoryName, string ip, string code, string channel, string language, string iscustomer, string other, Hashtable msgHash, string cityCode, string queryID, out string reply, out string systemState, out string isPfCode)
        {
            bool bRet = false;
            reply = "";
            systemState = "000";
            isPfCode = "0";
            try
            {
                #region 组织传递参数
                //组织传递参数
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_ISINTEGRATED, "1");

                // //任意Any = 0, //防伪DAC = 1,//积分 SGM = 2,//溯源TRACE = 3
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_BUSINESSTYPE, "1");
                if (channel.Trim().Equals("S"))
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, "10669588210896");
                else
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, directoryName);

                //厂家编号
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (!directoryName.Equals("00000"))
                    {
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FACTORYID, directoryName);
                    }
                }
                //IP地址
                if (!string.IsNullOrEmpty(ip))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);
                }
                //查询数码
                if (!string.IsNullOrEmpty(code))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CODE, code);
                }
                //是否为客服查询
                if (!string.IsNullOrEmpty(iscustomer))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_ISCALLEDFROMINSIDE, iscustomer);
                }

                //查询渠道
                //0-任意，1-短信，2-电话，10-网站，11-客户端，20-WAP，21-APP 
                if (!string.IsNullOrEmpty(channel))
                {
                    switch (channel)
                    {
                        case "W":
                            channel = "10";
                            break;
                        case "M":
                            channel = "20";
                            break;
                        case "A":
                            channel = "21";
                            break;
                        case "S":
                            channel = "1";
                            break;
                        default:
                            channel = "21";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CHANNELTYPE, channel);
                    if (channel.Equals("21"))
                    {
                        //用户自定义，对于ChannelType为21时，AppID可以如下Wechat[微信], Wochacha[我查查], FastInquiry[企业APP]
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_APPLICATIONID, "FastInquiry");
                    }
                }


                //查询语言
                if (!string.IsNullOrEmpty(language))
                {
                    switch (language)
                    {
                        case "1":
                            language = "zh-cn";
                            break;
                        case "2":
                            language = "en-US";
                            break;
                        default:
                            language = "zh-cn";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_LANGAUGETYPE, language);
                }

                //查询扩展字段
                if (!string.IsNullOrEmpty(other))
                {
                    //特殊抽取组配置
                    dict.Add(InquiryContext.INQUIRYCONTEXT_EXTEND_VARIABLE_DIMENSION_4, other);

                    //用户区分记录促销平台来源日志  用于促销参与情况分析
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F13", other);
                }

                //if (msgHash != null)
                //{
                //    if (msgHash.Count > 0)
                //    {
                //        int i = 14;
                //        foreach (DictionaryEntry Message in msgHash)
                //        {
                //            if (!string.IsNullOrEmpty(Message.Key.ToString()))
                //            {
                //                if (i == 21)
                //                    break;

                //                dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F" + i.ToString(), Message.Value.ToString());
                //                i++;
                //            }
                //        }
                //    }
                //}

                if (!string.IsNullOrEmpty(cityCode))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                }
                if (!string.IsNullOrEmpty(queryID))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F18", queryID);
                }

                #endregion

                try
                {
                    Dictionary<string, object> result = CompatibleHelper.SendInquiry(dict);

                    if (result.ContainsKey("INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"))
                    {
                        //01-新平台，05-老平台
                        if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("01"))
                            isPfCode = "1";
                        else if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("05"))
                            isPfCode = "0";
                    }
                    else
                    {
                        Logger.AppLog.Write("新平台解码未返回平台标志，请联系管理", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    }

                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"))//答复
                    {
                        reply = result["INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"].ToString();
                    }
                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCODE"))//答复码
                    {
                        systemState = result["INQUIRYCONTEXT_VARIABLE_REPLYCODE"].ToString();
                    }

                    Logger.AppLog.Write("新平台返回信息:是否为新平台数码:" + isPfCode + "--reply:" + reply + "--systemState:" + systemState, AppLog.LogMessageType.Info);

                    if (!string.IsNullOrEmpty(reply) && !string.IsNullOrEmpty(systemState))
                    {
                        bRet = true;
                    }

                    return bRet;
                }
                catch (Exception ex1)
                {
                    Logger.AppLog.Write("PfMainExecuteInquiry 新平台查询异常" + "--" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, AppLog.LogMessageType.Error);
                    return bRet;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PfMainExecuteInquiry 新平台加载DLL 异常" + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, AppLog.LogMessageType.Error);
                return bRet;
            }
        }

        /// <summary>
        /// 新平台防伪查询
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="ip">消费者查询IP地址</param>
        /// <param name="code">消费者查询数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="iscustomer">是否为客服查询</param>
        /// <param name="other">区分查询渠道不同答复</param>
        /// <param name="msgHash">其他信息集合</param>
        /// <param name="cityCode">门店所属区域编码</param>
        /// <param name="queryID">防伪查询记录请求ID</param>
        /// <param name="isGPS">是否为GPS定位</param>
        /// <param name="gps">GPS定位</param>
        /// <param name="reply">输出:防伪查询答复</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="isPfCode">输出:是否为新平台数码 (0:老平台 1:新平台)</param>
        /// <returns></returns>
        public bool PfMainExecuteInquiry(string directoryName, string ip, string code, string channel, string language, string iscustomer, string other, Hashtable msgHash, string cityCode, string queryID, bool isGPS, string GPS, out string reply, out string systemState, out string isPfCode)
        {
            bool bRet = false;
            reply = "";
            systemState = "000";
            isPfCode = "0";
            try
            {
                #region 组织传递参数
                //组织传递参数
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_ISINTEGRATED, "1");

                // //任意Any = 0, //防伪DAC = 1,//积分 SGM = 2,//溯源TRACE = 3
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_BUSINESSTYPE, "1");

                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, directoryName);


                //厂家编号
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (!directoryName.Equals("00000"))
                    {
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FACTORYID, directoryName);
                    }
                }
                ////IP地址
                //if (!string.IsNullOrEmpty(ip))
                //{
                //    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);
                //}

                if (isGPS)
                {
                    string provinceName = string.Empty;
                    string cityName = string.Empty;
                    string gpsCityCode = string.Empty;
                    string acitivityId = string.Empty;
                    GetGpsApiByIP(GPS, out  provinceName, out  cityName, out  gpsCityCode, out acitivityId);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_CITYCODE, gpsCityCode);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, GPS);

                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }
                }
                else
                {


                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }

                }

                if (!string.IsNullOrEmpty(queryID))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F18", queryID);
                }


                //查询数码
                if (!string.IsNullOrEmpty(code))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CODE, code);
                }
                //是否为客服查询
                if (!string.IsNullOrEmpty(iscustomer))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_ISCALLEDFROMINSIDE, iscustomer);
                }

                //查询渠道
                //0-任意，1-短信，2-电话，10-网站，11-客户端，20-WAP，21-APP 
                if (!string.IsNullOrEmpty(channel))
                {
                    switch (channel)
                    {
                        case "W":
                            channel = "10";
                            break;
                        case "M":
                            channel = "20";
                            break;
                        case "A":
                            channel = "21";
                            break;
                        case "S":
                            channel = "1";
                            break;
                        default:
                            channel = "21";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CHANNELTYPE, channel);
                    if (channel.Equals("21"))
                    {
                        //用户自定义，对于ChannelType为21时，AppID可以如下Wechat[微信], Wochacha[我查查], FastInquiry[企业APP]
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_APPLICATIONID, "FastInquiry");
                    }
                }


                //查询语言
                if (!string.IsNullOrEmpty(language))
                {
                    switch (language)
                    {
                        case "1":
                            language = "zh-cn";
                            break;
                        case "2":
                            language = "en-US";
                            break;
                        default:
                            language = "zh-cn";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_LANGAUGETYPE, language);
                }

                //查询扩展字段
                if (!string.IsNullOrEmpty(other))
                {
                    //特殊抽取组配置
                    dict.Add(InquiryContext.INQUIRYCONTEXT_EXTEND_VARIABLE_DIMENSION_4, other);

                    //用户区分记录促销平台来源日志  用于促销参与情况分析
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F13", other);
                }

                #endregion

                try
                {
                    Dictionary<string, object> result = CompatibleHelper.SendInquiry(dict);

                    if (result.ContainsKey("INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"))
                    {
                        //01-新平台，05-老平台
                        if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("01"))
                            isPfCode = "1";
                        else if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("05"))
                            isPfCode = "0";
                    }
                    else
                    {
                        Logger.AppLog.Write("新平台解码未返回平台标志，请联系管理", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    }

                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"))//答复
                    {
                        reply = result["INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"].ToString();
                    }
                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCODE"))//答复码
                    {
                        systemState = result["INQUIRYCONTEXT_VARIABLE_REPLYCODE"].ToString();
                    }

                    Logger.AppLog.Write("新平台返回信息:是否为新平台数码:" + isPfCode + "--reply:" + reply + "--systemState:" + systemState, AppLog.LogMessageType.Info);

                    if (!string.IsNullOrEmpty(reply) && !string.IsNullOrEmpty(systemState))
                    {
                        bRet = true;
                    }

                    return bRet;
                }
                catch (Exception ex1)
                {
                    Logger.AppLog.Write("PfMainExecuteInquiry 新平台查询异常" + "--" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, AppLog.LogMessageType.Error);
                    return bRet;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PfMainExecuteInquiry 新平台加载DLL 异常" + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, AppLog.LogMessageType.Error);
                return bRet;
            }
        }


        #region 新平台防伪查询-最新版
        /// <summary>
        /// 新平台防伪查询-最新版
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        ///  <param name="infactoryid">内部厂家编号</param>
        /// <param name="ip">消费者查询IP地址/手机号</param>
        /// <param name="code">消费者查询数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="iscustomer">是否为客服查询</param>
        /// <param name="other">oId 区分查询渠道不同答复</param>
        /// <param name="msgHash">用户信息，其他信息集合</param>
        /// <param name="cityCode">门店所属区域编码</param>
        /// <param name="queryID">防伪查询记录请求ID</param>
        /// <param name="isGPS">是否为GPS定位</param>
        /// <param name="gps">GPS定位</param>
        /// <param name="reply">输出:防伪查询答复</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="isPfCode">输出:是否为新平台数码 (0:老平台 1:新平台)</param>
        /// <returns></returns>
        public bool PfMainExecuteInquiry(string factoryid, string infactoryid, string ip, string code, string channel, string language, string iscustomer, string other, Hashtable msgHash, string cityCode, string queryID, bool isGPS, string GPS, out string reply, out string systemState, out string isPfCode)
        {
            bool bRet = false;
            reply = "";
            systemState = "000";
            isPfCode = "0";
            try
            {
                #region 组织传递参数
                //组织传递参数
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_ISINTEGRATED, "1");

                // //任意Any = 0, //防伪DAC = 1,//积分 SGM = 2,//溯源TRACE = 3
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_BUSINESSTYPE, "1");

                if (string.IsNullOrEmpty(infactoryid))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, factoryid);
                }
                else
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, infactoryid);
                }

                //厂家编号
                if (!string.IsNullOrEmpty(factoryid))
                {
                    if (!factoryid.Equals("00000"))
                    {
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FACTORYID, factoryid);
                    }
                }
                ////IP地址
                //if (!string.IsNullOrEmpty(ip))
                //{
                //    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);
                //}

                if (isGPS)
                {
                    string provinceName = string.Empty;
                    string cityName = string.Empty;
                    string gpsCityCode = string.Empty;
                    string acitivityId = string.Empty;
                    GetGpsApiByIP(GPS, out  provinceName, out  cityName, out  gpsCityCode, out acitivityId);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_CITYCODE, gpsCityCode);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, GPS);

                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }
                }
                else
                {


                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }

                }

                if (!string.IsNullOrEmpty(queryID))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F18", queryID);
                }


                //查询数码
                if (!string.IsNullOrEmpty(code))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CODE, code);
                }
                //是否为客服查询
                if (!string.IsNullOrEmpty(iscustomer))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_ISCALLEDFROMINSIDE, iscustomer);
                }

                //查询渠道
                //0-任意，1-短信，2-电话，10-网站，11-客户端，20-WAP，21-APP 
                if (!string.IsNullOrEmpty(channel))
                {
                    switch (channel)
                    {
                        case "W":
                            channel = "10";
                            break;
                        case "M":
                            channel = "20";
                            break;
                        case "A":
                            channel = "21";
                            break;
                        case "S":
                            channel = "1";
                            break;
                        default:
                            channel = "21";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CHANNELTYPE, channel);
                    if (channel.Equals("21"))
                    {
                        //用户自定义，对于ChannelType为21时，AppID可以如下Wechat[微信], Wochacha[我查查], FastInquiry[企业APP]
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_APPLICATIONID, "FastInquiry");
                    }
                }


                //查询语言
                if (!string.IsNullOrEmpty(language))
                {
                    switch (language)
                    {
                        case "1":
                            language = "zh-cn";
                            break;
                        case "2":
                            language = "en-US";
                            break;
                        default:
                            language = "zh-cn";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_LANGAUGETYPE, language);
                }

                //查询扩展字段
                if (!string.IsNullOrEmpty(other))
                {
                    //特殊抽取组配置
                    dict.Add(InquiryContext.INQUIRYCONTEXT_EXTEND_VARIABLE_DIMENSION_4, other);

                    //用户区分记录促销平台来源日志  用于促销参与情况分析
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F13", other);
                }

                #endregion

                try
                {
                    Dictionary<string, object> result = CompatibleHelper.SendInquiry(dict);

                    if (result.ContainsKey("INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"))
                    {
                        //01-新平台，05-老平台
                        if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("01"))
                            isPfCode = "1";
                        else if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("05"))
                            isPfCode = "0";
                    }
                    else
                    {
                        Logger.AppLog.Write("新平台解码未返回平台标志，请联系管理", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    }

                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"))//答复
                    {
                        reply = result["INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"].ToString();
                    }
                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCODE"))//答复码
                    {
                        systemState = result["INQUIRYCONTEXT_VARIABLE_REPLYCODE"].ToString();
                    }

                    Logger.AppLog.Write("新平台返回信息:是否为新平台数码:" + isPfCode + "--reply:" + reply + "--systemState:" + systemState, AppLog.LogMessageType.Info);

                    if (!string.IsNullOrEmpty(reply) && !string.IsNullOrEmpty(systemState))
                    {
                        bRet = true;
                    }

                    return bRet;
                }
                catch (Exception ex1)
                {
                    Logger.AppLog.Write("PfMainExecuteInquiry 新平台查询异常" + "--" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, AppLog.LogMessageType.Error);
                    return bRet;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PfMainExecuteInquiry 新平台加载DLL 异常" + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, AppLog.LogMessageType.Error);
                return bRet;
            }
        }
        #endregion








        /// <summary>
        /// 新平台防伪查询
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="ip">消费者查询IP地址</param>
        /// <param name="code">消费者查询数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="iscustomer">是否为客服查询</param>
        /// <param name="other">区分查询渠道不同答复</param>
        /// <param name="msgHash">其他信息集合</param>
        /// <param name="cityCode">门店所属区域编码</param>
        /// <param name="queryID">防伪查询记录请求ID</param>
        /// <param name="isGPS">是否为GPS定位</param>
        /// <param name="gps">GPS定位</param>
        /// <param name="gpsCityCode">GPS定位的CityCode</param>
        /// <param name="reply">输出:防伪查询答复</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="isPfCode">输出:是否为新平台数码 (0:老平台 1:新平台)</param>
        /// <returns></returns>
        public bool PfMainExecuteInquiry2(string directoryName, string  infactoryid,string ip, string code, string channel, string language, string iscustomer, string other, Hashtable msgHash, string cityCode, string queryID, bool isGPS, string GPS, string gpsCityCode, out string reply, out string systemState, out string isPfCode)
        {
            bool bRet = false;
            reply = "";
            systemState = "000";
            isPfCode = "0";
            try
            {
                #region 组织传递参数
                //组织传递参数
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_ISINTEGRATED, "1");

                // //任意Any = 0, //防伪DAC = 1,//积分 SGM = 2,//溯源TRACE = 3
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_BUSINESSTYPE, "1");

               // dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, directoryName);

                if (string.IsNullOrEmpty(infactoryid))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, directoryName);
                }
                else
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, infactoryid);
                }

                //厂家编号
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (!directoryName.Equals("00000"))
                    {
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FACTORYID, directoryName);
                    }
                }
                ////IP地址
                //if (!string.IsNullOrEmpty(ip))
                //{
                //    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);
                //}

                if (isGPS)
                {
                    //string provinceName = string.Empty;
                    //string cityName = string.Empty;
                    //string gpsCityCode = string.Empty;
                    //string acitivityId = string.Empty;
                    //GetGpsApiByIP(GPS, out  provinceName, out  cityName, out  gpsCityCode, out acitivityId);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_CITYCODE, gpsCityCode);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, GPS);

                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }
                }
                else
                {
                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }

                }

                if (!string.IsNullOrEmpty(queryID))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F18", queryID);
                }


                //查询数码
                if (!string.IsNullOrEmpty(code))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CODE, code);
                }
                //是否为客服查询
                if (!string.IsNullOrEmpty(iscustomer))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_ISCALLEDFROMINSIDE, iscustomer);
                }

                //查询渠道
                //0-任意，1-短信，2-电话，10-网站，11-客户端，20-WAP，21-APP 
                if (!string.IsNullOrEmpty(channel))
                {
                    switch (channel)
                    {
                        case "W":
                            channel = "10";
                            break;
                        case "M":
                            channel = "20";
                            break;
                        case "A":
                            channel = "21";
                            break;
                        case "S":
                            channel = "1";
                            break;
                        default:
                            channel = "21";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CHANNELTYPE, channel);
                    if (channel.Equals("21"))
                    {
                        //用户自定义，对于ChannelType为21时，AppID可以如下Wechat[微信], Wochacha[我查查], FastInquiry[企业APP]
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_APPLICATIONID, "FastInquiry");
                    }
                }


                //查询语言
                if (!string.IsNullOrEmpty(language))
                {
                    switch (language)
                    {
                        case "1":
                            language = "zh-cn";
                            break;
                        case "2":
                            language = "en-US";
                            break;
                        default:
                            language = "zh-cn";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_LANGAUGETYPE, language);
                }

                //查询扩展字段
                if (!string.IsNullOrEmpty(other))
                {
                    //特殊抽取组配置
                    dict.Add(InquiryContext.INQUIRYCONTEXT_EXTEND_VARIABLE_DIMENSION_4, other);

                    //用户区分记录促销平台来源日志  用于促销参与情况分析
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F13", other);
                }

                #endregion

                try
                {
                    Dictionary<string, object> result = CompatibleHelper.SendInquiry(dict);

                    if (result.ContainsKey("INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"))
                    {
                        //01-新平台，05-老平台
                        if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("01"))
                            isPfCode = "1";
                        else if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("05"))
                            isPfCode = "0";
                    }
                    else
                    {
                        Logger.AppLog.Write("新平台解码未返回平台标志，请联系管理", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    }

                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"))//答复
                    {
                        reply = result["INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"].ToString();
                    }
                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCODE"))//答复码
                    {
                        systemState = result["INQUIRYCONTEXT_VARIABLE_REPLYCODE"].ToString();
                    }

                    Logger.AppLog.Write("新平台返回信息:是否为新平台数码:" + isPfCode + "--reply:" + reply + "--systemState:" + systemState, AppLog.LogMessageType.Info);

                    if (!string.IsNullOrEmpty(reply) && !string.IsNullOrEmpty(systemState))
                    {
                        bRet = true;
                    }

                    return bRet;
                }
                catch (Exception ex1)
                {
                    Logger.AppLog.Write("PfMainExecuteInquiry 新平台查询异常" + "--" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, AppLog.LogMessageType.Error);
                    return bRet;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PfMainExecuteInquiry 新平台加载DLL 异常" + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, AppLog.LogMessageType.Error);
                return bRet;
            }
        }



        /// <summary>
        /// 新平台防伪查询
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="ip">消费者查询IP地址</param>
        /// <param name="code">消费者查询数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="iscustomer">是否为客服查询</param>
        /// <param name="other">区分查询渠道不同答复</param>
        /// <param name="msgHash">其他信息集合</param>
        /// <param name="cityCode">门店所属区域编码</param>
        /// <param name="queryID">防伪查询记录请求ID</param>
        /// <param name="isGPS">是否为GPS定位</param>
        /// <param name="gps">GPS定位</param>
        /// <param name="reply">输出:防伪查询答复</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="isPfCode">输出:是否为新平台数码 (0:老平台 1:新平台)</param>
        /// <returns></returns>
        public bool PfMainExecuteInquiryNew(string directoryName, string ip, string code, string channel, string language, string iscustomer, string other, Hashtable msgHash, string cityCode, string queryID, bool isGPS, string GPS, out string reply, out string systemState, out string isPfCode)
        {
            bool bRet = false;
            reply = "";
            systemState = "000";
            isPfCode = "0";
            try
            {
                #region 组织传递参数
                //组织传递参数
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_ISINTEGRATED, "1");

                // //任意Any = 0, //防伪DAC = 1,//积分 SGM = 2,//溯源TRACE = 3
                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_BUSINESSTYPE, "1");

                dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_TO, directoryName);


                //厂家编号
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (!directoryName.Equals("00000"))
                    {
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FACTORYID, directoryName);
                    }
                }
                ////IP地址
                //if (!string.IsNullOrEmpty(ip))
                //{
                //    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);
                //}

                if (isGPS)
                {
                    string provinceName = string.Empty;
                    string cityName = string.Empty;
                    string gpsCityCode = string.Empty;
                    string acitivityId = string.Empty;
                    GetGpsApiByIP(GPS, out  provinceName, out  cityName, out  gpsCityCode, out acitivityId);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_CITYCODE, gpsCityCode);

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, GPS);

                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }
                }
                else
                {


                    //消费者活动参与的手机号码
                    if (!string.IsNullOrEmpty(ip))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F16", ip);
                    }

                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_FROM, ip);

                    if (!string.IsNullOrEmpty(cityCode))
                    {
                        dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F17", cityCode);
                    }

                }

                if (!string.IsNullOrEmpty(queryID))
                {
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F18", queryID);
                }


                //查询数码
                if (!string.IsNullOrEmpty(code))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CODE, code);
                }
                //是否为客服查询
                if (!string.IsNullOrEmpty(iscustomer))
                {
                    dict.Add(InquiryContext.INQUIRYCONTEXT_CUSTOM_VARIABLE_ISCALLEDFROMINSIDE, iscustomer);
                }

                //查询渠道
                //0-任意，1-短信，2-电话，10-网站，11-客户端，20-WAP，21-APP 
                if (!string.IsNullOrEmpty(channel))
                {
                    switch (channel)
                    {
                        case "W":
                            channel = "10";
                            break;
                        case "M":
                            channel = "20";
                            break;
                        case "A":
                            channel = "21";
                            break;
                        case "S":
                            channel = "1";
                            break;
                        default:
                            channel = "21";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_CHANNELTYPE, channel);
                    if (channel.Equals("21"))
                    {
                        //用户自定义，对于ChannelType为21时，AppID可以如下Wechat[微信], Wochacha[我查查], FastInquiry[企业APP]
                        dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_APPLICATIONID, "Wechat");
                    }
                }


                //查询语言
                if (!string.IsNullOrEmpty(language))
                {
                    switch (language)
                    {
                        case "1":
                            language = "zh-cn";
                            break;
                        case "2":
                            language = "en-US";
                            break;
                        default:
                            language = "zh-cn";
                            break;
                    }
                    dict.Add(InquiryContext.INQUIRYCONTEXT_VARIABLE_LANGAUGETYPE, language);
                }

                //查询扩展字段
                if (!string.IsNullOrEmpty(other))
                {
                    //特殊抽取组配置
                    dict.Add(InquiryContext.INQUIRYCONTEXT_EXTEND_VARIABLE_DIMENSION_4, other);

                    //用户区分记录促销平台来源日志  用于促销参与情况分析
                    dict.Add("INQUIRYCONTEXT_EXTEND_VARIABLE_FACTORY_LOG_F13", other);
                }





                #endregion

                try
                {
                    Dictionary<string, object> result = CompatibleHelper.SendInquiry(dict);

                    if (result.ContainsKey("INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"))
                    {
                        //01-新平台，05-老平台
                        if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("01"))
                            isPfCode = "1";
                        else if (Convert.ToString(result["INQUIRYCONTEXT_CUSTOM_VARIABLE_ISNEWPLATFORM"]).Equals("05"))
                            isPfCode = "0";
                    }
                    else
                    {
                        Logger.AppLog.Write("新平台解码未返回平台标志，请联系管理", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    }

                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"))//答复
                    {
                        reply = result["INQUIRYCONTEXT_VARIABLE_REPLYCONTENT"].ToString();
                    }
                    if (result.ContainsKey("INQUIRYCONTEXT_VARIABLE_REPLYCODE"))//答复码
                    {
                        systemState = result["INQUIRYCONTEXT_VARIABLE_REPLYCODE"].ToString();
                    }

                    Logger.AppLog.Write("新平台返回信息:是否为新平台数码:" + isPfCode + "--reply:" + reply + "--systemState:" + systemState, AppLog.LogMessageType.Info);

                    if (!string.IsNullOrEmpty(reply) && !string.IsNullOrEmpty(systemState))
                    {
                        bRet = true;
                    }

                    return bRet;
                }
                catch (Exception ex1)
                {
                    Logger.AppLog.Write("PfMainExecuteInquiryNew 新平台查询异常" + "--" + ex1.TargetSite + "--" + ex1.StackTrace + "--" + ex1.Message, AppLog.LogMessageType.Error);
                    return bRet;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PfMainExecuteInquiryNew 新平台加载DLL 异常" + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, AppLog.LogMessageType.Error);
                return bRet;
            }
        }




        #region 通过GPS获取详细地理位置
        /// <summary>
        /// GPS解析获得详细GPS位置信息
        /// </summary>
        /// <param name="Longitude">经纬度(经度|纬度)</param>
        /// <param name="provinceName">输出:省份</param>
        /// <param name="cityName">输出:城市</param>
        /// <param name="cityCode">输出:（经度|纬度|城市编码）</param>
        /// <param name="acitivityId">输出：API接口返回的查询记录ID</param>
        /// <returns></returns>
        public bool GetGpsApiByIP(string gps, out string provinceName, out string cityName, out string cityCode, out string acitivityId)
        {
            bool bRet = false;
            provinceName = string.Empty;
            cityName = string.Empty;
            cityCode = string.Empty;
            acitivityId = string.Empty;
            try
            {
                GpsInterfaceEntity gpsinfo = null;

                string Longitude;
                string Latitude;

                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.Platform.LotteryPfMain.cs--GetGpsApiByIP: [GPS:" + gps + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                if (!string.IsNullOrEmpty(gps))
                {
                    if (KMSLotterySystemFront.Common.RegexExpress.IsGPS(gps))
                    {
                        if (gps.Contains("|"))
                        {
                            string[] gpslist = gps.Split('|');
                            Longitude = gpslist[0];
                            Latitude = gpslist[1];

                            //KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.Platform.LotteryPfMain.cs--GetGpsApiByIP: [Longitude:" + Longitude + "][Latitude:" + Latitude + "] ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);


                            if (!string.IsNullOrEmpty(Longitude) && !string.IsNullOrEmpty(Latitude))
                            {
                                DateTime dtAmStartgps = DateTime.Now.ToUniversalTime();

                                if (KMSLotterySystemFront.Common.WebHelper.GetGPSInfo("1", Longitude, Latitude, "", out gpsinfo))
                                {
                                    if (gpsinfo != null)
                                    {
                                        provinceName = gpsinfo.ResponseData.ProvinceName;
                                        cityName = gpsinfo.ResponseData.CityName;
                                        cityCode = gpsinfo.ResponseData.CityCode;
                                        acitivityId = gpsinfo.AcitivityId;

                                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.Platform.LotteryPfMain.cs--GetGpsApiByIP: [provinceName:" + provinceName + "][cityName:" + cityName + "][cityCode:" + cityCode + "] ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                                        DateTime dtNightEndgps = DateTime.Now.ToUniversalTime();
                                        System.TimeSpan tsgps = dtNightEndgps.Subtract(dtAmStartgps);
                                        Logger.AppLog.Write("KMSLotterySystemFront.Platform.LotteryPfMain.cs--GetGpsApiByIP: [Longitude:" + Longitude + "][Latitude:" + Latitude + "] [AcitivityId:" + gpsinfo.AcitivityId + "]" + tsgps.Seconds.ToString() + "-" + tsgps.Milliseconds.ToString(), Logger.AppLog.LogMessageType.Info);

                                        bRet = true;
                                    }
                                    else
                                    {
                                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.Platform.LotteryPfMain.cs--GetGpsApiByIP: [Longitude:" + Longitude + "][Latitude:" + Latitude + "] 解析GPS得到的gpsinfo为空", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                                    }
                                }
                            }
                        }
                        else//判断是否是IP地址，如果是IP地址，将根据IP地址，获取省市区
                        {

                            //   PubInfo info = new PubInfo();

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetGpsApiByXYZ: [GPS:" + gps + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }


        #endregion
    }
}
