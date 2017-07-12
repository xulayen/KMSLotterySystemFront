// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery 
// *文件名称：ECouponBll
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-9-22 13:29:23  
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
    public class ECouponBll
    {
        #region 公共变量
        public readonly static ECouponDao ClDao = new ECouponDao();
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

        #region 1) 获取活动总控
        /// <summary>
        /// 获取活动总控
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="channel">参与渠道</param>
        /// <param name="ayInfo">输出：活动总控信息</param>
        /// <returns>是否存在活动</returns>
        public bool GetService(string facid, string channel, out ActivityInfo ayInfo)
        {
            bool bRet = false;
            ayInfo = null;
            try
            {
                DataTable dsRet = ClDao.GetService(facid, channel);

                if (dsRet != null && dsRet.Rows.Count > 0)
                {
                    ayInfo = new ActivityInfo();
                    ayInfo.Activityid = dsRet.Rows[0]["ACTIVITYID"].ToString();
                    ayInfo.Activityname = dsRet.Rows[0]["ACTIVITYNAME"].ToString();
                    ayInfo.Activityfacid = dsRet.Rows[0]["FACID"].ToString();
                    ayInfo.Activityproid = dsRet.Rows[0]["PROID"].ToString();
                    //ayInfo.Productmark = dsRet.Rows[0]["PROMARK"].ToString().Trim();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ECouponBll:GetService:" + facid + "---" + channel + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 2) 判断该数码是否已经参与电子优惠卷活动
        /// <summary>
        /// 判断该数码是否已经参与电子优惠卷活动
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <returns></returns>
        public bool CheckECouponParLogByCode(string factoryid, string digitcode, out string EcouponCode, out string State)
        {
            bool bRet = false;
            EcouponCode = "";
            State = "";
            try
            {
                DataTable ShakeParData = null;

                ShakeParData = ClDao.GetECouponQueryParLog(digitcode, factoryid);

                if (ShakeParData != null && ShakeParData.Rows.Count > 0)
                {
                    State = ShakeParData.Rows[0]["STATE"].ToString();
                    EcouponCode = ShakeParData.Rows[0]["SNCODE"].ToString();
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ECouponBll:CheckECouponParLogByCode:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 3) 添加电子优惠卷参与记录
        /// <summary>
        /// 新增参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编码</param>
        /// <param name="area">参与区域</param>
        /// <param name="protype">产品类别</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="result">下行答复信息</param>
        /// <returns>参与记录是否增加成功</returns>
        public bool AddECouponLog(string facid, string ip, string digitCode, string channel, string area, string proid, string result,string sncode)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                return ClDao.AddECouponLog(guid, facid, ip, digitCode, channel, area, proid, result, sncode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 4) 判断在当前时间是否存在电子优惠卷活动
        /// <summary>
        /// 判断在当前时间是否存在电子优惠卷活动
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <returns>是否存在活动</returns>
        public bool GetECouponRecord(string facid)
        {
            bool bRet = false;
            
            try
            {
                DataTable dsRet = ClDao.GetECouponRecord(facid);

                if (dsRet != null && dsRet.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ECouponBll:GetECouponRecord:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion
    }
}
