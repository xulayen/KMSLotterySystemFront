using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using KMSLotterySystemFront.DBUtility;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront.DAL
{
    public class LotteryPlatFormDal
    {

        DataBase dataBase = new DataBase();


        /// <summary>
        /// 添加抽奖平台日志
        /// </summary>
        /// <param name="factoryID">厂家编号</param>
        /// <param name="code">数码</param>
        /// <param name="vip">来源IP地址/手机号码</param>
        /// <param name="param">入参</param>
        /// <param name="reply">返回的答复信息</param>
        /// <param name="funName">方法名称</param>
        /// <param name="state">系统执行状态</param>
        /// <param name="channel">查询渠道</param>
        /// <param name="fromIP">接口调用服务器IP</param>
        public  void AddLotteryLog(string factoryID, string code, string vip, string param, string reply, string funName, string state, string channel, string fromIP)
        {
            try
            {
                OracleParameter[] dbparam = new OracleParameter[9];

                dbparam[0] = new OracleParameter(":FACTORYID", OracleType.VarChar, 10);
                dbparam[1] = new OracleParameter(":CODE", OracleType.VarChar, 32);
                dbparam[2] = new OracleParameter(":VIP", OracleType.VarChar, 100);
                dbparam[3] = new OracleParameter(":PARAM", OracleType.VarChar, 3000);
                dbparam[4] = new OracleParameter(":REPLY", OracleType.VarChar, 3000);
                dbparam[5] = new OracleParameter(":FUNNAME", OracleType.VarChar, 50);
                dbparam[6] = new OracleParameter(":STATE", OracleType.VarChar, 10);
                dbparam[7] = new OracleParameter(":CHANNEL", OracleType.VarChar, 1);
                dbparam[8] = new OracleParameter(":FROMIP", OracleType.VarChar, 30);


                dbparam[0].Value = factoryID;
                dbparam[1].Value = code;
                dbparam[2].Value = vip;
                dbparam[3].Value = param;
                dbparam[4].Value = reply;
                dbparam[5].Value = funName;
                dbparam[6].Value = state;
                dbparam[7].Value = channel;
                dbparam[8].Value = fromIP;


                string sql = "INSERT INTO T_CCN_COMMON_LOTTERYLOG(FACTORYID,CODE,VIP,PARAM,REPLY,FUNNAME,STATE,CHANNEL,FROMIP) VALUES (:FACTORYID,:CODE,:VIP,:PARAM,:REPLY,:FUNNAME,:STATE,:CHANNEL,:FROMIP)";

                dataBase.ExecuteNonQuery(CommandType.Text, sql, dbparam);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.DAL.PubInfoDao.AddLotteryLog msg[factoryID:" + factoryID + "] [code:" + code + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
        }
    }
}
