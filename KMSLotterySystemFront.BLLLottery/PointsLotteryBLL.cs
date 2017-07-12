// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery 
// *文件名称：PointsLotteryBLL
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 13:52:29  
//
// *创建标识：
// *创建描述：
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
    public class PointsLotteryBLL
    {
        #region 公共变量
        public readonly static PointsLotteryDal ldao = new PointsLotteryDal();
        public readonly static UserDhLotteryDal udao = new UserDhLotteryDal();
        #endregion

        #region 1) 获取GUID
        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <returns></returns>
        public string getGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        #endregion

        #region 2) 新增积分抽奖参与记录
        /// <summary>
        /// 新增积分抽奖参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="points">消耗积分</param>
        /// <param name="result">参与积分抽奖活动答复</param>
        /// <param name="joindate">参与时间</param>
        /// <returns></returns>
        public bool AddPointsShakeLog(string facid, string ip, string userguid,string userid, string channel, string activityId, string area, string awardsno, string points, string result,string joindate)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[userid:" + userid + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[points:" + points + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                return ldao.AddPointsShakeLog(guid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:AddPointsShakeLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 新增积分抽奖参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="points">消耗积分</param>
        /// <param name="result">参与积分抽奖活动答复</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool AddPointsShakeLog(string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string awardsno, string points, string result, string joindate,string poolid)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                string pguid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[pguid:" + pguid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[userid:" + userid + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[points:" + points + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[poolid:" + poolid + "]");
                #endregion

                return ldao.AddPointsShakeLog(guid, pguid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate, poolid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:AddPointsShakeLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 3) 新增积分抽奖中奖记录
        /// <summary>
        /// 新增积分抽奖中奖记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="points">消耗积分</param>
        /// <param name="result">参与积分抽奖活动答复</param>
        /// <param name="joindate">参与时间</param>
        /// <returns></returns>
        public bool AddPointsLotteryLog(string facid, string ip, string userguid, string userid, string channel, string activityId,string poolid, string area, string awardsno, string points, string result, string joindate)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[userid:" + userid + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[points:" + points + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                return ldao.AddPointsLotteryLog(guid, facid, ip, userguid, userid, channel, activityId, poolid, area, awardsno, points, result, joindate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:AddPointsLotteryLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 4) 新增积分摇奖参与日志
        /// <summary>
        /// 新增积分抽奖中奖记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="lotterytype">类别(1:抽奖 0:摇奖)</param>
        /// <param name="points">消耗积分</param>
        /// <param name="result">参与积分抽奖活动答复</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="isModifyUserPoints">是否修改用户积分分值和添加扣减分值记录</param>
        /// <returns></returns>
        public bool AddPointsErnieLog(string facid, string ip, string userguid, string userid, string channel, string activityId, string area,string lotterytype, string points,string result, string joindate,string poolid,bool isModifyUserPoints)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                string guid2 = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[userid:" + userid + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[points:" + points + "]");
                sb.Append("[joindate:" + joindate + "]");
                sb.Append("[lotterytype:" + lotterytype + "]");
                sb.Append("[poolid:" + poolid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[isModifyUserPoints:" + isModifyUserPoints.ToString() + "]");

                #endregion

                return ldao.AddPointsErnieLog(guid, guid2, facid, ip, userguid, userid, channel, activityId, area, lotterytype, points, joindate, poolid, result, isModifyUserPoints);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:AddPointsErnieLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 5) 添加参与抽奖积分扣减记录
        /// <summary>
        /// 添加参与抽奖积分扣减记录
        /// </summary>
        /// <param name="guid">guid</param>
        /// <param name="userguid">userguid</param>
        /// <param name="points">消耗积分分值</param>
        /// <param name="lotterytype">活动类型(1:抽奖,0:摇奖)</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="ip">IP</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool AddPointsDhLottery(string userguid, int points, string lotterytype, string channel, string activityId, string ip, string facid)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[points:" + points.ToString() + "]");
                sb.Append("[lotterytype:" + lotterytype + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                #endregion

                return udao.AddPointsDhLottery(guid, userguid, points, lotterytype, channel, activityId, ip, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("UserDhLotteryBLL:AddPointsDhLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 6) 获取积分系统用户信息
        /// <summary>
        /// 获取积分系统用户信息
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="userifo">输出:用户信息</param>
        /// <returns>获取信息是否成功</returns>
        public bool GetUserPointsinfo(string userid, string facid, out Userinfo userinfo)
        {
            userinfo = null;
            bool bRet = false;
            try
            {
                DataTable usertable = udao.GetSgmUserPoint(userid, facid);

                if (usertable != null && usertable.Rows.Count == 1)
                {
                    userinfo = new Userinfo();
                    userinfo.Userid = usertable.Rows[0]["USERID"].ToString();
                    userinfo.Userguid = usertable.Rows[0]["USERGUID"].ToString();
                    userinfo.Usermobile = usertable.Rows[0]["MOBILE"].ToString();
                    userinfo.Username = usertable.Rows[0]["USERNAME"].ToString();
                    userinfo.Userpointtotal = Convert.ToInt32(usertable.Rows[0]["POINTTOTAL"].ToString());
                    userinfo.Userpointvalid = Convert.ToInt32(usertable.Rows[0]["POINTVALID"].ToString());
                    userinfo.Userpointused = Convert.ToInt32(usertable.Rows[0]["POINTUSED"].ToString());
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("UserDhLotteryBLL:GetUserPointsinfo:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return bRet;
        }
        #endregion

        #region 7) 用户参于积分抽奖活动

        /// <summary>
        /// 积分抽奖相关信息保存
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="lotterytype">类别(1:抽奖 0:摇奖)</param>
        /// <param name="points">扣减积分分值</param>
        /// <param name="result">答复</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="isModifyLottery">是否中奖</param>
        /// <param name="isModifyUserPoints">是否添加积分扣减记录和是否扣减用户积分分值</param>
        /// <returns>记录添加修改是否成功</returns>
        public bool UserPointLottery(string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string awardsno,string lotterytype, string points, string result, string joindate, string poolid, string smscontent, bool smsflag, bool isModifyLottery, bool isModifyUserPoints)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string sguid = getGuid();
                string lguid = getGuid();
                string dguid = getGuid();
                string pguid = getGuid();

                sb.Append("[sguid:" + sguid + "]");
                sb.Append("[lguid:" + lguid + "]");
                sb.Append("[dguid:" + dguid + "]");
                sb.Append("[pguid:" + pguid + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[userid:" + userid + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[lotterytype:" + lotterytype + "]");
                sb.Append("[points:" + points + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[joindate:" + joindate + "]");
                sb.Append("[poolid:" + poolid + "]");
                sb.Append("[smscontent:" + smscontent + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[isModifyLottery:" + isModifyLottery + "]");
                sb.Append("[isModifyUserPoints:" + isModifyUserPoints + "]");

                #endregion

                return ldao.ModifyUserPointByLottery(sguid, lguid, dguid, pguid, facid, ip, userguid, userid, channel, activityId, lotterytype, area, awardsno, points, result, joindate, poolid, smscontent, smsflag, isModifyLottery, isModifyUserPoints);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:UserPointLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 8) 参与活动次数加1
        /// <summary>
        /// 参与活动次数加1
        /// </summary>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool ModifyAwardNum(string facid, string activityid, string poolid)
        {
            try
            {
                return ldao.ModifyPointsAwardNum(facid, activityid, poolid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:ModifyAwardNum:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 9) 更新中奖数量
        /// <summary>
        /// 更新中奖数量
        /// </summary>
        /// <param name="facid">厂加编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="awardscode">奖项ID</param>
        /// <returns></returns>
        public bool ModifyLotteryNum(string facid, string poolid, string awardscode)
        {
            try
            {
                return ldao.ModifyLotteryNum(facid, poolid, awardscode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:ModifyLotteryNum:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 10) 用户参于积分摇奖活动

        /// <summary>
        /// 积分抽奖相关信息保存
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="lotterytype">类别(1:抽奖 0:摇奖)</param>
        /// <param name="points">扣减积分分值</param>
        /// <param name="result">参与积分摇奖活动答复</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="isModifyUserPoints">是否添加积分扣减记录和是否扣减用户积分分值</param>
        /// <returns>记录添加修改是否成功</returns>
        public bool UserPointErnie(string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string lotterytype, string points,string result, string joindate, string poolid, string smscontent, bool smsflag,  bool isModifyUserPoints)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string sguid = getGuid();
                string dguid = getGuid();
                string pguid = getGuid();

                sb.Append("[sguid:" + sguid + "]");
                sb.Append("[pguid:" + pguid + "]");
                sb.Append("[dguid:" + dguid + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[userid:" + userid + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[lotterytype:" + lotterytype + "]");
                sb.Append("[points:" + points + "]");
                sb.Append("[joindate:" + joindate + "]");
                sb.Append("[poolid:" + poolid + "]");
                sb.Append("[smscontent:" + smscontent + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[isModifyUserPoints:" + isModifyUserPoints + "]");

                #endregion

                return ldao.ModifyUserPointByErnie(sguid, pguid, dguid, facid, ip, userguid, userid, channel, activityId, lotterytype, area, points, result, joindate, poolid, smscontent, smsflag, isModifyUserPoints);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:UserPointErnie:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 11) 万金 抽奖成功则减掉中奖所需分值
        public bool UserPointLottery_WJ(string facid, string userguid, string poolguid)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("[facid:" + facid + "]");
                sb.Append("[userguid:" + userguid + "]");
                sb.Append("[poolguid:" + poolguid + "]");

                string points = "0";
                DataTable dtGiftAward = ldao.GetGiftAwardByPoolGuid(facid, poolguid);
                if (dtGiftAward != null)
                {
                    points = dtGiftAward.Rows[0]["GIFTPOINT"].ToString();
                    return ldao.ModifyUserPointByLottery_WJ(facid, userguid, points);
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryBLL:UserPointLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return false;
        }
        #endregion
    }
}
