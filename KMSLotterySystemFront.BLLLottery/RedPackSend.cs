using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KMSLotterySystemFront.Common;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using KMSLotterySystemFront.BLLLottery;
using KMSLotterySystemFront.Model;
using System.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using CCN.WeiXin.Pay;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using WxPayAPI;
using CCN.Code2D;

namespace KMSLotterySystemFront.BLLLottery
{
    public class RedPackSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="facId">厂家</param>
        /// <param name="ccnactivityid">内部活动编号</param>
        /// <param name="openId">微信粉丝号</param>
        /// <param name="code">数码</param>
        /// <param name="money">金额</param>
        /// <param name="hbtype">0普通红包 1裂变红包</param>
        /// <param name="total_num">红包发放数量 普通红包为1</param>
        /// <param name="systemState">状态</param>
        /// <param name="msgresult">答复</param>
        /// <returns></returns>
        public static bool RedPackSendInt(HttpContext current, string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num, out string systemState, out string msgresult)
        {
            DateTime dtAmStart = DateTime.Now.ToUniversalTime();
            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt IN[current:" + ((current == null) ? "为空" : "不为空") + "] [ip:" + WebHelper.GetClientIP(current) + "] [ApplicationPath:" + current.Request.PhysicalApplicationPath + "] [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            bool bRet = false;
            systemState = "000000";
            msgresult = "失败";
            string strhbtype = "01";//0 普通红包，1 裂变红包  2 查询红包

            KMSLotterySystemFront.Common.CharHelper chelp = new CharHelper();
            PubInfo info = new PubInfo();


            #region 参数校验
            if (!KMSLotterySystemFront.Common.Utility.CheckParamIsEmpty(facId, ccnactivityid, openId, money, hbtype, total_num, code))
            {
                systemState = "200100";//传入参数为空
                msgresult = "传入参数为空";
                return bRet;
            }
            else if (!strhbtype.Contains(hbtype))
            {
                systemState = "200112";//
                msgresult = "传入参数为空-红包发放类型不正确";
                return bRet;
            }

            else if (!Common.RegexExpress.IsValidDEMICAL(money))//发放金额不正确
            {
                systemState = "200101";//发放金额不正确
                msgresult = "发放金额不正确";
                return bRet;
            }
            else
            {

            }

            //判断参数长度 
            //校验数据格式
            #endregion
            //红包发放类别

            if (hbtype == "0")
            {
                total_num = "1";//普通红包数量为1

                if (Convert.ToDecimal(money) > 200 || Convert.ToDecimal(money) < 1)
                {
                    systemState = "200102";//发放金额不在1-200元之间
                    msgresult = "普通红包发放金额必须在1-200元之间";
                    return bRet;
                }

            }
            else//裂变红包   ，数量大于一  小于20
            {

                //裂变红包金额中，发放红包的最大数量控制

                int maxgroupnum = 20;//此处调取数据库配置

                decimal averagemoney = Convert.ToDecimal(money) / Convert.ToInt32(total_num);
                if (averagemoney < 1 || averagemoney > 200)
                {
                    systemState = "200103";//裂变红包中，平均单个红包金额必须在1-200之间
                    msgresult = "裂变红包中，平均单个红包金额必须在1-200之间";
                    return bRet;
                }
                else if (Convert.ToInt32(total_num) <= 1 || Convert.ToInt32(total_num) > 20)
                {
                    systemState = "200104";//裂变红包数量最大必须为2-20之间
                    msgresult = "裂变红包数量最大必须为2-20之间";
                    return bRet;
                }
                else if (Convert.ToDecimal(money) > 1000 || Convert.ToDecimal(money) < 1)//默认裂变红包金额在1-1000元之间
                {
                    systemState = "200105";//默认裂变红包金额在1-1000元之间
                    msgresult = "默认裂变红包金额在1-1000元之间";
                    return bRet;
                }
            }

            // string NewGuid = chelp.getGuid();
            string redpackguid = chelp.getGuid();//红包发放日志记录表
            string wxpay_guid = chelp.getGuid();//调取红包接口日志记录表


            bool issendrp = false;
            try
            {
                string ipServer = WebHelper.GetClientIP(current);
                #region 判断获取配置
                WechatRedPack redpackinfo = null;
                if (info.GetRedPackConfigNew(facId, ccnactivityid, hbtype, out systemState, out redpackinfo))
                {
                    redpackinfo.WEIXIN_RE_OPENID = openId;
                    redpackinfo.WEIXIN_CLIENT_IP = ipServer;
                    redpackinfo.WEIXIN_TOTAL_AMOUNT = (Decimal.Parse(money) * 100).ToString();
                    redpackinfo.WEIXIN_GUID = redpackguid;
                    redpackinfo.WEIXIN_CODEID = code;
                    redpackinfo.WEIXIN_LOTTERYID = "";
                    redpackinfo.WEIXIN_TOTAL_NUM = total_num;
                    redpackinfo.WEIXIN_HB_TYPE = hbtype;
                    issendrp = true;

                }
                #endregion

                #region 红包发放
                if (issendrp)
                {

                    int aaaaaa = redpackinfo.WEIXIN_SEND_NAME.Length;

                    byte[] bytese = new byte[aaaaaa];
                    int aa = bytese.Length;

                    string newmoney = (double.Parse(money) * 100).ToString();   //单位：分
                    WxPayResult wxpayresult = new WxPayResult();
                    WxPayData data = new WxPayData();
                    string mch_billno = WxPayApi.GetMch_billno(redpackinfo.WEIXIN_MCH_ID);
                    data.SetValue("mch_billno", mch_billno); //商户订单号
                    data.SetValue("mch_id", redpackinfo.WEIXIN_MCH_ID);  //  商户号
                    data.SetValue("wxappid", redpackinfo.WEIXIN_APPID); //  公众账号appid
                    data.SetValue("send_name", redpackinfo.WEIXIN_SEND_NAME);   //  商户名称
                    data.SetValue("re_openid", redpackinfo.WEIXIN_RE_OPENID);  //  用户openid
                    data.SetValue("total_amount", newmoney);  //付款金额
                    data.SetValue("total_num", total_num);  //  红包发放总人数
                    data.SetValue("wishing", redpackinfo.WEIXIN_WISHING);   //   红包祝福语
                    data.SetValue("client_ip", ipServer);  //Ip地址
                    data.SetValue("act_name", redpackinfo.WEIXIN_ACT_NAME);  //   活动名称
                    data.SetValue("remark", redpackinfo.WEIXIN_REMARK);  //  备注
                    data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());  //  随机字符串
                    if (hbtype == "1")//裂变红包
                    {
                        data.SetValue("amt_type", "ALL_RAND");  //  每人领取的金额随机
                    }
                    data.SetValue("sign", data.MakeSign(redpackinfo.WEIXIN_CERTIFICATESECRET));// 签名
                    string xml = data.ToXml();
                    KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSendInt IN [xml:" + xml + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    string response = "";
                    // string CashRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";//现金红包
                    // string GroupRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendgroupredpack"//裂变红包
                    // string  GetRedPackInfoUrl="https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo";//查询红包（包括裂变以及普通红包）
                    if (hbtype == "0")
                    {

                        response = Common.WxPay.WxPayHttpService.Post(current, xml, WxPayConfig.CashRedPackUrl, true, redpackinfo.WEIXIN_CERTIFICATEPATH, redpackinfo.WEIXIN_CERTPASS, 6);
                    }
                    else
                    {
                        response = Common.WxPay.WxPayHttpService.Post(current, xml, WxPayConfig.GroupRedPackUrl, true, redpackinfo.WEIXIN_CERTIFICATEPATH, redpackinfo.WEIXIN_CERTPASS, 6);
                    }

                    KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSendInt  OUT [xml:" + response + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                    redpackinfo.WEIXIN_MCH_BILLNO = mch_billno;
                    wxpayresult.hb_type = hbtype;
                    wxpayresult.total_num = total_num;
                    wxpayresult.ccnactivityid = ccnactivityid;
                    wxpayresult.ccnactivityname = redpackinfo.WEIXIN_SEND_NAME;
                    wxpayresult.facid = facId;
                    wxpayresult.codeid = code;
                    wxpayresult.ip = ipServer;
                    wxpayresult.mch_billno = mch_billno;
                    wxpayresult.mch_id = redpackinfo.WEIXIN_MCH_ID;
                    wxpayresult.wxappid = redpackinfo.WEIXIN_APPID;
                    wxpayresult.re_openid = redpackinfo.WEIXIN_RE_OPENID;
                    wxpayresult.total_amount = money;
                    wxpayresult.guid = wxpay_guid;

                    //解析返回的数据
                    if (!string.IsNullOrEmpty(response))
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);
                            XmlElement rootElement = doc.DocumentElement;
                            wxpayresult.return_code = rootElement.SelectSingleNode("return_code").InnerText;
                            wxpayresult.result_code = rootElement.SelectSingleNode("result_code").InnerText;
                            wxpayresult.return_msg = rootElement.SelectSingleNode("return_msg").InnerText;
                            wxpayresult.err_code = rootElement.SelectSingleNode("err_code").InnerText;
                            wxpayresult.err_code_des = rootElement.SelectSingleNode("err_code_des").InnerText;
                            wxpayresult.mch_billno = rootElement.SelectSingleNode("mch_billno").InnerText;
                            wxpayresult.mch_id = rootElement.SelectSingleNode("mch_id").InnerText;
                            wxpayresult.wxappid = rootElement.SelectSingleNode("wxappid").InnerText;
                            wxpayresult.re_openid = rootElement.SelectSingleNode("re_openid").InnerText;
                            wxpayresult.total_amount = rootElement.SelectSingleNode("total_amount").InnerText;
                            if (wxpayresult.return_code.ToUpper() == "SUCCESS" && wxpayresult.result_code.ToUpper() == "SUCCESS")
                            {
                                wxpayresult.send_listid = rootElement.SelectSingleNode("send_listid").InnerText;//红包订单的微信单号
                                redpackinfo.WEIXIN_SEND_LISTID = rootElement.SelectSingleNode("send_listid").InnerText;//红包订单的微信单号
                                systemState = "100100";//发放成功
                                msgresult = "发放成功";
                            }
                            else
                            {
                                systemState = "100200";//发放失败
                                msgresult = "发放失败";

                                //systemState = wxpayresult.result_code;//发放失败
                                //msgresult = wxpayresult.err_code_des;//红包接口发放失败具体原因


                            }
                            redpackinfo.WEIXIN_MCH_BILLNO = wxpayresult.mch_billno;
                            redpackinfo.WEIXIN_STATE = wxpayresult.result_code;
                            redpackinfo.WEIXIN_STATEDETAIL = wxpayresult.err_code_des;
                            //额外附加信息
                            wxpayresult.hb_type = hbtype;
                            wxpayresult.total_num = total_num;
                            wxpayresult.ccnactivityid = ccnactivityid;
                            wxpayresult.ccnactivityname = redpackinfo.WEIXIN_SEND_NAME;
                            wxpayresult.facid = facId;
                            wxpayresult.codeid = "";
                            wxpayresult.ip = ipServer;
                        }
                        catch (Exception ex)
                        {
                            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt 红包发放解析返回的xml异常:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                            systemState = "200108";//支付异常
                            msgresult = "支付异常";

                        }
                    }
                    else
                    {
                        redpackinfo.WEIXIN_STATE = "失败";
                        redpackinfo.WEIXIN_STATEDETAIL = "调取红包接口,无数据返回,支付异常";
                        wxpayresult.return_code = "FAIL";
                        wxpayresult.result_code = "红包调用异常";
                        systemState = "200108";//支付异常
                        msgresult = "支付异常";
                    }
                    bRet = info.AddRedPackLog(redpackinfo, wxpayresult);//记录日志
                }
                else
                {
                    if (systemState == "200109")
                    {
                        systemState = "200109";
                        msgresult = "红包发放配置信息已经失效";
                    }
                    else if (systemState == "200110")
                    {
                        systemState = "200110";
                        msgresult = "红包发放活动不在有效时间范围内";
                    }
                    else
                    {
                        //无配置信息
                        systemState = "200111";//无配置信息（）
                        msgresult = "红包发放无配置信息";
                    }
                }
                #endregion
                //记录红包发放日志
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt 红包发放异常:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                systemState = "200108";//支付异常
                msgresult = "支付异常";

            }
            DateTime dtNightEnd = DateTime.Now.ToUniversalTime();
            System.TimeSpan ts = dtNightEnd.Subtract(dtAmStart);
            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt   OUT [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "] [systemState:" + systemState + "] T" + ts.Seconds.ToString() + "-" + ts.Milliseconds.ToString(), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            return bRet;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="facId">厂家</param>
        /// <param name="ccnactivityid">内部活动编号</param>
        /// <param name="openId">微信粉丝号</param>
        /// <param name="code">数码</param>
        /// <param name="money">金额</param>
        /// <param name="hbtype">0普通红包 1裂变红包</param>
        /// <param name="total_num">红包发放数量 普通红包为1</param>
        /// <param name="systemState">状态</param>
        /// <param name="msgresult">答复</param>
        /// <returns></returns>
        public static bool RedPackSendInt(string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num, out string systemState, out string msgresult)
        {
            DateTime dtAmStart = DateTime.Now.ToUniversalTime();
            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt IN [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            bool bRet = false;
            systemState = "000000";
            msgresult = "失败";
            string strhbtype = "01";//0 普通红包，1 裂变红包  2 查询红包

            KMSLotterySystemFront.Common.CharHelper chelp = new CharHelper();
            PubInfo info = new PubInfo();


            #region 参数校验
            if (!KMSLotterySystemFront.Common.Utility.CheckParamIsEmpty(facId, ccnactivityid, openId, money, hbtype, total_num, code))
            {
                systemState = "200100";//传入参数为空
                msgresult = "传入参数为空";
                return bRet;
            }
            else if (!strhbtype.Contains(hbtype))
            {
                systemState = "200112";//
                msgresult = "传入参数为空-红包发放类型不正确";
                return bRet;
            }

            else if (!Common.RegexExpress.IsValidDEMICAL(money))//发放金额不正确
            {
                systemState = "200101";//发放金额不正确
                msgresult = "发放金额不正确";
                return bRet;
            }
            else
            {

            }

            //判断参数长度 
            //校验数据格式
            #endregion
            //红包发放类别

            if (hbtype == "0")
            {
                total_num = "1";//普通红包数量为1

                if (Convert.ToDecimal(money) > 200 || Convert.ToDecimal(money) < 1)
                {
                    systemState = "200102";//发放金额不在1-200元之间
                    msgresult = "普通红包发放金额必须在1-200元之间";
                    return bRet;
                }

            }
            else//裂变红包   ，数量大于一  小于20
            {

                //裂变红包金额中，发放红包的最大数量控制

                int maxgroupnum = 20;//此处调取数据库配置

                decimal averagemoney = Convert.ToDecimal(money) / Convert.ToInt32(total_num);
                if (averagemoney < 1 || averagemoney > 200)
                {
                    systemState = "200103";//裂变红包中，平均单个红包金额必须在1-200之间
                    msgresult = "裂变红包中，平均单个红包金额必须在1-200之间";
                    return bRet;
                }
                else if (Convert.ToInt32(total_num) <= 1 || Convert.ToInt32(total_num) > 20)
                {
                    systemState = "200104";//裂变红包数量最大必须为2-20之间
                    msgresult = "裂变红包数量最大必须为2-20之间";
                    return bRet;
                }
                else if (Convert.ToDecimal(money) > 1000 || Convert.ToDecimal(money) < 1)//默认裂变红包金额在1-1000元之间
                {
                    systemState = "200105";//默认裂变红包金额在1-1000元之间
                    msgresult = "默认裂变红包金额在1-1000元之间";
                    return bRet;
                }
            }

            // string NewGuid = chelp.getGuid();
            string redpackguid = chelp.getGuid();//红包发放日志记录表
            string wxpay_guid = chelp.getGuid();//调取红包接口日志记录表


            bool issendrp = false;
            try
            {
                string ipServer = WebHelper.GetClientIP(HttpContext.Current);
                #region 判断获取配置
                WechatRedPack redpackinfo = null;
                if (info.GetRedPackConfigNew(facId, ccnactivityid, hbtype, out systemState, out redpackinfo))
                {
                    redpackinfo.WEIXIN_RE_OPENID = openId;
                    redpackinfo.WEIXIN_CLIENT_IP = ipServer;
                    redpackinfo.WEIXIN_TOTAL_AMOUNT = (Decimal.Parse(money) * 100).ToString();
                    redpackinfo.WEIXIN_GUID = redpackguid;
                    redpackinfo.WEIXIN_CODEID = code;
                    redpackinfo.WEIXIN_LOTTERYID = "";
                    redpackinfo.WEIXIN_TOTAL_NUM = total_num;
                    redpackinfo.WEIXIN_HB_TYPE = hbtype;
                    issendrp = true;

                }
                #endregion

                #region 红包发放
                if (issendrp)
                {

                    int aaaaaa = redpackinfo.WEIXIN_SEND_NAME.Length;

                    byte[] bytese = new byte[aaaaaa];
                    int aa = bytese.Length;

                    string newmoney = (double.Parse(money) * 100).ToString();   //单位：分
                    WxPayResult wxpayresult = new WxPayResult();
                    WxPayData data = new WxPayData();
                    string mch_billno = WxPayApi.GetMch_billno(redpackinfo.WEIXIN_MCH_ID);
                    data.SetValue("mch_billno", mch_billno); //商户订单号
                    data.SetValue("mch_id", redpackinfo.WEIXIN_MCH_ID);  //  商户号
                    data.SetValue("wxappid", redpackinfo.WEIXIN_APPID); //  公众账号appid
                    data.SetValue("send_name", redpackinfo.WEIXIN_SEND_NAME);   //  商户名称
                    data.SetValue("re_openid", redpackinfo.WEIXIN_RE_OPENID);  //  用户openid
                    data.SetValue("total_amount", newmoney);  //付款金额
                    data.SetValue("total_num", total_num);  //  红包发放总人数
                    data.SetValue("wishing", redpackinfo.WEIXIN_WISHING);   //   红包祝福语
                    data.SetValue("client_ip", ipServer);  //Ip地址
                    data.SetValue("act_name", redpackinfo.WEIXIN_ACT_NAME);  //   活动名称
                    data.SetValue("remark", redpackinfo.WEIXIN_REMARK);  //  备注
                    data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());  //  随机字符串
                    if (hbtype == "1")//裂变红包
                    {
                        data.SetValue("amt_type", "ALL_RAND");  //  每人领取的金额随机
                    }
                    data.SetValue("sign", data.MakeSign(redpackinfo.WEIXIN_CERTIFICATESECRET));// 签名
                    string xml = data.ToXml();
                    KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSendInt IN [xml:" + xml + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    string response = "";
                    // string CashRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";//现金红包
                    // string GroupRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendgroupredpack"//裂变红包
                    // string  GetRedPackInfoUrl="https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo";//查询红包（包括裂变以及普通红包）
                    if (hbtype == "0")
                    {

                        response = Common.WxPay.WxPayHttpService.Post(HttpContext.Current, xml, WxPayConfig.CashRedPackUrl, true, redpackinfo.WEIXIN_CERTIFICATEPATH, redpackinfo.WEIXIN_CERTPASS, 6);
                    }
                    else
                    {
                        response = Common.WxPay.WxPayHttpService.Post(HttpContext.Current, xml, WxPayConfig.GroupRedPackUrl, true, redpackinfo.WEIXIN_CERTIFICATEPATH, redpackinfo.WEIXIN_CERTPASS, 6);
                    }

                    KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSendInt  OUT [xml:" + response + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                    redpackinfo.WEIXIN_MCH_BILLNO = mch_billno;
                    wxpayresult.hb_type = hbtype;
                    wxpayresult.total_num = total_num;
                    wxpayresult.ccnactivityid = ccnactivityid;
                    wxpayresult.ccnactivityname = redpackinfo.WEIXIN_SEND_NAME;
                    wxpayresult.facid = facId;
                    wxpayresult.codeid = code;
                    wxpayresult.ip = ipServer;
                    wxpayresult.mch_billno = mch_billno;
                    wxpayresult.mch_id = redpackinfo.WEIXIN_MCH_ID;
                    wxpayresult.wxappid = redpackinfo.WEIXIN_APPID;
                    wxpayresult.re_openid = redpackinfo.WEIXIN_RE_OPENID;
                    wxpayresult.total_amount = money;
                    wxpayresult.guid = wxpay_guid;

                    //解析返回的数据
                    if (!string.IsNullOrEmpty(response))
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);
                            XmlElement rootElement = doc.DocumentElement;
                            wxpayresult.return_code = rootElement.SelectSingleNode("return_code").InnerText;
                            wxpayresult.result_code = rootElement.SelectSingleNode("result_code").InnerText;
                            wxpayresult.return_msg = rootElement.SelectSingleNode("return_msg").InnerText;
                            wxpayresult.err_code = rootElement.SelectSingleNode("err_code").InnerText;
                            wxpayresult.err_code_des = rootElement.SelectSingleNode("err_code_des").InnerText;
                            wxpayresult.mch_billno = rootElement.SelectSingleNode("mch_billno").InnerText;
                            wxpayresult.mch_id = rootElement.SelectSingleNode("mch_id").InnerText;
                            wxpayresult.wxappid = rootElement.SelectSingleNode("wxappid").InnerText;
                            wxpayresult.re_openid = rootElement.SelectSingleNode("re_openid").InnerText;
                            wxpayresult.total_amount = rootElement.SelectSingleNode("total_amount").InnerText;
                            if (wxpayresult.return_code.ToUpper() == "SUCCESS" && wxpayresult.result_code.ToUpper() == "SUCCESS")
                            {
                                wxpayresult.send_listid = rootElement.SelectSingleNode("send_listid").InnerText;//红包订单的微信单号
                                redpackinfo.WEIXIN_SEND_LISTID = rootElement.SelectSingleNode("send_listid").InnerText;//红包订单的微信单号
                                systemState = "100100";//发放成功
                                msgresult = "发放成功";
                            }
                            else
                            {
                                systemState = "100200";//发放失败
                                msgresult = "发放失败";

                                //systemState = wxpayresult.result_code;//发放失败
                                //msgresult = wxpayresult.err_code_des;//红包接口发放失败具体原因


                            }
                            redpackinfo.WEIXIN_MCH_BILLNO = wxpayresult.mch_billno;
                            redpackinfo.WEIXIN_STATE = wxpayresult.result_code;
                            redpackinfo.WEIXIN_STATEDETAIL = wxpayresult.err_code_des;
                            //额外附加信息
                            wxpayresult.hb_type = hbtype;
                            wxpayresult.total_num = total_num;
                            wxpayresult.ccnactivityid = ccnactivityid;
                            wxpayresult.ccnactivityname = redpackinfo.WEIXIN_SEND_NAME;
                            wxpayresult.facid = facId;
                            wxpayresult.codeid = "";
                            wxpayresult.ip = ipServer;
                        }
                        catch (Exception ex)
                        {
                            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt 红包发放解析返回的xml异常:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                            systemState = "200108";//支付异常
                            msgresult = "支付异常";

                        }
                    }
                    else
                    {
                        redpackinfo.WEIXIN_STATE = "失败";
                        redpackinfo.WEIXIN_STATEDETAIL = "调取红包接口,无数据返回,支付异常";
                        wxpayresult.return_code = "FAIL";
                        wxpayresult.result_code = "红包调用异常";
                        systemState = "200108";//支付异常
                        msgresult = "支付异常";
                    }
                    bRet = info.AddRedPackLog(redpackinfo, wxpayresult);//记录日志
                }
                else
                {
                    if (systemState == "200109")
                    {
                        systemState = "200109";
                        msgresult = "红包发放配置信息已经失效";
                    }
                    else if (systemState == "200110")
                    {
                        systemState = "200110";
                        msgresult = "红包发放活动不在有效时间范围内";
                    }
                    else
                    {
                        //无配置信息
                        systemState = "200111";//无配置信息（）
                        msgresult = "红包发放无配置信息";
                    }
                }
                #endregion
                //记录红包发放日志
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt 红包发放异常:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                systemState = "200108";//支付异常
                msgresult = "支付异常";

            }
            DateTime dtNightEnd = DateTime.Now.ToUniversalTime();
            System.TimeSpan ts = dtNightEnd.Subtract(dtAmStart);
            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSendInt   OUT [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "] [systemState:" + systemState + "] T" + ts.Seconds.ToString() + "-" + ts.Milliseconds.ToString(), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            return bRet;
        }

        #region 司能红包发放专用
        /// <summary>
        /// 
        /// </summary>
        /// <param name="facId"></param>
        /// <param name="ccnactivityid"></param>
        /// <param name="openId"></param>
        /// <param name="code"></param>
        /// <param name="money"></param>
        /// <param name="hbtype"></param>
        /// <param name="total_num"></param>
        /// <param name="sendtype"></param>
        /// <param name="xmlinfo"></param>
        /// <param name="systemState"></param>
        /// <param name="msgresult"></param>
        /// <returns></returns>
        public static bool RedPackSendSN(HttpContext current, string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num, string sendtype, string xmlinfo, out string systemState, out string msgresult)
        {
            DateTime dtAmStart = DateTime.Now.ToUniversalTime();

            //KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSendInt IN [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery-RedPackSend IN[current:" + ((current == null) ? "为空" : "不为空") + "] [ip:" + WebHelper.GetClientIP(current) + "] [ApplicationPath:" + current.Request.PhysicalApplicationPath + "] [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);


            bool bRet = false;
            systemState = "000000";
            msgresult = "失败";
            string strhbtype = "01";//0 普通红包，1 裂变红包  2 查询红包

            KMSLotterySystemFront.Common.CharHelper chelp = new CharHelper();
            PubInfo info = new PubInfo();
            LotteryBusiness lb = new LotteryBusiness();

            if (sendtype == "3")//分享红包
            {
                //用户每次分享，向点击表中插入一条分享数据
                string lid = GetLidByXML(xmlinfo);
                lb.AddHomePageRecord(facId, openId, lid, "1", "1", "4");

                //校验当月是否已经发放过“分享奖励”红包

                bool b = false;
                if (b)
                {
                    systemState = "200100";//传入参数为空
                    msgresult = "传入参数为空";
                    return bRet;

                }


                //获取数据库配置金额
                DataTable dt_base = info.GetBaseDataByDataType(facId, "ShareMoneyLimit");
                if (dt_base != null && dt_base.Rows.Count > 0)
                {
                    money = dt_base.Rows[0]["LOTTERYMOENY"].ToString();
                }



            }

            #region 参数校验

            if (sendtype == "1")//扫码奖励-要检验code
            {

                if (!KMSLotterySystemFront.Common.Utility.CheckParamIsEmpty(facId, ccnactivityid, openId, money, hbtype, total_num, code))
                {
                    systemState = "200100";//传入参数为空
                    msgresult = "传入参数为空";
                    return bRet;
                }

                #region 解码操作

                #region 是否是二维码
                string qrcode = "";
                IDecoderAble cd = null;
                if (RegexExpress.IsQRCode(code))
                {
                    try
                    {

                        qrcode = code;
                        qrcode = System.Web.HttpUtility.UrlDecode(qrcode);
                        cd = CCN.Code2D.CodeManager.CreateGetDecoder(qrcode);
                        if (cd.Success)
                        {
                            code = cd.DecodeEntity.Digit;
                        }
                        else
                        {
                            Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.RedPackSend.cs :LotteryByACCodeSN-【codeQR:" + qrcode + "】【message:" + cd.ErrorMsg + "】", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                            systemState = "200106";//传入参数为空
                            msgresult = "数码错误";
                            return bRet;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.RedPackSend.cs :LotteryByACCodeSN-【codeQR:" + qrcode + "】【message:" + cd.ErrorMsg + "】", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        systemState = "200106";//传入参数为空
                        msgresult = "数码错误";
                        return bRet;
                    }
                }
                #endregion
                #endregion


                //检验数码是否领取过红包,如果领取过,将不能重复领取
                if (info.CheckCodeGetHB2(facId, ccnactivityid, code))
                {
                    systemState = "200107";//
                    msgresult = "当前用户已经领取红包（不能重复发放）";
                    return bRet;
                }
            }
            else//奖励红包，分享红包
            {
                if (!KMSLotterySystemFront.Common.Utility.CheckParamIsEmpty(facId, ccnactivityid, openId, money, hbtype, total_num))
                {
                    systemState = "200100";//传入参数为空
                    msgresult = "传入参数为空";
                    return bRet;
                }
                if (sendtype == "3")//分享红包
                {
                    //当是分享红包时,校验分享红包规则,一个月发放一次,还是此活动只发放一次.
                    string ShareMoneyDateLimitflag = "NN";//默认此活动发放一次
                    int ShareMoneyDateLimitNum = 1;
                    DataTable dt_sm = info.GetBaseDataByDataType(facId, "ShareMoneyDateLimit");
                    if (dt_sm != null && dt_sm.Rows.Count > 0)
                    {
                        ShareMoneyDateLimitflag = dt_sm.Rows[0]["CODEID"].ToString();
                        ShareMoneyDateLimitNum = int.Parse(dt_sm.Rows[0]["LOTTERYMOENY"].ToString());
                    }
                    if (lb.CheckShareRedPackDateLimit(facId, ccnactivityid, openId, "3", ShareMoneyDateLimitflag, ShareMoneyDateLimitNum))
                    {
                        systemState = "200112";
                        msgresult = "分享奖励已经达上限";
                        return bRet;
                    }
                }

            }
            if (!strhbtype.Contains(hbtype))
            {
                systemState = "200112";//
                msgresult = "传入参数为空-红包发放类型不正确";
                return bRet;
            }
            else if (!Common.RegexExpress.IsValidDEMICAL(money))//发放金额不正确
            {
                systemState = "200101";//发放金额不正确
                msgresult = "发放金额不正确";
                return bRet;
            }
            else
            {

            }
            //判断参数长度 
            //校验数据格式
            #endregion

            #region 红包发放类别
            if (hbtype == "0")
            {
                total_num = "1";//普通红包数量为1

                if (Convert.ToDecimal(money) > 200 || Convert.ToDecimal(money) < 1)
                {
                    systemState = "200102";//发放金额不在1-200元之间
                    msgresult = "普通红包发放金额必须在1-200元之间";
                    return bRet;
                }

            }
            else//裂变红包   ，数量大于一  小于20
            {

                //裂变红包金额中，发放红包的最大数量控制
                int maxgroupnum = 20;//此处调取数据库配置
                decimal averagemoney = Convert.ToDecimal(money) / Convert.ToInt32(total_num);
                if (averagemoney < 1 || averagemoney > 200)
                {
                    systemState = "200103";//裂变红包中，平均单个红包金额必须在1-200之间
                    msgresult = "裂变红包中，平均单个红包金额必须在1-200之间";
                    return bRet;
                }
                else if (Convert.ToInt32(total_num) <= 1 || Convert.ToInt32(total_num) > 20)
                {
                    systemState = "200104";//裂变红包数量最大必须为2-20之间
                    msgresult = "裂变红包数量最大必须为2-20之间";
                    return bRet;
                }
                else if (Convert.ToDecimal(money) > 1000 || Convert.ToDecimal(money) < 1)//默认裂变红包金额在1-1000元之间
                {
                    systemState = "200105";//默认裂变红包金额在1-1000元之间
                    msgresult = "默认裂变红包金额在1-1000元之间";
                    return bRet;
                }
            }

            #endregion

            // string NewGuid = chelp.getGuid();
            string redpackguid = chelp.getGuid();//红包发放日志记录表
            string wxpay_guid = chelp.getGuid();//调取红包接口日志记录表

            bool issendrp = false;
            try
            {
                // string ipServer = WebHelper.GetClientIP(HttpContext.Current);
                string ipServer = WebHelper.GetClientIP(current);
                #region 判断获取配置
                WechatRedPack redpackinfo = null;
                if (info.GetRedPackConfigNew(facId, ccnactivityid, hbtype, out systemState, out redpackinfo))
                {
                    redpackinfo.WEIXIN_RE_OPENID = openId;
                    redpackinfo.WEIXIN_CLIENT_IP = ipServer;
                    redpackinfo.WEIXIN_TOTAL_AMOUNT = (double.Parse(money) * 100).ToString();
                    redpackinfo.WEIXIN_GUID = redpackguid;
                    redpackinfo.WEIXIN_CODEID = code;
                    redpackinfo.WEIXIN_LOTTERYID = "";
                    redpackinfo.WEIXIN_TOTAL_NUM = total_num;
                    redpackinfo.WEIXIN_HB_TYPE = hbtype;
                    issendrp = true;

                }
                #endregion

                #region 红包发放
                if (issendrp)
                {

                    int aaaaaa = redpackinfo.WEIXIN_SEND_NAME.Length;

                    byte[] bytese = new byte[aaaaaa];
                    int aa = bytese.Length;

                    string newmoney = (double.Parse(money) * 100).ToString();   //单位：分
                    WxPayResult wxpayresult = new WxPayResult();
                    WxPayData data = new WxPayData();
                    string mch_billno = WxPayApi.GetMch_billno(redpackinfo.WEIXIN_MCH_ID);
                    data.SetValue("mch_billno", mch_billno); //商户订单号
                    data.SetValue("mch_id", redpackinfo.WEIXIN_MCH_ID);  //  商户号
                    data.SetValue("wxappid", redpackinfo.WEIXIN_APPID); //  公众账号appid
                    data.SetValue("send_name", redpackinfo.WEIXIN_SEND_NAME);   //  商户名称
                    data.SetValue("re_openid", redpackinfo.WEIXIN_RE_OPENID);  //  用户openid
                    data.SetValue("total_amount", newmoney);  //付款金额
                    data.SetValue("total_num", total_num);  //  红包发放总人数
                    data.SetValue("wishing", redpackinfo.WEIXIN_WISHING);   //   红包祝福语
                    data.SetValue("client_ip", ipServer);  //Ip地址
                    data.SetValue("act_name", redpackinfo.WEIXIN_ACT_NAME);  //   活动名称
                    data.SetValue("remark", redpackinfo.WEIXIN_REMARK);  //  备注
                    data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());  //  随机字符串

                    if (hbtype == "1")//裂变红包
                    {
                        data.SetValue("amt_type", "ALL_RAND");  //  每人领取的金额随机
                    }

                    data.SetValue("sign", data.MakeSign(redpackinfo.WEIXIN_CERTIFICATESECRET));// 签名
                    string xml = data.ToXml();
                    KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSend IN [xml:" + xml + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    string response = "";
                    if (hbtype == "0")
                    {

                        response = Common.WxPay.WxPayHttpService.Post(current, xml, WxPayConfig.CashRedPackUrl, true, redpackinfo.WEIXIN_CERTIFICATEPATH, redpackinfo.WEIXIN_CERTPASS, 6);
                    }
                    else
                    {
                        response = Common.WxPay.WxPayHttpService.Post(current, xml, WxPayConfig.GroupRedPackUrl, true, redpackinfo.WEIXIN_CERTIFICATEPATH, redpackinfo.WEIXIN_CERTPASS, 6);
                    }

                    KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSend  OUT [xml:" + response + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                    redpackinfo.WEIXIN_MCH_BILLNO = mch_billno;
                    wxpayresult.hb_type = hbtype;
                    wxpayresult.total_num = total_num;
                    wxpayresult.ccnactivityid = ccnactivityid;
                    wxpayresult.ccnactivityname = redpackinfo.WEIXIN_SEND_NAME;
                    wxpayresult.facid = facId;
                    wxpayresult.codeid = code;
                    wxpayresult.ip = ipServer;
                    wxpayresult.mch_billno = mch_billno;
                    wxpayresult.mch_id = redpackinfo.WEIXIN_MCH_ID;
                    wxpayresult.wxappid = redpackinfo.WEIXIN_APPID;
                    wxpayresult.re_openid = redpackinfo.WEIXIN_RE_OPENID;
                    wxpayresult.total_amount = (double.Parse(money) * 100).ToString();
                    wxpayresult.guid = wxpay_guid;

                    //解析返回的数据
                    if (!string.IsNullOrEmpty(response))
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);
                            XmlElement rootElement = doc.DocumentElement;
                            wxpayresult.return_code = rootElement.SelectSingleNode("return_code").InnerText;
                            wxpayresult.result_code = rootElement.SelectSingleNode("result_code").InnerText;
                            wxpayresult.return_msg = rootElement.SelectSingleNode("return_msg").InnerText;
                            wxpayresult.err_code = rootElement.SelectSingleNode("err_code").InnerText;
                            wxpayresult.err_code_des = rootElement.SelectSingleNode("err_code_des").InnerText;
                            wxpayresult.mch_billno = rootElement.SelectSingleNode("mch_billno").InnerText;
                            wxpayresult.mch_id = rootElement.SelectSingleNode("mch_id").InnerText;
                            wxpayresult.wxappid = rootElement.SelectSingleNode("wxappid").InnerText;
                            wxpayresult.re_openid = rootElement.SelectSingleNode("re_openid").InnerText;
                            wxpayresult.total_amount = rootElement.SelectSingleNode("total_amount").InnerText;
                            if (wxpayresult.return_code.ToUpper() == "SUCCESS" && wxpayresult.result_code.ToUpper() == "SUCCESS")
                            {
                                wxpayresult.send_listid = rootElement.SelectSingleNode("send_listid").InnerText;//红包订单的微信单号
                                redpackinfo.WEIXIN_SEND_LISTID = rootElement.SelectSingleNode("send_listid").InnerText;//红包订单的微信单号
                                systemState = "100100";//发放成功
                                msgresult = "发放成功";
                            }
                            else
                            {
                                systemState = "100200";//发放失败
                                msgresult = wxpayresult.err_code_des;
                                //systemState = wxpayresult.result_code;//发放失败
                                //msgresult = wxpayresult.err_code_des;//红包接口发放失败具体原因
                            }
                            redpackinfo.WEIXIN_MCH_BILLNO = wxpayresult.mch_billno;
                            redpackinfo.WEIXIN_STATE = wxpayresult.result_code;
                            redpackinfo.WEIXIN_STATEDETAIL = wxpayresult.err_code_des;
                            //额外附加信息
                            wxpayresult.hb_type = hbtype;
                            wxpayresult.total_num = total_num;
                            wxpayresult.ccnactivityid = ccnactivityid;
                            wxpayresult.ccnactivityname = redpackinfo.WEIXIN_SEND_NAME;
                            wxpayresult.facid = facId;
                            wxpayresult.codeid = "";
                            wxpayresult.ip = ipServer;
                        }
                        catch (Exception ex)
                        {
                            KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSend 红包发放解析返回的xml异常:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                            systemState = "200108";//支付异常
                            msgresult = "支付异常";

                        }
                    }
                    else
                    {
                        redpackinfo.WEIXIN_STATE = "失败";
                        redpackinfo.WEIXIN_STATEDETAIL = "调取红包接口,无数据返回,支付异常";
                        wxpayresult.return_code = "FAIL";
                        wxpayresult.result_code = "红包调用异常";
                        systemState = "200108";//支付异常
                        msgresult = "支付异常";
                    }

                    bRet = info.AddRedPackLog(redpackinfo, wxpayresult);//记录日志

                    //记录红包发放明细日志
                    bool bRet2 = lb.InsertIntoHbDetail(facId, ccnactivityid, redpackinfo.WEIXIN_SEND_NAME, sendtype, code, (double.Parse(money) * 100).ToString(), wxpayresult.result_code, wxpayresult.err_code_des, wxpayresult.mch_billno, wxpayresult.send_listid, xmlinfo);


                }
                else
                {
                    if (systemState == "200109")
                    {
                        systemState = "200109";
                        msgresult = "红包发放配置信息已经失效";
                    }
                    else if (systemState == "200110")
                    {
                        systemState = "200110";
                        msgresult = "红包发放活动不在有效时间范围内";
                    }
                    else
                    {
                        //无配置信息
                        systemState = "200111";//无配置信息（）
                        msgresult = "红包发放无配置信息";
                    }
                }
                #endregion
                //记录红包发放日志
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSend 红包发放异常:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                systemState = "200108";//支付异常
                msgresult = "支付异常";

            }
            DateTime dtNightEnd = DateTime.Now.ToUniversalTime();
            System.TimeSpan ts = dtNightEnd.Subtract(dtAmStart);
            KMSLotterySystemFront.Logger.AppLog.Write("WxPayService.asmx-RedPackSend   OUT [facId:" + facId + "] [ccnactivityid:" + ccnactivityid + "] [openId:" + openId + "] [money:" + money + "] [systemState:" + systemState + "] T" + ts.Seconds.ToString() + "-" + ts.Milliseconds.ToString(), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            return bRet;
        }
        #endregion


        #region 读取xml信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string GetLidByXML(string xml)
        {
            string lid = "";

            #region 解析xml
            if (!string.IsNullOrEmpty(xml))
            {
                xml = xml.Replace("&", "");
                if (xml.Contains("Info"))
                {
                    XmlNodeList nodeList = XmlHelperNew.GetXmlNodeList(xml, "//Info");

                    if (nodeList != null && nodeList.Count > 0)
                    {
                        foreach (XmlNode node in nodeList)
                        {
                            if (node != null)
                            {
                                foreach (XmlNode childNode in node.ChildNodes)
                                {
                                    if (childNode.Name.ToString().Trim().ToUpper().Equals("I_LOTTERYGUID"))
                                    {
                                        lid = childNode.InnerText.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            return lid;

        }
        #endregion
    }
}
