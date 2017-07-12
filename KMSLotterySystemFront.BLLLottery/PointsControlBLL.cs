// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery 
// *文件名称：PointsControl
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 11:03:23  
//
// *创建标识：PointsControlBLL.cs
// *创建描述：积分抽奖活动业务逻辑处理
//
// *修改信息：
// *修改备注：
// ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.DAL;
using KMSLotterySystemFront.Model;
using System.Data;

namespace KMSLotterySystemFront.BLLLottery
{
    public class PointsControlBLL
    {

        #region 公共变量
        public readonly static PointsControlDal ClDao = new PointsControlDal();
        #endregion

        #region 积分抽奖

        #region 1) 获取积分抽奖活动总控
        /// <summary>
        /// 获取积分抽奖活动总控
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="channel">参与渠道</param>
        /// <param name="type">类型0:积分抽奖活动 1:数码抽奖活动 </param>
        /// <param name="ayInfo">输出：活动总控信息</param>
        /// <returns>是否存在活动</returns>
        public bool GetPointsService(string facid, string channel, string type, out PointsActivity ayInfo)
        {
            bool bRet = false;
            ayInfo = null;
            try
            {
                DataTable dsRet = ClDao.GetPointsService(facid, channel, type);

                if (dsRet != null && dsRet.Rows.Count > 0)
                {
                    ayInfo = new PointsActivity();
                    ayInfo.Activityid = dsRet.Rows[0]["ACTIVITYID"].ToString();
                    ayInfo.Activityname = dsRet.Rows[0]["ACTIVITYNAME"].ToString();
                    ayInfo.Activityfacid = dsRet.Rows[0]["FACID"].ToString();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:GetPointsService:" + facid + "---" + channel + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 2) 获取活动是否开始
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="cytype">抽奖摇奖类别(0:抽奖 1:摇奖 2:数码抽奖)</param>
        /// <param name="poolid">输出:奖池编号</param>
        /// <param name="points">输出:参与所需用积分</param>
        /// <returns></returns>
        public bool GetPointsActivityByAID(string facid, string activityid, string type, string cytype, out string poolid, out int points)
        {
            poolid = "";
            points = 0;
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetPointsAwardRecord(activityid, facid, cytype);
                }
                else
                {
                    activitydtrecord = ClDao.GetPointsAwardRecord2(activityid, facid, cytype);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    bRet = true;
                    poolid = activitydtrecord.Rows[0]["POOLID"].ToString();
                    points = Convert.ToInt32(activitydtrecord.Rows[0]["POINTS"].ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetActivityByAID:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 2.1) 获取活动是否开始(PID)
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">检查类型</param>
        /// <param name="cytype">抽奖摇奖类别(0:抽奖 1:摇奖 2:数码抽奖)</param>
        /// <param name="poolid">输出:奖池编号</param>
        /// <param name="points">输出:参与所需用积分</param>
        /// <returns></returns>
        public bool GetPointsActivityByAPID(string facid, string activityid, string pid, string type, string cytype, out string poolid, out int points)
        {
            poolid = "";
            points = 0;
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetPointsAwardRecordbyPid(activityid, facid, pid, cytype);
                }
                else
                {
                    activitydtrecord = ClDao.GetPointsAwardRecordbyPid2(activityid, facid, pid, cytype);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    bRet = true;
                    poolid = activitydtrecord.Rows[0]["POOLID"].ToString();
                    points = Convert.ToInt32(activitydtrecord.Rows[0]["POINTS"].ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetPointsActivityByAPID:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 2.2) 获取活动是否开始

        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="cytype">抽奖摇奖类别(0:抽奖 1:摇奖 2:数码抽奖)</param>
        /// <param name="poolid">输出:奖池编号</param>
        /// <param name="points">输出:参与所需用积分</param>
        /// <param name="totalpop">输出:当前参与总人数</param>
        /// <param name="smstimes">输出:短信提醒次数阈值</param>
        /// <param name="maxlotterytimes">输出:最大中奖次数</param>
        /// <param name="cycletimes">输出:周期数量</param>
        /// <param name="cycle">输出:周期(年Y 月M 日D 周W)</param>
        /// <param name="maxjointimes">输出:参与最大次数</param>
        /// <param name="acitvityname">输出:活动名称</param>
        /// <param name="activityStartData">输出:活动开始日期</param>
        /// <param name="activityEndDate">输出:活动结束日期</param>
        /// <returns></returns>
        public bool GetPointsActivityByAID(string facid, string activityid, string type, string cytype, out string poolid, out int points, out int totalpop, out int smstimes, out int maxlotterytimes, out int cycletimes, out string cycle, out int maxjointimes, out string acitvityname, out string activityStartData, out string activityEndDate)
        {
            poolid = "";
            points = 0;
            totalpop = 0;
            smstimes = 0;
            maxlotterytimes = 0;
            cycletimes = 0;
            maxjointimes = 0;
            cycle = "";
            acitvityname = "";
            activityStartData = "";
            activityEndDate = "";



            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetPointsAwardRecord(activityid, facid, cytype);
                }
                else
                {
                    activitydtrecord = ClDao.GetPointsAwardRecord2(activityid, facid, cytype);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    bRet = true;
                    poolid = activitydtrecord.Rows[0]["POOLID"].ToString();
                    points = Convert.ToInt32(activitydtrecord.Rows[0]["POINTS"].ToString());
                    totalpop = Convert.ToInt32(activitydtrecord.Rows[0]["TOTALPOP"].ToString());
                    smstimes = Convert.ToInt32(activitydtrecord.Rows[0]["SMSTIMES"].ToString());
                    maxlotterytimes = Convert.ToInt32(activitydtrecord.Rows[0]["MAXLOTTERYTIMES"].ToString());
                    cycletimes = Convert.ToInt32(activitydtrecord.Rows[0]["CYCLETIMES"].ToString());
                    maxjointimes = Convert.ToInt32(activitydtrecord.Rows[0]["MAXJOINTIMES"].ToString());
                    cycle = activitydtrecord.Rows[0]["CYCLE"].ToString();
                    acitvityname = activitydtrecord.Rows[0]["ACTIVITYNAME"].ToString();
                    activityStartData = activitydtrecord.Rows[0]["STARTDATE"].ToString();//活动开始日期
                    activityEndDate = activitydtrecord.Rows[0]["ENDDATE"].ToString();//活动结束日期
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetActivityByAID:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 3) 获取奖池
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="type">0:积分抽奖,1:积分摇奖,2:数码抽奖</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetPointsAward(string facid, string activityid, string type, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetPointsAward(facid, activityid, type);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:GetPointsAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 3.1) 获取奖池(PID)
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">0:积分抽奖,1:积分摇奖,2:数码抽奖</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetPointsAward(string facid, string activityid, string pid, string type, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetPointsAwardbyPid(facid, activityid, pid, type);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:GetPointsAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 4) 检查用户参与活动的最大中奖次数
        /// <summary>
        /// 检查用户参与活动的最大中奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">用户ID</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonthData"></param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolId">奖池编号</param>
        /// <returns></returns>
        public bool CheckPointsUserPrizeMaxNum(string facid, string userid, int MaxNum, string StartData, string EndDate, string lastmonthData, string activityid, string poolId)
        {
            bool bRet = false;
            try
            {
                int number = ClDao.CheckPointsUserPrizeMaxNum(facid, userid, StartData, EndDate, lastmonthData, activityid, poolId);
                if (number >= MaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:CheckUserPrizeMaxNum:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 5) 检查用户参与活动的最大参与次数
        /// <summary>
        /// 检查用户参与活动的最大参与次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">用户ID</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonth"></param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool CheckPointsSendMaxNumber(string facid, string userid, int MaxNum, string StartData, string EndDate, string lastmonth, string activityId, string poolid)
        {
            bool bRet = false;
            try
            {
                int number = ClDao.CheckPointsSendMaxNumber(facid, userid, StartData, EndDate, lastmonth, activityId, poolid);
                if (number >= MaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:CheckSendMaxNumber:" + facid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 7) 获取今日实际中奖数量
        /// <summary>
        /// 获取今日实际中奖数量
        /// </summary>
        /// <param name="poolid">奖池编号</param>
        /// <param name="awardScode">奖项编号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public int GetPointsDayMaxLottery(string poolid, string awardScode, string facid)
        {

            int maxlotterynumber = 9999999;
            try
            {
                object oRet = ClDao.GetPointsDayMaxLottery(poolid, awardScode, facid);

                if (!string.IsNullOrEmpty(oRet.ToString()))
                {
                    maxlotterynumber = Convert.ToInt32(oRet.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:GetPointsDayMaxLottery:" + facid + "---" + poolid + "---" + awardScode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return maxlotterynumber;
            }
            return maxlotterynumber;
        }
        #endregion

        #region 8) 检查用户参与积分抽奖的最大中奖次数
        /// <summary>
        /// 检查用户参与积分抽奖的最大中奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">用户ID</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonthData"></param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolId">奖池编号</param>
        /// <returns></returns>
        public bool CheckErnieUserPrizeMaxNum(string facid, string userid, int MaxNum, string StartData, string EndDate, string lastmonthData, string activityid, string poolId)
        {
            bool bRet = false;
            try
            {
                int number = ClDao.CheckErnieUserPrizeMaxNum(facid, userid, StartData, EndDate, lastmonthData, activityid, poolId);
                if (number >= MaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:CheckUserPrizeMaxNum:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 5) 检查用户参与积分抽奖的最大参与次数
        /// <summary>
        /// 检查用户参与积分抽奖的最大参与次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">用户ID</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonth"></param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool CheckErnieSendMaxNumber(string facid, string userid, int MaxNum, string StartData, string EndDate, string lastmonth, string activityId, string poolid)
        {
            bool bRet = false;
            try
            {
                int number = ClDao.CheckErnieSendMaxNumber(facid, userid, StartData, EndDate, lastmonth, activityId, poolid);
                if (number >= MaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:CheckSendMaxNumber:" + facid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 9) 万金 获取抽奖积分是否在所需积分范围之内
        /// <summary>
        /// 检查 用户积分是否在抽奖所需积分范围之内
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="type"></param>
        /// <param name="userPointValid">用户可用积分</param>
        /// <returns></returns>
        public bool CheckPointsIsScope_WJ(string facid,string activityid, string poolid,string type, int userPointValid)
        {
            try
            {
                DataTable activitydtrecord = ClDao.GetPointsRulebyPid_WJ(facid, activityid, poolid, type);
                if (activitydtrecord.Rows.Count > 0)
                {
                    int minPointScope = Convert.ToInt32(activitydtrecord.Rows[0]["MINPOINTSSCOPE"]);
                    int maxPointScope = Convert.ToInt32(activitydtrecord.Rows[0]["MAXPOINTSSCOPE"]);
                    if (userPointValid >= minPointScope)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckPointsIsScope_WJ:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 9.1) 万金 获取活动是否开始
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="cytype">抽奖摇奖类别(0:抽奖 1:摇奖 2:数码抽奖)</param>
        /// <param name="poolid">输出:奖池编号</param>
        /// <param name="points">输出:参与所需用积分</param>
        /// <returns></returns>
        public bool GetPointsActivityByAID_WJ(string facid, string gpid, string pid, out string poolid, out int points)
        {
            poolid = "";
            points = 0;
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = activitydtrecord = ClDao.GetPointsAwardRecord_WJ(gpid, facid, pid);
                
                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    bRet = true;
                    poolid = activitydtrecord.Rows[0]["POOLID"].ToString();
                    points = Convert.ToInt32(activitydtrecord.Rows[0]["POINTS"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetPointsActivityByAID_WJ:" + facid + "---" + gpid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 9.2) 万金 获取奖池(PID)
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">0:积分抽奖,1:积分摇奖,2:数码抽奖</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetPointsAward_WJ(string facid, string activityid, string pid,string gpid, string type, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetPointsAwardbyPid_WJ(facid, activityid, pid,gpid, type);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlBLL:GetPointsAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion
        #endregion
    }
}
