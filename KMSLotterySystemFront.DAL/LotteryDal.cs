// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL
// *文件名称：LotteryDal.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：抽奖相关日志记录数据访问类
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
    public class LotteryDal
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
        private static string TableName_Verification = TableNamePrefix() + "CONSUMEREWARD";
        private static string TableName_User = TableNamePrefix() + "USER_";

        private const string SEND_SMS_SQL = "INSERT INTO T_SMS_SEND_TO(SEND_USERID,SEND_TO,SUBJECT,SUBJECT_TYPE,SEND_TYPE,SEND_CONTENT,CUSTOMER_CODE) select 'CCN_System',REMINDID,'预警提示','6','0'||TYPE,'{0}',:FACID FROM T_SGM_SHAKE_REMIND where FACID=:FACID and REMINDLEVEL<=:REMINDLEVEL  and DELETEFLAG='1'";

        private const string UPDATE_SHAKE_POOL_SQL = "UPDATE {0} P SET LOTTERYTIMES=LOTTERYTIMES+1 WHERE POOLID=:POOLID AND AWARDSCODE=:AWARDSCODE AND LOTTERYTIMES<TOTALTIMES AND FACID=:FACID";

        private const string UPDATE_SHAKE_RULE_SQL = "UPDATE {0} SET TOTALPOP=TOTALPOP+1 WHERE FACID=:FACID AND ACTIVITYID=:ACTIVITYID  AND POOLID=:POOLID AND DELETEFLAG='1'";

        private const string ADD_SHAKE_LOG_SQL = " INSERT INTO {0}  (GUID,USERID,USERTYPE,UPCONTENT,DIGIT,ACTIVITYID,AREA,JOINDATE,AWARDSNO,PROTYPE,DOWNCONTENT,CREATEDAE,FACID) "
                                                 + " VALUES(:GUID,:USERID,:USERTYPE,:UPCONTENT,:DIGIT,:ACTIVITYID,:AREA,:JOINDATE,:AWARDSNO,:PROTYPE,:DOWNCONTENT,:CREATEDAE,:FACID)";

        private const string ADD_SHAKE_PARLOG_SQL = "INSERT INTO {0}(GUID,USERID,USERTYPE,UPCONTENT,DIGIT,ACTIVITYID,AREA,JOINDATE,AWARDSNO,FACID,PROID,DOWNCONTENT,CREATEDAE,AGAINCODE,POOLID) "
                                                + " VALUES (:GUID,:USERID,:USERTYPE,:UPCONTENT,:DIGIT,:ACTIVITYID,:AREA,:JOINDATE,:AWARDSNO,:FACID,:PROID,:DOWNCONTENT,:CREATEDAE,:AGAINCODE,:POOLID)";

        private const string Query_SHAKE_PARLOG_FirstTime_SQL = "SELECT A.JOINDATE FROM {0} A WHERE A.FACID=:FACID AND A.DIGIT=:DIGIT ORDER BY A.CREATEDAE ASC";

        private const string ADD_SHAKE_NEWLOTTERY_SQL = "INSERT INTO {0}(GUID,USERID,USERTYPE,UPCONTENT,DIGIT,ACTIVITYID,AREA,JOINDATE,AWARDSNO,FACID,PROTYPE,DOWNCONTENT,CREATEDAE,AGAINCODE,POOLID) "
                                                    + " VALUES (:GUID,:USERID,:USERTYPE,:UPCONTENT,:DIGIT,:ACTIVITYID,:AREA,:JOINDATE,:AWARDSNO,:FACID,:PROTYPE,:DOWNCONTENT,:CREATEDAE,:AGAINCODE,:POOLID)";

        private const string SIGNIN_SHAKE_SQL = "SELECT NEWCODE FROM {0} R WHERE R.FACID=:FACTORYID AND R.SPRO=:DIGITCODE AND R.FLAG='1'";

        private const string SIGNIN_SHAKE_SQL3 = "SELECT NEWCODE FROM {0} R WHERE R.FACID=:FACTORYID AND R.SPRO=:DIGITCODE AND R.FLAG='1' AND R.STATE='9'";

        private const string SIGNIN_SHAKE_SQL2 = "SELECT NEWCODE FROM {0} R WHERE R.FACID=:FACTORYID AND R.SPRO=:DIGITCODE AND R.FLAG='9'";

        private const string SIGNIN_CONSUMEREWARD_SQL = "SELECT L.*,L.ROWID FROM {0} L WHERE L.DIGITALCODE=:DIGITALCODE AND  L.FACID=:FACID";

        private const string SIGNIN_SHAKE_SQL4 = "SELECT SPRO FROM {0} R WHERE R.FACID=:FACTORYID AND R.SPRO=:DIGITCODE AND OPENID=:OPENID AND R.FLAG='1' AND R.STATE='9'";

        /// <summary>
        /// 获取中奖数码信息
        /// </summary>
        private const string SQL_GETLOTTERYCODEINFO = "SELECT G.GUID,G.JOINDATE,G.AWARDSNO,B.NAME FROM {0} G  JOIN  t_sgm_wb_basedata_9999 B ON G.FACID=B.FACID AND  G.AWARDSNO=B.CODEID AND  B.DATATYPENAME='LotteryType' WHERE G.FACID=:FACID AND  G.DIGIT=:DIGIT   AND  G.DELETEFLAG='1'  ORDER BY G.JOINDATE DESC ";



        /// <summary>
        /// 获取中奖数码信息New
        /// </summary>
        private const string SQL_GETLOTTERYCODEINFONew = "SELECT G.GUID,G.JOINDATE,G.AWARDSNO,B.NAME ,R.LOTTERYLEVEL,R.LOTTERYNAME,R.F12,R.VDATE, r.ip,r.openid   FROM {0} G  JOIN  t_sgm_wb_basedata_9999 B ON G.FACID=B.FACID AND  G.AWARDSNO=B.CODEID AND  B.DATATYPENAME='LotteryType'   join {1}   R on  G.FACID=R.FACID  AND G.AWARDSNO=R.LOTTERYLEVEL AND G.DIGIT=R.SPRO WHERE G.FACID=:FACID AND  G.DIGIT=:DIGIT   AND  G.DELETEFLAG='1'  ORDER BY G.JOINDATE DESC ";


        private const string SIGINUSER_SHAKE_SQL = "SELECT SPRO,CATEGORY,IP,VDATE,LOTTERYLEVEL,CHANGE_WAY,CHANGE_TYPE,USER_NAME,USER_ADDRESS,USER_ZIPCODE,USER_TELEPHONE,CHANGE_DATE,POST_DATE,STATE,CLOSEING_DATE,MEMO,FLAG,LOTTERYNAME,COMPANY,NEWCODE,FACID FROM {0} WHERE LOWER(SPRO) = LOWER(:SPRO) AND FACID=:FACID";

        private const string ADD_SININUSER_SQL = "INSERT INTO {0}(GUID,SPRO,CATEGORY,IP,VDATE,LOTTERYLEVEL,CHANGE_WAY,CHANGE_TYPE,USER_NAME,USER_ADDRESS,USER_ZIPCODE,USER_TELEPHONE,CHANGE_DATE,POST_DATE,STATE,CLOSEING_DATE,MEMO,FLAG,LOTTERYNAME,COMPANY,NEWCODE,FACID)"
                                                + " VALUES(:GUID,:SPRO,:CATEGORY,:IP,:VDATE,:LOTTERYLEVEL,:CHANGE_WAY,:CHANGE_TYPE,:USER_NAME,:USER_ADDRESS,:USER_ZIPCODE,:USER_TELEPHONE,:CHANGE_DATE,:POST_DATE,:STATE,:CLOSEING_DATE,:MEMO,:FLAG,:LOTTERYNAME,:COMPANY,:NEWCODE,:FACID)";

        private const string ADD_REGISTER_SQL = "INSERT INTO {0}(GUID,SPRO,CATEGORY,IP,LOTTERYLEVEL,STATE,FLAG,NEWCODE,FACID,VDATE)"
                                                + " VALUES(:GUID,:SPRO,:CATEGORY,:IP,:LOTTERYLEVEL,'9','1',:NEWCODE,:FACID,:VDATE)";

        private const string ADD_LOTTERYREGISTER_SQL = "INSERT INTO {0}(GUID,SPRO,CATEGORY,IP,LOTTERYLEVEL,STATE,FLAG,NEWCODE,FACID,VDATE,{1})"
                                                + " VALUES(:GUID,:SPRO,:CATEGORY,:IP,:LOTTERYLEVEL,'9','1',:NEWCODE,:FACID,:VDATE,{2})";

        public const string ADD_CONSUMEREWARD_SQL = @"INSERT INTO {0}(CONSUMEGUID,DIGITALCODE,AWARDID,MOBILE,CARDID,GIFTNUM,CARBRAND,FACID,IP,PROVINCENAME,CITYNAME,DEALERNAME,LOGISTICSCODE,DEALERID,PRODUCTNAME,PRODUCTID,OPENID)"
                                                + "VALUES(:CONSUMEGUID,:DIGITALCODE,:AWARDID,:MOBILE,:CARDID,'1',:CARBRAND,:FACID,:IP,:PROVINCENAME,:CITYNAME,:DEALERNAME,:LOGISTICSCODE,:DEALERID,:PRODUCTNAME,:PRODUCTID,:OPENID)";

        public const string GET_CONSUMEREWARD_BY_OPENID_SQL1 = "SELECT xx.*,xx.rowid  FROM T_SGM_CONSUMEREWARD   xx where  xx.FACID=:FACID and xx.OPENID=:OPENID   and  xx.activitytype is  null or xx.activitytype='0'  ";

        // 20161101   修改 
        /// <summary>
        /// 核销信息
        /// </summary>
        public const string GET_CONSUMEREWARD_BY_OPENID_SQL2 = "SELECT xx.*,xx.rowid , yy.name   FROM T_SGM_CONSUMEREWARD   xx  join  t_sgm_wb_basedata_9999 yy on  xx.awardid=yy.codeid and  xx.facid=yy.facid  and  yy.datatypename='LotteryType' where  xx.FACID=:FACID and xx.OPENID=:OPENID AND xx.CARDID=:CARDID";





        private const string FIND_REGISTERLOTTER_SQL = "SELECT SPRO FROM {0} R WHERE R.IP=:IP AND R.FACID=:FACID AND R.FLAG='1'";


        private const string FIND_JDKQ_SQL = " select J.*,J.ROWID from  {0}  J WHERE  J.FACID=:FACID  AND  J.FLAG='1'  AND  J.CODE IS NULL ";

        private const string CHECK_CONSUMEREWARD_CARDID_SQL = "SELECT CARDID FROM {0} L WHERE L.CARDID=:CARDID AND L.FACID=:FACID";

        private const string CHECK_USER_LOTTERYLOG_DAY_SQL = "SELECT COUNT(1) FROM  {0}  A  WHERE A.ACTIVITYID=:ACTIVITYID AND A.FACID=:FACID  AND A.DELETEFLAG='1' AND TO_CHAR(A.CREATEDAE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')";

        private const string CHECK_SHAKE_LOG_BY_CODE_SQL = "SELECT DIGIT FROM {0} L WHERE L.DIGIT=:DIGIT AND L.FACID=:FACID AND L.DELETEFLAG='1'";

        private const string CHECK_SHAKE_LOG_BY_MOBILE_SQL = "SELECT DIGIT FROM {0} L WHERE L.USERID=:USERID AND L.FACID=:FACID AND L.DELETEFLAG='1'";

        private const string CHECK_SHAKE_LOG_BY_MOBILE_SQL2 = "SELECT U.IP FROM {0}  U WHERE U.FACID=:FACID AND U.IP=:USERID  AND U.LOTTERYLEVEL IS NOT NULL AND U.LOTTERYLEVEL <> '0' ";

        /// <summary>
        /// 检测手机号是否参与此次活动
        /// </summary>
        private const string CHECK_SHAKE_LOG_BY_MOBILE_SQL_A = "SELECT G.*,G.ROWID FROM {0} G WHERE G.FACID=:FACID  and  g.userid=:USERID  and  g.activityid=:ACTIVITYID    and  g.deleteflag='1'";




        //R5E二期 查询当前手机号当前月份是否有参与记录 layen 2016年8月31日 12:02:17 添加
        private const string CHECK_SHAKE_LOG_BY_MOBILE_SQL3 = "SELECT U.IP FROM {0}  U WHERE U.FACID=:FACID AND U.IP=:USERID  AND U.LOTTERYLEVEL IS NOT NULL AND to_char(sysdate, 'MM')=to_char(U.VDATE, 'MM') ";

        //R5E二期 邀请好友参与 layen 2016年8月31日 13:34:28 添加
        private const string INSERT_T_SGM_SHAKE_Invite = @"insert into T_SGM_SHAKE_Invite (guid,INVITEFROM, INVITETO, LOTTERYGUID, FACID) values (:GUID,:INVITEFROM,:INVITETO, :LOTTERYGUID, :FACID)";

        /// <summary>
        /// 检测数码是否邀请过好友
        /// </summary>
        private const string QueryCodeIsInvitedFried = "SELECT L.*,L.ROWID FROM T_SGM_SHAKE_Invite  L WHERE L.FACID=:FACID and  l.invitefrom=:INVITEFROM  and  l.lotteryguid=:LOTTERYGUID ";



        private const string CHECK_SHAKE_LOG_BY_OPENID_SQL = "SELECT U.OPENID FROM {0}  U WHERE U.FACID=:FACID AND U.OPENID=:OPENID  AND U.LOTTERYLEVEL IS NOT NULL AND U.LOTTERYLEVEL <> '0' ";


        private const string CHECK_SHAKE_LOG_BY_OPENID_SQL_A = "SELECT U.OPENID FROM {0}  U WHERE U.FACID=:FACID AND U.OPENID=:OPENID  AND  U.ACTIVITYID=:ACTIVITYID  AND U.LOTTERYLEVEL IS NOT NULL";


        private const string CHECK_SHAKE_LOG_BY_OPENID_SQL_B = "SELECT U.OPENID FROM {0}  U WHERE U.FACID=:FACID AND U.F3=:F3  AND  U.ACTIVITYID=:ACTIVITYID  AND U.LOTTERYLEVEL IS NOT NULL";




        private const string CHECK_AUTH_LOG_BY_OPENID_SQL = " SELECT COUNT(1) FROM T_SGM_SHAKE_AUTHWECHAT_LOG A WHERE A.FACTORYID=:FACTORYID AND A.OPENID=:OPENID AND SYSDATE <= (A.CREATEDATE + {0} / 24 / 60)";
        //
        private const string CHECK_SHAKE_LOG_BY_STORE_SQL = "SELECT S.STOREID FROM T_SGM_SHAKE_STORE S WHERE S.STOREID=:STOREID AND S.FLAG='1' AND S.FACID=:FACID";

        private const string CHECK_SHAKE_LOG_BY_MOBILEDAY_SQL2 = "SELECT COUNT(1) FROM T_SGM_SHAKE_REGISTERUSER_9999 U WHERE U.FACID=:FACID AND U.IP=:USERID AND TO_CHAR(U.VDATE,'YYYY-MM-DD')=  TO_CHAR(SYSDATE,'YYYY-MM-DD') ";

        private const string CHECK_STORE_LOTTERY_COUNT_WT = @"SELECT COUNT(1) LOTTERYNUM FROM T_SGM_SHAKE_REGISTERUSER_9999 X LEFT JOIN 
                                                            T_SGM_SHAKE_STORE S 
                                                            ON X.F1=S.STOREID 
                                                            WHERE 
                                                            X.LOTTERYLEVEL <> '0' AND 
                                                            X.LOTTERYLEVEL IS NOT NULL AND 
                                                            X.FACID=:FACID AND
                                                            S.FACID=:FACID AND 
                                                            S.FLAG='1' AND 
                                                            S.ISJOINACTIVITY='1' AND
                                                            S.STOREID=:STOREID ";

        private const string CHECK_SHAKE_LOG_BY_STORE_SQL2 = "SELECT S.STOREID FROM T_SGM_SHAKE_STORE S WHERE S.STOREID=:STOREID AND S.FLAG='1' AND  S.ISJOINACTIVITY='1'  AND S.FACID=:FACID";

        private const string CHECK_SHAKE_LOG_BY_MOBILE_VERIFYCODE = "SELECT S.STOREID FROM T_SGM_SHAKE_STORE S WHERE S.STOREID=:STOREID AND S.FLAG='1' AND  S.ISJOINACTIVITY='1'  AND S.FACID=:FACID";

        private const string GET_MOBILE_VERIFYCODE_SQL = "SELECT COUNT(1) FROM T_SGM_VERIFYCODE V WHERE V.MOBILE=:MOBILE AND V.VERIFCODE=:VERIFCODE AND V.FACID=:FACID AND  V.FLAG='1'";

        private const string GET_MOBILE_VERIFYCODE_SQL_A = "SELECT COUNT(1) FROM T_SGM_VERIFYCODE V WHERE V.MOBILE=:MOBILE AND V.VERIFCODE=:VERIFCODE AND V.FACID=:FACID ";


        private const string UPDATE_STCODE_SQL = "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'";


        private const string UPDATE_MOBILE_VERIFYCODE_SQL = "UPDATE T_SGM_VERIFYCODE V SET V.FLAG='2' WHERE V.MOBILE=:MOBILE AND V.VERIFCODE=:VERIFCODE AND V.FACID=:FACID";

        //private const string CHECK_SHAKE_LOG_BY_OPENIDSTORE_SQL = " SELECT COUNT(1) FROM T_SGM_SHAKE_REGISTERUSER_9999 R WHERE R.FACID=:FACID AND R.OPENID=:OPENID AND R.F1!=:STORE";


        /// <summary>
        ///检测openid是否在门店参与过活动（已中奖）
        /// </summary>
        private const string CHECK_SHAKE_LOG_BY_OPENIDSTORE_SQL = " SELECT COUNT(1) FROM T_SGM_SHAKE_REGISTERUSER_9999 R WHERE R.FACID=:FACID AND R.OPENID=:OPENID AND R.F1!=:STORE  and  r.lotterylevel is not null ";


        // private const string CHECK_SHAKE_USERREGIST_SQL = "SELECT * FROM {0} L WHERE L.OPENID=:OPENID AND L.FACID=:FACID AND L.FLAG='1'";
        // private const string CHECK_SHAKE_USERREGIST_SQL = "SELECT L.*,m.NAME FROM {0} L  left  join  t_sgm_wb_basedata_9999  m  on  l.lotterylevel=m.codeid and  m.facid=:FACID and  m.datatypename='LotteryType'  WHERE L.OPENID=:OPENID AND L.FACID=:FACID AND L.FLAG='1'";


        private const string CHECK_SHAKE_USERREGIST_SQL = "SELECT L.*,m.NAME , (select  M.NAME from  t_sgm_wb_basedata_9999  m  where  m.codeid=l.express_company  and   m.facid=:FACID  and  m.datatypename='Express'  ) as  EXPRESSNAME  FROM {0} L  left  join  t_sgm_wb_basedata_9999  m  on  l.lotterylevel=m.codeid and  m.facid=:FACID and  m.datatypename='LotteryType'  WHERE L.OPENID=:OPENID AND L.FACID=:FACID AND L.FLAG='1'";




        private const string CHECK_SHAKE_LOG_BY_MOBILEANDCODE_SQL = "SELECT DIGIT FROM {0} L WHERE L.DIGIT=:DIGIT AND L.FACID=:FACID AND L.USERID=:MOBILE AND L.DELETEFLAG='1'";

        private const string MODIFY_REGISTER_SQL = "UPDATE {0} Q "
                                                    + " SET CHANGE_WAY = :CHANGE_WAY,"
                                                    + " CHANGE_TYPE    = :CHANGE_TYPE,"
                                                    + " USER_NAME      = :USER_NAME,"
                                                    + " USER_ADDRESS   = :USER_ADDRESS,"
                                                    + " USER_ZIPCODE   = :USER_ZIPCODE,"
                                                    + " USER_TELEPHONE = :USER_TELEPHONE,"
                                                    + " CHANGE_DATE    = :CHANGE_DATE,"
                                                    + " LOTTERYNAME    = :LOTTERYNAME,"
                                                    + " COMPANY        = :COMPANY,"
                                                    + " STATE          = '1' "
                                                    + " WHERE Q.FACID  = :FACID"
                                                    + " AND Q.SPRO = :SPRO";

        public const string Insert_T_MOBILE_FLOW_ONLINE_SEND_SQL = @"INSERT INTO T_MOBILE_FLOW_ONLINE_SEND(GUID,FACID,ORDERID,ORDERTYPE,PACKCODE,MOBILE,PACKNUM,PACKMONEY,SIGN,ACTIVITYID,DIGITCODE) VALUES (:GUID,:FACID,:ORDERID,:ORDERTYPE,:PACKCODE,:MOBILE,:PACKNUM,:PACKMONEY,:SIGN,:ACTIVITYID,:DIGITCODE)";

        public const string Insert_T_MOBILE_ONLINE_SEND_NEW = @"INSERT INTO T_MOBILE_ONLINE_SEND X 
                                                        (X.FACID,X.MOBILE,X.ORDERID,X.NOTES,X.CITYCODE,X.CITYNAME,X.STATECODE,X.STATENAME,X.CARDNUM,X.CARDID,RESULTID,X.CARDNO,X.ORDERIDS) 
                                                        VALUES(:FACID,:MOBILE,:ORDERID,:NOTES,:CITYCODE,:CITYNAME,:STATECODE,:STATENAME,:CARDNUM,:CARDID,:RESULTID,:CARDNO,:ORDERIDS)";


        public const string Insert_T_MOBILE_ONLINE_SEND_NEW_BAK = @"INSERT INTO T_MOBILE_ONLINE_SEND_bak_9999 X 
                                                        (X.FACID,X.MOBILE,X.ORDERID,X.NOTES,X.CITYCODE,X.CITYNAME,X.STATECODE,X.STATENAME,X.CARDNUM,X.CARDID,RESULTID,X.CARDNO,X.ORDERIDS) 
                                                        VALUES(:FACID,:MOBILE,:ORDERID,:NOTES,:CITYCODE,:CITYNAME,:STATECODE,:STATENAME,:CARDNUM,:CARDID,:RESULTID,:CARDNO,:ORDERIDS)";

        public const string Insert_T_MOBILE_ONLINE_SEND = @"INSERT INTO T_MOBILE_ONLINE_SEND X 
                                                        (X.FACID,X.MOBILE,X.ORDERID,X.NOTES,X.CITYCODE,X.CITYNAME,X.STATECODE,X.STATENAME,X.CARDNUM,X.CARDID,RESULTID) 
                                                        VALUES(:FACID,:MOBILE,:ORDERID,:NOTES,:CITYCODE,:CITYNAME,:STATECODE,:STATENAME,:CARDNUM,:CARDID,:RESULTID)";



        public const string UpdateJDKQ_SQL = @" UPDATE T_SGM_SHAKE_JDECODE J SET J.CODE=:CODE, J.FLAG='2',J.LOTTREYLEVEL=:LOTTREYLEVEL WHERE J.ECODE=:ECODE AND J.FACID=:FACID AND J.FLAG='1'";


        /// <summary>
        /// 获取basedatavalue
        /// </summary>
        public const string GetBaseDataValue_sql = "  SELECT  x.codeid  FROM  t_sgm_wb_basedata_9999 X WHERE X.FACID =:FACID  and   x.Datatypename=:DATATYPENAME  ";


        public const string GetBaseDataValue_sql2 = "  SELECT *  FROM  t_sgm_wb_basedata_9999 X WHERE X.FACID =:FACID   and   x.Datatypename=:DATATYPENAME  AND  X.CODEID=:CODEID ";


        public const string GetLotteryTotal_SQL = @"SELECT sum(x.cardnum) FROM T_MOBILE_ONLINE_SEND x where x.facid=:FACID order by x.createdate desc ";

        public const string GetLotteryCurDay_SQL = @"SELECT sum(x.cardnum) FROM T_MOBILE_ONLINE_SEND x where x.facid=:FACID and x.resultid='1' and x.state='1' 
                                                    AND X.CREATEDATE > TO_DATE('{0}','YYYY-MM-DD hh24:mi:ss')
                                                    AND X.CREATEDATE < TO_DATE('{1}','YYYY-MM-DD hh24:mi:ss')  ";


        public const string GetRedTotal_SQL = @"SELECT sum(x.total_amount) FROM T_SGM_SHAKE_SENDRPLOG X where x.facid=:FACID ";

        public const string GetRedCurDay_SQL = @"SELECT sum(x.total_amount) FROM T_SGM_SHAKE_SENDRPLOG X where x.facid=:FACID and x.state='001' 
                                                AND x.createtime > TO_DATE('{0}','YYYY-MM-DD hh24:mi:ss')
                                                AND X.createtime < TO_DATE('{1}','YYYY-MM-DD hh24:mi:ss') ";


        public const string SQL_ADDInsurance = "INSERT INTO t_sgm_shake_insurance (GUID,FACID,DIGIT,LID,MOBILE,ORDERID,SERIESID,SERIESNAME,OPENID,PRODUCTID,PRODUCTNAME,PH_STATUS)VALUES(:GUID,:FACID,:DIGIT,:LID,:MOBILE,:ORDERID,:SERIESID,:SERIESNAME,:OPENID,:PRODUCTID,:PRODUCTNAME,'投保审核中...')";

        //德农专用
        public const string SQL_UPDATE_REGISTER_DL = "UPDATE {0} X SET X.STATE=:STATE WHERE X.FACID=:FACID AND X.OPENID=:OPENID ";


        //司能

        /// <summary>
        /// 添加卡券信息
        /// </summary>
        public const string SQL_AddCardInfo = "insert into T_SGM_CONSUMEREWARD (CONSUMEGUID,ACTIVITYTYPE,CARDID,OPENID,ISCONSUME,FACID,F1,F2,F3) VALUES(:CONSUMEGUID,:ACTIVITYTYPE,:CARDID,:OPENID,:ISCONSUME,:FACID,:F1,:F2,:F3)";

        /// <summary>
        /// 添加用户信息
        /// </summary>
        public const string SQL_AddUserRegistInfo = " insert  into {0} (USERGUID,USERTYPE,OPENID,WXSEX,WXNICKNAME,WXCOUNTRY,WXPROVINCE,WXCITY,WXIMG,PROVINCE,CITY,F1,F2,F3,F4,FACID) VALUES(:USERGUID,:USERTYPE,:OPENID,:WXSEX,:WXNICKNAME,:WXCOUNTRY,:WXPROVINCE,:WXCITY,:WXIMG,:PROVINCE,:CITY,:F1,:F2,:F3,:F4,:FACID)";




        /// <summary>
        /// 添加预约信息
        /// </summary>
        /// <returns></returns>
        public const string SQL_AddReserveInfo = "INSERT INTO T_SGM_SHAKE_RESERVE(RESERVEGUID,MOBILE,CHANNEL,FACID,USERNAME,STOREID,OPENID,F1) VALUES (:RESERVEGUID,:MOBILE,:CHANNEL,:FACID,:USERNAME,:STOREID,:OPENID,:F1)";

        /// <summary>
        /// 更新用户信息
        /// </summary>
        public const string SQL_UpdateUserInfo = " update  {0} xx set xx.username=:USERNAME,xx.carbrand=:CARBRAND,xx.carage=:CARAGE ,xx.productname=:PRODUCTNAME , xx.COMPLETEFLAG='1' where xx.facid=:FACID and  xx.userguid=:USERGUID and  xx.deleteflag='1' ";


        /// <summary>
        /// 更新用户信息2
        /// </summary>
        public const string SQL_UpdateUserInfo2 = " update  {0} xx set xx.username=:USERNAME,xx.carbrand=:CARBRAND,xx.carage=:CARAGE ,xx.productname=:PRODUCTNAME where xx.facid=:FACID and  xx.userguid=:USERGUID and  xx.deleteflag='1' ";

        /// <summary>
        /// 添加用户里程New
        /// </summary>
        public const string SQL_AddUserMileNew = "INSERT  INTO  T_SGM_SHAKE_USERMILEAGE (GUID,USERGUID,MILEAGEREWARD,FACID,SPRO,LOTTERYGUID,REWARDMILE,INVITEGUID)VALUES(:GUID,:USERGUID,:MILEAGEREWARD,:FACID,:SPRO,:LOTTERYGUID,:REWARDMILE,:INVITEGUID)";


        /// <summary>
        /// 获取用户基础信息
        /// </summary>
        public const string SQL_GetUserBaseInfo = " select   xx.*, xx.rowid  from  {0} xx where xx.facid=:FACID  and xx.userid=:USERID  and  xx.deleteflag='1' ";

        /// <summary>
        /// 修改用户基础信息
        /// </summary>
        public const string SQL_UpdateUserBaseInfo = " update   {0}  xx  set xx.f5=:F5 ,xx.pointtotal=pointtotal+:POINTTOTAL  WHERE  XX.FACID=:FACID AND  XX.USERID=:USERID AND XX.DELETEFLAG='1' ";





        /// <summary>
        /// 校验被邀请人是否中奖
        /// </summary>
        /// <returns></returns>
        public const string SQL_GetInviteInfoByInviteto = "SELECT l.*,l.rowid  FROM T_SGM_SHAKE_Invite L WHERE L.FACID =:FACID and l.inviteto =:INVITETO and   TO_CHAR(l.createdate,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM') order by l.createdate asc ";

        /// <summary>
        /// 根据groupid 查询是否有被邀请人中奖
        /// </summary>
        public const string SQL_GetInviteInfoByGroupid = "SELECT l.*, l.rowid  FROM T_SGM_SHAKE_Invite L WHERE L.FACID =:FACID  and  l.groupid=:GROUPID   and l.invitelotteryflag !='0' order  by  l.createdate asc ";


        /// <summary>
        /// 获取最先邀请同一好友的邀请人信息
        /// </summary>
        public const string SQL_GetInviteInfoByInviteto2 = "   select m.invitefrom,m.guid,n.userguid,m.invitetype  from   (SELECT l.*,l.rowid  FROM T_SGM_SHAKE_Invite L WHERE L.FACID =:FACID and l.inviteto =:INVITETO  and   TO_CHAR(l.createdate,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')   order by l.createdate asc)  m left join {0} n on m.facid = n.facid and m.invitefrom = n.userid   where   m.facid=:FACID and  n.facid=:FACID and  rownum = 1";

        /// <summary>
        /// 更新邀请表中被邀请人中奖信息
        /// </summary>
        public const string SQL_UpdateInviteByGuid = "update  T_SGM_SHAKE_Invite xx set xx.invitelotteryflag='1',xx.invitelotteryguid=:INVITELOTTERYGUID,xx.invitelotterydate=sysdate where xx.facid=:FACID AND  XX.GUID=:GUID ";


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
        public string GetTable(string table, string facid)
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

        public string GetTable(string facid)
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

            return TableName_Signin + newfactoryid;
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
                    //ParameterCache.PushCache("GetNewFactoryTalbeParam", param);
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

        #region 3) 预计消息提醒
        /// <summary>
        /// 预计消息提醒
        /// </summary>
        /// <param name="level">提示等级</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="content">提示内容</param>
        /// <returns>预计消息提醒</returns>
        public bool sendPrewarning(int level, string facid, string content)
        {
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("sendPrewarningParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":REMINDLEVEL", OracleType.VarChar, 10);
                    //将参数加入缓存
                    // ParameterCache.PushCache("sendPrewarningParam", param);
                }
                param[0].Value = facid;
                param[1].Value = level;

                DataBase dataBase = new DataBase();
                string sql = string.Format(SEND_SMS_SQL, content);
                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param);
                return (row > 0) ? true : false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 4) 更新奖池中奖数量
        /// <summary>
        /// 更新奖池中奖数量
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="awardscode">活动编码</param>
        /// <returns></returns>
        public bool UpdateLotteryNum(string facid, string poolid, string awardscode)
        {
            DataBase dataBase = new DataBase();
            try
            {
                OracleParameter[] param = null;
                param = UpdateLotteryNumParam(facid, poolid, awardscode);

                string table = GetTable(TableName_Pool, facid);
                string sql = string.Format(UPDATE_SHAKE_POOL_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param);
                return (row > 0) ? true : false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 5) 参与活动次数加
        /// <summary>
        /// 参与活动次数加
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool UpdateAwardNum(string facid, string activityid, string poolid)
        {
            DataBase dataBase = new DataBase();
            try
            {
                OracleParameter[] param = null;
                param = UpdateAwardNumParam(facid, activityid, poolid);

                string table = GetTable(TableName_Rule, facid);
                string sql = string.Format(UPDATE_SHAKE_RULE_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param);
                return (row > 0) ? true : false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 6) 序列化参数

        #region 获取参与活动基数参数
        /// <summary>
        /// 获取参与活动基数参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public OracleParameter[] UpdateAwardNumParam(string facid, string activityid, string poolid)
        {
            OracleParameter[] param = null;
            try
            {
                #region 序列化参数
                param = (OracleParameter[])ParameterCache.GetParams("UpdateAwardNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":ACTIVITYID", OracleType.Number);
                    param[1] = new OracleParameter(":POOLID", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    //将参数加入缓存
                    //ParameterCache.PushCache("UpdateAwardNumParam", param);
                }
                param[0].Value = activityid;
                param[1].Value = poolid;
                param[2].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)UpdateLotteryNumParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 获取预警信息发送参数
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

        #region 获取中奖人数参数

        /// <summary>
        /// 获取中奖人数参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="awardscode">奖项编号</param>
        /// <returns></returns>
        public OracleParameter[] UpdateLotteryNumParam(string facid, string poolid, string awardscode)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("UpdateLotteryNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":POOLID", OracleType.Number);
                    param[1] = new OracleParameter(":AWARDSCODE", OracleType.VarChar, 10);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("UpdateLotteryNumParam", param);
                }
                param[0].Value = poolid;
                param[1].Value = awardscode;
                param[2].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)UpdateLotteryNumParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion


        #region 修改注册状态 德农专用

        /// <summary>
        /// 修改注册状态 德农专用
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public OracleParameter[] UpdateRegister(string facid, string openid, string state)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("UpdateLotteryNumParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":OPENID", OracleType.VarChar);
                    param[1] = new OracleParameter(":STATE", OracleType.VarChar);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("UpdateLotteryNumParam", param);
                }
                param[0].Value = openid;
                param[1].Value = state;
                param[2].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)UpdateLotteryNumParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region  更新卡券信息
        /// <summary>
        /// 更新卡券信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ecode">京东卡号</param>
        /// <param name="awardscode">中奖编号（奖项等级）</param>
        /// <param name="digitcode">中奖数码</param>
        /// <returns></returns>
        public OracleParameter[] UpdateLotteryJDKQParam(string facid, string ecode, string awardscode, string digitcode)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("UpdateLotteryJDKQParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[4];

                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":ECODE", OracleType.VarChar, 64);
                    param[2] = new OracleParameter(":LOTTREYLEVEL", OracleType.VarChar, 2);
                    param[3] = new OracleParameter(":CODE", OracleType.VarChar, 32);

                    //将参数加入缓存
                    ParameterCache.PushCache("UpdateLotteryJDKQParam", param);
                }
                param[0].Value = facid;
                param[1].Value = ecode;
                param[2].Value = awardscode;
                param[3].Value = digitcode;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("   更新卡券信息)UpdateLotteryJDKQParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;

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
        private OracleParameter[] GetLotteryLogParam(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result)
        {

            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("AddLotteryLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[13];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 100);
                    param[2] = new OracleParameter(":USERTYPE", OracleType.VarChar, 1);
                    param[3] = new OracleParameter(":UPCONTENT", OracleType.VarChar, 255);
                    param[4] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[5] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[6] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[7] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[8] = new OracleParameter(":PROTYPE", OracleType.VarChar, 20);
                    param[9] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 1000);
                    param[10] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[11] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                    param[12] = new OracleParameter(":AREA", OracleType.VarChar, 50);

                    //将参数加入缓存
                    ParameterCache.PushCache("AddLotteryLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = ip;
                param[2].Value = channel;
                param[3].Value = digitCode;
                param[4].Value = digitCode;
                param[5].Value = activityId;
                param[6].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[7].Value = awardsno;
                param[8].Value = proid;
                param[9].Value = result;
                param[10].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[11].Value = facid;
                param[12].Value = area;
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

        #region 获取核销组织参数
        /// <summary>
        /// 新增核销记录
        /// </summary>
        /// <param name="guid4">唯一标识</param>
        /// <param name="digitCode">产品编码</param>
        /// <param name="awardsno">奖品编号</param>       
        /// <param name="ip">手机号码</param>
        /// <param name="cardnum">卡券号码</param>
        /// <param name="cardbrand">车型</param>
        /// <param name="CarBrand">汽车品牌</param>       
        /// <param name="facid">厂家编号</param>
        /// <param name="provincename">省份</param>
        /// <param name="cityname">城市</param>
        /// <param name="dealername">经销商名称</param>
        /// <param name="logisticscode">物流码</param>
        /// <param name="dealerid">经销商id</param>
        /// <param name="productname">产品名称</param>
        /// <param name="productid">产品id</param>
        /// <param name="openid">微信用户关注id</param>
        /// <returns></returns>
        private OracleParameter[] GetConsumerEwardParam(string guid4, string digitCode, string awardsno, string ip, string cardnum, string cardbrand, string facid, string cip, string provincename, string cityname, string dealername, string logisticscode, string dealerid, string productname, string productid, string openid)
        {

            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("AddConsumerEwardParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[16];
                    param[0] = new OracleParameter(":CONSUMEGUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":DIGITALCODE", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":AWARDID", OracleType.VarChar, 10);
                    param[3] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    param[4] = new OracleParameter(":CARDID", OracleType.VarChar, 10);
                    param[5] = new OracleParameter(":CARBRAND", OracleType.VarChar, 50);
                    param[6] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[7] = new OracleParameter(":IP", OracleType.VarChar, 32);
                    param[8] = new OracleParameter(":PROVINCENAME", OracleType.VarChar, 50);
                    param[9] = new OracleParameter(":CITYNAME", OracleType.VarChar, 50);
                    param[10] = new OracleParameter(":DEALERNAME", OracleType.VarChar, 64);
                    param[11] = new OracleParameter(":LOGISTICSCODE", OracleType.VarChar, 64);
                    param[12] = new OracleParameter(":DEALERID", OracleType.VarChar, 64);
                    param[13] = new OracleParameter(":PRODUCTNAME", OracleType.VarChar, 128);
                    param[14] = new OracleParameter(":PRODUCTID", OracleType.VarChar, 64);
                    param[15] = new OracleParameter(":OPENID", OracleType.VarChar, 64);




                    //将参数加入缓存
                    ParameterCache.PushCache("AddConsumerEwardParam", param);
                }
                param[0].Value = guid4;
                param[1].Value = digitCode;
                param[2].Value = awardsno;
                param[3].Value = ip;
                param[4].Value = cardnum;
                param[5].Value = cardbrand;
                param[6].Value = facid;
                param[7].Value = cip;
                param[8].Value = provincename;
                param[9].Value = cityname;
                param[10].Value = dealername;
                param[11].Value = logisticscode;
                param[12].Value = dealerid;
                param[13].Value = productname;
                param[14].Value = productid;
                param[15].Value = openid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("获取核销组织参数----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion


        #region 卡券添加信息组织参数
        /// <summary>
        /// 卡券添加信息组织参数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="cardid"></param>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="usermobile"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        private OracleParameter[] GetCardAddParam(string facid, string cardid, string userid, string username, string usermobile, Hashtable userHash)
        {

            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetCardAddParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[9];
                    param[0] = new OracleParameter(":CONSUMEGUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":ACTIVITYTYPE", OracleType.VarChar, 1);
                    param[2] = new OracleParameter(":CARDID", OracleType.VarChar, 30);
                    param[3] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    param[4] = new OracleParameter(":ISCONSUME", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[6] = new OracleParameter(":F1", OracleType.VarChar, 32);
                    param[7] = new OracleParameter(":F2", OracleType.VarChar, 32);
                    param[8] = new OracleParameter(":F3", OracleType.VarChar, 32);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetCardAddParam", param);
                }

                string openid = string.Empty;
                if (userHash.ContainsKey("OPENID"))
                {
                    openid = userHash["OPENID"].ToString();
                }
                param[0].Value = Guid.NewGuid().ToString().Replace("-", ""); ;
                param[1].Value = "1";//员工分享计划活动
                param[2].Value = cardid;
                param[3].Value = openid;
                param[4].Value = '0';//默认为0 ：未核销
                param[5].Value = facid;
                param[6].Value = userid;
                param[7].Value = username;
                param[8].Value = usermobile;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs 卡券添加信息组织参数GetCardAddParam()----" + ex.Message + "--" + ex.StackTrace + "---" + ex.TargetSite + "---" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion



        #region 用户补全资料 组织参数
        /// <summary>
        /// 用户补全资料组织参数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userguid"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        private OracleParameter[] GetUserUpdateParam(string facid, string userguid, Hashtable userHash)
        {
            OracleParameter[] param = null;
            try
            {

                #region 基础数据获取
                string USERNAME = string.Empty;
                string CARBRAND = string.Empty;
                string CARAGE = string.Empty;
                string PRODUCTNAME = string.Empty;
                if (userHash.Contains("USERNAME"))
                {
                    USERNAME = userHash["USERNAME"].ToString();
                }
                if (userHash.Contains("CARBRAND"))
                {
                    CARBRAND = (string)userHash["CARBRAND"].ToString();
                }
                if (userHash.Contains("CARAGE"))
                {
                    CARAGE = (string)userHash["CARAGE"].ToString();
                }
                if (userHash.Contains("PRODUCTNAME"))
                {
                    PRODUCTNAME = (string)userHash["PRODUCTNAME"].ToString();
                }
                #endregion

                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetUserUpdateParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[6];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[1] = new OracleParameter(":USERGUID", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":USERNAME", OracleType.VarChar, 40);
                    param[3] = new OracleParameter(":CARBRAND", OracleType.VarChar, 100);
                    param[4] = new OracleParameter(":CARAGE", OracleType.VarChar, 10);
                    param[5] = new OracleParameter(":PRODUCTNAME", OracleType.VarChar, 50);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetUserUpdateParam", param);
                }
                param[0].Value = facid;
                param[1].Value = userguid;
                param[2].Value = USERNAME;
                param[3].Value = CARBRAND;
                param[4].Value = CARAGE;
                param[5].Value = PRODUCTNAME;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs 用户补全资料组织参数GetUserUpdateParam()----" + ex.Message + "--" + ex.StackTrace + "---" + ex.TargetSite + "---" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 添加里程信息表组织参数信息NEW
        /// <summary>
        /// 添加里程信息表组织参数信息NEW
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userguid"></param>
        /// <param name="miletype"></param>
        /// <param name="code"></param>
        /// <param name="lid"></param>
        /// <param name="mile"></param>
        ///  <param name="inviteguid"></param>
        /// <returns></returns>
        public OracleParameter[] AddUserMileNewParam(string facid, string userguid, string miletype, string code, string lid, int mile, string inviteguid)
        {

            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("AddUserMileNewParam");
                if (param == null)
                {
                    param = new OracleParameter[8];
                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERGUID", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":MILEAGEREWARD", OracleType.VarChar, 64);
                    param[3] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[4] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                    param[5] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar, 32);
                    param[6] = new OracleParameter(":REWARDMILE", OracleType.Number);
                    param[7] = new OracleParameter(":INVITEGUID", OracleType.VarChar, 32);

                    //将参数加入缓存
                    ParameterCache.PushCache("AddUserMileNewParam", param);
                }
                param[0].Value = Guid.NewGuid().ToString().Replace("-", "");
                param[1].Value = userguid;
                param[2].Value = miletype;
                param[3].Value = facid;
                param[4].Value = code;
                param[5].Value = lid;
                param[6].Value = mile;
                param[7].Value = inviteguid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("17)AddUserMileNewParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion


        #region 更新邀请表中被邀请人中奖信息组织参数
        /// <summary>
        /// 更新邀请表中被邀请人中奖信息组织参数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="guid"></param>
        /// <param name="invitelotteryguid"></param>
        /// <returns></returns>
        private OracleParameter[] GetUpdateInviteByGuidParam(string facid, string guid, string invitelotteryguid)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetUpdateInviteByGuidParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":INVITELOTTERYGUID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetUpdateInviteByGuidParam", param);
                }
                param[0].Value = facid;
                param[1].Value = guid;
                param[2].Value = invitelotteryguid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs 更新邀请表中被邀请人中奖信息组织参数GetUpdateInviteByGuidParam()----" + ex.Message + "--" + ex.StackTrace + "---" + ex.TargetSite + "---" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        #endregion


        #region 用户注册表信息添加组织参数
        /// <summary>
        ///用户注册表信息添加组织参数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="cardid"></param>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="usermobile"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        private OracleParameter[] GetRegistAddParam(string facid, Hashtable userHash)
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
                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetRegistAddParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[16];
                    param[0] = new OracleParameter(":USERGUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERTYPE", OracleType.VarChar, 1);
                    param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    param[3] = new OracleParameter(":WXSEX", OracleType.VarChar, 1);
                    param[4] = new OracleParameter(":WXNICKNAME", OracleType.VarChar, 200);
                    param[5] = new OracleParameter(":WXCOUNTRY", OracleType.VarChar, 20);
                    param[6] = new OracleParameter(":WXPROVINCE", OracleType.VarChar, 20);
                    param[7] = new OracleParameter(":WXCITY", OracleType.VarChar, 20);
                    param[8] = new OracleParameter(":WXIMG", OracleType.VarChar, 200);
                    param[9] = new OracleParameter(":PROVINCE", OracleType.VarChar, 10);
                    param[10] = new OracleParameter(":CITY", OracleType.VarChar, 10);
                    param[11] = new OracleParameter(":F1", OracleType.VarChar, 50);
                    param[12] = new OracleParameter(":F2", OracleType.VarChar, 50);
                    param[13] = new OracleParameter(":F3", OracleType.VarChar, 50);
                    param[14] = new OracleParameter(":F4", OracleType.VarChar, 50);
                    param[15] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetRegistAddParam", param);
                }



                param[0].Value = Guid.NewGuid().ToString().Replace("-", ""); ;
                param[1].Value = "1";
                param[2].Value = OPENID;
                param[3].Value = WXSEX;
                param[4].Value = WXNICKNAME;
                param[5].Value = WXCOUNTRY;
                param[6].Value = WXPROVINCE;
                param[7].Value = WXCITY;
                param[8].Value = WXIMG;
                param[9].Value = PROVINCE;
                param[10].Value = CITY;
                param[11].Value = PHONESYSTEM;
                param[12].Value = BROWSERTYPE;
                param[13].Value = GPS;
                param[14].Value = GPSTIME;
                param[15].Value = facid;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs 用户注册表信息添加组织参数GetRegistAddParam()----" + ex.Message + "--" + ex.StackTrace + "---" + ex.TargetSite + "---" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion


        #region 获取投保信息-组织参数
        /// <summary>
        /// 获取投保信息-组织参数
        /// </summary>
        /// <param name="guid4">guid</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="lid">中奖规定</param>
        /// <param name="mobile">手机号</param>
        /// <param name="orderid">订单ID</param>
        /// <param name="seriesid">产品系列ID</param>
        /// <param name="seriesname">产品系列名称</param>
        /// <param name="openid">微信粉丝号</param>
        /// <param name="productid">产品ID</param>
        /// <param name="productname">产品名称</param>
        /// <returns></returns>
        private OracleParameter[] GetInsuranceParam(string guid4, string facid, string digitCode, string lid, string mobile, string orderid, string seriesid, string seriesname, string openid, string productid, string productname)
        {

            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("GetInsuranceParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[11];
                    param[0] = new OracleParameter(":GUID", OracleType.NVarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.NVarChar, 10);
                    param[2] = new OracleParameter(":DIGIT", OracleType.NVarChar, 16);
                    param[3] = new OracleParameter(":LID", OracleType.NVarChar, 32);
                    param[4] = new OracleParameter(":MOBILE", OracleType.NVarChar, 11);
                    param[5] = new OracleParameter(":ORDERID", OracleType.NVarChar, 32);
                    param[6] = new OracleParameter(":SERIESID", OracleType.NVarChar, 10);
                    param[7] = new OracleParameter(":SERIESNAME", OracleType.NVarChar, 20);
                    param[8] = new OracleParameter(":OPENID", OracleType.NVarChar, 32);
                    param[9] = new OracleParameter(":PRODUCTID", OracleType.NVarChar, 20);
                    param[10] = new OracleParameter(":PRODUCTNAME", OracleType.NVarChar, 100);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetInsuranceParam", param);
                }
                param[0].Value = guid4;
                param[1].Value = facid;
                param[2].Value = digitCode;
                param[3].Value = lid;
                param[4].Value = mobile;
                param[5].Value = orderid;
                param[6].Value = seriesid;
                param[7].Value = seriesname;
                param[8].Value = openid;
                param[9].Value = productid;
                param[10].Value = productname;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("获取投保信息组织参数----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        #endregion


        #region 获取真码抽奖日志记录组织参数
        /// <summary>
        /// 获取真码抽奖日志记录组织参数
        /// </summary>
        /// <param name="guid">guid</param>
        /// <param name="ip">IP</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="newcode">二次加密的数码</param>
        /// <returns></returns>
        private OracleParameter[] GetLotteryParLogParam(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode)
        {

            OracleParameter[] param = null;
            try
            {
                #region 组织参数
                param = (OracleParameter[])ParameterCache.GetParams("GetLotteryParLogParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[15];

                    //:GUID,:USERID,:USERTYPE,:UPCONTENT,:DIGIT,:ACTIVITYID,:AREA,:JOINDATE,:AWARDSNO,:FACID,:PROID,:DOWNCONTENT,:CREATEDAE,:AGAINCODE,:POOLID

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 100);
                    param[2] = new OracleParameter(":USERTYPE", OracleType.VarChar, 1);
                    param[3] = new OracleParameter(":UPCONTENT", OracleType.VarChar, 255);
                    param[4] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[5] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[6] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[7] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[8] = new OracleParameter(":PROID", OracleType.VarChar, 20);
                    param[9] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 1000);
                    param[10] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[11] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                    param[12] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[13] = new OracleParameter(":AGAINCODE", OracleType.VarChar, 32);
                    param[14] = new OracleParameter(":POOLID", OracleType.VarChar, 20);

                    //将参数加入缓存
                    ParameterCache.PushCache("AddLotteryParLogParam", param);
                }
                param[0].Value = guid;
                param[1].Value = ip;
                param[2].Value = channel;
                param[3].Value = digitCode;
                param[4].Value = digitCode;
                param[5].Value = activityId;
                param[6].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[7].Value = awardsno;
                param[8].Value = proid;
                param[9].Value = result;
                param[10].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[11].Value = facid;
                param[12].Value = area;
                param[13].Value = newcode;
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

        #region 获取抽奖日志记录组织参数
        /// <summary>
        /// 获取抽奖日志记录组织参数
        /// </summary>
        /// <param name="guid">guid</param>
        /// <param name="ip">ip</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <param name="poolid">奖池编号</param>
        /// <param name="newcode">二次加密的数码</param>
        /// <returns></returns>
        private OracleParameter[] GetNewLotteryParam(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode)
        {

            OracleParameter[] param = null;
            try
            {
                #region 组织参数
                param = (OracleParameter[])ParameterCache.GetParams("GetNewLotteryParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[15];

                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 100);
                    param[2] = new OracleParameter(":USERTYPE", OracleType.VarChar, 1);
                    param[3] = new OracleParameter(":UPCONTENT", OracleType.VarChar, 255);
                    param[4] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    param[5] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);
                    param[6] = new OracleParameter(":AREA", OracleType.VarChar, 50);
                    param[7] = new OracleParameter(":JOINDATE", OracleType.DateTime);
                    param[8] = new OracleParameter(":AWARDSNO", OracleType.Number);
                    param[9] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                    param[10] = new OracleParameter(":PROTYPE", OracleType.VarChar, 20);
                    param[11] = new OracleParameter(":DOWNCONTENT", OracleType.VarChar, 1000);
                    param[12] = new OracleParameter(":CREATEDAE", OracleType.DateTime);
                    param[13] = new OracleParameter(":AGAINCODE", OracleType.VarChar, 32);
                    param[14] = new OracleParameter(":POOLID", OracleType.VarChar, 20);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetNewLotteryParam", param);
                }
                param[0].Value = guid;
                param[1].Value = ip;
                param[2].Value = channel;
                param[3].Value = digitCode;
                param[4].Value = digitCode;
                param[5].Value = activityId;
                param[6].Value = area;
                param[7].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[8].Value = awardsno;
                param[9].Value = facid;
                param[10].Value = proid;
                param[11].Value = result;
                param[12].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[13].Value = newcode;
                param[14].Value = poolid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)组织中奖日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 获取注册数据参数
        /// <summary>
        /// 组织注册信息
        /// </summary>
        /// <param name="userinfo">注册信息实体</param>
        /// <returns></returns>
        private OracleParameter[] GetSiginUserParam(RegisterUser userinfo)
        {
            OracleParameter[] param = null;
            try
            {
                #region 组织参数
                param = (OracleParameter[])ParameterCache.GetParams("GetSiginUserParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[22];

                    param[0] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    param[2] = new OracleParameter(":IP", OracleType.VarChar, 30);
                    param[3] = new OracleParameter(":VDATE", OracleType.DateTime);
                    param[4] = new OracleParameter(":LOTTERYLEVEL", OracleType.VarChar, 1);
                    param[5] = new OracleParameter(":CHANGE_WAY", OracleType.VarChar, 1);
                    param[6] = new OracleParameter(":CHANGE_TYPE", OracleType.VarChar, 1);
                    param[7] = new OracleParameter(":USER_NAME", OracleType.VarChar, 30);
                    param[8] = new OracleParameter(":USER_ADDRESS", OracleType.VarChar, 100);
                    param[9] = new OracleParameter(":USER_ZIPCODE", OracleType.VarChar, 6);
                    param[10] = new OracleParameter(":USER_TELEPHONE", OracleType.VarChar, 30);
                    param[11] = new OracleParameter(":CHANGE_DATE", OracleType.DateTime);
                    param[12] = new OracleParameter(":POST_DATE", OracleType.DateTime);
                    param[13] = new OracleParameter(":STATE", OracleType.VarChar, 1);
                    param[14] = new OracleParameter(":CLOSEING_DATE", OracleType.DateTime);
                    param[15] = new OracleParameter(":MEMO", OracleType.VarChar, 100);
                    param[16] = new OracleParameter(":FLAG", OracleType.VarChar, 1);

                    param[17] = new OracleParameter(":LOTTERYNAME", OracleType.VarChar, 2);
                    param[18] = new OracleParameter(":COMPANY", OracleType.VarChar, 50);
                    param[19] = new OracleParameter(":NEWCODE", OracleType.VarChar, 16);
                    param[20] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[21] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetSiginUserParam", param);
                }
                param[0].Value = userinfo.SPRO;
                param[1].Value = userinfo.CATEGORY;
                param[2].Value = userinfo.IP;
                param[3].Value = userinfo.VDATE;
                param[4].Value = userinfo.LOTTERYLEVEL;
                param[5].Value = userinfo.CHANGE_WAY;
                param[6].Value = userinfo.CHANGE_TYPE;
                param[7].Value = userinfo.USER_NAME;
                param[8].Value = userinfo.USER_ADDRESS;
                param[9].Value = userinfo.USER_ZIPCODE;
                param[10].Value = userinfo.USER_TELEPHONE;
                param[11].Value = userinfo.CHANGE_DATE;
                param[12].Value = userinfo.POST_DATE;
                param[13].Value = userinfo.STATE;
                param[14].Value = userinfo.CLOSEING_DATE;

                param[15].Value = userinfo.MEMO;
                param[16].Value = userinfo.FLAG;
                param[17].Value = userinfo.Lotteryname;
                param[18].Value = userinfo.Company;
                param[19].Value = userinfo.Newcoed;
                param[20].Value = userinfo.Facid;
                param[21].Value = Guid.NewGuid().ToString().Replace("-", "");
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)组织注册信息----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        #region 获取数码预注册参数
        /// <summary>
        /// 获取数码预注册参数
        /// </summary>
        /// <param name="digitcode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="ip">IP</param>
        /// <param name="lotterylevel">中奖级别</param>
        /// <param name="newcode">二次加密的数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        private OracleParameter[] GetRegisterUserParam(string guid, string digitcode, string channel, string ip, string lotterylevel, string newcode, string facid)
        {
            OracleParameter[] param = null;
            try
            {
                #region 组织参数
                param = (OracleParameter[])ParameterCache.GetParams("GetRegisterUserParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[8];

                    param[0] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    param[2] = new OracleParameter(":IP", OracleType.VarChar, 30);
                    param[3] = new OracleParameter(":LOTTERYLEVEL", OracleType.VarChar, 1);
                    param[4] = new OracleParameter(":NEWCODE", OracleType.VarChar, 16);
                    param[5] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[6] = new OracleParameter(":VDATE", OracleType.DateTime);
                    param[7] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetRegisterUserParam", param);
                }
                param[0].Value = digitcode;
                param[1].Value = channel;
                param[2].Value = ip;
                param[3].Value = lotterylevel;
                param[4].Value = newcode;
                param[5].Value = facid;
                param[6].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[7].Value = guid;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)数码预注册----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion

        /// <summary>
        /// 组织注册信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        private OracleParameter[] GetModifyRegisterParam(RegisterUser userinfo)
        {
            OracleParameter[] param = null;
            try
            {
                #region 组织参数
                param = (OracleParameter[])ParameterCache.GetParams("GetModifyRegisterParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[11];

                    param[0] = new OracleParameter(":CHANGE_WAY", OracleType.VarChar, 1);
                    param[1] = new OracleParameter(":CHANGE_TYPE", OracleType.VarChar, 1);
                    param[2] = new OracleParameter(":USER_NAME", OracleType.VarChar, 30);
                    param[3] = new OracleParameter(":USER_ADDRESS", OracleType.VarChar, 100);
                    param[4] = new OracleParameter(":USER_ZIPCODE", OracleType.VarChar, 6);
                    param[5] = new OracleParameter(":USER_TELEPHONE", OracleType.VarChar, 30);
                    param[6] = new OracleParameter(":CHANGE_DATE", OracleType.DateTime);
                    param[7] = new OracleParameter(":LOTTERYNAME", OracleType.VarChar, 2);
                    param[8] = new OracleParameter(":COMPANY", OracleType.VarChar, 50);
                    param[9] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                    param[10] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetModifyRegisterParam", param);
                }

                param[0].Value = userinfo.CHANGE_WAY;
                param[1].Value = userinfo.CHANGE_TYPE;
                param[2].Value = userinfo.USER_NAME;
                param[3].Value = userinfo.USER_ADDRESS;
                param[4].Value = userinfo.USER_ZIPCODE;
                param[5].Value = userinfo.USER_TELEPHONE;
                param[6].Value = userinfo.CHANGE_DATE;

                param[7].Value = userinfo.Lotteryname;
                param[8].Value = userinfo.Company;
                param[9].Value = userinfo.SPRO;
                param[10].Value = userinfo.Facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)组织注册信息----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }


        #region 修改特殊抽奖数码数据

        /// <summary>
        /// 修改特殊抽奖数码数据
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="code">数码</param>
        /// <param name="mobile">手机号码</param>
        /// <returns></returns>
        public OracleParameter[] ModifySpCodeParam(string facid, string code, string mobile)
        {
            OracleParameter[] param = null;
            try
            {
                #region

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":MOBILE", OracleType.VarChar);
                param[1] = new OracleParameter(":DIGIT", OracleType.VarChar);
                param[2] = new OracleParameter(":FACID", OracleType.VarChar);

                param[0].Value = mobile;
                param[1].Value = code;
                param[2].Value = facid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("17)ModifySpCodeParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }
        #endregion



        #region 修改用户注册表信息
        /// <summary>
        /// 修改用户注册表信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="scannum">扫描奖励次数</param>
        ///  <param name="mile">奖励里程</param>
        /// <returns></returns>
        public OracleParameter[] ModifyUserBaseRegistParam(string facid, string mobile, string scannum, int mile)
        {
            OracleParameter[] param = null;
            try
            {
                #region

                param = (OracleParameter[])ParameterCache.GetParams("ModifyUserBaseRegistParam");
                if (param == null)
                {
                    param = new OracleParameter[4];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar);
                    param[2] = new OracleParameter(":F5", OracleType.VarChar);
                    param[3] = new OracleParameter(":POINTTOTAL", OracleType.Number);

                    //将参数加入缓存
                    ParameterCache.PushCache("ModifyUserBaseRegistParam", param);
                }
                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = scannum;
                param[3].Value = mile;

                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("17)ModifyUserBaseRegistParam----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        #endregion





        #endregion

        #region 7) 添加参与日志
        /// <summary>
        /// 添加参与日志
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">渠道(W,S,M)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <returns></returns>
        public bool AddLotteryLog(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetLotteryLogParam(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);

                string table = GetTable(TableName_Log, facid);
                string sql = string.Format(ADD_SHAKE_LOG_SQL, table);

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

        #region 7.1) 添加参与日志 (话费充值+信息收集)
        /// <summary>
        /// 添加参与日志
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">渠道(W,S,M)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <param name="registersql">要执行信息插入的sql语句</param>
        /// <returns></returns>
        public bool AddLotteryLog(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string registersql, bool isSpCode = false)
        {
            bool bRet = false;
            try
            {

                OracleParameter[] param2 = GetLotteryLogParam(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);

                OracleParameter[] param3 = ModifySpCodeParam(facid, digitCode, ip);

                DataBase dataBase = new DataBase();
                dataBase.BeginTrans();

                try
                {
                    int nRet1 = 0;
                    int nRet2 = 0;
                    int nRet3 = 0;

                    string table = GetTable(TableName_Log, facid);
                    string sql = string.Format(ADD_SHAKE_LOG_SQL, table);

                    nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);

                    if (!string.IsNullOrEmpty(registersql))
                    {
                        nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                    }
                    else
                    {
                        nRet2 = 1;
                    }



                    if (isSpCode)
                    {
                        nRet3 = dataBase.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param3);
                    }
                    else
                    {
                        nRet3 = 1;
                    }

                    if ((nRet1 + nRet2 + nRet3) == 3)
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
                    KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                    dataBase.RollBackTrans();
                    throw ex;
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:AddLotteryLog:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 7.2) 添加参与日志(实物开奖+信息收集)
        /// <summary>
        /// 添加参与日志(实物开奖+信息收集)
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">渠道(W,S,M)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <param name="registersql">要执行信息插入的sql语句</param>
        /// <returns></returns>
        public bool AddLotteryLogAndRegister(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string registersql)
        {
            DataBase dataBase = new DataBase();
            dataBase.BeginTrans();

            bool bRet = false;

            try
            {
                int nRet1 = 0;
                int nRet2 = 0;

                OracleParameter[] param2 = GetLotteryLogParam(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);

                string table = GetTable(TableName_Log, facid);
                string sql = string.Format(ADD_SHAKE_LOG_SQL, table);

                nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);

                if (!string.IsNullOrEmpty(registersql))
                {
                    nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                }
                else
                {
                    nRet2 = 1;
                }

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
                Logger.AppLog.Write("LotteryDal:AddLotteryLog:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                dataBase.RollBackTrans();
                return false;
            }
            return bRet;
        }
        #endregion


        #region 7.3) 添加参与日志(曼牌)
        /// <summary>
        /// 添加参与日志
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">渠道(W,S,M)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <param name="registersql">要执行信息插入的sql语句</param>
        /// <returns></returns>
        public bool AddLotteryLogMP(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result)
        {
            bool bRet = false;
            try
            {

                OracleParameter[] param2 = GetLotteryLogParam(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);

                DataBase dataBase = new DataBase();
                dataBase.BeginTrans();
                try
                {
                    int nRet1 = 0;

                    string table = GetTable(TableName_Log, facid);
                    string sql = string.Format(ADD_SHAKE_LOG_SQL, table);
                    nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                    if (nRet1 == 1)
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
                    KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                    dataBase.RollBackTrans();
                    throw ex;
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:AddLotteryLog:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion


        #region 8) 添加真码参与日志
        /// <summary>
        /// 添加参与日志
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">渠道(W,S,M)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">地区</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <returns></returns>
        public bool AddLotteryParLog(string guid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetLotteryParLogParam(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);


                string table = GetTable(TableName_PARLOG, facid);
                string sql = string.Format(ADD_SHAKE_PARLOG_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:AddLotteryParLogParam:" + facid + "---" + digitCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 9) 添加参与记录和真码参与记录
        /// <summary>
        /// 添加参与记录和真码参与记录
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="ismodifyAwardNum">是否修改参与次数</param>
        /// <param name="registersql">信息收集sql集合</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string guid1, string guid2, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool ismodifyAwardNum, bool isModifyParLog, string registersql)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param3 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = UpdateAwardNumParam(facid, activityId, poolid);


                if (param2 != null && param1 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;

                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        if (isModifyParLog) //是否增加真码参与日志
                        {
                            string table2 = GetTable(TableName_PARLOG, facid);
                            string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        }
                        else
                        {
                            nRet2 = 1;
                        }


                        if (ismodifyAwardNum) //是否修改参与次数
                        {
                            string table3 = GetTable(TableName_Rule, facid);
                            string sql3 = string.Format(UPDATE_SHAKE_RULE_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        }
                        else
                        {
                            nRet3 = 1;
                        }


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet4 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet4 = 1;
                        }


                        if ((nRet1 + nRet2 + nRet3 + nRet4) == 4)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        /// <summary>
        /// 添加参与记录和真码参与记录
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="ismodifyAwardNum">是否修改参与次数</param>
        /// <param name="registersql">信息收集sql集合</param>
        /// <param name="isSpCode">是否为中性码</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string guid1, string guid2, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool ismodifyAwardNum, bool isModifyParLog, string registersql, bool isSpCode)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param8 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = UpdateAwardNumParam(facid, activityId, poolid);
                param8 = ModifySpCodeParam(facid, digitCode, ip);


                if (param2 != null && param1 != null && param3 != null && param8 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;
                        int nRet8 = 0;

                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        if (isModifyParLog) //是否增加真码参与日志
                        {
                            string table2 = GetTable(TableName_PARLOG, facid);
                            string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        }
                        else
                        {
                            nRet2 = 1;
                        }


                        if (ismodifyAwardNum) //是否修改参与次数
                        {
                            string table3 = GetTable(TableName_Rule, facid);
                            string sql3 = string.Format(UPDATE_SHAKE_RULE_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        }
                        else
                        {
                            nRet3 = 1;
                        }


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet4 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet4 = 1;
                        }

                        if (isSpCode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }


                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet8) == 5)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }


        /// <summary>
        /// 添加参与记录和真码参与记录 德农专用
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="ismodifyAwardNum">是否修改参与次数</param>
        /// <param name="registersql">信息收集sql集合</param>
        /// <param name="isSpCode">是否为中性码</param>
        /// <returns></returns>
        public bool AddLotteryParLogDL(string guid1, string guid2, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool ismodifyAwardNum, bool isModifyParLog, string registersql)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param5 = null;
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = UpdateAwardNumParam(facid, activityId, poolid);
                param5 = UpdateRegister(facid, ip, "1");

                if (param2 != null && param1 != null && param3 != null && param5 != null)
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

                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        if (isModifyParLog) //是否增加真码参与日志
                        {
                            string table2 = GetTable(TableName_PARLOG, facid);
                            string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        }
                        else
                        {
                            nRet2 = 1;
                        }


                        if (ismodifyAwardNum) //是否修改参与次数
                        {
                            string table3 = GetTable(TableName_Rule, facid);
                            string sql3 = string.Format(UPDATE_SHAKE_RULE_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        }
                        else
                        {
                            nRet3 = 1;
                        }


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet4 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet4 = 1;
                        }


                        string table5 = GetTable(TableName_Signin, facid);
                        string sql5 = string.Format(SQL_UPDATE_REGISTER_DL, table5);
                        nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                        nRet5 = 1;

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5) == 5)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 9.1) 添加参与记录和真码参与记录
        /// <summary>
        /// 添加参与记录和真码参与记录
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="ismodifyAwardNum">是否修改参与次数</param>
        /// <param name="isModifyParLog">是否添加真码参与日志记录</param>
        /// <param name="isModifyParLog">是否添加真码参与日志记录</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string guid1, string guid2, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool ismodifyAwardNum, bool isModifyParLog)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param3 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = UpdateAwardNumParam(facid, activityId, poolid);


                if (param2 != null && param1 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;


                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        if (isModifyParLog) //是否增加真码参与日志
                        {
                            string table2 = GetTable(TableName_PARLOG, facid);
                            string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        }
                        else
                        {
                            nRet2 = 1;
                        }


                        if (ismodifyAwardNum) //是否修改参与次数
                        {
                            string table3 = GetTable(TableName_Rule, facid);
                            string sql3 = string.Format(UPDATE_SHAKE_RULE_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        }
                        else
                        {
                            nRet3 = 1;
                        }



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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 9.1) 添加参与记录和真码参与记录
        /// <summary>
        /// 添加参与记录和真码参与记录
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="ismodifyAwardNum">是否修改参与次数</param>
        /// <param name="isModifyParLog">是否添加真码参与日志记录</param>
        /// <param name="isModifyParLog">是否添加真码参与日志记录</param> 
        /// <param name="registersql">添加注册信息</param>
        /// <returns></returns>
        public bool AddLotteryParLogAndRegister(string guid1, string guid2, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool ismodifyAwardNum, bool isModifyParLog, string registersql)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param3 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = UpdateAwardNumParam(facid, activityId, poolid);


                if (param2 != null && param1 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;

                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        if (isModifyParLog) //是否增加真码参与日志
                        {
                            string table2 = GetTable(TableName_PARLOG, facid);
                            string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        }
                        else
                        {
                            nRet2 = 1;
                        }


                        if (ismodifyAwardNum) //是否修改参与次数
                        {
                            string table3 = GetTable(TableName_Rule, facid);
                            string sql3 = string.Format(UPDATE_SHAKE_RULE_SQL, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        }
                        else
                        {
                            nRet3 = 1;
                        }


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet4 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet4 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4) == 4)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 10) 添加中奖日志
        /// <summary>
        /// 添加抽奖日志
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <returns>是否成功</returns>
        public bool AddLottery(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, bool isAddRegister)
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
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                param6 = GetRegisterUserParam(guid1, digitCode, channel, ip, awardsno, newcode, facid);



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

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);

                        //中奖记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);


                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);

                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if (isAddRegister) //预注册
                        {
                            string table6 = GetTable(TableName_Signin, facid);
                            string sql6 = string.Format(ADD_REGISTER_SQL, table6);
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, sql6, param6);
                        }
                        else
                        {
                            nRet6 = 1;
                        }



                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6) == 6)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 10.1) 添加中奖日志
        /// <summary>
        /// 添加抽奖日志
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="registerSql">注册信息SQL</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryAndRegister(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registerSql)
        {
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            //OracleParameter[] param6 = null;
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                //param6 = GetRegisterUserParam(digitCode, channel, ip, awardsno, newcode, facid);



                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null)
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

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);

                        //中奖记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);


                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);

                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if (!string.IsNullOrEmpty(registerSql))
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, registerSql, null);
                        }
                        else
                        {
                            nRet6 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6) == 6)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 10.2) 添加中奖日志
        /// <summary>
        /// 添加抽奖日志
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="registerSql">注册信息SQL</param>
        /// <param name="newpoolid">奖池ID</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryAndRegister(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registerSql, string newpoolid)
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
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                param6 = UpdateAwardNumParam(facid, activityId, newpoolid);



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
                        int nRet7 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);

                        //中奖记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);


                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);

                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if (!string.IsNullOrEmpty(registerSql))
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, registerSql, null);
                        }
                        else
                        {
                            nRet6 = 1;
                        }

                        string table7 = GetTable(TableName_Rule, facid);
                        string sql7 = string.Format(UPDATE_SHAKE_RULE_SQL, table7);
                        nRet7 = dbc.ExecuteNonQuery(CommandType.Text, sql7, param6);


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
                        KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 11) 判断数码是否被注册过
        /// <summary>
        /// 判断数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="code">二次加密后的数码</param>
        /// <returns></returns>
        public object IsRegisterFlagByCode(string facid, string digitcode, string code)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("isRegisterByCodeParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("isRegisterByCodeParam", param);
                }
                param[0].Value = facid;
                param[1].Value = digitcode;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_Signin, facid);

                string sql = string.Format(SIGNIN_SHAKE_SQL, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:isRegisterByCode:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 11.1) 判断数码是否被注册过
        /// <summary>
        /// 判断数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="code">二次加密后的数码</param>
        /// <returns></returns>
        public object IsRegisterFlagByCode2(string facid, string digitcode, string code)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("IsRegisterFlagByCode2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("IsRegisterFlagByCode2", param);
                }
                param[0].Value = facid;
                param[1].Value = digitcode;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_Signin, facid);

                string sql = string.Format(SIGNIN_SHAKE_SQL2, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:IsRegisterFlagByCode2:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 11.1) 判断数码是否被注册过
        /// <summary>
        /// 判断数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="code">二次加密后的数码</param>
        /// <returns></returns>
        public object IsRegisterFlagByCode3(string facid, string digitcode, string code)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("IsRegisterFlagByCode2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("IsRegisterFlagByCode2", param);
                }
                param[0].Value = facid;
                param[1].Value = digitcode;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_Signin, facid);

                string sql = string.Format(SIGNIN_SHAKE_SQL3, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:IsRegisterFlagByCode2:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 11.4) 根据数码+openid检查数码是否被注册过
        /// <summary>
        /// 根据数码+openid检查数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="OPENID">OPENID</param>
        /// <returns></returns>
        public object IsRegisterFlagByCode4(string facid, string digitcode, string OPENID)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 16);
                param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);


                param[0].Value = facid;
                param[1].Value = digitcode;
                param[2].Value = OPENID;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_Signin, facid);

                string sql = string.Format(SIGNIN_SHAKE_SQL4, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:IsRegisterFlagByCode4:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 11.4) 根据分组编号+openid检查是否领取礼品
        /// <summary>
        /// 根据分组编号+openid检查是否领取礼品
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="groupid">分组ID</param>
        /// <param name="OPENID">OPENID</param>
        /// <returns></returns>
        public object isRegisterByGroupID(string facid, string groupid, string OPENID)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 16);
                param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);


                param[0].Value = facid;
                param[1].Value = groupid;
                param[2].Value = OPENID;

                DataBase dataBase = new DataBase();

                string tableLottery = GetTable(TableName_Signin, facid);

                string sql = string.Format(SIGNIN_SHAKE_SQL4, tableLottery);

                oRet = dataBase.ExecuteScalar(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:isRegisterByGroupID:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }
        #endregion

        #region 12) 通过数码获取注册信息
        /// <summary>
        /// 通过数码获取注册信息
        /// </summary>
        /// <param name="digitCode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public RegisterUser GetLotteryUserInfo(string digitCode, string facid)
        {

            DataBase dataBase = new DataBase();
            RegisterUser modelinfo = null;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryUserInfoParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryUserInfoParam", param);
                }
                param[0].Value = facid;
                param[1].Value = digitCode;
                #endregion

                string tableLottery = GetTable(TableName_Signin, facid);
                string sql = string.Format(SIGINUSER_SHAKE_SQL, tableLottery);

                using (IDataReader rdr = dataBase.ExecuteReader(CommandType.Text, sql, param))
                {
                    if (rdr.Read())
                    {
                        modelinfo = new RegisterUser(rdr.GetValue(0).ToString(), rdr.GetValue(1).ToString(), rdr.GetValue(2).ToString(), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetValue(6).ToString(), rdr.GetValue(7).ToString(), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetValue(10).ToString(), rdr.GetValue(11).ToString(), rdr.GetValue(12).ToString(), rdr.GetValue(13).ToString(), rdr.GetValue(14).ToString(), rdr.GetValue(15).ToString(), rdr.GetValue(16).ToString(), rdr.GetValue(17).ToString(), rdr.GetValue(18).ToString(), rdr.GetValue(19).ToString(), rdr.GetValue(20).ToString());
                    }
                    else
                    {
                        modelinfo = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetLotteryUserInfo:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return modelinfo;
            }
            return modelinfo;
        }
        #endregion

        #region 13) 添加中奖信息注册
        /// <summary>
        /// 中奖信息注册
        /// </summary>
        /// <param name="userinfo">注册信息实体类</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public int SignIn(RegisterUser userinfo, string facid)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetSiginUserParam(userinfo);

                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(ADD_SININUSER_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:SignIn:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return 0;
            }
        }
        #endregion

        #region 14) 中奖用户注册
        /// <summary>
        /// 中奖用户注册
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public int ModifyRegisterUserByCode(RegisterUser userinfo, string facid)
        {
            try
            {
                DataBase dataBase = new DataBase();
                OracleParameter[] param2 = GetModifyRegisterParam(userinfo);

                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(MODIFY_REGISTER_SQL, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:ModifyRegisterUserByCode:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return 0;
            }
        }
        #endregion

        #region 15) 中奖用户注册
        /// <summary>
        /// 中奖用户注册
        /// </summary>
        /// <param name="registersql">注册信息sql</param>
        /// <returns></returns>
        public int ModifyRegisterUserByCode(string registersql)
        {
            try
            {
                DataBase dataBase = new DataBase();
                int row = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:ModifyRegisterUserByCode:---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return 0;
            }
        }
        #endregion

        #region 15.1) 中奖用户注册
        /// <summary>
        /// 中奖用户注册
        /// </summary>
        /// <param name="registersql">注册信息sql</param>
        /// <returns></returns>
        public int InsertRegisterUserByCode(string registersql)
        {
            try
            {
                DataBase dataBase = new DataBase();
                int row = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:InsertRegisterUserByCode:---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return 0;
            }
        }
        #endregion

        #region 15.2) 中奖用户注册(发送邮件)

        /// <summary>
        /// 中奖用户注册(发送邮件)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="registersql">注册信息sql</param>
        /// <param name="code">数码</param>
        /// <param name="ecode">京东E卡卡密</param>
        /// <param name="email">邮箱</param>
        /// <param name="sendMailTemplate">邮件发送内容</param>
        /// <returns></returns>
        public bool ModifyRegisterUserByCode(string facid, string registersql, string code, string ecode, string email, string sendMailTemplate)
        {
            DataBase dataBase = new DataBase();
            dataBase.BeginTrans();

            bool bRet = false;
            try
            {
                try
                {
                    //三等奖注册
                    if (!string.IsNullOrEmpty(ecode) && !string.IsNullOrEmpty(email))
                    {

                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet4 = 0;
                        int nRet3 = 0;

                        #region 序列化参数
                        OracleParameter[] param = null;
                        param = new OracleParameter[4];
                        param[0] = new OracleParameter(":FACID", OracleType.VarChar, 8);
                        param[1] = new OracleParameter(":CODE", OracleType.VarChar, 24);
                        param[2] = new OracleParameter(":EMAIL", OracleType.VarChar, 50);
                        param[3] = new OracleParameter(":ECODE", OracleType.VarChar, 50);

                        param[0].Value = facid;
                        param[1].Value = code;
                        param[2].Value = email;
                        param[3].Value = ecode;

                        #endregion


                        //修改中奖纪录
                        if (!string.IsNullOrEmpty(registersql))
                            nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);

                        //修改京东卡密状态
                        string sql_jd = "UPDATE T_SGM_SHAKE_JDECODE J SET J.CODE=:CODE, J.FLAG='2',J.F1=:EMAIL WHERE J.ECODE=:ECODE AND J.FACID=:FACID AND J.FLAG='1'";
                        nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, sql_jd, param);

                        OracleParameter[] param4 = null;
                        param4 = sendPrewarningParam(1, facid, sendMailTemplate);
                        string sql4 = string.Format(SEND_SMS_SQL, sendMailTemplate);
                        nRet4 = dataBase.ExecuteNonQuery(CommandType.Text, sql4, param4);


                        string sql3 = string.Format("INSERT INTO T_SMS_SEND_TO (SEND_USERID,SEND_TO,SUBJECT,SUBJECT_TYPE,SEND_TYPE,SEND_CONTENT,CUSTOMER_CODE) VALUES ('CCN_System',:Mail,'卡密发送通知','6','02','{0}',:FACID)", sendMailTemplate);

                        #region 序列话参数

                        OracleParameter[] param3 = null;
                        param3 = new OracleParameter[2];
                        param3[0] = new OracleParameter(":FACID", OracleType.VarChar, 8);
                        param3[1] = new OracleParameter(":Mail", OracleType.VarChar, 50);


                        param3[0].Value = facid;
                        param3[1].Value = email;
                        #endregion

                        nRet3 = dataBase.ExecuteNonQuery(CommandType.Text, sql3, param3);

                        if ((nRet1 + nRet2 + nRet3 + nRet4) == 4)
                        {
                            bRet = true;
                            dataBase.CommitTrans();
                        }
                    }
                    else //一等奖注册（没有emial）
                    {
                        int nRet1 = 0;

                        //修改中奖纪录
                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                            dataBase.CommitTrans();
                        }
                        return (nRet1 > 0) ? true : false;
                    }
                }
                catch (Exception ex)
                {
                    KMSLotterySystemFront.Logger.AppLog.Write("LotteryDal:ModifyRegisterUserByCode--" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                    dataBase.RollBackTrans();
                    throw ex;
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:ModifyRegisterUserByCode:---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 16) 添加中奖日志（杜邦）
        /// <summary>
        /// 添加抽奖日志（杜邦）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryDuPont(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int lotteryNumber, int totalTimes, out string systemstate)
        {
            systemstate = string.Empty;
            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null)
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

                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //根据手机号获取杜邦中奖队列表数据（未合并数据为15元的直接充值）
                        string selectQueuedata = "select facid,mobile from t_sgm_shake_queuedata where DELETEFLAG='0' and facid='{0}' and mobile='{1}' group by FACID,MOBILE having count(mobile)>=4 ";
                        string insertQueuedata = @"insert into t_sgm_shake_queuedata (FACID,MOBILE,ORDERID,CITYCODE,CITYNAME,CARDNUM,DELETEFLAG) 
                                                    select facid,'{0}','{1}','{2}','{2}',nvl(b.lotterymoeny,0),'{3}' from t_sgm_wb_basedata_9999 b where b.facid='{4}' and b.datatypename='LotteryType' and codeid='{5}'";
                        string updateQueuedata = "update t_sgm_shake_queuedata set DELETEFLAG='1' where facid='{0}' and mobile='{1}' and DELETEFLAG='0'  and rownum<=4 ";

                        string insertMobileCZ = @"insert into T_MOBILE_ONLINE_SEND (FACID, MOBILE, ORDERID,STATECODE,CITYCODE,STATENAME,CITYNAME,CARDID,CARDNUM,RUNNUM,RESULTID,STATE) 
                                               values('{0}','{1}','{2}','{3}','{3}','{3}','{3}','140101','20','0','0','0')";

                        // 先插入一条中奖记录
                        nRet6 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(insertQueuedata, ip, digitCode, area, "0", facid, awardsno), null);
                        if (lotteryNumber >= totalTimes)
                        {
                            systemstate = "123";//本次活动充值已达上线金额
                            nRet4 = 1;
                            nRet7 = 1;
                        }
                        else
                        {
                            DataTable dt = dbc.ExecuteQuery(CommandType.Text, string.Format(selectQueuedata, facid, ip), null);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                nRet6 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(updateQueuedata, facid, ip), null);
                                if (nRet6 == 4)
                                {
                                    nRet6 = 1;
                                }
                                else
                                {
                                    nRet6 = 0;
                                }
                                //欧飞充值插入记录
                                string orderid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                                nRet7 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(insertMobileCZ, facid, ip, orderid, area), null);

                                //中奖充值记录+1
                                string table4 = GetTable(TableName_Pool, facid);
                                string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                                nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                                systemstate = "124";//中奖金额满20元

                            }
                            else
                            {
                                nRet4 = 1;
                                nRet7 = 1;
                            }
                        }

                        //预警消息提醒
                        if (smsflag && systemstate == "124")
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);

                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7) >= 7)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("（杜邦）记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("（杜邦）记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 17) 添加中奖日志（针对话费充值类抽奖活动）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动）逦彩
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRecharge(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int money)
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
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0");
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


                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }
                        //ORDERID 为数码
                        nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND, param6);


                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6) >= 6)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        /// <summary>
        /// 获取中奖日志的参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="orderid">订单号码=16位数码</param>
        /// <param name="notes">发送信息内容</param>
        /// <param name="citycode">城市代码</param>
        /// <param name="cityname">城市名称</param>
        /// <param name="statecode">状态码</param>
        /// <param name="statename">状态名称</param>
        /// <param name="money">中奖金额</param>
        /// <param name="cardid"></param>
        /// <param name="resultid"></param>
        /// <returns>参数对象数组</returns>
        private OracleParameter[] GetLotteryRechargeParameter(string facid, string mobile, string orderid, string notes, string citycode, string cityname, string statecode, string statename, string money, string cardid, string resultid)
        {
            OracleParameter[] param = null;
            try
            {
                #region
                param = (OracleParameter[])ParameterCache.GetParams("AddLotteryRechargeParameter");
                //构造参数
                if (param == null)
                {
                    //:FACID,:MOBILE,:ORDERID ,:NOTES ,:CITYCODE,:CITYNAME,:STATECODE,:STATENAME,:CARDNUM,:CARDID,:RESULTID
                    param = new OracleParameter[11];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    param[2] = new OracleParameter(":ORDERID", OracleType.VarChar, 100);
                    param[3] = new OracleParameter(":NOTES", OracleType.VarChar, 4000);
                    param[4] = new OracleParameter(":CITYCODE", OracleType.VarChar, 20);
                    param[5] = new OracleParameter(":CITYNAME", OracleType.VarChar, 20);
                    param[6] = new OracleParameter(":STATECODE", OracleType.VarChar, 20);
                    param[7] = new OracleParameter(":STATENAME", OracleType.VarChar, 20);
                    param[8] = new OracleParameter(":CARDNUM", OracleType.VarChar, 10);
                    param[9] = new OracleParameter(":CARDID", OracleType.VarChar, 20);
                    param[10] = new OracleParameter(":RESULTID", OracleType.VarChar, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("AddLotteryRechargeParameter", param);
                }
                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = orderid;
                param[3].Value = notes;
                param[4].Value = citycode;
                param[5].Value = cityname;
                param[6].Value = statecode;
                param[7].Value = statename;
                param[8].Value = money;
                param[9].Value = cardid;
                param[10].Value = resultid;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)组织参与日志表----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        /// <summary>
        /// 组织话费充值日志的参数
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="orderid">订单号码=16位数码</param>
        /// <param name="notes">发送信息内容</param>
        /// <param name="citycode">城市代码</param>
        /// <param name="cityname">城市名称</param>
        /// <param name="statecode">状态码</param>
        /// <param name="statename">状态名称</param>
        /// <param name="money">中奖金额</param>
        /// <param name="cardid">商品编号</param>
        /// <param name="resultid">状态</param>
        /// <param name="lotteryguid">中奖纪录guid</param>
        /// <returns></returns>
        private OracleParameter[] GetLotteryRechargeParameter(string facid, string mobile, string orderid, string notes, string citycode, string cityname, string statecode, string statename, string money, string cardid, string resultid, string lotteryguid)
        {
            OracleParameter[] param = null;
            try
            {
                #region

                //:FACID,:MOBILE,:ORDERID ,:NOTES ,:CITYCODE,:CITYNAME,:STATECODE,:STATENAME,:CARDNUM,:CARDID,:RESULTID
                param = new OracleParameter[13];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":ORDERID", OracleType.VarChar, 100);
                //param[3] = new OracleParameter(":NOTES", OracleType.VarChar, 4000);
                param[3] = new OracleParameter(":NOTES", OracleType.LongVarChar);
                param[4] = new OracleParameter(":CITYCODE", OracleType.VarChar, 20);
                param[5] = new OracleParameter(":CITYNAME", OracleType.VarChar, 20);
                param[6] = new OracleParameter(":STATECODE", OracleType.VarChar, 20);
                param[7] = new OracleParameter(":STATENAME", OracleType.VarChar, 20);
                param[8] = new OracleParameter(":CARDNUM", OracleType.VarChar, 10);
                param[9] = new OracleParameter(":CARDID", OracleType.VarChar, 20);
                param[10] = new OracleParameter(":RESULTID", OracleType.VarChar, 10);
                param[11] = new OracleParameter(":CARDNO", OracleType.VarChar, 40);
                param[12] = new OracleParameter(":ORDERIDS", OracleType.VarChar, 100);

                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = orderid;
                param[3].Value = notes;
                param[4].Value = citycode;
                param[5].Value = cityname;
                param[6].Value = statecode;
                param[7].Value = statename;
                param[8].Value = money;
                param[9].Value = cardid;
                param[10].Value = resultid;
                param[11].Value = lotteryguid;
                param[12].Value = orderid;
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

        //INSERT INTO T_MOBILE_FLOW_ONLINE_SEND(GUID,FACID,ORDERID,ORDERTYPE,PACKCODE,MOBILE,PACKNUM,PACKMONEY,SIGN,ACTIVITYID,DIGITCODE) VALUES (:GUID,:FACID,:ORDERID,:ORDERTYPE,:PACKCODE,:MOBILE,:PACKNUM,:PACKMONEY,:SIGN,:ACTIVITYID,:DIGITCODE)

        #region 流量充值参数组织

        private OracleParameter[] GetLotteryLLParameter(string guid, string facid, string orderid, string ordertype, string packcode, string mobile, string packnum, string packmoney, string sign, string activityid, string digitcode)
        {
            OracleParameter[] param = null;
            try
            {
                #region

                //:GUID,:FACID,:ORDERID,:ORDERTYPE,:PACKCODE,:MOBILE,:PACKNUM,:PACKMONEY,:SIGN,:ACTIVITYID,:DIGITCODE
                param = new OracleParameter[11];
                param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[2] = new OracleParameter(":ORDERID", OracleType.VarChar, 50);
                param[3] = new OracleParameter(":ORDERTYPE", OracleType.VarChar, 2);
                param[4] = new OracleParameter(":PACKCODE", OracleType.VarChar, 10);
                param[5] = new OracleParameter(":MOBILE", OracleType.VarChar, 32);
                param[6] = new OracleParameter(":PACKNUM", OracleType.VarChar, 10);
                param[7] = new OracleParameter(":PACKMONEY", OracleType.Number);
                param[8] = new OracleParameter(":SIGN", OracleType.VarChar, 50);
                param[9] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 50);
                param[10] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 20);

                param[0].Value = guid;
                param[1].Value = facid;
                param[2].Value = orderid;
                param[3].Value = ordertype;
                param[4].Value = packcode;
                param[5].Value = mobile;
                param[6].Value = packnum;
                param[7].Value = packmoney;
                param[8].Value = sign;
                param[9].Value = activityid;
                param[10].Value = digitcode;
                #endregion
            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("组织流量充值表 GetLotteryLLParameter----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        #endregion

        #region 17.1) 添加中奖日志（针对话费充值类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRecharge(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int money, string registersql, bool isspcode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;
            OracleParameter[] param9 = null;//如果中奖，那么就要跟邀请人发送话费
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0", guid3);
                param8 = ModifySpCodeParam(facid, digitCode, ip);
                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet8 = 0;
                        int nRet9 = 0;
                        int nRet10 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        //nRet5 = 1;

                        //ORDERID 为数码
                        nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param6);

                        #region 被邀请人中奖之后，邀请人即时获得话费
                        //无论如何都不会影响提交事物，可能不是老带新的用户参与的
                        nRet9 = 1;
                        nRet10 = 1;
                        //如果中奖
                        if (nRet3 > 0)
                        {
                            ControlDao cbll = new ControlDao();
                            object inviteMoney = cbll.GetLotteryName3(facid, "LotteryInviteType", "LOTTERYMOENY");
                            if (inviteMoney != null)
                            {
                                //查询当前中奖的手机号是不是有人邀请
                                string sql = string.Format("SELECT * FROM T_SGM_SHAKE_Invite X WHERE x.inviteto='{0}'  ORDER BY X.CREATEDATE ASC ", ip);
                                DataTable dtInvite = dbc.ExecuteQuery(CommandType.Text, sql, null);
                                //是否已经为邀请我的人发放奖励了，每个月只发放一次
                                //bool LotteryToFrom = false;
                                string INVITEFROM = string.Empty;
                                string guid = string.Empty;
                                if (dtInvite != null && dtInvite.Rows.Count > 0)
                                {
                                    int i = 0;
                                    foreach (DataRow dr in dtInvite.Rows)
                                    {
                                        INVITEFROM = dr["INVITEFROM"].ToString();
                                        guid = dr["GUID"].ToString();
                                        sql = string.Format("SELECT * FROM T_SGM_SHAKE_Invite X WHERE x.INVITEFROM='{0}' AND X.INVITEFROMLOTTERY='1' AND X.invitelotteryguid IS NOT NULL", INVITEFROM);
                                        DataTable dtInvite2 = dbc.ExecuteQuery(CommandType.Text, sql, null);
                                        if (dtInvite2 != null && dtInvite2.Rows != null && dtInvite2.Rows.Count > 0)
                                        {
                                            i++;
                                            continue;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //string INVITEFROMLOTTERY = dr["INVITEFROMLOTTERY"].ToString();
                                        //if (INVITEFROMLOTTERY == "1")
                                        //{
                                        //    LotteryToFrom = true;
                                        //    break;
                                        //}
                                    }

                                    if (dtInvite.Rows.Count == i)
                                    {
                                        INVITEFROM = string.Empty;
                                        guid = string.Empty;
                                    }
                                    //if (!LotteryToFrom)
                                    //{
                                    //string InviteMan = dtInvite.Rows[0]["INVITEFROM"].ToString();
                                    if (!string.IsNullOrEmpty(INVITEFROM) && !string.IsNullOrEmpty(guid))
                                    {
                                        param9 = GetLotteryRechargeParameter(facid, INVITEFROM, digitCode + "X", result, area, area, "001", "001", inviteMoney.ToString(), "140101", "0", guid3);
                                        nRet9 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param9);

                                        //string guid = dtInvite.Rows[0]["GUID"].ToString();
                                        sql = string.Format("update T_SGM_SHAKE_Invite X  set x.invitelotteryflag='1',x.INVITEFROMLOTTERY='1',x.invitelotteryguid='{0}',x.invitelotterydate=to_date('{1}','yyyy-mm-dd HH24:mi:ss') where x.guid='{2}'", guid3, DateTime.Now, guid);
                                        nRet10 = dbc.ExecuteNonQuery(CommandType.Text, sql, null);
                                    }
                                    //}
                                }
                            }
                        }
                        #endregion


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8 + nRet9 + nRet10) >= 10)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 17.1) 添加中奖日志（针对话费充值类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeAtlas(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int money, string registersql, bool isspcode, string ecode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param7 = null;
            OracleParameter[] param8 = null;
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                UpdateAwardNum(facid, activityId, poolid);
                param5 = sendPrewarningParam(3, facid, smscontent);
                //param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0", guid3);
                param6 = UpdateLotteryJDKQParam(facid, ecode, money.ToString(), digitCode);  //更新一号店
                param8 = ModifySpCodeParam(facid, digitCode, ip);
                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        //nRet5 = 1;

                        //更新京东卡密或者其他卡密
                        if (!string.IsNullOrEmpty(ecode))
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, UpdateJDKQ_SQL, param6);
                        }
                        else
                        {
                            nRet6 = 1;
                        }

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8) >= 8)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion


        #region 18) 添加中奖日志（针对红包类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRedEnvelopeRegister(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, double money, string registersql, bool isspcode)
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
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                UpdateAwardNum(facid, activityId, poolid);
                param6 = ModifySpCodeParam(facid, digitCode, ip);
                param5 = UpdateRegister(facid, ip, "1");
                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null)
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

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }


                        //修改注册信息
                        string table5 = GetTable(TableName_Signin, facid);
                        string sql5 = string.Format(SQL_UPDATE_REGISTER_DL, table5);
                        nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                        nRet5 = 1;


                        if (isspcode)
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param6);
                        }
                        else
                        {
                            nRet6 = 1;
                        }


                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7) >= 7)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("红包类抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("红包类抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion


        #region 17.2) 添加中奖日志（针对话费充值类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeYD(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int money, string registersql, bool isspcode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;
            OracleParameter[] param9 = null;//如果中奖，那么就要跟邀请人发送话费
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0", guid3);
                param8 = ModifySpCodeParam(facid, digitCode, ip);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet7 = 0;
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet7 + nRet8) >= 7)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 实物礼品+流量+话费  Tony 勿改

        #region 17.1) 添加中奖日志（针对实物礼品类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对实物礼品类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeSW(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, string registersql, bool isspcode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param8 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param8 = ModifySpCodeParam(facid, digitCode, ip);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param8 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;
                        int nRet7 = 0;
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //注册信息
                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }


                        //中性码参与
                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, UPDATE_STCODE_SQL, param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet7 + nRet8) == 6)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("实物礼品充值记录抽奖日志--AddLotteryRechargeSW--" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("实物礼品充值记录抽奖日志--AddLotteryRechargeSW--" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 17.2) 添加中奖日志（针对话费充值类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeHH(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int money, string registersql, bool isspcode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);
                param6 = GetLotteryRechargeParameter(facid, ip, digitCode + "_" + facid + "", result, area, area, "001", "001", money.ToString(), "140101", "0", guid3);
                param8 = ModifySpCodeParam(facid, digitCode, ip);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet7 = 0;
                        int nRet6 = 0;
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            //string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            //nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        string flag = GetBaseDataValue(facid, "IsRechargeBill");
                        if (flag == "0")//向测试充值表插入数据
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW_BAK, param6);
                        }
                        else
                        {
                            //增加话费充值记录
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param6);

                        }

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet7 + nRet6 + nRet8) >= 8)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 17.3) 添加中奖日志（针对流量充值类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="guid4">流量订单ORDERID</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="packCode">流量包订购代码</param>
        /// <param name="packNum">流量包大小</param>
        /// <param name="packMoney">流量包大小</param>
        /// <param name="activityid">监控平台分配的活动ID</param>
        /// <param name="sign">数据加密签名</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeLL(string guid1, string guid2, string guid3, string guid4, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, string registersql, bool isspcode, string packCode, string packNum, string packMoney, string activityid, string sign)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            // OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param6 = GetLotteryLLParameter(guid3, facid, guid4, "1", packCode, ip, packNum, packMoney, sign, activityid, digitCode);
                param8 = ModifySpCodeParam(facid, digitCode, ip);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param6 != null && param8 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;
                        int nRet7 = 0;
                        int nRet6 = 0;
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet1:" + nRet1, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet2:" + nRet2, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet3:" + nRet3, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet4:" + nRet4, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_FLOW_ONLINE_SEND_SQL, param6);
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet6:" + nRet6, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet7:" + nRet7, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----nRet8:" + nRet8, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet7 + nRet6 + nRet8) == 7)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("流量充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// R5E二期 根据被邀请人的中奖guid查询数据
        /// </summary>
        /// <param name="INVITELOTTERYGUID"></param>
        /// <returns></returns>
        public DataTable SelectInviteRecord(string INVITELOTTERYGUID, string facid)
        {
            DataBase dbc = new DataBase();
            dbc.BeginTrans();
            string sql = string.Format("SELECT * FROM T_SGM_SHAKE_Invite X WHERE x.INVITELOTTERYGUID='{0}' and x.facid='{1}'", INVITELOTTERYGUID, facid);
            DataTable dtInvite = dbc.ExecuteQuery(CommandType.Text, sql, null);
            dbc.CommitTrans();
            return dtInvite;

        }



        #region 17.1) 添加中奖日志（针对话费充值类抽奖活动+信息收集）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动+信息收集）
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <param name="registersql">注册SQL</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryEcode(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, int money, string registersql)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            // OracleParameter[] param5 = null;
            // OracleParameter[] param6 = null;
            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                // param5 = sendPrewarningParam(3, facid, smscontent);
                //param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0");
                //if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null)
                if (param2 != null && param3 != null && param1 != null && param4 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        int nRet4 = 0;
                        // int nRet5 = 0;
                        // int nRet6 = 0;
                        int nRet7 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        #region 不需要发送短信预警和充值

                        ////预警消息提醒
                        //if (smsflag)
                        //{
                        //    //string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                        //    //nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                        //    //nRet5 = 1;
                        //    nRet5 = 1;
                        //}
                        //else
                        //{
                        //    nRet5 = 1;
                        //}

                        //ORDERID 为数码
                        //nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND, param6);

                        #endregion

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet7) >= 5)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        //---------------------- wzp   开始

        #region 17.2) 添加中奖日志（针对实物，虚拟卡券类抽奖活动+信息收集） 喜力超凡
        /// <summary>
        /// 添加抽奖日志（针对实物，虚拟卡券类抽奖活动+信息收集）      喜力超凡
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="ecode">京东E卡卡号</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeXLCF(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registersql, bool isspcode, string ecode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;


            try
            {

                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);  // 获取参与抽奖日志组织参数
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取真码抽奖日志记录组织参数
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取抽奖日志记录组织参数
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno); //更新奖池信息（LOTTERYTIMES 中奖数量更新）
                param5 = sendPrewarningParam(3, facid, smscontent);    // 获取预警信息发送参数
                //param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0", guid3);
                param6 = UpdateLotteryJDKQParam(facid, ecode, awardsno, digitCode);  //更细京东卡券信息

                param8 = ModifySpCodeParam(facid, digitCode, ip);   //修改特殊抽奖数码数据

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        //更新京东卡密或者其他卡密
                        if (!string.IsNullOrEmpty(ecode))
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, UpdateJDKQ_SQL, param6);
                        }
                        else
                        {
                            nRet6 = 1;
                        }


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8) >= 8)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeXLCF()记录抽奖日志 异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeXLCF()记录抽奖日志 异常-----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion


        #region  17.3   获取京东卡券  壳牌喜力超凡
        //获取京东卡券
        /// <summary>
        /// 获取京东卡券
        /// </summary>
        /// <param name="facid"></param>
        /// <returns></returns>
        public DataTable GetJDKQ(string facid)
        {
            DataTable dt = null;

            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryJDKQByFacidParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryJDKQByFacidParam", param);
                }
                param[0].Value = facid;

                #endregion

                DataBase dataBase = new DataBase();

                string table = "t_sgm_shake_jdecode";

                dt = dataBase.ExecuteQuery(CommandType.Text, string.Format(FIND_JDKQ_SQL, table), param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" 获取京东卡券异常： LotteryDal:GetJDKQ:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }


            return dt;
        }
        #endregion

        //---------------------- wzp   结束

        #region 17.5) 添加中奖日志（信息收集） 壳牌加油站
        /// <summary>
        /// 添加抽奖日志（信息收集）      喜力超凡
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeShellStation(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registersql, bool isspcode)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            //OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;


            try
            {

                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);  // 获取参与抽奖日志组织参数
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取真码抽奖日志记录组织参数
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取抽奖日志记录组织参数
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno); //更新奖池信息（LOTTERYTIMES 中奖数量更新）
                param5 = sendPrewarningParam(3, facid, smscontent);    // 获取预警信息发送参数
                //param6 = UpdateLotteryJDKQParam(facid, ecode, awardsno, digitCode);  //更细京东卡券信息

                param8 = ModifySpCodeParam(facid, digitCode, ip);   //修改特殊抽奖数码数据

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param8 != null)
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
                        int nRet7 = 0;
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet7 + nRet8) >= 7)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeShellStation()记录抽奖日志 异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeShellStation()记录抽奖日志 异常-----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion


        #region 17.6) 添加中奖日志（话费充值+信息收集） 壳牌-WT
        /// <summary>
        /// 添加中奖日志（话费充值+信息收集）      壳牌-WT
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="mobilebill">话费充值金额</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeWT(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registersql, bool isspcode, string mobilebill)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;


            try
            {

                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);  // 获取参与抽奖日志组织参数
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取真码抽奖日志记录组织参数
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取抽奖日志记录组织参数
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno); //更新奖池信息（LOTTERYTIMES 中奖数量更新）
                param5 = sendPrewarningParam(3, facid, smscontent);    // 获取预警信息发送参数
                param6 = GetLotteryRechargeParameter(facid, ip, digitCode + "_WT2", result, area, area, "001", "001", mobilebill, "140101", "0", guid3);
                param8 = ModifySpCodeParam(facid, digitCode, ip);   //修改特殊抽奖数码数据

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet8 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);





                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }


                        //获取是否向正式充值记录表中插入数据

                        string flag = GetBaseDataValue(facid, "IsRechargeBill");
                        if (flag == "0")//向测试充值表插入数据
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW_BAK, param6);
                        }
                        else
                        {
                            //增加话费充值记录
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param6);

                        }

                        //更新京东卡密或者其他卡密
                        //if (!string.IsNullOrEmpty(ecode))
                        //{
                        //    nRet6 = dbc.ExecuteNonQuery(CommandType.Text, UpdateJDKQ_SQL, param6);
                        //}
                        //else
                        //{
                        //    nRet6 = 1;
                        //}


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8) >= 8)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeWT()记录抽奖日志 异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeWT()记录抽奖日志 异常-----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion


        #region 17.7) 添加中奖日志（话费充值+信息收集） 壳牌-通行证
        /// <summary>
        /// 添加中奖日志（话费充值+信息收集）      壳牌-通行证
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="userguid">用户guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="mobilebill">话费充值金额</param>
        /// <param name="miletype">奖励里程类别</param>
        /// <param name="mile">奖励里程</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeTXZ(string guid1, string guid2, string guid3, string userguid, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registersql, bool isspcode, string mobilebill, string miletype, int mile)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;

            OracleParameter[] param9 = null;//修改用户基础表 F5状态，
            OracleParameter[] param10 = null;//添加扫码奖励里程记录

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);  // 获取参与抽奖日志组织参数
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取真码抽奖日志记录组织参数
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取抽奖日志记录组织参数
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno); //更新奖池信息（LOTTERYTIMES 中奖数量更新）
                param5 = sendPrewarningParam(3, facid, smscontent);    // 获取预警信息发送参数
                param6 = GetLotteryRechargeParameter(facid, ip, digitCode + "_" + facid + "", result, area, area, "001", "001", mobilebill, "140101", "0", guid3);//话费充值
                param8 = ModifySpCodeParam(facid, digitCode, ip);   //修改特殊抽奖数码数据

                //根据厂家编号和手机号读取用户基础信息表
                DataTable dt_userbase = GetUserBaseInfo(facid, ip);
                int f5 = int.Parse(dt_userbase.Rows[0]["F5"].ToString());
                param9 = ModifyUserBaseRegistParam(facid, ip, (f5 + 1).ToString(), mile);//修改账户扫描奖励次数 F5字段
                //  param10 = AddUserMileParam(facid, userguid, miletype, digitCode, guid3, mile);//添加里程信息
                param10 = AddUserMileNewParam(facid, userguid, miletype, digitCode, guid3, mile, "");


                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null && param9 != null && param10 != null)
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
                        int nRet8 = 0;
                        int nRet9 = 0;
                        int nRet10 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }


                        //获取是否向正式充值记录表中插入数据

                        string flag = GetBaseDataValue(facid, "IsRechargeBill");
                        if (flag == "0")//向测试充值表插入数据
                        {
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW_BAK, param6);
                        }
                        else
                        {
                            //增加话费充值记录
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param6);

                        }
                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }


                        //修改扫码奖励次数
                        string table9 = GetTable(TableName_User, facid);
                        string sql9 = string.Format(SQL_UpdateUserBaseInfo, table9);
                        nRet9 = dbc.ExecuteNonQuery(CommandType.Text, sql9, param9);
                        //添加里程信息
                        nRet10 = dbc.ExecuteNonQuery(CommandType.Text, SQL_AddUserMileNew, param10);

                        #region //不使用
                        //int nRet11 = 0;//被邀请人添加奖励里程
                        //int nRet12 = 0;//回写邀请表中 被邀请人的中奖信息
                        //int nRet13 = 0;//被邀请人奖励话费信息

                        ////校验邀请表是否存在数据
                        //DataTable dt_invite = null;
                        //if (CheckIivited(facid, ip, out dt_invite))
                        //{
                        //    //被其他人邀请，循环遍历
                        //    if (dt_invite != null && dt_invite.Rows.Count > 0)
                        //    {

                        //        foreach (DataRow item in dt_invite.Rows)
                        //        {
                        //            string groupid = item["GROUPID"].ToString();
                        //            //检测groupid 组中，是否已经有人中奖，如果有人中奖，将继续循环下一个邀请组
                        //            if (!CheckInviteByGroupid(facid, groupid))
                        //            {
                        //                OracleParameter[] param11 = null;
                        //                OracleParameter[] param12 = null;
                        //                OracleParameter[] param13 = null;
                        //                // 组中没有人中奖，因此将当前用户中奖信息回写邀请表
                        //                string invitefrom = item["invitefrom"].ToString();//邀请人
                        //                string inviteguid = item["guid"].ToString();//邀请表guid
                        //                string invitetype = item["invitetype"].ToString();//邀请类型，扫码邀请，每月赠送邀请

                        //                // string inviteuserguid = item["userguid"].ToString();//邀请人userguid
                        //                DataTable dt_user = GetUserBaseInfo(facid, invitefrom);
                        //                string inviteuserguid = dt_user.Rows[0]["userguid"].ToString();

                        //                //给邀请人发放话费以及里程  话费 以及里程在数据库中配置

                        //                #region 获取奖励话费
                        //                int rewardmobilebill = 0;
                        //                DataTable dt_13 = GetBaseDataValueByCodeid(facid, "8", "RewardMileType");
                        //                if (dt_13 != null && dt_13.Rows.Count > 0)
                        //                {
                        //                    rewardmobilebill = int.Parse(dt_13.Rows[0]["LOTTERYMOENY"].ToString());
                        //                }

                        //                string orderid = digitCode + "R";

                        //                string rewardresult = "您邀请的好友" + ip + "已参与劲霸活动，奖励您" + rewardmobilebill + "元话费,请注意查收";
                        //                param13 = GetLotteryRechargeParameter(facid, invitefrom, orderid, rewardresult, area, area, "001", "001", rewardmobilebill.ToString(), "140101", "0", guid3);

                        //                if (flag == "0")//向测试充值表插入数据
                        //                {
                        //                    nRet13 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW_BAK, param13);
                        //                }
                        //                else
                        //                {
                        //                    //增加话费充值记录
                        //                    nRet13 = dbc.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param13);
                        //                }
                        //                #endregion

                        //                #region 奖励里程
                        //                int inviterewardmile = 0;
                        //                //添加里程信息
                        //                if (invitetype == "1")
                        //                {
                        //                    DataTable dt_5 = GetBaseDataValueByCodeid(facid, "5", "RewardMileType");
                        //                    if (dt_5 != null && dt_5.Rows.Count > 0)
                        //                    {
                        //                        inviterewardmile = int.Parse(dt_5.Rows[0]["LOTTERYMOENY"].ToString());
                        //                    }
                        //                    param11 = AddUserMileNewParam(facid, inviteuserguid, "5", "", "", inviterewardmile, inviteguid);//里程信息添加

                        //                }
                        //                else
                        //                {
                        //                    DataTable dt_6 = GetBaseDataValueByCodeid(facid, "6", "RewardMileType");
                        //                    if (dt_6 != null && dt_6.Rows.Count > 0)
                        //                    {
                        //                        inviterewardmile = int.Parse(dt_6.Rows[0]["LOTTERYMOENY"].ToString());
                        //                    }
                        //                    param11 = AddUserMileNewParam(facid, inviteuserguid, "6", "", "", inviterewardmile, inviteguid);//里程信息添加
                        //                }

                        //                nRet11 = dbc.ExecuteNonQuery(CommandType.Text, SQL_AddUserMileNew, param11);
                        //                #endregion

                        //                #region 回写邀请表中 的中奖数据
                        //                param12 = GetUpdateInviteByGuidParam(facid, inviteguid, guid3);
                        //                nRet12 = dbc.ExecuteNonQuery(CommandType.Text, SQL_UpdateInviteByGuid, param12);
                        //                #endregion
                        //                break;
                        //            }
                        //            else
                        //            {

                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        nRet11 = 1;
                        //        nRet12 = 1;
                        //        nRet13 = 1;
                        //    }
                        //}
                        //else//邀请表中，此账号  没有被其他人邀请，
                        //{
                        //    nRet11 = 1;
                        //    nRet12 = 1;
                        //    nRet13 = 1;
                        //}

                        #endregion
                        //校验此账户是否是被邀请人，如果是，将给邀请人发放奖励
                        //校验此被邀请人是否已经更新邀请表，如果更新，则意味着，已经给最先邀请 此账号的邀请人发放过奖励，将不能重复发放
                        #region 不使用
                        //if (CheckIivited(facid, ip))
                        //{
                        //    nRet11 = 1;
                        //    nRet12 = 1;
                        //}
                        //else
                        //{
                        //    //获取邀请人，给邀请人发放奖励，并回写数据
                        //    DataTable dt_in = GetInviteFromByTo(facid, ip);
                        //    if (dt_in != null && dt_in.Rows.Count > 0)
                        //    {
                        //        OracleParameter[] param11 = null;
                        //        OracleParameter[] param12 = null;
                        //        string invitefrom = dt_in.Rows[0]["invitefrom"].ToString();//邀请人
                        //        string inviteguid = dt_in.Rows[0]["guid"].ToString();//邀请表guid
                        //        string inviteuserguid = dt_in.Rows[0]["userguid"].ToString();//邀请人userguid
                        //        string invitetype = dt_in.Rows[0]["invitetype"].ToString();//邀请类型，扫码邀请，每月赠送邀请
                        //        //获取奖励里程
                        //        int inviterewardmile = 0;
                        //        //添加里程信息
                        //        if (invitetype == "1")
                        //        {
                        //            DataTable dt_5 = GetBaseDataValueByCodeid(facid, "5", "RewardMileType");
                        //            if (dt_5 != null && dt_5.Rows.Count > 0)
                        //            {
                        //                inviterewardmile = int.Parse(dt_5.Rows[0]["LOTTERYMOENY"].ToString());
                        //            }
                        //            param11 = AddUserMileNewParam(facid, inviteuserguid, "5", "", "", inviterewardmile, inviteguid);//里程信息添加

                        //        }
                        //        else
                        //        {
                        //            DataTable dt_6 = GetBaseDataValueByCodeid(facid, "6", "RewardMileType");
                        //            if (dt_6 != null && dt_6.Rows.Count > 0)
                        //            {
                        //                inviterewardmile = int.Parse(dt_6.Rows[0]["LOTTERYMOENY"].ToString());
                        //            }
                        //            param11 = AddUserMileNewParam(facid, inviteuserguid, "6", "", "", inviterewardmile, inviteguid);//里程信息添加
                        //        }

                        //        nRet11 = dbc.ExecuteNonQuery(CommandType.Text, SQL_AddUserMileNew, param11);

                        //        //回写邀请表数据
                        //        param12 = GetUpdateInviteByGuidParam(facid, inviteguid, guid3);
                        //        nRet12 = dbc.ExecuteNonQuery(CommandType.Text, SQL_UpdateInviteByGuid, param12);

                        //    }
                        //    else
                        //    {
                        //        nRet11 = 1;
                        //        nRet12 = 1;
                        //    }
                        //}
                        #endregion



                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8 + nRet9 + nRet10) >= 10)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeTXZ()记录抽奖日志 异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeTXZ()记录抽奖日志 异常-----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 18) 依据配置的注册信息字段进行注册


        /// <summary>
        /// 组织注册信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        private OracleParameter[] GetAddLotteryRegisterByDBConfigParam(string digitCode, string channel, string ip, string awardsno, string newcode, string facid, string[] dbconfigfieldlist, string[] smsmsglist)
        {
            OracleParameter[] param = null;
            try
            {

                //SPRO,CATEGORY,IP,LOTTERYLEVEL,STATE,FLAG,NEWCODE,FACID,VDATE
                #region 组织参数
                param = (OracleParameter[])ParameterCache.GetParams("GetAddLotteryRegisterByDBConfigParam" + facid);
                int paramcount = 8;
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[paramcount + dbconfigfieldlist.Length];

                    param[0] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                    param[1] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    param[2] = new OracleParameter(":IP", OracleType.VarChar, 30);
                    param[3] = new OracleParameter(":LOTTERYLEVEL", OracleType.VarChar, 1);
                    param[4] = new OracleParameter(":NEWCODE", OracleType.VarChar, 16);
                    param[5] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[6] = new OracleParameter(":VDATE", OracleType.DateTime);
                    param[7] = new OracleParameter(":GUID", OracleType.VarChar, 32);

                    for (int i = 0; i < dbconfigfieldlist.Length; i++)
                    {
                        param[paramcount + i] = new OracleParameter(":" + dbconfigfieldlist[i], OracleType.VarChar);
                    }

                    //将参数加入缓存
                    ParameterCache.PushCache("GetAddLotteryRegisterByDBConfigParam" + facid, param);
                }
                param[0].Value = digitCode;
                param[1].Value = channel;
                param[2].Value = ip;
                param[3].Value = awardsno;
                param[4].Value = newcode;
                param[5].Value = facid;
                param[6].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                param[7].Value = Guid.NewGuid().ToString().Replace("-", "");

                for (int i = 0; i < dbconfigfieldlist.Length; i++)
                {
                    param[paramcount + i].Value = smsmsglist[i];
                }

                #endregion

            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("18)GetAddLotteryRegisterByDBConfigParam 组织注册信息----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }


        /// <summary>
        /// 依据配置的注册信息字段进行注册
        /// </summary>
        /// <param name="digitCode">随机码</param>
        /// <param name="channel">渠道</param>
        /// <param name="ip">查询IP</param>
        /// <param name="awardsno">奖项</param>
        /// <param name="newcode">随机码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="dbconfigfieldlist">数据库配置信息字段</param>
        /// <param name="smsmsglist">输入的信息字段</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public int AddLotteryRegisterByDBConfig(string digitCode, string channel, string ip, string awardsno, string newcode, string facid, string[] dbconfigfieldlist, string[] smsmsglist, string tablename)
        {
            try
            {
                //组织配置字段拼接SQL
                string dbconfigParam = string.Join(",", dbconfigfieldlist);

                //组织输入值拼接SQL值
                string smsconfigValue = string.Join(",", smsmsglist);

                dbconfigParam = dbconfigParam.TrimEnd(',');  //将拼接的字段SQL截取最后一个,符号
                smsconfigValue = smsconfigValue.TrimEnd(',');

                //获取参数
                OracleParameter[] param2 = GetAddLotteryRegisterByDBConfigParam(digitCode, channel, ip, awardsno, newcode, facid, dbconfigfieldlist, smsmsglist);

                string table = GetTable(tablename, facid);
                string sql = ADD_LOTTERYREGISTER_SQL;
                sql = string.Format(ADD_LOTTERYREGISTER_SQL, table, dbconfigParam, ":" + dbconfigParam.Replace(",", ",:"));

                DataBase dataBase = new DataBase();
                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param2);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:AddLotteryRegisterByDBConfig:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return 0;
            }
        }

        #endregion

        #region 检查手机号码是否已经被注册过
        /// <summary>
        /// 检查手机号码是否已经被注册
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool GetLotteryRegisterByMobile(string mobile, string facid, string tableName)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryRegisterByMobileParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":IP", OracleType.VarChar, 30);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryRegisterByMobileParam", param);
                }
                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(tableName, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(FIND_REGISTERLOTTER_SQL, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryRegisterByMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion

        #region 检查数码是否被注册
        /// <summary>
        /// 检查数码是否被注册
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool GetLotteryCodeByCode(string digitcode, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryCodeByCodeParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryCodeByCodeParam", param);
                }
                param[0].Value = facid;
                param[1].Value = digitcode;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Log, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_CODE_SQL, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByCode:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 查看检查数码是否被发送手机被注册过
        /// <summary>
        /// 查看检查数码是否被发送手机被注册过
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="digigcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool GetLotteryCodeByCodeAndMobile(string mobile, string digigcode, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryCodeByCodeAndMobileParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                    param[2] = new OracleParameter(":MOBILE", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryCodeByCodeAndMobileParam", param);
                }
                param[0].Value = facid;
                param[1].Value = digigcode;
                param[2].Value = mobile;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Log, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_MOBILEANDCODE_SQL, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByCodeAndMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 查询手机号码是否进行过防伪查询
        /// <summary>
        /// 查询手机号码是否进行过防伪查询
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool GetLotteryCodeByMobile(string mobile, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryCodeByMobileParam2");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":USERID", OracleType.VarChar, 11);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetLotteryCodeByMobileParam2", param);
                }
                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Log, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_MOBILE_SQL, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 19) 添加中奖日志（针对红包抽奖活动）
        /// <summary>
        /// 添加抽奖日志（针对话费充值类抽奖活动）逦彩
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="money">中奖金额</param>
        /// <returns>是否成功</returns>
        public bool AddRedEnvelopeLottery(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string money)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;

            try
            {
                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno);
                param5 = sendPrewarningParam(3, facid, smscontent);

                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null)
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


                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);

                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);

                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5) >= 5)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("红包充值记录抽奖日志----" + ex.Message + "----" + ex.Source + "----" + ex.StackTrace + "----" + ex.TargetSite + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("话费充值记录抽奖日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 20) 根据检测用户是否存在 (马石油)
        /// <summary>
        ///  根据检测用户是否存在
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable CheckUserExist(string factoryid, string openid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckUserExist_M");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 64);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckUserExist_M", param);
                }
                param[0].Value = factoryid;
                param[1].Value = openid;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);

                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(CHECK_SHAKE_USERREGIST_SQL, table), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckUserExist:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
        #endregion

        #region 21 马石油

        #region 获取一个未使用的新京东E卡卡密

        /// <summary>
        /// 获取一个未使用的新京东E卡卡密
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="number">个数</param>
        /// <returns></returns>
        public bool GetNewJDECode(string facid, int number, out string ecode, out string epassword)
        {
            bool bRet = false;
            ecode = "";
            epassword = "";
            try
            {
                DataBase dataBase = new DataBase();

                #region 序列化参数

                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 8);
                param[1] = new OracleParameter(":CNUM", OracleType.Number, 2);

                param[0].Value = facid;
                param[1].Value = number.ToString();

                #endregion

                string sql = "SELECT TT.ECODE,TT.EPASSWORD FROM (SELECT ROWNUM RN,T.* FROM T_SGM_SHAKE_JDECODE T WHERE T.FACID=:FACID AND T.FLAG='1' ) TT WHERE TT.RN IN (SELECT TRUNC(DBMS_RANDOM.VALUE(1,(SELECT COUNT(*) FROM T_SGM_SHAKE_JDECODE WHERE FACID=:FACID AND FLAG='1'))) FROM DUAL CONNECT BY ROWNUM <:CNUM) ";

                DataTable jdcodelist = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (jdcodelist != null && jdcodelist.Rows.Count > 0)
                {
                    ecode = jdcodelist.Rows[0]["ECODE"].ToString();
                    epassword = jdcodelist.Rows[0]["EPASSWORD"].ToString();
                }

                if (!string.IsNullOrEmpty(ecode) && !string.IsNullOrEmpty(epassword))
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetNewJDECode----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 获取一个未使用的新京东E卡卡密

        /// <summary>
        /// 获取一个未使用的新京东E卡卡密
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="number">个数</param>
        /// <returns></returns>
        public bool GetNewJDECodeAtlas(string facid, int number, string level, out string ecode, out string epassword)
        {
            bool bRet = false;
            ecode = "";
            epassword = "";
            try
            {
                DataBase dataBase = new DataBase();

                #region 序列化参数

                OracleParameter[] param = null;
                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                param[1] = new OracleParameter(":CNUM", OracleType.Number);
                param[2] = new OracleParameter(":LOTTREYLEVEL", OracleType.VarChar);

                param[0].Value = facid;
                param[1].Value = number.ToString();
                param[2].Value = level;
                #endregion

                string sql = "SELECT TT.ECODE,TT.EPASSWORD FROM (SELECT ROWNUM RN,T.* FROM T_SGM_SHAKE_JDECODE T WHERE T.FACID=:FACID AND T.FLAG='1' AND T.LOTTREYLEVEL=:LOTTREYLEVEL ) TT WHERE TT.RN IN (SELECT TRUNC(DBMS_RANDOM.VALUE(1,(SELECT COUNT(*) FROM T_SGM_SHAKE_JDECODE WHERE FACID=:FACID AND FLAG='1' AND LOTTREYLEVEL = :LOTTREYLEVEL ))) FROM DUAL CONNECT BY ROWNUM <:CNUM) ";

                DataTable jdcodelist = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (jdcodelist != null && jdcodelist.Rows.Count > 0)
                {
                    ecode = jdcodelist.Rows[0]["ECODE"].ToString();
                    epassword = jdcodelist.Rows[0]["EPASSWORD"].ToString();
                }

                if (!string.IsNullOrEmpty(ecode) && !string.IsNullOrEmpty(epassword))
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetNewJDECode----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 修改京东E卡卡密状态和和对应的数码

        #endregion

        #endregion

        #region 21.2获取中奖数码信息 （通用版本）
        /// <summary>
        ///  根据factoryid、digitcode、mobile查询中奖核销信息
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        public DataTable GetLotteryCodeInfo(string factoryid, string digitcode)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryCodeInfo");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];

                    //param[0] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 32);
                    //param[1] = new OracleParameter(":FACID", OracleType.VarChar, 12);

                    param[0] = new OracleParameter(":DIGIT", OracleType.NVarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 20);


                    ParameterCache.PushCache("GetLotteryCodeInfo", param);
                }
                param[0].Value = digitcode;
                param[1].Value = factoryid;
                #endregion
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_LOTTERYLOG, factoryid);
                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(SQL_GETLOTTERYCODEINFO, table), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeInfo:" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }


        /// <summary>
        ///  根据factoryid、digitcode、mobile查询中奖核销信息
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        public DataTable GetLotteryCodeInfoNew(string factoryid, string digitcode)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetLotteryCodeInfoNew");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];

                    //param[0] = new OracleParameter(":DIGITCODE", OracleType.VarChar, 32);
                    //param[1] = new OracleParameter(":FACID", OracleType.VarChar, 12);

                    param[0] = new OracleParameter(":DIGIT", OracleType.NVarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 20);


                    ParameterCache.PushCache("GetLotteryCodeInfoNew", param);
                }
                param[0].Value = digitcode;
                param[1].Value = factoryid;
                #endregion
                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_LOTTERYLOG, factoryid);
                string table2 = GetTable(TableName_Signin, factoryid);
                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(SQL_GETLOTTERYCODEINFONew, table, table2), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeInfoNew:" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }


        #endregion

        #region 22) 根据检测用户是否存在 (曼牌)
        /// <summary>
        ///  根据检测用户是否存在
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable CheckUserExistByOpenid(string factoryid, string openid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckUserExist_M");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 64);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckUserExist_M", param);
                }
                param[0].Value = factoryid;
                param[1].Value = openid;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, factoryid);

                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(CHECK_SHAKE_USERREGIST_SQL, table), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckUserExistByOpenid:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
        #endregion

        #region 23) 添加中奖日志（针对实物，虚拟卡券类抽奖活动+信息收集） 曼牌
        /// <summary>
        /// 添加中奖日志（针对实物，虚拟卡券类抽奖活动+信息收集） 曼牌
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="cardnum">奖品对应的卡券号</param>       
        /// <param name="cardbrand">车型</param>
        ///<param name="cip">消费者IP</param>
        /// <param name="provincename">省份</param>
        /// <param name="cityname">城市</param>
        /// <param name="dealername">经销商名称</param>
        /// <param name="logisticscode">物流码</param>
        /// <param name="dealerid">经销商id</param>
        /// <param name="productname">产品名称</param>
        /// <param name="productid">产品id</param>
        /// <param name="openid">微信用户关注id</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeMannHummel(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registersql, bool isspcode, string cardnum, string cardbrand, string cip, string provincename, string cityname, string dealername, string logisticscode, string dealerid, string productname, string productid, string openid)
        {

            bool bRet = false;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param1 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param8 = null;

            OracleParameter[] param9 = null;


            try
            {

                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);  // 获取参与抽奖日志组织参数
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取真码抽奖日志记录组织参数
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取抽奖日志记录组织参数
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno); //更新奖池信息（LOTTERYTIMES 中奖数量更新）
                param5 = sendPrewarningParam(3, facid, smscontent);    // 获取预警信息发送参数
                //param6 = GetLotteryRechargeParameter(facid, ip, digitCode, result, area, area, "001", "001", money.ToString(), "140101", "0", guid3);

                string guid4 = Guid.NewGuid().ToString().Replace("-", "");

                param6 = GetConsumerEwardParam(guid4, digitCode, awardsno, ip, cardnum, cardbrand, facid, cip, provincename, cityname, dealername, logisticscode, dealerid, productname, productid, openid);  //获取核销记录组织参数  // UpdateLotteryJDKQParam(facid, cardnum, awardsno, digitCode);  //

                param8 = ModifySpCodeParam(facid, digitCode, ip);   //修改特殊抽奖数码数据


                param9 = UpdateAwardNumParam(facid, activityId, poolid);//修改参与总人数（totaltop+1）



                if (param2 != null && param3 != null && param1 != null && param4 != null && param5 != null && param6 != null && param8 != null)
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
                        int nRet8 = 0;
                        int nRet9 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //中奖充值记录+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);


                        //参与总人数加1
                        string table9 = GetTable(TableName_Rule, facid);
                        string sql9 = string.Format(UPDATE_SHAKE_RULE_SQL, table9);
                        nRet9 = dbc.ExecuteNonQuery(CommandType.Text, sql9, param9);



                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }

                        //新增核销记录表
                        if (!string.IsNullOrEmpty(cardnum))
                        {
                            string sql6 = string.Format(ADD_CONSUMEREWARD_SQL, TableName_Verification);
                            nRet6 = dbc.ExecuteNonQuery(CommandType.Text, sql6, param6);
                        }
                        else
                        {
                            nRet6 = 1;
                        }


                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet7 = 1;
                        }

                        if (isspcode)
                        {
                            nRet8 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param8);
                        }
                        else
                        {
                            nRet8 = 1;
                        }



                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8 + nRet9) >= 9)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeMannHummel()记录抽奖日志 异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeMannHummel()记录抽奖日志 异常-----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 24)添加参与,真码参与,中奖记录,注册信息,更新奖池信息,更新参与总人数，添加投保基础信息 (壳牌-太保项目)
        /// <summary>
        /// 添加中奖日志（针对实物，虚拟卡券类抽奖活动+信息收集） 曼牌
        /// </summary>
        /// <param name="guid1">参与记录表guid</param>
        /// <param name="guid2">参与真码记录表guid</param>
        /// <param name="guid3">中奖记录表guid</param>
        /// <param name="ip">ip/电话/手机号</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="area">所属城市</param>
        /// <param name="awardsno">中奖编码(0为未中奖)</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">抽奖答复</param>
        /// <param name="poolid">奖池ID</param>
        /// <param name="newcode">二次加密数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smscontent">预警短信内容</param>
        /// <param name="registersql">注册SQL</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="productid">产品编号</param> 
        /// <param name="productname">产品名称</param> 
        /// <param name="orderid">投保单号</param>       
        /// <param name="seriesid">产品系列</param>
        ///<param name="seriesname">产品系列名称</param>
        /// <param name="openid">微信用户关注id</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryRechargeTB(string guid1, string guid2, string guid3, string ip, string facid, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, string poolid, string newcode, bool smsflag, string smscontent, string registersql, bool isspcode, string productid, string productname, string orderid, string seriesid, string seriesname, string openid)
        {

            bool bRet = false;
            OracleParameter[] param1 = null;
            OracleParameter[] param2 = null;
            OracleParameter[] param3 = null;
            OracleParameter[] param4 = null;
            OracleParameter[] param5 = null;
            OracleParameter[] param6 = null;
            OracleParameter[] param7 = null;
            OracleParameter[] param8 = null;

            try
            {

                param1 = GetLotteryLogParam(guid1, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);  // 获取参与抽奖日志组织参数
                param2 = GetLotteryParLogParam(guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //获取真码抽奖日志记录组织参数
                param3 = GetNewLotteryParam(guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode); //中奖纪录表
                param4 = UpdateLotteryNumParam(facid, poolid, awardsno); //更新奖池信息（LOTTERYTIMES 中奖数量更新）
                param5 = sendPrewarningParam(3, facid, smscontent);    // 获取预警信息发送参数
                string guid4 = Guid.NewGuid().ToString().Replace("-", "");//投保信息表guid
                param6 = GetInsuranceParam(guid4, facid, digitCode, guid3, ip, orderid, seriesid, seriesname, openid, productid, productname);//投保信息
                param7 = ModifySpCodeParam(facid, digitCode, ip);   //修改特殊抽奖数码数据
                param8 = UpdateAwardNumParam(facid, activityId, poolid);//修改参与总人数（totaltop+1）


                if (param1 != null && param2 != null && param3 != null && param4 != null && param5 != null && param6 != null && param7 != null && param8 != null)
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
                        int nRet8 = 0;
                        int nRet9 = 0;

                        //增加参与日志
                        string table1 = GetTable(TableName_Log, facid);
                        string sql1 = string.Format(ADD_SHAKE_LOG_SQL, table1);
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);

                        //增加真码参与日志
                        string table2 = GetTable(TableName_PARLOG, facid);
                        string sql2 = string.Format(ADD_SHAKE_PARLOG_SQL, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        //增加中奖日志
                        string table3 = GetTable(TableName_LOTTERYLOG, facid);
                        string sql3 = string.Format(ADD_SHAKE_NEWLOTTERY_SQL, table3);
                        nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, param3);


                        //奖池信息+1
                        string table4 = GetTable(TableName_Pool, facid);
                        string sql4 = string.Format(UPDATE_SHAKE_POOL_SQL, table4);
                        nRet4 = dbc.ExecuteNonQuery(CommandType.Text, sql4, param4);


                        //预警消息提醒
                        if (smsflag)
                        {
                            string sql5 = string.Format(SEND_SMS_SQL, smscontent);
                            nRet5 = dbc.ExecuteNonQuery(CommandType.Text, sql5, param5);
                            nRet5 = 1;
                        }
                        else
                        {
                            nRet5 = 1;
                        }


                        //增加投保纪录
                        nRet6 = dbc.ExecuteNonQuery(CommandType.Text, SQL_ADDInsurance, param6);

                        //特殊数码
                        if (isspcode)
                        {
                            nRet7 = dbc.ExecuteNonQuery(CommandType.Text, "UPDATE T_SGM_STCODE S SET S.USE='1',S.MOBILE=:MOBILE,S.UPDATEDATE=SYSDATE WHERE S.CODE=:DIGIT AND S.FLAG='1' AND S.FACID=:FACID AND S.USE='0'", param7);
                        }
                        else
                        {
                            nRet7 = 1;
                        }


                        //参与总人数加1
                        string table8 = GetTable(TableName_Rule, facid);
                        string sql8 = string.Format(UPDATE_SHAKE_RULE_SQL, table8);
                        nRet8 = dbc.ExecuteNonQuery(CommandType.Text, sql8, param8);


                        //注册信息9
                        if (!string.IsNullOrEmpty(registersql))
                        {
                            nRet9 = dbc.ExecuteNonQuery(CommandType.Text, registersql, null);
                        }
                        else
                        {
                            nRet9 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3 + nRet4 + nRet5 + nRet6 + nRet7 + nRet8 + nRet9) >= 9)
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


                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeTB()记录抽奖日志 异常----" + ex.Message + "---" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddLotteryRechargeTB()记录抽奖日志 异常----" + ex.Message + "---" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }

        #endregion

        #region 查询券号是否已经存在（曼牌）
        /// <summary>
        ///  查询券号是否已经存在
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="cardnum">中奖券号</param>
        /// <returns></returns>
        public DataTable CheckCardNumExist(string factoryid, string cardnum)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckCardNumExist_M");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[1] = new OracleParameter(":CARDID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckCardNumExist_M", param);
                }
                param[0].Value = factoryid;
                param[1].Value = cardnum;
                #endregion

                DataBase dataBase = new DataBase();

                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(CHECK_CONSUMEREWARD_CARDID_SQL, TableName_Verification), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckCardNumExist:" + cardnum + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        #endregion

        #region 根据factoryid、digitcode、mobile查询中奖核销信息（曼牌）
        /// <summary>
        ///  根据factoryid、digitcode、mobile查询中奖核销信息
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="mobile">手机号</param>       
        /// <returns></returns>
        public DataTable QueryConsumeInfo(string factoryid, string digitcode)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("QueryConsumeInfo_M");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":DIGITALCODE", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    ParameterCache.PushCache("QueryConsumeInfo_M", param);
                }
                param[0].Value = digitcode;
                param[1].Value = factoryid;
                #endregion

                DataBase dataBase = new DataBase();

                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(SIGNIN_CONSUMEREWARD_SQL, TableName_Verification), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:QueryConsumeInfo:" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        #endregion

        #region 查询真码首次参与的时间（曼牌）
        /// <summary>
        ///  查询真码首次参与的时间
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        /// <returns></returns>
        public DataTable CheckDigitCodeFirstQueryDateTime(string factoryid, string digitcode)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckDigitCodeFirstQueryDateTime_M");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                    param[1] = new OracleParameter(":DIGIT", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckDigitCodeFirstQueryDateTime_M", param);
                }
                param[0].Value = factoryid;
                param[1].Value = digitcode;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_PARLOG, factoryid);
                DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(Query_SHAKE_PARLOG_FirstTime_SQL, table), param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckDigitCodeFirstQueryDateTime:" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        #endregion

        #region 根据openId、facId、cardId获取核销信息 曼牌
        /// <summary>
        /// 根据openId、facId、cardId获取核销信息 曼牌
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="openid">微信OPENID</param>
        /// <param name="cardid">奖券号</param>      
        /// <returns></returns>
        public DataTable GetConsumerEwardMann(string factoryid, string openid, string cardid)
        {
            DataTable dt = null;

            try
            {
                #region 组织参数
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetConsumerEwardMannt_M");
                //构造参数
                if (string.IsNullOrEmpty(cardid))
                {
                    if (param == null)
                    {
                        param = new OracleParameter[2];
                        param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                        param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 64);
                        //将参数加入缓存
                        ParameterCache.PushCache("GetConsumerEwardMann_M", param);
                    }
                    param[0].Value = factoryid;
                    param[1].Value = openid;

                    DataBase dataBase = new DataBase();

                    dt = dataBase.GetDataSet(CommandType.Text, string.Format(GET_CONSUMEREWARD_BY_OPENID_SQL1, TableName_Verification), param).Tables[0];


                    return dt;
                }
                else
                {
                    if (param == null)
                    {
                        param = new OracleParameter[3];
                        param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                        param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 64);
                        param[2] = new OracleParameter(":CARDID", OracleType.VarChar, 64);
                        //将参数加入缓存
                        ParameterCache.PushCache("GetConsumerEwardMann_M", param);
                    }
                    param[0].Value = factoryid;
                    param[1].Value = openid;
                    param[2].Value = cardid;


                    DataBase dataBase = new DataBase();

                    dt = dataBase.GetDataSet(CommandType.Text, string.Format(GET_CONSUMEREWARD_BY_OPENID_SQL2, TableName_Verification), param).Tables[0];
                    return dt;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetConsumerEwardMann:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
        #endregion

        #region 查询手机号码是否参与过活动
        /// <summary>
        /// 查询手机号码是否参与过活动
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByMobile(string mobile, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":USERID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_MOBILE_SQL2, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 查询手机号码当前月是否参与过活动
        /// <summary>
        /// 查询手机号码是否参与过活动
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckRecentHasRecordByMobile(string mobile, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":USERID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_MOBILE_SQL3, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 检查手机号码是否参与过活动且中奖 new  （20161020）
        /// <summary>
        /// 检查手机号码是否参与过活动且中奖
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public bool CheckMobileIsJoinActivity(string facid, string mobile, string activityid)
        {

            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":USERID", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = activityid;

                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_LOTTERYLOG, facid);


                //DataTable dt = dataBase.GetDataSet(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_MOBILE_SQL_A, table), param).Tables[0];

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_MOBILE_SQL_A, table), param);
                return row != null ? true : false;


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion

        #region 邀请好友
        public bool InviteFriendJoin(string factoryid, string from, string to, string lid, string code, out string systemcode)
        {
            systemcode = "00000";
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[5];
                param[0] = new OracleParameter(":GUID", OracleType.VarChar);
                param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar);
                param[2] = new OracleParameter(":INVITETO", OracleType.VarChar);
                param[3] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar);
                param[4] = new OracleParameter(":FACID", OracleType.VarChar);

                param[0].Value = Guid.NewGuid().ToString().Replace("-", "");
                param[1].Value = from;
                param[2].Value = to;
                param[3].Value = lid;
                param[4].Value = factoryid;
                #endregion
                DataBase dataBase = new DataBase();


                string sql = string.Format("SELECT x.lotteryguid,count(1) num FROM T_SGM_SHAKE_Invite X where x.lotteryguid='{0}' group by x.lotteryguid  ", lid);
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, null);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0]["NUM"]) >= 2)
                    {
                        systemcode = "003";
                        //已经邀请过了
                        return false;
                    }
                }
                bool c = dataBase.ExecuteNonQuery(CommandType.Text, INSERT_T_SGM_SHAKE_Invite, param) > 0;
                if (c)
                {
                    #region 根据数码和中将等级修改F14（是否邀请）字段
                    string table2 = GetTable(TableName_Signin, factoryid);
                    sql = string.Format("update {0} x set x.F14='1' where x.spro='{1}' and x.lotterylevel is not null and x.facid='{2}' and x.ip='{3}' and x.guid='{4}'", table2, code, factoryid, from, lid);
                    c = dataBase.ExecuteNonQuery(CommandType.Text, sql, null) > 0;
                    #endregion
                }
                return c;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByMobile:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        public bool InviteFriendJoin(string factoryid, string from, string to, string lid, string code)
        {

            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[5];
                param[0] = new OracleParameter(":GUID", OracleType.VarChar);
                param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar);
                param[2] = new OracleParameter(":INVITETO", OracleType.VarChar);
                param[3] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar);
                param[4] = new OracleParameter(":FACID", OracleType.VarChar);

                param[0].Value = Guid.NewGuid().ToString().Replace("-", "");
                param[1].Value = from;
                param[2].Value = to;
                param[3].Value = lid;
                param[4].Value = factoryid;
                #endregion
                DataBase dataBase = new DataBase();

                bRet = dataBase.ExecuteNonQuery(CommandType.Text, INSERT_T_SGM_SHAKE_Invite, param) > 0;


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:GetLotteryCodeByMobile:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion

        #region 检测数码是否已经邀请过好友

        /// <summary>
        /// 检测数码是否已经邀请过好友
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="from"></param>
        /// <param name="lid"></param>
        /// <returns></returns>
        public bool CheckCodeInvitedFriend(string factoryid, string from, string lid)
        {
            bool bRet = false;
            try
            {

                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":INVITEFROM", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar, 32);

                param[0].Value = factoryid;
                param[1].Value = from;
                param[2].Value = lid;

                #endregion
                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, QueryCodeIsInvitedFried, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:检测数码是否已经邀请过好友CheckCodeInvitedFriend() 异常【factoryid:" + factoryid + "】【from:" + from + "】【lid:" + lid + "】" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion


        #region 查询OPENID是否参与过活动
        /// <summary>
        /// 查询OPENID是否参与过活动
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="validTime">有效时间</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByOPENID(string OPENID, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 40);

                param[0].Value = facid;
                param[1].Value = OPENID;
                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_OPENID_SQL, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByOPENID:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 查询OPENID是否参与过活动
        /// <summary>
        /// 查询OPENID是否参与过活动
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public object CheckOpenidIsAuth(string OPENID, string facid, string validTime)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 40);

                param[0].Value = facid;
                param[1].Value = OPENID;
                #endregion

                DataBase dataBase = new DataBase();


                string sql = string.Format(CHECK_AUTH_LOG_BY_OPENID_SQL, validTime);

                object row = dataBase.ExecuteScalar(CommandType.Text, sql, param);

                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckOpenidIsAuth:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 查询OPENID是否参与过活动
        /// <summary>
        /// 查询OPENID是否参与过活动
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动ID</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByOPENID(string OPENID, string facid, string activityid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 40);
                param[2] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = OPENID;
                param[2].Value = activityid;

                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_OPENID_SQL_A, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByOPENID:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion


        #region 查询OPENID是否参与过活动
        /// <summary>
        /// 查询OPENID是否参与过活动
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动ID</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByOPENID2(string OPENID, string facid, string activityid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":F3", OracleType.VarChar, 40);
                param[2] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = OPENID;
                param[2].Value = activityid;

                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);

                object row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_SHAKE_LOG_BY_OPENID_SQL_B, table), param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByOPENID2:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion



        #region 查询手机号码当天参与此活动的情况
        /// <summary>
        /// 查询手机号码当天参与此活动的情况
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public object CheckLotteryCodeByMobileDay(string mobile, string facid)
        {
            object row;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                param[1] = new OracleParameter(":USERID", OracleType.VarChar);

                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion

                DataBase dataBase = new DataBase();

                row = dataBase.ExecuteScalar(CommandType.Text, CHECK_SHAKE_LOG_BY_MOBILEDAY_SQL2, param);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByMobileDay:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
        #endregion

        #region 针对超凡特殊校验
        /// <summary>
        /// 检查手机号码和验证码是否一次验证
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public object CheckVerifyCodeByMobile(string mobile, string verifyCode, string facid)
        {
            object row;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":VERIFCODE", OracleType.VarChar, 8);

                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = verifyCode;
                #endregion

                DataBase dataBase = new DataBase();

                row = dataBase.ExecuteScalar(CommandType.Text, GET_MOBILE_VERIFYCODE_SQL, param);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByMobileDay:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }


        /// <summary>
        /// 检查每日最大中奖次数是否达到上限
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="actid"></param>
        /// <returns></returns>
        public object CheckSumMaxLotteryDay(string facid, string actid)
        {
            object row;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":ACTIVITYID", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = actid;
                #endregion

                string table = GetTable(TableName_LOTTERYLOG, facid);

                DataBase dataBase = new DataBase();

                row = dataBase.ExecuteScalar(CommandType.Text, string.Format(CHECK_USER_LOTTERYLOG_DAY_SQL, table), param);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByMobileDay:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        /// <summary>
        /// 检查OPENID与提交的门店是否是异常情况
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public object CheckOpenidStore(string openid, string stroeid, string facid)
        {
            object row;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 36);
                param[2] = new OracleParameter(":STORE", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = openid;
                param[2].Value = stroeid;
                #endregion

                DataBase dataBase = new DataBase();

                row = dataBase.ExecuteScalar(CommandType.Text, CHECK_SHAKE_LOG_BY_OPENIDSTORE_SQL, param);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByMobileDay:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        /// <summary>
        /// 修改手机号码与验证码避免再次提交验证
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public bool ModifyVerifyCodeByMobile(string mobile, string verifyCode, string facid)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":VERIFCODE", OracleType.VarChar, 8);

                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = verifyCode;
                #endregion

                DataBase dataBase = new DataBase();

                int row = dataBase.ExecuteNonQuery(CommandType.Text, UPDATE_MOBILE_VERIFYCODE_SQL, param);
                return (row > 0) ? true : false;


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:ModifyVerifyCodeByMobile:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 校验手机验证码是否存在
        /// <summary>
        /// 校验手机验证码是否存在
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        public object CheckVerifyCodeIsExsit(string facid, string mobile, string verifyCode)
        {
            object row;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":VERIFCODE", OracleType.VarChar, 8);
                param[0].Value = facid;
                param[1].Value = mobile;
                param[2].Value = verifyCode;
                #endregion

                DataBase dataBase = new DataBase();

                row = dataBase.ExecuteScalar(CommandType.Text, GET_MOBILE_VERIFYCODE_SQL_A, param);
                return row;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckVerifyCodeIsExsit:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
        #endregion

        #region 判断门店是否可以参与活动
        /// <summary>
        /// 判断门店是否可以参与活动
        /// </summary>
        /// <param name="stroeID">门店编号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByStoreID(string stroeID, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":STOREID", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = stroeID;
                #endregion

                DataBase dataBase = new DataBase();

                object row = dataBase.ExecuteScalar(CommandType.Text, CHECK_SHAKE_LOG_BY_STORE_SQL, param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByStoreID:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 判断门店是否可以参与活动   喜力超凡
        /// <summary>
        /// 判断门店是否可以参与活动
        /// </summary>
        /// <param name="stroeID">门店编号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByStoreIDXLCF(string stroeID, string facid)
        {
            bool bRet = false;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":STOREID", OracleType.VarChar, 10);

                param[0].Value = facid;
                param[1].Value = stroeID;
                #endregion

                DataBase dataBase = new DataBase();

                object row = dataBase.ExecuteScalar(CommandType.Text, CHECK_SHAKE_LOG_BY_STORE_SQL, param);

                return row != null ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByStoreIDXLCF:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        /// <summary>
        ///  WT二期 查询门店中奖数量
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="facid"></param>
        /// <param name="lotterycount"></param>
        /// <returns></returns>
        public bool CheckHaveStoreLotteryCountWT(string storeID, string facid, out string lotterycount)
        {
            bool bRet = false;
            lotterycount = "0";
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                param[1] = new OracleParameter(":STOREID", OracleType.VarChar);

                param[0].Value = facid;
                param[1].Value = storeID;
                #endregion

                DataBase dataBase = new DataBase();

                object row = dataBase.ExecuteScalar(CommandType.Text, CHECK_STORE_LOTTERY_COUNT_WT, param);
                if (row != null)
                {
                    lotterycount = row.ToString();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckHaveStoreLotteryCountWT:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region
        /// <summary>
        /// 获得当前微信用户本次活动获奖数据集合
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable GetLotteryListByOpenid(string facid, string openid)
        {
            DataTable LotteryTable = null;
            try
            {
                #region 组织参数
                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);


                param[0].Value = facid;
                param[1].Value = openid;


                #endregion

                DataBase dataBase = new DataBase();
                string table = GetTable(TableName_Signin, facid);

                string user_sql = @"SELECT  U.GUID,U.IP,U.VDATE, B1.NAME,B1.SUBTYPE, S.CARDNUM,S.STATE,S.RUNDATE, F.RESULT,F.PACKNUM,F.RUNDATE, U.USER_NAME,U.USER_ADDRESS,U.USER_TELEPHONE FROM {0} U 
                                    LEFT JOIN t_sgm_wb_basedata_9999 B1 ON B1.CODEID=U.LOTTERYLEVEL AND  B1.DATATYPENAME='LotteryType' AND B1.FACID=U.FACID
                                    LEFT JOIN T_MOBILE_ONLINE_SEND S ON S.MOBILE=U.IP AND S.CARDNO=U.GUID   AND S.FACID=U.FACID
                                    LEFT JOIN T_MOBILE_FLOW_ONLINE_SEND F ON F.MOBILE=U.IP AND F.GUID=U.GUID  AND F.FACID=U.FACID
                                    WHERE U.FACID=:FACID AND U.OPENID=:OPENID  AND U.LOTTERYLEVEL IS NOT NULL AND U.LOTTERYLEVEL > 0
                                    ORDER BY U.VDATE DESC";

                LotteryTable = dataBase.GetDataSet(CommandType.Text, string.Format(user_sql, table), param).Tables[0];


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:CheckLotteryCodeByOPENID2:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return LotteryTable;
            }
            return LotteryTable;
        }
        #endregion


        #region 获取t_sgm_wb_basedata_9999中的数据
        /// <summary>
        /// 获取t_sgm_wb_basedata_9999中的数据
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="datatypename"></param>
        /// <returns></returns>
        public string GetBaseDataValue(string facid, string datatypename)
        {
            string flag = "0";
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetBaseDataValueParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetBaseDataValueParam", param);
                }
                param[0].Value = facid;
                param[1].Value = datatypename;

                DataBase dataBase = new DataBase();
                flag = dataBase.ExecuteScalar(CommandType.Text, GetBaseDataValue_sql, param).ToString();
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetBaseDataValue:" + facid + "---" + datatypename + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion


        #region 获取t_sgm_wb_basedata_9999中的数据2
        /// <summary>
        /// 获取t_sgm_wb_basedata_9999中的数据2
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="datatypename"></param>
        /// <returns></returns>
        public DataTable GetBaseDataValueByCodeid(string facid, string codeid, string datatypename)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetBaseDataValueByCodeid");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 32);
                    param[2] = new OracleParameter(":CODEID", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetBaseDataValueByCodeid", param);
                }
                param[0].Value = facid;
                param[1].Value = datatypename;
                param[2].Value = codeid;


                DataBase dataBase = new DataBase();

                dt = dataBase.ExecuteQuery(CommandType.Text, GetBaseDataValue_sql2, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:GetBaseDataValueByCodeid:" + facid + "---" + datatypename + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return dt;
        }
        #endregion



        /// <summary>
        /// 查询当前抽奖话费的总金额和当天总金额
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="bTime"></param>
        /// <param name="eTime"></param>
        /// <param name="LotterySum"></param>
        /// <param name="curDaySum"></param>
        /// <returns></returns>
        public bool QueryLotteryTotalCount(string facid, string bTime, string eTime, out string LotterySum, out string curDaySum)
        {
            object o = null;
            LotterySum = "0";
            curDaySum = "0";
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("QueryLotteryTotalCountParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("QueryLotteryTotalCountParam", param);
                }
                param[0].Value = facid;
                DataBase dataBase = new DataBase();
                o = dataBase.ExecuteScalar(CommandType.Text, GetLotteryTotal_SQL, param);
                if (o == null)
                {
                    return false;
                }
                else
                {
                    LotterySum = o.ToString();
                }

                o = null;
                o = dataBase.ExecuteScalar(CommandType.Text, string.Format(GetLotteryCurDay_SQL, bTime, eTime), param);
                if (o == null)
                {
                    return false;
                }
                else
                {
                    curDaySum = o.ToString();
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:QueryLotteryTotalCount:" + facid + "---" + bTime + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取红包总金额和当前总金额
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="bTime"></param>
        /// <param name="eTime"></param>
        /// <param name="RedSum"></param>
        /// <param name="curDaySum"></param>
        /// <returns></returns>
        public bool QueryRedTotalCount(string facid, string bTime, string eTime, out string RedSum, out string curDaySum)
        {
            object o = null;
            RedSum = "0";
            curDaySum = "0";
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("QueryRedTotalCountParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("QueryRedTotalCountParam", param);
                }
                param[0].Value = facid;
                DataBase dataBase = new DataBase();
                o = dataBase.ExecuteScalar(CommandType.Text, GetRedTotal_SQL, param);
                if (o == null)
                {
                    return false;
                }
                else
                {
                    RedSum = o.ToString();
                }

                o = null;
                o = dataBase.ExecuteScalar(CommandType.Text, string.Format(GetRedCurDay_SQL, bTime, eTime), param);
                if (o == null)
                {
                    return false;
                }
                else
                {
                    curDaySum = o.ToString();
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlDao:QueryRedTotalCount:" + facid + "---" + bTime + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return true;
        }


        string QueryCurrentDayCount_SQL = @"SELECT * FROM {0} X WHERE X.FACID='{1}' AND  to_char(sysdate, 'YYYY-MM-DD')=to_char(X.CREATEDAE, 'YYYY-MM-DD') AND X.AWARDSNO in ({2}) ";
        public int QueryCurrentDayCount(string facid, string awordno)
        {
            DataBase dataBase = new DataBase();
            string table = GetTable(TableName_LOTTERYLOG, facid);
            string sql = string.Format(QueryCurrentDayCount_SQL, table, facid, awordno);
            DataTable dtRet = dataBase.ExecuteQuery(CommandType.Text, sql, null);
            if (dtRet != null && dtRet.Rows != null && dtRet.Rows.Count > 0)
            {
                return dtRet.Rows.Count;
            }

            return 0;
        }

        private const string SELECT_REGISTER_JOINCOUNT_SQL = "SELECT count(1) joincount FROM {0} X WHERE X.FACID=:FACID AND X.IP=:IP AND to_char(sysdate, 'YYYY-MM-DD')=to_char(X.VDATE, 'YYYY-MM-DD') AND X.LOTTERYLEVEL IS NOT NULL ";
        public int SelectMobileJoinNum(string facid, string mobile)
        {
            string table = GetTable(TableName_Signin, facid);

            string sql = string.Format(SELECT_REGISTER_JOINCOUNT_SQL, table);

            OracleParameter[] param = new OracleParameter[2];
            param[0] = new OracleParameter(":FACID", OracleType.VarChar);
            param[1] = new OracleParameter(":IP", OracleType.VarChar);

            param[0].Value = facid;
            param[1].Value = mobile;

            DataBase dataBase = new DataBase();
            object o = dataBase.ExecuteScalar(CommandType.Text, sql, param);

            if (o != null)
            {
                return Convert.ToInt32(o);
            }
            return 0;
        }



        private const string SELECT_REGISTER_JOINCOUNT_SQL2 = "SELECT count(1) joincount FROM {0} X WHERE X.FACID=:FACID AND X.OPENID=:OPENID AND to_char(sysdate, 'YYYY-MM-DD')=to_char(X.VDATE, 'YYYY-MM-DD') AND X.LOTTERYLEVEL IS NOT NULL ";
        public int SelectOpenidJoinNum(string facid, string openid)
        {
            string table = GetTable(TableName_Signin, facid);

            string sql = string.Format(SELECT_REGISTER_JOINCOUNT_SQL2, table);

            OracleParameter[] param = new OracleParameter[2];
            param[0] = new OracleParameter(":FACID", OracleType.VarChar);
            param[1] = new OracleParameter(":OPENID", OracleType.VarChar);

            param[0].Value = facid;
            param[1].Value = openid;

            DataBase dataBase = new DataBase();
            object o = dataBase.ExecuteScalar(CommandType.Text, sql, param);

            if (o != null)
            {
                return Convert.ToInt32(o);
            }
            return 0;
        }



        public bool AddRegisterLog(string registersql)
        {
            bool bRet = false;
            try
            {
                DataBase dataBase = new DataBase();

                return dataBase.ExecuteNonQuery(CommandType.Text, registersql, null) > 0;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryDal:AddRegisterLog:" + registersql + "---", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        private const string SELECT_REGISTER = "SELECT * FROM {0} X WHERE X.FACID='{1}' ";
        public DataTable SelectRegister(string facid, string where)
        {

            DataBase dataBase = new DataBase();
            string table = GetTable(TableName_Signin, facid);

            string sql = string.Format(SELECT_REGISTER, table, facid);

            if (!string.IsNullOrEmpty(where))
            {
                sql += where;
            }

            return dataBase.ExecuteQuery(CommandType.Text, sql, null);
        }


        private const string SELECT_REGISTER_SUM = "SELECT sum({2}) FROM {0} X WHERE X.FACID='{1}' ";
        public string SelectRegister(string facid, string where, string field)
        {

            DataBase dataBase = new DataBase();
            string table = GetTable(TableName_Signin, facid);

            string sql = string.Format(SELECT_REGISTER_SUM, table, facid, field);

            if (!string.IsNullOrEmpty(where))
            {
                sql += where;
            }

            object o = dataBase.ExecuteScalar(CommandType.Text, sql, null);
            if (o != null)
            {
                return o.ToString();
            }

            return "0";

        }


        #region 司能卡券发放更新
        /// <summary>
        /// 司能卡券发放更新
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userid">员工编号</param>
        /// <param name="username">员工姓名</param>
        /// <param name="usermobile">员工手机</param>
        /// <param name="cardid">卡券号</param>
        /// <param name="userHash">好友微信信息</param>
        /// <returns></returns>
        public bool AddCardInfoAndRegist(string facid, string userid, string username, string usermobile, string cardid, Hashtable userHash)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] param1 = null;
                OracleParameter[] param2 = null;


                param1 = GetCardAddParam(facid, cardid, userid, username, usermobile, userHash);//卡券信息添加
                param2 = GetRegistAddParam(facid, userHash);//用户信息添加

                if (param1 != null && param2 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;

                        //卡券信息添加
                        nRet1 = dbc.ExecuteNonQuery(CommandType.Text, SQL_AddCardInfo, param1);

                        //用户信息添加
                        string table2 = GetTable(TableName_User, facid);
                        string sql2 = string.Format(SQL_AddUserRegistInfo, table2);
                        nRet2 = dbc.ExecuteNonQuery(CommandType.Text, sql2, param2);

                        if ((nRet1 + nRet2) >= 2)
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

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddCardInfoAndRegist()卡券发放日志 异常----" + ex.Message + "---" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

                        dbc.RollBackTrans();
                        throw ex;
                    }
                }


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.AddCardInfoAndRegist()卡券发放日志 异常----" + ex.Message + "---" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }

            return bRet;
        }
        #endregion

        #region 添加预约数据
        /// <summary>
        /// 添加预约数据 
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="mobile">预约手机号码</param>
        /// <param name="channel">预约渠道</param>
        /// <param name="username">预约姓名</param>
        /// <param name="storeid">门店id</param>
        /// <param name="openid">预约人的openid</param>
        /// <param name="f1">推荐人的openid</param>
        /// <returns></returns>
        public bool AddReserve(string factoryid, string mobile, string channel, string username, string storeid, string openid, string f1)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] param = null;

                //构造参数

                param = (OracleParameter[])ParameterCache.GetParams("AddReserve");
                if (param == null)
                {
                    param = new OracleParameter[8];
                    param[0] = new OracleParameter(":RESERVEGUID", OracleType.VarChar, 32);
                    param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                    param[2] = new OracleParameter(":CHANNEL", OracleType.VarChar, 2);
                    param[3] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[4] = new OracleParameter(":USERNAME", OracleType.VarChar, 40);
                    param[5] = new OracleParameter(":STOREID", OracleType.VarChar, 20);
                    param[6] = new OracleParameter(":OPENID", OracleType.VarChar, 64);
                    param[7] = new OracleParameter(":F1", OracleType.VarChar, 64);
                    //将参数加入缓存
                    ParameterCache.PushCache("AddReserve", param);
                }

                param[0].Value = Guid.NewGuid().ToString().Replace(" ", "");
                param[1].Value = mobile;
                param[2].Value = channel;
                param[3].Value = factoryid;
                param[4].Value = username;
                param[5].Value = storeid;
                param[6].Value = openid;
                param[7].Value = f1;
                DataBase dataBase = new DataBase();

                int i = dataBase.ExecuteNonQuery(CommandType.Text, SQL_AddReserveInfo, param);
                if (i > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  AddReserve 参数[factoryid:" + factoryid + "][mobile:" + mobile + "][channel:" + channel + "][username:" + username + "][storeid:" + storeid + "][openid:" + openid + "][f1:" + f1 + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }

            return bRet;

        }



        #endregion

        #region 添加用户注册
        /// <summary>
        /// 添加用户注册
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="registersql"></param>
        /// <returns></returns>
        public bool UserRegist(string facid, string registersql)
        {
            bool bRet = false;
            try
            {
                DataBase dataBase = new DataBase();
                int i = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  UserRegist 参数[facid:" + facid + "][registersql:" + registersql + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }
        #endregion

        #region 更新用户信息
        /// <summary>
        ///  更新用户信息
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="userguid"></param>
        /// <param name="firstcomplete">是否首次资料补全</param>
        /// <param name="isreward">是否添加补全奖励</param>
        /// <param name="mile"></param>
        /// <param name="isupdatebq">是否更新补全字段</param>
        /// <returns></returns>
        public bool UpdateUserInfo(string factoryid, string userguid, Hashtable userHash, bool firstcomplete, bool isreward, int mile, bool isupdatebq)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] param1 = null;
                OracleParameter[] param2 = null;
                param1 = GetUserUpdateParam(factoryid, userguid, userHash);//更新用户信息
                param2 = AddUserMileNewParam(factoryid, userguid, "7", "", "", mile, "");//里程信息添加

                if (param1 != null && param2 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;
                        if (firstcomplete)//首次补全资料
                        {
                            // SQL_UpdateUserInfo
                            if (isupdatebq)
                            {
                                string table1 = GetTable(TableName_User, factoryid);
                                string sql1 = string.Format(SQL_UpdateUserInfo, table1);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);
                            }
                            else
                            {
                                string table1 = GetTable(TableName_User, factoryid);
                                string sql1 = string.Format(SQL_UpdateUserInfo2, table1);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);
                            }
                        }
                        else
                        {
                            string table1 = GetTable(TableName_User, factoryid);
                            string sql1 = string.Format(SQL_UpdateUserInfo2, table1);
                            nRet1 = dbc.ExecuteNonQuery(CommandType.Text, sql1, param1);
                        }

                        if (isreward)
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, SQL_AddUserMileNew, param2);
                            //更新用户总里程数
                            string sql_updatemile = "update  {0} xx set xx.pointtotal=pointtotal+" + mile + "  where xx.facid='" + factoryid + "' and  xx.userguid='" + userguid + "' and  xx.deleteflag='1'";
                            string table3 = GetTable(TableName_User, factoryid);
                            string sql3 = string.Format(sql_updatemile, table3);
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, null);
                        }
                        else
                        {
                            nRet2 = 1;
                            nRet3 = 1;
                        }

                        if ((nRet1 + nRet2 + nRet3) >= 3)
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
                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.lotteryDal.UpdateUserInfo()用户补全资料 日志 异常----" + ex.Message + "---" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.LotteryDal.cs  UpdateUserInfo() 用户补全资料 facid:" + factoryid + "] [userguid:" + userguid + "][userHash:" + userHash + "][firstcomplete:" + firstcomplete + "][isreward:" + isreward + "][mile:" + mile + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;

        }
        #endregion

        #region 邀请好友
        /// <summary>
        /// 邀请好友
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile">邀请人</param>
        /// <param name="userguid">邀请人</param>
        /// <param name="mile">赠送里程</param>
        /// <param name="inviyetype">邀请类型</param>
        /// <param name="mobilelists">被邀请人列表</param>
        /// <returns></returns>
        public bool IinviteFriend(string facid, string mobile, string userguid, int mile, string inviyetype, List<MobileInfo> mobilelists)
        {
            bool bRet = false;
            try
            {

                string sql_addinvite = "";
                DataBase dbc = new DataBase();
                dbc.BeginTrans();
                int i = 0;
                Random rd = new Random();
                string randnum = rd.Next(10000, 99999).ToString();
                string groupid = mobile + randnum;
                foreach (MobileInfo item in mobilelists)
                {
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    sql_addinvite = "insert into T_SGM_SHAKE_Invite (guid,Invitefrom,Inviteto,Facid,Invitetype,groupid)values('" + guid + "','" + mobile + "','" + item.Mobile + "','" + facid + "','" + inviyetype + "','" + groupid + "')";

                    int nRet = dbc.ExecuteNonQuery(CommandType.Text, sql_addinvite, null);
                    if (nRet > 0)
                    {
                        i++;
                    }
                }

                //添加里程信息
                OracleParameter[] param2 = null;
                if (inviyetype == "0")
                {
                    // param2 = GetAddMileParam(facid, userguid, "4", mile);//里程信息添加
                    param2 = AddUserMileNewParam(facid, userguid, "4", "", "", mile, "");//里程信息添加
                }
                else
                {
                    // param2 = GetAddMileParam(facid, userguid, "3", mile);//里程信息添加
                    param2 = AddUserMileNewParam(facid, userguid, "3", "", "", mile, "");//里程信息添加
                }

                int nRet2 = dbc.ExecuteNonQuery(CommandType.Text, SQL_AddUserMileNew, param2);

                int nRet3 = 0;
                string sql_updatemile = "";
                if (inviyetype == "0")
                {
                    //减少扫码邀请扫码次数,并修改总里程数
                    //更新用户总里程数
                    sql_updatemile = "update  {0} xx set xx.pointtotal=pointtotal+" + mile + " , xx.f5=f5-1  where xx.facid='" + facid + "' and  xx.userguid='" + userguid + "' and  xx.deleteflag='1'";
                }
                else
                {
                    //修改总里程数
                    sql_updatemile = "update  {0} xx set xx.pointtotal=pointtotal+" + mile + "  where xx.facid='" + facid + "' and  xx.userguid='" + userguid + "' and  xx.deleteflag='1'";
                }

                string table3 = GetTable(TableName_User, facid);
                string sql3 = string.Format(sql_updatemile, table3);
                nRet3 = dbc.ExecuteNonQuery(CommandType.Text, sql3, null);

                if ((i + nRet2 + nRet3) >= (mobilelists.Count + 2))
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
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.LotteryDal.cs  IinviteFriend [facid:" + facid + "] [mobile:" + mobile + "][inviyetype:" + inviyetype + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion


        #region 根据手机号和厂家编号获取用户信息
        /// <summary>
        /// 根据手机号和厂家编号获取用户信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataTable GetUserBaseInfo(string facid, string mobile)
        {
            DataTable dt = null;
            try
            {

                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion
                DataBase dataBase = new DataBase();

                string table1 = GetTable(TableName_User, facid);
                string sql1 = string.Format(SQL_GetUserBaseInfo, table1);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql1, param);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  GetUserBaseInfo 参数[facid:" + facid + "][mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return dt;
        }
        #endregion


        #region 校验账号是否被邀请
        /// <summary>
        /// 校验账号是否被邀请
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckIivited(string facid, string mobile, out DataTable dt)
        {
            dt = null;
            bool bRet = false;
            try
            {

                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":INVITETO", OracleType.VarChar, 50);
                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion
                DataBase dataBase = new DataBase();
                dt = dataBase.ExecuteQuery(CommandType.Text, SQL_GetInviteInfoByInviteto, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  CheckIivited 参数[facid:" + facid + "][mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion


        #region 根据groupid 检测组中，是否有人中奖，已经给邀请人发放过奖励
        /// <summary>
        ///根据groupid 检测组中，是否有人中奖，已经给邀请人发放过奖励
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckInviteByGroupid(string facid, string groupid)
        {

            bool bRet = false;
            try
            {

                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":GROUPID", OracleType.VarChar, 16);
                param[0].Value = facid;
                param[1].Value = groupid;
                #endregion
                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, SQL_GetInviteInfoByGroupid, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  CheckInviteByGroupid 参数[facid:" + facid + "][groupid:" + groupid + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }

        #endregion


        #region 获取邀请人信息，给邀请人发放奖励
        /// <summary>
        /// 获取邀请人信息，给邀请人发放奖励
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataTable GetInviteFromByTo(string facid, string mobile)
        {
            DataTable dt = null;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 12);
                param[1] = new OracleParameter(":INVITETO", OracleType.VarChar, 50);
                param[0].Value = facid;
                param[1].Value = mobile;
                #endregion
                DataBase dataBase = new DataBase();

                string table1 = GetTable(TableName_User, facid);
                string sql = string.Format(SQL_GetInviteInfoByInviteto2, table1);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  GetInviteFromByTo 参数[facid:" + facid + "][mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return dt;
        }

        #endregion

        #region 添加邀请奖励信息
        /// <summary>
        /// 添加邀请奖励信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="inviteuserguid">奖励人的userguid</param>
        /// <param name="rewardtype">奖励类型</param>
        /// <param name="code">数码</param>
        /// <param name="lid">中奖guid</param>
        /// <param name="inviterewardmile">里程</param>
        /// <param name="inviteguid">邀请表guid</param>
        /// <returns></returns>
        public bool AddUserMileNew(string facid, string inviteuserguid, string rewardtype, string code, string lid, int inviterewardmile, string inviteguid)
        {

            bool bRet = false;
            try
            {

                OracleParameter[] param = AddUserMileNewParam(facid, inviteuserguid, rewardtype, code, lid, inviterewardmile, inviteguid);//里程信息添加
                DataBase dataBase = new DataBase();
                int i = dataBase.ExecuteNonQuery(CommandType.Text, SQL_AddUserMileNew, param);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {

                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  AddUserMileNew 参数[facid:" + facid + "][inviteuserguid:" + inviteuserguid + "][rewardtype:" + rewardtype + "][code:" + code + "][lid:" + lid + "][inviterewardmile:" + inviterewardmile + "][inviteguid:" + inviteguid + "][]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }
        #endregion

        #region 添加话费信息
        /// <summary>
        /// 添加话费信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="orderid"></param>
        /// <param name="notes"></param>
        /// <param name="citycode"></param>
        /// <param name="cityname"></param>
        /// <param name="statecode"></param>
        /// <param name="statename"></param>
        /// <param name="bill"></param>
        /// <param name="cardid"></param>
        /// <param name="resultid"></param>
        /// <param name="lotteryguid"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool AddMobileBill(string facid, string mobile, string orderid, string notes, string citycode, string cityname, string statecode, string statename, string bill, string cardid, string resultid, string lotteryguid, string flag)
        {
            bool bRet = false;

            try
            {
                OracleParameter[] param = GetLotteryRechargeParameter(facid, mobile, orderid, notes, citycode, cityname, statecode, statename, bill.ToString(), cardid, resultid, lotteryguid);
                DataBase dataBase = new DataBase();
                int i = 0;
                if (flag == "0")
                {
                    i = dataBase.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW_BAK, param);
                }
                else
                {
                    i = dataBase.ExecuteNonQuery(CommandType.Text, Insert_T_MOBILE_ONLINE_SEND_NEW, param);
                }

                if (i > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  AddMobileBill 参数[facid:" + facid + "][mobile:" + mobile + "][orderid:" + orderid + "][notes:" + notes + "][citycode:" + citycode + "][cityname:" + cityname + "][statecode:" + statecode + "][statename:" + statename + "] [bill:" + bill + "][cardid:" + cardid + "] [resultid:" + resultid + "][lotteryguid:" + lotteryguid + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }
        #endregion

        #region 获取抽奖答复信息
        /// <summary>
        /// 获取抽奖答复信息
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="replayid"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string GetShakeReplay(string factoryid, string replayid, string channel)
        {
            string replay = "";
            try
            {

                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[3];

                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 6);
                param[1] = new OracleParameter(":REPLYID", OracleType.VarChar, 20);
                param[2] = new OracleParameter(":CHANNEL", OracleType.VarChar, 2);

                param[0].Value = factoryid;
                param[1].Value = replayid;
                param[2].Value = channel;

                string sql = "SELECT   r.reply  FROM T_SGM_SHAKE_REPLY R WHERE r.facid=:FACID and  r.channel=:CHANNEL  AND  R.REPLYID=:REPLYID    AND    R.DELETEFLAG='1'";
                DataBase dataBase = new DataBase();
                DataTable DT = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (DT != null && DT.Rows.Count > 0)
                {
                    replay = DT.Rows[0][0].ToString();

                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  GetShakeReplay 参数[factoryid:" + factoryid + "][replayid:" + replayid + "][channel:" + channel + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                replay = "";
            }
            return replay;

        }
        #endregion


        #region 邀请表中回写数据
        /// <summary>
        /// 邀请表中回写数据
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="inviteguid"></param>
        /// <param name="lid"></param>
        /// <returns></returns>
        public bool UpdateInviteLotteryInfo(string facid, string inviteguid, string lid)
        {
            bool bRet = false;
            try
            {

                OracleParameter[] param = GetUpdateInviteByGuidParam(facid, inviteguid, lid);
                DataBase dataBase = new DataBase();
                int i = dataBase.ExecuteNonQuery(CommandType.Text, SQL_UpdateInviteByGuid, param);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {

                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  UpdateInviteLotteryInfo 参数[facid:" + facid + "][inviteguid:" + inviteguid + "][lid:" + lid + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }

        #endregion


        #region 壳牌工业油使用
        private const string INSERT_SGM_USER = @"INSERT INTO T_SGM_USER_9999 X(x.userguid,X.openid,x.pointtotal,x.pointvalid,x.pointused,x.mobile,X.FACID) VALUES (:userguid,:openid,:pointtotal,:pointvalid,:pointused,:mobile,:facid)";

        private const string SELECT_SGM_USER = @"SELECT x.userguid,X.openid,x.pointtotal,x.pointvalid,x.pointused,x.mobile,X.FACID, X.ROWID FROM T_SGM_USER_9999 X WHERE X.FACID=:facid and x.mobile=:mobile ";

        private const string UPDATE_SGM_USER = @"UPDATE T_SGM_USER_9999 X SET X.pointtotal=X.pointtotal + :pointval,X.pointvalid=X.pointvalid + :pointval where x.facid=:facid and x.mobile=:mobile ";

        private const string UPDATE_SGM_USER2 = @" UPDATE T_SGM_USER_9999 X SET X.POINTVALID=X.POINTTOTAL-:POINTNEED-X.Pointused,X.Pointused=X.Pointused+:POINTNEED WHERE X.FACID=:facid AND X.MOBILE=:mobile ";

        private const string INSERT_SGM_DHDETAIL = @"INSERT INTO T_SGM_DHDETAIL_9999 X (X.dhdetailguid, X.STOREMOBILE,X.POSTSTORE, X.POSTADDRESS ,x.giftname,x.dhnum,x.giftpoint,x.facid,X.F1) VALUES (:dhdetailguid,:mobile,:username,:postaddress,:giftname,1,:giftpoint,:facid,:f1) ";

        public bool InsertT_sgm_user(string facid, Hashtable userHsah)
        {
            OracleParameter[] param = null;
            bool bRet = false;
            try
            {
                param = (OracleParameter[])ParameterCache.GetParams("InsertT_sgm_userParam");
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":userguid", OracleType.VarChar);
                    param[1] = new OracleParameter(":openid", OracleType.VarChar);
                    param[2] = new OracleParameter(":pointtotal", OracleType.Number);
                    param[3] = new OracleParameter(":pointvalid", OracleType.Number);
                    param[4] = new OracleParameter(":pointused", OracleType.Number);
                    param[5] = new OracleParameter(":mobile", OracleType.VarChar);
                    param[6] = new OracleParameter(":facid", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("InsertT_sgm_userParam", param);
                }
                param[0].Value = Guid.NewGuid().ToString().Replace("-", "");
                param[1].Value = userHsah["openid"];
                param[2].Value = userHsah["pointtotal"];
                param[3].Value = userHsah["pointvalid"];
                param[4].Value = userHsah["pointused"];
                param[5].Value = userHsah["mobile"];
                param[6].Value = userHsah["facid"];

                DataBase dataBase = new DataBase();
                int i = dataBase.ExecuteNonQuery(CommandType.Text, INSERT_SGM_USER, param);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  InsertT_sgm_user 参数[facid:" + facid + "][userHsah:" + userHsah + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
            }

            return bRet;
        }

        public DataTable SelectT_sgm_user(string facid, string mobile)
        {
            OracleParameter[] param = null;
            DataTable dtRet = null;
            try
            {
                param = (OracleParameter[])ParameterCache.GetParams("SelectT_sgm_userParam");
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":facid", OracleType.VarChar);
                    param[1] = new OracleParameter(":mobile", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("SelectT_sgm_userParam", param);
                }
                param[0].Value = facid;
                param[1].Value = mobile;

                DataBase dataBase = new DataBase();
                dtRet = dataBase.ExecuteQuery(CommandType.Text, SELECT_SGM_USER, param);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  SelectT_sgm_user 参数[facid:" + facid + "][mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
            }


            return dtRet;
        }

        public bool UpdateT_sgm_user(string facid, Hashtable userHsah)
        {
            OracleParameter[] param = null;
            bool bRet = false;
            try
            {
                param = (OracleParameter[])ParameterCache.GetParams("UpdateT_sgm_userParam");
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":pointval", OracleType.Number);
                    param[1] = new OracleParameter(":mobile", OracleType.VarChar);
                    param[2] = new OracleParameter(":facid", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("UpdateT_sgm_userParam", param);
                }
                param[0].Value = userHsah["pointval"];
                param[1].Value = userHsah["mobile"];
                param[2].Value = facid;

                DataBase dataBase = new DataBase();
                int i = dataBase.ExecuteNonQuery(CommandType.Text, UPDATE_SGM_USER, param);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  UpdateT_sgm_user 参数[facid:" + facid + "][userHsah:" + userHsah + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
            }

            return bRet;
        }

        public bool AddPostAddressAndModifyUser(Hashtable postHash, string facid, string point, string mobile, DataTable dtU, string giftname)
        {
            bool bRet = false;
            DataBase dataBase = new DataBase();
            try
            {
                OracleParameter[] param = null;
                param = (OracleParameter[])ParameterCache.GetParams("UpdateT_sgm_userParam1");
                if (param == null)
                {
                    param = new OracleParameter[3];
                    param[0] = new OracleParameter(":POINTNEED", OracleType.Number);
                    param[1] = new OracleParameter(":mobile", OracleType.VarChar);
                    param[2] = new OracleParameter(":facid", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("UpdateT_sgm_userParam1", param);
                }
                param[0].Value = point;
                param[1].Value = mobile;
                param[2].Value = facid;

                dataBase.BeginTrans();
                int iRet1 = dataBase.ExecuteNonQuery(CommandType.Text, UPDATE_SGM_USER2, param);


                //(:userguid,:mobile,:username,:postaddress,:giftname,1,:giftpoint,:facid) 
                param = (OracleParameter[])ParameterCache.GetParams("InsertT_sgm_DHDETAIL");
                if (param == null)
                {
                    param = new OracleParameter[8];
                    param[0] = new OracleParameter(":dhdetailguid", OracleType.VarChar);
                    param[1] = new OracleParameter(":mobile", OracleType.VarChar);
                    param[2] = new OracleParameter(":username", OracleType.VarChar);
                    param[3] = new OracleParameter(":postaddress", OracleType.VarChar);
                    param[4] = new OracleParameter(":giftname", OracleType.VarChar);
                    param[5] = new OracleParameter(":giftpoint", OracleType.Number);
                    param[6] = new OracleParameter(":facid", OracleType.Number);
                    param[7] = new OracleParameter(":f1", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("InsertT_sgm_DHDETAIL", param);
                }
                // INSERT_SGM_DHDETAIL
                param[0].Value = mobile;
                param[1].Value = postHash["USER_MOBILE"].ToString().Trim();
                param[2].Value = postHash["USER_NAME"].ToString().Trim();
                param[3].Value = postHash["POSTADDRESS"].ToString().Trim();
                param[4].Value = giftname.Trim();
                param[5].Value = point.Trim();
                param[6].Value = facid.Trim();
                param[7].Value = postHash["F1"].ToString().Trim();
                int iRet2 = dataBase.ExecuteNonQuery(CommandType.Text, INSERT_SGM_DHDETAIL, param);
                if (iRet1 + iRet2 == 2)
                {
                    dataBase.CommitTrans();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                dataBase.RollBackTrans();
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  AddPostAddressAndModifyUser 参数[facid:" + facid + "][postHash:" + postHash + "] [dtU:" + dtU + "] [point:" + point + "] [mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
            }






            return bRet;
        }
        #endregion

        #region 康明斯使用
        private const string U_SGM_REGISTER_kms = @" UPDATE {0} X SET X.STATE='1',X.PROVINCE=:PROVINCE,X.USER_ADDRESS=:ADDRESS,X.CITY=:CITY WHERE X.FACID=:FACID AND X.IP=:IP AND X.SPRO=:SPRO AND X.STATE='9' AND X.guid =:GUID ";
        public bool UpdateSgmRegister_kms(string facid, string digit, string mobile, string lid, Hashtable userHash)
        {
            OracleParameter[] param = null;
            bool bRet = false;
            try
            {
                param = (OracleParameter[])ParameterCache.GetParams("UpdateSgmRegister");
                if (param == null)
                {
                    param = new OracleParameter[7];
                    param[0] = new OracleParameter(":PROVINCE", OracleType.VarChar);
                    param[1] = new OracleParameter(":ADDRESS", OracleType.VarChar);
                    param[2] = new OracleParameter(":CITY", OracleType.VarChar);
                    param[3] = new OracleParameter(":FACID", OracleType.VarChar);
                    param[4] = new OracleParameter(":IP", OracleType.VarChar);
                    param[5] = new OracleParameter(":SPRO", OracleType.VarChar);
                    param[6] = new OracleParameter(":GUID", OracleType.VarChar);
                    //将参数加入缓存
                    ParameterCache.PushCache("UpdateSgmRegister", param);
                }
                param[0].Value = userHash["PROVINCE"];
                param[1].Value = userHash["USER_ADDRESS"];
                param[2].Value = userHash["CITY"];
                param[3].Value = facid;
                param[4].Value = mobile;
                param[5].Value = digit;
                param[6].Value = lid;

                DataBase dataBase = new DataBase();
                int i = dataBase.ExecuteNonQuery(CommandType.Text, string.Format(U_SGM_REGISTER_kms, GetTable(TableName_Signin, facid)), param);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.LotteryDal.cs  UpdateSgmRegister 参数[facid:" + facid + "][mobile:" + mobile + "] [digit:" + digit + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal, ex);
            }

            return bRet;
        }

        #endregion

    }
}
