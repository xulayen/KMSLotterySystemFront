// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DAL
// *文件名称：DigitcodeDao.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：数码相关操作数据访问层
// *创建描述：
//
// *修改信息：20160329 将IP地址获取地址信息调整为新表
// *修改备注：修改人 金志新 jinzhixin@yesno.com.cn
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KMSLotterySystemFront.DBUtility;
using System.Data.OracleClient;
using KMSLotterySystemFront.Logger;


namespace KMSLotterySystemFront.DAL
{
    public class DigitcodeDao
    {
        #region sql列表
        /// <summary>
        /// 获取IP所在城市
        /// </summary>
        private const String QUERY_PROVICEANDCITY_BYIP_SQL = "SELECT ZNAME FROM T_CCN_IPADD WHERE STARTIP<=:IP  AND  ENDIP>=:IP1";

        /// <summary>
        /// 根据电话号码获取省份和城市
        /// </summary>
        private const string GET_CITYNAME_BY_PHONE = "SELECT C.PROVINCE_NAME,C.CITY_NAME FROM T_DICT_O_CITY C WHERE C.CITY_CODE=:CITY_CODE1  OR C.CITY_CODE=:CITY_CODE2 OR  C.CITY_CODE=:CITY_CODE3";

        /// <summary>
        /// 根据手机号码获取省份和城市
        /// </summary>
        private const string GET_CITYNAME_BY_MOBILE = "SELECT STATE_NAME AS PROVINCE_NAME,CITY_NAME  FROM T_DICT_CITY_MOBILE WHERE MSISDN=:MSISDN";

        /// <summary>
        /// 根据手机号码获取省份和城市
        /// </summary>
        private const string GET_CITYNAME_BY_MOBILENEW = "SELECT M.ZONE_CODE FROM T_REPORT_DICT_MOBILE M WHERE M.MOBILE_CODE=:MOBILE_CODE";


        /// <summary>
        /// 根据取号获取城市
        /// </summary>
        private const string GET_CITYNAME_BY_CITYCODE = "SELECT CITY_NAME FROM T_DICT_O_CITY WHERE CITY_CODE=:CITY_CODE  AND ROWNUM<2";

        /// <summary>
        /// 根据取号获取城市
        /// </summary>
        private const string GET_CITYNAME_BY_CITYCODE_NEW = "SELECT Z.PROVINCE_NAME,Z.CITY_NAME  FROM T_REPORT_DICT_ZONE Z WHERE Z.ZONE_CODE=:ZONE_CODE  AND ROWNUM<2";


        /// <summary>
        /// 检测老厂家编号和新厂家编号是否可以对应
        /// </summary>
        private const String CHECK_FACTORYIDISNEW_SQL = "SELECT PRODUCTID FROM T_FAC_PRODUCT WHERE  FACTORYID=:FACTORYID AND FACIDOLD=:FACIDOLD";
        private const String PARAM_FACIDOLD = ":FACIDOLD";
        private const String PARAM_FACTORYID = ":FACTORYID";

        /// <summary>
        /// 检测厂家产品是否属于回传激活的产品
        /// </summary>
        private const String CHECK_FACPRO_ACT_SQL = "SELECT ISACTIVATIONFLAG FROM T_FAC_PRODUCT WHERE  FACTORYID=:FACTORYID AND PRODUCTID=:PRODUCTID";

        /// <summary>
        /// 从视图中检索表是否存在
        /// </summary>
        private const String CHECK_TABLE_IS_EXIST_SQL = "SELECT TABLE_NAME FROM CAT WHERE TABLE_NAME=:TABLE_NAME";
        private const String PARAM_TABLENAME = ":TABLE_NAME";
        private const String CHECK_COLUMN_IS_EXIST_SQL = "SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME=:TABLE_NAME AND COLUMN_NAME=:COLUNM_NAME";
        private const String PARAM_COLUNMNAME = ":COLUNM_NAME";
        private const String GET_TABLE_ALLCOLMUNS_SQL = "SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME=:TABLE_NAME";

        private const string CHECK_FACPRODUCT_SQL = "SELECT FACTORYID FROM T_FAC_PRODUCT P WHERE P.FACTORYID =:FACTORYID AND P.PRODUCTID=:PRODUCTID";

        private string SQL_ADD_InfoCode_T_1 = "INSERT INTO {0} (TRACETYPE,LPRODUCTCODE,IP,CATEGORY) VALUES(:tracetype ,:lproductcode ,:ip,:CATEGORY)";
        private string SQL_ADD_InfoCode_T_2 = "INSERT INTO {0} (TRACETYPE,LPRODUCTCODE,IP,CATEGORY,SPRODUCTCODE) VALUES(:tracetype ,:lproductcode ,:ip,:CATEGORY,:sproductcode)";

        private string SQL_ADD_InfoCode_T1 = "INSERT INTO {0} (TRACETYPE,LPRODUCTCODE,IP,ATTRIBUTE,CCNFAC,CCNPRO,OUTPROID,PROVINCENAME,CITYNAME,CATEGORY,CALLED,BUILDDATE) VALUES(:tracetype,:code,:ip ,:attribute,:ccnfac,:ccnpro,:outproid,:provice,:city, :CATEGORY,:called,:BUILDDATE)";
        private string SQL_ADD_InfoCode_T2 = "INSERT INTO {0} (TRACETYPE,LPRODUCTCODE,IP,ATTRIBUTE,CCNFAC,CCNPRO,OUTPROID,PROVINCENAME,CITYNAME,CATEGORY,CALLED,SPRODUCTCODE,BUILDDATE) VALUES(:tracetype,:code,:ip ,:attribute,:ccnfac,:ccnpro,:outproid,:provice,:city, :CATEGORY,:called,:newcode,:BUILDDATE)";

        private string SQL_Update_Num = "UPDATE {0} SET NUM=TO_NUMBER(NUM)+1 WHERE SPRODUCTCODE='{1}'";
        private string SQL_Update_CodeVisit1 = "UPDATE {0} SET VISIT='1',VDATE=SYSDATE,NUM=TO_NUMBER(NUM)+1 WHERE SPRODUCTCODE='{1}'";
        private string SQL_Update_CodeVisit2 = "UPDATE {0} SET VISIT='1',VDATE=SYSDATE WHERE SPRODUCTCODE='{1}'";

        private string SQL_GETCODEMEG_SQL = "SELECT X.LPRODUCTCODE,X.TDATE,X.IP,X.CATEGORY FROM {0} X WHERE X.LPRODUCTCODE=:LPRODUCTCODE AND X.TRACETYPE='S' AND ROWNUM=1";

