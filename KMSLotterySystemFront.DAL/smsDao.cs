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
    public class smsDao
    {

        #region sql列表

        private static string MOBILETBLE = "T_SGM_VERIFYCODE";
        private string ADD_MOBILE_ACITVECODE_SQL = " INSERT INTO " + MOBILETBLE + "(USERID,MOBILE,VERIFCODE,IP,FACID) VALUES(:USERID,:MOBILE,:VERIFYCODE,:IP,:FACID)";
        private string GetVerifyMobile_SQL = "SELECT MOBILE FROM " + MOBILETBLE + " WHERE FLAG='0' AND FacID=:FACID AND MOBILE=:MOBILE AND VERIFCODE=:VERIFYCODE ";

        private string VerifyMobile_SQL = "SELECT MOBILE FROM " + MOBILETBLE + " WHERE FLAG='0' AND FacID=:FACID AND MOBILE=:MOBILE AND VERIFCODE=:VERIFYCODE AND SYSDATE <= (CREATETIME + {0} / 24 / 60) ";

        private string ModifyVerifyState = "UPDATE " + MOBILETBLE + " SET FLAG='1',VERIFYDATE=SYSDATE WHERE MOBILE=:MOBILE AND FacID=:FACID AND FLAG='0' AND VERIFCODE=:VERIFYCODE ";


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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="mobile"></param>
        /// <param name="vifycode"></param>
        /// <param name="ip"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public int AddVerifyCode(string userid, string mobile, string vifycode, string ip, string facid)
        {
            int iRet = 0;

            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[5];
                param[0] = new OracleParameter(":USERID", OracleType.VarChar, 50);
                param[1] = new OracleParameter(":MOBILE", OracleType.VarChar, 20);
                param[2] = new OracleParameter(":VERIFYCODE", OracleType.VarChar, 6);
                param[3] = new OracleParameter(":IP", OracleType.VarChar, 16);
                param[4] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = userid;
                param[1].Value = mobile;
                param[2].Value = vifycode;
                param[3].Value = ip;
                param[4].Value = facid;

                DataBase dataBase = new DataBase();
                return dataBase.ExecuteNonQuery(CommandType.Text, ADD_MOBILE_ACITVECODE_SQL, param);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:AddVerifyCode:" + facid + "---" + mobile + "---" + vifycode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return iRet;
            }
        }

        public object GetVerifyMobile(string facId, string mobile, string vifycode)
        {
            object oRet = null;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[3];

                param[0] = new OracleParameter(":MOBILE", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":VERIFYCODE", OracleType.VarChar, 6);
                param[2] = new OracleParameter(":FACID", OracleType.VarChar, 10);

                param[0].Value = mobile;
                param[1].Value = vifycode;
                param[2].Value = facId;

                DataBase dataBase = new DataBase();

                return dataBase.ExecuteScalar(CommandType.Text, GetVerifyMobile_SQL, param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:GetVerifyMobile:" + facId + "---" + mobile + "---" + vifycode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facId"></param>
        /// <param name="mobile"></param>
        /// <param name="vifycode"></param>
        /// <param name="validTime"></param>
        /// <returns></returns>
        public bool VerifyMobile(string facId, string mobile, string vifycode, int validTime)
        {
            bool bRet = false;
            try
            {

                DataBase dataBase = new DataBase();
                dataBase.BeginTrans();
                try
                {
                    int nRet1 = 0;
                    int nRet2 = 0;

                    OracleParameter[] param = null;
                    param = new OracleParameter[3];

                    param[0] = new OracleParameter(":MOBILE", OracleType.VarChar, 20);
                    param[1] = new OracleParameter(":VERIFYCODE", OracleType.VarChar, 6);
                    param[2] = new OracleParameter(":FACID", OracleType.VarChar, 10);


                    param[0].Value = mobile;
                    param[1].Value = vifycode;
                    param[2].Value = facId;

                    VerifyMobile_SQL = string.Format(VerifyMobile_SQL, validTime);

                    object obj = dataBase.ExecuteScalar(CommandType.Text, VerifyMobile_SQL, param);
                    if (obj != null)
                    {
                        if (obj.ToString().Equals(mobile))
                        {
                            nRet1 = 1;
                        }
                    }

                    

                    OracleParameter[] param2 = null;
                    param2 = new OracleParameter[3];

                    param2[0] = new OracleParameter(":MOBILE", OracleType.VarChar, 20);
                    param2[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    param2[2] = new OracleParameter(":VERIFYCODE", OracleType.VarChar, 6);

                    param2[0].Value = mobile;
                    param2[1].Value = facId;
                    param2[2].Value = vifycode;

                    nRet2 = dataBase.ExecuteNonQuery(CommandType.Text, ModifyVerifyState, param2);

                    if ((nRet2 + nRet1) >= 2)
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
                Logger.AppLog.Write("smsBLL:VerifyMobile:" + facId + "---" + mobile + "---" + vifycode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facId"></param>
        /// <param name="mobile"></param>
        /// <param name="vifycode"></param>
        /// <param name="validTime"></param>
        /// <returns></returns>
        public DataTable GetVerifyMobileByMobile(string facId, string mobile)
        {
            DataTable oRet = null;
            try
            {
                OracleParameter[] param = null;
                param = new OracleParameter[2];

                param[0] = new OracleParameter(":MOBILE", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                 

                param[0].Value = mobile;
                param[1].Value = facId;

                DataBase dataBase = new DataBase();

                return dataBase.ExecuteQuery(CommandType.Text, "SELECT  S.MESSAGE,S.CREATE_DATE,S.SEND_DATE FROM T_SMS_INTL_SEND S WHERE S.FACTORYID=:FACTORYID AND S.DEST_MSISDN=:MOBILE ORDER BY S.CREATE_DATE DESC", param);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:GetVerifyMobile:" + facId + "---" + mobile  + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
        }

    }
}
