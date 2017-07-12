using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using KMSLotterySystemFront.Model;
using KMSLotterySystemFront.DAL;
using KMSLotterySystemFront.Common;
using System.Collections;


namespace KMSLotterySystemFront.BLLLottery
{
    public class PubInfo
    {

        PubInfoDao dao = new PubInfoDao();

        public string getGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }


        /// <summary>
        /// 解析即买即对数据
        /// </summary>
        /// <param name="strJson">被解析数据</param>
        /// <param name="wc">版本号</param>
        /// <returns></returns>
        private bool AnalyzeJson(string strJson, out WechatClickEntity wc)
        {
            wc = null;
            try
            {
                dynamic ja = JsonConvert.DeserializeObject(strJson);

                wc = new WechatClickEntity();
                wc.OPENID = ja["openid"].ToString();
                wc.COUNTRY = ja["country"].ToString();
                wc.PROVINCE = ja["province"].ToString();
                wc.CITY = ja["city"].ToString();
                wc.NICKNAME = ja["nickname"].ToString();
                wc.SEX = ja["sex"].ToString();
                wc.IP = ja["ip"].ToString();
                wc.LOTTERYGUID = ja["lotteryguid"].ToString();
                wc.TYPE = ja["type"].ToString();
                wc.GUID = getGuid();
                wc.FACID = ja["facid"].ToString();
                return true;

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AnalyzeJson msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        /// <summary>
        /// 解析即买即对数据
        /// </summary>
        /// <param name="strJson">被解析数据</param>
        /// <param name="wc">版本号</param>
        /// <returns></returns>
        private bool AnalyzeJsonNew(string strJson, out WechatClickEntity wc)
        {
            wc = null;
            try
            {
                dynamic ja = JsonConvert.DeserializeObject(strJson);

                wc = new WechatClickEntity();
                wc.OPENID = ja["openid"].ToString();
                wc.COUNTRY = ja["country"].ToString();
                wc.PROVINCE = ja["province"].ToString();
                wc.CITY = ja["city"].ToString();
                wc.NICKNAME = ja["nickname"].ToString();
                wc.SEX = ja["sex"].ToString();
                wc.IP = ja["ip"].ToString();
                wc.LOTTERYGUID = ja["lotteryguid"].ToString();
                wc.TYPE = ja["type"].ToString();
                wc.GUID = getGuid();
                wc.FACID = ja["facid"].ToString();
                wc.CHANNEL = ja["channel"].ToString();
                wc.LINETYPE = ja["linetype"].ToString();
                return true;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AnalyzeJsonNew msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 解析微信授权用户数据
        /// </summary>
        /// <param name="strJson">被解析数据</param>
        /// <param name="wc">输出:微信数据实体</param>
        /// <returns></returns>
        private bool AnalyzeJsonAuthWechatUser(string strJson, string factoryid, out WechatUserEntity wc)
        {
            wc = null;
            try
            {
                dynamic ja = JsonConvert.DeserializeObject(strJson);

                wc = new WechatUserEntity();
                wc.openid = ja["openid"].ToString();
                wc.country = ja["country"].ToString();
                wc.province = ja["province"].ToString();
                wc.city = ja["city"].ToString();
                wc.nickname = ja["nickname"].ToString();
                wc.sex = ja["sex"].ToString();
                wc.guid = getGuid();
                wc.facid = factoryid;
                wc.headimgurl = ja["headimgurl"].ToString();
                wc.unionid = ja["unionid"].ToString();

                return true;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AnalyzeJsonAuthWechatUser msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="lid"></param>
        /// <param name="digitcode"></param>
        /// <param name="money"></param>
        /// <param name="systemstate"></param>
        /// <returns></returns>
        private bool AnalyzeJson(string strJson, out string facid, out string openid, out string lid, out string digitcode, out string systemstate)
        {
            facid = "";
            openid = "";
            lid = "";
            digitcode = "";
            systemstate = "000";
            try
            {
                dynamic ja = JsonConvert.DeserializeObject(strJson);

                facid = ja["facid"].ToString();
                openid = ja["openid"].ToString();
                lid = ja["lid"].ToString();
                digitcode = ja["digitcode"].ToString();
                systemstate = "001";
                return true;

            }
            catch (Exception ex)
            {
                systemstate = "005";
                Logger.AppLog.Write(" AnalyzeJson msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        #region 解析传入的json字符串

        /// <summary>
        /// 解析传入的json字符串
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="lid"></param>
        /// <param name="digitcode"></param>
        /// <param name="money"></param>
        /// <param name="systemstate"></param>
        /// <returns></returns>
        public bool AnalyzeJson(string strJson, out string facid, out string openid, out string lid, out string digitcode, out string money, out string systemstate)
        {
            facid = "";
            openid = "";
            lid = "";
            digitcode = "";
            money = "1";//默认为1元
            systemstate = "000";
            try
            {
                dynamic ja = JsonConvert.DeserializeObject(strJson);

                facid = ja["facid"].ToString();
                openid = ja["openid"].ToString();
                lid = ja["lid"].ToString();
                digitcode = ja["digitcode"].ToString();
                money = ja["money"].ToString();
                systemstate = "001";
                return true;

            }
            catch (Exception ex)
            {
                systemstate = "005";
                Logger.AppLog.Write(" AnalyzeJson msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }



        /// <summary>
        /// 解析传入的json字符串AnalyzeJsonHb
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="lid"></param>
        /// <param name="digitcode"></param>
        /// <param name="ccnactivityid"></param>
        /// <param name="systemstate"></param>
        /// <returns></returns>
        public bool AnalyzeJsonHb(string strJson, out string facid, out   string ccnactivityid, out string openid, out string lid, out string digitcode, out string systemstate)
        {
            facid = "";
            openid = "";
            lid = "";
            digitcode = "";
            ccnactivityid = "";//内部活动编号
            systemstate = "000";
            try
            {
                dynamic ja = JsonConvert.DeserializeObject(strJson);

                facid = ja["facid"].ToString();
                openid = ja["openid"].ToString();
                lid = ja["lid"].ToString();
                digitcode = ja["digitcode"].ToString();
                ccnactivityid = ja["ccnactivityid"].ToString();
                systemstate = "001";
                return true;

            }
            catch (Exception ex)
            {
                systemstate = "005";
                Logger.AppLog.Write(" AnalyzeJsonHb msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 输入参数
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public string GetLuckyMoney(string strJson, out string facid, out string openid, out string lid, out string digitcode, out string lotterylive, out string Money)
        {
            facid = "";
            openid = "";
            lid = "";
            digitcode = "";
            lotterylive = "";
            Money = "";
            string sRet = string.Empty;

            string systemstate = string.Empty;
            string luckymoney = string.Empty;

            try
            {
                if (AnalyzeJson(strJson, out facid, out openid, out lid, out digitcode, out systemstate))
                {

                    Logger.AppLog.Write(" GetLuckyMoney strJson[:" + strJson + "]---- Json 解析成功", Logger.AppLog.LogMessageType.Info);

                    #region 逻辑判断
                    if (string.IsNullOrEmpty(facid))
                    {
                        return Get_Rstr_OnlyStatus("100", "", "0");
                    }
                    if (string.IsNullOrEmpty(openid))
                    {
                        return Get_Rstr_OnlyStatus("101", "", "0");
                    }
                    if (string.IsNullOrEmpty(lid))
                    {
                        return Get_Rstr_OnlyStatus("102", "", "0");
                    }
                    if (string.IsNullOrEmpty(digitcode) || !RegexExpress.IsDigitCode16(digitcode))
                    {
                        return Get_Rstr_OnlyStatus("103", "", "0");
                    }

                    ControlBLL cbll = new ControlBLL();
                    LotteryBLL lbll = new LotteryBLL();

                    ActivityInfo ayinfo = null;

                    //判断活动总控
                    if (!cbll.GetService(facid, "X", out ayinfo))
                    {
                        return Get_Rstr_OnlyStatus("104", "", "0");
                    }

                    string newpoolid = string.Empty;

                    //判断活动是否开始
                    if (!cbll.GetActivityByAID(facid, ayinfo.Activityid, "2", out newpoolid))
                    {
                        return Get_Rstr_OnlyStatus("105", "", "0");
                    }

                    //判断活动是否结束
                    if (!cbll.GetActivityByAID(facid, ayinfo.Activityid, "0", out newpoolid))
                    {
                        return Get_Rstr_OnlyStatus("106", "", "0");
                    }


                    //判断openid+code+lid 是否存在中奖

                    DataTable lmdt = dao.getLuckyMoney(facid, openid, lid, digitcode);

                    if (lmdt != null && lmdt.Rows.Count > 0)
                    {
                        DataRow[] rowConfig = lmdt.Select("STATE IN ('1','6')");

                        if (!(rowConfig != null && rowConfig.Length == 1))
                        {
                            return Get_Rstr_OnlyStatus("108", "", "0");
                        }
                    }
                    else //没有找到中奖纪录
                    {
                        return Get_Rstr_OnlyStatus("107", "", "0");
                    }

                    DataTable hbtable = null;

                    //没有奖池
                    if (!dao.GetHB(facid, out hbtable))
                    {
                        return Get_Rstr_OnlyStatus("109", "", "0");
                    }

                    #endregion

                    int totalNum = 0;//当前参与总人数
                    int lotteryNumber = 99999;//奖项中奖数量
                    int awardscale = 999999;//中奖比例阈值
                    string awardscode = "0";//奖项编号
                    int totalTimes = 0;//本期奖项总数

                    totalNum = Convert.ToInt32(hbtable.Compute("SUM(LOTTERYTIMES)", "true"));


                    #region 红包抽奖逻辑
                    foreach (DataRow drt in hbtable.Rows)
                    {
                        awardscode = drt["LOTTERYID"].ToString().Trim();//奖项编号
                        totalTimes = Convert.ToInt32(drt["TOTALTIMES"].ToString());//本期奖项总数 (判断合并后20元总数)
                        lotteryNumber = Convert.ToInt32(drt["LOTTERYTIMES"].ToString());//奖项中奖数量 (记录已合并20元总数)
                        awardscale = Convert.ToInt32(drt["AWARDSCALE"].ToString());//中奖比例阈值


                        string awasName = string.Empty;

                        if (((totalNum + 1) % awardscale == 0) && (lotteryNumber < totalTimes))
                        {
                            awasName = cbll.GetLotteryName(facid, awardscode, "HBLotteryType");

                            ///获取中奖充值金额
                            Money = cbll.GetLotteryName2(facid, awardscode, "HBLotteryType", "LOTTERYMOENY");


                            int i = dao.ModifyLucklyMoeny(facid, openid, lid, digitcode, Money, "6", "1");

                            if (i > 0)
                            {
                                lotterylive = awardscode;
                                return Get_Rstr_OnlyStatus("001", openid, Money.ToString());
                            }
                            else
                            {
                                return Get_Rstr_OnlyStatus("002", "", "0");
                            }
                        }
                    }
                    return Get_Rstr_OnlyStatus("002", "", "0");
                    #endregion
                }
                else
                {
                    return Get_Rstr_OnlyStatus("004", "", "0");
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AddClickLog msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return Get_Rstr_OnlyStatus("005", "", "0");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="lid"></param>
        /// <param name="digitcode"></param>
        /// <param name="lotterylive"></param>
        /// <param name="systemstate"></param>
        /// <returns></returns>
        public bool ModifyLuckyMoney(string facid, string openid, string lid, string digitcode, string lotterylive, out string systemstate)
        {
            systemstate = "000";
            try
            {
                return dao.ModifyLucklyMoeny2(facid, openid, lid, digitcode, "9", "6", lotterylive);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" ModifyLuckyMoney msg [facid:" + facid + "] [openid:" + openid + "] [lid:" + lid + "] [digitcode:" + digitcode + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                systemstate = "005";
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="activityid"></param>
        /// <param name="systemstate"></param>
        /// <param name="redpackinfo"></param>
        /// <returns></returns>
        public bool GetRedPackConfig(string facid, string activityid, out string systemstate, out WechatRedPack redpackinfo)
        {
            systemstate = "000";
            redpackinfo = null;
            bool bRet = false;
            try
            {
                DataTable rdTable = dao.GetRedPackConfig(facid, activityid);

                if (rdTable != null && rdTable.Rows.Count > 0)
                {
                    string begintime = rdTable.Rows[0]["BEGINTIME"].ToString();
                    string endtime = rdTable.Rows[0]["ENDTIME"].ToString();
                    string deleteflag = rdTable.Rows[0]["DELETEFLAG"].ToString();

                    if (deleteflag.Equals("0"))
                    {
                        systemstate = "102";
                        return bRet;
                    }
                    if (!string.IsNullOrEmpty(rdTable.Rows[0]["BEGINTIME"].ToString()) && !string.IsNullOrEmpty(rdTable.Rows[0]["ENDTIME"].ToString()))
                    {
                        if (!(DateTime.Now >= Convert.ToDateTime(rdTable.Rows[0]["BEGINTIME"].ToString()) && DateTime.Now <= Convert.ToDateTime(rdTable.Rows[0]["ENDTIME"].ToString())))
                        {
                            systemstate = "103";
                            return bRet;
                        }
                    }

                    redpackinfo = new WechatRedPack();

                    redpackinfo.WEIXIN_ACT_NAME = rdTable.Rows[0]["ACTNAME"].ToString();
                    redpackinfo.WEIXIN_APPID = rdTable.Rows[0]["APPID"].ToString();
                    redpackinfo.WEIXIN_LOGO_IMGURL = rdTable.Rows[0]["LOGOIMGURL"].ToString();
                    redpackinfo.WEIXIN_MCH_ID = rdTable.Rows[0]["MCHID"].ToString();
                    redpackinfo.WEIXIN_SEND_NAME = rdTable.Rows[0]["SENDNAME"].ToString();
                    redpackinfo.WEIXIN_NICK_NAME = rdTable.Rows[0]["NICKNAME"].ToString();
                    redpackinfo.WEIXIN_REMARK = rdTable.Rows[0]["REMARK"].ToString();
                    redpackinfo.WEIXIN_SHARE_CONTENT = rdTable.Rows[0]["SHARECONTENT"].ToString();
                    redpackinfo.WEIXIN_SHARE_IMGURL = rdTable.Rows[0]["SHAREIMGURL"].ToString();
                    redpackinfo.WEIXIN_SHARE_URL = rdTable.Rows[0]["SHAREURL"].ToString();
                    redpackinfo.WEIXIN_WISHING = rdTable.Rows[0]["WISHING"].ToString();
                    redpackinfo.WEIXIN_CERTIFICATESECRET = rdTable.Rows[0]["CERTIFICATESECRET"].ToString();
                    redpackinfo.WEIXIN_CERTIFICATEPATH = rdTable.Rows[0]["CERTIFICATEPATH"].ToString();

                    redpackinfo.WEIXIN_ACTIVITYID = rdTable.Rows[0]["CCNACTIVITYID"].ToString();
                    redpackinfo.WEIXIN_ACTIVITYNAME = rdTable.Rows[0]["CCNACTIVITYNAME"].ToString();
                    redpackinfo.WEIXIN_FACID = rdTable.Rows[0]["FACID"].ToString();

                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" GetRedPackConfig msg [facid:" + facid + "] [activityid:" + activityid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                systemstate = "005";
                return bRet;
            }
            return bRet;
        }


        #region 读取发放红包配置表
        /// <summary>
        /// 读取发放红包配置表
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="activityid"></param>
        /// <param name="hb_type">红包类型</param>
        /// <param name="systemstate"></param>
        /// <param name="redpackinfo"></param>
        /// <returns></returns>
        public bool GetRedPackConfigNew(string facid, string activityid, string hb_type, out string systemstate, out WechatRedPack redpackinfo)
        {

            /*
             hb_type=0  发放普通红包
             hb_type=1  发放裂变红包
             hb_type=2  查询红包（普通和裂变）/查询企业付款
             hb_type=3  企业付款
             * 
             * 
             
            因为要调取红包配置表，由于不同的业务需求，查询红包配置表的条件不一样
             例如： 1.当发放红包时，要校验是否有配置信息，是否在活动期间之内
             *      2.当查询红包时，只需校验配置表中是否有配置信息，无需校验是否可用，以及是否在活动时间之内
             */

            systemstate = "000";
            redpackinfo = null;
            bool bRet = false;
            try
            {
                DataTable rdTable = dao.GetRedPackConfigNew(facid, activityid);

                if (rdTable != null && rdTable.Rows.Count > 0)
                {
                    string begintime = rdTable.Rows[0]["BEGINTIME"].ToString();
                    string endtime = rdTable.Rows[0]["ENDTIME"].ToString();
                    string deleteflag = rdTable.Rows[0]["DELETEFLAG"].ToString();


                    if (hb_type == "2")//红包查询
                    {

                    }
                    else //红包发放（normal ，group ）
                    {
                        if (deleteflag.Equals("0"))//配置已经不可用
                        {
                            systemstate = "200109";
                            return bRet;
                        }
                        if (!string.IsNullOrEmpty(rdTable.Rows[0]["BEGINTIME"].ToString()) && !string.IsNullOrEmpty(rdTable.Rows[0]["ENDTIME"].ToString()))
                        {
                            if (!(DateTime.Now >= Convert.ToDateTime(rdTable.Rows[0]["BEGINTIME"].ToString()) && DateTime.Now <= Convert.ToDateTime(rdTable.Rows[0]["ENDTIME"].ToString())))
                            {
                                systemstate = "200110"; //不在活动参与返回之内
                                return bRet;
                            }
                        }
                    }



                    redpackinfo = new WechatRedPack();
                    redpackinfo.WEIXIN_ACT_NAME = rdTable.Rows[0]["ACTNAME"].ToString();
                    redpackinfo.WEIXIN_APPID = rdTable.Rows[0]["APPID"].ToString();
                    redpackinfo.WEIXIN_LOGO_IMGURL = rdTable.Rows[0]["LOGOIMGURL"].ToString();
                    redpackinfo.WEIXIN_MCH_ID = rdTable.Rows[0]["MCHID"].ToString();
                    redpackinfo.WEIXIN_SEND_NAME = rdTable.Rows[0]["SENDNAME"].ToString();
                    redpackinfo.WEIXIN_NICK_NAME = rdTable.Rows[0]["NICKNAME"].ToString();
                    redpackinfo.WEIXIN_REMARK = rdTable.Rows[0]["REMARK"].ToString();
                    redpackinfo.WEIXIN_SHARE_CONTENT = rdTable.Rows[0]["SHARECONTENT"].ToString();
                    redpackinfo.WEIXIN_SHARE_IMGURL = rdTable.Rows[0]["SHAREIMGURL"].ToString();
                    redpackinfo.WEIXIN_SHARE_URL = rdTable.Rows[0]["SHAREURL"].ToString();
                    redpackinfo.WEIXIN_WISHING = rdTable.Rows[0]["WISHING"].ToString();
                    redpackinfo.WEIXIN_CERTIFICATESECRET = rdTable.Rows[0]["CERTIFICATESECRET"].ToString();
                    redpackinfo.WEIXIN_CERTIFICATEPATH = rdTable.Rows[0]["CERTIFICATEPATH"].ToString();//证书存放绝对路径
                    redpackinfo.WEIXIN_ACTIVITYID = rdTable.Rows[0]["CCNACTIVITYID"].ToString();
                    redpackinfo.WEIXIN_ACTIVITYNAME = rdTable.Rows[0]["CCNACTIVITYNAME"].ToString();
                    redpackinfo.WEIXIN_FACID = rdTable.Rows[0]["FACID"].ToString();
                    redpackinfo.WEIXIN_CERTPASS = rdTable.Rows[0]["CERTPASS"].ToString();//证书密码
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" GetRedPackConfigNew msg [facid:" + facid + "] [activityid:" + activityid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                systemstate = "005";
                return bRet;
            }
            return bRet;
        }

        #endregion



        #region 1.1检测当前中奖数码是否已经成功领取
        /// <summary>
        /// 检测当前中奖数码是否已经成功领取
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="ccnactivityid"></param>
        ///  <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="lid"></param>
        /// <returns></returns>
        public bool CheckCodeGetHB(string facid, string ccnactivityid, string openid, string code, string lid)
        {
            bool bRet = false;
            try
            {
                bRet = dao.CheckCodeGetHB(facid, ccnactivityid, openid, code, lid);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.PubInfo.cs-CheckCodeGetHB  IN[facid:" + facid + "] [ccnactivityid:" + ccnactivityid + "][openid:" + openid + "][code:" + code + "][lid:" + lid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion

        #region 1.2检测当前中奖数码是否已经成功领取（重载）
        /// <summary>
        /// 检测当前中奖数码是否已经成功领取（重载）
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="ccnactivityid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckCodeGetHB2(string facid, string ccnactivityid, string code)
        {
            bool bRet = false;
            try
            {
                bRet = dao.CheckCodeGetHB2(facid, ccnactivityid, code);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.PubInfo.cs-CheckCodeGetHB2  IN[facid:" + facid + "] [ccnactivityid:" + ccnactivityid + "][code:" + code + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion


        #region 检测数码是否中奖（查询中奖记录表）
        /// <summary>
        /// 检测数码是否中奖（查询中奖记录表）
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// <param name="lid">中奖guid</param>
        /// <returns></returns>
        public bool CheckCodeIsLotteryed(string facid, string code, string lid)
        {

            bool bRet = false;
            try
            {
                bRet = dao.CheckCodeIsLotteryed(facid, code, lid);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.PubInfo.cs-CheckCodeIsLotteryed  IN[facid:" + facid + "] [code:" + code + "][lid:" + lid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }


        #endregion





        /// <summary>
        /// 添加活动页面点击参看事件日志
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public string AddClickLog(string strJson)
        {
            string sRet = string.Empty;
            WechatClickEntity wc = null;
            try
            {
                if (AnalyzeJson(strJson, out wc))
                {
                    if (dao.AddClickInfo(wc) > 0)
                    {
                        return Get_Rstr_OnlyStatus("001");
                    }
                    else
                    {
                        return Get_Rstr_OnlyStatus("002");
                    }
                }
                else
                {
                    return Get_Rstr_OnlyStatus("004");
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AddClickLog msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return Get_Rstr_OnlyStatus("005");
            }
        }



        /// <summary>
        /// 添加活动页面点击参看事件日志
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public string AddClickLogNew(string strJson)
        {
            string sRet = string.Empty;
            WechatClickEntity wc = null;
            try
            {
                if (AnalyzeJsonNew(strJson, out wc))
                {
                    if (dao.AddClickInfoNew(wc) > 0)
                    {
                        return Get_Rstr_OnlyStatus("001");
                    }
                    else
                    {
                        return Get_Rstr_OnlyStatus("002");
                    }
                }
                else
                {
                    return Get_Rstr_OnlyStatus("004");
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AddClickLogNew msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return Get_Rstr_OnlyStatus("005");
            }
        }







        /// <summary>
        /// 添加微信授权用户记录
        /// </summary>
        /// <param name="strJson">数据集合</param>
        /// <param name="factoryid">厂家编号</param>
        /// <returns></returns>
        public string AddAuthWechatUser(string strJson, string factoryid)
        {
            string sRet = string.Empty;
            WechatUserEntity wc = null;
            try
            {
                if (AnalyzeJsonAuthWechatUser(strJson, factoryid, out wc))
                {
                    if (dao.AddAuthWechatUser(wc) > 0)
                    {
                        return Get_Rstr_OnlyStatus("001");
                    }
                    else
                    {
                        return Get_Rstr_OnlyStatus("002");
                    }
                }
                else
                {
                    return Get_Rstr_OnlyStatus("004");
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("AddAuthWechatUser msg[:" + strJson + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return Get_Rstr_OnlyStatus("005");
            }
        }

        //公共返回JSON数据,只含用STATUS一个数据的
        public string Get_Rstr_OnlyStatus(string r_status, string openid, string money)
        {
            return "{" + string.Format("\"status\":\"{0}\"", r_status) + "," + string.Format("\"openid\":\"{0}\"", openid) + "," + string.Format("\"money\":\"{0}\"", money) + "}";
        }

        //公共返回JSON数据,只含用STATUS一个数据的
        public string Get_Rstr_OnlyStatus(string r_status)
        {
            return "{" + string.Format("\"Status\":\"{0}\"", r_status) + "}";
        }

        /// <summary>
        /// 添加预约数据
        /// <param name="factoryid">厂家编号</param>
        /// <param name="mobile">预约手机号码</param>
        /// <param name="channel">预约渠道</param>
        /// <param name="username">预约姓名</param>
        /// <returns></returns>
        public bool AddReserve(string factoryid, string mobile, string channel, string username)
        {
            try
            {
                int IRet = dao.AddReserve(factoryid, getGuid(), mobile, channel, username);
                return (IRet > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AddReserve msg [factoryid:" + factoryid + "] [mobile:" + mobile + "] [channel:" + channel + "] [username:" + username + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }

        }


        /// <summary>
        /// 添加预约数据   -壳牌超凡喜力
        /// <param name="factoryid">厂家编号</param>
        /// <param name="mobile">预约手机号码</param>
        /// <param name="channel">预约渠道</param>
        /// <param name="username">预约姓名</param>
        /// <param name="storeid">预约门店</param>
        /// <returns></returns>
        public bool AddReserve(string factoryid, string mobile, string channel, string username, string storeid, string openid, string f1)
        {
            try
            {
                int IRet = dao.AddReserve(factoryid, getGuid(), mobile, channel, username, storeid, openid, f1);
                return (IRet > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AddReserve msg [factoryid:" + factoryid + "] [mobile:" + mobile + "] [channel:" + channel + "] [username:" + username + "]  [storeid:" + storeid + " ]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }

        }




        /// <summary>
        /// 获取抽奖答复信息
        /// <param name="factoryid">厂家编号</param>
        /// <param name="replayid">答复id</param>
        /// <param name="channel">预约渠道</param>
        /// <returns></returns>
        public string GetShakeReplay(string factoryid, string replayid, string channel)
        {
            string replay = "";
            try
            {
                replay = dao.GetShakeReplay(factoryid, replayid, channel);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo.GetShakeReplay msg [factoryid:" + factoryid + "] [channel:" + channel + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                replay = "";
            }
            return replay;

        }





        /// <summary>
        /// 检测门店是否参与活动
        /// <param name="factoryid">厂家编号</param>
        /// <param name="storeid">门店id</param>
        /// <returns></returns>
        public bool CheckStoreIsJoinActivity(string factoryid, string storeid)
        {
            try
            {
                bool bRet = dao.CheckStoreIsJoinActivity(factoryid, storeid);
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" CheckStoreIsJoinActivity msg [factoryid:" + factoryid + "] [storeid:" + storeid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }

        }



        /// <summary>
        /// 检测门店是否参与活动
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="storeid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool CheckStoreIsJoinActivity(string factoryid, string storeid, out DataTable dt)
        {
            bool bRet = false;
            dt = null;

            try
            {
                bRet = dao.CheckStoreIsJoinActivity(factoryid, storeid, out dt);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" CheckStoreIsJoinActivity msg [factoryid:" + factoryid + "] [storeid:" + storeid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;

        }




        /// <summary>
        /// 通过手机号码获取预约用户推荐人OPENID
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="reserveOpenId"></param>
        /// <returns></returns>
        public bool GetReserve(string factoryid, string mobile, out string reserveOpenId)
        {
            reserveOpenId = "";
            bool bRet = false;
            try
            {
                object oOet = dao.GetReserveByMobile(factoryid, mobile);
                if (oOet != null)
                {
                    reserveOpenId = oOet.ToString();
                    bRet = true;
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" AddReserve msg [factoryid:" + factoryid + "] [mobile:" + mobile + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool CheckReserve(string factoryid, string mobile, string channel)
        {
            try
            {
                object bRet = dao.GetReserveByMobile(factoryid, mobile, channel);

                return (bRet != null) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" CheckReserve msg [factoryid:" + factoryid + "] [mobile:" + mobile + "] [channel:" + channel + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        /// <summary>
        /// 检测手机号是否预约过
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <param name="dt">预约信息</param>
        /// <returns></returns>
        public bool CheckReserve(string factoryid, string mobile, string channel, out DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {

                bRet = dao.GetReserveByMobile(factoryid, mobile, channel, out dt);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" CheckReserve msg [factoryid:" + factoryid + "] [mobile:" + mobile + "] [channel:" + channel + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }


        /// <summary>
        /// 添加红包充值记录
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        public bool AddRedPackLog(WechatRedPack redpack)
        {
            int iRet = 0;
            try
            {
                if (redpack != null)
                {
                    iRet = dao.AddRedPackLog(redpack);
                }

                return (iRet > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("AddRedPackLog ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }

        }


        #region 添加红包发放日志，接口调用日志

        /// <summary>
        /// 添加红包发放日志，接口调用日志
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        public bool AddRedPackLog(WechatRedPack redpack, WxPayResult wxpayresult)
        {
            bool iRet = false;
            try
            {
                if (redpack != null && wxpayresult != null)
                {
                    iRet = dao.AddRedPackLog(redpack, wxpayresult);
                }
                return iRet;

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("AddRedPackLog( , ) ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }

        }
        #endregion





        #region 根据商户订单号查询红包信息 -日志记录
        /// <summary>
        ///根据商户订单号查询红包信息 -日志记录
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        public bool AddQueryRedPackLog(QueryHbResult queryhbresult)
        {
            bool iRet = false;
            try
            {
                if (queryhbresult != null)
                {
                    iRet = dao.AddQueryRedPackLog(queryhbresult);
                }
                return iRet;

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("AddQueryRedPackLog() ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion


        #region 根据错误码查询对应的解决方案
        /// <summary>
        /// 根据错误码查询对应的解决方案
        /// </summary>
        /// <param name="errorcode">错误码</param>
        /// <param name="ftype">接口调用类型（父类）</param>
        /// <param name="type">接口调用类型（小类）</param>
        /// <returns></returns>
        public string GetSolutionByErrorCode(string errorcode, string ftype, string type)
        {
            string solution = "";
            try
            {
                solution = dao.GetSolutionByErrorCode(errorcode, ftype, type);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("GetSolutionByErrorCode() ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return solution;
        }
        #endregion




        //获取省市区
        /// <summary>
        ///  获取省市区
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pid">省级编码id</param>
        /// <param name="cid">市级编码id</param>
        /// <returns></returns>
        public DataTable GetArea(string facid, string pid, string cid)
        {

            DataTable dt = null;
            try
            {
                #region 使用缓存
                //if (DataCache.GetCache(facid + "ClientDigitCodeLeng") != null)
                //{
                //    dt = DataCache.GetCache(facid + "ClientDigitCodeLeng") as DataTable;
                //}
                //{
                //    dt = dao.GetArea(facid, pid, cid);
                //    DataCache.SetCache(facid + "ClientDigitCodeLeng", dt);
                //}
                #endregion

                dt = dao.GetArea(facid, pid, cid);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 获取省市区出现异常 GetArea msg [facid:" + facid + "] [pid:" + pid + "]  [cid:" + cid + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }




        //获取省市
        /// <summary>
        ///  获取省市
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pname">省级名称</param>
        /// <returns></returns>
        public DataTable GetAreaNew(string facid, string pname)
        {

            DataTable dt = null;
            try
            {
                #region 使用缓存
                //if (DataCache.GetCache(facid + "SC") != null)    //SC :省 市
                //{
                //    dt = DataCache.GetCache(facid + "SC") as DataTable;
                //}
                //else
                //{
                //    dt = dao.GetAreaNew(facid, pname);
                //    DataCache.SetCache(facid + "SC", dt);
                //}
                #endregion

                // dt = dao.GetArea(facid, pid, cid);

                #region 不使用缓存
                dt = dao.GetAreaNew(facid, pname);
                #endregion


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 获取省市出现异常 GetAreaNew msg [facid:" + facid + "] [pname:" + pname + "]    ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }

        //获取门店所在省市
        /// <summary>
        /// 获取门店所在省市
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pname">省级名称</param>
        /// <returns></returns>
        public string GetAreaNew(string facid)
        {
            string Result = "{}";
            try
            {
                //使用缓存
                if (DataCache.GetCache(facid + "P_C") != null)
                {
                    Result = DataCache.GetCache(facid + "P_C") as String;
                }
                else
                {
                    DataTable dt = dao.GetAreaNew(facid, "");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //组合省json
                        string json_pro = "";
                        string json_city = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            string json_p_city = "";
                            string provincename = dr["STOREPROVINCE"].ToString();
                            if (!string.IsNullOrEmpty(provincename))
                            {
                                json_pro += "{\"text\":\"" + dr["STOREPROVINCE"].ToString() + "\",\"value\":\"" + dr["STOREPROVINCE"].ToString() + "\"},";
                                DataTable dt_city = dao.GetAreaNew(facid, provincename);
                                if (dt_city != null && dt_city.Rows.Count > 0)
                                {
                                    foreach (DataRow dr_c in dt_city.Rows)
                                    {
                                        string storecity = dr_c["STORECITY"].ToString();
                                        if (string.IsNullOrEmpty(storecity))
                                        {
                                            storecity = "";
                                        }
                                        json_p_city += "{\"text\":\"" + storecity + "\",\"value\":\"" + storecity + "\"},";

                                    }
                                    if (json_p_city.Length > 0)
                                    {
                                        json_p_city = json_p_city.Substring(0, json_p_city.Length - 1);
                                    }
                                    json_p_city = "\"" + provincename + "\":[" + json_p_city + "]";
                                }

                                json_city += json_p_city + ",";
                            }

                        }
                        if (json_city.Length > 0)
                        {
                            json_city = json_city.Substring(0, json_city.Length - 1);
                        }

                        if (json_pro.Length > 0)
                        {
                            json_pro = json_pro.Substring(0, json_pro.Length - 1);
                        }

                        Result = "{\"provs_data\":[" + json_pro + "],\"citys_data\":{" + json_city + "}}";

                        DataCache.SetCache(facid + "P_C", Result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 获取省市出现异常 GetAreaNew msg [facid:" + facid + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return Result;

        }







        //获取省市
        /// <summary>
        ///  获取省市
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pname">省级名称</param>
        /// <returns></returns>
        public bool GetAreaList(string facid, string storeProvince, string storeCity, string storeArea, string storeId, out string areaList)
        {
            areaList = string.Empty;
            DataTable dt = null;
            DataTable boundTable = null;
            StringBuilder sb = null;
            bool bRet = false;
            try
            {
                if (string.IsNullOrEmpty(storeProvince) && string.IsNullOrEmpty(storeCity) && string.IsNullOrEmpty(storeArea) && string.IsNullOrEmpty(storeId))
                {
                    #region 获取省份


                    #region 使用缓存
                    if (DataCache.GetCache(facid + "ShellJYZProvince") != null)
                    {
                        dt = DataCache.GetCache(facid + "ShellJYZProvince") as DataTable;
                    }
                    else
                    {
                        dt = dao.GetAreaNew(facid, storeProvince);
                        DataCache.SetCache(facid + "ShellJYZProvince", dt);
                    }
                    #endregion




                    if (dt != null && dt.Rows.Count > 0) //存在数据
                    {
                        sb = new StringBuilder();

                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            ++i;
                            sb.Append("{\"ID\":\"" + i + "\",\"NAME\":\"" + dr[0] + "\"},");
                        }

                        areaList = sb.ToString();
                        if (!string.IsNullOrEmpty(areaList))
                        {
                            bRet = true;
                        }
                    }
                    return bRet;
                    #endregion
                }
                else
                {
                    #region 获取除省份以外的数据

                    #region 使用缓存
                    if (DataCache.GetCache(facid + "Shell") != null)    //SC :省 市
                    {
                        dt = DataCache.GetCache(facid + "Shell") as DataTable;
                    }
                    else
                    {
                        dt = dao.GetAreaList(facid);
                        DataCache.SetCache(facid + "Shell", dt);
                    }
                    #endregion


                    if (dt != null && dt.Rows.Count > 0)
                    {




                        #region 定义字段和获取数据
                        DataView dataView = dt.DefaultView;
                        DataTable NewDt = new DataTable();


                        #endregion

                        if (dt != null && dt.Rows.Count > 0) //存在数据
                        {
                            sb = new StringBuilder();

                            int i = 0;

                            #region 组织数据
                            if (!string.IsNullOrEmpty(storeId))
                            {
                                DataRow[] rows = dt.Select("STOREID='" + storeId + "'");
                                if (rows != null && rows.Length == 1)
                                {
                                    sb.Append("{\"STOREID\":\"" + rows[0]["STOREID"].ToString() + "\",\"STORENAME\":\"" + rows[0]["STORENAME"].ToString() + "\",\"STOREADDRESS\":\"" + rows[0]["STOREADDRESS"].ToString() + "\",\"STORELINKMAN\":\"" + rows[0]["STORELINKMAN"].ToString() + "\",\"STORETEL\":\"" + rows[0]["STORETEL"].ToString() + "\"},");
                                }
                            }
                            else if (!string.IsNullOrEmpty(storeArea))
                            {
                                NewDt = dataView.ToTable(true, "STOREPROVINCE", "STORECITY", "STOREAREA", "STOREID", "STORENAME");
                                DataRow[] rows = NewDt.Select("STOREAREA='" + storeArea + "' AND STORECITY='" + storeCity + "' AND STOREPROVINCE='" + storeProvince + "'", "STORENAME ASC");

                                foreach (DataRow dr in rows)
                                {
                                    sb.Append("{\"STOREID\":\"" + dr["STOREID"] + "\",\"STORENAME\":\"" + dr["STORENAME"] + "\"},");
                                }

                            }
                            else if (!string.IsNullOrEmpty(storeCity))
                            {
                                NewDt = dataView.ToTable(true, "STOREPROVINCE", "STORECITY", "STOREAREA");
                                DataRow[] rows = NewDt.Select("STORECITY='" + storeCity + "' AND STOREPROVINCE='" + storeProvince + "'", "STOREAREA ASC");

                                foreach (DataRow dr in rows)
                                {
                                    ++i;
                                    sb.Append("{\"ID\":\"" + i + "\",\"NAME\":\"" + dr["STOREAREA"] + "\"},");
                                }

                            }
                            else if (!string.IsNullOrEmpty(storeProvince))
                            {
                                NewDt = dataView.ToTable(true, "STOREPROVINCE", "STORECITY");
                                DataRow[] rows = NewDt.Select("STOREPROVINCE='" + storeProvince + "'", "STORECITY ASC");

                                foreach (DataRow dr in rows)
                                {
                                    ++i;
                                    sb.Append("{\"ID\":\"" + i + "\",\"NAME\":\"" + dr["STORECITY"] + "\"},");
                                }
                            }
                            #endregion

                            NewDt.Dispose();

                            areaList = sb.ToString();
                            if (!string.IsNullOrEmpty(areaList))
                            {
                                bRet = true;
                            }
                            return bRet;
                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 获取省市出现异常 GetAreaList msg [facid:" + facid + "] [storeProvince:" + storeProvince + "] [storeCity:" + storeCity + "] [storeArea:" + storeArea + "] [storeId:" + storeId + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return bRet;
        }



        /// <summary>
        /// 壳牌喜力超凡促销活动 –获取省市下面的门店
        /// </summary>
        /// <param name="userID">授权帐号</param>
        /// <param name="userPwd">授权密码</param>
        /// <param name="provincename">省级名称</param>
        /// <param name="cityname">市级名称</param>
        /// <returns></returns>
        public DataTable GetStoreList(string facid, string provincename, string cityname)
        {
            DataTable dt = null;
            try
            {
                #region 使用缓存
                //if (DataCache.GetCache(facid + "SCS") != null)    //SCS :省-市-门店
                //{
                //    dt = DataCache.GetCache(facid + "SCS") as DataTable;
                //}
                //else
                //{
                //    dt = dao.GetStoreList(facid, provincename, cityname);
                //    DataCache.SetCache(facid + "SCS", dt);
                //}
                #endregion
                dt = dao.GetStoreList(facid, provincename, cityname);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 获取省市区出现异常 GetStoreList msg [facid:" + facid + "] [provincename:" + provincename + "]  [cityname:" + cityname + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }



        #region 查询基础数据表
        /// <summary>
        /// 根据基础类型查询基础数据
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="datatypename"></param>
        /// <returns></returns>
        public DataTable GetBaseDataByDataType(string facid, string datatypename)
        {

            DataTable dt = null;
            try
            {
                #region 使用缓存
                //if (DataCache.GetCache(facid + datatypename) != null)
                //{
                //    dt = DataCache.GetCache(facid + datatypename) as DataTable;
                //}
                //else
                //{
                //    dt = dao.GetBaseDataByDataType(facid, datatypename);
                //    DataCache.SetCache(facid + datatypename, dt);
                //}
                #endregion
                dt = dao.GetBaseDataByDataType(facid, datatypename);


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 根据基础类型查询基础数据 出现异常 GetBaseDataByDataType  [facid:" + facid + "] [datatypename:" + datatypename + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion

        #region 限制openID每月/每天/每年/ 参与红包发放的最大次数
        /// <summary>
        /// 限制openID每月/每天/每年/ 参与红包发放的最大次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="openid">微信粉丝号</param>
        /// <param name="datetype">限制类型（N:不区分 D：每天  M：每月  Y：每年）</param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public bool CheckOpenidHbSendLimit(string facid, string openid, string datetype, int limitnum)
        {
            bool bRet = false;
            try
            {
                bRet = dao.CheckOpenidHbSendLimit(facid, openid, datetype, limitnum);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo  限制openID每月/每天/每年/ 参与红包发放的最大次数 CheckOpenidHbSendLimit 异常：  [facid:" + facid + "] [openid:" + openid + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion

        #region 获取数码中奖纪录
        /// <summary>
        /// 获取数码中奖纪录
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// 
        /// <returns></returns>
        public DataTable GetLotteryInfoByCode(string facid, string code)
        {

            DataTable dt = null;
            try
            {
                dt = dao.GetLotteryInfoByCode(facid, code);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 获取数码中奖纪录 出现异常 GetLotteryInfoByCode  [facid:" + facid + "] [code:" + code + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion

        #region 检测活动是否开始/结束
        /// <summary>
        /// 检测活动是否开始/结束
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="type">1检测活动是否开始，2 检测活动是否结束，3 活动不在有效时间范围之内</param>
        /// <returns></returns>
        public bool CheckActivityTime(string facid, string type)
        {
            bool bRet = false;
            try
            {
                bRet = dao.CheckActivityTime(facid, type);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 检测活动是否开始/结束 出现异常 CheckActivityTime  [facid:" + facid + "] [type:" + type + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion


        #region  添加用户抽奖注册信息

        /// <summary>
        /// 添加用户抽奖注册信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="infolist"></param>
        /// <returns></returns>
        public bool AddLotteryRegister(string facid, Hashtable infolist)
        {

            bool bRet = false;
            try
            {
                bRet = dao.AddLotteryRegister(facid, infolist);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 添加用户抽奖注册信息  出现异常 AddLotteryRegister  [facid:" + facid + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion

        #region 添加邀请信息
        /// <summary>
        /// 添加邀请信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="infolist"></param>
        /// <returns></returns>
        public bool AddInviteInfo(string facid, Hashtable infolist)
        {

            bool bRet = false;
            try
            {
                bRet = dao.AddInviteInfo(facid, infolist);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 添加用户抽奖注册信息  出现异常 AddLotteryRegister  [facid:" + facid + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion




        #region 检测是否是中信码
        /// <summary>
        /// 检测是否是中信码
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// <param name="spUse"></param>
        /// <param name="spMobile"></param>
        /// <returns></returns>
        public bool CheckSpCodeExsit(string facid, string code, out  bool spUse, out  string spMobile)
        {
            //  CheckSpCodeExiet(factoryid, digitcode, out spUse, out spMobile);
            spUse = false;
            spMobile = "";
            bool bRet = false;
            try
            {
                bRet = dao.CheckSpCodeExsit(facid, code, out spUse, out spMobile);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 检测是否是中信码  出现异常 CheckSpCodeExsit  [facid:" + facid + "]  [code:" + code + "]   ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion

        #region 查询预警人
        public DataTable SelectRemindMan(string facid)
        {
            return dao.SelectRemindMan(facid);
        }
        #endregion


        #region 5) 通过IP或者电话或者手机获取城市
        /// <summary>
        /// 通过IP或者电话或者手机获取城市
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public bool GetCityCode(string ip, out string provice, out string cityCode)
        {
            bool flag = false;
            cityCode = "未知地区";
            provice = "未知地区";

            try
            {
                if (Common.RegexExpress.IsIP(ip))
                {
                    dao.QueryProviceAndCityByIP(ip, ref provice, ref cityCode);
                }
                else
                {
                    dao.QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref cityCode);
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:GetCityCode:" + ip + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion


    }
}
