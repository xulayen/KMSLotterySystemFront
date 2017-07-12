// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery
// *文件名称：ControlBLL.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：活动相关业务逻辑类
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
using KMSLotterySystemFront.Common;
using System.Collections;

namespace KMSLotterySystemFront.BLLLottery
{
    public class ControlBLL
    {
        #region 公共变量
        public readonly static ControlDao ClDao = new ControlDao();
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
                    ayInfo.Productmark = dsRet.Rows[0]["PROMARK"].ToString().Trim();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetService:" + facid + "---" + channel + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
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
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAID(string facid, string activityid, string type, out string poolid)
        {
            poolid = "";
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    bRet = true;
                    poolid = activitydtrecord.Rows[0]["poolid"].ToString();
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

        #region 2) 获取活动是否开始
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAIDYD(string facid, string activityid, string type, out DataTable dtPools)
        {
            dtPools = null;
            bool bRet = false;
            try
            {
                if (type.Equals("1"))
                {
                    dtPools = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    dtPools = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    dtPools = ClDao.GetAwardRecord2(activityid, facid);
                }

                if (dtPools != null && dtPools.Rows != null && dtPools.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetActivityByAIDYD:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 2) 根据新老用户获取活动是否开始
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAIDAndNewUser(string facid, string activityid, bool isOldUser, string dbproductid, string type, out string poolid)
        {
            poolid = "";
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    List<DataRow> drList = null;

                    if (isOldUser)
                    {
                        var c = from row in activitydtrecord.AsEnumerable() where !row.IsNull("ACTIVITYNAME") && row.Field<string>("ACTIVITYNAME") == "R5EOld" && !row.IsNull("PROTYPE") && row.Field<string>("PROTYPE").IndexOf(dbproductid) > -1 select row;
                        drList = c.ToList<DataRow>();

                        //activitydtrecord = Utility.ToDataTable(activitydtrecord.Select(" ACTIVITYNAME LIKE 'R5E old' and instr(PROTYPE,'" + dbproductid + "')>0 "));
                    }
                    else
                    {
                        var c = from row in activitydtrecord.AsEnumerable() where !row.IsNull("ACTIVITYNAME") && row.Field<string>("ACTIVITYNAME") == "R5ENew" && !row.IsNull("PROTYPE") && row.Field<string>("PROTYPE").IndexOf(dbproductid) > -1 select row;
                        drList = c.ToList<DataRow>();
                        //activitydtrecord = Utility.ToDataTable(activitydtrecord.Select(" ACTIVITYNAME LIKE 'R5E new' and instr(PROTYPE,'" + dbproductid + "')>0  "));
                    }
                    if (drList != null && drList.Count > 0)
                    {
                        bRet = true;
                        poolid = drList[0]["poolid"].ToString();
                    }

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


        #region 2) 根据新老用户获取活动是否开始
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAIDAnd69Code(string facid, string activityid, bool Is69Code, string dbproductid, string type, out string poolid)
        {
            poolid = "";
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    List<DataRow> drList = null;

                    if (Is69Code)
                    {
                        var c = from row in activitydtrecord.AsEnumerable() where !row.IsNull("ACTIVITYNAME") && row.Field<string>("ACTIVITYNAME") == "Shell易到69" && !row.IsNull("PROTYPE") && row.Field<string>("PROTYPE").IndexOf(dbproductid) > -1 select row;
                        drList = c.ToList<DataRow>();
                    }
                    else
                    {
                        var c = from row in activitydtrecord.AsEnumerable() where !row.IsNull("ACTIVITYNAME") && row.Field<string>("ACTIVITYNAME") == "Shell易到" && !row.IsNull("PROTYPE") && row.Field<string>("PROTYPE").IndexOf(dbproductid) > -1 select row;
                        drList = c.ToList<DataRow>();
                    }
                    if (drList != null && drList.Count > 0)
                    {
                        bRet = true;
                        poolid = drList[0]["poolid"].ToString();
                    }

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

        #region 根据外在条件判断获取规则
        /// <summary>
        /// 获取活动是否开始（根据外在条件判断获取规则）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="productType">产品编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAIDAndProductType(string facid, string activityid, string productType, string type, out string poolid)
        {
            Logger.AppLog.Write("ControlBLL:GetActivityByAIDAndProductType:productType=" + productType, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            poolid = "";
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                Logger.AppLog.Write("ControlBLL:GetActivityByAIDAndProductType:activitydtrecord=" + activitydtrecord.Rows.Count, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    //List<DataRow> drList = null;

                    //var c = from row in activitydtrecord.AsEnumerable() where !row.IsNull("PROTYPE") && row.Field<string>("PROTYPE").IndexOf(productType) > -1 select row;
                    //drList = c.ToList<DataRow>();

                    //if (drList != null && drList.Count > 0)
                    //{
                    //    bRet = true;
                    //    poolid = drList[0]["poolid"].ToString();
                    //}

                    foreach (DataRow dr in activitydtrecord.Rows)
                    {
                        string dtb = dr["PROTYPE"].ToString().Trim();//产品类型ID
                        Logger.AppLog.Write("dtb=" + dtb + "|| productType=" + productType, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                        if (("," + dtb + ",").IndexOf("," + productType + ",") > -1)
                        {
                            bRet = true;
                            poolid = dr["poolid"].ToString();
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetActivityByAIDAndProductType:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 根据外在条件判断获取规则
        /// <summary>
        /// 获取活动是否开始（根据外在条件判断获取规则）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="productType">产品编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAIDAndBatchid(string facid, string activityid, string batchid, string type, out string poolid)
        {
            Logger.AppLog.Write("ControlBLL:GetActivityByAIDAndBatchid:batchid=" + batchid, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            poolid = "";
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                Logger.AppLog.Write("ControlBLL:GetActivityByAIDAndBatchid:activitydtrecord=" + activitydtrecord.Rows.Count, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    //List<DataRow> drList = null;

                    //var c = from row in activitydtrecord.AsEnumerable() where !row.IsNull("PROTYPE") && row.Field<string>("PROTYPE").IndexOf(productType) > -1 select row;
                    //drList = c.ToList<DataRow>();

                    //if (drList != null && drList.Count > 0)
                    //{
                    //    bRet = true;
                    //    poolid = drList[0]["poolid"].ToString();
                    //}

                    foreach (DataRow dr in activitydtrecord.Rows)
                    {
                        string dtb = dr["BATCHID"].ToString().Trim();//产品类型ID
                        Logger.AppLog.Write("dtb=" + dtb + "|| batchid=" + batchid, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                        if (("," + dtb + ",").IndexOf("," + batchid + ",") > -1)
                        {
                            bRet = true;
                            poolid = dr["poolid"].ToString();
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetActivityByAIDAndBatchid:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
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
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAID(string facid, string activityid, string type, string batchid, string proid, out string poolid, out bool batchjoin)
        {
            poolid = "";
            batchjoin = true;
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {

                    if (!string.IsNullOrEmpty(proid))
                    {
                        activitydtrecord = Utility.ToDataTable(activitydtrecord.Select(string.Format(" PROTYPE='{0}' ", proid)));
                    }
                    if (activitydtrecord != null && activitydtrecord.Rows != null && activitydtrecord.Rows.Count > 0)
                    {
                        foreach (DataRow dr in activitydtrecord.Rows)
                        {
                            string dtb = dr["BATCHID"].ToString().Trim();//产品类型ID
                            if (("," + dtb + ",").IndexOf("," + batchid + ",") > -1)
                            {
                                bRet = true;
                                poolid = dr["poolid"].ToString();
                                break;
                            }
                        }
                    }
                    if (!bRet)
                    {
                        batchjoin = false;
                    }
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


        #region 2) 获取活动是否开始
        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="type">检查类型</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool GetActivityByAID(string facid, string activityid, string type, string batchid, out string poolid, out bool batchjoin)
        {
            poolid = "";
            batchjoin = true;
            bool bRet = false;
            try
            {
                DataTable activitydtrecord = null;
                if (type.Equals("1"))
                {
                    activitydtrecord = ClDao.GetAwardRecord(activityid, facid);
                }
                else if (type.Equals("2"))
                {
                    activitydtrecord = ClDao.GetAwardRecord3(activityid, facid);
                }
                else
                {
                    activitydtrecord = ClDao.GetAwardRecord2(activityid, facid);
                }

                if (activitydtrecord != null && activitydtrecord.Rows.Count > 0)
                {
                    if (activitydtrecord != null && activitydtrecord.Rows != null && activitydtrecord.Rows.Count > 0)
                    {
                        foreach (DataRow dr in activitydtrecord.Rows)
                        {
                            string dtb = dr["BATCHID"].ToString().Trim();//产品类型ID
                            if (("," + dtb + ",").IndexOf("," + batchid + ",") > -1)
                            {
                                bRet = true;
                                poolid = dr["poolid"].ToString();
                                break;
                            }
                        }
                    }
                    if (!bRet)
                    {
                        batchjoin = false;
                    }
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

        #region 3) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <returns>数码是否参与过抽奖</returns>
        public bool CheckShakeParLogByCode(string facid, string digitcode)
        {
            bool bRet = false;
            try
            {
                DataTable ShakeParData = null;

                ShakeParData = ClDao.GetQueryParLog(digitcode, facid);

                if (ShakeParData != null && ShakeParData.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckShakeParLogByCode:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 3) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <returns>数码是否参与过抽奖</returns>
        public bool CheckShakeParLogByCode2(string facid, string digitcode)
        {
            bool bRet = false;
            try
            {
                DataTable ShakeParData = null;

                ShakeParData = ClDao.GetQueryParLog2(digitcode, facid);

                if (ShakeParData != null && ShakeParData.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckShakeParLogByCode:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion


        #region 3) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <returns>数码是否参与过抽奖</returns>
        public bool CheckShakeParLogByCode3(string facid, string digitcode)
        {
            bool bRet = false;
            try
            {
                DataTable ShakeParData = null;

                ShakeParData = ClDao.GetQueryParLog3(digitcode, facid);

                if (ShakeParData != null && ShakeParData.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckShakeParLogByCode:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 3.1 检查数码是否属于特殊数码参与范围

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="isExiet">是否使用过</param>
        /// <param name="mobile">使用ben</param>
        /// <returns></returns>
        public bool CheckSpCodeExiet(string facid, string digitcode, out bool isExiet, out string mobile)
        {
            bool bRet = false;
            isExiet = false;
            mobile = "";

            try
            {
                DataTable spCodeData = null;
                string ues = string.Empty;
                spCodeData = ClDao.GetSpCode(digitcode, facid);

                if (spCodeData != null && spCodeData.Rows.Count > 0)
                {
                    bRet = true;
                    mobile = spCodeData.Rows[0]["MOBILE"].ToString();
                    ues = spCodeData.Rows[0]["USE"].ToString().Trim();
                }

                if (ues == "1")
                    isExiet = true;

                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckSpCodeExiet:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion

        #region 4) 获取奖池
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetAward(string facid, string activityid, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetAward(facid, activityid);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 4) 获取奖池
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetAward2(string facid, string activityid, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetAward2(facid, activityid);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 4) 获取奖池(宣顶)
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetAward(string facid, string activityid, string poolid, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetAward(facid, activityid);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    awardData = Utility.ToDataTable(awardData.Select(string.Format(" poolid='{0}' ", poolid)));
                    if (awardData != null && awardData.Rows.Count > 0)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 4.1) 获取奖池（杜邦）
        /// <summary>
        /// 获取奖池（杜邦）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">奖池ID</param>
        /// <param name="awardData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetAwardDupont(string facid, string activityid, out DataTable awardData)
        {
            bool bRet = false;
            awardData = null;
            try
            {
                awardData = ClDao.GetAwardDupont(facid, activityid);
                if (awardData != null && awardData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 5) 获取今日实际中奖数量
        /// <summary>
        /// 获取今日实际中奖数量
        /// </summary>
        /// <param name="poolid">奖池编号</param>
        /// <param name="awardScode">奖项编号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public int GetDayMaxLottery(string poolid, string awardScode, string facid)
        {

            int maxlotterynumber = 9999999;
            try
            {
                object oRet = ClDao.GetDayMaxLottery(poolid, awardScode, facid);

                if (!string.IsNullOrEmpty(oRet.ToString()))
                {
                    maxlotterynumber = Convert.ToInt32(oRet.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetDayMaxLottery:" + facid + "---" + poolid + "---" + awardScode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return maxlotterynumber;
            }
            return maxlotterynumber;
        }
        #endregion

        #region 6) 获取活动的N个月的日期
        /// <summary>
        /// 获取活动的N个月的日期
        /// </summary>
        /// <param name="StartData">开始时间</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public string GetPreMonth(string StartData, string number)
        {
            string mnumber = string.Empty;
            try
            {
                mnumber = ClDao.GetPreMonth(StartData, number);
            }
            catch (Exception ex)
            {

                Logger.AppLog.Write("ControlBLL:GetPreMonth:" + StartData + "---" + number + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return mnumber;
            }
            return mnumber;
        }
        #endregion

        #region 7) 检查用户参与活动的最大中奖次数
        /// <summary>
        /// 检查用户参与活动的最大中奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonthData"></param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolId">奖池编号</param>
        /// <returns></returns>
        public bool CheckUserPrizeMaxNum(string facid, string ip, int MaxNum, string StartData, string EndDate, string lastmonthData, string activityid, string poolId)
        {
            bool bRet = false;
            try
            {
                int number = ClDao.CheckUserPrizeMaxNum(facid, ip, StartData, EndDate, lastmonthData, activityid, poolId);
                if (number >= MaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckUserPrizeMaxNum:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 8) 检查用户参与活动的最大参与次数
        /// <summary>
        /// 检查用户参与活动的最大参与次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonth"></param>
        /// <param name="activityId">活动编号</param>
        /// <returns></returns>
        public bool CheckSendMaxNumber(string facid, string ip, int MaxNum, string StartData, string EndDate, string lastmonth, string activityId)
        {
            bool bRet = false;
            try
            {
                int number = ClDao.CheckSendMaxNumber(facid, ip, StartData, EndDate, lastmonth, activityId);
                if (number >= MaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckSendMaxNumber:" + facid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 9) 检查防伪码是否已经中过奖
        /// <summary>
        /// 检查防伪码是否已经中过奖
        /// </summary>
        /// <param name="digitCode">数码</param>
        /// <returns></returns>
        public bool CkeckDigitalHasBeenWinning(string digitCode, string facid)
        {
            bool bRet = false;
            try
            {
                object code = ClDao.CkeckDigitalHasBeenWinning(digitCode, facid);
                if (code != null)
                {
                    if (!string.IsNullOrEmpty(code.ToString()))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CkeckDigitalHasBeenWinning:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 10) 检查防伪码是否已经中过奖
        /// <summary>
        /// 检查防伪码是否已经中过奖
        /// </summary>
        /// <param name="digitCode"></param>
        /// <returns></returns>
        public bool CkeckDigitalHasBeenWinning(string digitCode, string facid, string activityId)
        {
            //CkeckDigitalHasBeenWinning

            bool bRet = false;
            try
            {
                object code = ClDao.CkeckDigitalHasBeenWinning(digitCode, facid, activityId);
                if (code != null)
                {
                    if (!string.IsNullOrEmpty(code.ToString()))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CkeckDigitalHasBeenWinning:" + facid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        /// <summary>
        /// 检查防伪码是否已经中过奖 包含为lotterylevel=0的数据
        /// </summary>
        /// <param name="digitCode"></param>
        /// <returns></returns>
        public bool CkeckDigitalHasBeenWinningContains0(string digitCode, string facid)
        {
            //CkeckDigitalHasBeenWinning

            bool bRet = false;
            try
            {
                DataTable dt = ClDao.CkeckDigitalHasBeenWinningContains0(digitCode, facid);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    return true;//已中奖
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CkeckDigitalHasBeenWinningContains0:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        /// <summary>
        /// 检查防伪码是否已经中过奖
        /// </summary>
        /// <param name="digitCode"></param>
        /// <returns></returns>
        public bool CkeckDigitalHasBeenWinning(string digitCode, string facid, string activityId, out string awardsno)
        {

            awardsno = string.Empty;
            bool bRet = false;
            try
            {
                DataTable list = ClDao.CkeckDigitalHasBeenWinningRed(digitCode, facid, activityId);
                if (list != null && list.Rows.Count > 0)
                {
                    awardsno = list.Rows[0]["AWARDSNO"].ToString();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CkeckDigitalHasBeenWinning:" + facid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 11.1) 获取奖项
        /// <summary>
        /// 获取奖项
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">活动编号</param>
        /// <param name="datatypename">类型</param>
        /// <returns></returns>
        public string GetLotteryName(string facid, string codeid, string datatypename)
        {
            string lotteryname = string.Empty;
            try
            {
                object lname = ClDao.GetLotteryName(facid, codeid, datatypename);
                if (!string.IsNullOrEmpty(lname.ToString()))
                {
                    lotteryname = lname.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLotteryName:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "---facid--" + facid + "--codeid--" + codeid + "-----datatypename---" + datatypename, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
                return lotteryname;
            }
            return lotteryname;
        }
        #endregion

        #region 11.2) 获取充值金额
        /// <summary>
        /// 获取奖项
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">活动编号</param>
        /// <param name="datatypename">类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public string GetLotteryName2(string facid, string codeid, string datatypename, string fieldName)
        {
            string lotteryname = string.Empty;
            try
            {
                object Money = ClDao.GetLotteryName2(facid, codeid, datatypename, fieldName);
                if (!string.IsNullOrEmpty(Money.ToString()))
                {
                    lotteryname = Money.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLotteryName:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return lotteryname;
            }
            return lotteryname;
        }
        #endregion

        #region 11.3) 获取充值金额
        /// <summary>
        /// 获取奖项
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">活动编号</param>
        /// <param name="datatypename">类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public string GetLotteryName3(string facid, string datatypename, string fieldName)
        {
            string lotteryname = string.Empty;
            try
            {
                object Money = ClDao.GetLotteryName3(facid, datatypename, fieldName);
                if (!string.IsNullOrEmpty(Money.ToString()))
                {
                    lotteryname = Money.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLotteryName:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return lotteryname;
            }
            return lotteryname;
        }
        #endregion

        #region 11.5) 获取奖项信息
        /// <summary>
        /// 获取奖项
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">活动编号</param>
        /// <param name="datatypename">类型</param>
        /// <returns></returns>
        public List<ShakeBaseData> GetBaseDataInfo(string facid, string codeid, string datatypename)
        {
            List<ShakeBaseData> basedatalist = new List<ShakeBaseData>();

            try
            {
                ShakeBaseData basedatainfo = null;
                DataTable lotteryinfo = ClDao.GetLotteryInfo(facid, codeid, datatypename);

                if (lotteryinfo != null && lotteryinfo.Rows.Count > 0)
                {
                    for (int i = 0; i < lotteryinfo.Rows.Count; i++)
                    {
                        basedatainfo = new ShakeBaseData();
                        basedatainfo.subtype = lotteryinfo.Rows[i]["SUBTYPE"].ToString();
                        basedatainfo.lotterymoeny = lotteryinfo.Rows[i]["LOTTERYMOENY"].ToString();
                        basedatainfo.memo = lotteryinfo.Rows[i]["MEMO"].ToString();
                        basedatainfo.codename = lotteryinfo.Rows[i]["NAME"].ToString();//
                        basedatainfo.codeid = lotteryinfo.Rows[i]["CODEID"].ToString();

                        basedatalist.Add(basedatainfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetBaseDataInfo:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "---facid--" + facid + "--codeid--" + codeid + "-----datatypename---" + datatypename, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
                return basedatalist;
            }
            return basedatalist;
        }
        #endregion

        #region 根据手机号码和金额获得流量平台的奖项编号

        public string GetAPPLhLottery(KMSLotterySystemFront.Common.RegexExpress.MobileType mobiletype, string money, out string packnum)
        {
            string codeid = string.Empty;
            packnum = string.Empty;
            try
            {
                switch (mobiletype)
                {
                    case KMSLotterySystemFront.Common.RegexExpress.MobileType.DianXin:
                        switch (money)
                        {
                            case "1": codeid = "100005"; packnum = "5"; break;
                            case "2": codeid = "100010"; packnum = "10"; break;
                            case "5": codeid = "100030"; packnum = "30"; break;
                            case "7": codeid = "100050"; packnum = "50"; break;
                            case "10": codeid = "100100"; packnum = "100"; break;
                            case "15": codeid = "100200"; packnum = "200"; break;
                            case "30": codeid = "100500"; packnum = "500"; break;
                            case "50": codeid = "101024"; packnum = "1024"; break;
                            default:
                                break;
                        }
                        break;
                    case KMSLotterySystemFront.Common.RegexExpress.MobileType.LianTong:
                        switch (money)
                        {
                            case "3": codeid = "100020"; packnum = "20"; break;
                            case "6": codeid = "100050"; packnum = "50"; break;
                            case "10": codeid = "100100"; packnum = "100"; break;
                            case "15": codeid = "100200"; packnum = "200"; break;
                            case "30": codeid = "100500"; packnum = "500"; break;
                            case "100": codeid = "101024"; packnum = "1024"; break;
                            default:
                                break;
                        }
                        break;
                    case KMSLotterySystemFront.Common.RegexExpress.MobileType.YiDong:
                        switch (money)
                        {
                            case "3": codeid = "100010"; packnum = "10"; break;
                            //case "4": codeid = "100020"; packnum = "20"; break;
                            //case "6": codeid = "100050"; packnum = "50"; break;
                            case "5": codeid = "100030"; packnum = "30"; break;
                            //case "10": codeid = "100070"; packnum = "70";break;
                            case "10": codeid = "100100"; packnum = "100"; break;
                            case "20": codeid = "100150"; packnum = "150"; break;
                            //case "20": codeid = "100200"; packnum = "200"; break;
                            case "30": codeid = "100500"; packnum = "500"; break;
                            case "50": codeid = "101024"; packnum = "1024"; break;
                            case "70": codeid = "102048"; packnum = "2048"; break;
                            case "100": codeid = "103072"; packnum = "3072"; break;
                            case "130": codeid = "104096"; packnum = "4096"; break;
                            case "180": codeid = "106144"; packnum = "6144"; break;
                            case "280": codeid = "111264"; packnum = "11264"; break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetAPPLhLottery(): ---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return "";
            }
            return codeid;
        }

        #endregion

        #region 11.4) 根据类型查询基础配置数据表
        /// <summary>
        ///根据类型查询基础配置数据表
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="datatypename">类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public DataTable GetBaseDataByDataType(string facid, string datatypename)
        {
            DataTable dt = null;
            try
            {
                dt = ClDao.GetBaseDataByDataType(facid, datatypename);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetBaseDataByDataType():" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return dt;
        }
        #endregion

        #region 根据监控平台的活动编号检查当前活动是否允许发送流量
        /// <summary>
        /// 根据监控平台的活动编号检查当前活动是否允许发送流量
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="aid">活动编号</param>
        /// <param name="money">金额</param>
        /// <param name="systemstate">输出:状态</param>
        /// <returns></returns>
        public bool ChekJkAactity(string facid, string aid, string money, out string systemstate)
        {
            systemstate = "000";
            bool bRet = false;
            try
            {
                /*
               *  001 正常
               *  002 活动未注册
               *  003 活动已经关闭
               *  004 不再授权许可范围
               *  005 超额
               * */


                string strflag = string.Empty;


                DataTable dt = ClDao.GetJkAactity(facid, aid);


                if (dt != null && dt.Rows.Count > 0)
                {
                    strflag = dt.Rows[0]["FLAG"].ToString();

                    if (string.IsNullOrEmpty(strflag))
                    {
                        systemstate = "004";
                        return false;
                    }
                    else
                    {
                        if (strflag.Equals("0"))
                        {
                            systemstate = "004";
                            return false;
                        }
                    }

                    if (!(DateTime.Now >= Convert.ToDateTime(dt.Rows[0]["BENGINDATE"].ToString()) && DateTime.Now <= Convert.ToDateTime(dt.Rows[0]["ENDDATE"].ToString())))
                    {
                        systemstate = "004";
                        return false;
                    }

                    //Double Sum_Moeny = Convert.ToDouble(dt.Rows[0]["BALANCEACCOUNT"].ToString()) - Convert.ToDouble(dt.Rows[0]["THRESHOLD"].ToString()); //=剩余金额 - 预警金额

                    //发送金额不能小于当前剩余金额
                    Double Sum_Money2 = Convert.ToDouble(dt.Rows[0]["BALANCEACCOUNT"].ToString()) - Convert.ToDouble(money);

                    if (Sum_Money2 < 0)
                    {
                        systemstate = "005";
                        return false;
                    }
                    else
                        return true;

                }
                else
                {
                    systemstate = "002";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLotteryName:" + facid + "---" + aid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                systemstate = "006";
                return bRet;
            }
        }

        #endregion

        #region 答复替换

        /// <summary>
        /// 中奖信息答复替换
        /// </summary>
        /// <param name="Result">答复消息</param>
        /// <param name="JxName">奖项名称</param>
        /// <param name="JPName">奖品名称</param>
        /// <param name="Money">获奖金额</param>
        /// <param name="XNQcode">虚拟券号</param>
        /// <param name="FindTime">查询时间</param>
        /// <param name="LotteryTime">获奖时间</param>
        /// <returns></returns>
        public string FormatResult(string Result, string JxName, string JPName, string Money, string XNQcode, string FindTime, string LotteryTime)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Result);
                if (!string.IsNullOrEmpty(JxName))
                {
                    sb.Replace("[奖项]", JxName);
                }
                if (!string.IsNullOrEmpty(JPName))
                {
                    sb.Replace("[奖品]", JPName);
                }
                if (!string.IsNullOrEmpty(Money))
                {
                    sb.Replace("[金额]", Money);
                }

                if (!string.IsNullOrEmpty(XNQcode))
                {
                    sb.Replace("[虚拟券]", XNQcode);
                }

                if (!string.IsNullOrEmpty(FindTime))
                {
                    sb.Replace("[查询时间]", FindTime);
                }

                if (!string.IsNullOrEmpty(LotteryTime))
                {
                    sb.Replace("[中奖时间]", LotteryTime);
                }

                return sb.ToString();

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:FormatResult:---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Result;
            }
        }

        #endregion

        #region 12) 获取已经中将的奖项名称
        /// <summary>
        /// 获取已经中将的奖项名称
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项等级</param>
        /// <param name="datatypename">数据字典名称</param>
        /// <param name="digitcode">数码</param>
        /// <param name="activityid">活动编号</param>
        /// <returns>返回已中奖的奖项名称</returns>
        public string GetLotteryName(string facid, string datatypename, string digitcode, string activityid)
        {
            string lotteryname = string.Empty;
            try
            {
                object lname = ClDao.GetLotteryName(facid, datatypename, digitcode, activityid);
                if (lname != null)
                {
                    if (!string.IsNullOrEmpty(lname.ToString()))
                    {
                        lotteryname = lname.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLotteryName:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return lotteryname;
            }
            return lotteryname;
        }
        #endregion

        #region 12.1) 获取已经中将的奖项级别
        /// <summary>
        /// 获取已经中将的奖项级别
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项等级</param>
        /// <param name="datatypename">数据字典名称</param>
        /// <param name="digitcode">数码</param>
        /// <param name="activityid">活动编号</param>
        /// <returns>返回已中奖的奖项名称</returns>
        public string GetLotteryLevel(string facid, string datatypename, string digitcode, string activityid)
        {
            string lotterylevel = string.Empty;
            try
            {
                object llevel = ClDao.GetLotteryLevel(facid, datatypename, digitcode, activityid);
                if (llevel != null)
                {
                    if (!string.IsNullOrEmpty(llevel.ToString()))
                    {
                        lotterylevel = llevel.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLotteryLevel:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return lotterylevel;
            }
            return lotterylevel;
        }
        #endregion

        #region 13) 获取数码的产品信息(参与活动权限)
        /// <summary>
        /// 获取数码的产品信息(参与活动权限)
        /// </summary>
        /// <param name="facid">厂家ID</param>
        /// <param name="proid">产品编号</param>
        /// <returns></returns>
        public bool GetProductInfo(string facid, string proid)
        {
            bool flag = false;
            try
            {
                DataTable shakedata = ClDao.CheckShakeFacProduct(facid, proid);
                if (shakedata != null && shakedata.Rows.Count > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {

                Logger.AppLog.Write("ControlBLL:GetProductInfo:" + facid + "---" + proid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion

        #region 14) 获取活动的N个月的日期
        /// <summary>
        /// 获取活动的N个月的日期
        /// </summary>
        /// <param name="StartData">开始时间</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public string GetPreTime(string StartData, string cycleType, string cycleNumber)
        {
            StringBuilder sb = new StringBuilder();
            int number = 1;
            string mnumber = string.Empty;
            try
            {
                #region 判断限定值
                if (string.IsNullOrEmpty(cycleNumber))
                {
                    number = 1;
                }
                else
                {
                    try
                    {
                        number = Convert.ToInt32(cycleNumber);
                    }
                    catch (Exception)
                    {
                        number = 1;
                    }
                }
                #endregion

                #region 判断限定类型
                if (string.IsNullOrEmpty(cycleType))
                {
                    switch (cycleType)
                    {
                        case "Y":
                            break;
                        case "M":

                            break;
                        case "D":

                            break;
                        case "W":
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                //mnumber = ClDao.GetPreMonth(StartData, cycleNumber);
            }
            catch (Exception ex)
            {

                Logger.AppLog.Write("ControlBLL:GetPreTime:" + StartData + "---" + cycleType + "---" + cycleNumber + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return mnumber;
            }
            return mnumber;
        }
        #endregion

        #region  15)根据code 和openID 查询数码注册信息表 （T_SGM_SHAKE_REGISTERUSER_9999  ）
        /// <summary>
        ///  根据code 和openID 查询数码注册信息表 
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="digitcode"></param>
        /// <returns></returns>
        public DataTable GetLottryCodeInfo(string factoryid, string digitcode)
        {
            DataTable dt = null;
            try
            {
                dt = ClDao.GetLottryCodeInfo(factoryid, digitcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLottryCodeInfo:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }

        #endregion

        /// <summary>
        /// 检查手机号码是否参与过抽奖
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="openid">微信粉丝号</param>
        /// <returns>手机是否参与过抽奖</returns>
        public bool CheckTheOpenidHave(string facid, string openid)
        {
            bool bRet = false;
            try
            {
                DataTable ShakeParData = null;

                ShakeParData = ClDao.CheckTheOpenidHave(facid, openid);

                if (ShakeParData != null && ShakeParData.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckTheMobileHave:" + facid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #region 16) 获取活动产品范围
        /// <summary>
        /// 获取活动产品范围
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="awardProructData">输出奖池信息</param>
        /// <returns></returns>
        public bool GetActiovityProduct(string facid, out DataTable awardProructData)
        {
            bool bRet = false;
            awardProructData = null;
            try
            {

                if (DataCache.GetCache(facid + "ActiovityProduct") != null)
                {
                    awardProructData = DataCache.GetCache(facid + "ActiovityProduct") as DataTable;
                }
                else
                {
                    awardProructData = ClDao.GetActiovityProduct(facid);

                    if (awardProructData != null && awardProructData.Rows.Count > 0)
                    {
                        DataCache.SetCache(facid + "ActiovityProduct", awardProructData);
                    }
                }

                //if (BaseDataCache.GetTable(facid + "TableActiovityProduct") != null)
                //{
                //    awardProructData = DataCache.GetCache(facid + "TableActiovityProduct") as DataTable;
                //}
                //else
                //{
                //    awardProructData = ClDao.GetActiovityProduct(facid);

                //    if (awardProructData != null && awardProructData.Rows.Count > 0)
                //    {
                //        BaseDataCache.SetTable(awardProructData, facid + "TableActiovityProduct");
                //    }
                //}




                if (awardProructData != null && awardProructData.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetActiovityProduct:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 17) 检查没日中奖数量是否超过上限 不区分奖池

        /// <summary>
        /// 检查用户参与活动的最大中奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="MaxNum">设定的允许最大参与次数</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="lastmonthData"></param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolId">奖池编号</param>
        /// <returns></returns>
        public bool CheckPrizeMaxNumByNowDay(string facid, string activityid)
        {
            bool bRet = false;
            try
            {
                int DbMaxNum = ClDao.GetMaxLotteryByNowDayConfig(facid, activityid);
                int LogNumber = ClDao.GetMaxLotteryByNowDay(facid, activityid);
                if (LogNumber >= DbMaxNum)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckPrizeMaxNumByNowDay:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }



        #endregion


        #region 新平台

        /// <summary>
        /// 检查手机号码是否参与过抽奖
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>手机是否参与过抽奖</returns>
        public bool CheckTheMobileHave(string facid, string mobile)
        {
            bool bRet = false;
            try
            {
                DataTable ShakeParData = null;

                ShakeParData = ClDao.CheckTheMobileHave(facid, mobile);

                if (ShakeParData != null && ShakeParData.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckTheMobileHave:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }


        /// <summary>
        ///  验证门店ID是否符合参与范围
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool CheckStoreID(string facid, string storeid)
        {
            bool bRet = false;
            try
            {
                object obj = ClDao.CheckStoreID(facid, storeid);

                if (obj != null)
                {
                    if (obj.ToString().Trim().Equals(storeid))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStoreID:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }


        #endregion

        #region 检查当天门店中奖总数量是否超过限定数量
        /// <summary>
        ///  检查当天门店中奖总数量是否超过限定数量
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool CheckStoreLotteryByDataNow(string facid, string storeid)
        {
            bool bRet = false;
            try
            {
                object obj = ClDao.CheckStoreLotteryByDataNow(facid, storeid);

                object MaxLotteryNun = ClDao.GetStoreMaxLotteryNum(facid, storeid);

                if (MaxLotteryNun != null && obj != null)
                {
                    int imax = Convert.ToInt32(MaxLotteryNun);
                    int ilottery = Convert.ToInt32(obj);

                    if (imax == 0)
                    {
                        bRet = true;
                    }
                    else
                    {
                        if (imax - ilottery >= 1)
                        {
                            bRet = false;
                        }
                        else
                        {
                            bRet = true;
                        }
                    }
                }
                else
                {
                    bRet = true;
                }

                Logger.AppLog.Write("ControlBLL:CheckStoreLotteryByDataNow [facid:" + facid + "] [storeid:" + storeid + "]  [bRet:" + bRet + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStoreLotteryByDataNow:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return true;
            }
            return bRet;
        }
        #endregion

        #region 获取门店允许最大中奖次数
        /// <summary>
        /// 获取门店允许最大中奖次数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public string GetStoreLimitMaxlottery(string facid, string storeid)
        {
            object MaxLotteryNun = "0";

            try
            {
                MaxLotteryNun = ClDao.GetStoreMaxLotteryNum(facid, storeid);

                Logger.AppLog.Write("ControlBLL:GetStoreLimitMaxlottery [facid:" + facid + "] [storeid:" + storeid + "]  [MaxLotteryNun:" + MaxLotteryNun + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetStoreLimitMaxlottery:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return MaxLotteryNun.ToString();

        }
        #endregion

        /// <summary>
        ///  验证门店ID是否符合参与范围    壳牌喜力超凡项目
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool CheckStoreIDXLCF(string facid, string storeid)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckStoreIDXLCF(facid, storeid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStoreIDXLCF:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        /// <summary>
        ///  检测门店ID是否有资格参与活动 (通用版本)
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool CheckStoreIsJoinActivity(string facid, string storeid, out string systemstate)
        {
            bool bRet = false;
            systemstate = "000";
            try
            {
                bRet = ClDao.CheckStoreIsJoinActivity(facid, storeid, out systemstate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStoreIsJoinActivity:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        /// <summary>
        ///  检测手机号是否参与过活动，以及是否过中奖
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckMobileIsJoinActivity(string facid, string mobile, out string result, out string systemstate)
        {
            bool bRet = false;
            systemstate = "000";
            result = "";
            try
            {
                bRet = ClDao.CheckMobileIsJoinActivity(facid, mobile, out result, out systemstate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckMobileIsJoinActivity: [facid:" + facid + "][mobile:" + mobile + "] " + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #region 检测手机号是否有资格参与活动
        /// <summary>
        ///  检测手机号是否有资格参与活动
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="timelimit">时间限制</param>
        /// <param name="result">返回:答复</param>
        /// <param name="systemstate">返回:系统执行状态</param>
        /// <returns></returns>
        public bool CheckMobileJoinActivityLimit(string factoryid, string mobile, string timelimit, out string result, out  string systemstate)
        {
            systemstate = "000";
            result = "查询失败";
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckMobileJoinActivityLimit(factoryid, mobile, timelimit, out  result, out systemstate);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBLL  CheckMobileJoinActivityLimit()  检测手机号是否有资格参与活动:  factoryid:【" + factoryid + "】 mobile:【" + mobile + "】 【timelimit：" + timelimit + "】  ：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return bRet;
        }
        #endregion

        #region 检测微信号是否有资格参与活动
        /// <summary>
        ///  检测微信号是否有资格参与活动
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="openid">微信号</param>
        /// <param name="timelimit">时间限制</param>
        /// <param name="result">返回:答复</param>
        /// <param name="systemstate">返回:系统执行状态</param>
        /// <returns></returns>
        public bool CheckOpenidJoinActivityLimit(string factoryid, string openid, string timelimit, out string result, out  string systemstate)
        {
            systemstate = "000";
            result = "查询失败";
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckOpenidJoinActivityLimit(factoryid, openid, timelimit, out  result, out systemstate);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBLL  CheckOpenidJoinActivityLimit()  检测微信号是否有资格参与活动:  【 factoryid: " + factoryid + "】【 openid:" + openid + "】 【timelimit：" + timelimit + "】  ：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return bRet;
        }
        #endregion


        #region 检测防伪数码是否有资格参与活动
        /// <summary>
        ///  检测防伪数码是否有资格参与活动
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitalcode">防伪数码</param>
        /// <param name="timelimit">时间限制</param>
        /// <param name="result">返回:答复</param>
        /// <param name="systemstate">返回:系统执行状态</param>
        /// <returns></returns>
        public bool CheckDigitalCodeJoinActivityLimit(string factoryid, string digitalcode, string timelimit, out string result, out  string systemstate)
        {
            systemstate = "000";
            result = "查询失败";
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckDigitalCodeJoinActivityLimit(factoryid, digitalcode, timelimit, out  result, out systemstate);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBLL  CheckDigitalCodeJoinActivityLimit()  检测防伪数码是否有资格参与活动:  【 factoryid: " + factoryid + "】【 digitalcode:" + digitalcode + "】 【timelimit：" + timelimit + "】  ：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return bRet;
        }
        #endregion

        #region  15)根据code 和openID 查询数码注册信息表 （T_SGM_SHAKE_REGISTERUSER_9999  ）
        /// <summary>
        ///  根据code 和openID 查询数码注册信息表 
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="digitcode"></param>
        /// <returns></returns>
        public DataTable GetLottryCodeInfo2(string factoryid, string lid)
        {
            DataTable dt = null;
            try
            {
                dt = ClDao.GetLottryCodeInfo2(factoryid, lid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLottryCodeInfo2:" + factoryid + "---" + lid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }

        #endregion

        #region  16)根据 openID 查询数码注册信息表 （T_SGM_SHAKE_REGISTERUSER_9999  ）
        /// <summary>
        ///  根据code 和openID 查询数码注册信息表 
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="digitcode"></param>
        /// <returns></returns>
        public DataTable GetLottryCodeInfoByOpenId(string factoryid, string openid)
        {
            DataTable dt = null;
            try
            {
                dt = ClDao.GetLottryCodeInfoByOPENID(factoryid, openid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLottryCodeInfo2:" + factoryid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }

        #endregion


        #region  16.2)根据手机号是否参与过扫码活动 （T_SGM_SHAKE_REGISTERUSER_9999  ）
        /// <summary>
        ///  根据手机号是否参与过扫码活动
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool GetLottryCodeInfoByMobile(string factoryid, string mobile, out DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {
                bRet = ClDao.GetLottryCodeInfoByMobile(factoryid, mobile, out dt);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:GetLottryCodeInfoByMobile:" + factoryid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
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
                bRet = ClDao.CheckOpenidHbSendLimit(facid, openid, datetype, limitnum);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBll  限制openID每月/每天/每年/ 参与红包发放的最大次数 CheckOpenidHbSendLimit 异常：  [facid:" + facid + "] [openid:" + openid + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion

        #region 限制手机号每月/每天/每年/ 参与红包发放的最大次数
        /// <summary>
        /// 限制手机号每月/每天/每年/ 参与红包发放的最大次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="datetype">限制类型（N:不区分 D：每天  M：每月  Y：每年）</param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public bool CheckMobileHbSendLimit(string facid, string mobile, string datetype, int limitnum)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckMobileHbSendLimit(facid, mobile, datetype, limitnum);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBll  限制手机号每月/每天/每年/ 参与红包发放的最大次数 CheckMobileHbSendLimit 异常：  [facid:" + facid + "] [mobile:" + mobile + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion

        #region 限制手机号每月/每天/每年/ 邀请朋友最大次数
        /// <summary>
        /// 限制手机号每月/每天/每年/ 邀请朋友最大次数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="datetype"></param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public bool CheckInviteFriendLimit(string facid, string mobile, string datetype, int limitnum)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckInviteFriendLimit(facid, mobile, datetype, limitnum);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBll  限制手机号每月/每天/每年/ 邀请朋友最大次数 CheckInviteFriendLimit 异常：  [facid:" + facid + "] [mobile:" + mobile + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }

        #endregion

        #region 限制手机号每月/每天/每年/ 邀请同一个人最大次数
        /// <summary>
        /// 限制手机号每月/每天/每年/ 邀请同一个人最大次数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile">邀请人</param>
        /// <param name="to">被邀请人</param>
        /// <param name="datetype"></param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public bool CheckInviteFriendLimit(string facid, string mobile, string to, string datetype, int limitnum)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckInviteFriendLimit(facid, mobile, to, datetype, limitnum);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBll  限制手机号每月/每天/每年/ 邀请朋友最大次数 CheckInviteFriendLimit 异常：  [facid:" + facid + "] [mobile:" + mobile + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }

        #endregion

        #region 检测邀请的好友是否参与过活动

        /// <summary>
        /// 检测邀请的好友是否参与过活动
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="to">被邀请人</param>
        /// <returns></returns>
        public bool CheckInviteFriendIsJoin(string facid, string to)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckInviteFriendIsJoin(facid, to);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBll 检测邀请的好友是否参与过活动 CheckInviteFriendIsJoin 异常：  [facid:" + facid + "] [to:" + to + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }


        #endregion

        #region  20)根据 openID+数码 提交心愿单信息 （T_SGM_SHAKE_REGISTERUSER_9999  ）
        /// <summary>
        /// 根据 openID+数码 提交心愿单信息 
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="openid">openid</param>
        /// <param name="xydcode">心愿单答案</param>
        /// <returns></returns>
        public bool BLLAddLotteryInfo(string factoryid, string digitcode, string openid, string xydcode)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.DalAddLotteryInfo(factoryid, digitcode, openid, xydcode) == 1 ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:BLLAddLotteryInfo:" + factoryid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 根据员工编号以及手机号检验用户是否存在
        /// <summary>
        /// 根据员工编号以及手机号检验用户是否存在
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid"></param>
        /// <param name="mobile"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool CheckStaffIsExsit(string facid, string userid, string mobile, out  DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckStaffIsExsit(facid, userid, mobile, out dt);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStaffIsExsit: 【facid:" + facid + "】【userid:" + userid + "】【mobile:" + mobile + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion

        #region 根据员工编号检验用户是否存在
        /// <summary>
        ///  根据员工编号/手机号 检验用户是否存在
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool CheckStaffIsExsit2(string facid, string userid, out  DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckStaffIsExsit2(facid, userid, out dt);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStaffIsExsit2: 【facid:" + facid + "】【userid:" + userid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion

        #region 员工授权绑定
        /// <summary>
        /// 员工授权绑定
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool UserAuthBind(string facid, Hashtable userHash)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.UserAuthBind(facid, userHash);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:UserAuthBind: 【facid:" + facid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion

        #region 校验店员openid是否存在
        /// <summary>
        /// 校验店员openid是否存在
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool CheckStaffIsExsit(string facid, string openid, out  DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckStaffIsExsit(facid, openid, out dt);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStaffIsExsit: 【facid:" + facid + "】【openid:" + openid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }


        #endregion

        #region 校验好友openid是否已经领取过
        /// <summary>
        /// 校验好友openid是否已经领取过
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool CheckOpenidIsGetCard(string facid, string openid, out DataTable dt)
        {

            dt = null;
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckOpenidIsGetCard(facid, openid, out dt);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckOpenidIsGetCard: 【facid:" + facid + "】【openid:" + openid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;

        }

        #endregion

        #region 校验员工发放券的数量是否达到上限
        /// <summary>
        /// 校验员工发放券的数量是否达到上限
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid">员工编号</param>
        /// <param name="datetype"></param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public bool CheckStaffSendCardLimit(string facid, string userid, string datetype, int limitnum)
        {

            bool bRet = false;
            try
            {
                bRet = ClDao.CheckStaffSendCardLimit(facid, userid, datetype, limitnum);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:CheckStaffSendCardLimit: 【facid:" + facid + "】【userid:" + userid + "】【datetype:" + datetype + "】【limitnum:" + limitnum + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion

        #region 检测手机号是否预约过
        /// <summary>
        /// 检测手机号是否预约过
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <param name="dt">预约信息</param>
        /// <returns></returns>
        public bool GetReserveByMobile(string factoryid, string mobile, string channel, out DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {

                bRet = ClDao.GetReserveByMobile(factoryid, mobile, channel, out dt);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.ControlBLL.cs  GetReserveByMobile [factoryid:" + factoryid + "] [mobile:" + mobile + "] [channel:" + channel + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion


        #region 检测手机号是否预约过2
        /// <summary>
        /// 检测手机号是否预约过2
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <param name="dt">预约信息</param>
        /// <returns></returns>
        public bool GetReserveByMobile(string factoryid, string mobile, out DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {

                bRet = ClDao.GetReserveByMobile(factoryid, mobile, out dt);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.ControlBLL.cs  GetReserveByMobile [factoryid:" + factoryid + "] [mobile:" + mobile + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion



        #region 检测门店是否参与活动
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
                bRet = ClDao.CheckStoreIsJoinActivity(factoryid, storeid, out dt);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBLL.cs  CheckStoreIsJoinActivity [factoryid:" + factoryid + "] [storeid:" + storeid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;

        }
        #endregion

        #region 检测openid是否注册
        /// <summary>
        /// 检测openid是否注册
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <param name="dt_userinfo"></param>
        /// <param name="dt_userlotteryinfo"></param>
        /// <param name="dt_usermile"></param>
        /// <returns></returns>
        public bool CheckOpenidIsRegist(string factoryid, string openid, out DataTable dt_userinfo, out DataTable dt_userlotteryinfo, out DataTable dt_usermile)
        {
            bool bRet = false;
            dt_userinfo = null;
            dt_userlotteryinfo = null;
            dt_usermile = null;

            try
            {
                bRet = ClDao.CheckOpenidIsRegist(factoryid, openid, out dt_userinfo, out dt_userlotteryinfo, out dt_usermile);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBLL.cs  CheckOpenidIsRegist [factoryid:" + factoryid + "] [openid:" + openid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;

        }

        #endregion

        #region 检测openid是否存在注册表（免登录使用）
        /// <summary>
        /// 检测openid是否注册（免登录使用）
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <param name="dt_userinfo"></param>
        /// <param name="dt_userlotteryinfo"></param>
        /// <param name="dt_usermile"></param>
        /// <returns>true:存在 false:不存在</returns>
        public bool CheckOpenidIsRegist(string factoryid, string openid, out DataTable codeInfo)
        {
            bool bRet = false;
            codeInfo = null;
            try
            {
                bRet = ClDao.CheckOpenidIsRegist(factoryid, openid, out codeInfo);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.ControlBLL.cs  CheckOpenidIsRegist [factoryid:" + factoryid + "] [openid:" + openid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;

        }

        #endregion

        #region 获取活动用户领取话费信息
        /// <summary>
        /// 获取活动用户领取话费信息
        /// </summary>
        /// <param name="factoryid"></param>
        /// <returns></returns>
        public DataTable GetUserBillInfo(string factoryid)
        {
            DataTable dt = null;
            try
            {
                dt = ClDao.GetUserBillInfo(factoryid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.ControlBLL.cs  GetUserBillInfo:" + factoryid + "--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion

        #region 获取邀请好友参与情况
        /// <summary>
        /// 获取邀请好友参与情况
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataTable GetInviteFriendJoinInfo(string facid, string mobile)
        {

            DataTable dt = null;
            try
            {
                dt = ClDao.GetInviteFriendJoinInfo(facid, mobile);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.ControlBLL.cs  GetInviteFriendJoinInfo: [facid:" + facid + "][mobile:" + mobile + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;

        }
        #endregion

        #region 检测用户当月是否使用赠送的资格邀请过好友
        public bool CheckInviteFriendByType(string facid, string mobile, string invietype)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckInviteFriendByType(facid, mobile, invietype);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.ControlBLL.cs  CheckInviteFriendByType: [facid:" + facid + "][mobile:" + mobile + "] [invietype:" + invietype + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }


        #endregion


        #region 校验用户当月是否扫描同一类产品
        /// <summary>
        /// 校验用户当月是否扫描同一类产品
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="productid"></param>
        /// <returns></returns>
        public bool CheckScanProductAgain(string facid, string mobile, string productid)
        {
            bool bRet = false;
            try
            {
                bRet = ClDao.CheckScanProductAgain(facid, mobile, productid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.ControlBLL.cs  CheckScanProductAgain: [facid:" + facid + "][mobile:" + mobile + "] [productid:" + productid + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion

        #region 添加邮寄地址&更新用户可用升数、已用升数(工业油专用)
        public bool AddPostAddressAndModifyUser(Hashtable post, string facid, string mobile, string lotterylevel, out string systemState)
        {
            systemState = "000";
            string _t = GetLotteryName2(facid, lotterylevel, "LotteryType", "LOTTERYMOENY");  //获取当前奖项所需的升数

            string giftname = GetLotteryName2(facid, lotterylevel, "LotteryType", "NAME");  //giftname
            LotteryBLL lbll = new LotteryBLL();
            LotteryDal dal = new LotteryDal();
            DataTable dtU = lbll.SelectT_sgm_user(facid, mobile);
            if (dtU != null && dtU.Rows != null && dtU.Rows.Count > 0)
            {
                int valid = Convert.ToInt32(dtU.Rows[0]["POINTVALID"].ToString());
                int need = Convert.ToInt32(_t);
                if (need > valid)
                {
                    systemState = "104";//可用升数不足
                    return false;
                }

                bool c = dal.AddPostAddressAndModifyUser(post, facid, _t, mobile, dtU, giftname);

                if (c)
                {
                    systemState = "001";
                }
            }
            else
            {
                systemState = "105";//用户不存在
                return false;
            }


            return false;
        }
        #endregion

    }
}
