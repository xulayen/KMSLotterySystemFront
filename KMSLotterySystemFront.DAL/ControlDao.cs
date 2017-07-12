// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL
// *文件名称：ControlDao.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：活动控制数据操作层,控制活动
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
using System.Collections;

namespace KMSLotterySystemFront.DAL
{
    public class ControlDao
    {
        #region sql列表

        private static string TableName_CONTROL = TableNamePrefix() + "SHAKE_CONTROL_";
        private static string TableName_PARLOG = TableNamePrefix() + "SHAKE_PARLOG_";
        private static string TableName_Log = TableNamePrefix() + "SHAKE_LOG_";
        private static string TableName_LOTTERYLOG = TableNamePrefix() + "SHAKE_LOTTERYLOG_";
        private static string TableName_Rule = TableNamePrefix() + "SHAKE_RULE_";
        private static string TableName_Pool = TableNamePrefix() + "SHAKE_POOL_";
        private static string TableName_MapFacPro = TableNamePrefix() + "SHAKE_MAPFACPRO_";
        private static string TableName_Product = TableNamePrefix() + "SHAKE_PRODUCT_";
        private static string TableName_Signin = TableNamePrefix() + "shake_register_";
        private static string TableName_User = TableNamePrefix() + "USER_";//用户注册表

        private const string Query_Control_Sql = "SELECT SERVICEID,JOINTYPE,FACID,PROID,ACTIVITYID,PROMARK,PTABLEFIELD,DELETEFLAG,CREATEDATE,ACTIVITYNAME FROM t_sgm_wb_shake_control_9999 S WHERE S.FACID=:FACID AND S.JOINTYPE LIKE '%{0}%' AND DELETEFLAG='1'";
        private string Query_Rule_Sql = "SELECT R.ACTIVITYID,R.POOLID,R.BATCHID,r.protype,r.activityname FROM {0} R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND R.FACID=:FACID AND R.ENDDATE>=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') AND R.STARTDATE<=TO_DATE('" + DateTime.Now + "','YYYY-MM-DD HH24:MI:SS') ";
        private string Query_Rule_Sql2 = "SELECT R.ACTIVITYID,R.POOLID,R.BATCHID ,r.protype , r.activityname FROM {0} R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND R.FACID=:FACID AND   TO_CHAR(R.ENDDATE,'YYYY-MM-DD HH24:MI:SS')>=TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') ";
        private string Query_Rule_Sql3 = "SELECT R.ACTIVITYID,R.POOLID,R.BATCHID ,r.protype , r.activityname FROM {0} R WHERE R.DELETEFLAG='1' AND R.ACTIVITYID=:ACTIVITYID AND R.FACID=:FACID AND   TO_CHAR(R.STARTDATE,'YYYY-MM-DD HH24:MI:SS')<=TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') ";

        public const string Query_ParLog_Sql = "SELECT * FROM {0} L WHERE L.DIGIT=:DIGIT AND L.DELETEFLAG='1' AND L.FACID=:FACID AND L.AWARDSNO IS NOT NULL AND L.AWARDSNO <> '0' ";

        public const string Query_ParLog_Sql2 = "SELECT * FROM {0} L WHERE L.USERID=:USERID AND L.DELETEFLAG='1' AND L.FACID=:FACID ";

        public const string Query_ParLog_Sql3 = "SELECT * FROM {0} L WHERE L.DIGIT=:DIGIT AND L.DELETEFLAG='1' AND L.FACID=:FACID  ";

        public const string Query_Register_Sql1 = "SELECT * FROM {0} L WHERE L.IP=:IP AND L.FACID=:FACID AND L.LOTTERYLEVEL IS NOT NULL ";

        public const string Query_Register_Sql2 = "SELECT * FROM {0} L WHERE L.OPENID=:OPENID AND L.FACID=:FACID AND L.LOTTERYLEVEL IS NOT NULL ";

        public const string Query_Procut_Sql = "SELECT * FROM {0} P WHERE P.FACID=:FACID AND P.DELETEFLAG='1'";

        public const string Query_ShakeBaseData_Sql = "SELECT * FROM t_sgm_wb_basedata_9999 B WHERE FACID=:FACID";

        public const string Query_LotteryCount_Act_Sql = "SELECT COUNT(1) FROM {0} L WHERE L.FACID=:FACID AND L.ACTIVITYID=:ACTIVITYID AND TO_CHAR(L.CREATEDAE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') AND L.DELETEFLAG='1'";

        public const string Query_LotteryCount_Config_Sql = "SELECT SUM(P.MAXOPENLOTTER) FROM {0} P LEFT JOIN {1} R ON R.FACID=P.FACID AND R.POOLID=P.POOLID  WHERE R.FACID=:FACID AND R.ACTIVITYID=:ACTIVITYID AND R.DELETEFLAG='1' AND P.DELETEFLAG='1'";

        public const string Query_JcActivity_Sql = "SELECT NVL(AC.BALANCEACCOUNT,0) BALANCEACCOUNT,A.* FROM T_CCN_ACTIVITY_FACTORYACTIVITY A LEFT JOIN T_CCN_ACTIVITY_FACACTIACCOUNT AC ON AC.ACTIVITYID=A.ACTIVITYID WHERE A.ACTIVITYID=:ACTIVITYID";


        ///徐磊 2017年1月13日 10:45:27 修改 to_char(sysdate,'YYYY-MM-DD HH24:MI:SS') 解决 时间延迟问题
        public string QUERY_AD_RULE_POOL_SQL = "SELECT r.ACTIVITYID,"
                                                            + " r.SMSTIMES,"
                                                            + " r.TOTALPOP,"
                                                            + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                            + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                            + " r.CYCLETIMES,"
                                                            + " r.CYCLE,"
                                                            + " r.DIGITLIMIT,"
                                                            + " r.MAXLOTTERYTIMES,"
                                                            + " r.MAXJOINTIMES,"
                                                            + " r.POOLID,"
                                                            + " r.PROTYPE,"
                                                            + " p.AWARDSCODE,"
                                                            + " p.MAXOPENLOTTER,"
                                                            + " p.TOTALTIMES,"
                                                            + " p.LOTTERYTIMES,"
                                                            + " p.AWARDSCALE,"
                                                            + " p.SMSVALUE,"
                                                            + " p.POOLSORT  "
                                                        + " FROM {0} r, {1} p"
                                                        + " WHERE r.ACTIVITYID =:ACTIVITYID"
                                                        + " AND r.POOLID = p.POOLID"
                                                        + " AND r.DELETEFLAG = '1'"
                                                        + " AND p.DELETEFLAG = '1'"
                                                        + " AND r.STARTDATE <="
                                                        + "  TO_DATE(to_char(sysdate,'YYYY-MM-DD HH24:MI:SS'), 'YYYY-MM-DD HH24:MI:SS')"
                                                        + " AND TO_DATE(to_char(sysdate,'YYYY-MM-DD HH24:MI:SS'), 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                        + " AND p.LOTTERYTIMES < p.TOTALTIMES "
                                                        + " AND r.FACID=p.FACID"
                                                        + " AND r.FACID=:FACID"
                                                        + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";

        public string QUERY_AD_RULE_POOL_SQL2 = "SELECT r.ACTIVITYID,"
                                                           + " r.SMSTIMES,"
                                                           + " r.TOTALPOP,"
                                                           + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                           + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                           + " r.CYCLETIMES,"
                                                           + " r.CYCLE,"
                                                           + " r.DIGITLIMIT,"
                                                           + " r.MAXLOTTERYTIMES,"
                                                           + " r.MAXJOINTIMES,"
                                                           + " r.POOLID,"
                                                           + " r.PROTYPE,"
                                                           + " p.AWARDSCODE,"
                                                           + " p.MAXOPENLOTTER,"
                                                           + " p.TOTALTIMES,"
                                                           + " p.LOTTERYTIMES,"
                                                           + " p.AWARDSCALE,"
                                                           + " p.SMSVALUE,"
                                                           + " p.POOLSORT  "
                                                       + " FROM {0} r, {1} p"
                                                       + " WHERE r.ACTIVITYID =:ACTIVITYID"
                                                       + " AND r.POOLID = p.POOLID"
                                                       + " AND r.DELETEFLAG = '1'"
                                                       + " AND p.DELETEFLAG = '1'"
                                                       + " AND r.STARTDATE <="
                                                       + "  TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS')"
                                                       + " AND TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                       + " AND p.LOTTERYTIMES <= p.TOTALTIMES "
                                                       + " AND r.FACID=p.FACID"
                                                       + " AND r.FACID=:FACID"
                                                       + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";

        public string QUERY_AD_RULE_POOL_SQL_Dupont = "SELECT r.ACTIVITYID,"
                                                    + " r.SMSTIMES,"
                                                    + " r.TOTALPOP,"
                                                    + " TO_CHAR(r.STARTDATE, 'YYYY-MM-DD HH24:MI:SS')  STARTDATE,"
                                                    + " TO_CHAR(r.ENDDATE, 'YYYY-MM-DD HH24:MI:SS')  ENDDATE,"
                                                    + " r.CYCLETIMES,"
                                                    + " r.CYCLE,"
                                                    + " r.DIGITLIMIT,"
                                                    + " r.MAXLOTTERYTIMES,"
                                                    + " r.MAXJOINTIMES,"
                                                    + " r.POOLID,"
                                                    + " r.PROTYPE,"
                                                    + " p.AWARDSCODE,"
                                                    + " p.MAXOPENLOTTER,"
                                                    + " p.TOTALTIMES,"
                                                    + " p.LOTTERYTIMES,"
                                                    + " p.AWARDSCALE,"
                                                    + " p.SMSVALUE,"
                                                    + " p.POOLSORT  "
                                                + " FROM {0} r, {1} p"
                                                + " WHERE r.ACTIVITYID =:ACTIVITYID"
                                                + " AND r.POOLID = p.POOLID"
                                                + " AND r.DELETEFLAG = '1'"
                                                + " AND p.DELETEFLAG = '1'"
                                                + " AND r.STARTDATE <="
                                                + "  TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS')"
                                                + " AND TO_DATE('" + DateTime.Now + "', 'YYYY-MM-DD HH24:MI:SS') < r.ENDDATE"
                                                + " AND r.FACID=p.FACID"
                                                + " AND r.FACID=:FACID"
                                                + " ORDER BY r.ACTIVITYSORT, p.POOLSORT ASC";


        private const string QUERY_LOTTERYLOG_SQL = " SELECT COUNT(POOLID) FROM {0} L WHERE L.FACID=:FACID AND L.AWARDSNO=:AWARDSNO AND L.POOLID=:POOLID AND TO_CHAR(JOINDATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') AND DELETEFLAG='1'";

        private const string CHECK_USER_SHAKE_PARLOG_SQL = "SELECT COUNT(USERID) FROM {0} A WHERE A.USERID=:USERID "
                                                             + " AND A.ACTIVITYID=:ACTIVITYID "
                                                             + " AND DIGIT IS NOT NULL "
                                                             + " AND TO_DATE(:SDATE, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE "
                                                             + " AND JOINDATE <=TO_DATE(:EDATE, 'YYYY-MM-DD HH24:MI:SS') "
                                                             + " AND TO_DATE(:LMONTH, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE"
                                                             + " AND DELETEFLAG='1' "
                                                             + " AND FACID =:FACID";

        private const string CHECK_USER_LOTTERYLOG_SQL = " SELECT COUNT(USERID) FROM {0} A  WHERE A.ACTIVITYID=:ACTIVITYID "
                                                         + " AND A.POOLID=:POOLID"
                                                         + " AND A.USERID=:USERID"
                                                         + " AND TO_DATE(:SDATE, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE "
                                                         + " AND JOINDATE <=TO_DATE(:EDATE, 'YYYY-MM-DD HH24:MI:SS') "
                                                         + " AND TO_DATE(:LMONTH, 'YYYY-MM-DD HH24:MI:SS') <=JOINDATE"
                                                         + " AND DELETEFLAG='1'"
                                                         + " AND FACID =:FACID ";



