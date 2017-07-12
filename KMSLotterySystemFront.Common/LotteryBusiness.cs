using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CCN.Code2D;

using KMSLotterySystemFront;
using System.Data;
using System.Xml;
using KMSLotterySystemFront.Model;
using System.Data.OracleClient;
using KMSLotterySystemFront.DBUtility;

namespace KMSLotterySystemFront.Common
{
    public class LotteryBusiness
    {


        #region 1) 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// <summary>
        /// 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// </summary>
        /// <param name="userId">授权帐号</param>
        /// <param name="userPwd">授权密码</param>
        /// <param name="queryIp">请求服务器IP地址</param>
        /// <param name="factoryID">输出:厂家编号</param>
        /// <returns></returns>
        public bool ChekcNewPFUser(string userId, string userPwd, string queryIp, out AcUserEntity AcUserInfo, out string systemState)
        {
            bool bRet = false;
            systemState = "000";
            AcUserInfo = null;


            try
            {
                string id = string.Empty;
                DataTable userTable = null;

                if (DataCache.GetCache(userId + "AcUserQuery") != null)
                {
                    //userTable = DataCache.GetCache(userId + "AcUser") as DataTable;
                    AcUserInfo = DataCache.GetCache(userId + "AcUserQuery") as AcUserEntity;
                }
                else
                {
                    userTable = GetPfUserListByUserID(userId, userPwd);

                    if (userTable != null && userTable.Rows.Count > 0)
                    {
                        AcUserInfo = new AcUserEntity();

                        AcUserInfo.factoryid = userTable.Rows[0]["FACTORYID"].ToString();
                        AcUserInfo.infactoryid = userTable.Rows[0]["INFACTORYID"].ToString();
                        AcUserInfo.id = userTable.Rows[0]["ID"].ToString();
                        AcUserInfo.oid = userTable.Rows[0]["OID"].ToString();
                        AcUserInfo.reply = userTable.Rows[0]["REPLY"].ToString();
                        AcUserInfo.iscustomer = userTable.Rows[0]["ISCUSTOMER"].ToString();
                        AcUserInfo.projectid = userTable.Rows[0]["PROJECTID"].ToString();
                        AcUserInfo.activename = userTable.Rows[0]["ACTIVENAME"].ToString();
                        AcUserInfo.begintime = userTable.Rows[0]["BEGINTIME"].ToString();
                        AcUserInfo.endtime = userTable.Rows[0]["ENDTIME"].ToString();
                        AcUserInfo.facproductid = userTable.Rows[0]["FACPRODUCTID"].ToString();
                        AcUserInfo.sendmessage = userTable.Rows[0]["SENDMESSAGE"].ToString();
                        AcUserInfo.sendmessageuserid = userTable.Rows[0]["SENDMESSAGEUSERID"].ToString();
                        AcUserInfo.sendmessageuserpwd = userTable.Rows[0]["SENDMESSAGEUSERPWD"].ToString();
                        AcUserInfo.tokenkey = userTable.Rows[0]["TOKENKEY"].ToString();
                        AcUserInfo.f1 = userTable.Rows[0]["F1"].ToString();
                        AcUserInfo.f2 = userTable.Rows[0]["F2"].ToString();
                        AcUserInfo.f3 = userTable.Rows[0]["F3"].ToString();
                        AcUserInfo.f4 = userTable.Rows[0]["F4"].ToString();
                        AcUserInfo.f5 = userTable.Rows[0]["F5"].ToString();
                    }

                    //if (DataCache.GetCache(userId + "AcUserQuery") == null)
                    //{
                    //    DataCache.SetCache(userId + "AcUserQuery", AcUserInfo);
                    //}
                }


                if (AcUserInfo != null)
                {
                    if (!string.IsNullOrEmpty(AcUserInfo.begintime) && !string.IsNullOrEmpty(AcUserInfo.endtime))
                    {
                        if (!(DateTime.Now >= Convert.ToDateTime(AcUserInfo.begintime) && DateTime.Now <= Convert.ToDateTime(AcUserInfo.endtime)))
                        {
                            //接口授权时间不在许可范围之内
                            systemState = "009";
                            return false;
                        }
                    }

                    if (!string.IsNullOrEmpty(AcUserInfo.id))
                    {
                        DataTable IPTable = new DataTable();

                        if (DataCache.GetCache(userId + "AcUserQueryIP") != null)
                        {
                            IPTable = DataCache.GetCache(userId + "AcUserQueryIP") as DataTable;
                        }
                        else
                        {
                            IPTable = GetPfQueryIPList(AcUserInfo.id);
                        }

                        if (IPTable != null && IPTable.Rows.Count > 0)
                        {
                            //IP地址未授权
                            if (IPTable.Select("VIP='" + queryIp.Trim() + "' AND TYPE='1'").Count() <= 0)
                            {
                                bool bRet2 = false;
                                //查询段匹配集合
                                DataRow[] rowConfig = IPTable.Select("TYPE='0'");

                                //循环段匹配集合数据,查找是否存在满足段匹配的数据
                                for (int i = 0; i < rowConfig.Length; i++)
                                {
                                    string dbip = rowConfig[i]["VIP"].ToString();

                                    if (queryIp.Trim().StartsWith(dbip))
                                    {
                                        bRet2 = true;
                                        break;
                                    }
                                }

                                if (!bRet2)
                                {
                                    systemState = "101";
                                    return false;
                                }
                                else
                                {
                                    systemState = "001";
                                    return true;
                                }
                            }
                            else
                            {
                                systemState = "001";
                                return true;
                            }
                        }
                        else//未找到此用户对应授权的IP地址
                        {
                            systemState = "101";
                            return false;
                        }
                    }
                    else
                    {
                        //授权账号密码错误或不存在。
                        systemState = "006";
                        return false;
                    }

                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitCodeBusiness.cs--ChekcNewPFUser--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion


        #region 12) 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        public bool ChekcNewPFUser(string userId, string userPwd, string queryIp, out string factoryID, out string systemState)
        {
            bool bRet = false;
            systemState = "000";
            factoryID = string.Empty;
            try
            {
                DBUtility.DataBase dataBase = new DBUtility.DataBase();

                string id = string.Empty;
                string oid = string.Empty;

                DataTable userTable = GetPfUserListByUserID(userId, userPwd);

                if (userTable != null && userTable.Rows.Count > 0)
                {
                    factoryID = userTable.Rows[0]["FACTORYID"].ToString();
                    id = userTable.Rows[0]["ID"].ToString();
                    oid = userTable.Rows[0]["OID"].ToString();
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }


                if (!string.IsNullOrEmpty(id))
                {
                    DataTable IPTable = GetPfQueryIPList(id);

                    if (IPTable != null && IPTable.Rows.Count > 0)
                    {
                        //IP地址未授权
                        if (IPTable.Select("VIP='" + queryIp.Trim() + "' AND TYPE='1'").Count() <= 0)
                        {
                            bool bRet2 = false;
                            //查询段匹配集合
                            DataRow[] rowConfig = IPTable.Select("TYPE='0'");

                            //循环段匹配集合数据,查找是否存在满足段匹配的数据
                            for (int i = 0; i < rowConfig.Length; i++)
                            {
                                string dbip = rowConfig[i]["VIP"].ToString();

                                if (queryIp.Trim().StartsWith(dbip))
                                {
                                    bRet2 = true;
                                    break;
                                }
                            }

                            if (!bRet2)
                            {
                                systemState = "101";
                                return false;
                            }
                            else
                            {
                                systemState = "001";
                                return true;
                            }
                        }
                        else
                        {
                            systemState = "001";
                            return true;
                        }
                    }
                    else//未找到此用户对应授权的IP地址
                    {
                        systemState = "101";
                        return false;
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitCodeBusiness.cs--ChekcNewPFUser--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion


        #region 13) 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// <summary>
        /// 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// </summary>
        /// <param name="userId">授权帐号</param>
        /// <param name="userPwd">授权密码</param>
        /// <param name="queryIp">请求服务器IP地址</param>
        /// <param name="factoryID">输出:厂家编号</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="dbReply">输出:返回信息</param>
        /// <param name="isCustomer">输出:是否记录防伪查询日志</param>
        /// <param name="oId">输出:类别ID</param>
        /// <param name="sendMessageTemplate">输出:短信发送模板</param>
        /// <returns></returns>
        public bool ChekcNewPFUser(string userId, string userPwd, string queryIp, out string factoryID, out string systemState, out string dbReply, out string isCustomer, out string oId)
        {
            bool bRet = false;
            systemState = "000";
            factoryID = string.Empty;
            oId = string.Empty;
            dbReply = string.Empty;
            isCustomer = string.Empty;

            try
            {
                string id = string.Empty;
                DataTable userTable = GetPfUserListByUserID(userId, userPwd);

                if (userTable != null && userTable.Rows.Count > 0)
                {
                    factoryID = userTable.Rows[0]["FACTORYID"].ToString();
                    id = userTable.Rows[0]["ID"].ToString();
                    oId = userTable.Rows[0]["OID"].ToString();
                    dbReply = userTable.Rows[0]["REPLY"].ToString();
                    isCustomer = userTable.Rows[0]["ISCUSTOMER"].ToString();

                    if (!string.IsNullOrEmpty(userTable.Rows[0]["begintime"].ToString()) && !string.IsNullOrEmpty(userTable.Rows[0]["endtime"].ToString()))
                    {
                        if (!(DateTime.Now >= Convert.ToDateTime(userTable.Rows[0]["begintime"].ToString()) && DateTime.Now <= Convert.ToDateTime(userTable.Rows[0]["endtime"].ToString())))
                        {
                            //接口授权时间不在许可范围之内
                            systemState = "009";
                            return false;
                        }
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }

                if (!string.IsNullOrEmpty(id))
                {
                    DataTable IPTable = GetPfQueryIPList(id);

                    if (IPTable != null && IPTable.Rows.Count > 0)
                    {
                        //IP地址未授权
                        if (IPTable.Select("VIP='" + queryIp.Trim() + "' AND TYPE='1'").Count() <= 0)
                        {
                            bool bRet2 = false;
                            //查询段匹配集合
                            DataRow[] rowConfig = IPTable.Select("TYPE='0'");

                            //循环段匹配集合数据,查找是否存在满足段匹配的数据
                            for (int i = 0; i < rowConfig.Length; i++)
                            {
                                string dbip = rowConfig[i]["VIP"].ToString();

                                if (queryIp.Trim().StartsWith(dbip))
                                {
                                    bRet2 = true;
                                    break;
                                }
                            }

                            if (!bRet2)
                            {
                                systemState = "101";
                                return false;
                            }
                            else
                            {
                                systemState = "001";
                                return true;
                            }
                        }
                        else
                        {
                            systemState = "001";
                            return true;
                        }
                    }
                    else//未找到此用户对应授权的IP地址
                    {
                        systemState = "101";
                        return false;
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitCodeBusiness.cs--ChekcNewPFUser--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion

        #region 13) 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// <summary>
        /// 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// </summary>
        /// <param name="userId">授权帐号</param>
        /// <param name="userPwd">授权密码</param>
        /// <param name="queryIp">请求服务器IP地址</param>
        /// <param name="factoryID">输出:厂家编号</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="dbReply">输出:返回信息</param>
        /// <param name="isCustomer">输出:是否记录防伪查询日志</param>
        /// <param name="oId">输出:类别ID</param>
        /// <param name="sendMessageTemplate">输出:短信发送模板</param>
        /// <returns></returns>
        public bool ChekcNewPFUser(string userId, string userPwd, string queryIp, out string factoryID, out string systemState, out string dbReply, out string isCustomer, out string oId, out string projectID, out string activeName, out string sendMessageTemplate, out string sendUserName, out string sendUserPwd)
        {
            bool bRet = false;
            systemState = "000";
            factoryID = string.Empty;
            oId = string.Empty;
            dbReply = string.Empty;
            isCustomer = string.Empty;
            sendMessageTemplate = string.Empty;
            projectID = string.Empty;
            activeName = string.Empty;
            sendUserName = string.Empty;
            sendUserPwd = string.Empty;

            try
            {
                string id = string.Empty;
                DataTable userTable = GetPfUserListByUserID(userId, userPwd);

                if (userTable != null && userTable.Rows.Count > 0)
                {
                    factoryID = userTable.Rows[0]["FACTORYID"].ToString();
                    id = userTable.Rows[0]["ID"].ToString();
                    oId = userTable.Rows[0]["OID"].ToString();
                    dbReply = userTable.Rows[0]["REPLY"].ToString();
                    isCustomer = userTable.Rows[0]["ISCUSTOMER"].ToString();
                    projectID = userTable.Rows[0]["PROJECTID"].ToString();
                    activeName = userTable.Rows[0]["ACTIVENAME"].ToString();
                    sendMessageTemplate = userTable.Rows[0]["SENDMESSAGE"].ToString();
                    sendUserName = userTable.Rows[0]["SENDMESSAGEUSERID"].ToString();
                    sendUserPwd = userTable.Rows[0]["SENDMESSAGEUSERPWD"].ToString();

                    if (!string.IsNullOrEmpty(userTable.Rows[0]["begintime"].ToString()) && !string.IsNullOrEmpty(userTable.Rows[0]["endtime"].ToString()))
                    {
                        if (!(DateTime.Now >= Convert.ToDateTime(userTable.Rows[0]["begintime"].ToString()) && DateTime.Now <= Convert.ToDateTime(userTable.Rows[0]["endtime"].ToString())))
                        {
                            //接口授权时间不在许可范围之内
                            systemState = "009";
                            return false;
                        }
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }

                if (!string.IsNullOrEmpty(id))
                {
                    DataTable IPTable = GetPfQueryIPList(id);

                    if (IPTable != null && IPTable.Rows.Count > 0)
                    {
                        //IP地址未授权
                        if (IPTable.Select("VIP='" + queryIp.Trim() + "' AND TYPE='1'").Count() <= 0)
                        {
                            bool bRet2 = false;
                            //查询段匹配集合
                            DataRow[] rowConfig = IPTable.Select("TYPE='0'");

                            //循环段匹配集合数据,查找是否存在满足段匹配的数据
                            for (int i = 0; i < rowConfig.Length; i++)
                            {
                                string dbip = rowConfig[i]["VIP"].ToString();

                                if (queryIp.Trim().StartsWith(dbip))
                                {
                                    bRet2 = true;
                                    break;
                                }
                            }

                            if (!bRet2)
                            {
                                systemState = "101";
                                return false;
                            }
                            else
                            {
                                systemState = "001";
                                return true;
                            }
                        }
                        else
                        {
                            systemState = "001";
                            return true;
                        }
                    }
                    else//未找到此用户对应授权的IP地址
                    {
                        systemState = "101";
                        return false;
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitCodeBusiness.cs--ChekcNewPFUser--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion

        #region 13) 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// <summary>
        /// 查询新平台授权帐号和密码以及IP地址是否授权是否正确
        /// </summary>
        /// <param name="userId">授权帐号</param>
        /// <param name="userPwd">授权密码</param>
        /// <param name="queryIp">请求服务器IP地址</param>
        /// <param name="factoryID">输出:厂家编号</param>
        /// <param name="systemState">输出:系统执行状态</param>
        /// <param name="dbReply">输出:返回信息</param>
        /// <param name="isCustomer">输出:是否记录防伪查询日志</param>
        /// <param name="oId">输出:类别ID</param>
        /// <returns></returns>
        public bool ChekcNewPFUser(string userId, string userPwd, string queryIp, out string factoryID, out string systemState, out string dbReply, out string isCustomer, out string oId, out string projectID)
        {
            bool bRet = false;
            systemState = "000";
            factoryID = string.Empty;
            oId = string.Empty;
            dbReply = string.Empty;
            isCustomer = string.Empty;
            projectID = string.Empty;

            try
            {
                string id = string.Empty;
                DataTable userTable = GetPfUserListByUserID(userId, userPwd);

                if (userTable != null && userTable.Rows.Count > 0)
                {
                    factoryID = userTable.Rows[0]["FACTORYID"].ToString();
                    id = userTable.Rows[0]["ID"].ToString();
                    oId = userTable.Rows[0]["OID"].ToString();
                    dbReply = userTable.Rows[0]["REPLY"].ToString();
                    isCustomer = userTable.Rows[0]["ISCUSTOMER"].ToString();
                    projectID = userTable.Rows[0]["PROJECTID"].ToString();

                    if (!string.IsNullOrEmpty(userTable.Rows[0]["begintime"].ToString()) && !string.IsNullOrEmpty(userTable.Rows[0]["endtime"].ToString()))
                    {
                        if (!(DateTime.Now >= Convert.ToDateTime(userTable.Rows[0]["begintime"].ToString()) && DateTime.Now <= Convert.ToDateTime(userTable.Rows[0]["endtime"].ToString())))
                        {
                            //接口授权时间不在许可范围之内
                            systemState = "009";
                            return false;
                        }
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }

                if (!string.IsNullOrEmpty(id))
                {
                    DataTable IPTable = GetPfQueryIPList(id);

                    if (IPTable != null && IPTable.Rows.Count > 0)
                    {
                        //IP地址未授权
                        if (IPTable.Select("VIP='" + queryIp.Trim() + "' AND TYPE='1'").Count() <= 0)
                        {
                            bool bRet2 = false;
                            //查询段匹配集合
                            DataRow[] rowConfig = IPTable.Select("TYPE='0'");

                            //循环段匹配集合数据,查找是否存在满足段匹配的数据
                            for (int i = 0; i < rowConfig.Length; i++)
                            {
                                string dbip = rowConfig[i]["VIP"].ToString();

                                if (queryIp.Trim().StartsWith(dbip))
                                {
                                    bRet2 = true;
                                    break;
                                }
                            }

                            if (!bRet2)
                            {
                                systemState = "101";
                                return false;
                            }
                            else
                            {
                                systemState = "001";
                                return true;
                            }
                        }
                        else
                        {
                            systemState = "001";
                            return true;
                        }
                    }
                    else//未找到此用户对应授权的IP地址
                    {
                        systemState = "101";
                        return false;
                    }
                }
                else
                {
                    //授权账号密码错误或不存在。
                    systemState = "006";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitCodeBusiness.cs--ChekcNewPFUser--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        #endregion


        #region 61) 依据用户账号获取对应账号信息
        public DataTable GetPfUserListByUserID(string userId, string userPwd)
        {
            DataTable dsRet = null;
            try
            {
                OracleParameter[] parms = (OracleParameter[])ParameterCache.GetParams("GetPfUserListByUserIDParam");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[2];
                    parms[0] = new OracleParameter(":USERID", OracleType.VarChar, 32);
                    parms[1] = new OracleParameter(":USERPWD", OracleType.VarChar, 32);
                    //将参数加入缓存
                    ParameterCache.PushCache("GetPfUserListByUserIDParam", parms);
                }
                parms[0].Value = userId.Trim();
                parms[1].Value = userPwd.Trim();

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();
                    string sql = "SELECT U.* FROM  T_CCN_AC_USER U WHERE U.USERID=:USERID AND U.USERPWD=:USERPWD AND U.FLAG='1'";
                    dsRet = dataBase.GetDataSet(CommandType.Text, sql, parms).Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(userId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return dsRet;
        }
        #endregion

        #region 61.1) 依据用户账号获取对应账号信息

        /// <summary>
        /// 用户授权ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetPfUserListByUserID(string userId)
        {
            DataTable dsRet = null;
            try
            {
                OracleParameter[] parms = new OracleParameter[1];
                parms[0] = new OracleParameter(":USERID", OracleType.VarChar, 32);
                parms[0].Value = userId.Trim();

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();
                    string sql = "SELECT U.* FROM  T_CCN_AC_USER U WHERE U.USERID=:USERID AND U.FLAG='1'";
                    dsRet = dataBase.GetDataSet(CommandType.Text, sql, parms).Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(userId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return dsRet;
        }
        #endregion

        #region 61) 依据用户账号获取对应账号信息
        public DataTable GetPfQueryIPList(string aId)
        {
            DataTable dsRet = null;
            try
            {
                OracleParameter[] parms = (OracleParameter[])ParameterCache.GetParams("GetPfQueryIPListParam");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[1];
                    parms[0] = new OracleParameter(":AID", OracleType.Number, 10);

                    //将参数加入缓存
                    ParameterCache.PushCache("GetPfQueryIPListParam", parms);
                }
                parms[0].Value = aId.Trim();

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();
                    string sql = "SELECT VIP,TYPE FROM T_CCN_AC_QUERYIP I WHERE I.AID=:AID AND FLAG='1'";
                    dsRet = dataBase.GetDataSet(CommandType.Text, sql, parms).Tables[0];

                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(aId + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return dsRet;
        }
        #endregion

        #region 查询门店所属城市编号
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="storeID"></param>
        /// <returns></returns>
        private string GetCityCodeInfoByStoreID(string factoryID, string storeID)
        {
            object cityCode = null;
            try
            {

                OracleParameter[] parms = new OracleParameter[2];
                parms[0] = new OracleParameter(":STOREID", OracleType.VarChar, 10);
                parms[1] = new OracleParameter(":FACID", OracleType.VarChar, 6);

                parms[0].Value = storeID.Trim();
                parms[1].Value = factoryID.Trim();

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();
                    string sql = "SELECT S.STORENAMEED FROM T_SGM_SHAKE_STORE S WHERE S.STOREID=:STOREID AND S.FACID=:FACID";
                    cityCode = dataBase.ExecuteScalar(CommandType.Text, sql, parms);
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(factoryID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return cityCode.ToString();
        }

        /// <summary>
        /// 查询门店所属城市编号
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="storeID"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public bool GetCityCodeByStoreID(string factoryID, string storeID, out string cityCode)
        {
            bool bRet = false;

            cityCode = string.Empty;

            try
            {

                if (!string.IsNullOrEmpty(storeID))
                {
                    cityCode = GetCityCodeInfoByStoreID(factoryID, storeID);
                }

                if (!string.IsNullOrEmpty(cityCode))
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitCodeBusiness.cs--ChekcNewPFUser--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 根据被邀人获取邀请人信息
        /// <summary>
        /// 根据被邀人获取邀请人信息
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="Invitedmobile">被邀人手机号码</param>
        /// <returns></returns>
        public DataTable GetInviteInfoByInvited(string facid, string Invitedmobile)
        {

            DataTable dt = null;

            try
            {
                OracleParameter[] parms = (OracleParameter[])ParameterCache.GetParams("GetInviteInfoByInvited");
                //构造参数
                if (parms == null)
                {
                    parms = new OracleParameter[2];
                    parms[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    parms[1] = new OracleParameter(":INVITETO", OracleType.Number, 11);


                    //将参数加入缓存
                    ParameterCache.PushCache("GetInviteInfoByInvited", parms);
                }
                parms[0].Value = facid;
                parms[1].Value = Invitedmobile;

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();
                    string sql = "select m.invitefrom,n.openid,n.user_name from (SELECT  * FROM T_SGM_SHAKE_Invite  L  WHERE  L.FACID=:FACID  and  l.inviteto=:INVITETO order by  l.createdate asc ) m left  join   T_SGM_SHAKE_REGISTERUSER_9999 n on m.facid=n.facid and  m.invitefrom=n.ip where rownum=1 ";
                    dt = dataBase.GetDataSet(CommandType.Text, sql, parms).Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.Common.LotteryBusiness.cs--GetInviteInfoByInvited()--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return dt;
        }
        #endregion

        #region 检测分享红包奖励限制
        /// <summary>
        /// 检测分享红包奖励限制
        /// </summary>
        /// <param name="facid">抽奖编号</param>
        /// <param name="ccnactivityid">活动内部ID</param>
        /// <param name="openid">接收红包的openid</param>
        /// <param name="sendtype">（1代表扫码奖励，2代表邀请好友获奖励，3 分享奖励）</param>
        /// <param name="datetype">时间限制</param>
        /// <param name="limitnum">次数限制</param>
        /// <returns></returns>
        public bool CheckShareRedPackDateLimit(string facid, string ccnactivityid, string openid, string sendtype, string datetype, int limitnum)
        {
            bool bRet = false;

            try
            {
                string sql = "";
                if (datetype == "D")//当天
                {
                    sql = "SELECT B.*,B.ROWID FROM t_smg_wxpay_hbdetail  B WHERE  B.FACID=:FACID  and b.ccnactivityid=:CCNACTIVITYID and  b.useropenid=:OPENID and  b.hbsendtype=:HBSENDTYPE and TO_CHAR(B.createtime,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')   and  b.state='SUCCESS'  and  b.deleteflag='1' order by b.createtime desc";
                }
                else if (datetype == "M")//当月
                {
                    sql = "SELECT B.*,B.ROWID FROM t_smg_wxpay_hbdetail  B WHERE  B.FACID=:FACID  and b.ccnactivityid=:CCNACTIVITYID and  b.useropenid=:OPENID and  b.hbsendtype=:HBSENDTYPE and TO_CHAR(B.createtime,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')   and  b.state='SUCCESS'   and  b.deleteflag='1' order by b.createtime desc";


                }
                else if (datetype == "Y")//当年
                {

                    sql = "SELECT B.*,B.ROWID FROM t_smg_wxpay_hbdetail  B WHERE  B.FACID=:FACID  and b.ccnactivityid=:CCNACTIVITYID and  b.useropenid=:OPENID  and  b.hbsendtype=:HBSENDTYPE and TO_CHAR(B.createtime,'YYYY')=TO_CHAR(SYSDATE,'YYYY')   and  b.state='SUCCESS'   and  b.deleteflag='1' order by b.createtime desc";

                }
                else if (datetype == "N")
                {
                    sql = "SELECT B.*,B.ROWID FROM t_smg_wxpay_hbdetail  B WHERE  B.FACID=:FACID  and b.ccnactivityid=:CCNACTIVITYID and  b.useropenid=:OPENID and  b.hbsendtype=:HBSENDTYPE   and  b.state='SUCCESS'   and  b.deleteflag='1' order by b.createtime desc";

                }
                else//本次活动只允许奖励一次
                {
                    sql = "SELECT B.*,B.ROWID FROM t_smg_wxpay_hbdetail  B WHERE  B.FACID=:FACID  and b.ccnactivityid=:CCNACTIVITYID and  b.useropenid=:OPENID  and  b.hbsendtype=:HBSENDTYPE   and  b.state='SUCCESS'   and  b.deleteflag='1' order by b.createtime desc";
                }

                OracleParameter[] param = null;
                param = new OracleParameter[4];
                param[0] = new OracleParameter(":FACID", OracleType.VarChar, 20);
                param[1] = new OracleParameter(":CCNACTIVITYID", OracleType.VarChar, 32);
                param[2] = new OracleParameter(":OPENID", OracleType.VarChar, 32);
                param[3] = new OracleParameter(":HBSENDTYPE", OracleType.Char, 1);

                param[0].Value = facid;
                param[1].Value = ccnactivityid;
                param[2].Value = openid;
                param[3].Value = sendtype;

                //  DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, param);
                DataBase dataBase = new DataBase();
                DataTable dt = dataBase.GetDataSet(CommandType.Text, sql, param).Tables[0];

                if (datetype == "N")//不限制
                {
                    bRet = true;
                }
                else
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
                Logger.AppLog.Write("KMSLotterySystemFront.Common.LotteryBusiness.cs--CheckShareRedPackDateLimit()--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion

        #region 向红包明细表中插入数据
        /// <summary>
        /// 向红包明细表中插入数据
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ccnactivityid">内部活动ID</param>
        /// <param name="ccnactivityname">内部活动名称</param>
        /// <param name="hbsendtype">红包发放类型（1代表扫码奖励，2代表邀请好友获奖励，3 分享奖励）</param>
        /// <param name="code">中奖数码</param>
        /// <param name="money">中奖金额：单位：分</param>
        /// <param name="state">红包发放状态</param>
        /// <param name="statedetail">红包发放状态描述</param>
        /// <param name="mch_billno">商户订单号</param>
        /// <param name="send_listid">微信单号</param>
        /// <param name="xml">其他参数（包含中奖数码，中奖人姓名等信息）</param>
        /// <returns></returns>
        public bool InsertIntoHbDetail(string facid, string ccnactivityid, string ccnactivityname, string hbsendtype, string code, string money, string state, string statedetail, string mch_billno, string send_listid, string xml)
        {
            bool bRet = false;
            try
            {
                //sb.Append("<I_OPENID>" + i_useropenid + "</I_OPENID>");//邀请人openid
                //sb.Append("<I_USERNAME>" + i_username + "</I_USERNAME>");//邀请人姓名
                //sb.Append("<I_USERMOBILE>" + i_usermobile + "</I_USERMOBILE>");//邀请人手机号
                //sb.Append("<I_CODE></I_CODE>");//邀请人中奖数码
                //sb.Append("<I_LOTTERYGUID>" + lid + "</I_ED_LOTTERYGUID>");//邀请人中奖guid
                //sb.Append("<I_ED_OPENID>" + user_openid + "</I_ED_OPENID>");//被邀请人openid
                //sb.Append("<I_ED_USERNAME>" + user_name + "</I_ED_USERNAME>");//被邀请人姓名
                //sb.Append("<I_ED_USERMOBILE>" + ip + "</I_ED_USERMOBILE>");//被邀请人手机号
                //sb.Append("<I_ED_CODE>" + ip + "</I_ED_CODE>");//被邀请人中奖号码
                //sb.Append("<I_ED_LOTTERYGUID>" + lid + "</I_ED_LOTTERYGUID>");//被邀请人中奖guid

                string I_OPENID = "";//邀请人openid
                string I_USERNAME = "";//邀请人姓名
                string I_USERMOBILE = "";//邀请人手机号
                string I_CODE = "";//邀请人中奖数码
                string I_LOTTERYGUID = "";//邀请人中奖guid
                string I_ED_OPENID = "";//被邀请人openid
                string I_ED_USERNAME = "";//被邀请人姓名
                string I_ED_USERMOBILE = "";//被邀请人手机号
                string I_ED_CODE = "";//被邀请人中奖号码
                string I_ED_LOTTERYGUID = "";//被邀请人中奖guid
                //解析xml
                #region 解析xml
                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Replace("&", "");
                    if (xml.Contains("Info"))
                    {
                        XmlNodeList nodeList = XmlHelperNew.GetXmlNodeList(xml, "//Info");

                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                if (node != null)
                                {
                                    foreach (XmlNode childNode in node.ChildNodes)
                                    {
                                        if (childNode.Name.ToString().Trim().ToUpper().Equals("I_OPENID"))
                                        {
                                            I_OPENID = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_USERNAME"))
                                        {
                                            I_USERNAME = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_USERMOBILE"))
                                        {
                                            I_USERMOBILE = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_CODE"))
                                        {
                                            I_CODE = childNode.InnerText.ToString();
                                        }

                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_ED_OPENID"))
                                        {
                                            I_ED_OPENID = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_ED_USERNAME"))
                                        {
                                            I_ED_USERNAME = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_ED_USERMOBILE"))
                                        {
                                            I_ED_USERMOBILE = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_ED_CODE"))
                                        {
                                            I_ED_CODE = childNode.InnerText.ToString();
                                        }
                                        else if (childNode.Name.ToString().Trim().ToUpper().Equals("I_ED_LOTTERYGUID"))
                                        {
                                            I_ED_LOTTERYGUID = childNode.InnerText.ToString();
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                if (RegexExpress.IsQRCode(I_CODE))
                {
                    I_CODE = GetCode(I_CODE);
                }

                if (RegexExpress.IsQRCode(I_ED_CODE))
                {
                    I_ED_CODE = GetCode(I_ED_CODE);
                }

                string guid = Guid.NewGuid().ToString().Replace("-", "");
                string sql = "";
                DataBase dataBase = new DataBase();

                if (hbsendtype == "1")//扫码奖励
                {
                    sql = " insert into t_smg_wxpay_hbdetail  (GUID,Ccnactivityid,Ccnactivityname,Facid,username,Usermobile,Useropenid,Usercode,Hbsendtype,Total_Amount,State,Statedetail,Mch_Billno,send_listid) values('" + guid + "','" + ccnactivityid + "','" + ccnactivityname + "','" + facid + "','" + I_USERNAME + "','" + I_USERMOBILE + "','" + I_OPENID + "','" + code + "','" + hbsendtype + "','" + money + "','" + state + "','" + statedetail + "','" + mch_billno + "','" + send_listid + "') ";


                }
                else if (hbsendtype == "2")//邀请好友奖励
                {
                    sql = " insert into t_smg_wxpay_hbdetail  (GUID,Ccnactivityid,Ccnactivityname,Facid,username,Usermobile,Useropenid,iusername,iusermobile,iuseropenid,iusercode,Hbsendtype,Total_Amount,State,Statedetail,Mch_Billno,send_listid) values('" + guid + "','" + ccnactivityid + "','" + ccnactivityname + "','" + facid + "','" + I_USERNAME + "','" + I_USERMOBILE + "','" + I_OPENID + "','" + I_ED_USERNAME + "','" + I_ED_USERMOBILE + "','" + I_ED_OPENID + "','" + I_ED_CODE + "','" + hbsendtype + "','" + money + "','" + state + "','" + statedetail + "','" + mch_billno + "','" + send_listid + "') ";
                }
                else//分享朋友圈奖励
                {
                    sql = " insert into t_smg_wxpay_hbdetail  (GUID,Ccnactivityid,Ccnactivityname,Facid,username,Usermobile,Useropenid,Hbsendtype,Total_Amount,State,Statedetail,Mch_Billno,send_listid) values('" + guid + "','" + ccnactivityid + "','" + ccnactivityname + "','" + facid + "','" + I_USERNAME + "','" + I_USERMOBILE + "','" + I_OPENID + "','" + hbsendtype + "','" + money + "','" + state + "','" + statedetail + "','" + mch_billno + "','" + send_listid + "') ";

                }

                if (dataBase.ExecuteNonQuery(CommandType.Text, sql, null) > 0)
                {
                    bRet = true;
                    if (hbsendtype == "2")//更新邀请表
                    {
                        string sql_update = "update T_SGM_SHAKE_Invite xx  set xx.invitelotteryflag='1' , xx.invitelotteryguid='" + I_ED_LOTTERYGUID + "' ,  xx.invitelotterydate=sysdate where  xx.facid='" + facid + "' and  xx.inviteto='" + I_ED_USERMOBILE + "' and  xx.invitefrom='" + I_USERMOBILE + "' ";
                        if (dataBase.ExecuteNonQuery(CommandType.Text, sql_update, null) > 0)
                        {
                            bRet = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.Common.LotteryBusiness.cs--InsertIntoHbDetail()--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion


        #region 司能分享朋友圈时，向 T_SGM_SHAKE_CHECKTOLOG加入一条分享数据
        /// <summary>
        /// 向homepage页添加记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="openid">分享人的openid</param>
        /// <param name="lid">分享人的中奖guid</param>
        /// <param name="sharetype">类别(1:海报link、分享link)</param>
        /// <param name="linetype">1 代表进入线上homepage   2 代表进入线下homepage</param>
        /// <param name="channel">通道 ：0 代表线下  ；；线上：  1:广告 2:短信推送 3:微信公众号 4:朋友圈转发</param>
        /// <returns></returns>
        public bool AddHomePageRecord(string facid, string openid, string lid, string sharetype, string linetype, string channel)
        {
            bool bRet = false;
            try
            {
                OracleParameter[] parms = (OracleParameter[])ParameterCache.GetParams("AddHomePageRecord");
                //构造参数
                if (parms == null)
                {
                    //GUID,SHAREOPENID,SHAREOPENID,LOTTERYGUID,TYPE,LINETYPE,CHANNEL
                    parms = new OracleParameter[7];
                    parms[0] = new OracleParameter(":GUID", OracleType.VarChar, 32);
                    parms[1] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    parms[2] = new OracleParameter(":SHAREOPENID", OracleType.VarChar, 32);
                    parms[3] = new OracleParameter(":LOTTERYGUID", OracleType.VarChar, 32);
                    parms[4] = new OracleParameter(":TYPE", OracleType.VarChar, 1);
                    parms[5] = new OracleParameter(":LINETYPE", OracleType.VarChar, 1);
                    parms[6] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                    //将参数加入缓存
                    ParameterCache.PushCache("AddHomePageRecord", parms);
                }
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                parms[0].Value = guid;
                parms[1].Value = facid;
                parms[2].Value = openid;
                parms[3].Value = lid;
                parms[4].Value = sharetype;
                parms[5].Value = linetype;
                parms[6].Value = channel;

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();

                    string sql = "insert  into T_SGM_SHAKE_CHECKTOLOG (GUID,FACID,SHAREOPENID,LOTTERYGUID,TYPE,LINETYPE,CHANNEL)values(:GUID,:FACID,:SHAREOPENID,:LOTTERYGUID,:TYPE,:LINETYPE,:CHANNEL)";


                    if (dataBase.ExecuteNonQuery(CommandType.Text, sql, parms) > 0)
                    {
                        bRet = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.Common.LotteryBusiness.cs--AddHomePageRecord() 传递参数[facid：" + facid + "] [openid:" + openid + "] [lid:" + lid + "] [sharetype:" + sharetype + "] [linetype:" + linetype + "] [channel:" + channel + "]--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion


        #region 校验是否已经给邀请人发放过奖励，如果发放过将不重复发放
        /// <summary>
        /// 校验是否已经给邀请人发放过奖励，如果发放过将不重复发放
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="frommobile">邀请人手机号</param>
        /// <param name="fromopenid">邀请人openid</param>
        /// <param name="tomobile">被邀请人手机号</param>
        /// <param name="toopenid">被邀请人openid</param>
        /// <param name="hbsendtype">1:扫码奖励，2：邀请好友奖励，3：分享奖励</param>
        /// <returns></returns>
        public bool CheckIsSendHbToInvite(string facid, string frommobile, string fromopenid, string tomobile, string toopenid, string hbsendtype)
        {
            bool bRet = false;
            try
            {


                OracleParameter[] parms = (OracleParameter[])ParameterCache.GetParams("CheckIsSendHbToInvite");
                //构造参数
                if (parms == null)
                {
                    //GUID,SHAREOPENID,SHAREOPENID,LOTTERYGUID,TYPE,LINETYPE,CHANNEL
                    parms = new OracleParameter[6];
                    parms[0] = new OracleParameter(":FACID", OracleType.VarChar, 10);
                    parms[1] = new OracleParameter(":USERMOBILE", OracleType.VarChar, 32);
                    parms[2] = new OracleParameter(":USEROPENID", OracleType.VarChar, 32);
                    parms[3] = new OracleParameter(":IUSERMOBILE", OracleType.VarChar, 32);
                    parms[4] = new OracleParameter(":IUSEROPENID", OracleType.VarChar, 32);
                    parms[5] = new OracleParameter(":HBSENDTYPE", OracleType.VarChar, 1);

                    //将参数加入缓存
                    ParameterCache.PushCache("CheckIsSendHbToInvite", parms);
                }

                parms[0].Value = facid;
                parms[1].Value = frommobile;
                parms[2].Value = fromopenid;
                parms[3].Value = tomobile;
                parms[4].Value = toopenid;
                parms[5].Value = hbsendtype;

                if (parms != null)
                {
                    DataBase dataBase = new DataBase();

                    string sql = "SELECT B.*, B.ROWID FROM t_smg_wxpay_hbdetail B WHERE B.FACID =:FACID  and b.usermobile =:USERMOBILE  and b.useropenid =:USEROPENID   and b.iusermobile =:IUSERMOBILE  and b.iuseropenid =:IUSEROPENID   and b.state = 'SUCCESS'  and b.hbsendtype =:HBSENDTYPE  and b.deleteflag = '1' order by b.createtime desc ";

                    DataTable dt = dataBase.ExecuteQuery(CommandType.Text, sql, parms);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        bRet = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.Common.LotteryBusiness.cs--CheckIsSendHbToInvite() 传递参数[facid：" + facid + "] [frommobile:" + frommobile + "] [fromopenid:" + fromopenid + "] [tomobile:" + tomobile + "] [toopenid:" + toopenid + "] [hbsendtype:" + hbsendtype + "]--" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }

            return bRet;


        }
        #endregion



        #region 解码
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qrcode"></param>
        /// <returns></returns>
        public string GetCode(string qrcode)
        {
            string code = string.Empty;
            #region 是否是二维码
            IDecoderAble cd = null;
            if (RegexExpress.IsQRCode(qrcode))
            {
                try
                {
                    qrcode = System.Web.HttpUtility.UrlDecode(qrcode);
                    cd = CCN.Code2D.CodeManager.CreateGetDecoder(qrcode);
                    if (cd.Success)
                    {
                        code = cd.DecodeEntity.Digit;
                    }
                }
                catch (Exception ex)
                {
                    Logger.AppLog.Write("KMSLotterySystemFront.Common.LotteryBusiness.cs--GetCode() 传递参数[qrcode：" + qrcode + "] --" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
                }
            }
            #endregion
            return code;
        }
        #endregion
    }
}
