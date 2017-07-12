// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery 
// *文件名称：UserDhLotteryBLL
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 14:59:28  
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
    public class UserDhLotteryBLL
    {
        #region 公共变量
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

        #region 2) 添加参与抽奖积分扣减记录
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

        #region 3) 获取积分系统用户信息
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

                if (usertable != null && usertable.Rows.Count > 0)
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

        #region 4)
        public bool ModifyRegisterInfo(string facid, string lid, string mobile, string postname, string postaddr)
        {
            return udao.ModifyRegisterInfo(facid, lid, mobile, postname, postaddr);
        }
        #endregion
    }
}
