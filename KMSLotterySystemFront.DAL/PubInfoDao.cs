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
    public class PubInfoDao
    {

        DataBase dataBase = new DataBase();

        private static string TableName_Signin = TableNamePrefix() + "SHAKE_REGISTERUSER_";

        private static string TableName_Lottery = TableNamePrefix() + "shake_lotterylog_";

        private const String GET_TABLE_ALLCOLMUNS_SQL = "SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME=:TABLE_NAME";

        private const string GET_LUCKYMONEY_SQL = "SELECT B.* FROM {0} B WHERE B.FACID=:FACID AND  B.SPRO=:SPRO AND B.OPENID=:OPENID AND B.GUID=:GUID";

        private const string MODIFY_LUCKYMONEY_SQL = "UPDATE {0} R SET R.F5='1',R.SHOP_DATE=SYSDATE,F6=:MONEY,STATE=:NSTATE WHERE R.FACID=:FACID AND R.SPRO=:SPRO AND R.OPENID=:OPENID AND R.GUID=:GUID AND R.STATE=:LSTATE";

        private const string GET_HB_POOL_SQL = "SELECT * FROM T_SGM_SHAKE_HB H WHERE H.FACID=:FACID AND H.LOTTERYTIMES<H.TOTALTIMES ORDER BY H.SORT ASC";

        private const string GET_HB_REDPACKCONFIG_SQL = "SELECT * FROM T_SGM_SHAKE_RPCONFIG G WHERE G.FACID=:FACID AND G.CCNACTIVITYID=:CCNACTIVITYID";


        /// <summary>
        /// 新配置红包
        /// </summary>
        private const string GET_HB_REDPACKCONFIG_SQLNew = "SELECT * FROM  t_sgm_wxpay_config  G WHERE G.FACID=:FACID AND G.CCNACTIVITYID=:CCNACTIVITYID";


        private const string MODIFY_LUCKYMONEY_SQL2 = "UPDATE {0} R SET R.SHOPINGDATE=SYSDATE,STATE=:NSTATE WHERE R.FACID=:FACID AND  R.SPRO=:SPRO AND R.OPENID=:OPENID AND R.GUID=:GUID AND STATE=:LSTATE";

        private const string MODIFY_HB_SQL = "UPDATE T_SGM_SHAKE_HB P SET LOTTERYTIMES=LOTTERYTIMES+1 WHERE LOTTERYID=:LOTTERYID AND LOTTERYTIMES<TOTALTIMES AND FACID=:FACID";

        private const string ADDCHECKLOG_SQL = "INSERT INTO T_SGM_SHAKE_CHECKTOLOG(GUID,OPENID,COUNTRY,PROVINCE,CITY,NICKNAME,SEX,IP,LOTTERYGUID,TYPE,FACID,LINETYPE,CHANNEL) VALUES (:GUID,:OPENID,:COUNTRY,:PROVINCE,:CITY,:NICKNAME,:SEX,:IP,:LOTTERYGUID,:TYPE,:FACID,:LINETYPE,:CHANNEL)";

        private const string ADDCHECKLOG_SQL2 = "INSERT INTO T_SGM_SHAKE_CHECKTOLOG(GUID,OPENID,COUNTRY,PROVINCE,CITY,NICKNAME,SEX,IP,LOTTERYGUID,TYPE,FACID) VALUES (:GUID,:OPENID,:COUNTRY,:PROVINCE,:CITY,:NICKNAME,:SEX,:IP,:LOTTERYGUID,:TYPE,:FACID)";

        private const string ADDRESERVE_SQL = "INSERT INTO T_SGM_SHAKE_RESERVE(RESERVEGUID,MOBILE,CHANNEL,FACID,USERNAME) VALUES (:RESERVEGUID,:MOBILE,:CHANNEL,:FACID,:USERNAME)";

        private const string ADDAREA_SQL = "SELECT XX.STOREID,XX.STORENAME,XX.STOREPROVINCE,XX.STORECITY,XX.STOREAREA,XX.STOREADDRESS,XX.STORELINKMAN,XX.STORETEL,XX.DEALERID,XX.DEALERNAME FROM T_SGM_SHAKE_STORE XX  WHERE XX.FACID=:FACID AND  XX.FLAG='1' AND XX.ISJOINACTIVITY='1' ORDER BY XX.STOREPROVINCE,XX.STORECITY,XX.STOREAREA";

        private const string ADD_AUTHWECHATLOG_SQL = "INSERT INTO T_SGM_SHAKE_AUTHWECHAT_LOG(GUID,FACTORYID,OPENID,NICKNAME,HEADIMGURL,COUNTRY,PROVINCE,CITY,SEX,UNIONID) VALUES (:GUID,:FACTORYID,:OPENID,:NICKNAME,:HEADIMGURL,:COUNTRY,:PROVINCE,:CITY,:SEX,:UNIONID)";


        private const string ADD_HBRecord = " insert into t_smg_wxpay_hbrecord  (GUID,CCNACTIVITYID,CCNACTIVITYNAME,CODEID,FACID,RE_OPENID,TOTAL_AMOUNT,STATE,IP,MCH_BILLNO,SEND_LISTID,LOTTERYID,TOTAL_NUM,HB_TYPE,STATEDETAIL)values(:GUID,:CCNACTIVITYID,:CCNACTIVITYNAME,:CODEID,:FACID,:RE_OPENID,:TOTAL_AMOUNT,:STATE,:IP,:MCH_BILLNO,:SEND_LISTID,:LOTTERYID,:TOTAL_NUM,:HB_TYPE,:STATEDETAIL)";

        private const string ADD_WxPayRecord = " INSERT INTO t_sgm_wxpay_record (GUID,CCNACTIVITYID,CCNACTIVITYNAME,CODEID,FACID,RE_OPENID,TOTAL_AMOUNT,MCH_ID,WXAPPID,MCH_BILLNO,SEND_LISTID,IP,RETURN_CODE,RETURN_MSG,RESULT_CODE,ERR_CODE,ERR_CODE_DES,HB_TYPE,TOTAL_NUM)VALUES(:GUID,:CCNACTIVITYID,:CCNACTIVITYNAME,:CODEID,:FACID,:RE_OPENID,:TOTAL_AMOUNT,:MCH_ID,:WXAPPID,:MCH_BILLNO,:SEND_LISTID,:IP,:RETURN_CODE,:RETURN_MSG,:RESULT_CODE,:ERR_CODE,:ERR_CODE_DES,:HB_TYPE,:TOTAL_NUM)";

        private const string SELECT_SHAKE_REMIND_SQL = "SELECT X.* FROM T_SGM_SHAKE_REMIND X WHERE X.FACID=:FACID AND X.DELETEFLAG='1' AND X.TYPE='1' ";
        /// <summary>
        /// 检测当前中奖数码是否已经成功领取
        /// </summary>
        private const string SQL_CheckCodeGetHB = "select xx.*,xx.rowid  from    t_smg_wxpay_hbrecord  xx where XX.FACID=:FACID AND  XX.CCNACTIVITYID=:CCNACTIVITYID AND XX.CODEID=:CODEID AND  XX.LOTTERYID=:LOTTERYID AND  XX.RE_OPENID=:RE_OPENID  AND  XX.DELETEFLAG='1' AND  XX.STATE='SUCCESS'";


        /// <summary>
        /// 检测当前中奖数码是否已经成功领取2
        /// </summary>
        private const string SQL_CheckCodeGetHB2 = "select xx.*,xx.rowid  from    t_smg_wxpay_hbrecord  xx where XX.FACID=:FACID AND  XX.CCNACTIVITYID=:CCNACTIVITYID AND XX.CODEID=:CODEID  AND  XX.DELETEFLAG='1' AND  XX.STATE='SUCCESS'";

        /// <summary>
        /// 检测当前数码是否中奖
        /// </summary>
        private const string SQL_CheckCodeIsLotteryed = "select * from  {0}  xx where xx.FACID=:FACID  and  xx.GUID=:GUID   AND  XX.DIGIT=:DIGIT  AND  XX.DELETEFLAG='1' ";


        //insert  into t_sgm_wxpay_queryrecord (GUID,ccnactivityid,ccnactivityname,facid,mch_id,mch_billno,detail_id,status,send_type,hb_type,total_num,total_amount,reason,send_time,refund_time,refund_amount,wishing,remark,act_name,hblist,openid,amount,rcv_time,return_code,return_msg,result_code,err_code,err_code_des,solution)values()

        /// <summary>
        /// 根据商户订单号查询红包发放信息
        /// </summary>
        private const string SQL_QueryHb_Record = "insert  into t_sgm_wxpay_queryrecord (GUID,CCNACTIVITYID,CCNACTIVITYNAME,FACID,MCH_ID,MCH_BILLNO,DETAIL_ID,STATUS,SEND_TYPE,HB_TYPE,TOTAL_NUM,TOTAL_AMOUNT,REASON,SEND_TIME,REFUND_TIME,REFUND_AMOUNT,WISHING,REMARK,ACT_NAME,HBLIST,OPENID,AMOUNT,RCV_TIME,RETURN_CODE,RETURN_MSG,RESULT_CODE,ERR_CODE,ERR_CODE_DES,SOLUTION)values(:GUID,:CCNACTIVITYID,:CCNACTIVITYNAME,:FACID,:MCH_ID,:MCH_BILLNO,:DETAIL_ID,:STATUS,:SEND_TYPE,:HB_TYPE,:TOTAL_NUM,:TOTAL_AMOUNT,:REASON,:SEND_TIME,:REFUND_TIME,:REFUND_AMOUNT,:WISHING,:REMARK,:ACT_NAME,:HBLIST,:OPENID,:AMOUNT,:RCV_TIME,:RETURN_CODE,:RETURN_MSG,:RESULT_CODE,:ERR_CODE,:ERR_CODE_DES,:SOLUTION)";




        /// <summary>
        ///  根据错误码查询对应的解决方案
        /// </summary>
        /// <returns></returns>
        private const string SQL_GetSolutionByErrorCode = " SELECT * FROM  t_sgm_wxpay_errorcode XX  WHERE XX.ERRORCODE=:ERRORCODE AND  XX.FTYPE=:FTYPE  AND  XX.TYPE=:TYPE AND  XX.DELETEFLAG='1' ";


        /// <summary>
        /// 获取数码中奖信息
        /// </summary>
        //private const string SQL_GetLotteryInfoByCode = "select xx.*,xx.rowid ,yy.total_amount  from  {0}  xx   join t_smg_wxpay_hbrecord yy on xx.facid=yy.facid and  xx.spro=yy.codeid and  yy.state='SUCCESS' where xx.facid=:FACID  and  xx.spro=:SPRO   and  xx.LOTTERYLEVEL is not null   AND  XX.FLAG='1'   ";

        private const string SQL_GetLotteryInfoByCode = "select xx.*,xx.rowid  from  {0}  xx  where xx.facid=:FACID  and  xx.spro=:SPRO   and  xx.LOTTERYLEVEL is not null   AND  XX.FLAG='1'   ";



        /// <summary>
        /// 根据电话号码获取省份和城市
        /// </summary>
        private const string GET_CITYNAME_BY_PHONE = "SELECT C.PROVINCE_NAME,C.CITY_NAME FROM T_DICT_O_CITY C WHERE C.CITY_CODE=:CITY_CODE1  OR C.CITY_CODE=:CITY_CODE2 OR  C.CITY_CODE=:CITY_CODE3";


        /// <summary>
        /// 根据手机号码获取省份和城市
        /// </summary>
        private const string GET_CITYNAME_BY_MOBILENEW = "SELECT M.ZONE_CODE FROM T_REPORT_DICT_MOBILE M WHERE M.MOBILE_CODE=:MOBILE_CODE";


        /// <summary>
        /// 根据取号获取城市
        /// </summary>
        private const string GET_CITYNAME_BY_CITYCODE_NEW = "SELECT Z.PROVINCE_NAME,Z.CITY_NAME  FROM T_REPORT_DICT_ZONE Z WHERE Z.ZONE_CODE=:ZONE_CODE  AND ROWNUM<2";


        /// <summary>
        /// 根据手机号获取预约信息
        /// </summary>
        private const string GET_ReserveInfoByMobile = " SELECT  A.*,B.STORENAME,B.STOREADDRESS  FROM T_SGM_SHAKE_RESERVE  A   LEFT  JOIN  T_SGM_SHAKE_STORE B  ON A.FACID=B.FACID AND  A.STOREID=B.STOREID  WHERE  A.FACID=:FACID AND  A.MOBILE=:MOBILE   and a.channel=:CHANNEL ";


        /// <summary>
        /// 获取门店信息
        /// </summary>
        /// <returns></returns>
        private const string GET_StoreInfoByStoreid = " select * from T_SGM_SHAKE_STORE  XX  where xx.facid=:FACID  and  xx.Storeid=:STOREID  and   xx.FLAG='1' ";

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

        /// <summary>
        /// 从视图中检索表是否存在
        /// </summary>
        private const String PARAM_TABLENAME = ":TABLE_NAME";


        #region 添加扫描点击活动或者转发活动点击纪录
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public int AddClickInfo(WechatClickEntity wc)
        {
            try
            {

                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[11];

                param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":COUNTRY", OracleType.VarChar, 32);
                param[3] = new OracleParameter(":PROVINCE", OracleType.VarChar, 32);
                param[4] = new OracleParameter(":CITY", OracleType.VarChar, 32);
                param[5] = new OracleParameter(":NICKNAME", OracleType.VarChar, 32);
                param[6] = new OracleParameter(":SEX", OracleType.VarChar, 1);
                param[7] = new OracleParameter(":IP", OracleType.VarChar, 32);
                param[8] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar, 32);
                param[9] = new OracleParameter(":TYPE", OracleType.VarChar, 1);
                param[10] = new OracleParameter(":FACID", OracleType.VarChar, 10);


                param[0].Value = wc.GUID;
                param[1].Value = wc.OPENID;
                param[2].Value = wc.COUNTRY;
                param[3].Value = wc.PROVINCE;
                param[4].Value = wc.CITY;
                param[5].Value = wc.NICKNAME;
                param[6].Value = wc.SEX;
                param[7].Value = wc.IP;
                param[8].Value = wc.LOTTERYGUID;
                param[9].Value = wc.TYPE;
                param[10].Value = wc.FACID;

                return dataBase.ExecuteNonQuery(CommandType.Text, ADDCHECKLOG_SQL2, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("AddClickInfo----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }
        #endregion

        #region 添加扫描点击活动或者转发活动点击纪录或进入homepage （线上，线下）页    new
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public int AddClickInfoNew(WechatClickEntity wc)
        {
            try
            {

                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[13];

                param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":COUNTRY", OracleType.VarChar, 32);
                param[3] = new OracleParameter(":PROVINCE", OracleType.VarChar, 32);
                param[4] = new OracleParameter(":CITY", OracleType.VarChar, 32);
                param[5] = new OracleParameter(":NICKNAME", OracleType.VarChar, 32);
                param[6] = new OracleParameter(":SEX", OracleType.VarChar, 1);
                param[7] = new OracleParameter(":IP", OracleType.VarChar, 32);
                param[8] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar, 32);
                param[9] = new OracleParameter(":TYPE", OracleType.VarChar, 1);
                param[10] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[11] = new OracleParameter(":LINETYPE", OracleType.VarChar, 1);
                param[12] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);


                param[0].Value = wc.GUID;
                param[1].Value = wc.OPENID;
                param[2].Value = wc.COUNTRY;
                param[3].Value = wc.PROVINCE;
                param[4].Value = wc.CITY;
                param[5].Value = wc.NICKNAME;
                param[6].Value = wc.SEX;
                param[7].Value = wc.IP;
                param[8].Value = wc.LOTTERYGUID;
                param[9].Value = wc.TYPE;
                param[10].Value = wc.FACID;
                param[11].Value = wc.LINETYPE;
                param[12].Value = wc.CHANNEL;

                return dataBase.ExecuteNonQuery(CommandType.Text, ADDCHECKLOG_SQL, param);



            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("AddClickInfo----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }
        #endregion

        #region 微信用户授权记录
        /// <summary>
        /// 添加微信用户授权记录
        /// </summary>
        /// <param name="wechat">微信用户信息主体</param>
        /// <returns></returns>
        public int AddAuthWechatUser(WechatUserEntity wechat)
        {
            try
            {

                OracleParameter[] param = new OracleParameter[10];

                param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 40);
                param[3] = new OracleParameter(":NICKNAME", OracleType.VarChar, 50);
                param[4] = new OracleParameter(":HEADIMGURL", OracleType.VarChar, 200);
                param[5] = new OracleParameter(":COUNTRY", OracleType.VarChar, 30);
                param[6] = new OracleParameter(":PROVINCE", OracleType.VarChar, 30);
                param[7] = new OracleParameter(":CITY", OracleType.VarChar, 30);
                param[8] = new OracleParameter(":SEX", OracleType.VarChar, 2);
                param[9] = new OracleParameter(":UNIONID", OracleType.VarChar, 100);


                param[0].Value = wechat.guid;
                param[1].Value = wechat.facid;
                param[2].Value = wechat.openid;
                param[3].Value = wechat.nickname ?? "";
                param[4].Value = wechat.headimgurl ?? "";
                param[5].Value = wechat.country ?? "";
                param[6].Value = wechat.province ?? "";
                param[7].Value = wechat.city ?? "";
                param[8].Value = wechat.sex ?? "";
                param[9].Value = wechat.unionid ?? "";

                return dataBase.ExecuteNonQuery(CommandType.Text, ADD_AUTHWECHATLOG_SQL, param);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("AddAuthWechatUser----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                throw ex;
            }
        }
        #endregion

        #region 红包判断相关

        /// <summary>
        /// 获取中奖信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="openid">微信ID</param>
        /// <param name="lid">中奖纪录ID</param>
        /// <param name="digitcode">数码</param>
        /// <returns></returns>
        public DataTable getLuckyMoney(string facid, string openid, string lid, string digitcode)
        {
            DataTable lmtd = null;

            try
            {

                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[4];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":SPRO", OracleType.VarChar, 24);
                param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[3] = new OracleParameter(":GUID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = digitcode;
                param[2].Value = openid;
                param[3].Value = lid;
                #endregion


                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(GET_LUCKYMONEY_SQL, table);

                lmtd = dataBase.ExecuteQuery(CommandType.Text, sql, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("getLuckyMoney----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return null;
            }
            return lmtd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="lid"></param>
        /// <param name="digitcode"></param>
        /// <param name="money"></param>
        /// <param name="nstate"></param>
        /// <param name="lstate"></param>
        /// <returns></returns>
        public int ModifyLucklyMoeny(string facid, string openid, string lid, string digitcode, string money, string nstate, string lstate)
        {
            try
            {

                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[7];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":SPRO", OracleType.VarChar, 24);
                param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[3] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                param[4] = new OracleParameter(":NSTATE", OracleType.VarChar, 2);
                param[5] = new OracleParameter(":LSTATE", OracleType.VarChar, 2);
                param[6] = new OracleParameter(":MONEY", OracleType.VarChar, 5);

                param[0].Value = facid;
                param[1].Value = digitcode;
                param[2].Value = openid;
                param[3].Value = lid;
                param[4].Value = nstate;
                param[5].Value = lstate;
                param[6].Value = money;
                #endregion


                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(MODIFY_LUCKYMONEY_SQL, table);

                return dataBase.ExecuteNonQuery(CommandType.Text, sql, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("getLuckyMoney----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }


            //UPDATE T_SGM_SHAKE_REGISTERUSER_9999 R SET R.F5='1',R.SHOP_DATE=SYSDATE,F6=:MONEY WHERE R.FACID='9733' R.SPRO='' AND R.OPENID='' AND R.GUID='';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="lid"></param>
        /// <param name="digitcode"></param>
        /// <param name="nstate"></param>
        /// <param name="lstate"></param>
        /// <param name="lotterylive"></param>
        /// <returns></returns>
        public bool ModifyLucklyMoeny2(string facid, string openid, string lid, string digitcode, string nstate, string lstate, string lotterylive)
        {
            bool bRet = false;
            try
            {
                int nRet2 = 0;
                int nRet1 = 0;

                DataBase dataBase = new DataBase();
                dataBase.BeginTrans();

                try
                {

                    #region 组织参数
                    OracleParameter[] param = null;

                    param = new OracleParameter[6];
                    param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param[1] = new OracleParameter(":SPRO", OracleType.VarChar, 24);
                    param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                    param[3] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    param[4] = new OracleParameter(":NSTATE", OracleType.VarChar, 2);
                    param[5] = new OracleParameter(":LSTATE", OracleType.VarChar, 2);

                    param[0].Value = facid;
                    param[1].Value = digitcode;
                    param[2].Value = openid;
                    param[3].Value = lid;
                    param[4].Value = nstate;
                    param[5].Value = lstate;

                    OracleParameter[] param2 = null;

                    param2 = new OracleParameter[2];
                    param2[0] = new OracleParameter(":LOTTERYID", OracleType.VarChar, 2);
                    param2[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);


                    param2[0].Value = lotterylive;
                    param2[1].Value = facid;


                    #endregion


                    string table = GetTable(TableName_Signin, facid);
                    string sql = string.Format(MODIFY_LUCKYMONEY_SQL2, table);

                    nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, sql, param);
                    nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, MODIFY_HB_SQL, param2);

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
                    KMSLotterySystemFront.Logger.AppLog.Write("ModifyLucklyMoeny2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                    dataBase.RollBackTrans();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("ModifyLucklyMoeny2----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return bRet;
            }
            return bRet;
        }


        /// <summary>
        /// 获取红包奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="hbtd"></param>
        /// <returns></returns>
        public bool GetHB(string facid, out DataTable hbtd)
        {
            hbtd = null;
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[1];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = facid;

                #endregion

                hbtd = dataBase.ExecuteQuery(CommandType.Text, GET_HB_POOL_SQL, param);

                if (hbtd != null && hbtd.Rows.Count > 0)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("getLuckyMoney----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }


        #region  获取红包奖池1.1

        /// <summary>
        /// 获取红包奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="hbtd"></param>
        /// <returns></returns>
        public DataTable GetRedPackConfig(string facid, string activityid)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);

                param[0].Value = facid;
                param[1].Value = activityid;

                #endregion

                return dataBase.ExecuteQuery(CommandType.Text, GET_HB_REDPACKCONFIG_SQL, param);

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetRedPackConfig----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return null;
            }
        }
        #endregion


        #region 获取红包奖池 1.2

        /// <summary>
        /// 获取红包奖池
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="hbtd"></param>
        /// <returns></returns>
        public DataTable GetRedPackConfigNew(string facid, string activityid)
        {
            try
            {
                #region 组织参数
                OracleParameter[] param = null;

                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);

                param[0].Value = facid;
                param[1].Value = activityid;

                #endregion

                return dataBase.ExecuteQuery(CommandType.Text, GET_HB_REDPACKCONFIG_SQLNew, param);

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetRedPackConfigNew----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return null;
            }
        }
        #endregion


        #region 1.1检测当前中奖数码是否已经成功领取
        /// <summary>
        /// 检测当前中奖数码是否已经成功领取
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="ccnactivityid"></param>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="lid"></param>
        /// <returns></returns>
        public bool CheckCodeGetHB(string facid, string ccnactivityid, string openid, string code, string lid)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[5];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);
                param[2] = new OracleParameter(":CODEID", OracleType.VarChar, 32);
                param[3] = new OracleParameter(":LOTTERYID", OracleType.VarChar, 32);
                param[4] = new OracleParameter(":RE_OPENID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = ccnactivityid;
                param[2].Value = code;
                param[3].Value = lid;
                param[4].Value = openid;

                DataTable DT = dataBase.ExecuteQuery(CommandType.Text, SQL_CheckCodeGetHB, param);
                if (DT != null && DT.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.PubInfoDao.cs-CheckCodeGetHB  IN[facid:" + facid + "] [ccnactivityid:" + ccnactivityid + "][openid:" + openid + "][code:" + code + "][lid:" + lid + "]-----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion

        #region 1.2检测当前中奖数码是否已经成功领取(重载)
        /// <summary>
        /// 检测当前中奖数码是否已经成功领取
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="ccnactivityid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckCodeGetHB2(string facid, string ccnactivityid, string code)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);
                param[2] = new OracleParameter(":CODEID", OracleType.VarChar, 32);

                param[0].Value = facid;
                param[1].Value = ccnactivityid;
                param[2].Value = code;

                DataTable DT = dataBase.ExecuteQuery(CommandType.Text, SQL_CheckCodeGetHB2, param);
                if (DT != null && DT.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.PubInfoDao.cs-CheckCodeGetHB2  IN[facid:" + facid + "] [ccnactivityid:" + ccnactivityid + "][code:" + code + "]-----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion




        #region 检测数码是否中奖（查询中奖记录表）
        /// <summary>
        /// 检测数码是否中奖（查询中奖记录表）
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// <param name="lid">中奖guid</param>
        /// <returns></returns>
        public bool CheckCodeIsLotteryed(string facid, string code, string lid)
        {

            bool bRet = false;
            try
            {

                OracleParameter[] param = null;
                param = new OracleParameter[3];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":GUID", OracleType.NVarChar, 32);
                param[2] = new OracleParameter(":DIGIT", OracleType.NVarChar, 32);

                param[0].Value = facid;
                param[1].Value = lid;
                param[2].Value = code;

                string table = GetTable(TableName_Lottery, facid);

                string sql = string.Format(SQL_CheckCodeIsLotteryed, table);

                DataTable DT = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (DT != null && DT.Rows.Count > 0)
                {
                    bRet = true;
                }


            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.DAL.PubInfoDao.cs-CheckCodeIsLotteryed  IN[facid:" + facid + "] [code:" + code + "][lid:" + lid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }


        #endregion


        #endregion

        #region 17) 活动预约

        /// <summary>
        /// 添加预约数据
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="guid">记录guid</param>
        /// <param name="mobile">预约手机号码</param>
        /// <param name="channel">预约渠道</param>
        /// <param name="username">预约姓名</param>
        /// <returns></returns>
        public int AddReserve(string factoryid, string guid, string mobile, string channel, string username)
        {
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[5];

                param[0] = new OracleParameter(":RESERVEGUID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":CHANNEL", OracleType.VarChar, 2);
                param[3] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[4] = new OracleParameter(":USERNAME", OracleType.VarChar, 40);


                param[0].Value = guid;
                param[1].Value = mobile;
                param[2].Value = channel;
                param[3].Value = factoryid;
                param[4].Value = username;


                return dataBase.ExecuteNonQuery(CommandType.Text, ADDRESERVE_SQL, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("AddReserve----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }

        /// <summary>
        /// 添加预约数据  -壳牌超凡喜力
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="guid">记录guid</param>
        /// <param name="mobile">预约手机号码</param>
        /// <param name="channel">预约渠道</param>
        /// <param name="username">预约姓名</param>
        /// <param name="storeid">门店id</param>
        /// <param name="openid">预约人的openid</param>
        /// <param name="f1">推荐人的openid</param>
        /// <returns></returns>
        public int AddReserve(string factoryid, string guid, string mobile, string channel, string username, string storeid, string openid, string f1)
        {
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[8];

                param[0] = new OracleParameter(":RESERVEGUID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[2] = new OracleParameter(":CHANNEL", OracleType.VarChar, 2);
                param[3] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[4] = new OracleParameter(":USERNAME", OracleType.VarChar, 40);
                param[5] = new OracleParameter(":STOREID", OracleType.VarChar, 20);
                param[6] = new OracleParameter(":OPENID", OracleType.VarChar, 64);
                param[7] = new OracleParameter(":F1", OracleType.VarChar, 64);


                param[0].Value = guid;
                param[1].Value = mobile;
                param[2].Value = channel;
                param[3].Value = factoryid;
                param[4].Value = username;
                param[5].Value = storeid;
                param[6].Value = openid;
                param[7].Value = f1;

                string sql = "INSERT INTO T_SGM_SHAKE_RESERVE(RESERVEGUID,MOBILE,CHANNEL,FACID,USERNAME,STOREID,OPENID,F1) VALUES (:RESERVEGUID,:MOBILE,:CHANNEL,:FACID,:USERNAME,:STOREID,:OPENID,:F1)";

                return dataBase.ExecuteNonQuery(CommandType.Text, sql, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("AddReserve----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }


        #region  获取抽奖答复信息
        /// <summary>
        /// 获取抽奖答复信息
        /// <param name="factoryid">厂家编号</param>
        /// <param name="replayid">答复id</param>
        /// <param name="channel">预约渠道</param>
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

                DataTable DT = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (DT != null && DT.Rows.Count > 0)
                {
                    replay = DT.Rows[0][0].ToString();

                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao.cs  GetShakeReplay----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }

            return replay;
        }
        #endregion


        /// <summary>
        /// 检测门店是否参与活动
        /// <param name="factoryid">厂家编号</param>
        /// <param name="storeid">门店id</param>
        /// <returns></returns>
        public bool CheckStoreIsJoinActivity(string factoryid, string storeid)
        {
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[2];

                param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = storeid;
                param[1].Value = factoryid;

                //select XX.ISJOINACTIVITY from   T_SGM_SHAKE_STORE  XX  where xx.facid=:FACID  and  xx.Storeid=:STOREID;

                string sql = "select XX.ISJOINACTIVITY from T_SGM_SHAKE_STORE  XX  where xx.facid=:FACID  and  xx.Storeid=:STOREID  and   FLAG='1'  ";

                if (!string.IsNullOrEmpty(dataBase.ExecuteScalar(CommandType.Text, sql, param).ToString()))
                {
                    string isjoin = dataBase.ExecuteScalar(CommandType.Text, sql, param).ToString();
                    if (isjoin == "1")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetReserveByMobile----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }



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
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[2];

                param[0] = new OracleParameter(":STOREID", OracleType.VarChar, 32);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = storeid;
                param[1].Value = factoryid;

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
                KMSLotterySystemFront.Logger.AppLog.Write("CheckStoreIsJoinActivity----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }




        /// <summary>
        /// 检测手机号是否预约
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object GetReserveByMobile(string factoryid, string mobile, string channel)
        {
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[2];

                param[0] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = mobile;
                param[1].Value = factoryid;


                string sql = "SELECT A.RESERVEGUID FROM T_SGM_SHAKE_RESERVE  A WHERE A.MOBILE=:MOBILE AND A.FACID=:FACID ";

                return dataBase.ExecuteScalar(CommandType.Text, sql, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetReserveByMobile----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }


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

                dt = dataBase.ExecuteQuery(CommandType.Text, GET_ReserveInfoByMobile, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetReserveByMobile----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }

            return bRet;
        }





        /// <summary>
        /// 通过手机号码查询预约用户被推荐人OPENID
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public object GetReserveByMobile(string factoryid, string mobile)
        {
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[2];

                param[0] = new OracleParameter(":MOBILE", OracleType.VarChar, 11);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = mobile;
                param[1].Value = factoryid;


                string sql = "SELECT A.F1 FROM T_SGM_SHAKE_RESERVE  A WHERE A.MOBILE=:MOBILE AND A.FACID=:FACID ";

                return dataBase.ExecuteScalar(CommandType.Text, sql, param);


            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("GetReserveByMobile----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }

        /// <summary>
        /// 添加红包充值记录
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        public int AddRedPackLog(WechatRedPack redpack)
        {
            try
            {
                OracleParameter[] param = null;
                //构造参数

                param = new OracleParameter[12];

                param[0] = new OracleParameter(":GUID", OracleType.VarChar, 50);
                param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);
                param[2] = new OracleParameter(":CCNACTIVITYNAME", OracleType.VarChar, 60);
                param[3] = new OracleParameter(":CODEID", OracleType.VarChar, 32);
                param[4] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[5] = new OracleParameter(":RE_OPENID", OracleType.VarChar, 60);
                param[6] = new OracleParameter(":TOTAL_AMOUNT", OracleType.VarChar, 10);
                param[7] = new OracleParameter(":ACT_ID", OracleType.VarChar, 32);
                param[8] = new OracleParameter(":STATE", OracleType.VarChar, 10);
                param[9] = new OracleParameter(":IP", OracleType.VarChar, 32);
                param[10] = new OracleParameter(":F1", OracleType.VarChar, 100);
                param[11] = new OracleParameter(":WEIXIN_MCHBILLNO", OracleType.VarChar, 50);


                param[0].Value = redpack.WEIXIN_GUID;
                param[1].Value = redpack.WEIXIN_ACTIVITYID;
                param[2].Value = redpack.WEIXIN_ACTIVITYNAME;
                param[3].Value = redpack.WEIXIN_CODEID;
                param[4].Value = redpack.WEIXIN_FACID;
                param[5].Value = redpack.WEIXIN_RE_OPENID;
                param[6].Value = redpack.WEIXIN_TOTAL_AMOUNT;
                param[7].Value = redpack.WEIXIN_ACT_ID;
                param[8].Value = redpack.WEIXIN_STATE;
                param[9].Value = redpack.WEIXIN_CLIENT_IP;
                param[10].Value = redpack.WEIXIN_SENDMESSAGE;
                param[11].Value = redpack.WEIXIN_MCHBILLNO;

                string sql = "INSERT INTO T_SGM_SHAKE_SENDRPLOG(GUID,CCNACTIVITYID,CCNACTIVITYNAME,CODEID,FACID,RE_OPENID,TOTAL_AMOUNT,ACT_ID,STATE,IP,F1,WEIXIN_MCHBILLNO) VALUES (:GUID,:CCNACTIVITYID,:CCNACTIVITYNAME,:CODEID,:FACID,:RE_OPENID,:TOTAL_AMOUNT,:ACT_ID,:STATE,:IP,:F1,:WEIXIN_MCHBILLNO)";

                return dataBase.ExecuteNonQuery(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("AddReserve----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return 0;
            }
        }




        /// <summary>
        /// 添加红包充值记录,红包API调用记录
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        public bool AddRedPackLog(WechatRedPack redpack, WxPayResult wxpayresult)
        {

            bool bRet = false;

            OracleParameter[] param1 = null;
            OracleParameter[] param2 = null;

            try
            {
                param1 = GetRedPackLogParam(redpack);
                param2 = GetWxPayResultLogParam(wxpayresult);
                if (param1 != null && param2 != null)
                {
                    DataBase dataBase = new DataBase();
                    dataBase.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;

                        //添加红包纪录日志
                        nRet1 = dataBase.ExecuteNonQuery(CommandType.Text, ADD_HBRecord, param1);

                        //添加红包发放API纪录日志
                        nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, ADD_WxPayRecord, param2);

                        if ((nRet1 + nRet2) >= 2)
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

                        KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfo.AddRedPackLog( , ) 红包发放 异常----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dataBase.RollBackTrans();

                    }
                }

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfo.AddRedPackLog( , ) 红包发放 异常----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
            return bRet;
        }


        #region 根据商户订单号查询红包信息 -日志记录
        /// <summary>
        /// 根据商户订单号查询红包信息 -日志记录
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        public bool AddQueryRedPackLog(QueryHbResult queryhbresult)
        {
            bool iRet = false;
            try
            {
                OracleParameter[] param1 = null;
                param1 = GetQueryRedPackLogParam(queryhbresult);
                int f = dataBase.ExecuteNonQuery(CommandType.Text, SQL_QueryHb_Record, param1);
                if (f >= 1)
                {
                    iRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao.cs-AddQueryRedPackLog() ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
            return iRet;
        }

        #endregion


        #region 根据错误码查询对应的解决方案
        /// <summary>
        /// 根据错误码查询对应的解决方案
        /// </summary>
        /// <param name="errorcode">错误码</param>
        /// <param name="ftype">接口调用类型（父类）</param>
        /// <param name="type">接口调用类型（小类）</param>
        /// <returns></returns>
        public string GetSolutionByErrorCode(string errorcode, string ftype, string type)
        {
            string solution = "";
            try
            {

                OracleParameter[] param = new OracleParameter[3];
                param[0] = new OracleParameter(":ERRORCODE", OracleType.VarChar, 32);//错误码
                param[1] = new OracleParameter(":FTYPE", OracleType.VarChar, 2);//请求接口类型（大类）
                param[2] = new OracleParameter(":TYPE", OracleType.VarChar, 2);//请求接口类型(小类)

                param[0].Value = errorcode;
                param[1].Value = ftype;
                param[2].Value = type;

                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, SQL_GetSolutionByErrorCode, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    solution = dt.Rows[0]["SOLUTION"].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao :GetSolutionByErrorCode() ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return solution;
        }
        #endregion




        /// <summary>
        /// 根据商户订单查询红包记录-组建参数 
        /// </summary>
        /// <param name="queryhbresult"></param>
        /// <returns></returns>
        private OracleParameter[] GetQueryRedPackLogParam(QueryHbResult queryhbresult)
        {
            OracleParameter[] param = null;
            //构造参数
            try
            {
                #region

                param = (OracleParameter[])ParameterCache.GetParams("GetQueryRedPackLogParam");
                if (param == null)
                {
                    param = new OracleParameter[29];
                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);//guid
                    param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);//活动内部编号
                    param[2] = new OracleParameter(":CCNACTIVITYNAME", OracleType.VarChar, 60);//活动名称
                    param[3] = new OracleParameter(":FACID", OracleType.VarChar, 10);//厂家编号
                    param[4] = new OracleParameter(":MCH_ID", OracleType.VarChar, 32);//微信支付分配的商户号
                    param[5] = new OracleParameter(":MCH_BILLNO", OracleType.VarChar, 32);//商户使用查询API填写的商户单号的原路返回
                    param[6] = new OracleParameter(":DETAIL_ID", OracleType.VarChar, 32);//使用API发放现金红包时返回的红包单号
                    param[7] = new OracleParameter(":STATUS", OracleType.VarChar, 16);//红包状态：（SENDING:发放中 SENT:已发放待领取 FAILED：发放失败 RECEIVED:已领取 RFUND_ING:退款中 REFUND:已退款）
                    param[8] = new OracleParameter(":SEND_TYPE", OracleType.VarChar, 32);//发放类型：（API:通过API接口发放 UPLOAD:通过上传文件方式发放 ACTIVITY:通过活动方式发放）
                    param[9] = new OracleParameter(":HB_TYPE", OracleType.VarChar, 32);//红包类型：（GROUP:裂变红包 NORMAL:普通红包）
                    param[10] = new OracleParameter(":TOTAL_NUM", OracleType.VarChar, 4);//发放数量
                    param[11] = new OracleParameter(":TOTAL_AMOUNT", OracleType.VarChar, 10);//红包总金额（单位分）
                    param[12] = new OracleParameter(":REASON", OracleType.VarChar, 100);//失败原因
                    param[13] = new OracleParameter(":SEND_TIME", OracleType.VarChar, 32);//红包发送时间
                    param[14] = new OracleParameter(":REFUND_TIME", OracleType.VarChar, 32);//红包退款时间（红包的退款时间（如果其未领取的退款））
                    param[15] = new OracleParameter(":REFUND_AMOUNT", OracleType.VarChar, 10);//红包退款金额
                    param[16] = new OracleParameter(":WISHING", OracleType.VarChar, 128);//祝福语
                    param[17] = new OracleParameter(":REMARK", OracleType.VarChar, 256);//活动描述（低版本微信可见）
                    param[18] = new OracleParameter(":ACT_NAME", OracleType.VarChar, 32);//活动名称
                    param[19] = new OracleParameter(":HBLIST", OracleType.Clob, 2000);//裂变红包领取列表
                    param[20] = new OracleParameter(":OPENID", OracleType.VarChar, 32);//领取红包的openid
                    param[21] = new OracleParameter(":AMOUNT", OracleType.VarChar, 10);//openid领取的金额
                    param[22] = new OracleParameter(":RCV_TIME", OracleType.VarChar, 32);//openid领取红包的时间
                    param[23] = new OracleParameter(":RETURN_CODE", OracleType.VarChar, 16);//SUCCESS/FAIL此字段是通信标识
                    param[24] = new OracleParameter(":RETURN_MSG", OracleType.VarChar, 128);//返回信息与return_code 对应
                    param[25] = new OracleParameter(":RESULT_CODE", OracleType.VarChar, 16);//业务结果（SUCCESS/FAIL）
                    param[26] = new OracleParameter(":ERR_CODE", OracleType.VarChar, 32);//错误码信息
                    param[27] = new OracleParameter(":ERR_CODE_DES", OracleType.VarChar, 128);//结果信息描述
                    param[28] = new OracleParameter(":SOLUTION", OracleType.VarChar, 300);//针对错误码对应的解决方案

                    //(:GUID,:CCNACTIVITYID,:CCNACTIVITYNAME,:FACID,:MCH_ID,:MCH_BILLNO,:DETAIL_ID,:STATUS,:SEND_TYPE,:HB_TYPE,:TOTAL_NUM,:TOTAL_AMOUNT,:REASON,:SEND_TIME,:REFUND_TIME,:REFUND_AMOUNT,:WISHING,:REMARK,:ACT_NAME,:HBLIST,:OPENID,:AMOUNT,:RCV_TIME,:RETURN_CODE,:RETURN_MSG,:RESULT_CODE,:ERR_CODE,:ERR_CODE_DES,:SOLUTION)

                    //将参数加入缓存
                    ParameterCache.PushCache("GetQueryRedPackLogParam", param);
                }

                param[0].Value = queryhbresult.guid;
                param[1].Value = queryhbresult.ccnactivityid;
                param[2].Value = queryhbresult.ccnactivityname;
                param[3].Value = queryhbresult.facid;
                param[4].Value = queryhbresult.mch_id;
                param[5].Value = queryhbresult.mch_billno;
                param[6].Value = queryhbresult.detail_id;
                param[7].Value = queryhbresult.status;
                param[8].Value = queryhbresult.send_type;
                param[9].Value = queryhbresult.hb_type;
                param[10].Value = queryhbresult.total_num;
                param[11].Value = queryhbresult.total_amount;
                param[12].Value = queryhbresult.reason;
                param[13].Value = queryhbresult.send_time;
                param[14].Value = queryhbresult.refund_time;
                param[15].Value = queryhbresult.refund_amount;
                param[16].Value = queryhbresult.wishing;
                param[17].Value = queryhbresult.remark;
                param[18].Value = queryhbresult.act_name;

                if (queryhbresult.result_code == "FAIL")
                {
                    param[19].Value = " ";
                }
                else
                {
                    if (string.IsNullOrEmpty(queryhbresult.hblist))
                    {
                        param[19].Value = " ";
                    }
                    else
                    {
                        param[19].Value = queryhbresult.hblist;
                    }
                }
                param[20].Value = queryhbresult.openid;
                param[21].Value = queryhbresult.amount;
                param[22].Value = queryhbresult.rcv_time;
                param[23].Value = queryhbresult.return_code;
                param[24].Value = queryhbresult.return_msg;
                param[25].Value = queryhbresult.result_code;
                param[26].Value = queryhbresult.err_code;
                param[27].Value = queryhbresult.err_code_des;
                param[28].Value = queryhbresult.solution;
                #endregion

            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("DAL层-根据商户号查询红包信息日志-GetQueryRedPackLogParam()组建参数异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        /// <summary>
        /// 红包日志-组建参数
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        private OracleParameter[] GetRedPackLogParam(WechatRedPack redpack)
        {
            OracleParameter[] param = null;
            //构造参数
            try
            {
                #region

                param = (OracleParameter[])ParameterCache.GetParams("GetRedPackLogParam");
                if (param == null)
                {
                    param = new OracleParameter[15];
                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 50);//guid
                    param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);//活动内部编号
                    param[2] = new OracleParameter(":CCNACTIVITYNAME", OracleType.VarChar, 60);//活动名称
                    param[3] = new OracleParameter(":CODEID", OracleType.VarChar, 32);//数码
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 10);//厂家编号
                    param[5] = new OracleParameter(":RE_OPENID", OracleType.VarChar, 60);//接收充值的OPENID
                    param[6] = new OracleParameter(":TOTAL_AMOUNT", OracleType.VarChar, 10);//金额
                    param[7] = new OracleParameter(":STATE", OracleType.VarChar, 10);//红包发放状态
                    param[8] = new OracleParameter(":IP", OracleType.VarChar, 32);//调用端IP地址
                    param[9] = new OracleParameter(":MCH_BILLNO", OracleType.VarChar, 50);//商户订单号
                    param[10] = new OracleParameter(":SEND_LISTID", OracleType.VarChar, 50);//红包订单的微信单号
                    param[11] = new OracleParameter(":LOTTERYID", OracleType.VarChar, 32);//中奖guid
                    param[12] = new OracleParameter(":TOTAL_NUM", OracleType.VarChar, 50);//发放数量
                    param[13] = new OracleParameter(":HB_TYPE", OracleType.VarChar, 1);//红包类型（0 普通红包  1 裂变红包）
                    param[14] = new OracleParameter(":STATEDETAIL", OracleType.VarChar, 100);//发放状态描述


                    //将参数加入缓存
                    ParameterCache.PushCache("GetRedPackLogParam", param);
                }

                param[0].Value = redpack.WEIXIN_GUID;
                param[1].Value = redpack.WEIXIN_ACTIVITYID;
                param[2].Value = redpack.WEIXIN_ACTIVITYNAME;
                param[3].Value = redpack.WEIXIN_CODEID;
                param[4].Value = redpack.WEIXIN_FACID;
                param[5].Value = redpack.WEIXIN_RE_OPENID;
                param[6].Value = redpack.WEIXIN_TOTAL_AMOUNT;
                param[7].Value = redpack.WEIXIN_STATE;
                param[8].Value = redpack.WEIXIN_CLIENT_IP;
                param[9].Value = redpack.WEIXIN_MCH_BILLNO;
                param[10].Value = redpack.WEIXIN_SEND_LISTID;
                param[11].Value = redpack.WEIXIN_LOTTERYID;
                param[12].Value = redpack.WEIXIN_TOTAL_NUM;
                param[13].Value = redpack.WEIXIN_HB_TYPE;
                param[14].Value = redpack.WEIXIN_STATEDETAIL;
                #endregion

            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("红包日志-GetRedPackLogParam()组建参数异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }

        /// <summary>
        /// 红包API调用日志-组建参数
        /// </summary>
        /// <param name="redpack"></param>
        /// <returns></returns>
        private OracleParameter[] GetWxPayResultLogParam(WxPayResult wxpayresult)
        {
            OracleParameter[] param = null;
            //构造参数
            try
            {
                #region

                param = (OracleParameter[])ParameterCache.GetParams("GetWxPayResultLogParam");
                if (param == null)
                {
                    param = new OracleParameter[19];
                    param[0] = new OracleParameter(":GUID", OracleType.VarChar, 50);//guid
                    param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 20);//活动内部编号
                    param[2] = new OracleParameter(":CCNACTIVITYNAME", OracleType.VarChar, 60);//活动名称
                    param[3] = new OracleParameter(":CODEID", OracleType.VarChar, 32);//数码
                    param[4] = new OracleParameter(":FACID", OracleType.VarChar, 10);//厂家编号
                    param[5] = new OracleParameter(":RE_OPENID", OracleType.VarChar, 60);//接收充值的OPENID
                    param[6] = new OracleParameter(":TOTAL_AMOUNT", OracleType.VarChar, 10);//金额
                    param[7] = new OracleParameter(":MCH_ID", OracleType.VarChar, 32);//商户号
                    param[8] = new OracleParameter(":WXAPPID", OracleType.VarChar, 32);//微信分配的公众账号ID
                    param[9] = new OracleParameter(":MCH_BILLNO", OracleType.VarChar, 32);//商户订单号
                    param[10] = new OracleParameter(":SEND_LISTID", OracleType.VarChar, 32);//红包订单的微信单号
                    param[11] = new OracleParameter(":IP", OracleType.VarChar, 32);//调用端IP地址
                    param[12] = new OracleParameter(":RETURN_CODE", OracleType.VarChar, 32);//返回状态码 SUCCESS/FAIL此字段是通信标识
                    param[13] = new OracleParameter(":RETURN_MSG", OracleType.VarChar, 100);//返回信息，与return_code对应
                    param[14] = new OracleParameter(":RESULT_CODE", OracleType.VarChar, 32);//业务结果
                    param[15] = new OracleParameter(":ERR_CODE", OracleType.VarChar, 32);//错误码信息
                    param[16] = new OracleParameter(":ERR_CODE_DES", OracleType.VarChar, 200);//错误码信息描述
                    param[17] = new OracleParameter(":HB_TYPE", OracleType.Char, 1);//红包类型（0 普通红包  1 裂变红包）
                    param[18] = new OracleParameter(":TOTAL_NUM", OracleType.VarChar, 2);//红包发放数量

                    //将参数加入缓存
                    ParameterCache.PushCache("GetWxPayResultLogParam", param);
                }

                param[0].Value = wxpayresult.guid;
                param[1].Value = wxpayresult.ccnactivityid;
                param[2].Value = wxpayresult.ccnactivityname;
                param[3].Value = wxpayresult.codeid;
                param[4].Value = wxpayresult.facid;
                param[5].Value = wxpayresult.re_openid;
                param[6].Value = wxpayresult.total_amount;
                param[7].Value = wxpayresult.mch_id;
                param[8].Value = wxpayresult.wxappid;
                param[9].Value = wxpayresult.mch_billno;
                param[10].Value = wxpayresult.send_listid;
                param[11].Value = wxpayresult.ip;
                param[12].Value = wxpayresult.return_code;
                param[13].Value = wxpayresult.return_msg;
                param[14].Value = wxpayresult.result_code;
                param[15].Value = wxpayresult.err_code;
                param[16].Value = wxpayresult.err_code_des;
                param[17].Value = wxpayresult.hb_type;
                param[18].Value = wxpayresult.total_num;
                #endregion

            }
            catch (Exception ex)
            {
                param = null;
                KMSLotterySystemFront.Logger.AppLog.Write("红包API调用日志-组建参数-GetWxPayResultLogParam()异常----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return param;
        }


        #endregion


        //获取省市区
        /// <summary>
        ///  获取省市区
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pid">省级编码id</param>
        /// <param name="cid">市级编码id</param>
        /// <returns></returns>
        public DataTable GetArea(string facid, string pid, string cid)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;

                if (string.IsNullOrEmpty(pid) && string.IsNullOrEmpty(cid))   //获取所有省
                {
                    string sql = "select c.proviceid,c.provicename from  t_sgm_area  c where c.flag='1' group by c.proviceid,c.provicename order by c.provicename ";
                    dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                }
                if (!string.IsNullOrEmpty(pid) && string.IsNullOrEmpty(cid))  //获取省下面所有市
                {

                    string sql = "  select c.cityid,c.cityname from t_sgm_area  c where c.flag='1'  and c.proviceid='" + pid + "' group by c.cityid,c.cityname order by c.cityname ";
                    dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                }
                if (!string.IsNullOrEmpty(pid) && !string.IsNullOrEmpty(cid))  //根据省，市 获取市下面的区查询 
                {
                    string sql = "SELECT C.COUNTYID,C.COUNTYNAME,c.provicename,c.cityname FROM  t_sgm_area C where c.flag='1'  and c.proviceid='" + pid + "' and c.cityid='" + cid + "' ORDER BY C.COUNTYNAME  ";
                    dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 获取省市区出现异常 GetArea msg [facid:" + facid + "] [pid:" + pid + "]  [cid:" + cid + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }



        //获取省市 1.2
        /// <summary>
        ///  获取省市    1.2
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pname">省级名称</param>
        /// <returns></returns>
        public DataTable GetAreaNew(string facid, string pname)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;

                if (string.IsNullOrEmpty(pname))   //获取所有省
                {
                    string sql = "select  xx.storeprovince  from   T_SGM_SHAKE_STORE  XX  where xx.facid='" + facid + "' and  xx.flag='1' and  xx.isjoinactivity='1'  group by  xx.storeprovince   order by xx.storeprovince ";
                    dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                }

                if (!string.IsNullOrEmpty(pname))  //获取省下面所有市
                {
                    string sql = " select  xx.storecity  from  T_SGM_SHAKE_STORE  XX  where xx.facid='" + facid + "' and  xx.storeprovince='" + pname + "'  and  xx.flag='1'  and  xx.isjoinactivity='1'   group by  xx.storecity   order by xx.storecity  ";

                    dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 获取省市出现异常 GetAreaNew msg [facid:" + facid + "] [pname:" + pname + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }

        //获取省市 1.2
        /// <summary>
        ///  获取省市    1.2
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="pname">省级名称</param>
        /// <returns></returns>
        public DataTable GetAreaList(string facid)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = new OracleParameter[1];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[0].Value = facid;

                dt = dataBase.ExecuteQuery(CommandType.Text, ADDAREA_SQL, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 获取省市出现异常 GetAreaNew msg [facid:" + facid + "] ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                throw ex;
            }
            return dt;
        }


        /// <summary>
        /// 获取省市下面的门店
        /// </summary>
        /// <param name="userID">授权帐号</param>
        /// <param name="userPwd">授权密码</param>
        /// <param name="provincename">省级名称</param>
        /// <param name="cityname">市级名称</param>
        /// <returns></returns>
        public DataTable GetStoreList(string facid, string provincename, string cityname)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;

                param = new OracleParameter[3];


                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                param[1] = new OracleParameter(":STOREPROVINCE", OracleType.VarChar, 20);
                param[2] = new OracleParameter(":STORECITY", OracleType.VarChar, 20);


                param[0].Value = facid;
                param[1].Value = provincename;
                param[2].Value = cityname;


                string sql = "select xx.*,xx.rowid from   T_SGM_SHAKE_STORE  XX  where xx.facid=:FACID  and   xx.storeprovince=:STOREPROVINCE   and  xx.storecity=:STORECITY ";
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 获取省市下面的门店 GetStoreList msg [facid:" + facid + "] [provincename:" + provincename + "]  [cityname:" + cityname + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }



        #region 查询基础数据表
        /// <summary>
        /// 根据基础类型查询基础数据
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="datatypename"></param>
        /// <returns></returns>
        public DataTable GetBaseDataByDataType(string facid, string datatypename)
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":DATATYPENAME", OracleType.VarChar, 50);
                param[0].Value = facid;
                param[1].Value = datatypename;
                string sql = "select xx.*,xx.rowid from   T_SGM_BASEDATA  XX  where xx.facid=:FACID  and   xx.DATATYPENAME=:DATATYPENAME ";
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 根据基础类型查询基础数据 出现异常 GetBaseDataByDataType  [facid:" + facid + "] [datatypename:" + datatypename + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion

        #region 限制openID每月/每天/每年/ 参与红包发放的最大次数
        /// <summary>
        /// 限制openID每月/每天/每年/ 参与红包发放的最大次数
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <param name="datetype"></param>
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

                    //sql = "  select xx.*, xx.rowid from t_smg_wxpay_hbrecord xx where xx.facid =:FACID  and xx.re_openid =:RE_OPENID   AND TO_CHAR(xx.createtime,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')  and  xx.state='SUCCESS' ";


                    sql = "SELECT B.*,B.ROWID FROM T_SGM_SHAKE_REGISTERUSER_9999  B WHERE  B.FACID=:FACID  and TO_CHAR(B.Vdate,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') and  b.flag='1' and b.openid=:OPENID   and  b.lotterylevel is not  null   order by b.vdate desc";



                }
                else if (datetype == "M")//当月
                {
                    //sql = "  select xx.*, xx.rowid from t_smg_wxpay_hbrecord xx where xx.facid =:FACID  and xx.re_openid =:RE_OPENID   AND TO_CHAR(xx.createtime,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM') and  xx.state='SUCCESS'  ";

                    sql = "SELECT B.*,B.ROWID FROM T_SGM_SHAKE_REGISTERUSER_9999  B WHERE  B.FACID=:FACID  and TO_CHAR(B.Vdate,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM') and  b.flag='1' and b.openid=:OPENID   and  b.lotterylevel is not  null   order by b.vdate desc ";


                }
                else if (datetype == "Y")//当年
                {
                    //sql = "  select xx.*, xx.rowid from t_smg_wxpay_hbrecord xx where xx.facid =:FACID  and xx.re_openid =:RE_OPENID   AND TO_CHAR(xx.createtime,'YYYY')=TO_CHAR(SYSDATE,'YYYY') and  xx.state='SUCCESS'  ";

                    sql = "SELECT B.*,B.ROWID FROM T_SGM_SHAKE_REGISTERUSER_9999  B WHERE  B.FACID=:FACID  and TO_CHAR(B.Vdate,'YYYY')=TO_CHAR(SYSDATE,'YYYY') and  b.flag='1' and b.openid=:OPENID   and  b.lotterylevel is not  null   order by b.vdate desc ";

                }
                else//不区分
                {
                    //sql = "  select xx.*, xx.rowid from t_smg_wxpay_hbrecord xx where xx.facid =:FACID  and xx.re_openid =:RE_OPENID and  xx.state='SUCCESS'  ";

                    sql = "SELECT B.*,B.ROWID FROM T_SGM_SHAKE_REGISTERUSER_9999  B WHERE  B.FACID=:FACID  and  b.flag='1' and b.openid=:OPENID   and  b.lotterylevel is not  null   order by b.vdate desc ";

                }

                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[0].Value = facid;
                param[1].Value = openid;


                DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count >= limitnum)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao  限制openID每月/每天/每年/ 参与红包发放的最大次数 CheckOpenidHbSendLimit 异常：  [facid:" + facid + "] [openid:" + openid + "]  [datetype:" + datetype + "]  [limitnum:" + limitnum + "]  --" + ex.TargetSite + "--" + ex.StackTrace + "--" + ex.Message, Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
        }
        #endregion

        #region 获取数码中奖纪录
        /// <summary>
        /// 获取数码中奖纪录
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable GetLotteryInfoByCode(string facid, string code)
        {

            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":SPRO", OracleType.VarChar, 16);
                param[0].Value = facid;
                param[1].Value = code;

                string table = GetTable(TableName_Signin, facid);
                string sql = string.Format(SQL_GetLotteryInfoByCode, table);
                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 获取数码中奖纪录 出现异常 GetLotteryInfoByCode  [facid:" + facid + "] [code:" + code + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return dt;
            }
            return dt;
        }
        #endregion

        #region 检测活动是否开始/结束
        /// <summary>
        /// 检测活动是否开始/结束
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="type">1检测活动是否开始，2 检测活动是否结束，3 活动不在有效时间范围之内</param>
        /// <returns></returns>
        public bool CheckActivityTime(string facid, string type)
        {
            bool bRet = false;
            DataTable dt = null;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[1];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[0].Value = facid;
                string sql = "";
                if (type == "1")
                {
                    sql = "SELECT  i.*,i.rowid  FROM t_Ccn_Ac_User  I   WHERE I.FACTORYID=:FACID    AND   TO_CHAR(I.Begintime,'YYYY-MM-DD HH24:MI:SS')<=TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') AND FLAG='1' ";
                }
                else if (type == "2")
                {
                    sql = "SELECT  i.*,i.rowid  FROM t_Ccn_Ac_User  I   WHERE I.FACTORYID=:FACID    AND   TO_CHAR(I.Endtime,'YYYY-MM-DD HH24:MI:SS')>=TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS')   AND FLAG='1' ";
                }
                else
                {
                    sql = "SELECT  i.*,i.rowid  FROM t_Ccn_Ac_User  I   WHERE I.FACTORYID=:FACID  AND   TO_CHAR(I.Begintime,'YYYY-MM-DD HH24:MI:SS')<=TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS')   AND   TO_CHAR(I.Endtime,'YYYY-MM-DD HH24:MI:SS')>=TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS')   AND FLAG='1' ";

                }

                dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                }



            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo 检测活动是否开始/结束 出现异常 CheckActivityTime  [facid:" + facid + "] [type:" + type + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion

        #region  添加用户抽奖注册信息

        /// <summary>
        /// 添加用户抽奖注册信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="infolist"></param>
        /// <returns></returns>
        public bool AddLotteryRegister(string facid, Hashtable infolist)
        {
            bool bRet = false;
            try
            {
                string registersql = "";
                InsertRechargeRegister(facid, infolist, out   registersql);
                int i = dataBase.ExecuteNonQuery(CommandType.Text, registersql, null);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDal 添加用户抽奖注册信息  出现异常 AddLotteryRegister  [facid:" + facid + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion


        #region  添加点击homepage信息

        /// <summary>
        /// 添加点击homepage信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="infolist"></param>
        /// <returns></returns>
        public bool AddInviteInfo(string facid, Hashtable infolist)
        {
            bool bRet = false;
            try
            {
                string homeclicksql = "";
                InsertRechargerHomeClick(facid, infolist, out   homeclicksql);
                int i = dataBase.ExecuteNonQuery(CommandType.Text, homeclicksql, null);
                if (i > 0)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDal 添加点击homepage信息  出现异常 AddInviteInfo  [facid:" + facid + "]  ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }

        #endregion


        #region 中奖用户正式注册(组建sql)

        /// <summary>
        /// 中奖用户正式注册(组建sql)
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertRechargeRegister(string factoryid, Hashtable userHash, out  string sql)
        {
            sql = "";
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = "T_SGM_SHAKE_REGISTERUSER_9999";

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = GetTableColmunList(factoryid, RegisterTalbe);

                #region 构造注册信息sql语句

                /*
                    INSERT INTO T_SGM_SHAKE_REGISTERUSER_9999 Q (,,,,,) VALUES(,,,,,)
                 */

                string result = null;
                StringBuilder strFWSQL = new StringBuilder("INSERT INTO " + RegisterTalbe + "(");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 动态构造需要添加的字段
                        //判断注册信息项是否包含在表中
                        string currentItem = userinfo.Key.ToString().ToUpper();
                        if (colnumList.Contains(currentItem))
                        {
                            strFWSQL.Append(currentItem);
                            strFWSQL.Append(",");
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                result = strFWSQL.ToString();
                result = result.Substring(0, result.Length - 1);
                strFWSQL = new StringBuilder(result);
                strFWSQL.Append(") VALUES (");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 动态构造需要添加的值
                        //判断注册信息项是否包含在表中
                        string currentItem = userinfo.Key.ToString().ToUpper();
                        if (colnumList.Contains(currentItem))
                        {
                            strFWSQL.Append("'");
                            strFWSQL.Append(userinfo.Value);
                            strFWSQL.Append("'");
                            strFWSQL.Append(",");
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                result = strFWSQL.ToString();
                result = result.Substring(0, result.Length - 1);
                strFWSQL = new StringBuilder(result);
                strFWSQL.Append(")");
                #endregion

                //如表不存在则直接返回注册失败
                if (sqlfalg)
                {
                    return false;
                }
                else
                {
                    sql = strFWSQL.ToString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDal :InsertRechargeRegister:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 点击homepage页面(组建sql)

        /// <summary>
        /// 点击homepage页面(组建sql)
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertRechargerHomeClick(string factoryid, Hashtable userHash, out  string sql)
        {
            sql = "";
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //homepageclick记录表
                string homepageclickTalbe = "T_SGM_SHAKE_CHECKTOLOG";

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = GetTableColmunList(factoryid, homepageclickTalbe);

                #region 构造注册信息sql语句

                /*
                    INSERT INTO T_SGM_SHAKE_REGISTERUSER_9999 Q (,,,,,) VALUES(,,,,,)
                 */

                string result = null;
                StringBuilder strFWSQL = new StringBuilder("INSERT INTO " + homepageclickTalbe + "(");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 动态构造需要添加的字段
                        //判断注册信息项是否包含在表中
                        string currentItem = userinfo.Key.ToString().ToUpper();
                        if (colnumList.Contains(currentItem))
                        {
                            strFWSQL.Append(currentItem);
                            strFWSQL.Append(",");
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                result = strFWSQL.ToString();
                result = result.Substring(0, result.Length - 1);
                strFWSQL = new StringBuilder(result);
                strFWSQL.Append(") VALUES (");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 动态构造需要添加的值
                        //判断注册信息项是否包含在表中
                        string currentItem = userinfo.Key.ToString().ToUpper();
                        if (colnumList.Contains(currentItem))
                        {
                            strFWSQL.Append("'");
                            strFWSQL.Append(userinfo.Value);
                            strFWSQL.Append("'");
                            strFWSQL.Append(",");
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                result = strFWSQL.ToString();
                result = result.Substring(0, result.Length - 1);
                strFWSQL = new StringBuilder(result);
                strFWSQL.Append(")");
                #endregion

                //如表不存在则直接返回注册失败
                if (sqlfalg)
                {
                    return false;
                }
                else
                {
                    sql = strFWSQL.ToString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDal :InsertRechargerHomeClick:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion



        #region 9)获取表结构
        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<String> GetTableColmunList(string directoryName, string tableName)
        {
            List<String> colnumList = new List<string>();
            try
            {
                IDataParameter[] param = ParameterCache.GetParams("GetTableAllColmunsParam");
                if (param == null)
                {
                    param = new IDataParameter[1];
                    param[0] = new OracleParameter(PARAM_TABLENAME, OracleType.VarChar, 60);
                    ParameterCache.PushCache("GetTableAllColmunsParam", param);
                }
                param[0].Value = tableName;
                DataBase dataBase = new DataBase();
                DataTable dbRet = dataBase.GetDataSet(CommandType.Text, GET_TABLE_ALLCOLMUNS_SQL, param).Tables[0];
                if (dbRet != null && dbRet.Rows.Count > 0)
                {
                    foreach (DataRow row in dbRet.Rows)
                    {
                        colnumList.Add(row[0].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDal :GetTableColmunList:" + directoryName + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return colnumList;
        }
        #endregion


        #region 检测是否是中信码
        /// <summary>
        /// 检测是否是中信码
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// <param name="spUse"></param>
        /// <param name="spMobile"></param>
        /// <returns></returns>
        public bool CheckSpCodeExsit(string facid, string code, out  bool spUse, out  string spMobile)
        {

            spUse = false;
            spMobile = "";
            bool bRet = false;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[2];
                param[0] = new OracleParameter(":DIGIT", OracleType.VarChar, 16);
                param[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = code;
                param[1].Value = facid;

                // DataBase dataBase = new DataBase();

                string sql = "SELECT * FROM T_SGM_STCODE S WHERE S.FLAG='1' AND S.FACID=:FACID AND S.CODE=:DIGIT";

                DataTable dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    bRet = true;
                    string isused = dt.Rows[0]["USE"].ToString();
                    spMobile = dt.Rows[0]["MOBILE"].ToString();
                    if (isused == "1")
                    {
                        spUse = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 检测是否是中信码  出现异常 CheckSpCodeExsit  [facid:" + facid + "]  [code:" + code + "]   ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion

        #region 查询预警人
        /// <summary>
        /// 检测是否是中信码
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="code"></param>
        /// <param name="spUse"></param>
        /// <param name="spMobile"></param>
        /// <returns></returns>
        public DataTable SelectRemindMan(string facid)
        {
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[1];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar);

                param[0].Value = facid;

                DataTable dt = dataBase.GetDataSet(CommandType.Text, SELECT_SHAKE_REMIND_SQL, param).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao 检测是否是中信码  出现异常 SelectRemindMan  [facid:" + facid + "]   ----" + ex.Message + "---" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite + " --" + DateTime.Now.ToString() + "\n\r", Logger.AppLog.LogMessageType.Fatal);

            }
            return null;
        }
        #endregion


        #region 获取IP/电话/手机/区号所在的省份和城市

        /// <summary>
        /// 获取IP所在的省份和城市
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="provice">所在省份</param>
        /// <param name="city">所在城市</param>
        /// <returns></returns>
        public Boolean QueryProviceAndCityByIP(string ip, ref string provice, ref string city)
        {
            Boolean bRet = false;
            string[] strIpArr = ip.Split('.');
            Int64 nIpOne = Int64.Parse(strIpArr[0]) * 256 * 256 * 256;
            Int64 nIpTwo = Int64.Parse(strIpArr[1]) * 256 * 256;
            Int64 nIpThree = Int64.Parse(strIpArr[2]) * 256;
            Int64 nIpFour = Int64.Parse(strIpArr[3]);
            string strIPLen = (nIpOne + nIpTwo + nIpThree + nIpFour).ToString();
            try
            {
                IDataParameter[] param = null;
                param = new IDataParameter[2];
                param[0] = new OracleParameter(":IP", OracleType.VarChar, 40);
                param[1] = new OracleParameter(":IP1", OracleType.VarChar, 40);

                param[0].Value = strIPLen;
                param[1].Value = strIPLen;
                DataBase dataBase = new DataBase();

                DataTable ipList = dataBase.ExecuteQuery(CommandType.Text, "SELECT P.PROVINCE,P.CITY FROM T_REPORT_DICT_IPV4 P WHERE P.START_IP<=:IP AND P.END_IP>=:IP1", param);

                if (ipList != null && ipList.Rows.Count > 0)
                {
                    provice = ipList.Rows[0]["PROVINCE"].ToString();
                    city = ipList.Rows[0]["CITY"].ToString();
                }


                if (city.Length > 10)
                {
                    city = "未知地区";
                }
                if (provice.Length > 10)
                {
                    provice = "未知地区";
                }

                ///////////////////////////////////////////////////////////
                if (!String.IsNullOrEmpty(provice) && !String.IsNullOrEmpty(city))
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {

                Logger.AppLog.Write("QueryProviceAndCityByIP:IP[" + ip + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + ip, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }


        /// <summary>
        /// 根据手机号所在的省份和城市
        /// </summary>
        /// <param name="phoneNo">电话号码</param>
        /// <param name="provice">所在省份</param>
        /// <param name="city">所在城市</param>
        /// <returns></returns>
        public Boolean QueryProviceAndCityByPhoneAndMobile(string phoneNo, ref string provice, ref string city)
        {
            Boolean bRet = false;

            string strSQL = "";
            IDataParameter[] param;

            string zone_code = string.Empty;
            try
            {
                if (phoneNo.Length == 11 && phoneNo.Substring(0, 1) == "1")
                {
                    strSQL = GET_CITYNAME_BY_MOBILENEW;

                    param = new IDataParameter[1];
                    param[0] = new OracleParameter(":MOBILE_CODE", OracleType.VarChar, 10);

                    param[0].Value = phoneNo.Substring(0, 7);


                    DataBase dataBase = new DataBase();
                    DataTable dtRet = dataBase.ExecuteQuery(CommandType.Text, strSQL, param);

                    if (dtRet != null && dtRet.Rows.Count > 0)
                    {
                        zone_code = dtRet.Rows[0]["ZONE_CODE"].ToString();
                    }
                    GetCityByCode(zone_code, out provice, out city);

                }

                ///////////////////////////////////////////////////////////
                if (!String.IsNullOrEmpty(provice) && !String.IsNullOrEmpty(city))
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("QueryProviceAndCityByPhoneAndMobile:phoneNo[" + phoneNo + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + phoneNo, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }


        /// <summary>
        /// 根据区号获取城市
        /// </summary>
        /// <param name="cityCode">区号</param>
        /// <returns></returns>
        public bool GetCityByCode(string cityCode, out string provice, out string cityname)
        {
            Boolean bRet = false;
            provice = "";
            cityname = "";
            IDataParameter[] param;
            try
            {
                param = new IDataParameter[1];
                param[0] = new OracleParameter(":ZONE_CODE", OracleType.VarChar, 10);
                param[0].Value = cityCode;

                DataBase dataBase = new DataBase();
                DataTable dtRet = dataBase.ExecuteQuery(CommandType.Text, GET_CITYNAME_BY_CITYCODE_NEW, param);

                if (dtRet != null && dtRet.Rows.Count > 0)
                {
                    provice = dtRet.Rows[0]["PROVINCE_NAME"].ToString();
                    cityname = dtRet.Rows[0]["CITY_NAME"].ToString();
                }
                else
                {
                    provice = "未知地区";
                    cityname = "未知地区";
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("GetCityByCode:cityCode[" + cityCode + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + cityCode, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }
        #endregion
    }
}
