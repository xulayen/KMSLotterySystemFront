using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using KMSLotterySystemFront.Logger;
using KMSLotterySystemFront.Common;
using System.Threading;
using System.Text;
using CCN.Code2D;
using KMSLotterySystemFront.BLL;
using KMSLotterySystemFront.Error;
using System.Collections;
using System.Data;

namespace KMSLotterySystemFront.Fac9667.server
{
    /// <summary>
    /// KMS 的摘要说明
    /// </summary>
    public class KMS : BaseAshx
    {
        private RechargeLotteryNewPf LotteryService = null;
        private SendMessageWebService.SendMessageWebServiceSoapClient SendMessageWebService = null;
        private string
         Sender = ConfigurationManager.AppSettings["Sender"].ToString(),//发送人
         Message = ConfigurationManager.AppSettings["Message"].ToString(),//发送信息
         facid = ConfigurationManager.AppSettings["facid"].ToString(),
         KMSUserID = ConfigurationManager.AppSettings["KMSUserID"].ToString(),
         KMSPWD = ConfigurationManager.AppSettings["KMSPWD"].ToString(),
         FactoryID = ConfigurationManager.AppSettings["FactoryID"].ToString();

        #region 操作枚举
        /// <summary>
        /// 枚举类型
        /// </summary>
        public enum KMSActionType : int
        {
            /// <summary>
            /// 1、发送验证码
            /// </summary>
            SendMessage = 0,
            /// <summary>
            /// 2、提交验证
            /// </summary>
            SubmitVerify = 1,
            /// <summary>
            /// 3、写入客户端日志
            /// </summary>
            WriteClientLog = 2,
            /// <summary>
            /// 4、根据数码抽奖
            /// </summary>
            GetLotteryByACCode = 3,
            /// <summary>
            /// 设置微信信息
            /// </summary>
            SetWxData = 6,
            /// <summary>
            /// openid是否参与过中奖
            /// </summary>
            ExistsOpenid = 8,
            /// <summary>
            /// 完善个人信息接口
            /// </summary>
            ModifyInfo = 10,

        }
        #endregion

        #region 获取微信对象(哪怕只有一个openid也可以获取成对象)
        private OAuthUserInfo _OAuthUserInfo = null;
        /// <summary>
        /// 获取微信对象(哪怕只有一个openid也可以获取成对象)
        /// </summary>
        private OAuthUserInfo OAuthUserInfo
        {
            get
            {
                string u = "",
                    wxTemp = "\"openid\":\"{0}\",\"nickname\":\"{1}\",\"sex\":{2},\"language\":\"{3}\",\"city\":\"{4}\",\"province\":\"{5}\",\"country\":\"{6}\",\"headimgurl\":\"{7}\",\"privilege\":[]",
                    openid = SGM.Common.BaseFunction.getRequest("openid") ?? "";
                try
                {
                    if (_OAuthUserInfo == null)
                    {
                        if ((ConfigurationManager.AppSettings["UAT"] ?? "") == "1")
                        {
                            u = "{\"openid\":\"oAU3pjt3cCaaqCm4jjVuB2kjuaXo\",\"nickname\":\"幸福就是，我在闹她在笑~~~噗哈哈\",\"sex\":1,\"language\":\"zh_CN\",\"city\":\"\",\"province\":\"\",\"country\":\"中国\",\"headimgurl\":\"http://wx.qlogo.cn/mmopen/PiajxSqBRaEIEYnibm7RFHfZ4aS99ibNKHS0GjibQWvaBQov4zIlLBSs2YF2DQNF0qBQkoIyria566O4FfrXIoBUQpg/0\",\"privilege\":[]}";
                        }
                        else
                        {
                            if (SGM.Common.Web.SessionHelper.GetSession("wxuserdata_KMS") != null)
                            {
                                u = SGM.Common.Web.SessionHelper.GetSession("wxuserdata_KMS").ToString();
                            }
                            else if (HttpContext.Current.Request.Cookies["wxuserdata_KMS"] != null)
                            {
                                u = HttpContext.Current.Request.Cookies["wxuserdata_KMS"].Value.ToString();
                            }
                            else
                            {
                                u = "{" + string.Format(wxTemp, openid, "", "1", "", "", "", "", "") + "}";
                            }
                        }
                        AppLog.Write(string.Format("[KMS OAuthUserInfo]:[openid:{0}] [wxuserdata_KMS:{1}]", openid, u), AppLog.LogMessageType.Info);
                        return _OAuthUserInfo = (new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<OAuthUserInfo>(u));
                    }
                    return _OAuthUserInfo;
                }
                catch (Exception ex)
                {
                    u = "{" + string.Format(wxTemp, openid, "", "1", "", "", "", "", "") + "}";
                    AppLog.Write(string.Format("[KMS Get_OAuthUserInfo]:[errorMsg:{0}] [openid:{1}] [wxuserdata_KMS:{2}]", ex.Message, openid, u), AppLog.LogMessageType.Error);
                    return _OAuthUserInfo = (new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<OAuthUserInfo>(u));
                }

            }
        }
        #endregion