        private const string CHECK_USER_ISLOTTERYFLAG_SQL = " SELECT DIGIT FROM {0} T WHERE T.DIGIT=:DIGIT AND T.ACTIVITYID=:ACTIVITYID AND T.FACID=:FACID AND DELETEFLAG='1'";

        private const string CHECK_USER_ISLOTTERYFLAG_SQL3 = " SELECT DIGIT,AWARDSNO FROM {0} T WHERE T.DIGIT=:DIGIT AND T.ACTIVITYID=:ACTIVITYID AND T.FACID=:FACID AND DELETEFLAG='1'";


        private const string CHECK_USER_ISLOTTERYFLAG_SQL2 = " SELECT DIGIT FROM {0} T WHERE T.DIGIT=:DIGIT  AND T.FACID=:FACID AND DELETEFLAG='1'";

        private const string CHECK_USER_ISLOTTERYFLAG_SQL4 = " SELECT * FROM {0} X WHERE X.SPRO=:DIGIT AND X.FACID=:FACID AND X.FLAG='1' AND X.LOTTERYLEVEL >= 0 ";

        private const string GET_LOTTERY_A_SQL = " SELECT NAME FROM t_sgm_wb_basedata_9999 WHERE CODEID=:CODEID AND DATATYPENAME=:DATATYPENAME AND FACID=:FACID";

        private const string GET_LOTTERY_A_SQL2 = " SELECT {0} FROM t_sgm_wb_basedata_9999 WHERE CODEID=:CODEID AND DATATYPENAME=:DATATYPENAME AND FACID=:FACID";

        private const string GET_LOTTERY_A_SQL5 = " SELECT * FROM t_sgm_wb_basedata_9999 WHERE CODEID=:CODEID AND DATATYPENAME=:DATATYPENAME AND FACID=:FACID";

        private const string GET_LOTTERY_A_SQL3 = " SELECT {0} FROM t_sgm_wb_basedata_9999 WHERE DATATYPENAME=:DATATYPENAME AND FACID=:FACID";

        private const string GET_LOTTERY_A_SQL4 = " SELECT *  FROM t_sgm_wb_basedata_9999 WHERE DATATYPENAME=:DATATYPENAME AND FACID=:FACID";




        private const string GET_LOTTERYNAME_BY_DIGIT_SQL = "SELECT NAME "
                                                            + " FROM t_sgm_wb_basedata_9999"
                                                            + " WHERE CODEID = (SELECT L.AWARDSNO"
                                                                            + " FROM {0} L"
                                                                            + " WHERE L.FACID = :FACID"
            //+ " AND L.ACTIVITYID = :ACTIVITYID"
                                                                            + " AND L.DIGIT = :DIGIT "
                                                                            + " AND L.DELETEFLAG='1')"
                                                            + " AND DATATYPENAME = :DATATYPENAME"
                                                            + " AND FACID = :FACID";


        private const string GET_LOTTERYLEVEL_BY_DIGIT_SQL = "SELECT CODEID "
                                                       + " FROM t_sgm_wb_basedata_9999"
                                                       + " WHERE CODEID = (SELECT L.AWARDSNO"
                                                                       + " FROM {0} L"
                                                                       + " WHERE L.FACID = :FACID"
            //+ " AND L.ACTIVITYID = :ACTIVITYID"
                                                                       + " AND L.DIGIT = :DIGIT "
                                                                       + " AND L.DELETEFLAG='1')"
                                                       + " AND DATATYPENAME = :DATATYPENAME"
                                                       + " AND FACID = :FACID";

        // private const string GET_LOTTERY_DIGITCODEINFO_SQL = "SELECT * FROM  {0} L WHERE L.FACID = :FACID AND L.SPRO = :SPRO  AND  L.FLAG='1' ";

        //20160914-wzp 修改 开始
        //private const string GET_LOTTERY_DIGITCODEINFO_SQL = "SELECT L.*,m.NAME FROM  {0} L left  join  t_sgm_wb_basedata_9999  m  on  l.lotterylevel=m.codeid and  m.facid=:FACID and  m.datatypename='LotteryType' WHERE L.FACID = :FACID AND L.SPRO = :SPRO  AND  L.FLAG='1' ";
        private const string GET_LOTTERY_DIGITCODEINFO_SQL = "SELECT L.*,m.NAME, (select  M.NAME from  t_sgm_wb_basedata_9999  m  where  m.codeid=l.express_company  and   m.facid=:FACID  and  m.datatypename='Express'  ) as  EXPRESSNAME FROM  {0} L left  join  t_sgm_wb_basedata_9999  m  on  l.lotterylevel=m.codeid and  m.facid=:FACID and  m.datatypename='LotteryType' WHERE L.FACID = :FACID AND L.SPRO = :SPRO  AND  L.FLAG='1' ";
        //结束

        private const string GET_LOTTERY_DIGITCODEINFO_SQL2 = "SELECT L.SPRO,L.OPENID,L.F1,L.F14 FROM {0} L WHERE L.FACID = :FACID AND L.GUID = :GUID AND L.FLAG='1'";

        private const string GET_LOTTERY_DIGITCODEINFO_SQL3 = "SELECT * FROM {0} L WHERE L.FACID = :FACID AND L.OPENID = :OPENID AND L.FLAG='1' AND LOTTERYLEVEL IS NOT NULL";

        private const string GET_LOTTERY_DIGITCODEINFO_SQL4 = "SELECT * FROM {0} L WHERE L.FACID = :FACID AND L.IP =:IP  AND L.FLAG='1'  AND LOTTERYLEVEL IS NOT NULL";




        private const string CHECK_SHAKEPRODUCT_SQL = " SELECT * FROM {0} S WHERE S.FACID=:FACID AND S.PROID=:PROID AND DELETEFLAG='1'";

        private const string ADD_LOTTERY_XYD_SQL = "UPDATE {0} U SET U.F7=:XXDCODE WHERE U.SPRO=:DIGITCODE AND U.FACID=:FACID AND U.OPENID=:OPENID AND U.FLAG='1' AND U.F10='001'";

        /// <summary>
        /// 根据手机号获取投保信息
        /// </summary>
        private const string SQL_GetInsuranceByMobile = "select xx.*,xx.rowid  from  t_sgm_shake_insurance  xx  where xx.facid=:FACID   and  xx.mobile=:MOBILE  and xx.deleteflag='1'  order  by  xx.createdate desc   ";


        /// <summary>
        /// 根据微信openid获取投保信息
        /// </summary>
        private const string SQL_GetInsuranceByOpenid = "select xx.*,xx.rowid  from  t_sgm_shake_insurance  xx  where xx.facid=:FACID   and  xx.openid=:OPENID   and xx.deleteflag='1' order  by  xx.createdate desc  ";



        /// <summary>
        /// 根据防伪数码获取投保信息
        /// </summary>
        private const string SQL_GetInsuranceByDigit = "select xx.*,xx.rowid  from  t_sgm_shake_insurance  xx  where xx.facid=:FACID   and  xx.digit=:DIGIT   and xx.deleteflag='1' order  by  xx.createdate desc  ";


        /// <summary>
        /// 检测手机号是否参与过活动
        /// </summary>
        public const string Query_GetRegistUserInfo = "SELECT B.*,B.ROWID FROM {0}  B WHERE  B.FACID=:FACID  and  b.ip=:IP order by b.vdate desc ";

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public const string Query_GetRegistUserInfo2 = " select xx.*,xx.rowid  from  {0} xx where xx.facid=:FACID  and  xx.userid=:USERID  and xx.mobile=:MOBILE  and  xx.deleteflag='1' ";

        /// <summary>
        /// 获取用户信息3
        /// </summary>
        public const string Query_GetRegistUserInfo3 = " select xx.*,xx.rowid  from  {0} xx where xx.facid=:FACID  and  xx.userid=:USERID  and  xx.deleteflag='1' ";


        /// <summary>
        /// 店员授权绑定
        /// </summary>
        public const string Update_UserRegistInfo = "update {0} xx set  xx.USERTYPE='0',xx.openid=:OPENID,xx.wxsex=:WXSEX,xx.wxnickname=:WXNICKNAME,xx.wxcountry=:WXCOUNTRY,xx.wxprovince=:WXPROVINCE,xx.wxcity=:WXCITY,xx.wximg=:WXIMG,xx.username=:USERNAME,xx.province=:PROVINCE,xx.city=:CITY,xx.f1=:F1,xx.f2=:F2,xx.f3=:F3,xx.f4=:F4  where  xx.facid=:FACID and  xx.userid=:USERID and xx.mobile=:MOBILE and xx.deleteflag='1' ";

        /// <summary>
        /// 根据openid获取卡券领取信息
        /// </summary>
        public const string Query_GetConsumeCardInfo = "SELECT xx.*,xx.rowid  FROM T_SGM_CONSUMEREWARD   xx where  xx.facid=:FACID and  xx.openid=:OPENID and  xx.ACTIVITYTYPE='1'  and  xx.deleteflag='1' ";


        public const string Query_GetConsumeCardInfoByUserid = "SELECT xx.*,xx.rowid  FROM T_SGM_CONSUMEREWARD   xx where  xx.facid=:FACID and  xx.F1=:F1 AND  XX.F3=:F3  and  xx.ACTIVITYTYPE='1'  and  xx.deleteflag='1' ";


        /// <summary>
        /// 校验员工是否存在
        /// </summary>
        public const string Query_GetUserRegistInfo = "select mm.*,nn.cardid from {0} mm left join  T_SGM_CONSUMEREWARD  nn  on mm.facid=nn.facid and  mm.openid=nn.openid and  nn.activitytype='1'where mm.facid=:FACID and  mm.openid=:OPENID  and  mm.usertype='0'  and   mm.deleteflag='1'";




        /// <summary>
        /// 根据手机号获取预约信息
        /// </summary>
        private const string GET_ReserveInfoByMobile = " SELECT  A.*,B.STORENAME,B.STOREADDRESS,b.STOREPROVINCE,b.STORECITY  FROM T_SGM_SHAKE_RESERVE  A   LEFT  JOIN  T_SGM_SHAKE_STORE B  ON A.FACID=B.FACID AND  A.STOREID=B.STOREID  WHERE  A.FACID=:FACID AND  A.MOBILE=:MOBILE   and a.channel=:CHANNEL ";

        /// <summary>
        /// 获取手机号预约信息
        /// </summary>
        private const string GET_ReserveInfoByMobile2 = " SELECT  A.*,B.STORENAME,B.STOREADDRESS,b.STOREPROVINCE,b.STORECITY  FROM T_SGM_SHAKE_RESERVE  A   LEFT  JOIN  T_SGM_SHAKE_STORE B  ON A.FACID=B.FACID AND  A.STOREID=B.STOREID  WHERE  A.FACID=:FACID AND  A.MOBILE=:MOBILE ";


        /// <summary>
        /// 获取门店信息
        /// </summary>
        /// <returns></returns>
        private const string GET_StoreInfoByStoreid = " select * from T_SGM_SHAKE_STORE  XX  where xx.facid=:FACID  and  xx.Storeid=:STOREID  and   xx.FLAG='1' ";