        /// <summary>
        /// 获取数码防伪厂家产品表配置
        /// </summary>
        private const String QUERY_PRODUCTCODEINFO_BYCODE_SQL = "SELECT A.AGAINNUM,A.LOGICCOLOR,A.LOGICINFO,A.FACIDOLD,A.PRODUCTNAME,(SELECT B.FACTORYNAME FROM T_FAC_FACTORY B WHERE A.FACTORYID=B.FACTORYID) FACTORYNAME,CUSTODY,(SELECT FLOOR(SYSDATE-TO_DATE('{0}','YYYY-MM-DD')) FROM DUAL) CodeExistedDay,IS_NINEPALACEGRID,ISACTIVATIONFLAG FROM T_FAC_PRODUCT A WHERE  A.FACTORYID=:FACTORYID AND A.PRODUCTID=:PRODUCTID";

        #endregion

        #region 1)获取IP/电话/手机/区号所在的省份和城市
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
                AppLog.Write("QueryProviceAndCityByIP:IP[" + ip + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + ip, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }

        /// <summary>
        /// 获取IP所在的省份和城市
        /// </summary>
        /// <param name="phoneNo">电话号码</param>
        /// <param name="provice">所在省份</param>
        /// <param name="city">所在城市</param>
        /// <returns></returns>
        public Boolean QueryProviceAndCityByPhone1(string phoneNo, ref string provice, ref string city)
        {
            Boolean bRet = false;

            string strSQL = "";
            IDataParameter[] param;
            try
            {
                if (phoneNo.Length == 11 && phoneNo.Substring(0, 1) == "1")
                {
                    strSQL = GET_CITYNAME_BY_MOBILE;
                    param = ParameterCache.GetParams("QueryProviceAndCityByMobileParam");
                    if (param == null)
                    {
                        param = new IDataParameter[1];
                        param[0] = new OracleParameter(":MSISDN", OracleType.VarChar, 10);
                        ParameterCache.PushCache("QueryProviceAndCityByMobileParam", param);
                    }
                    param[0].Value = phoneNo.Substring(0, 7);
                }
                else
                {
                    strSQL = GET_CITYNAME_BY_PHONE;
                    param = ParameterCache.GetParams("QueryProviceAndCityByPhoneParam");
                    if (param == null)
                    {
                        param = new IDataParameter[3];
                        param[0] = new OracleParameter(":CITY_CODE1", OracleType.VarChar, 10);
                        param[1] = new OracleParameter(":CITY_CODE2", OracleType.VarChar, 10);
                        param[2] = new OracleParameter(":CITY_CODE3", OracleType.VarChar, 10);
                        ParameterCache.PushCache("QueryProviceAndCityByPhoneParam", param);
                    }
                    if (phoneNo.StartsWith("0"))
                    {
                        param[0].Value = phoneNo.Substring(0, 2);
                        param[1].Value = phoneNo.Substring(0, 3);
                        param[2].Value = phoneNo.Substring(0, 4);
                    }
                    else
                    {
                        param[0].Value = "0" + phoneNo.Substring(0, 2);
                        param[1].Value = "0" + phoneNo.Substring(0, 3);
                        param[2].Value = "0" + phoneNo.Substring(0, 4);
                    }
                }
                DataBase dataBase = new DataBase();
                DataTable dtRet = dataBase.ExecuteQuery(CommandType.Text, strSQL, param);
                if (dtRet != null && dtRet.Rows.Count > 0)
                {
                    provice = dtRet.Rows[0]["province_name"].ToString();
                    city = dtRet.Rows[0]["city_name"].ToString();
                }
                else
                {
                    provice = "未知地区";
                    city = "未知地区";
                }


                ///////////////////////////////////////////////////////////
                if (!String.IsNullOrEmpty(provice) && !String.IsNullOrEmpty(city))
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("QueryProviceAndCityByPhone:phoneNo[" + phoneNo + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + phoneNo, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }


        /// <summary>
        /// 获取IP所在的省份和城市
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
                AppLog.Write("QueryProviceAndCityByPhoneAndMobile:phoneNo[" + phoneNo + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + phoneNo, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }


        /// <summary>
        /// 获取IP所在的省份和城市
        /// </summary>
        /// <param name="phoneNo">电话号码</param>
        /// <param name="provice">所在省份</param>
        /// <param name="city">所在城市</param>
        /// <returns></returns>
        public Boolean QueryProviceAndCityByMobile(string phoneNo, ref string provice, ref string city)
        {
            Boolean bRet = false;

            string strSQL = "";
            IDataParameter[] param;
            try
            {
                if (phoneNo.Length == 11 && phoneNo.Substring(0, 1) == "1")
                {
                    strSQL = GET_CITYNAME_BY_MOBILE;
                    param = ParameterCache.GetParams("QueryProviceAndCityByMobileParam");
                    if (param == null)
                    {
                        param = new IDataParameter[1];
                        param[0] = new OracleParameter(":MSISDN", OracleType.VarChar, 10);
                        ParameterCache.PushCache("QueryProviceAndCityByMobileParam", param);
                    }
                    param[0].Value = phoneNo.Substring(0, 7);
                }
                else
                {
                    strSQL = GET_CITYNAME_BY_PHONE;
                    param = ParameterCache.GetParams("QueryProviceAndCityByPhoneParam");
                    if (param == null)
                    {
                        param = new IDataParameter[3];
                        param[0] = new OracleParameter(":CITY_CODE1", OracleType.VarChar, 10);
                        param[1] = new OracleParameter(":CITY_CODE2", OracleType.VarChar, 10);
                        param[2] = new OracleParameter(":CITY_CODE3", OracleType.VarChar, 10);
                        ParameterCache.PushCache("QueryProviceAndCityByPhoneParam", param);
                    }
                    if (phoneNo.StartsWith("0"))
                    {
                        param[0].Value = phoneNo.Substring(0, 2);
                        param[1].Value = phoneNo.Substring(0, 3);
                        param[2].Value = phoneNo.Substring(0, 4);
                    }
                    else
                    {
                        param[0].Value = "0" + phoneNo.Substring(0, 2);
                        param[1].Value = "0" + phoneNo.Substring(0, 3);
                        param[2].Value = "0" + phoneNo.Substring(0, 4);
                    }
                }
                DataBase dataBase = new DataBase();
                DataTable dtRet = dataBase.ExecuteQuery(CommandType.Text, strSQL, param);

                dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "FUN_GETCITYNAME", param);

                if (dtRet != null && dtRet.Rows.Count > 0)
                {
                    provice = dtRet.Rows[0]["province_name"].ToString();
                    city = dtRet.Rows[0]["city_name"].ToString();
                }
                else
                {
                    provice = "未知地区";
                    city = "未知地区";
                }


                ///////////////////////////////////////////////////////////
                if (!String.IsNullOrEmpty(provice) && !String.IsNullOrEmpty(city))
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("QueryProviceAndCityByPhone:phoneNo[" + phoneNo + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + phoneNo, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }

        /// <summary>
        /// 根据区号获取城市
        /// </summary>
        /// <param name="cityCode">区号</param>
        /// <returns></returns>
        public bool GetCityByCode(string cityCode, out string cityname)
        {
            Boolean bRet = false;
            cityname = "";
            IDataParameter[] param;
            try
            {
                param = ParameterCache.GetParams("QueryCityByCodeParam");
                if (param == null)
                {
                    param = new IDataParameter[1];
                    param[0] = new OracleParameter(":CITY_CODE", OracleType.VarChar, 10);
                    ParameterCache.PushCache("QueryCityByCodeParam", param);
                }
                param[0].Value = cityCode;

                DataBase dataBase = new DataBase();
                object oRet = dataBase.ExecuteScalar(CommandType.Text, GET_CITYNAME_BY_CITYCODE, param);
                if (oRet != null)
                {
                    bRet = true;
                    cityname = oRet.ToString();
                }
                else
                {
                    bRet = false;
                    cityname = "未知区域";
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("GetCityByCode:cityCode[" + cityCode + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + cityCode, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

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
                AppLog.Write("GetCityByCode:cityCode[" + cityCode + "]---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "--Param:" + cityCode, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

            }
            return bRet;
        }



        #endregion

        #region 2)检测老厂家编号和数码解析的厂家编号是否对应
        /// <summary>
        /// 检测老厂家编号和数码解析的厂家编号是否对应
        /// </summary>
        /// <param name="oldFactoryID">旧厂家编号</param>
        /// <param name="newFactoryID">数码中的新厂家编号</param>
        /// <param name="proID">数码中的产品编号</param>
        /// <returns></returns>
        public Boolean CheckFactoryIDIsNew(string oldFactoryID, string newFactoryID, string proID)
        {
            Boolean bRet = false;
            try
            {
                OracleParameter[] param = this.GetCheckFactoryIDIsNewParam(oldFactoryID, newFactoryID);
                if (param != null)
                {
                    DataBase dataBase = new DataBase();
                    object oRet = dataBase.ExecuteScalar(CommandType.Text, CHECK_FACTORYIDISNEW_SQL, param);
                    if (oRet != null)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(oldFactoryID + "---" + newFactoryID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                throw ex;
            }
            return bRet;
        }
        #endregion

        #region 3)获取检测老厂家编号和数码解析的厂家编号是否对应的参数
        /// <summary>
        /// 获取检测老厂家编号和数码解析的厂家编号是否对应的参数
        /// </summary>
        /// <param name="oldFactoryID">旧厂家编号</param>
        /// <param name="newFactoryID">新厂家编号</param>
        /// <returns></returns>
        private OracleParameter[] GetCheckFactoryIDIsNewParam(string oldFactoryID, string newFactoryID)
        {
            OracleParameter[] parms = null;
            try
            {
                parms = (OracleParameter[])ParameterCache.GetParams("GetCheckFactoryIDIsNewParam");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[2];
                    parms[0] = new OracleParameter(PARAM_FACTORYID, OracleType.VarChar, 5);
                    parms[1] = new OracleParameter(PARAM_FACIDOLD, OracleType.VarChar, 5);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCheckFactoryIDIsNewParam", parms);
                }
                parms[0].Value = newFactoryID;
                parms[1].Value = oldFactoryID;
            }
            catch (Exception ex)
            {
                parms = null;
                Logger.AppLog.Write(oldFactoryID + "---" + newFactoryID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return parms;
        }
        #endregion

        #region 4)检测表名是否存在
        /// <summary>
        /// 检测表名是否存在
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public Boolean CheckTableIsExist(string directoryName, string tableName)
        {
            Boolean bRet = false;
            try
            {
                OracleParameter[] parms = GetCheckTableIsExistParam(directoryName, tableName);
                if (parms != null)
                {
                    DataBase db = new DataBase();
                    object oRet = db.ExecuteScalar(CommandType.Text, CHECK_TABLE_IS_EXIST_SQL, parms);
                    if (oRet != null)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("检查表名是否存在,tableName：" + tableName + "*directoryName:" + directoryName + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                throw ex;
            }
            return bRet;
        }
        #endregion

        #region 5)获取检测表名是否存在的执行参数
        /// <summary>
        /// 获取方法GetTableNameByCode的执行参数
        /// </summary>
        /// <param name="directoryName">当前服务厂家</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        private OracleParameter[] GetCheckTableIsExistParam(string directoryName, string tableName)
        {
            OracleParameter[] parms = null;
            try
            {
                parms = (OracleParameter[])ParameterCache.GetParams("GetCheckTableIsExistParam");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[1];
                    parms[0] = new OracleParameter(PARAM_TABLENAME, OracleType.VarChar, 60);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCheckTableIsExistParam", parms);
                }
                parms[0].Value = tableName;
            }
            catch (Exception ex)
            {
                parms = null;
                Logger.AppLog.Write(directoryName + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return parms;
        }
        #endregion

        #region 6)获取检测表中某列是否存在的执行参数
        /// <summary>
        /// 获取检测表中某列是否存在的执行参数
        /// </summary>
        /// <param name="directoryName">当前服务厂家</param>
        /// <param name="tableName">表名</param>
        /// <param name="colunmName">列名</param>
        /// <returns></returns>
        private OracleParameter[] GetCheckColunmIsExistParam(string directoryName, string tableName, string colunmName)
        {
            OracleParameter[] parms = null;
            try
            {
                parms = (OracleParameter[])ParameterCache.GetParams("GetCheckColunmIsExistParam");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[2];
                    parms[0] = new OracleParameter(PARAM_TABLENAME, OracleType.VarChar, 60);
                    parms[1] = new OracleParameter(PARAM_COLUNMNAME, OracleType.VarChar, 100);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCheckColunmIsExistParam", parms);
                }
                parms[0].Value = tableName;
                parms[1].Value = colunmName;
            }
            catch (Exception ex)
            {
                parms = null;
                Logger.AppLog.Write(directoryName + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return parms;
        }
        #endregion

        #region 7)检测表名是否存在
        /// <summary>
        /// 检测表名是否存在,存在则返回True,不存在则返回False
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="tableName">表名</param>
        /// <param name="colunmName">列名</param>
        /// <returns>存在则返回True,不存在则返回False</returns>
        public Boolean CheckColunmIsExist(string directoryName, string tableName, string colunmName)
        {
            Boolean bRet = false;
            try
            {
                OracleParameter[] parms = GetCheckColunmIsExistParam(directoryName, tableName, colunmName);
                if (parms != null)
                {
                    DataBase db = new DataBase();
                    object oRet = db.ExecuteScalar(CommandType.Text, CHECK_COLUMN_IS_EXIST_SQL, parms);
                    if (oRet != null)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("检查列名是否存在,tableName:" + tableName + "*colunmName:" + colunmName + "*directoryName:" + directoryName + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                throw ex;
            }
            return bRet;
        }
        #endregion

        #region 8) 通过factoryid和productids判断T_FAC_PRODUCT表是否存在记录
        /// <summary>
        /// 通过factoryid和productids判断T_FAC_PRODUCT表是否存在记录
        /// </summary>
        /// <param name="_factoryid">厂家编号</param>
        /// <param name="_productid">产品编号</param>
        /// <returns></returns>
        public bool CheckFacProduct(string _factoryid, string _productid)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckFacProductParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":PRODUCTID", OracleType.Char, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckFacProductParam", param);
                }
                param[0].Value = _factoryid;
                param[1].Value = _productid;
                DataBase dataBase = new DataBase();
                object oRet = dataBase.ExecuteScalar(CommandType.Text, CHECK_FACPRODUCT_SQL, param);
                if (oRet != null)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(_factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;

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
                AppLog.Write("GetTableAllColmuns:" + ex.Message, AppLog.LogMessageType.Fatal);
            }
            return colnumList;
        }
        #endregion

        #region 10)通过sql获取数据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataBySql(string sql)
        {
            try
            {
                DataBase dataBase = new DataBase();
                DataSet dsRet = dataBase.GetDataSet(CommandType.Text, sql, null);
                return dsRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
        #endregion

        #region 11.1)增加数码防伪查询日志-以厂家编号
        private OracleParameter[] GetInsertCodeQueryLogParam(string tracetype, string lproductcode, string ip, string category)
        {

            OracleParameter[] parms = null;
            try
            {
                #region
                parms = (OracleParameter[])ParameterCache.GetParams("GetInsertCodeQueryLogParam");
                if (parms == null)
                {
                    parms = new OracleParameter[4];
                    parms[0] = new OracleParameter(":tracetype", OracleType.VarChar, 4);
                    parms[1] = new OracleParameter(":lproductcode", OracleType.VarChar, 21);
                    parms[2] = new OracleParameter(":ip", OracleType.VarChar);
                    parms[3] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    ParameterCache.PushCache("GetInsertCodeQueryLogParam", parms);
                }
                parms[0].Value = tracetype;
                parms[1].Value = lproductcode;
                parms[2].Value = ip;
                parms[3].Value = category;
                #endregion
            }
            catch (Exception ex)
            {
                parms = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11)组织T表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return parms;
        }
        #endregion

        #region 11.2)增加数码防伪查询日志-以厂家编号
        private OracleParameter[] GetInsertCodeQueryLogParam(string tracetype, string lproductcode, string ip, string sproductcode, string category)
        {

            OracleParameter[] parms = null;
            try
            {
                #region
                parms = (OracleParameter[])ParameterCache.GetParams("GetInsertCodeQueryLogParam2");
                if (parms == null)
                {
                    parms = new OracleParameter[5];
                    parms[0] = new OracleParameter(":tracetype", OracleType.VarChar, 4);
                    parms[1] = new OracleParameter(":lproductcode", OracleType.VarChar, 21);
                    parms[2] = new OracleParameter(":ip", OracleType.VarChar);
                    parms[3] = new OracleParameter(":sproductcode", OracleType.VarChar, 21);
                    parms[4] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    ParameterCache.PushCache("GetInsertCodeQueryLogParam2", parms);
                }
                parms[0].Value = tracetype;
                parms[1].Value = lproductcode;
                parms[2].Value = ip;
                parms[3].Value = sproductcode;
                parms[4].Value = category;
                #endregion
            }
            catch (Exception ex)
            {
                parms = null;
                KMSLotterySystemFront.Logger.AppLog.Write("11.2)组织T厂家产品表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return parms;
        }
        #endregion

        #region 12.1)增加数码防伪查询日志-以厂家编号非二次加密
        /// <summary>
        /// 增加数码防伪查询日志-以厂家编号非二次加密
        /// </summary>
        /// <param name="tracetype">查询类别(S,LD,UA..)</param>
        /// <param name="code">一次加密的数码</param>
        /// <param name="ip">查询者IP</param>
        /// <param name="attribute">attribute</param>
        /// <param name="ccnfac">数码生成厂家编号</param>
        /// <param name="ccnpro">数码生成产品编号</param>
        /// <param name="outproid">outproid</param>
        /// <param name="provice">所属地省份</param>
        /// <param name="city">所属城市</param>
        /// <param name="called">called</param>
        /// <param name="category">查询渠道(X,A,W,M)</param>
        /// <param name="codeCreateDate">生码日期</param>
        /// <returns></returns>
        private OracleParameter[] GetInsertCodeQueryLogParam(string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string provice, string city, string called, string category, string codeCreateDate)
        {

            OracleParameter[] parms = null;
            try
            {
                #region
                parms = (OracleParameter[])ParameterCache.GetParams("GetInsertCodeQueryLogParam3");
                if (parms == null)
                {
                    parms = new OracleParameter[12];
                    parms[0] = new OracleParameter(":tracetype", OracleType.VarChar, 4);
                    parms[1] = new OracleParameter(":code", OracleType.VarChar, 21);
                    parms[2] = new OracleParameter(":ip", OracleType.VarChar);
                    parms[3] = new OracleParameter(":attribute", OracleType.VarChar, 12);
                    parms[4] = new OracleParameter(":ccnfac", OracleType.VarChar, 5);
                    parms[5] = new OracleParameter(":ccnpro", OracleType.VarChar, 2);
                    parms[6] = new OracleParameter(":outproid", OracleType.VarChar, 20);
                    parms[7] = new OracleParameter(":provice", OracleType.VarChar, 20);
                    parms[8] = new OracleParameter(":city", OracleType.VarChar, 20);
                    parms[9] = new OracleParameter(":called", OracleType.VarChar, 15);
                    parms[10] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    parms[11] = new OracleParameter(":BUILDDATE", OracleType.VarChar, 20);
                    ParameterCache.PushCache("GetInsertCodeQueryLogParam3", parms);
                }
                parms[0].Value = tracetype;
                parms[1].Value = code;
                parms[2].Value = ip;
                parms[3].Value = attribute;
                parms[4].Value = ccnfac;
                parms[5].Value = ccnpro;
                parms[6].Value = outproid;
                parms[7].Value = provice;
                parms[8].Value = city;
                parms[9].Value = called;
                parms[10].Value = category;
                parms[11].Value = codeCreateDate;
                #endregion
            }
            catch (Exception ex)
            {
                parms = null;
                KMSLotterySystemFront.Logger.AppLog.Write("12.1)组织T表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return parms;
        }
        #endregion

        #region 12.2)增加数码防伪查询日志-以厂家编号二次加密
        /// <summary>
        /// 增加数码防伪查询日志-以厂家编号二次加密
        /// </summary>
        /// <param name="tracetype">查询类别(S,LD,UA..)</param>
        /// <param name="code">一次加密的数码</param>
        /// <param name="ip">查询者IP</param>
        /// <param name="attribute">attribute</param>
        /// <param name="ccnfac">数码生成厂家编号</param>
        /// <param name="ccnpro">数码生成产品编号</param>
        /// <param name="outproid">outproid</param>
        /// <param name="provice">所属地省份</param>
        /// <param name="city">所属城市</param>
        /// <param name="called">called</param>
        /// <param name="newcode">二次加密的数码</param>
        /// <param name="category">查询渠道(X,A,W,M)</param>
        /// <param name="codeCreateDate">生码日期</param>
        /// <returns></returns>
        private OracleParameter[] GetInsertCodeQueryLogParam(string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string provice, string city, string called, string newcode, string category, string codeCreateDate)
        {

            OracleParameter[] parms = null;
            try
            {
                #region
                parms = (OracleParameter[])ParameterCache.GetParams("GetInsertCodeQueryLogParam4");
                if (parms == null)
                {
                    parms = new OracleParameter[13];
                    parms[0] = new OracleParameter(":tracetype", OracleType.VarChar, 4);
                    parms[1] = new OracleParameter(":code", OracleType.VarChar, 21);
                    parms[2] = new OracleParameter(":ip", OracleType.VarChar);
                    parms[3] = new OracleParameter(":attribute", OracleType.VarChar, 12);
                    parms[4] = new OracleParameter(":ccnfac", OracleType.VarChar, 5);
                    parms[5] = new OracleParameter(":ccnpro", OracleType.VarChar, 2);
                    parms[6] = new OracleParameter(":outproid", OracleType.VarChar, 20);
                    parms[7] = new OracleParameter(":provice", OracleType.VarChar, 20);
                    parms[8] = new OracleParameter(":city", OracleType.VarChar, 20);
                    parms[9] = new OracleParameter(":called", OracleType.VarChar, 15);
                    parms[10] = new OracleParameter(":newcode", OracleType.VarChar, 21);
                    parms[11] = new OracleParameter(":CATEGORY", OracleType.VarChar, 1);
                    parms[12] = new OracleParameter(":BUILDDATE", OracleType.VarChar, 20);
                    ParameterCache.PushCache("GetInsertCodeQueryLogParam4", parms);
                }
                parms[0].Value = tracetype;
                parms[1].Value = code;
                parms[2].Value = ip;
                parms[3].Value = attribute;
                parms[4].Value = ccnfac;
                parms[5].Value = ccnpro;
                parms[6].Value = outproid;
                parms[7].Value = provice;
                parms[8].Value = city;
                parms[9].Value = called;
                parms[10].Value = newcode;
                parms[11].Value = category;
                parms[12].Value = codeCreateDate;
                #endregion
            }
            catch (Exception ex)
            {
                parms = null;
                KMSLotterySystemFront.Logger.AppLog.Write("12.2)组织T表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return parms;
        }
        #endregion

        #region 13)记录T表日志(数码复查)
        /// <summary>
        /// 复查记录P表、T厂家表，T厂家产品表
        /// </summary>
        /// <param name="tableName_p">P表表名</param>
        /// <param name="tableName_t">T厂家表表名</param>
        /// <param name="tableName_tp">T厂家产品表表名</param>
        /// <param name="tracetype">查询类型</param>
        /// <param name="code">输入的数码</param>
        /// <param name="ip">客户端查询IP地址</param>
        /// <param name="attribute">批次</param>
        /// <param name="ccnfac">ccnfac</param>
        /// <param name="ccnpro">ccnpro</param>
        /// <param name="outproid">outproid</param>
        /// <param name="called">called</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="isEncypt">是否二次加密</param>
        /// <param name="codeCreateDate">数码生成日期</param>
        /// <returns>是否成功</returns>
        public bool ModifyCodeNotFirstQueryInfo(string tableName_p, string tableName_t, string tableName_tp, string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string called, string newcode, string channel, bool isEncypt, string codeCreateDate)
        {
            bool bRet = false;
            try
            {
                bool flag_t = false; //检查T表中是否存在SPRODUCTCODE字段
                bool flag_tp = false;//检查T产品表中是否存在SPRODUCTCODE字段

                OracleParameter[] param2 = null;
                OracleParameter[] param3 = null;
                //检查T总表的SPRODUCTCODE是否存在
                //bool checktflag_t = CheckFieldInTable(tableName_t, "SPRODUCTCODE");
                //检查T子表的SPRODUCTCODE是否存在 
                //bool checktflag_tp = CheckFieldInTable(tableName_tp, "SPRODUCTCODE");


                #region 获取IP或者电话所在的省份和城市
                string provice = "未知地区";
                string city = "未知地区";
                try
                {

                    if (channel.Equals("S") || channel.Equals("T"))
                    {
                        QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref city);
                    }
                    else
                    {
                        QueryProviceAndCityByIP(ip, ref provice, ref city);
                    }

                }
                catch (Exception exx)
                {
                    Logger.AppLog.Write(exx.StackTrace + "---" + exx.Source + "---" + exx.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                }

                #endregion

                #region 检查T厂家表和T厂家产品表是否存在sproductcode字段
                if (CheckColunmIsExist(ccnfac, tableName_t, "SPRODUCTCODE") && isEncypt)
                {
                    flag_t = true;
                }

                if (CheckColunmIsExist(ccnfac, tableName_tp, "SPRODUCTCODE") && isEncypt)
                {
                    flag_tp = true;
                }
                #endregion

                #region 组织更新T厂家表参数
                if (flag_t)
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, newcode, channel);
                }
                else
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, channel);
                }
                #endregion

                #region 组织更新T厂家产品表参数
                if (flag_tp)
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, newcode, channel, codeCreateDate);
                }
                else
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, channel, codeCreateDate);
                }
                #endregion

                if (param2 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;

                        #region 更新P表
                        //回传激活设计UA条件下不更新P表 begin 20130314 jinzhixin
                        if (tracetype != "UA")
                        {
                            if (CheckColunmIsExist(ccnfac, tableName_p, "NUM"))
                            {
                                string strSQL = string.Format(SQL_Update_Num, tableName_p, newcode);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            }
                            else
                            {
                                nRet1 = 1;
                            }
                        }
                        else
                        {
                            nRet1 = 1;
                        }
                        //回传激活设计UA条件下不更新P表 end 20130314 jinzhixin
                        #endregion

                        #region 更新T厂家表
                        if (flag_t)
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_2, tableName_t), param2);
                        }
                        else
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_1, tableName_t), param2);
                        }
                        #endregion

                        #region 更新T厂家产品表
                        if (flag_tp)
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T2, tableName_tp), param3);
                        }
                        else
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T1, tableName_tp), param3);
                        }
                        #endregion

                        #region 如果P表、T厂家表、T厂家产品表数据都更新成功回滚或者提交事务
                        if ((nRet1 + nRet2 + nRet3) == 3)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("13)记录T厂家产品表日志----" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }


                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("13)记录T厂家产品表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 13)记录T表日志(数码复查)  GPS
        /// <summary>
        /// 复查记录P表、T厂家表，T厂家产品表  GPS
        /// </summary>
        /// <param name="tableName_p">P表表名</param>
        /// <param name="tableName_t">T厂家表表名</param>
        /// <param name="tableName_tp">T厂家产品表表名</param>
        /// <param name="tracetype">查询类型</param>
        /// <param name="code">输入的数码</param>
        /// <param name="ip">客户端查询IP地址</param>
        /// <param name="attribute">批次</param>
        /// <param name="ccnfac">ccnfac</param>
        /// <param name="ccnpro">ccnpro</param>
        /// <param name="outproid">outproid</param>
        /// <param name="called">called</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="isEncypt">是否二次加密</param>
        /// <param name="codeCreateDate">数码生成日期</param>
        /// <param name="isGPS">IP是否为GPS定位数据</param>
        /// <param name="ProvinceName">根据GPS定位获取的省份名称</param>
        /// <param name="CityName">根据GPS定位获取的城市名称</param>
        /// <returns>是否成功</returns>
        public bool ModifyCodeNotFirstQueryInfo_GPS(string tableName_p, string tableName_t, string tableName_tp, string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string called, string newcode, string channel, bool isEncypt, string codeCreateDate, bool isGPS, string ProvinceName, string CityName)
        {
            bool bRet = false;
            try
            {
                bool flag_t = false; //检查T表中是否存在SPRODUCTCODE字段
                bool flag_tp = false;//检查T产品表中是否存在SPRODUCTCODE字段

                OracleParameter[] param2 = null;
                OracleParameter[] param3 = null;
                //检查T总表的SPRODUCTCODE是否存在
                //bool checktflag_t = CheckFieldInTable(tableName_t, "SPRODUCTCODE");
                //检查T子表的SPRODUCTCODE是否存在 
                //bool checktflag_tp = CheckFieldInTable(tableName_tp, "SPRODUCTCODE");


                #region 获取IP或者电话所在的省份和城市
                string provice = "未知地区";
                string city = "未知地区";

                try
                {
                    if (isGPS)
                    {
                        provice = ProvinceName;
                        city = CityName;
                    }
                    else
                    {
                        if (channel.Equals("S") || channel.Equals("T"))
                        {
                            QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref city);
                        }
                        else
                        {
                            QueryProviceAndCityByIP(ip, ref provice, ref city);
                        }
                    }
                }
                catch (Exception exx)
                {
                    Logger.AppLog.Write(exx.StackTrace + "---" + exx.Source + "---" + exx.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                }

                #endregion

                #region 检查T厂家表和T厂家产品表是否存在sproductcode字段
                if (CheckColunmIsExist(ccnfac, tableName_t, "SPRODUCTCODE") && isEncypt)
                {
                    flag_t = true;
                }

                if (CheckColunmIsExist(ccnfac, tableName_tp, "SPRODUCTCODE") && isEncypt)
                {
                    flag_tp = true;
                }
                #endregion

                #region 组织更新T厂家表参数
                if (flag_t)
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, newcode, channel);
                }
                else
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, channel);
                }
                #endregion

                #region 组织更新T厂家产品表参数
                if (flag_tp)
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, newcode, channel, codeCreateDate);
                }
                else
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, channel, codeCreateDate);
                }
                #endregion

