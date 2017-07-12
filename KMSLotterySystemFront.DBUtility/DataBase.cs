// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.DBUtility
// *文件名称：DataBase.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：数据库操作实体
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Reflection;
using System.Configuration;

namespace KMSLotterySystemFront.DBUtility
{
    public sealed class DataBase
    {
        #region 数据库连接字符
        //数据库连接字符串，从Config文件中直接读取．在给对象分配内存时就直接读取，以后就可以从缓存中读取，以提高执行速度
        private string m_connectionStringSettingName = null;
        private ConnectionStringSettings connectionStringSettings = null;
        private DbProviderFactory DBProviderFactory = null;

        #endregion

        #region 数据库对象声明
        //数据库连接对象
        private DbConnection connection = null;
        //数据库操作命令对象
        private DbCommand command = null;
        //数据库事务对象
        private DbTransaction trans = null;
        #endregion

        /// <summary>
        ///数据库连接字符串
        /// </summary>
        public static string dbConnectionString = string.Empty;

        /// <summary>
        ///数据库连接驱动
        /// </summary>
        private static string dbProviderName = "System.Data.OracleClient";

        /// <summary>
        /// 数据库连接驱动
        /// SQL：    System.Data.SqlClent
        /// Oracle： System.Data.OracleClient
        /// </summary>
        public static string DbBroviderName
        {
            get { return DataBase.dbProviderName; }
            set { DataBase.dbProviderName = value; }
        }

