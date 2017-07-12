
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.Common;
using KMSLotterySystemFront.BLLLottery;
using KMSLotterySystemFront.Model;
using System.Data;
using System.Collections;
using System.Xml;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KMSLotterySystemFront.Platform;
using KMSLotterySystemFront.Error;


namespace KMSLotterySystemFront.BLL
{
    public class RechargeLotteryNewPf
    {
        #region 实例
        public readonly static DigitcodeBLL dbll = new DigitcodeBLL();
        public readonly static ControlBLL cbll = new ControlBLL();
        public readonly static LotteryBLL lbll = new LotteryBLL();

        public readonly static PointsControlBLL pcbll = new PointsControlBLL();
        public readonly static PointsLotteryBLL plbll = new PointsLotteryBLL();

        #endregion

        #region 康明斯抽奖(老平台)
        /// <summary>
        /// 壳牌GGK营销活动-- 话费、流量、实物礼品
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="language">查询语言</param>
        /// <param name="codeState">数码被查询状态</param>
        /// <param name="codeResult">数码查询答复</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="userHsah">注册信息项目</param>
        /// <param name="ip">手机号码</param>
        /// <param name="queryID">防伪查询记录请求ID</param>
        /// <param name="token">数据加密校验token</param>
        /// <param name="jkactivityid">监控平台活动ID</param>
        /// <param name="result">输出:抽奖答复</param>
        /// <param name="systemstate">输出:抽奖执行状态</param>
        /// <param name="lotteryLevel">输出:中奖等级</param>
        /// <param name="lotteryName">输出:中奖奖项等级</param>
        /// <param name="lid">输出:中奖纪录id</param>
        /// <returns></returns>
        public bool QueryLotteryRechargeKMS(string factoryid, string digitcode, string channel, int language, string mobile, Hashtable userHsah, string ip, out string result, out string systemstate, out string lotteryLevel, out string lotteryName, out string lid)
        {
            lid = "";
            result = "";
            systemstate = "000";
            lotteryLevel = "";
            lotteryName = "";
            bool bErrCode = false;
            ActivityInfo ayinfo = null;
            //通过IP/电话/手机号码 获取城市
            string provice = string.Empty;
            string cityName = string.Empty;
            CodeCreateInfo codeCreateInfo = new CodeCreateInfo();

            //获取默认消息回复
            DefaultMsg defaultmsg = new DefaultMsg();
            string colorstr = string.Empty;


            #region 2）参数初始化

            string ServiceId = string.Empty;
            string strFacIDProID = string.Empty;
            string FacID = string.Empty;//解码的厂家ID
            string ProID = "";//解码的产品ID
            string activityFacID = string.Empty;//活动的厂家ID
            string activityProID = string.Empty;//活动的产品ID
            string TrackNum = string.Empty;
            string pTableField = "OUTPROID";//P表中产品编码字段名称
            string productMark = "1";//产品标志 0 P表区分产品 ,1 p表不区分产品
            string activityId = string.Empty;//活动编码
            string activityName = string.Empty;//活动名称
            string poolId = string.Empty;//奖池编码
            string builddateStr = string.Empty; //生码日期
            string dbproductid = string.Empty;
            string dbproductname = string.Empty;
            string dbproducttime = string.Empty;
            bool isOldUser = false;//是否是老用户
            bool hasJoinInMouth = false;//（当前自然月是否参加过活动 针对 R5E二期）


            //活动规则信息
            int smstotalop = 1;//短信提醒参与次数阈值
            int totalNum = 0;//当前参与总人数
            //int lotteryNumber =Convert.ToInt32(activitydt.Rows[0]["LOTTERYTIMES"].ToString());//中奖次数??????
            string activityStartData = string.Empty;//活动开始日期
            string activityEndDate = string.Empty;//活动结束日期
            int cycletimes = 1;//周期数量
            string cycleType = "D";//周期(年Y 月M 日D 周W)
            string digitlimit = "1";//数码限制 1 首次
            int maxLotteryNumber = 1;//最大许可中奖数量
            int maxJointTime = 1;//参与最大次数
            string protype = "";//产品类型

            //奖池规则参数
            int dayMaxLotterynumber = 1;//每日已中奖的数量
            int lotteryNumber = 99999;//奖项中奖数量
            int awardscale = 999999;//中奖比例阈值
            int smsValue = 1;//短信提醒阈值
            int maxOpenLotter = 0;//每日开奖最大数量
            int totalTimes = 0;//本期奖项总数
            string awardscode = "0";//奖项编号
            string openid = null;
            // string mobileType = string.Empty;
            KMSLotterySystemFront.Common.RegexExpress.MobileType mobileType;
            #endregion

            try
            {
                string newcode = "";

                bool isEncypt = false;

                string fwsql = string.Empty;

                bool isSpCode = false;

                CodeAttachedInfo codeAttachedInfo;

                MsgMdl msg = new MsgMdl();

                //获取省份、城市
                dbll.GetCityCode(mobile, out provice, out cityName);
                if (userHsah == null) userHsah = new Hashtable();
                #region √注册信息必填项信息组织
                userHsah.Add("FACID", factoryid);
                userHsah.Add("SPRO", digitcode);
                userHsah.Add("IP", mobile);
                userHsah.Add("CATEGORY", channel);
                userHsah.Add("F11", provice);
                userHsah.Add("F12", cityName);
                #endregion

                #region √判断活动总控
                //判断活动总控
                if (!cbll.GetService(factoryid, channel, out ayinfo))
                {
                    result = defaultmsg.NotActiivties;
                    systemstate = CustomerSystemState.NN抽奖活动不存在.GetEnumDescription();
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, "0", cityName, "0", "0", result, userHsah, true, systemstate);
                    return false;
                }
                userHsah.Add("ACTIVITYID", ayinfo.Activityid);
                #endregion

                #region √获取回复消息
                //获取和判断回复消息
                bool megflag = KMSLotterySystemFront.Common.SysConfig.Instance.GetDigitCodeResult(factoryid, language.ToString(), channel, ayinfo.Activityid, out msg);
                if (!megflag)
                {
                    result = defaultmsg.NotMsg;
                    systemstate = CustomerSystemState.NN活动答复未配置.GetEnumDescription();
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", "0", result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √检查数码格式/长度等

                CharHelper cHelper = new CharHelper();
                digitcode = cHelper.ReplaceQuanJiaoToBanJiao(digitcode);

                #region 验证数码格式
                //数码格式错误
                if (!RegexExpress.IsDigitCode16(digitcode))
                {
                    result = msg.DigitalFormatError;
                    systemstate = CustomerSystemState.NN防伪数码格式错误.GetEnumDescription();
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", "0", result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region 或者电话号码格式错误
                if (!RegexExpress.IsMobile(mobile))
                {
                    result = defaultmsg.ErrorOfIp;
                    systemstate = CustomerSystemState.NN手机号码格式不正确.GetEnumDescription();
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", "0", result, userHsah, true, systemstate);
                    return false;
                }
                //网站的话要验证IP
                if (channel == "W")
                {
                    if (!RegexExpress.IsIP(ip))
                    {
                        result = defaultmsg.ErrorOfIp;
                        systemstate = CustomerSystemState.NNIP地址格式不正确.GetEnumDescription();
                        lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", "0", result, userHsah, true, systemstate);
                        return false;
                    }
                }

                mobileType = RegexExpress.CheckNewMobileType(mobile);
                //mobileType = RegexExpress.CheckNewMobileType(mobile).ToString();


                #endregion

                #endregion

                codeCreateInfo = DecryptCode.GetCodeInfo(digitcode);
                dbproductid = codeCreateInfo.ProID;

                #region √判断数码是否为指定活动参与的数码厂家
                //判断数码是否为指定活动参与的数码厂家 
                if (codeCreateInfo.FacID != factoryid)
                {
                    systemstate = CustomerSystemState.NN不是指定活动参与的数码厂家.GetEnumDescription();
                    result = msg.ErrorOfTheDigital;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", codeCreateInfo.ProID, result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √判断数码所属表是否存在
                //判断数码所属表是否存在
                if (!dbll.CheckTableIsExist(factoryid, codeCreateInfo.TableName))
                {
                    systemstate = CustomerSystemState.NN数码不存在.GetEnumDescription();
                    result = msg.ErrorOfTheDigital;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", codeCreateInfo.ProID, result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √判断数码已经过期
                //判断数码已经过期
                if (!dbll.CheckFacProduct(codeCreateInfo.FacID, codeCreateInfo.ProID))
                {
                    systemstate = CustomerSystemState.NN数码不存在.GetEnumDescription();
                    result = msg.ErrorOfTheDigital;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", codeCreateInfo.ProID, result, userHsah, true, systemstate);

                    return false;
                }
                #endregion

                #region √判断二次加密
                if (!bErrCode)
                {
                    isEncypt = DecryptCode.GetAllEncypt(codeCreateInfo, digitcode, out newcode);
                    if (isEncypt) //需要二次加密同时加密出来的数码为空时 
                    {
                        if (string.IsNullOrEmpty(newcode))
                        {
                            bErrCode = true;
                        }
                    }
                    else
                    {
                        newcode = digitcode;
                    }
                }
                #endregion

                #region √判断数码所属表信息表结构
                //判断数码所属表信息表结构
                if (!dbll.GetTableColmunList(factoryid, codeCreateInfo.TableName, newcode, out fwsql))
                {
                    systemstate = CustomerSystemState.NN数码不存在.GetEnumDescription();
                    result = msg.ErrorOfTheDigital;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", codeCreateInfo.ProID, result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √判断数码是否存在
                //判断数码是否存在
                codeAttachedInfo = dbll.GetCodeAttachedInfo(factoryid, fwsql, codeCreateInfo.FacID, codeCreateInfo.ProID, codeCreateInfo.CreateDate.ToString("yyyyMMdd"));
                if (!codeAttachedInfo.CodeIsExist)
                {
                    systemstate = "110";
                    result = msg.ErrorOfTheDigital;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", codeCreateInfo.ProID, result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                string ProductType = string.Empty;

                #region √判断产品是否可以参与活动
                if (ayinfo.Productmark.Equals("0") && string.IsNullOrEmpty(dbproductid))
                {
                    systemstate = CustomerSystemState.NN产品不参与活动.GetEnumDescription(); //
                    result = msg.ProductNotActivities;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                    return false;
                }

                if (ayinfo.Productmark.Equals("1"))
                {
                    DataTable ProductData = new DataTable();

                    if (cbll.GetActiovityProduct(factoryid, out ProductData))
                    {
                        string ConfigProductTime = lbll.GetBaseDataValue(factoryid, "ProductTime");

                        string DBConfigProductTime = lbll.GetBaseDataValue(factoryid, "DBProductTime");

                        string DBConfigProvice = lbll.GetBaseDataValue(factoryid, "DBMobileProvice");

                        DataRow[] rowProduct = ProductData.Select("PROID='" + dbproductid + "'");

                        if (rowProduct != null && rowProduct.Length == 1)
                        {
                            string ProductTypeAct = rowProduct[0]["F1"].ToString();//是否需要进行生产时间判断  1:需要  0:不需要
                            ProductType = rowProduct[0]["F2"].ToString();//产品类别,
                            #region R4、R5产品生产时间判断
                            if (!string.IsNullOrEmpty(ConfigProductTime)) //配置的生产时间为空
                            {
                                #region 统一生产时间校验（改进点：时间配置到product中可对单个产品进行校验）
                                if (!string.IsNullOrEmpty(ProductTypeAct))
                                {
                                    if (ProductTypeAct.Equals("1"))
                                    {
                                        if (!string.IsNullOrEmpty(dbproducttime))
                                        {
                                            if (Convert.ToDateTime(dbproducttime) < Convert.ToDateTime(ConfigProductTime)) //数码实际生产时间必须大于配置的时间
                                            {
                                                systemstate = CustomerSystemState.NN生产时间不在许可范围.GetEnumDescription(); //生产时间不在许可范围
                                                result = msg.MessageF2;
                                                lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            systemstate = CustomerSystemState.NN数码生产时间为空.GetEnumDescription(); //数码生产时间为空
                                            result = msg.MessageF3;
                                            lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                                            return false;
                                        }
                                    }
                                }
                                #endregion

                            }
                            #endregion

                        }
                        else
                        {
                            systemstate = CustomerSystemState.NN产品不参与活动.GetEnumDescription(); //此产品不在参与范围
                            result = msg.ProductNotActivities;
                            lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                            return false;
                        }
                    }
                    else
                    {
                        systemstate = CustomerSystemState.NN未配置参与活动的产品.GetEnumDescription();
                        result = msg.MessageF4;
                        lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                        return false;
                    }
                }

                #endregion

                #region √判断回传激活

                bool TraceTypeIsUA = false;

                if (codeAttachedInfo.CodeIsExist)//数码存在
                {
                    //判断是否需要回传激活
                    //if (dbll.CheckProductIsAct(factoryid, codeCreateInfo.ProID))
                    if (codeAttachedInfo.Is_Activation_Flag)//判断是否需要回传激活
                    {
                        //判断是否激活
                        if (codeAttachedInfo.Activation_Flag == "0")
                        {
                            TraceTypeIsUA = true;
                        }
                    }
                }
                #endregion


                #region GPS数据处理
                string provinceName = string.Empty;
                string cityName2 = string.Empty;
                string cityCode = string.Empty;
                string gpsValues = string.Empty;
                string acitivityId = string.Empty;

                bool isGPS = false;
                //ip = "121.544379|31.221517";
                if (RegexExpress.IsGPS(ip))
                {
                    LotteryPfMain pfm = new LotteryPfMain();

                    if (!string.IsNullOrEmpty(ip))
                    {
                        if (ip.Contains("|"))
                        {
                            isGPS = true;
                            pfm.GetGpsApiByIP(ip, out provinceName, out  cityName2, out  cityCode, out acitivityId);
                            gpsValues = ip + "|" + cityCode;
                        }
                        else
                        {
                            isGPS = false;
                            gpsValues = ip;
                        }
                    }
                    else
                    {
                        gpsValues = ip;
                    }
                }
                else
                {
                    gpsValues = ip;
                }
                #endregion

                #region √更新数码表记录

                //更新P表T表记录
                string tablename_p = "T_INFOCODE_P" + codeCreateInfo.FacID + codeCreateInfo.ProID;
                string tablename_t = "T_INFOCODE_T" + codeCreateInfo.FacID + codeCreateInfo.ProID;
                string tablename_t_o = "T_INFOCODE_T" + codeCreateInfo.FacID;


                //防伪查询记录更新
                if (codeAttachedInfo.IsQueryed)
                {
                    //复查
                    dbll.ModifyCodeNotFirstQueryInfo_GPS(tablename_p, tablename_t, tablename_t_o, TraceType.L.ToString(), digitcode, ip, codeAttachedInfo.SaleArea, codeCreateInfo.FacID, codeAttachedInfo.CreateCodeProID, codeAttachedInfo.ProID, "", newcode, channel, isEncypt, codeCreateInfo.CreateDate.ToString("yyyyMMdd"), isGPS, provinceName, cityName2);
                }
                else
                {
                    string strTraceType = string.Empty;
                    if (TraceTypeIsUA)
                        strTraceType = "UA";
                    else
                        strTraceType = "S";

                    //首查
                    dbll.ModifyCodeNotFirstQueryInfo_GPS(tablename_p, tablename_t, tablename_t_o, strTraceType, digitcode, ip, codeAttachedInfo.SaleArea, codeCreateInfo.FacID, codeAttachedInfo.CreateCodeProID, codeAttachedInfo.ProID, "", newcode, channel, isEncypt, codeCreateInfo.CreateDate.ToString("yyyyMMdd"), isGPS, provinceName, cityName2);
                }
                #endregion

                #region √数码未激活答复数码不存在
                if (TraceTypeIsUA)
                {
                    systemstate = CustomerSystemState.NN数码未激活.GetEnumDescription();
                    //未激活
                    result = msg.ErrorOfTheDigital;
                    lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", codeCreateInfo.ProID, result, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                string newpoolid = string.Empty;
                string FirstQueryDateString = string.Empty;

                #region √判断活动是否开始(判断活动是否开始和是否结束)即判断是否在活动期间范围内
                //判断活动是否开始
                if (!cbll.GetActivityByAID(factoryid, ayinfo.Activityid, "2", out newpoolid))
                {
                    result = msg.ActivitiesNoStart;
                    systemstate = CustomerSystemState.NN活动未开始.GetEnumDescription();
                    lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √判断活动是否结束
                //判断活动是否结束
                if (!cbll.GetActivityByAID(factoryid, ayinfo.Activityid, "0", out newpoolid))
                {
                    result = msg.ActivitiesHaveEnded;
                    systemstate = CustomerSystemState.NN活动已结束.GetEnumDescription();
                    lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √获取奖池
                DataTable activitydt = null;
                //获取奖池 
                if (!cbll.GetAward(factoryid, ayinfo.Activityid, out activitydt))
                {
                    KMSLotterySystemFront.Logger.AppLog.Write("GetAward 获取奖池:activitydt=" + activitydt, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                    //没有奖池
                    result = msg.DigitalIsNotWinning;
                    //lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, newpoolid, cityName, "0", dbproductid, result, newcode, true, true, userHsah,true);
                    systemstate = CustomerSystemState.NN活动奖池为空.GetEnumDescription();
                    lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                    return false;
                }

                KMSLotterySystemFront.Logger.AppLog.Write("GetAward 获取奖池:activitydt=" + activitydt, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                #endregion

                #region 已经注释 根据条件判断活动是否开始或者结束如：批次 产品类型等
                /****
                #region 判断活动是否开始(判断活动是否开始和是否结束)即判断是否在活动期间范围内
                //判断活动是否开始
                if (!cbll.GetActivityByAIDAndBatchid(factoryid, ayinfo.Activityid, codeAttachedInfo.BatchID, "2", out newpoolid))
                {
                    if (string.IsNullOrEmpty(newpoolid))
                    {
                        //没有奖池
                        result = msg.MessageF1;
                        //lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, newpoolid, cityName, "0", dbproductid, result, newcode, true, true, userHsah,true);
                        systemstate = "140";
                        lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                    }
                    else
                    {
                        result = msg.ActivitiesNoStart;
                        systemstate = "114";
                        lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                    }
                    return false;
                }
                #endregion
                
                KMSLotterySystemFront.Logger.AppLog.Write("GetActivityByAIDAndProductType 判断活动是否开始:poolid=" + newpoolid, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                #region 判断活动是否结束
                //判断活动是否结束
                if (!cbll.GetActivityByAIDAndBatchid(factoryid, ayinfo.Activityid, codeAttachedInfo.BatchID, "0", out newpoolid))
                {
                    if (string.IsNullOrEmpty(newpoolid))
                    {
                        //没有奖池
                        result = msg.MessageF1;
                        //lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, newpoolid, cityName, "0", dbproductid, result, newcode, true, true, userHsah,true);
                        systemstate = "141";
                        lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                    }
                    else
                    {
                        result = msg.ActivitiesHaveEnded;
                        systemstate = "115";
                        lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, ayinfo.Activityid, cityName, "0", dbproductid, result, userHsah, true, systemstate);
                    }
                    return false;
                }
                KMSLotterySystemFront.Logger.AppLog.Write("GetActivityByAIDAndProductType 判断活动是否结束:poolid=" + newpoolid, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                #endregion

                ****/
                #endregion

                #region √检查每日中奖数量是否超过上限
                string lotterydaymaxnum = lbll.GetBaseDataValue(factoryid, "LotteryDayMaxNum");
                DataTable dtKK = lbll.SelectRegister(factoryid, string.Format(" AND X.ip='{0}' AND LOTTERYLEVEL > 0 ", mobile));
                if (!string.IsNullOrEmpty(lotterydaymaxnum) && dtKK != null && dtKK.Rows != null && dtKK.Rows.Count > 0)
                {
                    if (dtKK.Rows.Count >= Convert.ToInt32(lotterydaymaxnum))
                    {
                        result = msg.MessageF5; //当日开奖数量已经超过上限
                        systemstate = CustomerSystemState.NN用户已达当日开奖上限.GetEnumDescription();
                        lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                        return false;
                    }
                }
                #endregion

                #region √检查此数码是否已经参与过抽奖 并且是否已经填写邮寄信息
                openid = userHsah["OPENID"].ToString();
                dtKK = lbll.SelectRegister(factoryid, string.Format(" AND X.SPRO='{0}' AND X.IP = '{1}' AND X.STATE='9' AND X.OPENID='{2}' ", digitcode, mobile, openid));
                if (dtKK != null && dtKK.Rows != null && dtKK.Rows.Count > 0)
                {
                    result = msg.MessageF6; //当前手机这个数码没有填写邮寄地址
                    systemstate = CustomerSystemState.NN未填写邮寄信息.GetEnumDescription();
                    lotteryLevel = dtKK.Rows[0]["LOTTERYLEVEL"].ToString();
                    lid = dtKK.Rows[0]["GUID"].ToString();
                    lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                #region √筛选出奖池（因活动参与条件不一样，每个规则下可能有N个奖池）
                activitydt = Utility.ToDataTable(activitydt.Select(" POOLID='" + newpoolid + "' "));

                if (activitydt == null)
                {
                    //没有奖池
                    result = msg.DigitalIsNotWinning;
                    systemstate = CustomerSystemState.NN活动奖池为空.GetEnumDescription();
                    lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                    return false;
                }
                #endregion

                bool once = true;//只运行一次
                foreach (DataRow drt in activitydt.Rows)
                {
                    #region √获取 活动规则	奖池规则 信息
                    smstotalop = Convert.ToInt32(drt["SMSTIMES"].ToString());//短信提醒次数阈值
                    totalNum = Convert.ToInt32(drt["TOTALPOP"].ToString());//当前参与总人数					
                    activityStartData = drt["STARTDATE"].ToString();//活动开始日期
                    activityEndDate = drt["ENDDATE"].ToString();//活动结束日期
                    cycletimes = Convert.ToInt32(drt["CYCLETIMES"].ToString());//周期数量
                    cycleType = drt["CYCLE"].ToString().ToUpper();//周期(年Y 月M 日D 周W)
                    digitlimit = drt["DIGITLIMIT"].ToString();//数码限制 1 首次
                    maxLotteryNumber = Convert.ToInt32(drt["MAXLOTTERYTIMES"].ToString());//最大中奖次数
                    maxJointTime = Convert.ToInt32(drt["MAXJOINTIMES"].ToString());//参与最大次数
                    poolId = drt["POOLID"].ToString().Trim();//奖池编码(父)	
                    protype = drt["PROTYPE"].ToString().Trim();//产品类型ID


                    //获取奖池规则信息
                    awardscode = drt["AWARDSCODE"].ToString().Trim();//奖项编号
                    dayMaxLotterynumber = cbll.GetDayMaxLottery(poolId, awardscode, factoryid);//获取奖池+奖项编号 每日已中奖的数量
                    maxOpenLotter = Convert.ToInt32(drt["MAXOPENLOTTER"].ToString());//每日开奖最大数量
                    totalTimes = Convert.ToInt32(drt["TOTALTIMES"].ToString());//本期奖项总数 (判断合并后20元总数)
                    lotteryNumber = Convert.ToInt32(drt["LOTTERYTIMES"].ToString());//奖项中奖数量 (记录已合并20元总数)
                    awardscale = Convert.ToInt32(drt["AWARDSCALE"].ToString());//中奖比例阈值
                    smsValue = Convert.ToInt32(drt["SMSVALUE"].ToString());//短信提醒阈值
                    #endregion

                    #region √检查防伪码是否已经中过奖
                    if (cbll.CkeckDigitalHasBeenWinningContains0(digitcode, factoryid)) //防伪码已中奖
                    {
                        result = msg.DigitalHasBeenWinning;
                        systemstate = CustomerSystemState.NN数码已经参与过活动.GetEnumDescription();
                        lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                        return false;
                    }
                    #endregion

                    #region √只运行一次，暂时不用
                    if (once)
                    {
                        if ((totalNum + 1) % smstotalop == 0)
                        {
                            #region 短信预警回复
                            string lottosmsstr = string.Empty;
                            lottosmsstr = msg.ParticipateSMSWarning;
                            lottosmsstr = string.Format(lottosmsstr, activityName, (totalNum + 1).ToString());
                            lbll.sendPrewarning(3, factoryid, lottosmsstr);
                            #endregion
                        }

                        lbll.ModifyAwardNum(factoryid, ayinfo.Activityid, newpoolid);
                        once = false;
                    }
                    #endregion

                    string awasName = string.Empty;
                    bool isLotteryFlag = false;
                    ShakeBaseData basedatainfo = null;

                    #region √计算中奖规则
                    if (((totalNum + 1) % awardscale == 0) && (dayMaxLotterynumber < maxOpenLotter) && (lotteryNumber < totalTimes))
                    {
                        basedatainfo = cbll.GetBaseDataInfo(factoryid, awardscode, "LotteryType").First(c => c.codeid == awardscode);
                        awasName = basedatainfo.codename;
                        isLotteryFlag = true;
                    }
                    #endregion

                    if (isLotteryFlag)
                    {
                        #region 中奖
                        try
                        {
                            //短信预计信息
                            string lottosmsstr = string.Empty;

                            lotteryLevel = awardscode;
                            lotteryName = awasName;

                            userHsah.Add("LOTTERYLEVEL", awardscode);

                            bool lotteryflag = false;
                            string strlid = string.Empty;

                            //类别（1:话费、2：红包、3：流量、4：虚拟券、5：实物礼品、6：旅游）
                            if (!string.IsNullOrEmpty(basedatainfo.subtype))
                            {
                                RegexExpress.LotteryType lotterytypeinfo = (RegexExpress.LotteryType)Enum.Parse(typeof(RegexExpress.LotteryType), basedatainfo.subtype);
                                switch (lotterytypeinfo)
                                {

                                    case RegexExpress.LotteryType.lotterySW:
                                        #region 实物礼品抽奖
                                        userHsah.Add("STATE", "9");
                                        result = cbll.FormatResult(msg.HasBeenWinningSW, awasName, basedatainfo.memo, "", "", "", "");
                                        lotteryflag = lbll.AddRechargeLotterySW(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, awardscode, dbproductid, result, newcode, userHsah, CustomerSystemState.Y中奖SW.GetEnumDescription(), isSpCode, out strlid);
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (lotteryflag)
                            {
                                lid = strlid;
                                systemstate = CustomerSystemState.Y中奖SW.GetEnumDescription();
                                return true;
                            }
                            else
                            {
                                result = msg.DigitalIsNotWinning;
                                systemstate = CustomerSystemState.NN未中奖.GetEnumDescription();
                                lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                                return true;
                            }

                        }
                        catch (Exception ex)
                        {
                            KMSLotterySystemFront.Logger.AppLog.Write("数码：“" + digitcode + "”抽奖运算异常：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
                            result = defaultmsg.ErrorOfSystem;
                            systemstate = CustomerSystemState.N系统异常.GetEnumDescription();
                            lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, poolId, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);
                            lbll.sendPrewarning(1, factoryid, result + ex.ToString());
                            return false;
                        }
                        #endregion
                    }
                }

                result = msg.DigitalIsNotWinning;
                userHsah.Add("LOTTERYLEVEL", "0");
                systemstate = CustomerSystemState.NN未中奖_未命中奖池.GetEnumDescription();
                lbll.AddLotteryParLog2(factoryid, mobile, digitcode, channel, ayinfo.Activityid, newpoolid, cityName, "0", dbproductid, result, newcode, false, true, userHsah, true, systemstate);

                return false;

            }
            catch (Exception exm)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("数码：[" + digitcode + "]抽奖运算异常：" + exm.TargetSite + "--" + exm.StackTrace + "--" + exm.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, exm);
                result = defaultmsg.ErrorOfSystem;
                systemstate = CustomerSystemState.N系统异常.GetEnumDescription();
                lbll.AddLotteryLog(factoryid, mobile, digitcode, channel, "0", cityName, "0", dbproductid, result, userHsah, true, systemstate);
                lbll.sendPrewarning(1, factoryid, result + exm.ToString());
                return false;
            }
        }

        /// <summary>
        /// openid是否参与过活动
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <param name="systemState"></param>
        /// <returns></returns>
        public Dictionary<string, string> SelectOpenidKMS(string factoryid, string openid, out string systemState)
        {
            systemState = "0000";
            DataTable dtKK = lbll.SelectRegister(factoryid, string.Format(" AND X.openid = '{0}' ", openid));


            Dictionary<string, string> dicResult = null;

            if (dtKK != null && dtKK.Rows != null && dtKK.Rows.Count > 0)
            {
                dicResult = new Dictionary<string, string>();
                dicResult.Add("mobile", dtKK.Rows[0]["IP"].ToString());
                dicResult.Add("openid", dtKK.Rows[0]["OPENID"].ToString());
                dicResult.Add("lid", dtKK.Rows[0]["GUID"].ToString());
                dicResult.Add("lotteryLevel", dtKK.Rows[0]["LOTTERYLEVEL"].ToString());
                systemState = CustomerSystemState.Yopenid存在记录.GetEnumDescription();
            }
            else
            {
                systemState = CustomerSystemState.NNopenid不存在记录.GetEnumDescription();
            }

            return dicResult;
        }

        /// <summary>
        /// 更新邮寄信息和STATE=9的状态
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="digitcode"></param>
        /// <param name="lid"></param>
        /// <param name="mobile"></param>
        /// <param name="systemState"></param>
        /// <returns></returns>
        public bool ModifyPostAdrKMS(string factoryid, string digitcode, string lid, string mobile, Hashtable userHash, out string systemState)
        {

            DataTable dtKK = lbll.SelectRegister(factoryid, string.Format(" and x.spro = '{0}' and x.ip='{1}' and x.guid='{2}' ", digitcode, mobile, lid));
            if (dtKK != null && dtKK.Rows != null && dtKK.Rows.Count > 0)
            {
                if (dtKK.Rows[0]["STATE"].ToString() == "1")
                {
                    systemState = CustomerSystemState.Y已填写邮寄地址.GetEnumDescription();     //邮寄信息已经填写
                    return false;
                }
            }
            else
            {
                systemState = CustomerSystemState.NN未查询到中奖记录.GetEnumDescription();   //未查询到中奖记录
                return false;
            }

            systemState = "";
            bool bRet = lbll.UpdateSgmRegister_kms(factoryid, digitcode, mobile, lid, userHash);
            if (bRet)
            {
                systemState = CustomerSystemState.Y成功.GetEnumDescription();  //更新成功
            }
            else
            {
                systemState = CustomerSystemState.NN失败.GetEnumDescription();  //更新失败
            }

            return false;
        }


        #endregion

    }
}
