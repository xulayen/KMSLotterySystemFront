// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery
// *文件名称：DigitcodeBLL.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：数码相关业务逻辑类
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.DAL;
using KMSLotterySystemFront.Common;
using System.Data;


namespace KMSLotterySystemFront.BLLLottery
{
    public class DigitcodeBLL
    {
        #region 公共变量
        public readonly static DigitcodeDao Ddao = new DigitcodeDao();
        #endregion

        #region 1) 检查表名是否存在
        /// <summary>
        /// 检查表名是否存在
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public bool CheckTableIsExist(string facid, string tablename)
        {
            bool flag = false;
            try
            {
                flag = Ddao.CheckTableIsExist(facid, tablename);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:CheckTableIsExist:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion

        #region 2) 检查产品是否存在
        /// <summary>
        /// 检查产品是否存在
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="proid"></param>
        /// <returns></returns>
        public bool CheckFacProduct(string facid, string proid)
        {
            bool flag = false;
            try
            {
                flag = Ddao.CheckFacProduct(facid, proid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:CheckFacProduct:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion

        #region 3) 获取数码对应P表的表结构
        /// <summary>
        /// 获取数码对应P表的表结构
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="tablename">表名</param>
        /// <param name="newcode">二次加密的数码</param>
        /// <param name="sql">所执行的sql</param>
        /// <returns></returns>
        public bool GetTableColmunList(string facid, string tablename, string newcode, out string sql)
        {
            bool flag = false;
            sql = "";
            try
            {
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(facid, tablename);
                if (colnumList != null && colnumList.Count > 0)
                {
                    #region 构造防伪查询语句
                    StringBuilder strFWSQL = new StringBuilder("SELECT SPRODUCTCODE,VISIT,VDATE");
                    if (colnumList.Contains("NUM"))
                    {
                        strFWSQL.Append(",NUM");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL NUM");
                    }
                    if (colnumList.Contains("ATTRIBUTE"))
                    {
                        strFWSQL.Append(",ATTRIBUTE");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL ATTRIBUTE");
                    }
                    if (colnumList.Contains("LOGICCOLOR"))
                    {
                        strFWSQL.Append(",LOGICCOLOR");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL LOGICCOLOR");
                    }
                    if (colnumList.Contains("CCNFAC"))
                    {
                        strFWSQL.Append(",CCNFAC");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL CCNFAC");
                    }
                    if (colnumList.Contains("CCNPRO"))
                    {
                        strFWSQL.Append(",CCNPRO");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL CCNPRO");
                    }
                    if (colnumList.Contains("OUTPROID"))
                    {
                        strFWSQL.Append(",OUTPROID");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL OUTPROID");
                    }

                    //产品激活标记，此标记用于产品回传激活判断
                    if (colnumList.Contains("ACTIVATION_FLAG"))
                    {
                        strFWSQL.Append(",ACTIVATION_FLAG");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL ACTIVATION_FLAG");
                    }


                    //产品批次
                    if (colnumList.Contains("BATCHID"))
                    {
                        strFWSQL.Append(",BATCHID");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL BATCHID");
                    }


                    //产品生产批次
                    if (colnumList.Contains("BATCH"))
                    {
                        strFWSQL.Append(",BATCH");
                    }
                    else
                    {
                        strFWSQL.Append(",NULL BATCH");
                    }

                    strFWSQL.Append(" FROM " + tablename + " WHERE SPRODUCTCODE='" + newcode + "'");
                    #endregion

                    flag = true;
                    sql = strFWSQL.ToString();
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:CheckFacProduct:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion

        #region 4) 获取数码相关的信息
        /// <summary>
        /// 获取数码相关的信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="fwSql">查询sql</param>
        /// <param name="codefacid">数码所属工厂</param>
        /// <param name="proid">数码所属产品</param>
        /// <param name="codeCreateDate">生码日期</param>
        /// <returns></returns>
        public CodeAttachedInfo GetCodeAttachedInfo(string facid, string fwSql, string codefacid, string proid, string codeCreateDate)
        {
            CodeAttachedInfo codeAttachedInfo = new CodeAttachedInfo();
            try
            {
                #region 1) 获取P表数据

                DataSet dsRet = Ddao.GetDataBySql(fwSql);
                if (dsRet != null && dsRet.Tables[0].Rows.Count > 0)
                {
                    //被查询过
                    if (dsRet.Tables[0].Rows[0]["VISIT"].ToString() == "1")
                    {
                        codeAttachedInfo.IsQueryed = true;
                        if (dsRet.Tables[0].Rows[0]["VDATE"].ToString() != "")
                        {
                            codeAttachedInfo.FirstQueryDate = DateTime.Parse(dsRet.Tables[0].Rows[0]["VDATE"].ToString());
                        }
                        if (dsRet.Tables[0].Rows[0]["NUM"] != null)
                        {
                            if (dsRet.Tables[0].Rows[0]["NUM"].ToString() != "")
                            {
                                codeAttachedInfo.QueryNum = int.Parse(dsRet.Tables[0].Rows[0]["NUM"].ToString());
                            }
                        }
                    }
                    else
                    {
                        codeAttachedInfo.IsQueryed = false;
                        codeAttachedInfo.QueryNum = 0;
                    }
                    if (dsRet.Tables[0].Rows[0]["ATTRIBUTE"] != null)
                    {
                        codeAttachedInfo.SaleArea = dsRet.Tables[0].Rows[0]["ATTRIBUTE"].ToString();
                    }
                    if (dsRet.Tables[0].Rows[0]["LOGICCOLOR"] != null)
                    {
                        codeAttachedInfo.LogicColor = dsRet.Tables[0].Rows[0]["LOGICCOLOR"].ToString();
                    }

                    if (dsRet.Tables[0].Rows[0]["OUTPROID"] != null)
                    {
                        codeAttachedInfo.ProID = dsRet.Tables[0].Rows[0]["OUTPROID"].ToString();
                    }

                    if (!String.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["CCNFAC"].ToString()))
                    {
                        codeAttachedInfo.FacID = dsRet.Tables[0].Rows[0]["CCNFAC"].ToString();
                    }
                    else
                    {
                        codeAttachedInfo.FacID = codefacid;
                    }


                    codeAttachedInfo.CreateCodeProID = proid;

                    //if (!String.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["CCNPRO"].ToString()))
                    //{
                    //    codeAttachedInfo.CreateCodeProID = dsRet.Tables[0].Rows[0]["CCNPRO"].ToString();
                    //}
                    //else
                    //{
                    //    codeAttachedInfo.CreateCodeProID = proid;
                    //}

                    // 数码回传激活标志
                    if (!String.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["ACTIVATION_FLAG"].ToString()))
                    {
                        codeAttachedInfo.Activation_Flag = dsRet.Tables[0].Rows[0]["ACTIVATION_FLAG"].ToString();
                    }
                    else
                    {
                        codeAttachedInfo.Activation_Flag = "";
                    }


                    // 批次
                    if (!String.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["BATCHID"].ToString()))
                    {
                        codeAttachedInfo.BatchID = dsRet.Tables[0].Rows[0]["BATCHID"].ToString();
                    }
                    else
                    {
                        codeAttachedInfo.BatchID = "";
                    }


                    // 产品生产批次
                    if (!String.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["BATCH"].ToString()))
                    {
                        codeAttachedInfo.Batch = dsRet.Tables[0].Rows[0]["BATCH"].ToString();
                    }
                    else
                    {
                        codeAttachedInfo.Batch = "";
                    }

                    codeAttachedInfo.CodeIsExist = true;
                }
                else
                {
                    codeAttachedInfo.CodeIsExist = false;
                }
                #endregion

                #region 2)获取产品表相关配置信息
                //数码存在情况下
                if (codeAttachedInfo.CodeIsExist)
                {
                    DataTable dbProInfoRet = GetCodeFactoryProduct(facid, codeAttachedInfo.FacID, codeAttachedInfo.CreateCodeProID, codeCreateDate);
                    if (dbProInfoRet != null && dbProInfoRet.Rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(dbProInfoRet.Rows[0]["AGAINNUM"].ToString()))
                        {
                            codeAttachedInfo.AllowQueryMaxNum = int.Parse(dbProInfoRet.Rows[0]["AGAINNUM"].ToString());
                        }
                        if (dbProInfoRet.Rows[0]["LOGICCOLOR"].ToString() == "1")
                        {
                            codeAttachedInfo.IsLogicColor = true;
                        }
                        else
                        {
                            codeAttachedInfo.IsLogicColor = false;
                        }
                        if (dbProInfoRet.Rows[0]["ISACTIVATIONFLAG"].ToString() == "1")
                        {
                            codeAttachedInfo.Is_Activation_Flag = true;
                        }
                        else
                        {
                            codeAttachedInfo.Is_Activation_Flag = false;
                        }
                        codeAttachedInfo.LogicColorReadInfo = dbProInfoRet.Rows[0]["LOGICINFO"].ToString();
                        codeAttachedInfo.OldFacID = dbProInfoRet.Rows[0]["FACIDOLD"].ToString();
                        codeAttachedInfo.ProductName = dbProInfoRet.Rows[0]["PRODUCTNAME"].ToString();
                        codeAttachedInfo.FactoryName = dbProInfoRet.Rows[0]["FACTORYNAME"].ToString();
                        codeAttachedInfo.CodeEffectiveNum = int.Parse(dbProInfoRet.Rows[0]["CUSTODY"].ToString());//yhr20101116 数码有效期
                        codeAttachedInfo.CodeExistedDay = int.Parse(dbProInfoRet.Rows[0]["CodeExistedDay"].ToString());//yhr20101121 数码已存在天数
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:GetCodeAttachedInfo:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return codeAttachedInfo;
            }
            return codeAttachedInfo;
        }

        #endregion

        #region 5) 通过IP或者电话或者手机获取城市
        /// <summary>
        /// 通过IP或者电话或者手机获取城市
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public bool GetCityCode(string ip, out string cityCode)
        {
            bool flag = false;
            cityCode = "未知地区";
            string provice = "未知地区";
            try
            {
                if (Common.RegexExpress.IsIP(ip))
                {
                    Ddao.QueryProviceAndCityByIP(ip, ref provice, ref cityCode);
                }
                else
                {
                    Ddao.QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref cityCode);
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:GetCityCode:" + ip + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion


        #region 5) 通过IP或者电话或者手机获取城市
        /// <summary>
        /// 通过IP或者电话或者手机获取城市
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public bool GetCityCode(string ip, out string provice, out string cityCode)
        {
            bool flag = false;
            cityCode = "未知地区";
            provice = "未知地区";
            try
            {
                if (Common.RegexExpress.IsIP(ip))
                {
                    Ddao.QueryProviceAndCityByIP(ip, ref provice, ref cityCode);
                }
                else
                {
                    Ddao.QueryProviceAndCityByPhoneAndMobile(ip, ref provice, ref cityCode);
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:GetCityCode:" + ip + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }
        #endregion

        #region 6) 复查记录P表、T厂家表，T厂家产品表
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
            return Ddao.ModifyCodeNotFirstQueryInfo(tableName_p, tableName_t, tableName_tp, tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, called, newcode, channel, isEncypt, codeCreateDate);
        }

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
        /// <param name="isGPS">IP是否为GPS定位数据</param>
        /// <param name="ProvinceName">根据GPS定位获取的省份名称</param>
        /// <param name="CityName">根据GPS定位获取的城市名称</param>
        /// <returns>是否成功</returns>
        public bool ModifyCodeNotFirstQueryInfo_GPS(string tableName_p, string tableName_t, string tableName_tp, string tracetype, string code, string ip, string attribute, string ccnfac, string ccnpro, string outproid, string called, string newcode, string channel, bool isEncypt, string codeCreateDate, bool isGPS, string ProvinceName, string CityName)
        {
            return Ddao.ModifyCodeNotFirstQueryInfo_GPS(tableName_p, tableName_t, tableName_tp, tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, called, newcode, channel, isEncypt, codeCreateDate, isGPS, ProvinceName, CityName);
        }

        #endregion

        #region 7) 首次查询记录P表、T厂家表，T厂家产品表
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
            return Ddao.ModifyCodeFirstQueryInfo(tableName_p, tableName_t, tableName_tp, tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, called, newcode, channel, isEncypt, codeCreateDate);
        }

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
            return Ddao.ModifyCodeFirstQueryInfo_GPS(tableName_p, tableName_t, tableName_tp, tracetype, code, ip, attribute, ccnfac, ccnpro, outproid, called, newcode, channel, isEncypt, codeCreateDate, isGPS, ProvinceName, CityName);
        }

        #endregion

        #region 8) 获取数码信息
        /// <summary>
        /// 获取数码信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="table">表名</param>
        /// <param name="code">数码</param>
        /// <param name="lproductcode">输出：数码</param>
        /// <param name="tdate">输出：首次查询时间</param>
        /// <param name="ip">输出：查询地址</param>
        /// <param name="channel">输出：查询渠道</param>
        /// <returns></returns>
        public bool GetCodeMegByCode(string facid, string table, string code, out string lproductcode, out string tdate, out string ip, out string channel)
        {
            bool flag = false;
            lproductcode = "";
            tdate = "";
            ip = "";
            channel = "";
            try
            {
                DataSet dsRet = Ddao.GetCodeMessageByCode(table, code);
                if (dsRet != null && dsRet.Tables[0].Rows.Count > 0)
                {

                    if (!string.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["LPRODUCTCODE"].ToString()))
                    {
                        lproductcode = dsRet.Tables[0].Rows[0]["LPRODUCTCODE"].ToString();
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["TDATE"].ToString()))
                    {
                        tdate = dsRet.Tables[0].Rows[0]["TDATE"].ToString();
                    }
                    if (!string.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["IP"].ToString()))
                    {
                        ip = dsRet.Tables[0].Rows[0]["IP"].ToString();
                    }
                    if (!string.IsNullOrEmpty(dsRet.Tables[0].Rows[0]["CATEGORY"].ToString()))
                    {
                        channel = dsRet.Tables[0].Rows[0]["CATEGORY"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:GetCodeMegByCode:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }
            return flag;
        }

        #endregion

        #region 9) 判断数码是否需要回传激活
        /// <summary>
        /// 判断数码是否需要回传激活
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="prodic">产品编号</param>
        /// <returns></returns>
        public bool CheckProductIsAct(string factoryid, string prodic)
        {
            bool flag = false;

            try
            {
                return Ddao.CheckProductIsAct(factoryid, prodic);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:CheckProductIsAct: ---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return flag;
            }

        }
        #endregion

        #region 10) 获取产品表信息
        /// <summary>
        /// 获取产品表信息
        /// </summary>
        /// <param name="factoryid">调用厂家编号</param>
        /// <param name="facid">虚拟厂家编号</param>
        /// <param name="proid">虚拟产品编号</param>
        /// <returns>返回厂家产品配置信息</returns>
        public DataTable GetCodeFactoryProduct(string directoryName, string facid, string proid, string codeCreateDate)
        {
            DataTable dbProInfoRet = null;
            try
            {
                return Ddao.GetCodeFactoryProduct(directoryName, facid, proid, codeCreateDate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("DigitcodeBLL:GetCodeFactoryProduct: ---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return dbProInfoRet;
            }
        }
        #endregion

        #region 11)检测颜色是否符合规则
        /// <summary>
        /// 检查颜色是否符合规则
        /// </summary>
        /// <param name="_color">16位逻辑变色的代码或者是代号集合，以符号（:）分隔</param>
        /// <returns></returns>
        public bool CheckColorLen(string _color)
        {
            bool flag = true;
            try
            {
                if (_color.IndexOf(":") > 1)
                {
                    string[] clist = _color.Split(':');
                    if (clist.Length == 16)
                    {
                        foreach (string c in clist)
                        {
                            if (!string.IsNullOrEmpty(c) && c.Length == 2)
                            {
                                continue;
                            }
                            else
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }

            }
            catch (Exception e)
            {
                flag = false;
            }

            return flag;

        }
        #endregion
    }
}
