// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL 
// *文件名称：ECouponDao
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-9-22 13:35:54  
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
using KMSLotterySystemFront.Model;
using System.Data;
using System.Data.OracleClient;
using KMSLotterySystemFront.DBUtility;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront.DAL
{
    public class ECouponDao
    {
        #region sql常量
        private const string Query_Control_Sql = "SELECT A.FACID,A.PROID,A.ACTIVITYID,A.ACTIVITYNAME,A.STARTDATE,A.ENDDATE,A.CHANNEL,A.ACTIVITYTYPE,A.FLAG FROM T_SGM_WAPACTIVITY A WHERE A.FACID=:FACID AND A.CHANNEL LIKE '%{0}%' AND A.ACTIVITYTYPE='2' AND A.FLAG='1'";

        private static string TableName_PARLOG = TableNamePrefix() + "ECOUPON_PARLOG_";

        public const string Query_ParLog_Sql = "SELECT * FROM {0} L WHERE L.DIGIT=:DIGIT AND L.DELETEFLAG='1' AND L.FACID=:FACID";

        private const string ADD_ECoupon_LOG_SQL = " INSERT INTO {0}  (GUID,USERID,USERTYPE,UPCONTENT,DIGIT,AREA,JOINDATE,PROID,DOWNCONTENT,CREATEDAE,FACID,SNCODE) "
                                                + " VALUES(:GUID,:USERID,:USERTYPE,:UPCONTENT,:DIGIT,:AREA,:JOINDATE,:PROID,:DOWNCONTENT,:CREATEDAE,:FACID,:SNCODE)";

        private string Query_ECoupon_Sql = "SELECT * FROM T_SGM_WAPACTIVITY R WHERE R.FLAG='1' AND R.ACTIVITYTYPE='2' AND R.FACID=:FACID AND TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') BETWEEN R.STARTDATE AND R.ENDDATE";


        #endregion

        #region 1) 获取表头
        public static string TableNamePrefix()
        {
            return "t_sgm_wb_";
        }
        #endregion

        #region 2) 表合并
        /// <summary>
        /// 表合并
        /// </summary>
        /// <param name="table">表前缀</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        private string GetTable(string table, string facid)
        {
            string newfactoryid = string.Empty;
            if (DataCache.GetCache(facid + "_FactoryTable") != null)
            {
                newfactoryid = DataCache.GetCache(facid + "_FactoryTable") as string;
            }
            else
            {
                newfactoryid = GetNewFactoryTalbe(facid);
                if (string.IsNullOrEmpty(newfactoryid))
                {
                    newfactoryid = facid;
                }
                DataCache.SetCache(facid + "_FactoryTable", newfactoryid);
            }

            return table + newfactoryid;
        }


        /// <summary>
        /// 获取是否是缓存分合表厂家
        /// </summary>
        /// <param name="facid"></param>
        /// <returns></returns>
        public string GetNewFactoryTalbe(string facid)
        {
            string oRet = string.Empty;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetNewFactoryTalbeParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetNewFactoryTalbeParam", param);
                }
                param[0].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = "SELECT T.RESULT_FACID FROM t_sgm_wb_table_control T WHERE T.FACID=:FACID";

                object oRets = dataBase.ExecuteScalar(CommandType.Text, sql, param);

                oRet = oRets.ToString();
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetNewFactoryTalbe:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }

        #endregion

        #region 1) 获取活动总控
        /// <summary>
        /// 获取活动总控
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public DataTable GetService(string facid, string channel)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetServiceParamE");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //param[1] = new OracleParameter(":JOINTYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetServiceParamE", param);
                }
                param[0].Value = facid;
                // param[1].Value = channel;
                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, string.Format(Query_Control_Sql, channel), param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ECouponDao:GetService:" + facid + "---" + channel + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 2) 检查数码是否参与过电子优惠卷活动
        /// <summary>
        /// 检查数码是否参与过电子优惠卷活动
        /// </summary>
        /// <param name="digitcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable GetECouponQueryParLog(string digitcode, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetECouponQueryParLog");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetECouponQueryParLog", param);
                }
                param[0].Value = digitcode;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_PARLOG, facid);
                string sql = string.Format(Query_ParLog_Sql, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ECouponDao:GetECouponQueryParLog:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 3) 添加电子优惠卷参与记录 
        public bool AddECouponLog(string guid,string facid, string ip, string digitCode, string channel, string area, string proid, string result,string sncode)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetECouponLogParam(guid, ip, facid, digitCode, channel, area, proid, result, sncode);

                string table = GetTable(TableName_PARLOG, facid);
                string sql = string.Format(ADD_ECoupon_LOG_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:AddLotteryLog:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 获取参与抽奖日志组织参数
        /// <summary>
        /// 获取参与抽奖日志组织参数
        /// </summary>
        /// <param name="guid">guid</param>
        /// <param name="ip">IP</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">奖池编号</param>
        /// <param name="result">答复</param>
        /// <returns></returns>
        private OracleParameter[] GetECouponLogParam(string guid, string ip, string facid, string digitCode, string channel, string area, string proid, string result,string sncode)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetECouponLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[12];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 100);
                    param[2] = new OracleParameter(":USERTYPE", OracleType.VarChar, 1);
                    param[3] = new OracleParameter(":UPCONTENT", OracleType.VarChar, 255);
                    param[4] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[5] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[6] = new OracleParameter(":PROID", OracleType.VarChar, 2);
                    param[7] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 1000);
                    param[8] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[9] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                    param[10] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[11] = new OracleParameter(":SNCODE", OracleType.VarChar, 50);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetECouponLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = ip;
                param[2].Value = channel;
                param[3].Value = digitCode;
                param[4].Value = digitCode;
                param[5].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[6].Value = proid;
                param[7].Value = result;
                param[8].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[9].Value = facid;
                param[10].Value = area;
                param[11].Value = sncode;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 4) 获取在当前时间内是否存在电子优惠卷活动
        /// <summary>
        /// 获取活动开始记录
        /// </summary>
        /// <param name="ACTIVITYID">活动编号</param>
        /// <returns></returns>
        public DataTable GetECouponRecord(string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetECouponRecord");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                   
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetECouponRecord", param);
                }
                param[0].Value = facid;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_ECoupon_Sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ECouponDao:GetECouponRecord:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

    }
}
