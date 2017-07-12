// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL 
// *文件名称：PointsControlDao
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 11:51:40  
//
// *创建标识：PointsControlDao.cs
// *创建描述：积分抽奖活动业务数据访问处理
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
    public class PointsControlDal
    {
        #region sql列表

        private static string TableName_POINTS_SHAKE = TableNamePrefix() + "POINTS_SHAKE_LOG_";
        private static string TableName_POINTS_LOTTERYLOG = TableNamePrefix() + "POINTS_LOTTERYLOG_";

        private static string TableName_POINTS_SHAKE_PLOG = TableNamePrefix() + "POINTS_SHAKE_PLOG_";

        private static string TableName_ERNIE_PAR = TableNamePrefix() + "POINTS_ERNIEPARLOG_";
        private static string TableName_ERNIE = TableNamePrefix() + "POINTS_ERNIE_";

        private const string Query_Control_Sql = "SELECT SERVICEID,CHANNEL,FACID,ACTIVITYID,DELETEFLAG,CREATEDATE,ACTIVITYNAME FROM T_SGM_POINTS_SHAKE_CONTROL S WHERE S.FACID=:FACID AND S.CHANNEL LIKE '%{0}%' AND DELETEFLAG='1' "; //AND ACTIVITYTYPE=:ACTIVITYTYPE

        private string Query_Points_Rule_Sql = "SELECT R.ACTIVITYID,R.POOLID,R.POINTS,TOTALPOP,SMSTIMES,MAXLOTTERYTIMES,CYCLETIMES,CYCLE,MAXJOINTIMES,ACTIVITYNAME,STARTDATE,ENDDATE FROM T_SGM_POINTS_SHAKE_RULE R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND R.FACID=:FACID AND R.ENDDATE>=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND R.STARTDATE<=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND TYPE=:TYPE";
        private string Query_Points_Rule_Sql2 = "SELECT R.ACTIVITYID,R.POOLID,R.POINTS,TOTALPOP,SMSTIMES,MAXLOTTERYTIMES,CYCLETIMES,CYCLE,MAXJOINTIMES,ACTIVITYNAME,STARTDATE,ENDDATE FROM T_SGM_POINTS_SHAKE_RULE R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND R.FACID=:FACID AND R.ENDDATE>=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND TYPE=:TYPE";

        private string Query_Points_Rule_Sql_Pid = "SELECT R.ACTIVITYID,R.POOLID,R.POINTS,TOTALPOP,SMSTIMES,MAXLOTTERYTIMES,CYCLETIMES,CYCLE,MAXJOINTIMES,ACTIVITYNAME,STARTDATE,ENDDATE FROM T_SGM_POINTS_SHAKE_RULE R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND POOLID=:POOLID AND R.FACID=:FACID AND R.ENDDATE>=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND R.STARTDATE<=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND TYPE=:TYPE";
        private string Query_Points_Rule_Sql2_Pid = "SELECT R.ACTIVITYID,R.POOLID,R.POINTS,TOTALPOP,SMSTIMES,MAXLOTTERYTIMES,CYCLETIMES,CYCLE,MAXJOINTIMES,ACTIVITYNAME,STARTDATE,ENDDATE FROM T_SGM_POINTS_SHAKE_RULE R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND POOLID=:POOLID AND R.FACID=:FACID AND R.ENDDATE>=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND TYPE=:TYPE";

        private const string QUERY_POINTS_LOTTERYLOG_SQL = " SELECT COUNT(POOLID) FROM {0} L WHERE L.FACID=:FACID AND L.AWARDSNO=:AWARDSNO AND L.POOLID=:POOLID AND TO_CHAR(JOINDATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') AND DELETEFLAG='1'";

        private string QUERY_Points_RULE_POOL_SQL = "SELECT r.ACTIVITYID,"
                                                          + " r.SMSTIMES,"
                                                          + " r.TOTALPOP,"
                                                          + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                          + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                          + " r.CYCLETIMES,"
                                                          + " r.CYCLE,"
                                                          + " r.MAXLOTTERYTIMES,"
                                                          + " r.MAXJOINTIMES,"
                                                          + " r.POOLID,"
                                                          + " p.AWARDSCODE,"
                                                          + " p.MAXOPENLOTTER,"
                                                          + " p.TOTALTIMES,"
                                                          + " p.LOTTERYTIMES,"
                                                          + " p.AWARDSCALE,"
                                                          + " p.SMSVALUE,"
                                                          + " p.POOLSORT"
                                                      + " FROM T_SGM_POINTS_SHAKE_RULE r, T_SGM_POINTS_SHAKE_POOL p"
                                                      + " WHERE r.ACTIVITYID =:ACTIVITYID"
                                                      + " AND r.POOLID = p.POOLID"
                                                      + " AND r.DELETEFLAG = '1'"
                                                      + " AND p.DELETEFLAG = '1'"
                                                      + " AND r.TYPE = :TYPE"
                                                      + " AND r.STARTDATE <="
                                                      + " TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS')"
                                                      + " AND TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                      + " AND p.LOTTERYTIMES < p.TOTALTIMES "
                                                      + " AND r.FACID=p.FACID"
                                                      + " AND r.FACID=:FACID"
                                                      + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";

        private string QUERY_Points_RULE_POOL_SQL2 = "SELECT r.ACTIVITYID,"
                                                         + " r.SMSTIMES,"
                                                         + " r.TOTALPOP,"
                                                         + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                         + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                         + " r.CYCLETIMES,"
                                                         + " r.CYCLE,"
                                                         + " r.MAXLOTTERYTIMES,"
                                                         + " r.MAXJOINTIMES,"
                                                         + " r.POOLID,"
                                                         + " p.AWARDSCODE,"
                                                         + " p.MAXOPENLOTTER,"
                                                         + " p.TOTALTIMES,"
                                                         + " p.LOTTERYTIMES,"
                                                         + " p.AWARDSCALE,"
                                                         + " p.SMSVALUE,"
                                                         + " p.POOLSORT, "
                                                         + " p.ID AS POOLGUID"
                                                     + " FROM T_SGM_POINTS_SHAKE_RULE r, T_SGM_POINTS_SHAKE_POOL p"
                                                     + " WHERE r.ACTIVITYID =:ACTIVITYID"
                                                     + " AND r.POOLID = p.POOLID"
                                                     + " AND r.DELETEFLAG = '1'"
                                                     + " AND p.DELETEFLAG = '1'"
                                                     + " AND r.TYPE = :TYPE"
                                                     + " AND r.STARTDATE <="
                                                     + " TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS')"
                                                     + " AND TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                     + " AND p.LOTTERYTIMES < p.TOTALTIMES "
                                                     + " AND r.FACID=p.FACID"
                                                     + " AND r.FACID=:FACID"
                                                     + " AND r.POOLID=:POOLID"
                                                     + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";


        private const string CHECK_POINTS_USER_SHAKE_PARLOG_SQL = "SELECT COUNT(USERID) FROM {0} A WHERE A.USERGUID=:USERGUID "
                                                            + " AND A.ACTIVITYID=:ACTIVITYID "
                                                            + " AND TO_DATE(:SDATE, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE "
                                                            + " AND JOINDATE <=TO_DATE(:EDATE, 'YYYY-MM-DD HH24:MI:SS') "
                                                            + " AND TO_DATE(:LMONTH, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE"
                                                            + " AND DELETEFLAG='1' "
                                                            + " AND POOLID=:POOLID "
                                                            + " AND FACID =:FACID";

        private const string CHECK_POINTS_USER_LOTTERYLOG_SQL = " SELECT COUNT(USERID) FROM {0} A  WHERE A.ACTIVITYID=:ACTIVITYID "
                                                         + " AND A.POOLID=:POOLID"
                                                         + " AND A.USERGUID=:USERGUID"
                                                         + " AND TO_DATE(:SDATE, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE "
                                                         + " AND JOINDATE <=TO_DATE(:EDATE, 'YYYY-MM-DD HH24:MI:SS') "
                                                         + " AND TO_DATE(:LMONTH, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE"
                                                         + " AND DELETEFLAG='1'"
                                                         + " AND FACID =:FACID ";

        private const string CHECK_ERNIE_USER_SHAKE_PARLOG_SQL = "SELECT COUNT(USERID) FROM {0} A WHERE A.USERGUID=:USERGUID "
                                                           + " AND A.ACTIVITYID=:ACTIVITYID "
                                                           + " AND TO_DATE(:SDATE, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE "
                                                           + " AND JOINDATE <=TO_DATE(:EDATE, 'YYYY-MM-DD HH24:MI:SS') "
                                                           + " AND TO_DATE(:LMONTH, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE"
                                                           + " AND DELETEFLAG='1' " 
                                                           + " AND POOLID=:POOLID " 
                                                           + " AND FACID =:FACID";

        private const string CHECK_ERNIE_USER_LOG_SQL = " SELECT COUNT(USERID) FROM {0} A  WHERE A.ACTIVITYID=:ACTIVITYID "
                                                         + " AND A.POOLID=:POOLID"
                                                         + " AND A.USERGUID=:USERGUID"
                                                         + " AND TO_DATE(:SDATE, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE "
                                                         + " AND JOINDATE <=TO_DATE(:EDATE, 'YYYY-MM-DD HH24:MI:SS') "
                                                         + " AND TO_DATE(:LMONTH, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE"
                                                         + " AND DELETEFLAG='1'"
                                                         + " AND FACID =:FACID ";


        private string QUERY_Points_RULE_POOL_SQL_WJ = "SELECT r.ACTIVITYID,"
                                                         + " r.SMSTIMES,"
                                                         + " r.TOTALPOP,"
                                                         + " r.MINPOINTSSCOPE,"
                                                         + " r.MAXPOINTSSCOPE,"
                                                         + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                         + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                         + " r.CYCLETIMES,"
                                                         + " r.CYCLE,"
                                                         + " r.MAXLOTTERYTIMES,"
                                                         + " r.MAXJOINTIMES,"
                                                         + " r.POOLID,"
                                                         + " p.AWARDSCODE,"
                                                         + " p.MAXOPENLOTTER,"
                                                         + " p.TOTALTIMES,"
                                                         + " p.LOTTERYTIMES,"
                                                         + " p.AWARDSCALE,"
                                                         + " p.SMSVALUE,"
                                                         + " p.POOLSORT"
                                                     + " FROM T_SGM_POINTS_SHAKE_RULE r, T_SGM_POINTS_SHAKE_POOL p"
                                                     + " WHERE r.ACTIVITYID =:ACTIVITYID "
                                                     + " AND r.POOLID = p.POOLID"
                                                     + " AND r.TYPE = :TYPE"
                                                     + " AND r.DELETEFLAG = '1'"
                                                     + " AND p.DELETEFLAG = '1'"
                                                     + " AND r.STARTDATE <="
                                                     + " TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS')"
                                                     + " AND TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                     + " AND p.LOTTERYTIMES < p.TOTALTIMES "
                                                     + " AND r.FACID=p.FACID"
                                                     + " AND r.FACID=:FACID"
                                                     + " AND r.POOLID=:POOLID"
                                                     + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";

        private string Query_Points_Rule_Sql_WJ = @"SELECT 
                                                    R.ACTIVITYID,R.POOLID,R.TOTALPOP,
                                                    R.SMSTIMES,R.MAXLOTTERYTIMES,R.CYCLETIMES,R.CYCLE,
                                                    R.MAXJOINTIMES,R.ACTIVITYNAME,R.STARTDATE,R.ENDDATE,
                                                    P.COSTPOINTS AS POINTS
                                                    FROM  T_SGM_POINTS_SHAKE_RULE R,T_SGM_POINTS_SHAKE_POOL P, T_SGM_GIFT_AWARD_9454 A 
                                                    WHERE R.POOLID = P.POOLID AND P.ID = A.POOLGUID AND 
                                                    A.GIFTGUID = :GIFTGUID AND P.POOLID=:POOLID AND P.DELETEFLAG='1' 
                                                    AND R.FACID=:FACID AND R.ENDDATE>=TO_DATE('" + DateTime.Now + @"','YYYY-MM-DD HH24:MI:SS')";


        private string QUERY_Points_RULE_POOL_SQL2_WJ = "SELECT r.ACTIVITYID,"
                                                         + " r.SMSTIMES,"
                                                         + " r.TOTALPOP,"
                                                         + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                         + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                         + " r.CYCLETIMES,"
                                                         + " r.CYCLE,"
                                                         + " r.MAXLOTTERYTIMES,"
                                                         + " r.MAXJOINTIMES,"
                                                         + " r.POOLID,"
                                                         + " p.AWARDSCODE,"
                                                         + " p.MAXOPENLOTTER,"
                                                         + " p.TOTALTIMES,"
                                                         + " p.LOTTERYTIMES,"
                                                         + " p.AWARDSCALE,"
                                                         + " p.SMSVALUE,"
                                                         + " p.POOLSORT, "
                                                         + " p.ID AS POOLGUID"
                                                     + " FROM T_SGM_POINTS_SHAKE_RULE r, T_SGM_POINTS_SHAKE_POOL p,T_SGM_GIFT_AWARD_9999 g"
                                                     + " WHERE r.ACTIVITYID =:ACTIVITYID"
                                                     + " AND r.POOLID = p.POOLID"
                                                     + " AND p.ID = g.POOLGUID"
                                                     + " AND g.GIFTGUID = :GIFTGUID"
                                                     + " AND r.DELETEFLAG = '1'"
                                                     + " AND p.DELETEFLAG = '1'"
                                                     + " AND r.TYPE = :TYPE"
                                                     + " AND r.STARTDATE <="
                                                     + " TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS')"
                                                     + " AND TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                     + " AND p.LOTTERYTIMES < p.TOTALTIMES "
                                                     + " AND r.FACID=p.FACID"
                                                     + " AND r.FACID=:FACID"
                                                     + " AND r.POOLID=:POOLID"
                                                     + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";
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

        #region 3) 获取活动总控
        /// <summary>
        /// 获取活动总控
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public DataTable GetPointsService(string facid, string channel,string type)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsServiceParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //param[1] = new OracleParameter(":ACTIVITYTYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsServiceParam", param);
                }
                param[0].Value = facid;
                //param[1].Value = type;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, string.Format(Query_Control_Sql, channel), param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDao:GetPointsService:" + facid + "---" + channel + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 4) 获取积分抽奖活动开始记录
        /// <summary>
        /// 获取积分抽奖活动开始记录
        /// </summary>
        /// <param name="ACTIVITYID">活动编号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="type">类型(0:积分抽奖 1:积分摇奖 2:数码抽奖)</param>
        /// <returns></returns>
        public DataTable GetPointsAwardRecord(string activityid, string facid,string type)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardRecordParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardRecordParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_Points_Rule_Sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAwardRecord:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 4.1) 获取积分抽奖活动开始记录(PID)
        /// <summary>
        /// 获取积分抽奖活动开始记录
        /// </summary>
        /// <param name="ACTIVITYID">活动编号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">类型(0:积分抽奖 1:积分摇奖 2:数码抽奖)</param>
        /// <returns></returns>
        public DataTable GetPointsAwardRecordbyPid(string activityid, string facid,string pid, string type)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardRecordbyPidParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[4];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":POOLID", OracleType.VarChar, 6);
                    param[3] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardRecordbyPidParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = pid;
                param[3].Value = type;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_Points_Rule_Sql_Pid, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAwardRecordbyPid:" + facid + "---" + activityid + "---" + pid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 5) 获取活动开始记录

        /// <summary>
        /// 获取活动开始记录
        /// </summary>
        /// <param name="activityid">活动编码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="type">类别(0:积分抽奖 1:积分摇奖 2:数码抽奖)</param>
        /// <returns></returns>
        public DataTable GetPointsAwardRecord2(string activityid, string facid, string type)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardRecord2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardRecord2", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_Points_Rule_Sql2, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAwardRecord2:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 5.1) 获取活动开始记录(PID)

        /// <summary>
        /// 获取活动开始记录
        /// </summary>
        /// <param name="activityid">活动编码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="pid">活动ID</param>
        /// <param name="type">类别(0:积分抽奖 1:积分摇奖 2:数码抽奖)</param>
        /// <returns></returns>
        public DataTable GetPointsAwardRecordbyPid2(string activityid, string facid,string pid, string type)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardRecordbyPid2Param");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[4];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    param[4] = new OracleParameter(":POOLID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardRecordbyPid2Param", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;
                param[3].Value = pid;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_Points_Rule_Sql2_Pid, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAwardRecord2:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 6) 获取积分抽奖奖池
        /// <summary>
        /// 获取积分抽奖奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <returns></returns>
        public DataTable GetPointsAward(string facid, string activityid,string type)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, QUERY_Points_RULE_POOL_SQL, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 6.1) 获取积分抽奖奖池(PID)
        /// <summary>
        /// 获取积分抽奖奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">类别</param>
        /// <returns></returns>
        public DataTable GetPointsAwardbyPid(string facid, string activityid,string pid, string type)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardParamPID2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[4];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    param[3] = new OracleParameter(":POOLID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardParamPID2", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;
                param[3].Value = pid;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, QUERY_Points_RULE_POOL_SQL2, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 7) 检查用户最大参与次数
        /// <summary>
        /// 检查用户最大参与次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">参与用户IP地址/手机号码</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="lastmonth"></param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public int CheckPointsSendMaxNumber(string facid, string userid, string StartData, string EndDate, string lastmonth, string activityId,string poolid)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckPointsSendMaxNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SDATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":EDATE", OracleType.VarChar);
                    param[3] = new OracleParameter(":LMONTH", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[6] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckPointsSendMaxNumParam", param);
                }
                param[0].Value = activityId;
                param[1].Value = StartData;
                param[2].Value = EndDate;
                param[3].Value = lastmonth;
                param[4].Value = facid;
                param[5].Value = userid;
                param[6].Value = poolid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_POINTS_SHAKE_PLOG, facid);

                string sql = string.Format(CHECK_POINTS_USER_SHAKE_PARLOG_SQL, table);

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:CheckPointsSendMaxNumber:" + facid + "--" + userid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }

        #endregion

        #region 8) 检查此人指定活动时间内超过最大中奖次数
        /// <summary>
        /// 检查此人指定活动时间内超过最大中奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">参与用户IP地址/手机号码</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="lastmonthData"></param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolId">奖池编号</param>
        /// <returns></returns>
        public int CheckPointsUserPrizeMaxNum(string facid, string userid, string StartData, string EndDate, string lastmonthData, string activityId, string poolId)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckPointsUserPrizeMaxNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SDATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":EDATE", OracleType.VarChar);
                    param[3] = new OracleParameter(":LMONTH", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[6] = new OracleParameter(":POOLID", OracleType.VarChar, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("CheckPointsUserPrizeMaxNumParam", param);
                }
                param[0].Value = activityId;
                param[1].Value = StartData;
                param[2].Value = EndDate;
                param[3].Value = lastmonthData;
                param[4].Value = facid;
                param[5].Value = userid;
                param[6].Value = poolId;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_POINTS_LOTTERYLOG, facid);

                string sql = string.Format(CHECK_POINTS_USER_LOTTERYLOG_SQL, table);

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:CheckPointsUserPrizeMaxNum:" + facid + "--" + userid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }

        #endregion

        #region 9) 获取今日中奖数
        /// <summary>
        /// 获取今日中奖数
        /// </summary>
        /// <param name="poolid">奖池Id</param>
        /// <param name="awardScode">奖项ID</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public object GetPointsDayMaxLottery(string poolid, string awardScode, string facid)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsDayMaxLotteryParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[1] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsDayMaxLotteryParam", param);
                }
                param[0].Value = awardScode;
                param[1].Value = poolid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_POINTS_LOTTERYLOG, facid);

                string sql = string.Format(QUERY_POINTS_LOTTERYLOG_SQL, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsDayMaxLottery:" + facid + "---" + poolid + "---" + awardScode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 10) 检查用户最大参与次数
        /// <summary>
        /// 检查用户最大参与次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">参与用户IP地址/手机号码</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="lastmonth"></param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public int CheckErnieSendMaxNumber(string facid, string userid, string StartData, string EndDate, string lastmonth, string activityId, string poolid)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckErnieSendMaxNumberParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SDATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":EDATE", OracleType.VarChar);
                    param[3] = new OracleParameter(":LMONTH", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[6] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckErnieSendMaxNumberParam", param);
                }
                param[0].Value = activityId;
                param[1].Value = StartData;
                param[2].Value = EndDate;
                param[3].Value = lastmonth;
                param[4].Value = facid;
                param[5].Value = userid;
                param[6].Value = poolid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_ERNIE_PAR, facid);

                string sql = string.Format(CHECK_ERNIE_USER_SHAKE_PARLOG_SQL, table);

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:CheckErnieSendMaxNumber:" + facid + "--" + userid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }

        #endregion

        #region 8) 检查此人指定活动时间内超过最大中奖次数
        /// <summary>
        /// 检查此人指定活动时间内超过最大中奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="userid">参与用户IP地址/手机号码</param>
        /// <param name="StartData">开始时间</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="lastmonthData"></param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolId">奖池编号</param>
        /// <returns></returns>
        public int CheckErnieUserPrizeMaxNum(string facid, string userid, string StartData, string EndDate, string lastmonthData, string activityId, string poolId)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckErnieUserPrizeMaxNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SDATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":EDATE", OracleType.VarChar);
                    param[3] = new OracleParameter(":LMONTH", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[6] = new OracleParameter(":POOLID", OracleType.VarChar, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("CheckErnieUserPrizeMaxNumParam", param);
                }
                param[0].Value = activityId;
                param[1].Value = StartData;
                param[2].Value = EndDate;
                param[3].Value = lastmonthData;
                param[4].Value = facid;
                param[5].Value = userid;
                param[6].Value = poolId;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_ERNIE, facid);

                string sql = string.Format(CHECK_ERNIE_USER_LOG_SQL, table);

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:CheckErnieUserPrizeMaxNum:" + facid + "--" + userid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }

        #endregion

        #region 11) 万金 查询获取活动规则
        /// <summary>
        /// 获取积分抽奖奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">类别</param>
        /// <returns></returns>
        public DataTable GetPointsRulebyPid_WJ(string facid, string activityid, string pid, string type)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsRulebyPid_WJ");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[4];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    param[3] = new OracleParameter(":POOLID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsRulebyPid_WJ", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;
                param[3].Value = pid;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, QUERY_Points_RULE_POOL_SQL_WJ, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 11.2) 万金 获取活动开始记录

        /// <summary>
        /// 获取活动开始记录
        /// </summary>
        /// <param name="activityid">活动编码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="type">类别(0:积分抽奖 1:积分摇奖 2:数码抽奖)</param>
        /// <returns></returns>
        public DataTable GetPointsAwardRecord_WJ(string gpid, string facid, string poolid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardRecord_WJ");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":GIFTGUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":POOLID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardRecord_WJ", param);
                }
                param[0].Value = gpid;
                param[1].Value = facid;
                param[2].Value = poolid;

                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_Points_Rule_Sql_WJ, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAwardRecord_WJ:" + facid + "---" + gpid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 11.3) 获取积分抽奖奖池(PID)
        /// <summary>
        /// 获取积分抽奖奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="pid">活动规则ID</param>
        /// <param name="type">类别</param>
        /// <returns></returns>
        public DataTable GetPointsAwardbyPid_WJ(string facid, string activityid, string pid,string gpid, string type)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetPointsAwardbyPid_WJ");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[5];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":TYPE", OracleType.Char, 1);
                    param[3] = new OracleParameter(":POOLID", OracleType.VarChar, 6);
                    param[4] = new OracleParameter(":GIFTGUID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsAwardbyPid_WJ", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;
                param[2].Value = type;
                param[3].Value = pid;
                param[4].Value = gpid;

                DataBase dataBase = new DataBase();

                string sql = QUERY_Points_RULE_POOL_SQL2_WJ.Replace("9999", facid);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsControlDal:GetPointsAwardbyPid_WJ:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }

        #endregion
    }
}