        /// <summary>
        /// 检测openid是否注册（壳牌-通行证专用）
        /// </summary>
        /// <returns></returns>
        private const string GET_UserInfoByOpenid = "SELECT  CASE WHEN  B.REWARDMILE IS NULL THEN  0 ELSE   B.REWARDMILE  END  AS  REWARDMILE ,  CASE WHEN B.REWARDMILE >= (SELECT X.LOTTERYMOENY   FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType' AND X.CODEID = '5') THEN (SELECT X.NAME FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType'  AND X.CODEID = '5') WHEN B.REWARDMILE >= (SELECT X.LOTTERYMOENY   FROM t_sgm_wb_basedata_9999 X WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType'  AND X.CODEID = '4') THEN (SELECT X.NAME   FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType'   AND X.CODEID = '4')  WHEN B.REWARDMILE >= (SELECT X.LOTTERYMOENY   FROM t_sgm_wb_basedata_9999 X WHERE X.FACID =:FACID AND X.DATATYPENAME = 'GloryLevelType'   AND X.CODEID = '3') THEN  (SELECT X.NAME  FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType'  AND X.CODEID = '3')  WHEN B.REWARDMILE >= (SELECT X.LOTTERYMOENY   FROM t_sgm_wb_basedata_9999 X   WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType' AND X.CODEID = '2') THEN  (SELECT X.NAME  FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID   AND X.DATATYPENAME = 'GloryLevelType'  AND X.CODEID = '2')   WHEN B.REWARDMILE >= (SELECT X.LOTTERYMOENY FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID  AND X.DATATYPENAME = 'GloryLevelType'   AND X.CODEID = '1') THEN  (SELECT X.NAME  FROM t_sgm_wb_basedata_9999 X  WHERE X.FACID =:FACID   AND X.DATATYPENAME = 'GloryLevelType'  AND X.CODEID = '1')   ELSE   '新手上路' END GloryLevelType,  A.USERGUID, A.USERID, A.USERNAME,  A.PROVINCE, A.CITY, A.OPENID, A.WXSEX ,A.WXNICKNAME,A.WXCOUNTRY, A.WXPROVINCE,  A.WXCITY,  A.WXIMG,  A.CARBRAND,  A.CARAGE,  A.PRODUCTNAME ,A.F5 FROM {0} A LEFT JOIN (SELECT A.FACID, USERGUID,  SUM(A.REWARDMILE ) AS  REWARDMILE   FROM T_SGM_SHAKE_USERMILEAGE A LEFT JOIN t_sgm_wb_basedata_9999 B  ON A.FACID = B.FACID  AND B.DATATYPENAME = 'RewardMileType'  AND A.MILEAGEREWARD = B.CODEID  WHERE A.FACID =:FACID GROUP BY A.USERGUID, A.FACID) B ON A.FACID = B.FACID AND A.USERGUID = B.USERGUID WHERE A.FACID =:FACID AND A.OPENID =:OPENID ";

        /// <summary>
        /// 检测openid是否存在（免登录使用）
        /// </summary>
        /// <returns></returns>
        private const string ExistOpenid = " SELECT * FROM {0} X WHERE X.OPENID=:OPENID AND X.FACID=:FACID ";



        /// <summary>
        /// 查询用户扫码中奖信息
        /// </summary>
        /// <returns></returns>
        private const string GET_UserShakeLotteryInfo = "SELECT B.*,B.ROWID FROM {0}  B WHERE  B.FACID=:FACID and  b.openid=:OPENID  and  b.lotterylevel is not null  order by b.vdate desc ";

        /// <summary>
        /// 根据userguid获取里程信息
        /// </summary>
        /// <returns></returns>

        private const string Query_UserRewardMileInfo = " select XX.*,XX.ROWID ,yy.name from  T_SGM_SHAKE_USERMILEAGE xx  left  join  t_sgm_wb_basedata_9999 yy on xx.facid=yy.facid and  xx.mileagereward=yy.codeid  and  yy.datatypename='RewardMileType' WHERE xx.facid=:FACID and   XX.USERGUID=:USERGUID  ";


        /// <summary>
        /// 获取活动用户领取话费信息
        /// </summary>
        /// <returns></returns>
        //private const string Query_GetUserBillInfo = " SELECT distinct(xx.mobile), xx.cardnum ,xx.createdate  FROM     T_MOBILE_ONLINE_SEND   XX  WHERE  XX.FACID=:FACID   ORDER  BY XX.CREATEDATE DESC ";
        private const string Query_GetUserBillInfo = "SELECT xx.mobile  FROM {0} XX WHERE XX.FACID =:FACID  ORDER BY XX.CREATEDATE DESC ";




        /// <summary>
        /// 获取邀请好友参与情况
        /// </summary>
        /// <returns></returns>
        private const string Query_GetInviteFriendJoinInfo = "SELECT l.invitefrom,l.inviteto,l.createdate , l.invitelotteryflag, case when  l.invitelotteryflag='0'  then 0 else 1 end  as state FROM T_SGM_SHAKE_Invite  L left join  {0} m on   l.facid=m.facid and   l.inviteto=m.ip WHERE L.FACID=:FACID and  l.invitefrom=:INVITEFROM  and TO_CHAR(l.createdate,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')  group by  m.ip,l.invitefrom,l.inviteto,l.createdate , l.invitelotteryflag  order by l.createdate desc";



        /// <summary>
        /// 获取邀请好友参与情况2
        /// </summary>
        /// <returns></returns>
        private const string Query_GetInviteFriendJoinInfo2 = "SELECT l.invitefrom,l.inviteto,l.createdate , l.invitelotteryflag, case when  l.invitelotteryflag='0'  then 2 else 1 end  as state FROM T_SGM_SHAKE_Invite  L left join  {0} m on   l.facid=m.facid and   l.inviteto=m.ip WHERE L.FACID=:FACID and  l.invitefrom=:INVITEFROM  and TO_CHAR(l.createdate,'YYYY-MM')<TO_CHAR(SYSDATE,'YYYY-MM')  group by  m.ip,l.invitefrom,l.inviteto,l.createdate , l.invitelotteryflag  order by l.createdate desc ";


        /// <summary>
        ///获取每月赠送邀请资格
        /// </summary>
        /// <returns></returns>
        private const string Query_GetInviteFriendByType = " SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM  and  l.INVITETYPE=:INVITETYPE  and TO_CHAR(L.CREATEDATE,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM') ORDER  BY  L.CREATEDATE DESC  ";


        /// <summary>
        ///校验用户当月是否扫描同一类产品 
        /// </summary>
        /// <returns></returns>
        private const string Query_GetUserLotteryInfoByMobileandProduct = " SELECT B.*,B.ROWID FROM {0}  B WHERE  B.FACID=:FACID  and b.ip=:IP and  b.lotterylevel is not null and b.productid=:PRODUCTID   and   TO_CHAR(B.VDATE,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')  ";

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
        public DataTable GetService(string facid, string channel)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetServiceParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //param[1] = new OracleParameter(":JOINTYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetServiceParam", param);
                }
                param[0].Value = facid;
                // param[1].Value = channel;
                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, string.Format(Query_Control_Sql, channel), param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetService:" + facid + "---" + channel + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 4) 获取活动开始记录
        /// <summary>
        /// 获取活动开始记录
        /// </summary>
        /// <param name="ACTIVITYID">活动编号</param>
        /// <returns></returns>
        public DataTable GetAwardRecord(string activityid, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetAwardRecordParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetAwardRecordParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Rule, facid);
                string sql = string.Format(Query_Rule_Sql, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetAwardRecord:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 5) 获取活动开始记录2

        /// <summary>
        /// 获取活动是否结束
        /// </summary>
        /// <param name="ACTIVITYID">活动编号</param>
        /// <returns></returns>
        public DataTable GetAwardRecord2(string activityid, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetAwardRecordParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetAwardRecordParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Rule, facid);
                string sql = string.Format(Query_Rule_Sql2, table);


