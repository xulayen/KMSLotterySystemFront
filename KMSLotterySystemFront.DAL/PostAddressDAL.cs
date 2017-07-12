using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.Model;
using KMSLotterySystemFront.DBUtility;
using System.Data.OracleClient;
using System.Data;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront.DAL
{
    public class PostAddressDAL
    {
        private static string POSTADD_TABLE = "T_SGM_POSTADDR_";

        /// <summary>
        /// 添加一个地址
        /// </summary>
        private string SQL_INSERT_POSTADD = "insert into {0} (POSTGUID, USERGUID, USERTYPE, PROVINCEID, CITYID, COUNTYID, ZIP, POSTNAME, MOBILE, TELPHONE, POSTADDR,facid) values(:POSTGUID, :USERGUID, :USERTYPE, :PROVINCEID, :CITYID, :COUNTYID, :ZIP, :POSTNAME, :MOBILE, :TELPHONE, :POSTADDR,:FacID)";

        #region 添加地址管理
        /// <summary>
        /// 得到用户所有的地址管理
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool AddPostAdd(string facid, PostAddEntity Info)
        {
            DataBase dataBase = new DataBase();
            bool result = false;
            try
            {
                //:POSTGUID, :USERGUID, :USERTYPE, :PROVINCEID, :CITYID, :COUNTYID, :ZIP, :POSTNAME, :MOBILE, :TELPHONE, :POSTADDR,  :CREATEDATE
                OracleParameter[] param = null;

                #region 构造参数
                param = (OracleParameter[])ParameterCache.GetParams("AddPostAddParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[12];

                    param[0] = new OracleParameter(":POSTGUID", OracleType.VarChar);
                    param[1] = new OracleParameter(":USERGUID", OracleType.VarChar);
                    param[2] = new OracleParameter(":USERTYPE", OracleType.VarChar);
                    param[3] = new OracleParameter(":PROVINCEID", OracleType.VarChar);
                    param[4] = new OracleParameter(":CITYID", OracleType.VarChar);
                    param[5] = new OracleParameter(":COUNTYID", OracleType.VarChar);
                    param[6] = new OracleParameter(":ZIP", OracleType.VarChar);
                    param[7] = new OracleParameter(":POSTNAME", OracleType.VarChar);
                    param[8] = new OracleParameter(":MOBILE", OracleType.VarChar);
                    param[9] = new OracleParameter(":TELPHONE", OracleType.VarChar);
                    param[10] = new OracleParameter(":POSTADDR", OracleType.VarChar);
                    param[11] = new OracleParameter(":FacID", OracleType.VarChar);

                    //将参数加入缓存
                    ParameterCache.PushCache("AddPostAddParam", param);
                }
                param[0].Value = Guid.NewGuid().ToString().Replace("-", "");
                param[1].Value = Info.Userguid;
                param[2].Value = Info.Usertype;
                param[3].Value = Info.Provinceid;
                param[4].Value = Info.Cityid;
                param[5].Value = Info.Countyid;
                param[6].Value = Info.Zip;
                param[7].Value = Info.Postname;
                param[8].Value = Info.Mobile;
                param[9].Value = Info.Telphone;
                param[10].Value = Info.Postaddr;
                param[11].Value = facid;
                #endregion

                string table = GetTable(POSTADD_TABLE, facid);

                string sql = string.Format(SQL_INSERT_POSTADD, table);

                int row = dataBase.ExecuteNonQuery(CommandType.Text, sql, param);
                if (row > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                //CommonLogger.Instance.WriteDBLog(facid, LogBigType.ExceptionLog, LogSubType.AccountLog, "添加地址管理异常:" + ex.Message, "", strUserType, strHierarchy);
            }
            return result;
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
        string GetNewFactoryTalbe(string facid)
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
                Logger.AppLog.Write("PostAddressDAL:GetNewFactoryTalbe:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return oRet;
            }
            return oRet;
        }

        #endregion
    }
}