        /// <summary>
        /// 返回连接字符串
        /// </summary>
        /// <returns></returns>
        public static string DBConnectionString
        {
            get
            {
                #region 备份
                ////只实例化一次
                //if (string.IsNullOrEmpty(dbConnectionString))
                //{
                //    try
                //    {



                //        string clientid = "111";
                //        if (ConfigurationManager.AppSettings["ClientID"] != null)
                //        {
                //            clientid = ConfigurationManager.AppSettings["ClientID"].ToString();
                //        }

                //        if (clientid.Equals("AA0002"))
                //        {
                //            dbConnectionString = ConfigurationManager.ConnectionStrings["OracleConnectionStringTest"].ToString();
                //        }
                //        else
                //        {
                //            System.Reflection.Assembly ass = Assembly.Load("CommonDbManagment, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7d575d636298cb4d");
                //            object app = ass.CreateInstance("CCN.DbManagement.CompanyDbManagement");
                //            if (app != null)
                //                dbConnectionString = app.GetType().InvokeMember("OraConnString",
                //                               BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                //                               null, app, new object[] { clientid }) as string;
                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}
                //return dbConnectionString;
                #endregion

                try
                {
                    #region 判断是否在测试环境
                    string clientid = "0";
                    if (ConfigurationManager.AppSettings["IsDebug"] != null)
                    {
                        clientid = ConfigurationManager.AppSettings["IsDebug"].ToString();
                    }
                    #endregion

                     
                    if (clientid.Equals("1"))//测试环境
                    {
                        dbConnectionString = ConfigurationManager.ConnectionStrings["OracleConnectionStringTest"].ToString();
                    }
                    else
                    {
                        #region 正式环境统一密码获取方式
                        //检查新老平台
                        string pftype = "0";
                        pftype = ConfigurationManager.AppSettings["PlatformType"].ToString();
                        if (ConfigurationManager.AppSettings["PlatformType"] != null)
                        {
                            pftype = ConfigurationManager.AppSettings["PlatformType"].ToString();
                        }

                        //0:老平台 1:新平台
                        if (pftype.Equals("0"))
                        {
                            #region 老平台统一密码组件获取方式
                            if (string.IsNullOrEmpty(dbConnectionString))
                            {
                                if (ConfigurationManager.AppSettings["ClientID"] != null)
                                {
                                    clientid = ConfigurationManager.AppSettings["ClientID"].ToString();
                                }

                                System.Reflection.Assembly ass = Assembly.Load("CommonDbManagment, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7d575d636298cb4d");
                                object app = ass.CreateInstance("CCN.DbManagement.CompanyDbManagement");
                                if (app != null)
                                    dbConnectionString = app.GetType().InvokeMember("OraConnString",
                                                   BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                                                   null, app, new object[] { clientid }) as string;

                                KMSLotterySystemFront.Logger.AppLog.Write("老平台链接字符串为:" + dbConnectionString, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                            }
                            #endregion
                        }
                        else
                        {
                            #region 新平台统一密码获取方式
                            if (string.IsNullOrEmpty(dbConnectionString))
                            {
                                string newClientId = "a001";
                                if (ConfigurationManager.AppSettings["ClientID"] != null)
                                {
                                    newClientId = ConfigurationManager.AppSettings["ClientID"].ToString();
                                }

                                DBCSLib.Database db = new DBCSLib.Database();
                                db.GetRemoteData(newClientId);

                                string username = db.DatabaseUsername();
                                string password = db.DatabasePassword();

                                dbConnectionString = "Data Source={0};User ID={1};Password={2};";
                                if (ConfigurationManager.AppSettings["NewConnectionString"] != null)
                                {
                                    dbConnectionString = ConfigurationManager.AppSettings["NewConnectionString"].ToString();
                                }
                                dbConnectionString = string.Format(dbConnectionString, username, password);

                                KMSLotterySystemFront.Logger.AppLog.Write("新平台链接字符串为:" + dbConnectionString, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                            }
                            #endregion
                        }
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    KMSLotterySystemFront.Logger.AppLog.Write("获取统一密码组件-初始化为-异常" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                    throw ex;
                }

                return dbConnectionString;
            }
        }

        private bool bIsReading;

        #region 构造函数
        /// <summary>
        /// 空参构造函数,创建默认数据库的数据工厂
        /// </summary>
        public DataBase()
        {

            try
            {
                this.m_connectionStringSettingName = DBConnectionString;
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    string newdbconnection = DBConnectionString.ToString();
                    if (string.IsNullOrEmpty(newdbconnection))
                    {
                        throw new ApplicationException("数据库配置异常,请检查节点ConnectionStringSettingName是否存在或未初始化!");
                    }
                    else
                    {
                        this.m_connectionStringSettingName = newdbconnection;
                    }
                }
                //this.connectionStringSettings = DBConnectionString;
                this.DBProviderFactory = DbProviderFactories.GetFactory(DbBroviderName);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            //try
            //{
            //    this.m_connectionStringSettingName = ConfigurationManager.AppSettings.Get("ConnectionStringSettingName");
            //    //this.m_connectionStringSettingName = CCN.Common.SysConfig.Instance.ConnectionStringSettingName;
            //    if (m_connectionStringSettingName == null || m_connectionStringSettingName == "")
            //    {
            //        throw new ApplicationException("数据库配置异常,请检查节点ConnectionStringSettingName是否存在或未初始化!");
            //    }
            //    else
            //    {
            //        this.connectionStringSettings = ConfigurationManager.ConnectionStrings[m_connectionStringSettingName];
            //        if (connectionStringSettings == null)
            //        {
            //            throw new ApplicationException("数据库配置异常,请检查节点" + m_connectionStringSettingName + "是否存在!");
            //        }
            //        else
            //        {
            //            try
            //            {
            //                this.DBProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            //            }
            //            catch
            //            {
            //                throw new ApplicationException("数据库配置异常导致数据工厂创建失败,请检查连接字符串！");
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// 带参构造函数,创建指定数据库的数据工厂
        /// </summary>
        /// <param name="connectionStringSettingName">指定的数据库的配置</param>
        public DataBase(string connectionStringSettingName)
        {
            try
            {
                this.m_connectionStringSettingName = DBConnectionString;
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ApplicationException("数据库配置异常,请检查节点ConnectionStringSettingName是否存在或未初始化!");
                }
                //this.connectionStringSettings = DBConnectionString;
                this.DBProviderFactory = DbProviderFactories.GetFactory(DbBroviderName);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            //try
            //{
            //    if (connectionStringSettingName == null || connectionStringSettingName == "")
            //    {
            //        throw new ApplicationException("数据库配置异常,请检查节点ConnectionStringSettingName是否存在!");
            //    }
            //    else
            //    {
            //        this.connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringSettingName];
            //        if (connectionStringSettings == null)
            //        {
            //            throw new ApplicationException("数据库配置异常,请检查节点" + connectionStringSettingName + "是否存在!");
            //        }
            //        else
            //        {
            //            try
            //            {
            //                this.DBProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            //            }
            //            catch
            //            {
            //                throw new ApplicationException("数据库配置异常导致数据工厂创建失败,请检查连接字符串！");
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region 打开数据库连接对象
        /// <summary>
        ///打开数据库连接对象，并实例化Command命令对象
        /// </summary>
        private void openDB()
        {
            if (connection == null)
            {
                this.connection = DBProviderFactory.CreateConnection();
                //this.connection.ConnectionString = connectionStringSettings.ConnectionString;
                this.connection.ConnectionString = dbConnectionString.ToString();
            }
            if (this.connection.State == ConnectionState.Closed && this.connection != null)
            {
                this.connection.Open();
                //实例化一个命令对象
                this.command = this.connection.CreateCommand();
            }
        }
        #endregion

        #region 关闭数据库连接对象
        /// <summary>
        /// 关闭数据库连接对象
        /// </summary>
        private void closeDB()
        {
            if (this.connection.State != ConnectionState.Closed)
            {
                this.connection.Close();
                this.connection = null;
                bIsReading = false;
            }
            if (trans != null)
            {
                trans = null;
            }
        }
        #endregion

        #region 开始事务
        /// <summary>
        /// 开始事务
        /// 因为OracleConnection 不支持并行事务。所以在添加事务前必须要检查是否为空！
        /// </summary>
        public void BeginTrans()
        {
            this.openDB();
            if (this.trans == null)
            {
                this.trans = this.connection.BeginTransaction();
            }
            //if (this.command.Transaction!= null)
            //{
            this.command.Transaction = this.trans;
            //}
        }
        #endregion

        #region 开始事务,指定事务隔离级别
        /// <summary>
        /// 开始事务,指定事务隔离级别
        /// </summary>
        public void BeginTrans(IsolationLevel isoLevel)
        {
            this.openDB();
            this.trans = this.connection.BeginTransaction(isoLevel);
            this.command.Transaction = this.trans;
        }
        #endregion

        #region 提交事务且关闭数据库连接
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTrans()
        {
            this.trans.Commit();
            this.closeDB();
        }
        #endregion

        #region 回滚事务且关闭数据库连接
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBackTrans()
        {
            this.trans.Rollback();
            this.closeDB();
        }
        #endregion

        #region 填充Command命令参数
        /// <summary>
        /// 填充Command命令
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <param name="type">命令对象执行类型</param>
        /// <param name="strSql">执行语句或过程名</param>
        /// <param name="prams">参数集合</param>
        private void PushCommand(IDbCommand cmd, CommandType type, string strSql, IDataParameter[] parms)
        {
            cmd.CommandType = type;
            cmd.CommandText = strSql;
            cmd.Parameters.Clear();
            if (parms == null)
            {
                return;
            }
            //IEnumerator pPrams=prams.GetEnumerator();	
            foreach (IDataParameter param in parms)
            {
                //防止参数在别处引用，用克隆的方法把参数加到command对象的参数集合中
                cmd.Parameters.Add(((ICloneable)param).Clone());
            }
        }
        #endregion

        #region 执行SQl语句或存储过程,返回数据读取流对象IDataReader
        /// <summary>
        /// 执行SQl语句或存储过程,返回数据读取流对象,在使用完DataReader对象后请关闭DataReader对象
        /// </summary>
        /// <param name="type">命令对象执行类型（过程或SQL语句）</param>
        /// <param name="strSql">执行语句或存储过程名</param>
        /// <param name="prams">参数集合</param>
        /// <returns>返回数据读取流对象</returns>
        public IDataReader ExecuteReader(CommandType type, string strSql, IDataParameter[] parms)
        {
            this.openDB();
            this.PushCommand(this.command, type, strSql, parms);
            try
            {
                //将CommandBehavior.CloseConnection传递给ExecuteReader方法，以确保相关的连接在关闭DataReader时被关闭
                //如果从一个方法返回DataReader，而且不能控制DataReader或相关连接的关闭，则这样做特别有用
                //注意：但如果在事务中这样做，则不保险
                bIsReading = true;
                return this.command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ep)
            {
                bIsReading = false;
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw ep;
            }
        }
        #endregion

        #region 执行SQl语句或存储过程,返回首行首列的值ExecuteScalar
        /// <summary>
        /// 执行SQl语句或存储过程,返回首行首列的值
        /// </summary>
        /// <param name="type">命令对象执行类型（过程或SQL语句）</param>
        /// <param name="strSql">执行语句或存储过程名</param>
        /// <param name="prams">参数集合</param>
        /// <returns>返回首行首列的值，Object类型</returns>
        public object ExecuteScalar(CommandType type, string strSql, IDataParameter[] parms)
        {
            this.openDB();
            try
            {
                this.PushCommand(this.command, type, strSql, parms);
                return this.command.ExecuteScalar();
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                if (this.trans == null && !bIsReading)
                {
                    this.closeDB();
                }
            }
        }
        #endregion

        #region ExecuteNonQuery返回受影响的行数
        /// <summary>
        /// 执行SQl语句或存储过程，不返回结果集的，只返回SQl语句或存储过程影响的行数
        /// </summary>
        /// <param name="type">命令对象执行类型（过程或SQL语句）</param>
        /// <param name="strSql">执行语句或存储过程名</param>
        /// <param name="prams">参数集合</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(CommandType type, string strSql, IDataParameter[] parms)
        {
            this.openDB();
            try
            {
                PushCommand(this.command, type, strSql, parms);
                return this.command.ExecuteNonQuery();
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                if (this.trans == null && !bIsReading)
                {
                    this.closeDB();
                }
            }
        }
        #endregion

        #region FillDataSet返回数据集对象
        /// <summary>
        /// 执行SQl语句或存储过程,返回数据集对象DataSet,可以向DataSet中添加多张指定过表名的表
        /// </summary>
        /// <param name="type">命令对象执行类型（存储过程或SQL语句）</param>
        /// <param name="strSql">执行SQl语句或存储过程名</param>
        /// <param name="prams">参数集合</param>
        /// <param name="strTableName">要填充数据的数据表名称</param>
        /// <param name="ds">要填充数据的数据集对象</param>
        /// <returns>返回数据集对象</returns>
        public void FillDataSet(CommandType type, string strSql, IDataParameter[] parms, string strTableName, DataSet ds)
        {
            try
            {
                if (strTableName == string.Empty)
                {
                    throw new ApplicationException("请指定要填充的表名!");
                }
                if (ds == null)
                {
                    throw new ApplicationException("请实例化要填充的DataSet对象!");
                }
                this.openDB();
                DbDataAdapter DAP = DBProviderFactory.CreateDataAdapter();
                this.PushCommand(this.command, type, strSql, parms);
                DAP.SelectCommand = this.command;
                DAP.Fill(ds, strTableName);
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                if (this.trans == null && !bIsReading)
                {
                    this.closeDB();
                }
            }
        }
        #endregion

        #region  执行SQl语句或存储过程,返回数据集对象DataSet
        /// <summary>
        /// 执行SQl语句或存储过程,返回数据集对象DataSet
        /// </summary>
        /// <param name="type">命令对象执行类型（存储过程或SQL语句）</param>
        /// <param name="strSql">执行SQl语句或存储过程名</param>
        /// <param name="prams">参数集合</param>
        /// <returns>返回数据集对象</returns>
        public DataSet GetDataSet(CommandType type, string strSql, IDataParameter[] parms)
        {
            DataSet ds = new DataSet();
            try
            {
                this.openDB();
                DbDataAdapter DAP = DBProviderFactory.CreateDataAdapter();
                this.PushCommand(this.command, type, strSql, parms);
                DAP.SelectCommand = this.command;
                DAP.Fill(ds);
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                if (this.trans == null && !bIsReading)
                {
                    this.closeDB();
                }
            }
            return ds;
        }
        #endregion


        #region 本方法用于执行查询,返回DataTable结果集
        /// <summary>
        /// 本方法用于执行查询,返回DataTable结果集
        /// </summary>
        /// <param name="type">Command命令类型</param>
        /// <param name="strSql">Command待执行语句</param>
        /// <param name="paramArr">Command参数数组</param>
        /// <returns>返回DataTable结果集</returns>
        public DataTable ExecuteQuery(CommandType type, string strSql, IDataParameter[] parms)
        {
            DataTable dt = new DataTable();
            try
            {
                this.openDB();
                DbDataAdapter DAP = DBProviderFactory.CreateDataAdapter();
                this.PushCommand(this.command, type, strSql, parms);
                DAP.SelectCommand = this.command;
                DAP.Fill(dt);
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                if (this.trans == null && !bIsReading)
                {
                    this.closeDB();
                }
            }
            return dt;
        }
        #endregion
    }
}
