// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL 
// *文件名称：PointsLotteryDao
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 13:52:48  
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
    public class PointsLotteryDal
    {

        #region sql列表

        //private static string TableName_CONTROL = TableNamePrefix() + "SHAKE_CONTROL_";
        //private static string TableName_PARLOG = TableNamePrefix() + "SHAKE_PARLOG_";
        private static string TableName_Log = TableNamePrefix() + "POINTS_SHAKE_LOG_";
        private static string TableName_PLog = TableNamePrefix() + "POINTS_SHAKE_PLOG_";
        private static string TableName_LOTTERYLOG = TableNamePrefix() + "POINTS_LOTTERYLOG_";
        private static string TableName_Ernie_LOG = TableNamePrefix() + "POINTS_ERNIE_LOG_";
        private static string TableName_Ernie = TableNamePrefix() + "POINTS_ERNIE_LOG_";

        private static string TableName_Ernie_Par = TableNamePrefix() + "POINTS_ERNIEPARLOG_";

        private static string TableName_DhLottery = TableNamePrefix() + "DHLOTTERY_";

        private static string TableName_SGM_User = TableNamePrefix() + "USER_";

        private static string TableName_Pool = TableNamePrefix() + "SHAKE_POOL_";

        private static string TableName_RULE = TableNamePrefix() + "SGM_POINTS_SHAKE_RULE";

        //private static string TableName_Rule = TableNamePrefix() + "SHAKE_RULE_";
        //private static string TableName_MapFacPro = TableNamePrefix() + "SHAKE_MAPFACPRO_";
        //private static string TableName_Product = TableNamePrefix() + "SHAKE_PRODUCT_";
        //private static string TableName_Signin = TableNamePrefix() + "SHAKE_REGISTERUSER_";


        //private const string SEND_SMS_SQL = "INSERT INTO T_SMS_SEND_TO(SEND_USERID,SEND_TO,SUBJECT,SUBJECT_TYPE,SEND_TYPE,SEND_CONTENT,CUSTOMER_CODE) select 'CCN_System',REMINDID,'预警提示','6','0'||TYPE,'{0}','CCN_001' FROM T_SGM_SHAKE_REMIND where FACID=:FACID and REMINDLEVEL<=:REMINDLEVEL  and DELETEFLAG='1'";

        //private const string UPDATE_SHAKE_POOL_SQL = "UPDATE {0} P SET LOTTERYTIMES=LOTTERYTIMES+1 WHERE POOLID=:POOLID AND AWARDSCODE=:AWARDSCODE AND LOTTERYTIMES<TOTALTIMES AND FACID=:FACID";

        //private const string UPDATE_SHAKE_RULE_SQL = "UPDATE {0} SET TOTALPOP=TOTALPOP+1 WHERE FACID=:FACID AND ACTIVITYID=:ACTIVITYID  AND POOLID=:POOLID AND DELETEFLAG='1'";

        private const string ADD_POINTS_SHAKE_LOG_SQL = " INSERT INTO {0} (GUID,USERID,USERGUID,IP,CHANNEL,JOINDATE,DOWNCONTENT,AWARDSNO,DELETEFLAG,CREATEDAE,POINTS,AREA,ACTIVITYID,FACID)  "
                                                 + " VALUES(:GUID,:USERID,:USERGUID,:IP,:CHANNEL,:JOINDATE,:DOWNCONTENT,:AWARDSNO,:DELETEFLAG,:CREATEDAE,:POINTS,:AREA,:ACTIVITYID,:FACID) ";

        private const string ADD_POINTS_SHAKE_PLOG_SQL = " INSERT INTO {0} (GUID,USERID,USERGUID,IP,CHANNEL,JOINDATE,DOWNCONTENT,AWARDSNO,DELETEFLAG,CREATEDAE,POINTS,AREA,ACTIVITYID,FACID,POOLID)  "
                                                + " VALUES(:GUID,:USERID,:USERGUID,:IP,:CHANNEL,:JOINDATE,:DOWNCONTENT,:AWARDSNO,:DELETEFLAG,:CREATEDAE,:POINTS,:AREA,:ACTIVITYID,:FACID,:POOLID) ";



        private const string ADD_POINTS_LOTTERYLOG_SQL = " INSERT INTO {0} (GUID,USERID,USERGUID,IP,CHANNEL,JOINDATE,DOWNCONTENT,AWARDSNO,DELETEFLAG,CREATEDAE,POINTS,AREA,ACTIVITYID,FACID,POOLID)  "
                                                 + " VALUES(:GUID,:USERID,:USERGUID,:IP,:CHANNEL,:JOINDATE,:DOWNCONTENT,:AWARDSNO,:DELETEFLAG,:CREATEDAE,:POINTS,:AREA,:ACTIVITYID,:FACID,:POOLID) ";

        private const string ADD_POINTS_Ernie_Log_SQL = " INSERT INTO {0} (GUID,USERID,USERGUID,IP,CHANNEL,JOINDATE,DELETEFLAG,CREATEDAE,POINTS,AREA,ACTIVITYID,FACID,POOLID,DOWNCONTENT)  "
                                                 + " VALUES(:GUID,:USERID,:USERGUID,:IP,:CHANNEL,:JOINDATE,:DELETEFLAG,:CREATEDAE,:POINTS,:AREA,:ACTIVITYID,:FACID,:POOLID,:DOWNCONTENT) ";

        private const string ADD_POINTS_Ernie_ParLog_SQL = " INSERT INTO {0} (GUID,USERID,USERGUID,IP,CHANNEL,JOINDATE,DELETEFLAG,CREATEDAE,POINTS,AREA,ACTIVITYID,FACID,POOLID)  "
                                                + " VALUES(:GUID,:USERID,:USERGUID,:IP,:CHANNEL,:JOINDATE,:DELETEFLAG,:CREATEDAE,:POINTS,:AREA,:ACTIVITYID,:FACID,:POOLID) ";

        private const string ADD_POINTS_Ernie_SQL = " INSERT INTO {0} (GUID,USERID,USERGUID,AWARDSNO,DELETEFLAG,CREATEDAE,LOTTERYID,ACTIVITYID,FACID)  "
                                                 + " VALUES(:GUID,:USERID,:USERGUID,:AWARDSNO,:DELETEFLAG,:CREATEDAE,:LOTTERYID,:ACTIVITYID,:FACID) ";

        private const string ADD_POINTS_DHLottery_SQL = " INSERT INTO {0}  (GUID,USERGUID,POINTS,LOTTERYTYPE,DELETEFLAG,ACTIVITYID,CREATEDATE,FACID,CHANNEL,IP,AWARDSNO)  "
                                               + " VALUES(:GUID,:USERGUID,:POINTS,:LOTTERYTYPE,:DELETEFLAG,:ACTIVITYID,:CREATEDATE,:FACID,:CHANNEL,:IP,:AWARDSNO) ";

        private const string Get_Sgm_User_Sql = "SELECT U.USERGUID,U.USERID,U.MOBILE,U.USERNAME,U.POINTTOTAL,U.POINTVALID,U.POINTUSED,U.DELETEFLAG,U.FACID FROM {0} U WHERE (U.USERID=:USERID OR U.MOBILE=:MOBILE) AND U.FACID=:FACID AND U.DELETEFLAG='1'";

        private const string Modify_User_Point_Sql = "UPDATE {0} SET POINTVALID=POINTVALID-:POINTUSED,POINTUSED=POINTUSED+:POINTUSED WHERE DELETEFLAG='1' AND  POINTVALID>=:POINTUSED AND USERGUID=:USERGUID AND FACID =:FACID";

        private const string SEND_SMS_SQL = "INSERT INTO T_SMS_SEND_TO(SEND_USERID,SEND_TO,SUBJECT,SUBJECT_TYPE,SEND_TYPE,SEND_CONTENT,CUSTOMER_CODE) select 'CCN_System',REMINDID,'预警提示','6','0'||TYPE,'{0}','CCN_001' FROM T_SGM_SHAKE_REMIND where FACID=:FACID and REMINDLEVEL<=:REMINDLEVEL  and DELETEFLAG='1'";

        private const string MODIFY_POINTS_SHAKE_POOL_SQL = "UPDATE T_SGM_POINTS_SHAKE_POOL P SET LOTTERYTIMES=LOTTERYTIMES+1 WHERE POOLID=:POOLID AND AWARDSCODE=:AWARDSCODE AND LOTTERYTIMES<TOTALTIMES AND FACID=:FACID AND DELETEFLAG='1'";

        private const string MODIFY_POINTS_SHAKE_RULE_SQL = "UPDATE T_SGM_POINTS_SHAKE_RULE SET TOTALPOP=TOTALPOP+1 WHERE FACID=:FACID AND ACTIVITYID=:ACTIVITYID  AND POOLID=:POOLID AND DELETEFLAG='1'";

        private const string Get_Shake_Pool_Sql_WJ = "SELECT GIFTPOINT FROM T_SGM_GIFT_AWARD_9999 WHERE FACID=:FACID AND POOLGUID=:POOLGUID";
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
                Logger.AppLog.Write("LotteryDal:GetNewFactoryTalbe:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }

        #endregion

        #region 3) 获取参与抽奖日志组织参数
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
        private OracleParameter[] GetPointsShakeLogParam(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string awardsno, string points, string result, string joindate)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsShakeLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[14];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[2] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[3] = new OracleParameter(":IP", OracleType.VarChar, 20);
                    param[4] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[6] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 255);
                    param[7] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[8] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[9] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[10] = new OracleParameter(":POINTS", OracleType.Number);
                    param[11] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[12] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[13] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsShakeLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userid;
                param[2].Value = userguid;
                param[3].Value = ip;
                param[4].Value = channel;
                param[5].Value = joindate;
                param[6].Value = result;
                param[7].Value = awardsno;
                param[8].Value = "1";
                param[9].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[10].Value = points;
                param[11].Value = area;
                param[12].Value = activityId;
                param[13].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("3)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 3.1) 获取参与抽奖日志组织参数(真)
        /// <summary>
        /// 新增积分抽奖参与记录(真)
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
        private OracleParameter[] GetPointsShakePLogParam(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string awardsno, string points, string result, string joindate, string poolid)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsShakePLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[15];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[2] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[3] = new OracleParameter(":IP", OracleType.VarChar, 20);
                    param[4] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[6] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 255);
                    param[7] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[8] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[9] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[10] = new OracleParameter(":POINTS", OracleType.Number);
                    param[11] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[12] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[13] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[14] = new OracleParameter(":POOLID", OracleType.VarChar, 10);//POOLID

                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsShakePLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userid;
                param[2].Value = userguid;
                param[3].Value = ip;
                param[4].Value = channel;
                param[5].Value = joindate;
                param[6].Value = result;
                param[7].Value = awardsno;
                param[8].Value = "1";
                param[9].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[10].Value = points;
                param[11].Value = area;
                param[12].Value = activityId;
                param[13].Value = facid;
                param[14].Value = poolid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("3.1)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 4) 添加积分抽奖参与日志

        /// <summary>
        /// 新增积分抽奖参与记录
        /// </summary>
        /// <param name="guid">参与</param>
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
        public bool AddPointsShakeLog(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string awardsno, string points, string result, string joindate)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetPointsShakeLogParam(guid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate);

                string table = GetTable(TableName_Log, facid);
                string sql = string.Format(ADD_POINTS_SHAKE_LOG_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryDal:AddPointsShakeLog:" + facid + "---" + userid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 新增积分抽奖参与记录
        /// </summary>
        /// <param name="lguid">参与GUID</param>
        /// <param name="pguid">参与GUID(真)</param>
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
        /// <param name="poolid">奖池ID</param>
        /// <returns></returns> 
        public bool AddPointsShakeLog(string lguid, string pguid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string awardsno, string points, string result, string joindate, string poolid)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param1 = null;

            try
            {
                param1 = GetPointsShakeLogParam(lguid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate);
                param2 = GetPointsShakePLogParam(pguid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate, poolid);

                DataBase dataBase = new DataBase();

                if (param2 != null && param1 != null)
                {
                    dataBase.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;

                        #region 普通参与
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_POINTS_SHAKE_LOG_SQL, table1);
                        nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, sql1, param1);
                        #endregion

                        #region 真参与
                        string table2 = GetTable(TableName_PLog, facid);
                        string sql2 = string.Format(ADD_POINTS_SHAKE_PLOG_SQL, table2);
                        nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        #endregion

                        if ((nRet1 + nRet2) == 2)
                        {
                            dataBase.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dataBase.RollBackTrans();
                        }
                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("记录积分抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dataBase.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryDal:AddPointsShakeLog2:" + facid + "---" + userid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 5) 获取抽奖中奖日志组织参数
        /// <summary>
        /// 获取抽奖中奖日志组织参数
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
        private OracleParameter[] GetPointsLotteryLogParam(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string poolid, string area, string awardsno, string points, string result, string joindate)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsLotteryLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[15];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[2] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[3] = new OracleParameter(":IP", OracleType.VarChar, 20);
                    param[4] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[6] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 255);
                    param[7] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[8] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[9] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[10] = new OracleParameter(":POINTS", OracleType.Number);
                    param[11] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[12] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[13] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[14] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsLotteryLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userid;
                param[2].Value = userguid;
                param[3].Value = ip;
                param[4].Value = channel;
                param[5].Value = joindate;
                param[6].Value = result;
                param[7].Value = awardsno;
                param[8].Value = "1";
                param[9].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[10].Value = points;
                param[11].Value = area;
                param[12].Value = activityId;
                param[13].Value = facid;
                param[14].Value = poolid;
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

        #region 6) 添加积分抽奖中奖日志

        /// <summary>
        /// 新增积分抽奖参与记录
        /// </summary>
        /// <param name="guid">参与</param>
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
        public bool AddPointsLotteryLog(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string poolid, string area, string awardsno, string points, string result, string joindate)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetPointsLotteryLogParam(guid, facid, ip, userguid, userid, channel, activityId, poolid, area, awardsno, points, result, joindate);

                string table = GetTable(TableName_LOTTERYLOG, facid);
                string sql = string.Format(ADD_POINTS_LOTTERYLOG_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryDal:AddPointsLotteryLog:" + facid + "---" + userid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 7) 获取参与摇奖日志组织参数
        /// <summary>
        /// 获取参与摇奖日志组织参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="points">消耗积分</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池编码（子）</param>
        /// <param name="result">参与积分摇奖活动答复</param>
        /// <returns></returns>
        private OracleParameter[] GetPointsErnieLogParam(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string points, string joindate, string poolid, string result)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsErnieLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[14];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[2] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[3] = new OracleParameter(":IP", OracleType.VarChar, 20);
                    param[4] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[6] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[7] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[8] = new OracleParameter(":POINTS", OracleType.Number);
                    param[9] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[10] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[11] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[12] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    param[13] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 255);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsErnieLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userid;
                param[2].Value = userguid;
                param[3].Value = ip;
                param[4].Value = channel;
                param[5].Value = joindate;
                param[6].Value = "1";
                param[7].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[8].Value = points;
                param[9].Value = area;
                param[10].Value = activityId;
                param[11].Value = facid;
                param[12].Value = poolid;
                param[13].Value = result;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("7)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 8) 添加积分摇奖参与日志

        /// <summary>
        /// 新增积分摇奖参与记录
        /// </summary>
        /// <param name="guid">参与GUID</param>
        /// <param name="dguid">积分扣减GUID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="lotterytype">类别(1:抽奖 0:摇奖)</param>
        /// <param name="points">消耗积分</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="result">参与积分摇奖活动答复</param>
        /// <param name="isModifyUserPoints">是否修改用户积分分值和添加扣减分值记录</param>
        /// <returns></returns>
        public bool AddPointsErnieLog(string guid, string dguid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string lotterytype, string points, string joindate, string poolid, string result, bool isModifyUserPoints)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;

            try
            {

                DataBase dataBase = new DataBase();
                param1 = GetPointsErnieLogParam(guid, facid, ip, userguid, userid, channel, activityId, area, points, joindate, poolid, result);
                param2 = GetPointsDhLotteryParam(dguid, userguid, Convert.ToInt32(points), lotterytype, channel, activityId, ip, facid, "0");
                param3 = GetModifyUserPointsParam(userguid, Convert.ToInt32(points), facid);

                if (param2 != null && param3 != null && param1 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;

                        #region 积分摇奖参与记录添加
                        string table1 = GetTable(TableName_Ernie_LOG, facid);
                        string sql1 = string.Format(ADD_POINTS_Ernie_Log_SQL, table1);
                        nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, sql1, param1);
                        #endregion

                        #region 扣减用户分值和添加积分扣减记录添加
                        if (isModifyUserPoints)
                        {
                            string table2 = GetTable(TableName_DhLottery, facid);
                            string sql2 = string.Format(ADD_POINTS_DHLottery_SQL, table2);
                            nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, sql2, param2);

                            string table3 = GetTable(TableName_SGM_User, facid);
                            string sql3 = string.Format(Modify_User_Point_Sql, table3);
                            nRet3 = dataBase.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        }
                        else
                        {
                            nRet2 = 1;
                            nRet3 = 1;
                        }
                        #endregion

                        if ((nRet1 + nRet2 + nRet3) == 3)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("记录积分摇奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryDal:AddPointsErnieLog:" + facid + "---" + userid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 9) 获取参与摇奖中奖日志组织参数
        /// <summary>
        /// 获取参与摇奖中奖日志组织参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="points">消耗积分</param>
        /// <param name="joindate">参与时间</param>
        /// <returns></returns>
        private OracleParameter[] GetPointsErnieParam(string guid, string facid, string userguid, string userid, string activityId, string awardsno, string lotteryid)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsErnieParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[9];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[2] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[3] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[4] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[6] = new OracleParameter(":LOTTERYID", OracleType.VarChar, 50);
                    param[7] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[8] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsErnieParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userid;
                param[2].Value = userguid;
                param[3].Value = awardsno;
                param[4].Value = "1";
                param[5].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[6].Value = lotteryid;
                param[7].Value = activityId;
                param[8].Value = facid;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("9)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 10) 添加积分摇奖中奖记录

        /// <summary>
        /// 新增积分摇奖中奖记录
        /// </summary>
        /// <param name="guid">参与</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="points">消耗积分</param>
        /// <param name="joindate">参与时间</param>
        /// <returns></returns> 
        public bool AddPointsErnie(string guid, string facid, string userguid, string userid, string activityId, string awardsno, string lotteryid)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetPointsErnieParam(guid, facid, userguid, userid, activityId, awardsno, lotteryid);

                string table = GetTable(TableName_Ernie, facid);
                string sql = string.Format(ADD_POINTS_Ernie_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("PointsLotteryDal:AddPointsErnie:" + facid + "---" + userid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 11) 获取参与抽奖日志组织参数

        /// <summary>
        /// 获取参与抽奖日志组织参数
        /// </summary>
        /// <param name="guid">guid</param>
        /// <param name="userguid">userguid</param>
        /// <param name="points">消耗积分分值</param>
        /// <param name="lotterytype">活动类型(1:抽奖,0:摇奖)</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="ip">IP</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="awardsno">奖项编号</param>
        /// <returns></returns>
        private OracleParameter[] GetPointsDhLotteryParam(string guid, string userguid, int points, string lotterytype, string channel, string activityId, string ip, string facid, string awardsno)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsDhLotteryParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[11];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERGUID", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":POINTS", OracleType.Number);
                    param[3] = new OracleParameter(":LOTTERYTYPE", OracleType.VarChar, 1);
                    param[4] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[6] = new OracleParameter(":CREATEDATE", OracleType.DateTime);
                    param[7] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[8] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    param[9] = new OracleParameter(":IP", OracleType.VarChar, 100);
                    param[10] = new OracleParameter(":AWARDSNO", OracleType.VarChar, 5);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsDhLotteryParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userguid;
                param[2].Value = points;
                param[3].Value = lotterytype;
                param[4].Value = "1";
                param[5].Value = activityId;
                param[6].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[7].Value = facid;
                param[8].Value = channel;
                param[9].Value = ip;
                param[10].Value = awardsno;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("3)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 12) 添加积分抽奖参与日志

        /// <summary>
        /// 获取参与抽奖日志组织参数
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
        public bool AddPointsDhLottery(string guid, string userguid, int points, string lotterytype, string channel, string activityId, string ip, string facid, string awardsno)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetPointsDhLotteryParam(guid, userguid, points, lotterytype, channel, activityId, ip, facid, awardsno);

                string table = GetTable(TableName_DhLottery, facid);
                string sql = string.Format(ADD_POINTS_DHLottery_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("UserDhLotteryDal:AddPointsDhLottery:" + facid + "---" + userguid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 13) 获取用户相关信息

        /// <summary>
        /// 获取用户相关信息
        /// </summary>
        /// <param name="userid">用户登录帐号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns>用户信息</returns>
        public DataTable GetSgmUserPoint(string userid, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {

                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetSgmUserParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetSgmUserParam", param);
                }
                param[0].Value = userid;
                param[1].Value = userid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_SGM_User, facid);
                string sql = string.Format(Get_Sgm_User_Sql, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("UserDhLotteryDal:GetSgmUserPoint:" + facid + "---" + userid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }

        #endregion

        #region 14) 获取修改用户积分信息相关参数

        /// <summary>
        /// 获取修改用户积分信息相关参数
        /// </summary>
        /// <param name="userguid">用户GUID</param>
        /// <param name="points">扣减积分分值</param>
        /// <param name="facid">厂家编号</param>
        /// <returns>参数列表</returns>
        private OracleParameter[] GetModifyUserPointsParam(string userguid, int points, string facid)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetModifyUserPointsParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];

                    param[0] = new OracleParameter(":POINTUSED", OracleType.Number);
                    param[1] = new OracleParameter(":USERGUID", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 20);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetModifyUserPointsParam", param);
                }
                param[0].Value = points;
                param[1].Value = userguid;
                param[2].Value = facid;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("14)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 15) 获取中奖人数参数

        /// <summary>
        /// 获取中奖人数参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="awardscode">奖项编号</param>
        /// <returns></returns>
        public OracleParameter[] ModifyPointsLotteryNumParam(string facid, string poolid, string awardscode)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("ModifyPointsLotteryNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":POOLID", OracleType.Number);
                    param[1] = new OracleParameter(":AWARDSCODE", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("ModifyPointsLotteryNumParam", param);
                }
                param[0].Value = poolid;
                param[1].Value = awardscode;
                param[2].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("15)ModifyPointsLotteryNumParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 16) 获取预警信息发送参数
        /// <summary>
        /// 预警信息发送
        /// </summary>
        /// <param name="level">奖项等级</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public OracleParameter[] sendPrewarningParam(int level, string facid, string content)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("sendPrewarningParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":REMINDLEVEL", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("sendPrewarningParam", param);
                }
                param[0].Value = facid;
                param[1].Value = level;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("sendPrewarningParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 17) 积分抽奖相关信息保存
        /// <summary>
        /// 积分抽奖相关信息保存
        /// </summary>
        /// <param name="sguid">积分抽奖参与guid</param>
        /// <param name="lguid">积分抽奖中奖guid</param>
        /// <param name="dguid">积分抽奖扣减积分guid</param>
        /// <param name="pguid">真参与抽奖记录guid</param>
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
        public bool ModifyUserPointByLottery(string sguid, string lguid, string dguid, string pguid, string facid, string ip, string userguid, string userid, string channel, string activityId, string lotterytype, string area, string awardsno, string points, string result, string joindate, string poolid, string smscontent, bool smsflag, bool isModifyLottery, bool isModifyUserPoints)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param7 = null;

            try
            {
                param1 = GetPointsShakeLogParam(sguid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate);
                param2 = GetPointsLotteryLogParam(lguid, facid, ip, userguid, userid, channel, activityId, poolid, area, awardsno, points, result, joindate);
                param3 = GetPointsDhLotteryParam(dguid, userguid, Convert.ToInt32(points), lotterytype, channel, activityId, ip, facid, awardsno);
                param4 = GetModifyUserPointsParam(userguid, Convert.ToInt32(points), facid);
                param5 = ModifyPointsLotteryNumParam(facid, poolid, awardsno);
                param6 = sendPrewarningParam(3, facid, smscontent);
                param7 = GetPointsShakePLogParam(sguid, facid, ip, userguid, userid, channel, activityId, area, awardsno, points, result, joindate, poolid);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param7 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;
                        int nRet5 = 0;
                        int nRet6 = 0;
                        int nRet7 = 0;

                        //增加积分抽奖参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_POINTS_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加积分抽奖参与日志(真)
                        string table7 = GetTable(TableName_PLog, facid);
                        string sql7 = string.Format(ADD_POINTS_SHAKE_PLOG_SQL, table7);
                        nRet7 = dbc.ExecuteNonQuery(CommandType.Text, sql7, param7);

                        //增加积分抽奖中奖日志
                        if (isModifyLottery)
                        {
                            string table2 = GetTable(TableName_LOTTERYLOG, facid);
                            string sql2 = string.Format(ADD_POINTS_LOTTERYLOG_SQL, table2);
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                            //中奖记录+1
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, MODIFY_POINTS_SHAKE_POOL_SQL, param5);
                        }
                        else
                        {
                            nRet2 = 1;
                            nRet5 = 1;
                        }



                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql6 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, sql6, param6);
                            nRet6 = 1;
                        }
                        else
                        {
                            nRet6 = 1;
                        }

                        if (isModifyUserPoints) //添加积分扣减记录和修改用户积分信息
                        {
                            string table3 = GetTable(TableName_DhLottery, facid);
                            string sql3 = string.Format(ADD_POINTS_DHLottery_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);

                            string table4 = GetTable(TableName_SGM_User, facid);
                            string sql4 = string.Format(Modify_User_Point_Sql, table4);
                            nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);
                        }
                        else
                        {
                            nRet3 = 1;
                            nRet4 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7) == 7)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("记录积分抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录积分抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 18) 获取参与活动基数参数
        /// <summary>
        /// 获取参与活动基数参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public OracleParameter[] ModifyPointsAwardNumParam(string facid, string activityid, string poolid)
        {
            OracleParameter[] param = null;
            try
            {
                #region 序列化参数
                param = (OracleParameter[])ParameterCache.GetParams("ModifyPointsAwardNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.Number);
                    param[1] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("ModifyPointsAwardNumParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = poolid;
                param[2].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("18)ModifyPointsAwardNumParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 19) 参与活动次数加
        /// <summary>
        /// 参与活动次数加
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool ModifyPointsAwardNum(string facid, string activityid, string poolid)
        {
            DataBase dataBase = new DataBase();
            try
            {
                OracleParameter[] param = null;
                param = ModifyPointsAwardNumParam(facid, activityid, poolid);
                int row = dataBase.ExecuteNonQuery(CommandType.Text, MODIFY_POINTS_SHAKE_RULE_SQL, param);
                return (row > 0) ? true : false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 20) 更新奖池中奖数量
        /// <summary>
        /// 更新奖池中奖数量
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="awardscode">活动编码</param>
        /// <returns></returns>
        public bool ModifyLotteryNum(string facid, string poolid, string awardscode)
        {
            DataBase dataBase = new DataBase();
            try
            {
                OracleParameter[] param = null;
                param = ModifyPointsLotteryNumParam(facid, poolid, awardscode);
                int row = dataBase.ExecuteNonQuery(CommandType.Text, MODIFY_POINTS_SHAKE_POOL_SQL, param);
                return (row > 0) ? true : false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 21) 积分摇奖活动相关数据更新和插入
        /// <summary>
        /// 积分摇奖活动相关数据更新和插入
        /// </summary>
        /// <param name="sguid">积分摇奖参与guid</param>
        /// <param name="dguid">积分摇奖扣减积分guid</param>
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
        /// <param name="isModifyUserPoints">是否添加积分扣减记录和是否扣减用户积分分值</param>
        /// <returns>记录添加修改是否成功</returns>
        public bool ModifyUserPointByErnie(string sguid, string pguid, string dguid, string facid, string ip, string userguid, string userid, string channel, string activityId, string lotterytype, string area, string points, string result, string joindate, string poolid, string smscontent, bool smsflag, bool isModifyUserPoints)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;

            try
            {
                param1 = GetPointsErnieLogParam(sguid, facid, ip, userguid, userid, channel, activityId, area, points, joindate, poolid, result);
                param2 = GetPointsErnieParLogParam(pguid, facid, ip, userguid, userid, channel, activityId, area, points, joindate, poolid);
                param3 = GetPointsDhLotteryParam(dguid, userguid, Convert.ToInt32(points), lotterytype, channel, activityId, ip, facid, "0");
                param4 = GetModifyUserPointsParam(userguid, Convert.ToInt32(points), facid);
                param5 = ModifyPointsAwardNumParam(facid, activityId, poolid);
                param6 = sendPrewarningParam(3, facid, smscontent);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;
                        int nRet5 = 0;
                        int nRet6 = 0;

                        //增加积分抽奖参与日志
                        string table1 = GetTable(TableName_Ernie, facid);
                        string sql1 = string.Format(ADD_POINTS_Ernie_Log_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加积分抽奖参与日志(真)
                        string table2 = GetTable(TableName_Ernie_Par, facid);
                        string sql2 = string.Format(ADD_POINTS_Ernie_ParLog_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //参与记录+1
                        nRet5 = dbc.ExecuteNonQuery(CommandType.Text, MODIFY_POINTS_SHAKE_RULE_SQL, param5);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql6 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, sql6, param6);
                            nRet6 = 1;
                        }
                        else
                        {
                            nRet6 = 1;
                        }

                        if (isModifyUserPoints) //添加积分扣减记录和修改用户积分信息
                        {
                            string table3 = GetTable(TableName_DhLottery, facid);
                            string sql3 = string.Format(ADD_POINTS_DHLottery_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);

                            string table4 = GetTable(TableName_SGM_User, facid);
                            string sql4 = string.Format(Modify_User_Point_Sql, table4);
                            nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);
                        }
                        else
                        {
                            nRet3 = 1;
                            nRet4 = 1;
                        }

                        if ((nRet1 + nRet3 + nRet4 + nRet5 + nRet6) == 5)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("记录积分摇奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录积分摇奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 22) 获取参与摇奖正确日志组织参数
        /// <summary>
        /// 获取参与摇奖正确日志组织参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP/电话/手机号</param>
        /// <param name="userguid">用户GUID</param>
        /// <param name="userid">用户ID</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="area">所属地区</param>
        /// <param name="points">消耗积分</param>
        /// <param name="joindate">参与时间</param>
        /// <param name="poolid">奖池编码（子）</param>
        /// <returns></returns>
        private OracleParameter[] GetPointsErnieParLogParam(string guid, string facid, string ip, string userguid, string userid, string channel, string activityId, string area, string points, string joindate, string poolid)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsErnieParLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[13];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[2] = new OracleParameter(":USERGUID", OracleType.VarChar, 50);
                    param[3] = new OracleParameter(":IP", OracleType.VarChar, 20);
                    param[4] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[6] = new OracleParameter(":DELETEFLAG", OracleType.VarChar, 1);
                    param[7] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[8] = new OracleParameter(":POINTS", OracleType.Number);
                    param[9] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[10] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[11] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[12] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPointsErnieParLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = userid;
                param[2].Value = userguid;
                param[3].Value = ip;
                param[4].Value = channel;
                param[5].Value = joindate;
                param[6].Value = "1";
                param[7].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[8].Value = points;
                param[9].Value = area;
                param[10].Value = activityId;
                param[11].Value = facid;
                param[12].Value = poolid;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("7)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 23) 万金中奖减去中奖积分
        public bool ModifyUserPointByLottery_WJ(string facid, string userguid, string points)
        {
            try
            {
                OracleParameter[] param4 = null;
                param4 = GetModifyUserPointsParam(userguid, Convert.ToInt32(points), facid);
                DataBase dbc = new DataBase();
                string table4 = GetTable(TableName_SGM_User, facid);
                string sql4 = string.Format(Modify_User_Point_Sql, table4);
                int count = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);
                if (count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录积分抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        public DataTable GetGiftAwardByPoolGuid(string facid,string poolGuid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetGiftAwardByPoolGuid");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":POOLGUID", OracleType.VarChar, 50);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 50);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetGiftAwardByPoolGuid", param);
                }
                param[0].Value = poolGuid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = Get_Shake_Pool_Sql_WJ.Replace("9999", facid);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("UserDhLotteryDal:GetGiftAwardByPoolGuid:" + facid + "---" + poolGuid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
 
        }
    }
}