        #region 入口
        public override void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/json";
            DateTime dtAmStart = DateTime.Now.ToUniversalTime();
            ResonseMessage result = new ResonseMessage();
            KMSActionType type = default(KMSActionType);
            try
            {
                LotteryService = new RechargeLotteryNewPf();
                SendMessageWebService = new KMSLotterySystemFront.SendMessageWebService.SendMessageWebServiceSoapClient();
                string action = SGM.Common.BaseFunction.getRequest("action") ?? "";
                if (string.IsNullOrEmpty(action)) { action = "-1"; }
                type = (KMSActionType)Enum.Parse(typeof(KMSActionType), action);
                switch (type)
                {
                    case KMSActionType.SendMessage:
                        base.ProcessRequest(context);
                        result = SendMessage(context);
                        base.ClearTokenSession();
                        break;
                    case KMSActionType.WriteClientLog:
                        WriteClientLog(context);
                        break;
                    case KMSActionType.SubmitVerify:
                        base.ProcessRequest(context);
                        result = SubmitVerify(context);
                        base.ClearTokenSession();
                        break;
                    case KMSActionType.GetLotteryByACCode:
                        base.ProcessRequest(context);
                        result = GetLotteryByACCode(context);
                        base.ClearTokenSession();
                        break;
                    case KMSActionType.SetWxData:
                        SetWxData(context);
                        break;
                    case KMSActionType.ModifyInfo:
                        base.ProcessRequest(context);
                        result = ModifyInfo(context);
                        base.ClearTokenSession();
                        break;
                    case KMSActionType.ExistsOpenid:
                        result = ExistsOpenid(context);
                        break;

                }

                if (type != KMSActionType.WriteClientLog)
                {
                    DateTime dtNightEnd = DateTime.Now.ToUniversalTime();
                    System.TimeSpan ts = dtNightEnd.Subtract(dtAmStart);
                    AppLog.Write("[KMS " + type + " - 耗时T: " + ts.Seconds + "s-" + ts.Milliseconds + "ms ] End", AppLog.LogMessageType.Info);
                }

            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException)
                {
                    AppLog.Write("[KMS " + type + "] [非法提交]", AppLog.LogMessageType.Fatal);
                }
                else
                {
                    AppLog.Write("[KMS " + type + "] [Exception:" + ex.Message + "]", AppLog.LogMessageType.Error, ex);
                }
            }
            finally
            {
                context.Response.Write(result.ToJson());
            }
        }


        #endregion

        #region openid是否参与多活动
        private ResonseMessage ExistsOpenid(HttpContext context)
        {
            ResonseMessage result = null;
            string systemState = "", openid = SGM.Common.BaseFunction.getRequest("openid") ?? "";
            AppLog.Write("[KMS ExistsOpenid]-IN:", AppLog.LogMessageType.Info);
            Dictionary<string, string> dicResult = LotteryService.SelectOpenidKMS(facid, openid, out systemState);
            AppLog.Write("[KMS ExistsOpenid]-OUT:【systemState:" + systemState + "】", AppLog.LogMessageType.Info);
            return result = new ResonseMessage()
             {
                 sysCode = systemState,
                 data = dicResult,
                 state = "1"
             };
        }
        #endregion

        #region √更新个人信息
        private ResonseMessage ModifyInfo(HttpContext context)
        {
            ResonseMessage result = null;
            string systemState = "",
                mobile = SGM.Common.BaseFunction.getRequest("mobile") ?? "",
                provice = SGM.Common.BaseFunction.getRequest("provice") ?? "",
                city = SGM.Common.BaseFunction.getRequest("city") ?? "",
                address = SGM.Common.BaseFunction.getRequest("address") ?? "",
                lid = SGM.Common.BaseFunction.getRequest("lid") ?? "",
                digitcode = SGM.Common.BaseFunction.getRequest("digitcode") ?? "";

            Hashtable postInfo = new Hashtable();
            postInfo.Add("PROVINCE", provice);
            postInfo.Add("USER_ADDRESS", address);
            postInfo.Add("CITY", city);

            AppLog.Write("[KMS ModifyPostAdrKMS]-IN:【facid:" + facid + "】【digitcode:" + digitcode + "】【lid:" + lid + "】【mobile:" + mobile + "】【address:" + address + "】", AppLog.LogMessageType.Info);
            bool c = LotteryService.ModifyPostAdrKMS(facid, digitcode, lid, mobile, postInfo, out systemState);
            AppLog.Write("[KMS ModifyPostAdrKMS]-OUT:【systemState:" + systemState + "】", AppLog.LogMessageType.Info);
            if (c)
            {
                result = new ResonseMessage();
                result.sysCode = systemState;
                result.msg = "成功";
                result.state = "1";
            }
            else
            {
                result = new ResonseMessage();
                result.sysCode = systemState;
                result.msg = "失败";
                result.state = "0";
            }
            return result;
        }
        #endregion

        #region √获取微信信息 记录到 session、cookie
        private void SetWxData(HttpContext context)
        {
            string openid = SGM.Common.BaseFunction.getRequest("data") ?? "";
            AppLog.Write("[KMS SetWxData]-IN: openid:" + openid, AppLog.LogMessageType.Info);
            string wx_data = KMSLotterySystemFront.Common.Utility.HttpGet("openid=" + openid + "&libraryid=1", "http://wechat.cummins.com.cn/kang/weixin/userinfo.do");
            AppLog.Write("[KMS SetWxData]-OUT: wx_data:" + wx_data, AppLog.LogMessageType.Info);
            SGM.Common.Web.SessionHelper.AddSession("wxuserdata_KMS", wx_data);
            KMSLotterySystemFront.Common.Utility.SetCookie("wxuserdata_KMS", wx_data, 1);
        }
        #endregion

        #region √数码抽奖
        private ResonseMessage GetLotteryByACCode(HttpContext context)
        {
            ResonseMessage result = null;
            string scancode = SGM.Common.BaseFunction.getRequest("q") ?? "", //扫描的二维码/输入的数码
             channel = SGM.Common.BaseFunction.getRequest("channel") ?? "M",  //渠道
             username = SGM.Common.BaseFunction.getRequest("username") ?? "",  //用户名称
             cityname = SGM.Common.BaseFunction.getRequest("cityname") ?? "",  //用户城市
             mobile = SGM.Common.BaseFunction.getRequest("mobile") ?? "",  //用户输入的手机号
             openid = SGM.Common.BaseFunction.getRequest("openid") ?? "",  //openid
             verifycode = SGM.Common.BaseFunction.getRequest("verifycode") ?? "",//验证码
            lonlat = SGM.Common.BaseFunction.getRequest("lonlat") ?? "",//经纬度
            browser = SGM.Common.BaseFunction.getRequest("browser") ?? "",//浏览器
            system = SGM.Common.BaseFunction.getRequest("system") ?? "", //系统
            time = SGM.Common.BaseFunction.getRequest("time") ?? "", //gps耗时
            networktype = SGM.Common.BaseFunction.getRequest("networktype") ?? "WIFI", //网络类型
             facid_ = null,  //根据扫描的数码得到的厂家编号
            proid_ = null,  //根据扫描的数码得到的产品编号
            code_ = null,   //根据扫描的数码得到的数码
            systemState_ = "000";   //系统返回的状态

            object formData = SGM.Common.Web.SessionHelper.GetSession("formDataKMS");
            if (channel != "M") channel = "M";
            AppLog.Write(string.Format("[KMS GetLotteryByACCode]:[scancode:{0}] [facid:{1}] [username:{2}] [openid:{3}]", scancode, facid, username, openid), AppLog.LogMessageType.Info);
            if (string.IsNullOrEmpty(scancode))
            {
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN防伪数码不能为空.ToString(),
                    sysCode = CustomerSystemState.NN防伪数码不能为空.GetEnumDescription()
                };
            }

            if (string.IsNullOrEmpty(mobile) || mobile.Equals("undefined") || mobile.Equals("null"))
            {
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN手机号码不能为空.ToString(),
                    sysCode = CustomerSystemState.NN手机号码不能为空.GetEnumDescription()
                };
            }

            if (OAuthUserInfo == null && string.IsNullOrEmpty(openid) && string.IsNullOrEmpty(OAuthUserInfo.openid))
            {
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NNopenid为空.ToString(),
                    sysCode = CustomerSystemState.NNopenid为空.GetEnumDescription()
                };
            }

            string[] facparam = FactoryID.Split(','); //配置文件中，配置多个厂家编码
            if (scancode.Contains("http://")) //二维码
            {
                IDecoderAble cd = CodeManager.CreateGetDecoder(scancode);
                if (cd.Success)
                {
                    facid_ = cd.DecodeEntity.CustomerId;
                    code_ = cd.DecodeEntity.Digit;
                    proid_ = cd.DecodeEntity.ProductNo;
                    AppLog.Write(string.Format("[KMS GetLotteryByACCode 解码成功]:[scancode:{0}] [facid_:{1}] [code_:{2}] [proid_:{3}] ", scancode, facid_, code_, proid_), AppLog.LogMessageType.Info);

                    if (!facparam.Contains(facid))  //解码成功，但不是本厂家生产的数码
                    {
                        AppLog.Write(string.Format("[KMS GetLotteryByACCode 解码成功但不是本厂家生产的数码] : [scancode:{0}] [facid_:{1}] [code_:{2}] [proid_:{3}]", scancode, facid_, code_, proid_), AppLog.LogMessageType.Info);
                        return result = new ResonseMessage()
                        {
                            state = "0",
                            msg = CustomerSystemState.NN解码的厂家不是本活动厂家.ToString(),
                            sysCode = CustomerSystemState.NN解码的厂家不是本活动厂家.GetEnumDescription()
                        };
                    }
                }
                else
                {
                    AppLog.Write(string.Format("[KMS GetLotteryByACCode 解码失败]:[scancode:{0}] [msg:{1}] ", scancode, cd.ErrorMsg), AppLog.LogMessageType.Info);
                    return result = new ResonseMessage()
                    {
                        state = "0",
                        msg = CustomerSystemState.NN二维码解码失败.ToString(),
                        sysCode = CustomerSystemState.NN二维码解码失败.GetEnumDescription(),
                        desc = cd.ErrorMsg
                    };
                }
            }
            else
            {
                code_ = scancode;
            }

            if (!KMSLotterySystemFront.Common.Utility.Check_DigitCode(code_))
            {
                AppLog.Write(string.Format("[KMS GetLotteryByACCode 数码格式错误]:[scancode:{0}] ", scancode), AppLog.LogMessageType.Info);
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN防伪数码格式错误.ToString(),
                    sysCode = CustomerSystemState.NN防伪数码格式错误.GetEnumDescription(),
                    desc = CustomerSystemState.NN防伪数码格式错误.ToString()
                };
            }

            #region 组织用户参数
            Hashtable userHash = new Hashtable();
            userHash.Add("OPENID", (OAuthUserInfo == null ? openid : OAuthUserInfo.openid));
            userHash.Add("USER_NICKNAME", (OAuthUserInfo == null ? openid : OAuthUserInfo.nickname));
            userHash.Add("WX_COUNTRY", (OAuthUserInfo == null ? openid : OAuthUserInfo.country));
            userHash.Add("WX_PROVINCE", (OAuthUserInfo == null ? openid : OAuthUserInfo.province));
            userHash.Add("WX_CITY", (OAuthUserInfo == null ? openid : OAuthUserInfo.city));
            userHash.Add("USER_SEX", (OAuthUserInfo == null ? openid : OAuthUserInfo.sex.ToString()));
            userHash.Add("WX_HEADIMGURL", (OAuthUserInfo == null ? "" : OAuthUserInfo.headimgurl));
            userHash.Add("USER_NAME", username);
            userHash.Add("F2", cityname);
            userHash.Add("F4", channel);
            userHash.Add("F5", networktype);
            userHash.Add("GPS", lonlat);
            userHash.Add("PHONESYSTEM", system);
            userHash.Add("BROWSERTYPE", browser);
            userHash.Add("GPSTIME", time);
            userHash.Add("F15", formData);
            #endregion

            string language = "zh_cn";
            AppLog.Write(string.Format("[KMS GetLotteryByACCode QueryLotteryRechargeKMS IN]:[KMSUserID:{0}], [KMSPWD:{1}], [mobile:{2}], [code_:{3}], [language{4}], [channel:{5}]", KMSUserID, KMSPWD, mobile, code_, language, "X"), AppLog.LogMessageType.Info);
            #region 调用接口
            string lottery_result = "", lotteryLevel = "", lotterName = "", lid = "";
            #endregion
            string message = "", ip = WebHelper.GetClientIP(HttpContext.Current);

            LotteryService.QueryLotteryRechargeKMS(facid, code_, channel, 1, mobile, userHash, ip, out lottery_result, out systemState_, out lotteryLevel, out lotterName, out lid);
            AppLog.Write(string.Format("[KMS GetLotteryByACCode QueryLotteryRechargeKMS OUT]: [lottery_result:{0}] [systemState_:{1}] [lotteryLevel:{2}] [lotterName:{3}] [lid:{4}]", lottery_result, systemState_, lotteryLevel, lotterName, lid), AppLog.LogMessageType.Info);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("result", lottery_result);
            dic.Add("digitcode", code_);
            dic.Add("mobile", mobile);
            dic.Add("lid", lid);
            dic.Add("lotterName", lotterName);
            dic.Add("lotteryLevel", lotteryLevel);
            //{"state":"1","data":{"result":"用户姓名不正确","digitcode":"1111111111111111","mobile":"18301956235","dtLottery":"{}"},"sysCode":"321","msg":"用户姓名不正确","desc":null,"line":null}
            result = new ResonseMessage()
            {
                state = "1",
                msg = lottery_result,
                data = dic,
                sysCode = systemState_
            };
            return result;
        }
        #endregion

        #region √表单提交验证
        private ResonseMessage SubmitVerify(HttpContext context)
        {
            ResonseMessage result = null;
            string mobile = SGM.Common.BaseFunction.getRequest("mobile") ?? "";  //
            string vifycode = SGM.Common.BaseFunction.getRequest("verifycode") ?? "";  //
            string channel = SGM.Common.BaseFunction.getRequest("channel") ?? "";  //
            string openid = SGM.Common.BaseFunction.getRequest("openid") ?? "";  //
            string join = SGM.Common.BaseFunction.getRequest("join") ?? "";  //
            string userName = SGM.Common.BaseFunction.getRequest("userName") ?? "";
            string cityname = SGM.Common.BaseFunction.getRequest("cityName") ?? "";


            string Tag = SGM.Common.BaseFunction.getRequest("Tag") ?? "";
            string Area = SGM.Common.BaseFunction.getRequest("Area") ?? "";
            string province = SGM.Common.BaseFunction.getRequest("province") ?? "";
            string Sex = SGM.Common.BaseFunction.getRequest("Sex") ?? "";
            string Address = SGM.Common.BaseFunction.getRequest("Address") ?? "";

            string formData = "{\"id\":\"\",\"sex\":\"" + Sex + "\",\"address\":\"" + Address + "\",\"tag\":\"" + Tag + "\",\"tel\":\"" + mobile + "\",\"name\":\"" + userName + "\",\"libraryid\":\"1\",\"openid\":\"" + openid + "\",\"dept\":\"\",\"memberstyle\":\"\",\"area\":\"" + Area + "\",\"province\":\"" + province + "\",\"city\":\"" + cityname + "\"}";
            AppLog.Write("[KMS SubmitVerify] [formDataKMS:" + formData + "]", AppLog.LogMessageType.Info);

            if (string.IsNullOrEmpty(mobile))
            {
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN手机号码格式不正确.ToString(),
                    sysCode = CustomerSystemState.NN手机号码格式不正确.GetEnumDescription()
                };
            }
            if (string.IsNullOrEmpty(vifycode))
            {
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN手机验证码格式不正确.ToString(),
                    sysCode = CustomerSystemState.NN手机验证码格式不正确.GetEnumDescription()
                };
            }
            string systemState = "";
            bool tresult = SendMessageWebService.NewVerifyMobile(KMSUserID, KMSPWD, mobile, vifycode, out systemState);
            AppLog.Write("[KMS SubmitVerify NewVerifyMobile] [facid:" + facid + "] [mobile:" + mobile + "] [vifycode:" + vifycode + "] [systemState:" + systemState + "]", AppLog.LogMessageType.Info);

            if (tresult)
            {
                SGM.Common.Web.SessionHelper.AddSession("formDataKMS", formData);

                if (join == "1")
                {
                    string paramter = "{\"id\":\"\",\"sex\":\"" + Sex + "\",\"address\":\"" + Address + "\",\"tag\":\"" + Tag + "\",\"tel\":\"" + mobile + "\",\"name\":\"" + userName + "\",\"libraryid\":\"1\",\"openid\":\"" + openid + "\",\"dept\":\"\",\"memberstyle\":\"\",\"area\":\"" + Area + "\",\"province\":\"" + province + "\",\"city\":\"" + cityname + "\"}";
                    AppLog.Write("[KMS SubmitVerify SubmitVerify-IN-saveuserinfo.do] [paramter:" + paramter + "]", AppLog.LogMessageType.Info);
                    string data = KMSLotterySystemFront.Common.Utility.HttpPost(paramter, "http://wechat.cummins.com.cn/open/kang/saveuserinfo");
                    AppLog.Write("[KMS SubmitVerify SubmitVerify-OUT-saveuserinfo.do] [RESULT:" + data + "]", AppLog.LogMessageType.Info);
                }


                result = new ResonseMessage()
                {
                    state = "1",
                    msg = CustomerSystemState.Y手机验证码验证成功.ToString(),
                    sysCode = CustomerSystemState.Y手机验证码验证成功.GetEnumDescription()
                };
            }
            else
            {
                result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN验证码验证失败.ToString(),
                    sysCode = CustomerSystemState.NN验证码验证失败.GetEnumDescription()
                };
            }



            return result;
        }
        #endregion

        #region √发送验证码
        private ResonseMessage SendMessage(HttpContext context)
        {
            ResonseMessage result = null;
            string sendTo = SGM.Common.BaseFunction.getRequest("mobile") ?? "",
             storid = SGM.Common.BaseFunction.getRequest("storid") ?? "",
             systemstate = "";
            if (string.IsNullOrEmpty(sendTo))
            {
                return result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN手机号码格式不正确.ToString(),
                    sysCode = CustomerSystemState.NN手机号码格式不正确.GetEnumDescription(),
                    desc = CustomerSystemState.NN手机号码格式不正确.ToString()
                };
            }
            bool isflag = SendMessageWebService.NewSendMessages(KMSUserID, KMSPWD, Sender, sendTo, Message, "1", out systemstate);
            AppLog.Write("[KMS SendMessage NewSendMessages] --[facid:" + facid + "] [Sender:" + Sender + "] [sendTo:" + sendTo + "] [Message:" + Message + "] [systemstate:" + systemstate + "]", AppLog.LogMessageType.Info);
            if (systemstate == "10001")
            {
                result = new ResonseMessage()
                {
                    state = "1",
                    msg = CustomerSystemState.Y手机验证码发送成功.ToString(),
                    sysCode = CustomerSystemState.Y手机验证码发送成功.GetEnumDescription(),
                };
            }
            else
            {
                result = new ResonseMessage()
                {
                    state = "0",
                    msg = CustomerSystemState.NN验证码发送失败.ToString(),
                    sysCode = CustomerSystemState.NN验证码发送失败.GetEnumDescription(),
                };
            }

            AppLog.Write("[KMS SendMessage] [result:" + result.ToJson() + "]", AppLog.LogMessageType.Info);

            return result;

        }
        #endregion

        #region √客户端写入日志（只记录客户端错误日志）
        private void WriteClientLog(HttpContext context)
        {
            string type = context.Request["type"];
            string e = context.Request["e"];
            AppLog.Write("[client:" + type + "]：[message:" + e + "]", AppLog.LogMessageType.Error);
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}