                if (param2 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;

                        #region 更新P表
                        //回传激活设计UA条件下不更新P表 begin 20130314 jinzhixin
                        if (tracetype != "UA")
                        {
                            if (CheckColunmIsExist(ccnfac, tableName_p, "NUM"))
                            {
                                string strSQL = string.Format(SQL_Update_Num, tableName_p, newcode);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            }
                            else
                            {
                                nRet1 = 1;
                            }
                        }
                        else
                        {
                            nRet1 = 1;
                        }
                        //回传激活设计UA条件下不更新P表 end 20130314 jinzhixin
                        #endregion

                        #region 更新T厂家表
                        if (flag_t)
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_2, tableName_t), param2);
                        }
                        else
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_1, tableName_t), param2);
                        }
                        #endregion

                        #region 更新T厂家产品表
                        if (flag_tp)
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T2, tableName_tp), param3);
                        }
                        else
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T1, tableName_tp), param3);
                        }
                        #endregion

                        #region 如果P表、T厂家表、T厂家产品表数据都更新成功回滚或者提交事务
                        if ((nRet1 + nRet2 + nRet3) == 3)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("13)记录T厂家产品表日志----" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }


                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("13)记录T厂家产品表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 14)记录T表日志（数码首查）
        /// <summary>
        /// 首次查询记录P表、T厂家表，T厂家产品表
        /// </summary>
        /// <param name="tableName_p">P表表名</param>
        /// <param name="tableName_t">T厂家表表名</param>
        /// <param name="tableName_tp">T厂家产品表表名</param>
        /// <param name="tracetype">查询类型</param>
        /// <param name="code">输入的数码</param>
        /// <param name="ip">客户端查询IP地址</param>
        /// <param name="attribute">批次</param>
        /// <param name="ccnfac">ccnfac</param>
        /// <param name="ccnpro">ccnpro</param>
        /// <param name="outproid">outproid</param>
        /// <param name="called">called</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="isEncypt">是否二次加密</param>
        /// <param name="codeCreateDate">数码生成日期</param>
        /// <returns>是否成功</returns>  
        public bool ModifyCodeFirstQueryInfo(string tableName_p, string tableName_t, string tableName_tp, string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string called, string newcode, string channel, bool isEncypt, string codeCreateDate)
        {
            bool bRet = false;
            try
            {
                bool flag_t = false; //检查T表中是否存在SPRODUCTCODE字段
                bool flag_tp = false;//检查T产品表中是否存在SPRODUCTCODE字段

                OracleParameter[] param2 = null;
                OracleParameter[] param3 = null;
                ////检查T总表的SPRODUCTCODE是否存在
                //bool checktflag_t = CheckFieldInTable(tableName_t, "SPRODUCTCODE");
                ////检查T子表的SPRODUCTCODE是否存在 
                //bool checktflag_tp = CheckFieldInTable(tableName_tp, "SPRODUCTCODE");

                #region 获取IP或者电话所在的省份和城市
                string provice = "未知地区";
                string city = "未知地区";

                try
                {



                    if (channel.Equals("S") || channel.Equals("T"))
                    {
                        QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref city);
                    }
                    else
                    {
                        QueryProviceAndCityByIP(ip, ref provice, ref city);
                    }

                }
                catch (Exception exx)
                {
                    Logger.AppLog.Write(exx.StackTrace + "---" + exx.Source + "---" + exx.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                }

                #endregion

                #region 检查T厂家表和T厂家产品表是否存在sproductcode字段
                if (CheckColunmIsExist(ccnfac, tableName_t, "SPRODUCTCODE") && isEncypt)
                {
                    flag_t = true;
                }

                if (CheckColunmIsExist(ccnfac, tableName_tp, "SPRODUCTCODE") && isEncypt)
                {
                    flag_tp = true;
                }
                #endregion

                #region 组织更新T厂家表参数
                if (flag_t)
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, newcode, channel);
                }
                else
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, channel);
                }
                #endregion

                #region 组织更新T厂家产品表参数
                if (flag_tp)
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, newcode, channel, codeCreateDate);
                }
                else
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, channel, codeCreateDate);
                }
                #endregion

                if (param2 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;

                        #region 更新P表
                        //回传激活设计UA条件下不更新P表 begin 20130314 jinzhixin
                        if (tracetype != "UA")
                        {
                            if (CheckColunmIsExist(ccnfac, tableName_p, "NUM"))
                            {
                                string strSQL = string.Format(SQL_Update_CodeVisit1, tableName_p, newcode);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            }
                            else
                            {
                                string strSQL = string.Format(SQL_Update_CodeVisit2, tableName_p, newcode);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            }
                        }
                        else
                        {
                            nRet1 = 1;
                        }
                        //回传激活设计UA条件下不更新P表 end 20130314 jinzhixin
                        #endregion

                        #region 更新T厂家表
                        if (flag_t)
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_2, tableName_t), param2);
                        }
                        else
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_1, tableName_t), param2);
                        }
                        #endregion

                        #region 更新T厂家产品表
                        if (flag_tp)
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T2, tableName_tp), param3);
                        }
                        else
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T1, tableName_tp), param3);
                        }
                        #endregion

                        #region 如果P表、T厂家表、T厂家产品表数据都更新成功回滚或者提交事务
                        if ((nRet1 + nRet2 + nRet3) == 3)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("14)记录T厂家产品表日志----" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }


                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("14)记录T厂家产品表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 14.1)记录T表日志（数码首查） GPS
        /// <summary>
        /// 首次查询记录P表、T厂家表，T厂家产品表
        /// </summary>
        /// <param name="tableName_p">P表表名</param>
        /// <param name="tableName_t">T厂家表表名</param>
        /// <param name="tableName_tp">T厂家产品表表名</param>
        /// <param name="tracetype">查询类型</param>
        /// <param name="code">输入的数码</param>
        /// <param name="ip">客户端查询IP地址</param>
        /// <param name="attribute">批次</param>
        /// <param name="ccnfac">ccnfac</param>
        /// <param name="ccnpro">ccnpro</param>
        /// <param name="outproid">outproid</param>
        /// <param name="called">called</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="isEncypt">是否二次加密</param>
        /// <param name="codeCreateDate">数码生成日期</param>
        /// <param name="isGPS">IP是否为GPS定位数据</param>
        /// <param name="ProvinceName">根据GPS定位获取的省份名称</param>
        /// <param name="CityName">根据GPS定位获取的城市名称</param>
        /// <returns>是否成功</returns>  
        public bool ModifyCodeFirstQueryInfo_GPS(string tableName_p, string tableName_t, string tableName_tp, string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string called, string newcode, string channel, bool isEncypt, string codeCreateDate, bool isGPS, string ProvinceName, string CityName)
        {
            bool bRet = false;
            try
            {
                bool flag_t = false; //检查T表中是否存在SPRODUCTCODE字段
                bool flag_tp = false;//检查T产品表中是否存在SPRODUCTCODE字段

                OracleParameter[] param2 = null;
                OracleParameter[] param3 = null;
                ////检查T总表的SPRODUCTCODE是否存在
                //bool checktflag_t = CheckFieldInTable(tableName_t, "SPRODUCTCODE");
                ////检查T子表的SPRODUCTCODE是否存在 
                //bool checktflag_tp = CheckFieldInTable(tableName_tp, "SPRODUCTCODE");

                #region 获取IP或者电话所在的省份和城市
                string provice = "未知地区";
                string city = "未知地区";
                try
                {
                    if (isGPS)
                    {
                        provice = ProvinceName;
                        city = CityName;
                    }
                    else
                    {
                        if (channel.Equals("S") || channel.Equals("T"))
                        {
                            QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref city);
                        }
                        else
                        {
                            QueryProviceAndCityByIP(ip, ref provice, ref city);
                        }
                    }
                }
                catch (Exception exx)
                {
                    Logger.AppLog.Write(exx.StackTrace + "---" + exx.Source + "---" + exx.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                }

                #endregion

                #region 检查T厂家表和T厂家产品表是否存在sproductcode字段
                if (CheckColunmIsExist(ccnfac, tableName_t, "SPRODUCTCODE") && isEncypt)
                {
                    flag_t = true;
                }

                if (CheckColunmIsExist(ccnfac, tableName_tp, "SPRODUCTCODE") && isEncypt)
                {
                    flag_tp = true;
                }
                #endregion

                #region 组织更新T厂家表参数
                if (flag_t)
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, newcode, channel);
                }
                else
                {
                    param2 = GetInsertCodeQueryLogParam(tracetype, code, ip, channel);
                }
                #endregion

                #region 组织更新T厂家产品表参数
                if (flag_tp)
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, newcode, channel, codeCreateDate);
                }
                else
                {
                    param3 = GetInsertCodeQueryLogParam(tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, provice, city, called, channel, codeCreateDate);
                }
                #endregion

                if (param2 != null && param3 != null)
                {
                    DataBase dbc = new DataBase();
                    dbc.BeginTrans();
                    try
                    {
                        int nRet1 = 0;
                        int nRet2 = 0;
                        int nRet3 = 0;

                        #region 更新P表
                        //回传激活设计UA条件下不更新P表 begin 20130314 jinzhixin
                        if (tracetype != "UA")
                        {
                            if (CheckColunmIsExist(ccnfac, tableName_p, "NUM"))
                            {
                                string strSQL = string.Format(SQL_Update_CodeVisit1, tableName_p, newcode);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            }
                            else
                            {
                                string strSQL = string.Format(SQL_Update_CodeVisit2, tableName_p, newcode);
                                nRet1 = dbc.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            }
                        }
                        else
                        {
                            nRet1 = 1;
                        }
                        //回传激活设计UA条件下不更新P表 end 20130314 jinzhixin
                        #endregion

                        #region 更新T厂家表
                        if (flag_t)
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_2, tableName_t), param2);
                        }
                        else
                        {
                            nRet2 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T_1, tableName_t), param2);
                        }
                        #endregion

                        #region 更新T厂家产品表
                        if (flag_tp)
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T2, tableName_tp), param3);
                        }
                        else
                        {
                            nRet3 = dbc.ExecuteNonQuery(CommandType.Text, string.Format(SQL_ADD_InfoCode_T1, tableName_tp), param3);
                        }
                        #endregion

                        #region 如果P表、T厂家表、T厂家产品表数据都更新成功回滚或者提交事务
                        if ((nRet1 + nRet2 + nRet3) == 3)
                        {
                            dbc.CommitTrans();
                            bRet = true;
                        }
                        else
                        {
                            dbc.RollBackTrans();
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        KMSLotterySystemFront.Logger.AppLog.Write("14)记录T厂家产品表日志----" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                        dbc.RollBackTrans();
                        throw ex;
                    }
                }


                return bRet;
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("14)记录T厂家产品表日志----" + ex.Message + "----" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                return false;
            }
        }
        #endregion

        #region 15) 查询数码的首次查询时间/查询方式/查询地址

        /// <summary>
        /// 根据数码查询数码查询状态
        /// </summary>
        /// <param name="tablename">表名</param>
        /// <param name="code">数码</param>
        /// <returns></returns>
        public DataSet GetCodeMessageByCode(string tablename, string code)
        {
            try
            {

                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("GetCodeMessageByCodeParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[1];
                    param[0] = new OracleParameter(":LPRODUCTCODE", OracleType.VarChar, 16);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetCodeMessageByCodeParam", param);
                }
                param[0].Value = code;

                string sql = string.Format(SQL_GETCODEMEG_SQL, tablename);


                DataBase dataBase = new DataBase();
                DataSet dsRet = dataBase.GetDataSet(CommandType.Text, sql, param);
                return dsRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        #endregion

        #region 16)检测产品是否属于回传激活的产品
        /// <summary>
        /// 检测产品是否属于回传激活的产品,属于回传激活则返回True,不属于回传激活则返回False
        /// </summary>
        /// <param name="directoryName">厂家编号</param>
        /// <param name="proid">产品编号</param>
        /// <returns>属于回传激活则返回True,不属于回传激活则返回False</returns>
        public Boolean CheckProductIsAct(string directoryName, string proid)
        {
            Boolean bRet = false;
            DataBase db = new DataBase();
            try
            {
                OracleParameter[] param = (OracleParameter[])ParameterCache.GetParams("CheckProductIsActParam");
                //构造参数
                if (param == null)
                {
                    param = new OracleParameter[2];
                    param[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 6);
                    param[1] = new OracleParameter(":PRODUCTID", OracleType.VarChar, 10);
                    //将参数加入缓存
                    ParameterCache.PushCache("CheckProductIsActParam", param);
                }
                param[0].Value = directoryName;
                param[1].Value = proid;

                object oRet = db.ExecuteScalar(CommandType.Text, CHECK_FACPRO_ACT_SQL, param);
                if (oRet != null)
                {
                    if (oRet.ToString() == "1")
                        bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("判断是否需要回传激活--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 17)获取产品表信息

        /// <summary>
        /// 获取数码附属信息参数
        /// </summary>
        /// <param name="directoryName">当前服务厂家</param>
        /// <param name="facID">数码解析的厂家</param>
        /// <param name="proID">数码解析的产品</param>
        /// <returns></returns>
        private OracleParameter[] GetCodeAttachedInfoParam1(string directoryName, string facID, string proID)
        {
            OracleParameter[] parms = null;
            try
            {
                parms = (OracleParameter[])ParameterCache.GetParams("GetCodeAttachedInfoParam1");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[2];
                    parms[0] = new OracleParameter(PARAM_FACTORYID, OracleType.Char, 5);
                    parms[1] = new OracleParameter(":PRODUCTID", OracleType.Char, 3);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetCodeAttachedInfoParam1", parms);
                }
                parms[0].Value = facID;
                parms[1].Value = proID;

            }
            catch (Exception ex)
            {
                parms = null;
                Logger.AppLog.Write(directoryName + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
            }
            return parms;
        }

        /// <summary>
        /// 获取产品表信息
        /// </summary>
        /// <param name="factoryid">调用厂家编号</param>
        /// <param name="facid">虚拟厂家编号</param>
        /// <param name="proid">虚拟产品编号</param>
        /// <returns>返回厂家产品配置信息</returns>
        public DataTable GetCodeFactoryProduct(string directoryName, string facid, string proid, string codeCreateDate)
        {
            DataTable bRet = null;
            try
            {
                OracleParameter[] parms = GetCodeAttachedInfoParam1(directoryName, facid, proid);
                if (parms != null)
                {
                    DataBase db = new DataBase();
                    bRet = db.GetDataSet(CommandType.Text, string.Format(QUERY_PRODUCTCODEINFO_BYCODE_SQL, codeCreateDate), parms).Tables[0];
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("获取产品表信息,directoryName：" + directoryName + "*facid:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                throw ex;
            }
            return bRet;
        }
        #endregion
    }
}
