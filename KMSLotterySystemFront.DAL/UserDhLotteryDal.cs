// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL 
// *文件名称：UserDhLotteryDal
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 14:59:16  
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
    public class UserDhLotteryDal
    {
        #region sql列表


        private static string TableName_DhLottery = TableNamePrefix() + "POINTS_SHAKE_LOG_";

        private static string TableName_SGM_User = TableNamePrefix() + "USER_";

        private static string TableName_SHAKE_REGISTERUSER = TableNamePrefix()+"SHAKE_REGISTERUSER_";

        private const string ADD_POINTS_DHLottery_SQL = " INSERT INTO {0}  (GUID,USERGUID,POINTS,LOTTERYTYPE,DELETEFLAG,ACTIVITYID,CREATEDATE,FACID,CHANNEL,IP)  "
                                                 + " VALUES(:GUID,:USERGUID,:POINTS,:LOTTERYTYPE,:DELETEFLAG,:ACTIVITYID,:CREATEDATE,:FACID,:CHANNEL,:IP) ";

        private const string Get_Sgm_User_Sql = " SELECT U.USERGUID,U.USERID,U.MOBILE,U.USERNAME,U.POINTTOTAL,U.POINTVALID,U.POINTUSED,U.DELETEFLAG,U.FACID FROM {0} U WHERE (U.USERID=:USERID OR U.MOBILE=:MOBILE) AND U.FACID=:FACID AND U.DELETEFLAG='1'";

        private const string Modify_User_Point_Sql = "UPDATE T_SGM_USER_00446 SET POINTVALID=POINTVALID-:POINTUSED,POINTUSED=POINTUSED+:POINTUSED WHERE DELETEFLAG='1' AND  POINTVALID>=:POINTUSED AND USERGUID=:USERGUID AND FACID =:FACID";

        private const string Modify_Register_Info_Sql = @"UPDATE {0} x set x.F5=:F5,x.F6=:F6,X.F15=:F15 WHERE X.GUID=:GUID AND X.facid=:FACID ";
        
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
        private OracleParameter[] GetPointsDhLotteryParam(string guid, string userguid, int points, string lotterytype, string channel, string activityId, string ip, string facid)
        {

            OracleParameter[] param = null;
            try
            {
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetPointsDhLotteryParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[10];

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

        #region 4) 添加积分抽奖参与日志

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
        public bool AddPointsDhLottery(string guid, string userguid, int points, string lotterytype, string channel, string activityId, string ip, string facid)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetPointsDhLotteryParam(guid, userguid, points, lotterytype, channel, activityId, ip, facid);

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

        #region 5) 获取用户相关信息

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


        #region 6) 修改用户相关信息

        public bool ModifyRegisterInfo(string facid, string lid, string mobile, string postname, string postaddr)
        {
            bool bRet = false;

            try
            {
                string table = GetTable(TableName_SHAKE_REGISTERUSER, facid);
                string sql = string.Format(Modify_Register_Info_Sql, table);
                OracleParameter[] param = null;
                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("ModifyRegisterInfoParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[5];

                    param[0] = new OracleParameter(":F5", OracleType.VarChar);
                    param[1] = new OracleParameter(":F6", OracleType.VarChar);
                    param[2] = new OracleParameter(":F15", OracleType.VarChar);
                    param[3] = new OracleParameter(":GUID", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar);


                    //将参数加入缓存
                    ParameterCache.PushCache("ModifyRegisterInfoParam", param);
                }
                param[0].Value = postname;
                param[1].Value = mobile;
                param[2].Value = postaddr;
                param[3].Value = lid;
                param[4].Value = facid;

                #endregion
                DataBase dataBase = new DataBase();

                bRet = dataBase.ExecuteNonQuery(CommandType.Text, sql, param) > 0;

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("UserDhLotteryDal:ModifyRegisterInfoParam:" + facid + "---" + postname + "--" + mobile + "--" + lid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion


    }
}
