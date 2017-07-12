// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Common
// *文件名称：SysConfig.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：应用程序相关静态类应用实体
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KMSLotterySystemFront.Model;
using System.Collections;

namespace KMSLotterySystemFront.Common
{
    public sealed class SysConfig
    {
        #region 答复消息加载单例
        public DataTable dbLotteryResult = new DataTable();

        private object resultobj = new object();

        public DataTable InitResult
        {
            get
            {
                if (dbLotteryResult == null)
                {
                    lock (resultobj)
                    {
                        dbLotteryResult = new DataTable();
                    }
                }
                return dbLotteryResult;
            }

        }
        #endregion

        #region 单实例 Single Pattern
        private static SysConfig instance;
        private static object oSnyc = new object();
        //单实例
        public static SysConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (oSnyc)
                    {
                        instance = new SysConfig();
                    }
                }
                return instance;
            }
        }
        #endregion

        #region 初始化config配置属性
        /// <summary>
        /// GPS定位API接口URL地址
        /// </summary>
        private String _GpsApiUrl;

        /// <summary>
        /// GPS定位API接口URL地址
        /// </summary>
        public String GpsApiUrl
        {
            get { return _GpsApiUrl; }
            set { _GpsApiUrl = value; }
        }

        /// <summary>
        /// GPS定位API接口AppKey
        /// </summary>
        private String _GpsApiAppKey;

        /// <summary>
        /// GPS定位API接口AppKey
        /// </summary>
        public String GpsApiAppKey
        {
            get { return _GpsApiAppKey; }
            set { _GpsApiAppKey = value; }
        }
        #endregion

        #region sql常量
        private readonly static string LOTTERYRESULT_APPLICATION_SQL = "SELECT R.REPLYID,R.REPLY,R.FACID,R.ACTIVITYID,R.CHANNEL,R.LANGUAGE,R.DELETEFLAG FROM t_sgm_wb_shake_reply_9999 R WHERE R.DELETEFLAG='1'";
        #endregion

        #region 初始化抽奖答复信息+获取对应厂家回复配置
        /// <summary>
        /// 抽奖答复配置表
        /// </summary>
        /// <returns></returns>
        public bool InitApplicationResultByDBConfig()
        {
            bool bRet = false;
            try
            {
                DBUtility.DataBase dataBase = new DBUtility.DataBase();
                DataTable dbRet = dataBase.ExecuteQuery(CommandType.Text, LOTTERYRESULT_APPLICATION_SQL, null);

                if (dbRet != null && dbRet.Rows.Count > 0)
                {
                    dbLotteryResult = dbRet;
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("SysConfig.cs--InitApplicationResultByDBConfig--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }

        /// <summary>
        /// 获取回复语句
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="productid">产品编号</param>
        /// <param name="language">语言</param>
        /// <param name="channlet">通道类型</param>
        /// <param name="type">类型（正常，存在）</param>
        /// <param name="result">返回:回复语句</param>
        /// <returns></returns>
        public bool GetDigitCodeResult(string directoryName, string language, string channlet, string activityid, out MsgMdl result)
        {
            bool flag = false;
            result = null;
            try
            {
                //查询该厂家正常回复
                DataRow[] dr2 = dbLotteryResult.Select("FACID='" + directoryName + "' AND  LANGUAGE='" + language + "' AND CHANNEL='" + channlet + "' AND ACTIVITYID='" + activityid + "'");
                if (dr2 != null && dr2.Length > 0)
                {
                    result = new MsgMdl();
                    foreach (DataRow dr in dr2)
                    {
                        #region 获取正确的回复内容
                        switch (dr[0].ToString().Trim())
                        {
                            case "ActivitiesHaveEnded"://活动已经结束
                                result.ActivitiesHaveEnded = dr[1].ToString().Trim();
                                break;
                            case "ActivitiesNoStart"://活动还未开始
                                result.ActivitiesNoStart = dr[1].ToString().Trim();
                                break;
                            case "TelecomMobile"://电信手机
                                result.TelecomMobile = dr[1].ToString().Trim();
                                break;
                            case "LotteryOver"://奖项已兑完
                                result.LotteryOver = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinning"://已经中奖
                                result.HasBeenWinning = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningHH"://中奖-话费
                                result.HasBeenWinningHH = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningSW"://中奖-实物
                                result.HasBeenWinningSW = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningLL"://中奖-流量
                                result.HasBeenWinningLL = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningHB"://中奖-红包
                                result.HasBeenWinningHB = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningXNQ"://中奖-虚拟券
                                result.HasBeenWinningXNQ = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningkq"://已经中奖 (京东卡券)
                                result.HasBeenWinningkq = dr[1].ToString().Trim();
                                break;

                            case "ExceedTheMaximumParticipation"://超过最大参与次数
                                result.ExceedTheMaximumParticipation = dr[1].ToString().Trim();
                                break;
                            case "ExceedTheMaximumWinning"://超过最大中奖次数
                                result.ExceedTheMaximumWinning = dr[1].ToString().Trim();
                                break;
                            case "ErrorOfTheDigital"://数码错误或者非指定产品码
                                result.ErrorOfTheDigital = dr[1].ToString().Trim();
                                break;
                            case "DigitalHasBeenDrawing"://条码已经参与过抽奖
                                result.DigitalHasBeenDrawing = dr[1].ToString().Trim();
                                break;
                            case "DigitalHasBeenWinning"://条码已经中过抽奖
                                result.DigitalHasBeenWinning = dr[1].ToString().Trim();
                                break;
                            case "DigitalHasBeenQuery"://条码复查
                                result.DigitalHasBeenQuery = dr[1].ToString().Trim();
                                break;
                            case "DigitalFormatError"://条码格式错误
                                result.DigitalFormatError = dr[1].ToString().Trim();
                                break;
                            case "SystemError"://系统故障
                                result.SystemError = dr[1].ToString().Trim();
                                break;
                            case "DigitalIsNotWinning"://未中奖
                                result.DigitalIsNotWinning = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinning20"://成功充值20元
                                result.HasBeenWinning20 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinning30"://成功充值100元
                                result.HasBeenWinning30 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinning100"://成功充值100元
                                result.HasBeenWinning100 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinning5"://成功充值5元
                                result.HasBeenWinning5 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinning50"://成功充值50元
                                result.HasBeenWinning50 = dr[1].ToString().Trim();
                                break;
                            case "SysCodeNotActivation"://数码未防伪
                                result.SysCodeNotActivation = dr[1].ToString().Trim();
                                break;
                            case "ProductNotActivities"://产品未参与活动
                                result.ProductNotActivities = dr[1].ToString().Trim();
                                break;
                            case "LottoSMSWarning"://中奖短信预警
                                result.LottoSMSWarning = dr[1].ToString().Trim();
                                break;
                            case "ParticipateSMSWarning"://参与短信预警
                                result.ParticipateSMSWarning = dr[1].ToString().Trim();
                                break;
                            case "DigitCodeNotRegister"://数码没有被注册过
                                result.DigitCodeNotRegister = dr[1].ToString().Trim();
                                break;
                            case "DigitCodeReigster"://数码已经被注册
                                result.DigitCodeReigster = dr[1].ToString().Trim();
                                break;
                            case "CodeRegisterSendMail"://发送邮件提醒内容
                                result.CodeRegisterSendMail = dr[1].ToString().Trim();
                                break;
                            case "MessageF1":
                                result.MessageF1 = dr[1].ToString().Trim();
                                break;
                            case "MessageF2":
                                result.MessageF2 = dr[1].ToString().Trim();
                                break;
                            case "MessageF3":
                                result.MessageF3 = dr[1].ToString().Trim();
                                break;
                            case "MessageF4":
                                result.MessageF4 = dr[1].ToString().Trim();
                                break;
                            case "MessageF5":
                                result.MessageF5 = dr[1].ToString().Trim();
                                break;
                            case "MessageF6":
                                result.MessageF6 = dr[1].ToString().Trim();
                                break;
                            case "MessageF7":
                                result.MessageF7 = dr[1].ToString().Trim();
                                break;
                            case "MessageF8":
                                result.MessageF8 = dr[1].ToString().Trim();
                                break;
                            case "MessageF9":
                                result.MessageF9 = dr[1].ToString().Trim();
                                break;
                            case "MessageF10":
                                result.MessageF10 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF1":
                                result.HasBeenWinningF1 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF2":
                                result.HasBeenWinningF2 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF3":
                                result.HasBeenWinningF3 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF4":
                                result.HasBeenWinningF4 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF5":
                                result.HasBeenWinningF5 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF6":
                                result.HasBeenWinningF6 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF7":
                                result.HasBeenWinningF7 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF8":
                                result.HasBeenWinningF8 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF9":
                                result.HasBeenWinningF9 = dr[1].ToString().Trim();
                                break;
                            case "HasBeenWinningF10":
                                result.HasBeenWinningF10 = dr[1].ToString().Trim();
                                break;
                            case "StoreNoJoinActivity":
                                result.StoreNoJoinActivity = dr[1].ToString().Trim();
                                break;
                            case "StoreInfoIsWrong":
                                result.StoreInfoIsWrong = dr[1].ToString().Trim();
                                break;
                            case "OpenidHasBeenJoin":
                                result.OpenidHasBeenJoin = dr[1].ToString().Trim();
                                break;

                            case "MobileHasBeenWinning":
                                result.MobileHasBeenWinning = dr[1].ToString().Trim();
                                break;
                            case "MobileHasBeenQuery":
                                result.MobileHasBeenQuery = dr[1].ToString().Trim();
                                break;
                            case "MobileVerifyCodeNotExsit":
                                result.MobileVerifyCodeNotExsit = dr[1].ToString().Trim();
                                break;

                            case "MobileTBSuccess":
                                result.MobileTBSuccess = dr[1].ToString().Trim();
                                break;
                            case "MobileTBFail":
                                result.MobileTBFail = dr[1].ToString().Trim();
                                break;
                            case "OpenidTBSuccess":
                                result.OpenidTBSuccess = dr[1].ToString().Trim();
                                break;
                            case "OpenidTBFail":
                                result.OpenidTBFail = dr[1].ToString().Trim();
                                break;
                            case "DigitTBSuccess":
                                result.DigitTBSuccess = dr[1].ToString().Trim();
                                break;
                            case "DigitTBFail":
                                result.DigitTBFail = dr[1].ToString().Trim();
                                break;
                            case "OpenidIsError":
                                result.OpenidIsError = dr[1].ToString().Trim();
                                break;
                            case "UserNameIsError":
                                result.UserNameIsError = dr[1].ToString().Trim();
                                break;

                            case "RedPackNumLimit":
                                result.RedPackNumLimit = dr[1].ToString().Trim();
                                break;

                            default:
                                break;
                        }
                        #endregion

                        #region 获取内容 屏蔽
                        //if (dr[0].ToString().Trim() == "ActivitiesHaveEnded")
                        //{//活动已经结束
                        //    result.ActivitiesHaveEnded = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "ActivitiesNoStart")
                        //{//活动还未开始
                        //    result.ActivitiesNoStart = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "TelecomMobile")
                        //{//电信手机
                        //    result.TelecomMobile = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "LotteryOver")
                        //{//奖项已兑完 
                        //    result.LotteryOver = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "HasBeenWinning")
                        //{//已经中奖 LotteryOver
                        //    result.HasBeenWinning = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "ExceedTheMaximumParticipation")
                        //{//超过最大参与次数 ExceedTheMaximumNumberOfTimesParticipation
                        //    result.ExceedTheMaximumParticipation = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "ExceedTheMaximumWinning")
                        //{//超过最大中奖次数 ExceedTheMaximumNumberOfTimesWinning
                        //    result.ExceedTheMaximumWinning = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "ErrorOfTheDigital")
                        //{//数码错误或者非指定产品码 ErrorOfTheDigital
                        //    result.ErrorOfTheDigital = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitalHasBeenDrawing")
                        //{//条码已经参与过抽奖 DigitalHasBeenDrawing
                        //    result.DigitalHasBeenDrawing = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitalHasBeenWinning")
                        //{//条码已经中过抽奖 DigitalHasBeenWinning
                        //    result.DigitalHasBeenWinning = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitalHasBeenQuery")
                        //{//条码复查 DigitalHasBeenQuery
                        //    result.DigitalHasBeenQuery = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitalFormatError")
                        //{//条码格式错误
                        //    result.DigitalFormatError = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "SystemError")
                        //{//系统故障
                        //    result.SystemError = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitalIsNotWinning")
                        //{//未中奖 DigitalIsNotWinning
                        //    result.DigitalIsNotWinning = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "HasBeenWinning20")
                        //{//成功充值20元
                        //    result.HasBeenWinning20 = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "HasBeenWinning30")
                        //{//成功充值100元
                        //    result.HasBeenWinning30 = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "HasBeenWinning100")
                        //{//成功充值100元
                        //    result.HasBeenWinning100 = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "HasBeenWinning5")
                        //{//成功充值5元
                        //    result.HasBeenWinning5 = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "HasBeenWinning50")
                        //{//成功充值50元
                        //    result.HasBeenWinning50 = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "SysCodeNotActivation")
                        //{//数码未防伪
                        //    result.SysCodeNotActivation = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "ProductNotActivities")
                        //{//产品未参与活动
                        //    result.ProductNotActivities = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "LottoSMSWarning")
                        //{//中奖短信预警
                        //    result.LottoSMSWarning = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "ParticipateSMSWarning")
                        //{//参与短信预警
                        //    result.ParticipateSMSWarning = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitCodeNotRegister")
                        //{//数码没有被注册过
                        //    result.DigitCodeNotRegister = dr[1].ToString().Trim();
                        //}
                        //else if (dr[0].ToString().Trim() == "DigitCodeReigster")
                        //{//数码已经被注册
                        //    result.DigitCodeReigster = dr[1].ToString().Trim();
                        //}

                        #endregion

                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("SysConfig:GetDigitCodeResult:" + directoryName + "---" + language + "---" + channlet + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion


        #region 初始化配置（confing）

        public Boolean InitLotteryConfig()
        {
            //是否加异常判断
            Boolean bRet = false;
            try
            {
                //获得GPS定位API接口URL地址
                GpsApiUrl = this.GetKeyValue("GPS_API_URL");
                //获得GPS定位API接口APPKEY
                GpsApiAppKey = this.GetKeyValue("GPS_API_APP_KEY");

                bRet = true;


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("SysConfig.cs--InitLotteryApplictionConfig--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion


        #region 4)读取配置文件键值，以字符串返回。键没定义时返回空串
        /// <summary>
        /// 读取配置文件键值，以字符串返回。键没定义时返回空串。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String GetKeyValue(string key)
        {
            //读取配置文件，没有时会返回null，不会出错
            string strValue = System.Configuration.ConfigurationManager.AppSettings[key];
            if (strValue == null)
            {
                strValue = "";
            }
            return strValue;
        }
        #endregion


    }
}