                //Logger.AppLog.Write(" ControlDao:GetAwardRecord: 判断活动结束时间SQL语句： " + sql, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);


                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetAwardRecord:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }

        /// <summary>
        /// 获取活动是否开始
        /// </summary>
        /// <param name="ACTIVITYID">活动编号</param>
        /// <returns></returns>
        public DataTable GetAwardRecord3(string activityid, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetAwardRecordParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetAwardRecordParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Rule, facid);
                string sql = string.Format(Query_Rule_Sql3, table);

                //Logger.AppLog.Write(" ControlDao:GetAwardRecord: 判断活动开始时间SQL语句： " + sql, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetAwardRecord:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 6) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="digitcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable GetQueryParLog(string digitcode, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetQueryParLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetQueryParLogParam", param);
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
                Logger.AppLog.Write("ControlDao:GetQueryParLogParam:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 6) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="digitcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable GetQueryParLog2(string digitcode, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetQueryParLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetQueryParLogParam", param);
                }
                param[0].Value = digitcode;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_PARLOG, facid);
                string sql = string.Format(Query_ParLog_Sql2, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetQueryParLogParam:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 6) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="digitcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable GetQueryParLog3(string digitcode, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetQueryParLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetQueryParLogParam", param);
                }
                param[0].Value = digitcode;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_PARLOG, facid);
                string sql = string.Format(Query_ParLog_Sql3, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetQueryParLogParam:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 7) 获取奖池
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <returns></returns>
        public DataTable GetAward(string facid, string activityid)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetAwardRecordParam2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetAwardRecordParam2", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string tableRule = GetTable(TableName_Rule, facid);
                string tablePool = GetTable(TableName_Pool, facid);

                string sql = string.Format(QUERY_AD_RULE_POOL_SQL, tableRule, tablePool);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];

                //DataSet ds = dataBase.GetDataSet(CommandType.Text, sql, param);

                //if (ds != null && ds.Tables.Count > 0)
                //{
                //    dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
                //}




            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetAward:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 7) 获取奖池
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <returns></returns>
        public DataTable GetAward2(string facid, string activityid)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetAwardRecordParam2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetAwardRecordParam2", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string tableRule = GetTable(TableName_Rule, facid);
                string tablePool = GetTable(TableName_Pool, facid);

                string sql = string.Format(QUERY_AD_RULE_POOL_SQL2, tableRule, tablePool);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];

                //DataSet ds = dataBase.GetDataSet(CommandType.Text, sql, param);

                //if (ds != null && ds.Tables.Count > 0)
                //{
                //    dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
                //}




            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetAward2:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 7.1) 获取奖池（杜邦）
        /// <summary>
        /// 获取奖池（杜邦）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <returns></returns>
        public DataTable GetAwardDupont(string facid, string activityid)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetAwardRecordParam3");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetAwardRecordParam3", param);
                }
                param[0].Value = activityid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string tableRule = GetTable(TableName_Rule, facid);
                string tablePool = GetTable(TableName_Pool, facid);

                string sql = string.Format(QUERY_AD_RULE_POOL_SQL_Dupont, tableRule, tablePool);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetAwardDupont:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion


        #region 8) 获取今日中奖数
        /// <summary>
        /// 获取今日中奖数
        /// </summary>
        /// <param name="poolid">奖池Id</param>
        /// <param name="awardScode">奖项ID</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public object GetDayMaxLottery(string poolid, string awardScode, string facid)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetDayMaxLotteryParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[1] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetDayMaxLotteryParam", param);
                }
                param[0].Value = awardScode;
                param[1].Value = poolid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(QUERY_LOTTERYLOG_SQL, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetDayMaxLottery:" + facid + "---" + poolid + "---" + awardScode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 9) 获取活动的N个月的日期
        /// <summary>
        /// 获取活动的N个月的日期
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public string GetPreMonth(string StartData, string number)
        {
            string sql = "select extract(YEAR from add_months(sysdate,-" + number + ") ) ||'-'||extract(month from add_months(sysdate,-" + number + ") ) ||'-'||extract(DAY from to_date('" + StartData + "', 'YYYY-MM-DD HH24:MI:SS')) as d from dual ";
            DataSet myds = null;
            DataTable dt = null;
            string temp, timeTemp;

            try
            {
                DataBase dataBase = new DataBase();

                myds = dataBase.GetDataSet(CommandType.Text, sql, null);

                if (myds != null && myds.Tables.Count > 0 && myds.Tables[0].Rows.Count > 0)
                {
                    dt = myds.Tables[0];
                    temp = dt.Rows[0][0].ToString() + " 00:00:00";
                    //coming 2012-07-04
                    sql = "select extract(DAY from to_date('" + StartData + "', 'YYYY-MM-DD HH24:MI:SS')) as d,extract(DAY from sysdate) as f from dual ";
                    int t1, t2;

                    DataSet myds1 = null;
                    DataTable dt1 = null;
                    try
                    {
                        myds1 = dataBase.GetDataSet(CommandType.Text, sql, null);

                        if (myds1 != null && myds1.Tables.Count > 0)
                        {
                            dt1 = myds1.Tables[0];
                            t1 = int.Parse(dt1.Rows[0][0].ToString());
                            t2 = int.Parse(dt1.Rows[0][1].ToString());
                            if (t1 < t2)
                            {
                                temp = DateTime.Now.ToString("yyyy-MM") + "-" + dt1.Rows[0][0].ToString() + " 00:00:00";
                            }
                        }
                        else
                        {
                            temp = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";
                        }
                    }
                    catch
                    {
                        temp = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";
                    }
                }
                else
                {
                    temp = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";
                }
                timeTemp = Convert.ToDateTime(temp).ToString() + " 00:00:00";
            }
            catch
            {
                temp = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";
            }
            return temp;
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
        /// <returns></returns>
        public int CheckSendMaxNumber(string facid, string userid, string StartData, string EndDate, string lastmonth, string activityId)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckSendMaxNumberParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[6];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SDATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":EDATE", OracleType.VarChar);
                    param[3] = new OracleParameter(":LMONTH", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USERID", OracleType.VarChar, 30);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckSendMaxNumberParam", param);
                }
                param[0].Value = activityId;
                param[1].Value = StartData;
                param[2].Value = EndDate;
                param[3].Value = lastmonth;
                param[4].Value = facid;
                param[5].Value = userid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_PARLOG, facid);

                string sql = string.Format(CHECK_USER_SHAKE_PARLOG_SQL, table);

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckSendMaxNumber:" + facid + "--" + userid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }

        #endregion

        #region 11) 检查此人指定活动时间内超过最大中奖次数
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
        public int CheckUserPrizeMaxNum(string facid, string userid, string StartData, string EndDate, string lastmonthData, string activityId, string poolId)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckUserPrizeMaxNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SDATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":EDATE", OracleType.VarChar);
                    param[3] = new OracleParameter(":LMONTH", OracleType.VarChar);
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USERID", OracleType.VarChar, 30);
                    param[6] = new OracleParameter(":POOLID", OracleType.VarChar, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("CheckUserPrizeMaxNumParam", param);
                }
                param[0].Value = activityId;
                param[1].Value = StartData;
                param[2].Value = EndDate;
                param[3].Value = lastmonthData;
                param[4].Value = facid;
                param[5].Value = userid;
                param[6].Value = poolId;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(CHECK_USER_LOTTERYLOG_SQL, table);

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckUserPrizeMaxNumParam:" + facid + "--" + userid + "---" + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }

        #endregion

        #region 12) 检查数码是否已经中奖+活动编号
        /// <summary>
        /// 检查数码是否已经中奖
        /// </summary>
        /// <param name="digitCode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编码</param>
        /// <returns></returns>
        public object CkeckDigitalHasBeenWinning(string digitCode, string facid, string activityid)
        {
            //Logger.AppLog.Write("ControlDao:CkeckDigitalHasBeenWinning:" + digitCode + " " + facid + " " + activityid, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CkeckDigitalHasBeenWinningParam");

                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("CkeckDigitalHasBeenWinningParam", param);
                }
                param[0].Value = digitCode;
                param[1].Value = activityid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(CHECK_USER_ISLOTTERYFLAG_SQL, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CkeckDigitalHasBeenWinning:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }

        /// <summary>
        /// 检查数码是否已经中奖
        /// </summary>
        /// <param name="digitCode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编码</param>
        /// <returns></returns>
        public DataTable CkeckDigitalHasBeenWinningContains0(string digitCode, string facid)
        {
            //Logger.AppLog.Write("ControlDao:CkeckDigitalHasBeenWinning:" + digitCode + " " + facid + " " + activityid, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            DataTable oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CkeckDigitalHasBeenWinning4Param");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("CkeckDigitalHasBeenWinning4Param", param);
                }
                param[0].Value = digitCode;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_Signin, facid);

                string sql = string.Format(CHECK_USER_ISLOTTERYFLAG_SQL4, tableLottery);

                oRet = dataBase.ExecuteQuery(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CkeckDigitalHasBeenWinningContains0:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }


        /// <summary>
        /// 检查数码是否已经中奖
        /// </summary>
        /// <param name="digitCode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编码</param>
        /// <returns></returns>
        public DataTable CkeckDigitalHasBeenWinningRed(string digitCode, string facid, string activityid)
        {

            DataTable oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CkeckDigitalHasBeenWinningParam2");

                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("CkeckDigitalHasBeenWinningParam2", param);
                }
                param[0].Value = digitCode;
                param[1].Value = activityid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(CHECK_USER_ISLOTTERYFLAG_SQL3, tableLottery);

                oRet = dataBase.ExecuteQuery(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CkeckDigitalHasBeenWinning:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }

        #endregion

        #region 13) 检查数码是否已经中奖
        /// <summary>
        /// 检查数码是否已经中奖
        /// </summary>
        /// <param name="digitCode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public object CkeckDigitalHasBeenWinning(string digitCode, string facid)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CkeckDigitalHasBeenWinningParam2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("CkeckDigitalHasBeenWinningParam2", param);
                }
                param[0].Value = digitCode;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(CHECK_USER_ISLOTTERYFLAG_SQL2, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CkeckDigitalHasBeenWinning:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 14) 获取奖项名称
        /// <summary>
        /// 获取奖项名称
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项ID</param>
        /// <param name="typeid">类型</param>
        /// <returns></returns>
        public object GetLotteryName(string facid, string codeid, string typeid)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryNameParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":CODEID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryNameParam", param);
                }
                param[0].Value = codeid;
                param[1].Value = typeid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                oRet = dataBase.ExecuteScalar(CommandType.Text, GET_LOTTERY_A_SQL, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryName:" + facid + "---" + typeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 14) 获取充值金额
        /// <summary>
        /// 获取充值金额
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项ID</param>
        /// <param name="typeid">类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public object GetLotteryName2(string facid, string codeid, string typeid, string fieldName)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryNameParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":CODEID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryNameParam", param);
                }
                param[0].Value = codeid;
                param[1].Value = typeid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();


                oRet = dataBase.ExecuteScalar(CommandType.Text, string.Format(GET_LOTTERY_A_SQL2, fieldName), param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryName:" + facid + "---" + typeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 14) 获取充值金额
        /// <summary>
        /// 获取充值金额
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项ID</param>
        /// <param name="typeid">类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public object GetLotteryName3(string facid, string typeid, string fieldName)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryNameParam3");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryNameParam3", param);
                }
                param[0].Value = typeid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();


                oRet = dataBase.ExecuteScalar(CommandType.Text, string.Format(GET_LOTTERY_A_SQL3, fieldName), param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryName:" + facid + "---" + typeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 14.5) 获取具体奖项详情
        /// <summary>
        /// 获取奖项名称
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项ID</param>
        /// <param name="typeid">类型</param>
        /// <returns></returns>
        public DataTable GetLotteryInfo(string facid, string codeid, string typeid)
        {
            DataTable oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryNameParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":CODEID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryNameParam", param);
                }
                param[0].Value = codeid;
                param[1].Value = typeid;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                oRet = dataBase.GetDataSet(CommandType.Text, GET_LOTTERY_A_SQL5, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryName:" + facid + "---" + typeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 18) 获取BaseData配置表数据
        /// <summary>
        /// 获取BaseData配置表数据
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable GetShakeBaseData(string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetShakeBaseDataParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //param[1] = new OracleParameter(":JOINTYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetServiceParam", param);
                }
                param[0].Value = facid;
                // param[1].Value = channel;
                DataBase dataBase = new DataBase();

                dbRet = dataBase.GetDataSet(CommandType.Text, Query_ShakeBaseData_Sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetShakeBaseData:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion


        #region 14.2) 根据类型查询基础配置数据表
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
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetBaseDataByDataType");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetBaseDataByDataType", param);
                }
                param[0].Value = datatypename;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, GET_LOTTERY_A_SQL4, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetBaseDataByDataType(): [facid:" + facid + "]-- [datatypename:" + datatypename + "] --" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return dt;
        }
        #endregion

        #region 15) 获取已经中将的奖项名称
        /// <summary>
        /// 获取已经中将的奖项名称
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项等级</param>
        /// <param name="datatypename">数据字典名称</param>
        /// <param name="digitcode">数码</param>
        /// <param name="activityid">活动编号</param>
        /// <returns>返回已中奖的奖项名称</returns>
        public object GetLotteryName(string facid, string datatypename, string digitcode, string activityid)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryNameParam2");
                //构造参数

                #region 屏蔽
                //if (param == null)
                //{
                //    param = new OracleParameter[4];
                //    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 32);
                //    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                //    param[2] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                //    param[3] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                //    //将参数加入缓存
                //    ParameterCache.PushCache("GetLotteryNameParam2", param);
                //}
                //param[0].Value = activityid;
                //param[1].Value = datatypename;
                //param[2].Value = digitcode;
                //param[3].Value = facid;
                #endregion

                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryNameParam2", param);
                }
                param[0].Value = datatypename;
                param[1].Value = digitcode;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(GET_LOTTERYNAME_BY_DIGIT_SQL, table);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryName:" + facid + "---" + digitcode + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion




        #region 16) 获取活动参与的产品限定范围
        /// <summary>
        /// 获取奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <returns></returns>
        public DataTable GetActiovityProduct(string facid)
        {
            DataTable dbRet = new DataTable();
            try
            {
                OracleParameter[] param = new OracleParameter[1];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[0].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = string.Format(Query_Procut_Sql, GetTable(TableName_Product, facid));

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetActiovityProduct:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 获取当前活动当日获奖数量
        /// <summary>
        /// 获取当前活动当日获奖数量
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityId">活动编号</param>
        /// <returns></returns>
        public int GetMaxLotteryByNowDay(string facid, string activityId)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                param[0].Value = activityId;
                param[1].Value = facid;


                DataBase dataBase = new DataBase();

                string sql = string.Format(Query_LotteryCount_Act_Sql, GetTable(TableName_LOTTERYLOG, facid));

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetMaxLotteryByNowDay:" + facid + "-- " + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }
        #endregion

        #region 获取预设当日最大开奖数量-依据活动sum所有奖池
        /// <summary>
        /// 获取预设当日最大开奖数量-依据活动sum所有奖池
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public int GetMaxLotteryByNowDayConfig(string facid, string activityId)
        {
            int Number = 0;
            try
            {
                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                param[0].Value = activityId;
                param[1].Value = facid;


                DataBase dataBase = new DataBase();

                string sql = string.Format(Query_LotteryCount_Config_Sql, GetTable(TableName_Pool, facid), GetTable(TableName_Rule, facid));

                Number = Convert.ToInt32(dataBase.ExecuteScalar(CommandType.Text, sql, param));
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetMaxLotteryByNowDayConfig:" + facid + "-- " + activityId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return Number;
            }
            return Number;
        }
        #endregion

        #region 15.1) 获取已经中将的奖项级别
        /// <summary>
        ///获取已经中将的奖项级别
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="codeid">奖项等级</param>
        /// <param name="datatypename">数据字典名称</param>
        /// <param name="digitcode">数码</param>
        /// <param name="activityid">活动编号</param>
        /// <returns>返回已中奖的奖项名称</returns>
        public object GetLotteryLevel(string facid, string datatypename, string digitcode, string activityid)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryLevelParam2");
                //构造参数

                #region 屏蔽
                //if (param == null)
                //{
                //    param = new OracleParameter[4];
                //    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 32);
                //    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                //    param[2] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                //    param[3] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                //    //将参数加入缓存
                //    ParameterCache.PushCache("GetLotteryNameParam2", param);
                //}
                //param[0].Value = activityid;
                //param[1].Value = datatypename;
                //param[2].Value = digitcode;
                //param[3].Value = facid;
                #endregion

                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryLevelParam2", param);
                }
                param[0].Value = datatypename;
                param[1].Value = digitcode;
                param[2].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_LOTTERYLOG, facid);

                string sql = string.Format(GET_LOTTERYLEVEL_BY_DIGIT_SQL, table);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryLevel:" + facid + "---" + digitcode + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion


        #region   15.2  根据code 和openID 查询数码注册信息表
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
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLottryCodeInfo");
                //构造参数

                #region 屏蔽
                //if (param == null)
                //{
                //    param = new OracleParameter[4];
                //    param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 32);
                //    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                //    param[2] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                //    param[3] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                //    //将参数加入缓存
                //    ParameterCache.PushCache("GetLotteryNameParam2", param);
                //}
                //param[0].Value = activityid;
                //param[1].Value = datatypename;
                //param[2].Value = digitcode;
                //param[3].Value = facid;
                #endregion

                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":SPRO", OracleType.VarChar, 16);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetLottryCodeInfo", param);
                }
                param[0].Value = factoryid;
                param[1].Value = digitcode;
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(GET_LOTTERY_DIGITCODEINFO_SQL, table);
                dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLottryCodeInfo:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion


        #region 16) 通过factoryid和productids判断T_SGM_SHAKE_PRODUCT_XXXX配置记录
        /// <summary>
        /// 通过factoryid和productids判断T_SGM_SHAKE_PRODUCT_XXXX配置记录
        /// </summary>
        /// <param name="_factoryid">厂家编号</param>
        /// <param name="_productid">产品编号</param>
        /// <returns></returns>
        public DataTable CheckShakeFacProduct(string _factoryid, string _productid)
        {
            DataTable ShakeData = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckShakeFacProductParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":PROID", OracleType.Char, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckShakeFacProductParam", param);
                }
                param[0].Value = _factoryid;
                param[1].Value = _productid;
                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Product, _factoryid);

                string sql = string.Format(CHECK_SHAKEPRODUCT_SQL, table);

                ShakeData = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(_factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return ShakeData;
            }
            return ShakeData;

        }
        #endregion

        /// <summary>
        /// 检查openid是否参与过抽奖
        /// </summary>
        /// <param name="mobile">openid</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable CheckTheOpenidHave(string facid, string openid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckTheOpenidHaveParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":OPENID", OracleType.VarChar);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckTheOpenidHaveParam", param);
                }
                param[0].Value = openid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(Query_Register_Sql2, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckTheOpenidHave:" + facid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }


        #region 新平台
        #region 1) 检查手机号码是否参与过抽奖
        /// <summary>
        /// 检查手机号码是否参与过抽奖
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable CheckTheMobileHave(string facid, string mobile)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckTheMobileHaveParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":IP", OracleType.VarChar);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckTheMobileHaveParam", param);
                }
                param[0].Value = mobile;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(Query_Register_Sql1, table);

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckTheMobileHave:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion


        /// <summary>
        ///  验证门店ID是否符合参与范围
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="storeid">门店编码</param>
        /// <returns></returns>
        public object CheckStoreID(string facid, string storeid)
        {
            object dbobj;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetCheckStoreIDParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCheckStoreIDParam", param);
                }
                param[0].Value = storeid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();


                string sql = "SELECT A.STOREID FROM T_SGM_SHAKE_STORE A WHERE A.STOREID=:STOREID AND A.FACID=:FACID AND FLAG='1'";

                dbobj = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStoreID:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
            return dbobj;
        }


        /// <summary>
        ///  获取门店每日最大可获奖次数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="storeid">门店编码</param>
        /// <returns></returns>
        public object GetStoreMaxLotteryNum(string facid, string storeid)
        {
            object dbobj;
            try
            {
                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 16);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                param[0].Value = storeid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = " SELECT S.MAXLOTTERY FROM T_SGM_SHAKE_STORE S WHERE S.FACID=:FACID AND S.STOREID=:STOREID ";

                dbobj = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetStoreMaxLotteryNum:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
            return dbobj;
        }

        /// <summary>
        ///  检查当天门店中奖总数量是否超过限定数量
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="storeid">门店编码</param>
        /// <returns></returns>
        public object CheckStoreLotteryByDataNow(string facid, string storeid)
        {
            object dbobj;
            try
            {
                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 16);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                param[0].Value = storeid;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = " SELECT COUNT(1) LOTTERY FROM T_SGM_SHAKE_REGISTERUSER_9999 U  WHERE   U.FACID=:FACID AND TO_CHAR(U.VDATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') AND U.LOTTERYLEVEL IS NOT NULL AND U.LOTTERYLEVEL !='0' AND U.F1=:STOREID ";

                dbobj = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStoreLotteryByDataNow:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
            return dbobj;
        }

        /// <summary>
        ///  验证门店ID是否符合参与范围  壳牌喜力超凡项目
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool CheckStoreIDXLCF(string facid, string storeid)
        {
            bool ret = false;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetCheckStoreIDNewParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCheckStoreIDNewParam", param);
                }
                param[0].Value = storeid;
                param[1].Value = facid;


                DataBase dataBase = new DataBase();
                string sql = "SELECT A.isjoinactivity  FROM T_SGM_SHAKE_STORE A WHERE A.STOREID=:STOREID AND A.FACID=:FACID AND FLAG='1' and  ISJOINACTIVITY='1' ";

                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "1") //该门店允许参与活动
                    {
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStoreIDXLCF:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return ret;
        }
        #endregion



        #region 验证门店ID是否符合参与范围   (通用版本)
        /// <summary>
        ///  验证门店ID是否符合参与范围  
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool CheckStoreIsJoinActivity(string facid, string storeid, out string systemstate)
        {
            bool ret = false;
            systemstate = "000";
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetCheckStoreIDNewParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCheckStoreIDNewParam", param);
                }
                param[0].Value = storeid;
                param[1].Value = facid;


                DataBase dataBase = new DataBase();
                string sql = "SELECT A.isjoinactivity  FROM T_SGM_SHAKE_STORE A WHERE A.STOREID=:STOREID AND A.FACID=:FACID AND FLAG='1' ";

                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "1") //该门店允许参与活动
                    {
                        ret = true;
                        systemstate = "001";//门店参与活动
                    }
                    else
                    {
                        systemstate = "002";//此门店不参与活动

                    }
                }
                else
                {

                    systemstate = "003";//门店ID不存在
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStoreIsJoinActivity:" + facid + "---" + storeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return ret;
        }
        #endregion

        #region 检测手机号是否参与过活动，以及是否过中奖
        /// <summary>
        ///  检测手机号是否参与过活动，以及是否过中奖
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckMobileIsJoinActivity(string facid, string mobile, out  string result, out string systemstate)
        {
            bool ret = false;
            systemstate = "000";
            result = "";
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckMobileIsJoinActivityNewParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":USERID", OracleType.VarChar, 11);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckMobileIsJoinActivityNewParam", param);
                }

                param[0].Value = mobile;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = "SELECT  *   FROM  T_SGM_SHAKE_REGISTERUSER_9999  A WHERE A.IP=:USERID  AND  A.FACID=:FACID  AND FLAG='1' ";

                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] drlist = dt.Select(" LOTTERYLEVEL>=1  ");
                    if (drlist != null && drlist.Length > 0)
                    {
                        systemstate = "001";
                        result = "该手机号已经中过奖";
                    }
                    else
                    {
                        systemstate = "002";
                        result = "该手机号参与过抽奖，但是没有中过奖";
                    }
                }
                else
                {
                    systemstate = "003";
                    result = "该手机号没有参与过抽奖活动";
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckMobileIsJoinActivity:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return ret;
        }
        #endregion

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

                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckMobileJoinActivityLimit");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckMobileJoinActivityLimit", param);
                }

                param[0].Value = factoryid;
                param[1].Value = mobile;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, SQL_GetInsuranceByMobile, param);

                if (dt != null && dt.Rows.Count > 0)
                {

                    DataRow[] drlist = dt.Select(" PH_STATUS='投保成功' ");
                    if (drlist != null && drlist.Length > 0)//手机号已经投保成功
                    {
                        systemstate = "201";
                        result = "手机号已经投保成功，不允许再次参与活动";
                    }
                    else
                    {
                        //检测24小时机制
                        DateTime jointimelast = DateTime.Parse(dt.Rows[0]["createdate"].ToString());
                        DateTime dtNightEnd = DateTime.Now;
                        System.TimeSpan ts = dtNightEnd.Subtract(jointimelast);
                        double hours = ts.TotalHours;

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckMobileJoinActivityLimit()  检测手机号是否有资格参与活动:  jointimelast:【" + jointimelast.ToString() + "】 dtNightEnd:【" + dtNightEnd.ToString() + "】：", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckMobileJoinActivityLimit()  检测手机号是否有资格参与活动:  hours:【" + hours + "】 timelimit:【" + timelimit + "】：", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        if (hours >= int.Parse(timelimit))//时间间隔超过设置的时间，允许手机号再次参与
                        {
                            systemstate = "203";
                            result = "手机号允许参与活动";
                            bRet = true;
                        }
                        else
                        {
                            systemstate = "202";
                            result = "手机号不允许允许参与活动(时间受限)";
                        }
                    }
                }
                else//该手机号从未参与投保信息，可以参加活动
                {
                    systemstate = "204";
                    result = "该手机号从未参与投保信息，可以参加活动";
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckMobileJoinActivityLimit()  检测手机号是否有资格参与活动:  factoryid:【" + factoryid + "】 mobile:【" + mobile + "】 【timelimit：" + timelimit + "】  ：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
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

                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckOpenidJoinActivityLimit");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckOpenidJoinActivityLimit", param);
                }

                param[0].Value = factoryid;
                param[1].Value = openid;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, SQL_GetInsuranceByOpenid, param);

                if (dt != null && dt.Rows.Count > 0)
                {

                    DataRow[] drlist = dt.Select(" PH_STATUS='投保成功' ");
                    if (drlist != null && drlist.Length > 0)//微信号已经投保成功
                    {
                        systemstate = "201";
                        result = "微信号已经投保成功，不允许再次参与活动";
                    }
                    else
                    {
                        //检测24小时机制
                        DateTime jointimelast = DateTime.Parse(dt.Rows[0]["createdate"].ToString());
                        DateTime dtNightEnd = DateTime.Now;
                        System.TimeSpan ts = dtNightEnd.Subtract(jointimelast);
                        double hours = ts.TotalHours;

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckOpenidJoinActivityLimit()  检测微信号是否有资格参与活动:  jointimelast:【" + jointimelast.ToString() + "】 dtNightEnd:【" + dtNightEnd.ToString() + "】：", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckOpenidJoinActivityLimit()  检测微信号是否有资格参与活动:  hours:【" + hours + "】 timelimit:【" + timelimit + "】：", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        if (hours >= int.Parse(timelimit))//时间间隔超过设置的时间，允许微信号再次参与
                        {
                            systemstate = "203";
                            result = "微信号允许参与活动";
                            bRet = true;
                        }
                        else
                        {
                            systemstate = "202";
                            result = "微信号不允许允许参与活动(时间受限)";
                        }
                    }
                }
                else//该微信号从未参与投保信息，可以参加活动
                {
                    systemstate = "204";
                    result = "该微信号从未参与投保信息，可以参加活动";
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckOpenidJoinActivityLimit()  检测微信号是否有资格参与活动: 【  factoryid: " + factoryid + "】 【openid: " + openid + "】 【timelimit：" + timelimit + "】  ：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
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

                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckDigitalCodeJoinActivityLimit");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckDigitalCodeJoinActivityLimit", param);
                }

                param[0].Value = factoryid;
                param[1].Value = digitalcode;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, SQL_GetInsuranceByDigit, param);

                if (dt != null && dt.Rows.Count > 0)
                {

                    DataRow[] drlist = dt.Select(" PH_STATUS='投保成功' ");
                    if (drlist != null && drlist.Length > 0)//防伪数码已经投保成功
                    {
                        systemstate = "201";
                        result = "防伪数码已经投保成功，不允许再次参与活动";
                    }
                    else
                    {
                        //检测24小时机制
                        DateTime jointimelast = DateTime.Parse(dt.Rows[0]["createdate"].ToString());
                        DateTime dtNightEnd = DateTime.Now;
                        System.TimeSpan ts = dtNightEnd.Subtract(jointimelast);
                        double hours = ts.TotalHours;

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckDigitalCodeJoinActivityLimit()  检测防伪数码是否有资格参与活动:  jointimelast:【" + jointimelast.ToString() + "】 dtNightEnd:【" + dtNightEnd.ToString() + "】：", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckDigitalCodeJoinActivityLimit()  检测防伪数码是否有资格参与活动:  hours:【" + hours + "】 timelimit:【" + timelimit + "】：", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);


                        if (hours >= int.Parse(timelimit))//时间间隔超过设置的时间，允许微信号再次参与
                        {
                            systemstate = "203";
                            result = "防伪数码允许参与活动";
                            bRet = true;
                        }
                        else
                        {
                            systemstate = "202";
                            result = "防伪数码不允许允许参与活动(时间受限)";
                        }
                    }
                }
                else//该微信号从未参与投保信息，可以参加活动
                {
                    systemstate = "204";
                    result = "该防伪数码从未参与投保信息，可以参加活动";
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  CheckDigitalCodeJoinActivityLimit()  检测防伪数码是否有资格参与活动: 【  factoryid: " + factoryid + "】 【digitalcode: " + digitalcode + "】 【timelimit：" + timelimit + "】  ：" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return bRet;
        }
        #endregion

        #region   15.2  根据code 和中奖纪录id 查询数码注册信息表
        /// <summary>
        ///  根据code 和中奖纪录id 查询数码注册信息表 
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="lid"></param>
        /// <returns></returns>
        public DataTable GetLottryCodeInfo2(string factoryid, string lid)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":GUID", OracleType.VarChar, 32);

                param[0].Value = factoryid;
                param[1].Value = lid;
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(GET_LOTTERY_DIGITCODEINFO_SQL2, table);
                dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLottryCodeInfo2:" + factoryid + "---" + lid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion

        #region   16  根据 OPENID 查询数码注册信息表
        /// <summary>
        ///  根据 OPENID 查询数码注册信息表 
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable GetLottryCodeInfoByOPENID(string factoryid, string openid)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);

                param[0].Value = factoryid;
                param[1].Value = openid;
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(GET_LOTTERY_DIGITCODEINFO_SQL3, table);
                dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLottryCodeInfo2:" + factoryid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion


        #region  16.2)根据手机号是否参与过扫码活动 （T_SGM_SHAKE_REGISTERUSER_9999  ）
        /// <summary>
        /// 根据手机号是否参与过扫码活动
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool GetLottryCodeInfoByMobile(string factoryid, string mobile, out DataTable dt)
        {
            dt = null;
            bool bRet = false;

            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":IP", OracleType.VarChar, 30);

                param[0].Value = factoryid;
                param[1].Value = mobile;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(GET_LOTTERY_DIGITCODEINFO_SQL4, table);

                dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLottryCodeInfoByMobile:" + factoryid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion



        #region 17) 检查数码是否参与过抽奖
        /// <summary>
        /// 检查数码是否参与过抽奖
        /// </summary>
        /// <param name="digitcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public DataTable GetSpCode(string digitcode, string facid)
        {
            DataTable dbRet = new DataTable();

            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                param[0].Value = digitcode;
                param[1].Value = facid;

                DataBase dataBase = new DataBase();

                string sql = "SELECT * FROM T_SGM_STCODE S WHERE S.FLAG='1' AND S.FACID=:FACID AND S.CODE=:DIGIT";

                dbRet = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetSpCode:" + facid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbRet;
            }
            return dbRet;
        }
        #endregion

        #region 18)限制openID每月/每天/每年/ 参与红包发放的最大次数
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
                string sql = "";
                if (datetype == "D")//当天
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USEROPENID=:USEROPENID  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  and   TO_CHAR(XX.CREATETIME,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')  order  by xx.createtime  desc   ";
                }
                else if (datetype == "M")//当月
                {
                    sql = " select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USEROPENID=:USEROPENID  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  and   TO_CHAR(XX.CREATETIME,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')  order  by xx.createtime  desc ";


                }
                else if (datetype == "Y")//当年
                {

                    sql = " select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USEROPENID=:USEROPENID  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  and   TO_CHAR(XX.CREATETIME,'YYYY')=TO_CHAR(SYSDATE,'YYYY')  order  by xx.createtime  desc  ";


                }
                else if (datetype == "N")
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USEROPENID=:USEROPENID  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'   order  by xx.createtime  desc  ";
                }

                else if (datetype == "NN")
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USEROPENID=:USEROPENID  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'   order  by xx.createtime  desc  ";
                }


                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":USEROPENID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = openid;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (datetype != "N")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count >= limitnum)
                        {
                            bRet = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  限制openID每月/每天/每年/ 参与红包发放的最大次数 CheckOpenidHbSendLimit 异常：  [facid:" + facid + "] [openid:" + openid + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion

        #region 18.2)限制手机号每月/每天/每年/ 参与红包发放的最大次数
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
                string sql = "";
                if (datetype == "D")//当天
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USERMOBILE=:USERMOBILE  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  and   TO_CHAR(XX.CREATETIME,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')  order  by xx.createtime  desc  ";
                }
                else if (datetype == "M")//当月
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USERMOBILE=:USERMOBILE  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  and   TO_CHAR(XX.CREATETIME,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')  order  by xx.createtime  desc  ";


                }
                else if (datetype == "Y")//当年
                {

                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USERMOBILE=:USERMOBILE  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  and   TO_CHAR(XX.CREATETIME,'YYYY')=TO_CHAR(SYSDATE,'YYYY')  order  by xx.createtime  desc ";


                }
                else if (datetype == "N")
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USERMOBILE=:USERMOBILE  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  order  by xx.createtime  desc ";
                }

                else if (datetype == "NN")
                {
                    sql = "select xx.*,xx.rowid  from    t_smg_wxpay_hbdetail   xx where xx.facid=:FACID AND  XX.USERMOBILE=:USERMOBILE  AND  XX.HBSENDTYPE='1'   AND  XX.STATE='SUCCESS'  order  by xx.createtime  desc ";
                }

                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":USERMOBILE", OracleType.VarChar, 11);

                param[0].Value = facid;
                param[1].Value = mobile;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (datetype != "N")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count >= limitnum)
                        {
                            bRet = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  限制手机号每月/每天/每年/ 参与红包发放的最大次数 CheckMobileHbSendLimit 异常：  [facid:" + facid + "] [mobile:" + mobile + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion



        #region 19)限制手机号每月/每天/每年/ 邀请朋友最大次数
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
                string sql = "";
                if (datetype == "D")//当天
                {
                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM and TO_CHAR(L.CREATEDATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') ORDER  BY  L.CREATEDATE DESC  ";
                }
                else if (datetype == "M")//当月
                {
                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM and TO_CHAR(L.CREATEDATE,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM') ORDER  BY  L.CREATEDATE DESC  ";


                }
                else if (datetype == "Y")//当年
                {

                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM and TO_CHAR(L.CREATEDATE,'YYYY')=TO_CHAR(SYSDATE,'YYYY') ORDER  BY  L.CREATEDATE DESC  ";

                }
                else if (datetype == "N")
                {
                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM  ORDER  BY  L.CREATEDATE DESC  ";
                }

                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar, 11);

                param[0].Value = facid;
                param[1].Value = mobile;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (datetype != "N")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count >= limitnum)
                        {
                            bRet = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  限制手机号每月/每天/每年/ 邀请朋友最大次数 CheckInviteFriendLimit 异常：  [facid:" + facid + "] [from:" + mobile + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }

        #endregion

        #region 19.2)限制手机号每月/每天/每年/ 邀请同一个人最大次数
        /// <summary>
        /// 限制手机号每月/每天/每年/ 邀请同一个人最大次数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="datetype"></param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public bool CheckInviteFriendLimit(string facid, string mobile, string to, string datetype, int limitnum)
        {
            bool bRet = false;

            try
            {
                string sql = "";
                if (datetype == "D")//当天
                {
                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM   and  L.INVITETO=:INVITETO  and TO_CHAR(L.CREATEDATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') ORDER  BY  L.CREATEDATE DESC  ";
                }
                else if (datetype == "M")//当月
                {
                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM  and  L.INVITETO=:INVITETO  and TO_CHAR(L.CREATEDATE,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM') ORDER  BY  L.CREATEDATE DESC  ";


                }
                else if (datetype == "Y")//当年
                {

                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM and  L.INVITETO=:INVITETO  and TO_CHAR(L.CREATEDATE,'YYYY')=TO_CHAR(SYSDATE,'YYYY') ORDER  BY  L.CREATEDATE DESC  ";

                }
                else if (datetype == "N")
                {
                    sql = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and   L.INVITEFROM=:INVITEFROM and  L.INVITETO=:INVITETO   ORDER  BY  L.CREATEDATE DESC  ";
                }

                OracleParameter[] param = null;
                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":INVITETO", OracleType.VarChar, 11);
                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = to;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (datetype != "N")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count >= limitnum)
                        {
                            bRet = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao  限制手机号每月/每天/每年/ 邀请同一个人最大次数 CheckInviteFriendLimit 异常：  [facid:" + facid + "] [from:" + mobile + "] [to:" + to + "]   [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }

        #endregion

        #region 19.3检测邀请的好友是否参与过活动

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

                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckInviteFriendIsJoin");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":IP", OracleType.VarChar, 11);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckInviteFriendIsJoin", param);
                }
                param[0].Value = facid;
                param[1].Value = to;

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(Query_GetRegistUserInfo, table);
                DataTable dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao 检测邀请的好友是否参与过活动 CheckInviteFriendIsJoin 异常：  [facid:" + facid + "] [to:" + to + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }

        #endregion

        #region   20  根据 OPENID 查询数码注册信息表
        /// <summary>
        /// 根据 openID+数码 提交心愿单信息 
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="openid">openid</param>
        /// <param name="xydcode">心愿单答案</param>
        /// <returns></returns>
        public int DalAddLotteryInfo(string factoryid, string digitcode, string openid, string xydcode)
        {
            int bNum = 0;
            try
            {
                OracleParameter[] param = new OracleParameter[4];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 16);
                param[3] = new OracleParameter(":XXDCODE", OracleType.VarChar, 5);

                param[0].Value = factoryid;
                param[1].Value = openid;
                param[2].Value = digitcode;
                param[3].Value = xydcode;
                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(ADD_LOTTERY_XYD_SQL, table);
                bNum = dataBase.ExecuteNonQuery(CommandType.Text, sql, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:DalAddLotteryInfo:" + factoryid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bNum;
            }
            return bNum;
        }
        #endregion

        #region 21) 通过监控活动ID获取到监控活动平台的信息
        /// <summary>
        /// 通过监控活动ID获取到监控活动平台的信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <returns>返回已中奖的奖项名称</returns>
        public DataTable GetJkAactity(string facid, string activityid)
        {
            DataTable activity = null;
            try
            {
                OracleParameter[] param = new OracleParameter[1];
                param[0] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 32);
                param[0].Value = activityid;

                DataBase dataBase = new DataBase();

                activity = dataBase.GetDataSet(CommandType.Text, Query_JcActivity_Sql, param).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetJkAactiry:" + facid + "---" + activityid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return activity;
            }
            return activity;
        }
        #endregion


        #region 22根据员工编号以及手机号检验用户是否存在
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

                OracleParameter[] param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":USERID", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);


                param[0].Value = facid;
                param[1].Value = userid;
                param[2].Value = mobile;

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_User, facid);
                string sql = string.Format(Query_GetRegistUserInfo2, table);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStaffIsExsit: 【facid:" + facid + "】【userid:" + facid + "】【mobile:" + mobile + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }


        #endregion

        #region 22.2 根据员工编号/手机号 检验用户是否存在
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
            OracleParameter[] param = null;
            try
            {

                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("CheckStaffIsExsit2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckStaffIsExsit2", param);
                }

                param[0].Value = facid;
                param[1].Value = userid;

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_User, facid);
                string sql = string.Format(Query_GetRegistUserInfo3, table);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStaffIsExsit2: 【facid:" + facid + "】【userid:" + userid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion


        #region 22.2根据openid校验店员是否存在
        /// <summary>
        /// 根据openid校验店员是否存在,并返回相关信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid"></param>
        /// <param name="mobile"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool CheckStaffIsExsit(string facid, string openid, out  DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {
                DataBase dataBase = new DataBase();

                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = openid;


                string table = GetTable(TableName_User, facid);
                string sql = string.Format(Query_GetUserRegistInfo, table);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;

                    string userid = dt.Rows[0]["USERID"].ToString();
                    string mobile = dt.Rows[0]["MOBILE"].ToString();

                    //dt中包含店员基础信息，以及店员是否领取卡券,此外还要查询员工总共有多少卡券，以及发放了多少卡券
                    //员工发放卡券总数量
                    int cardtotal = 10;//默认数量是10
                    DataTable dt_cardlimit = GetBaseDataByDataType(facid, "StaffHaveCardLimit");
                    if (dt_cardlimit != null && dt_cardlimit.Rows.Count > 0)
                    {
                        cardtotal = int.Parse(dt_cardlimit.Rows[0]["LOTTERYMOENY"].ToString());
                    }
                    //员工目前发放了多少卡券
                    int cardsendnum = 0;
                    DataTable dt_cardsend = GetStaffSendCardByUserid(facid, userid, mobile);
                    if (dt_cardsend != null && dt_cardsend.Rows.Count > 0)
                    {
                        cardsendnum = dt_cardsend.Rows.Count;
                    }
                    dt.Columns.Add("CARDTOTALNUM", typeof(string));
                    dt.Columns.Add("CARDSENDNUM", typeof(string));

                    dt.Rows[0]["CARDTOTALNUM"] = cardtotal.ToString();
                    dt.Rows[0]["CARDSENDNUM"] = cardsendnum.ToString();
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStaffIsExsit: 【facid:" + facid + "】【openid:" + openid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion

        #region 23员工授权绑定
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

                #region 声明变量
                string OPENID = string.Empty;//员工微信openid
                string WXSEX = string.Empty;//微信性别
                string WXNICKNAME = string.Empty;//微信性别
                string WXCOUNTRY = string.Empty;//微信中国
                string WXPROVINCE = string.Empty;//微信省
                string WXCITY = string.Empty;//微信市
                string WXIMG = string.Empty;//微信图像
                string USERID = string.Empty;//员工编号
                string MOBILE = string.Empty;//员工手机号
                string USERNAME = string.Empty;//员工姓名
                string PROVINCE = string.Empty;//省
                string CITY = string.Empty;//市
                string PHONESYSTEM = string.Empty;//手机系统
                string BROWSERTYPE = string.Empty;//浏览器类型
                string GPSTIME = string.Empty;//获取gps的时间
                string GPS = string.Empty;//GPS 118.91388654687499|32.109331297655
                #endregion

                #region 从userHash读取数据

                if (userHash.ContainsKey("OPENID"))
                {
                    OPENID = userHash["OPENID"].ToString();
                }
                if (userHash.ContainsKey("WXSEX"))
                {
                    WXSEX = userHash["WXSEX"].ToString();
                }
                if (userHash.ContainsKey("WXNICKNAME"))
                {
                    WXNICKNAME = userHash["WXNICKNAME"].ToString();
                }
                if (userHash.ContainsKey("WXCOUNTRY"))
                {
                    WXCOUNTRY = userHash["WXCOUNTRY"].ToString();
                }
                if (userHash.ContainsKey("WXPROVINCE"))
                {
                    WXPROVINCE = userHash["WXPROVINCE"].ToString();
                }
                if (userHash.ContainsKey("WXCITY"))
                {
                    WXCITY = userHash["WXCITY"].ToString();
                }
                if (userHash.ContainsKey("WXIMG"))
                {
                    WXIMG = userHash["WXIMG"].ToString();
                }

                if (userHash.ContainsKey("USERID"))
                {
                    USERID = userHash["USERID"].ToString();
                }
                if (userHash.ContainsKey("MOBILE"))
                {
                    MOBILE = userHash["MOBILE"].ToString();
                }
                if (userHash.ContainsKey("USERNAME"))
                {
                    USERNAME = userHash["USERNAME"].ToString();
                }
                if (userHash.ContainsKey("PROVINCE"))
                {
                    PROVINCE = userHash["PROVINCE"].ToString();
                }
                if (userHash.ContainsKey("CITY"))
                {
                    CITY = userHash["CITY"].ToString();
                }
                if (userHash.ContainsKey("PHONESYSTEM"))
                {
                    PHONESYSTEM = userHash["PHONESYSTEM"].ToString();
                }
                if (userHash.ContainsKey("BROWSERTYPE"))
                {
                    BROWSERTYPE = userHash["BROWSERTYPE"].ToString();
                }
                if (userHash.ContainsKey("GPS"))
                {
                    GPS = userHash["GPS"].ToString();
                }
                if (userHash.ContainsKey("GPSTIME"))
                {
                    GPSTIME = userHash["GPSTIME"].ToString();
                }
                #endregion

                USERNAME = USERNAME.Trim();

                //OracleParameter[] param = GetUserBindParam(facid, userHash);
                //if (param != null)
                //{
                DataBase dataBase = new DataBase();
                string sql_update = "update {0}  xx set  xx.USERTYPE='0',xx.openid='" + OPENID + "',xx.wxsex='" + WXSEX + "',xx.wxnickname='" + WXNICKNAME + "',xx.wxcountry='" + WXCOUNTRY + "',xx.wxprovince='" + WXPROVINCE + "',xx.wxcity='" + WXCITY + "',xx.wximg='" + WXIMG + "',xx.mobile='" + MOBILE + "',xx.province='" + PROVINCE + "',xx.city='" + CITY + "',xx.f1='" + PHONESYSTEM + "',xx.f2='" + BROWSERTYPE + "',xx.f3='" + GPS + "',xx.f4='" + GPSTIME + "'  where  xx.facid='" + facid + "' and  xx.userid='" + USERID + "' and xx.deleteflag='1'";



                string table = GetTable(TableName_User, facid);

                string sql = string.Format(sql_update, table);

                int i = dataBase.ExecuteNonQuery(CommandType.Text, sql, null);
                if (i > 0)
                {
                    bRet = true;

                }
                //}

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:UserAuthBind: 【facid:" + facid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion

        #region 24校验好友openid是否已经领取过
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

                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = openid;

                DataBase dataBase = new DataBase();

                dt = dataBase.ExecuteQuery(CommandType.Text, Query_GetConsumeCardInfo, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckOpenidIsGetCard: 【facid:" + facid + "】【userid:" + facid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion


        #region 25校验员工发放券的数量是否达到上限
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
                string sql = "";
                if (datetype == "D")//当天
                {
                    sql = "SELECT xx.*,xx.rowid  FROM T_SGM_CONSUMEREWARD   xx where  xx.facid=:FACID and  xx.F1=:F1  AND  TO_CHAR(XX.CREATETIME,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')  and  xx.deleteflag='1' ORDER  BY  XX.CREATETIME DESC   ";
                }
                else if (datetype == "M")//当月
                {
                    sql = "SELECT xx.*,xx.rowid  FROM T_SGM_CONSUMEREWARD   xx where  xx.facid=:FACID and  xx.F1=:F1  AND  TO_CHAR(XX.CREATETIME,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')  and  xx.deleteflag='1' ORDER  BY  XX.CREATETIME DESC   ";


                }
                else if (datetype == "Y")//当年
                {

                    sql = "SELECT xx.*,xx.rowid  FROM T_SGM_CONSUMEREWARD   xx where  xx.facid=:FACID and  xx.F1=:F1  AND  TO_CHAR(XX.CREATETIME,'YYYY')=TO_CHAR(SYSDATE,'YYYY')  and  xx.deleteflag='1' ORDER  BY  XX.CREATETIME DESC ";


                }
                else if (datetype == "N")
                {
                    sql = " SELECT xx.*,xx.rowid  FROM  T_SGM_CONSUMEREWARD  xx where  xx.facid=:FACID and  xx.F1=:F1  and  xx.deleteflag='1' ORDER  BY  XX.CREATETIME DESC";
                }
                else if (datetype == "NN")
                {
                    sql = " SELECT xx.*,xx.rowid  FROM  T_SGM_CONSUMEREWARD  xx where  xx.facid=:FACID and  xx.F1=:F1  and  xx.deleteflag='1' ORDER  BY  XX.CREATETIME DESC";

                }


                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":F1", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = userid;

                DataBase dataBase = new DataBase();

                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (datetype != "N")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count >= limitnum)
                        {
                            bRet = true;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:CheckStaffSendCardLimit: 【facid:" + facid + "】【userid:" + userid + "】【datetype:" + datetype + "】【limitnum:" + limitnum + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion


        #region 获取员工已经发放了多少卡券明细
        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataTable GetStaffSendCardByUserid(string facid, string userid, string mobile)
        {
            DataTable dt = null;
            OracleParameter[] param = null;
            try
            {
                param = (OracleParameter[])ParameterCache.GetParams("GetStaffSendCardByUserid");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[1] = new OracleParameter(":F1", OracleType.VarChar, 20);
                    param[2] = new OracleParameter(":F3", OracleType.VarChar, 11);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetStaffSendCardByUserid", param);
                }
                param[0].Value = facid;
                param[1].Value = userid;
                param[2].Value = mobile;
                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, Query_GetConsumeCardInfoByUserid, param);

            }
            catch (Exception ex)
            {

                Logger.AppLog.Write("ControlDao:GetStaffSendCardByUserid: 【facid:" + facid + "】【userid:" + facid + "】-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return dt;
        }

        #endregion


        #region 获取员工绑定组织参数
        /// <summary>
        /// 获取员工绑定组织参数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        private OracleParameter[] GetUserBindParam(string facid, Hashtable userHash)
        {

            OracleParameter[] param = null;
            try
            {

                #region 声明变量
                string OPENID = string.Empty;//员工微信openid
                string WXSEX = string.Empty;//微信性别
                string WXNICKNAME = string.Empty;//微信性别
                string WXCOUNTRY = string.Empty;//微信中国
                string WXPROVINCE = string.Empty;//微信省
                string WXCITY = string.Empty;//微信市
                string WXIMG = string.Empty;//微信图像
                string USERID = string.Empty;//员工编号
                string MOBILE = string.Empty;//员工手机号
                string USERNAME = string.Empty;//员工姓名
                string PROVINCE = string.Empty;//省
                string CITY = string.Empty;//市
                string PHONESYSTEM = string.Empty;//手机系统
                string BROSWERTYPE = string.Empty;//浏览器类型
                string GPSTIME = string.Empty;//获取gps的时间
                string GPS = string.Empty;//GPS 118.91388654687499|32.109331297655
                #endregion

                #region 从userHash读取数据

                if (userHash.ContainsKey("OPENID"))
                {
                    OPENID = userHash["OPENID"].ToString();
                }
                if (userHash.ContainsKey("WXSEX"))
                {
                    WXSEX = userHash["WXSEX"].ToString();
                }
                if (userHash.ContainsKey("WXNICKNAME"))
                {
                    WXNICKNAME = userHash["WXNICKNAME"].ToString();
                }
                if (userHash.ContainsKey("WXCOUNTRY"))
                {
                    WXCOUNTRY = userHash["WXCOUNTRY"].ToString();
                }
                if (userHash.ContainsKey("WXPROVINCE"))
                {
                    WXPROVINCE = userHash["WXPROVINCE"].ToString();
                }
                if (userHash.ContainsKey("WXCITY"))
                {
                    WXCITY = userHash["WXCITY"].ToString();
                }
                if (userHash.ContainsKey("WXIMG"))
                {
                    WXIMG = userHash["WXIMG"].ToString();
                }

                if (userHash.ContainsKey("USERID"))
                {
                    USERID = userHash["USERID"].ToString();
                }
                if (userHash.ContainsKey("MOBILE"))
                {
                    MOBILE = userHash["MOBILE"].ToString();
                }
                if (userHash.ContainsKey("USERNAME"))
                {
                    USERNAME = userHash["USERNAME"].ToString();
                }
                if (userHash.ContainsKey("PROVINCE"))
                {
                    PROVINCE = userHash["PROVINCE"].ToString();
                }
                if (userHash.ContainsKey("CITY"))
                {
                    CITY = userHash["CITY"].ToString();
                }
                if (userHash.ContainsKey("PHONESYSTEM"))
                {
                    PHONESYSTEM = userHash["PHONESYSTEM"].ToString();
                }
                if (userHash.ContainsKey("BROWSERTYPE"))
                {
                    BROSWERTYPE = userHash["BROWSERTYPE"].ToString();
                }
                if (userHash.ContainsKey("GPS"))
                {
                    GPS = userHash["GPS"].ToString();
                }
                if (userHash.ContainsKey("GPSTIME"))
                {
                    GPSTIME = userHash["GPSTIME"].ToString();
                }
                #endregion

                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetUserBindParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[17];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":WXSEX ", OracleType.Char, 1);
                    param[3] = new OracleParameter(":WXNICKNAME", OracleType.VarChar, 200);
                    param[4] = new OracleParameter(":WXCOUNTRY", OracleType.VarChar, 20);
                    param[5] = new OracleParameter(":WXPROVINCE", OracleType.VarChar, 20);
                    param[6] = new OracleParameter(":WXCITY", OracleType.VarChar, 20);
                    param[7] = new OracleParameter(":WXIMG", OracleType.VarChar, 200);
                    param[8] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                    param[9] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    param[10] = new OracleParameter(":USERNAME", OracleType.VarChar, 20);
                    param[11] = new OracleParameter(":PROVINCE", OracleType.VarChar, 10);
                    param[12] = new OracleParameter(":CITY", OracleType.VarChar, 10);
                    param[13] = new OracleParameter(":F1", OracleType.VarChar, 50);
                    param[14] = new OracleParameter(":F2", OracleType.VarChar, 50);
                    param[15] = new OracleParameter(":F3", OracleType.VarChar, 50);
                    param[16] = new OracleParameter(":F4", OracleType.VarChar, 50);

                    //update T_SGM_USER_9999 xx set  xx.USERTYPE='0',xx.openid=:OPENID,xx.wxsex=:WXSEX,xx.wxnickname=:WXNICKNAME,xx.wxcountry=:WXCOUNTRY,xx.wxprovince=:WXPROVINCE,xx.wxcity=:WXCITY,xx.wximg=:WXIMG,xx.username=:USERNAME,xx.province=:PROVINCE,xx.city=:CITY,xx.f1=:F1,xx.f2=:F2,xx.f3=:F3,xx.f4=:F4  where  xx.facid=:FACID and  xx.userid=:USERID and xx.mobile=:MOBILE and xx.deleteflag='1'
                    //将参数加入缓存
                    ParameterCache.PushCache("GetUserBindParam", param);
                }

                param[0].Value = facid;
                param[1].Value = OPENID;
                param[2].Value = WXSEX;
                param[3].Value = WXNICKNAME;
                param[4].Value = WXCOUNTRY;
                param[5].Value = WXPROVINCE;
                param[6].Value = WXCITY;
                param[7].Value = WXIMG;
                param[8].Value = USERID;
                param[9].Value = MOBILE;
                param[10].Value = USERNAME;
                param[11].Value = PROVINCE;
                param[12].Value = CITY;
                param[13].Value = PHONESYSTEM;
                param[14].Value = BROSWERTYPE;
                param[15].Value = GPS;
                param[16].Value = GPSTIME;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("获取员工绑定组织参数----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return param;
        }
        #endregion


        #region 检测手机号是否预约
        /// <summary>
        /// 检测手机号是否预约
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool GetReserveByMobile(string factoryid, string mobile, string channel, out DataTable dt)
        {
            bool bRet = false;
            dt = null;
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetReserveByMobile");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    param[2] = new OracleParameter(":CHANNEL", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetReserveByMobile", param);
                }

                param[0].Value = factoryid;
                param[1].Value = mobile;
                param[2].Value = channel;

                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, GET_ReserveInfoByMobile, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  GetReserveByMobile----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }

            return bRet;
        }
        #endregion



        #region 检测手机号是否预约2
        /// <summary>
        /// 检测手机号是否预约2
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool GetReserveByMobile(string factoryid, string mobile, out DataTable dt)
        {
            bool bRet = false;
            dt = null;
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetReserveByMobile");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetReserveByMobile", param);
                }

                param[0].Value = factoryid;
                param[1].Value = mobile;
                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, GET_ReserveInfoByMobile2, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  GetReserveByMobile----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }

            return bRet;
        }
        #endregion



        #region 检测门店是否参与活动

        /// <summary>
        /// 检测门店是否参与活动
        /// <param name="factoryid">厂家编号</param>
        /// <param name="storeid">门店id</param>
        /// <param name="dt">门店信息</param>
        /// <returns></returns>
        public bool CheckStoreIsJoinActivity(string factoryid, string storeid, out DataTable dt)
        {
            dt = null;
            bool bRet = false;
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("CheckStoreIsJoinActivity");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckStoreIsJoinActivity", param);
                }

                param[0].Value = storeid;
                param[1].Value = factoryid;

                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, GET_StoreInfoByStoreid, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string isjoin = dt.Rows[0]["ISJOINACTIVITY"].ToString();
                    if (isjoin == "1")
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  CheckStoreIsJoinActivity----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

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
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("CheckOpenidIsRegist");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckOpenidIsRegist", param);
                }

                param[0].Value = factoryid;
                param[1].Value = openid;

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_User, factoryid);
                string sql = string.Format(GET_UserInfoByOpenid, table);
                dt_userinfo = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (dt_userinfo != null && dt_userinfo.Rows.Count > 0)
                {
                    string userid = dt_userinfo.Rows[0]["USERID"].ToString();//用户账号/手机号
                    string USERGUID = dt_userinfo.Rows[0]["USERGUID"].ToString();//用户guid

                    //将该用户的每月赠送的邀请好友的奖励中添加到此表中dt_userinfo

                    //检测该用户当月是否使用每月赠送的奖励邀请好友
                    string giveinvite = "1";
                    if (CheckInviteFriendByType(factoryid, userid, "1"))
                    {
                        giveinvite = "0";
                    }
                    dt_userinfo.Columns.Add("GIVEINVITE", typeof(string));
                    dt_userinfo.Rows[0]["GIVEINVITE"] = giveinvite;

                    bRet = true;

                    //查询用户中奖信息
                    dt_userlotteryinfo = GetUserShakeLotteryInfo(factoryid, openid);

                    //将用户最新扫码的油品添加到用户基础信息表中
                    string lastproduct = "";
                    dt_userinfo.Columns.Add("LASTPRODUCT", typeof(string));
                    if (dt_userlotteryinfo != null && dt_userlotteryinfo.Rows.Count > 0)
                    {
                        lastproduct = dt_userlotteryinfo.Rows[0]["PRODUCTNAME"].ToString();
                    }
                    dt_userinfo.Rows[0]["LASTPRODUCT"] = lastproduct;

                    //查询用户里程信息
                    dt_usermile = GetUserRewardMileInfo(factoryid, USERGUID);
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  CheckOpenidIsRegist [factoryid:" + factoryid + "] [openid:" + openid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion


        #region 检测openid是否注册（免登录使用）
        /// <summary>
        /// 检测openid是否注册（免登录使用）
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <param name="dt_userinfo"></param>
        /// <param name="dt_userlotteryinfo"></param>
        /// <param name="dt_usermile"></param>
        /// <returns></returns>
        public bool CheckOpenidIsRegist(string factoryid, string openid, out DataTable codeInfo)
        {
            bool bRet = false;
            OracleParameter[] param = null;
            codeInfo = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("CheckOpenidIsRegist");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckOpenidIsRegist", param);
                }

                param[0].Value = factoryid;
                param[1].Value = openid;

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(ExistOpenid, table);
                codeInfo = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (codeInfo != null && codeInfo.Rows != null && codeInfo.Rows.Count > 0)
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  CheckOpenidIsRegist [factoryid:" + factoryid + "] [openid:" + openid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion


        #region 查询用户扫码中奖信息
        /// <summary>
        /// 查询用户扫码中奖信息
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable GetUserShakeLotteryInfo(string factoryid, string openid)
        {
            DataTable dt = null;
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetUserShakeLotteryInfo");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetUserShakeLotteryInfo", param);
                }

                param[0].Value = factoryid;
                param[1].Value = openid;
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);
                string sql = string.Format(GET_UserShakeLotteryInfo, table);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  GetUserShakeLotteryInfo [factoryid:" + factoryid + "] [openid:" + openid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return dt;

        }
        #endregion


        #region 根据userguid查询里程信息
        /// <summary>
        /// 根据userguid查询里程信息
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="userguid"></param>
        /// <returns></returns>
        public DataTable GetUserRewardMileInfo(string factoryid, string userguid)
        {
            DataTable dt = null;
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetUserRewardMileInfo");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":USERGUID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetUserRewardMileInfo", param);
                }

                param[0].Value = factoryid;
                param[1].Value = userguid;
                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, Query_UserRewardMileInfo, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.ControlDao.cs  GetUserRewardMileInfo [factoryid:" + factoryid + "] [userguid:" + userguid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }
            return dt;

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
            OracleParameter[] param = null;
            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetUserBillInfo");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetUserBillInfo", param);
                }

                param[0].Value = factoryid;
                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_User, factoryid);
                string sql = string.Format(Query_GetUserBillInfo, table);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.ControlDao.cs  GetUserBillInfo:" + factoryid + "--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
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
            OracleParameter[] param = null;
            try
            {

                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetInviteFriendJoinInfo");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar, 11);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetInviteFriendJoinInfo", param);
                }

                param[0].Value = facid;
                param[1].Value = mobile;
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(Query_GetInviteFriendJoinInfo, table);

                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                DataTable dt2 = GetInviteFriendJoinInfo2(facid, mobile);
                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt2.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr["invitefrom"] = item["invitefrom"].ToString();
                            dr["inviteto"] = item["inviteto"].ToString();
                            dr["createdate"] = DateTime.Parse(item["createdate"].ToString());
                            dr["state"] = item["state"].ToString();
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dt = dt2;
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.ControlDao.cs   GetInviteFriendJoinInfo: [facid:" + facid + "][mobile:" + mobile + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;

        }
        #endregion



        #region 获取邀请好友参与情况2
        /// <summary>
        /// 获取邀请好友参与情况2
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataTable GetInviteFriendJoinInfo2(string facid, string mobile)
        {

            DataTable dt = null;
            OracleParameter[] param = null;
            try
            {

                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("GetInviteFriendJoinInfo2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar, 11);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetInviteFriendJoinInfo2", param);
                }

                param[0].Value = facid;
                param[1].Value = mobile;
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(Query_GetInviteFriendJoinInfo2, table);

                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.ControlDao.cs   GetInviteFriendJoinInfo: [facid:" + facid + "][mobile:" + mobile + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;

        }
        #endregion









        #region 检测用户当月是否使用赠送的资格邀请过好友
        public bool CheckInviteFriendByType(string facid, string mobile, string invietype)
        {
            bool bRet = false;

            OracleParameter[] param = null;

            try
            {


                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("CheckInviteFriendByType");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                    param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar, 11);
                    param[2] = new OracleParameter(":INVITETYPE", OracleType.Char, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckInviteFriendByType", param);
                }


                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = invietype;

                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, Query_GetInviteFriendByType, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.ControlDao.cs   CheckInviteFriendByType: [facid:" + facid + "][mobile:" + mobile + "] [invietype:" + invietype + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

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
            OracleParameter[] param = null;

            try
            {
                //构造参数
                param = (OracleParameter[])ParameterCache.GetParams("CheckScanProductAgain");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":IP", OracleType.VarChar, 30);
                    param[2] = new OracleParameter(":PRODUCTID", OracleType.VarChar, 30);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckScanProductAgain", param);
                }
                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = productid;

                DataBase dataBase = new DataBase();

                string table = GetTable(TableName_Signin, facid);

                string sql = string.Format(Query_GetUserLotteryInfoByMobileandProduct, table);

                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.ControlDao.cs  CheckScanProductAgain: [facid:" + facid + "][mobile:" + mobile + "] [productid:" + productid + "]-" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion
    }
}
