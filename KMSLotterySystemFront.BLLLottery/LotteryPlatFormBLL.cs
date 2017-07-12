using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.DAL;

namespace KMSLotterySystemFront.BLLLottery
{

    public class LotteryPlatFormBLL
    {
        LotteryPlatFormDal dao = new LotteryPlatFormDal();

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
                dao.AddLotteryLog(factoryID, code, vip, param, reply, funName, state, channel, fromIP);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.PubInfo.AddLotteryLog msg [factoryID:" + factoryID + "] [code:" + code + "]  ----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);

            }

        }
    }
}
