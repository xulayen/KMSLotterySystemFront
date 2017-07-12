// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.BLLLottery
// *文件名称：LotteryBLL.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：奖项奖品相关业务逻辑类
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
using KMSLotterySystemFront.Model;
using System.Collections;
using System.Data;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront.BLLLottery
{
    public class LotteryBLL
    {

        #region 公共变量
        public readonly static LotteryDal ldao = new LotteryDal();
        public readonly static DigitcodeDao Ddao = new DigitcodeDao();
        #endregion

        #region 1) 获取GUID
        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <returns></returns>
        public string getGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        #endregion

        #region 2) 新增参与记录

        /// <summary>
        /// 新增参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编码</param>
        /// <param name="area">参与区域</param>
        /// <param name="protype">产品类别</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="result">下行答复信息</param>
        /// <returns>参与记录是否增加成功</returns>
        public bool AddLotteryLog(string facid, string ip, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                return ldao.AddLotteryLog(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 2.1) 新增参与记录(实物开奖+信息收集)
        /// <summary>
        /// 新增参与记录(实物开奖+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编码</param>
        /// <param name="area">参与区域</param>
        /// <param name="protype">产品类别</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="result">下行答复信息</param>
        /// <param name="userHsah">新注册信息</param>
        /// <returns>参与记录是否增加成功</returns>
        public bool AddLotteryLogAndRegister(string facid, string ip, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, Hashtable userHsah)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                userHsah.Add("GUID", guid);
                userHsah.Add("IP", ip);
                userHsah.Add("CATEGORY", channel);
                userHsah.Add("LOTTERYLEVEL", "0");

                string registersql = string.Empty;

                if (!InsertLotteryAndRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }
                return ldao.AddLotteryLogAndRegister(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryLogAndRegister:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 2.1) 话费充值+信息收集

        /// <summary>
        /// 新增参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">手机号/IP</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编码</param>
        /// <param name="area">参与区域</param>
        /// <param name="protype">产品类别</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="result">下行答复信息</param>
        /// <param name="userHsah">数码</param>
        /// <returns>参与记录是否增加成功</returns>
        public bool AddLotteryLog(string facid, string ip, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, Hashtable userHsah)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                userHsah.Add("GUID", guid);

                string registersql = string.Empty;

                if (!InsertRechargeRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }
                return ldao.AddLotteryLog(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, registersql);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 2.1) 话费充值+更多信息收集

        /// <summary>
        /// 新增参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">手机号/IP</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编码</param>
        /// <param name="area">参与区域</param>
        /// <param name="protype">产品类别</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="result">下行答复信息</param>
        /// <param name="userHsah">数码</param>
        /// <returns>参与记录是否增加成功</returns>
        public bool AddLotteryLog(string facid, string ip, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, Hashtable userHsah, bool isRegister, string systemState, bool isSpCode = false)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion

                //userHsah.Add("GUID", guid);
                //userHsah.Add("F10", systemState);

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid);
                }

                if (!userHsah.ContainsKey("F10"))
                {
                    userHsah.Add("F10", systemState);
                }


                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                if (!isRegister)
                {
                    registersql = "";
                }

                return ldao.AddLotteryLog(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, registersql, isSpCode);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion



        #region 2.2)   新增参与记录 ()

        /// <summary>
        /// 新增参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">手机号/IP</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型</param>
        /// <param name="activityId">活动编码</param>
        /// <param name="area">参与区域</param>
        /// <param name="protype">产品类别</param>
        /// <param name="awardsno">是否中奖</param>
        /// <param name="result">下行答复信息</param>
        /// <param name="userHsah">数码</param>
        /// <returns>参与记录是否增加成功</returns>
        public bool AddLotteryLogMP(string facid, string ip, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, bool isRegister, string systemState)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[result:" + result + "]");
                #endregion
                //userHsah.Add("GUID", guid);
                //userHsah.Add("F10", systemState);
                return ldao.AddLotteryLogMP(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryLogMP:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion







        #region 3) 真码参与日志
        /// <summary>
        /// 真码参与日志
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <returns></returns>
        public bool AddLotteryParLog(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid = getGuid();
                sb.Append("[guid:" + guid + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                #endregion

                return ldao.AddLotteryParLog(guid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 4) 添加参与记录和真码参与记录
        /// <summary>
        /// 添加参与记录和真码参与记录
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                return ldao.AddLotteryParLog2(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 4.1) 添加参与记录和真码参与记录 (+信息收集)
        /// <summary>
        /// 添加参与记录和真码参与记录(+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <param name="userHsah">收集信息集合</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog, Hashtable userHsah)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                userHsah.Add("GUID", guid1);


                string registersql = string.Empty;

                if (!InsertRechargeRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryParLog2(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 4.2) 添加参与记录和真码参与记录(实物开奖+信息收集)
        /// <summary>
        /// 添加参与记录和真码参与记录(实物开奖+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <param name="userHsah">新注册信息</param>
        /// <returns></returns>
        public bool AddLotteryParLogAndRegister(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog, Hashtable userHsah)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                userHsah.Add("GUID", guid1);
                userHsah.Add("IP", ip);
                userHsah.Add("CATEGORY", channel);
                userHsah.Add("LOTTERYLEVEL", "0");
                string registersql = string.Empty;

                if (!InsertLotteryAndRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }


                return ldao.AddLotteryParLogAndRegister(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLogAndRegister:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 4.1) 添加参与记录和真码参与记录 (+信息收集)
        /// <summary>
        /// 添加参与记录和真码参与记录(+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <param name="userHsah">收集信息集合</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog, Hashtable userHsah, bool isRegister, string systemState)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid1);
                }

                if (!userHsah.ContainsKey("F10"))
                {
                    userHsah.Add("F10", systemState);
                }

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryParLog2(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 添加参与记录和真码参与记录(+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <param name="userHsah">收集信息集合</param>
        /// <param name="isRegister">是否注册</param>
        /// <param name="systemState">系统执行状态</param>
        /// <param name="isSpCode">是否是中性码</param>
        /// <returns></returns>
        public bool AddLotteryParLog2(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog, Hashtable userHsah, bool isRegister, string systemState, bool isSpCode)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid1);
                }

                if (!userHsah.ContainsKey("F10"))
                {
                    userHsah.Add("F10", systemState);
                }

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryParLog2(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog, registersql, isSpCode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        #endregion

        #region 4.1) 添加参与记录和真码参与记录 德农专用 (+信息收集)
        /// <summary>
        /// 添加参与记录和真码参与记录(+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <param name="userHsah">收集信息集合</param>
        /// <returns></returns>
        public bool AddLotteryParLogDL(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog, Hashtable userHsah, bool isRegister, string systemState)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid1);
                }

                if (!userHsah.ContainsKey("F10"))
                {
                    userHsah.Add("F10", systemState);
                }

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryParLogDL(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 添加参与记录和真码参与记录和修改注册状态 德农专用(+信息收集)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="ismodifyAwardNum">是否更新奖池</param>
        /// <param name="isModifyParLog">是否添加参与记录</param>
        /// <param name="userHsah">收集信息集合</param>
        /// <param name="isRegister">是否注册</param>
        /// <param name="systemState">系统执行状态</param>
        /// <param name="isSpCode">是否是中性码</param>
        /// <returns></returns>
        public bool AddLotteryParLog3(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool ismodifyAwardNum, bool isModifyParLog, Hashtable userHsah, bool isRegister, string systemState, bool isSpCode)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string guid1 = getGuid();
                string guid2 = getGuid();

                #region 参数组织
                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[ismodifyAwardNum:" + ismodifyAwardNum + "]");
                #endregion

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid1);
                }

                if (!userHsah.ContainsKey("F10"))
                {
                    userHsah.Add("F10", systemState);
                }

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryParLog2(guid1, guid2, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, ismodifyAwardNum, isModifyParLog, registersql, isSpCode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog2:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        #endregion

        #region 5) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,短信预警

        /// <summary>
        /// 添加参与,真码参与,中奖记录,短信预警
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="isAddRegister">是否添加预注册信息</param>
        /// <returns>是否成功</returns>
        public bool AddLottery(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, bool isAddRegister)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                #endregion

                return ldao.AddLottery(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, isAddRegister);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryParLog:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 5.1) 添加参与,真码参与,中奖记录,信息收集,已中奖人数,短信预警(实物开奖+信息收集)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,短信预警,信息收集
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="registersql">信息收集执行sql</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryAndRegister(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string newpooid)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                #endregion

                userHsah.Add("GUID", guid1);
                userHsah.Add("IP", ip);
                userHsah.Add("CATEGORY", channel);
                userHsah.Add("LOTTERYLEVEL", awardsno);

                string registersql = string.Empty;

                if (!InsertLotteryAndRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryAndRegister(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, newpooid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryAndRegister:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 5.2) 添加参与,真码参与,中奖记录,信息收集,已中奖人数,短信预警(实物开奖+信息收集)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,短信预警,信息收集
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="registersql">信息收集执行sql</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryAndRegister(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                #endregion

                userHsah.Add("GUID", guid1);
                userHsah.Add("IP", ip);
                userHsah.Add("CATEGORY", channel);
                userHsah.Add("LOTTERYLEVEL", awardsno);

                string registersql = string.Empty;

                if (!InsertLotteryAndRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryAndRegister(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryAndRegister:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 6) 参与活动次数加1
        /// <summary>
        /// 参与活动次数加1
        /// </summary>
        /// <param name="activityid">活动编号</param>
        /// <param name="poolid">奖池编号</param>
        /// <returns></returns>
        public bool ModifyAwardNum(string facid, string activityid, string poolid)
        {
            try
            {
                return ldao.UpdateAwardNum(facid, activityid, poolid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:ModifyAwardNum:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 7) 更新中奖数量
        /// <summary>
        /// 更新中奖数量
        /// </summary>
        /// <param name="facid">厂加编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="awardscode">奖项ID</param>
        /// <returns></returns>
        public bool UpdateLotteryNum(string facid, string poolid, string awardscode)
        {
            try
            {
                return ldao.UpdateLotteryNum(facid, poolid, awardscode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:UpdateLotteryNum:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 8) 发送中奖预警信息
        /// <summary>
        /// 发送中奖预警信息
        /// </summary>
        /// <param name="level">数据字典ID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="content">短信预警内容</param>
        /// <returns></returns>
        public bool sendPrewarning(int level, string facid, string content)
        {
            try
            {
                return ldao.sendPrewarning(level, facid, content);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:sendPrewarning:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 9) 检查数码是否被注册过
        /// <summary>
        /// 检查数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="code">二次加密后的数码</param>
        /// <returns>true:被注册，false:未被注册</returns>
        public bool isRegisterByCode(string facid, string digitcode, string code)
        {
            bool bRet = false;
            try
            {
                object dbcode = ldao.IsRegisterFlagByCode(facid, digitcode, code);
                if (dbcode != null)
                {
                    if (!string.IsNullOrEmpty(dbcode.ToString()))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:isRegisterByCode:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 针对壳牌喜力特殊验证


        #region 查询手机号码当天参与此活动的情况
        /// <summary>
        ///  查询手机号码当天参与此活动的情况
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool CheckLotteryCodeByMobileDay(string facid, string mobile, int num)
        {

            Logger.AppLog.Write("BLLLottery:CheckLotteryCodeByMobileDay [facid:" + facid + "] [mobile:" + mobile + "] [num:" + num + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            bool bRet = false;
            try
            {
                object obj = ldao.CheckLotteryCodeByMobileDay(facid, mobile);

                if (obj != null)
                {
                    bRet = Convert.ToInt32(obj) <= num;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:CheckLotteryCodeByMobileDay:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        public bool CheckVerifyCodeByMobile(string facid, string mobile, string verifyCode)
        {
            Logger.AppLog.Write("BLLLottery:CheckVerifyCodeByMobile [facid:" + facid + "] [mobile:" + mobile + "] [verifyCode:" + verifyCode + "]  ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            bool bRet = false;
            try
            {
                object obj = ldao.CheckVerifyCodeByMobile(mobile, verifyCode, facid);

                if (obj != null)
                {
                    if (obj.ToString().Equals("1"))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:CheckVerifyCodeByMobile:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }



        #region 检验手机验证码是否存在
        /// <summary>
        /// 检验手机验证码是否存在
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        public bool CheckVerifyCodeIsExsit(string facid, string mobile, string verifyCode)
        {
            Logger.AppLog.Write("BLLLottery:CheckVerifyCodeIsExsit [facid:" + facid + "] [mobile:" + mobile + "] [verifyCode:" + verifyCode + "]  ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            bool bRet = false;
            try
            {
                object obj = ldao.CheckVerifyCodeIsExsit(facid, mobile, verifyCode);

                if (obj != null)
                {
                    if (int.Parse(obj.ToString()) > 0)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:CheckVerifyCodeIsExsit:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        #endregion






        /// <summary>
        /// 检查每日最大中奖次数是否达到上限
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="maxLotteryNum"></param>
        /// <param name="actid"></param>
        /// <returns></returns>
        public bool CheckSumMaxLotteryDay(string facid, string maxLotteryNum, string actid)
        {
            Logger.AppLog.Write("BLLLottery:CheckSumMaxLotteryDay [facid:" + facid + "] [maxLotteryNum:" + maxLotteryNum + "] [actid:" + actid + "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            bool bRet = false;
            try
            {
                object obj = ldao.CheckSumMaxLotteryDay(facid, actid);

                if (obj != null && !string.IsNullOrEmpty(maxLotteryNum))
                {
                    int lotterynum = Convert.ToInt32(obj);

                    if (Convert.ToInt32(maxLotteryNum) - lotterynum >= 1)
                    {
                        bRet = true;
                    }
                }
                else
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:CheckSumMaxLotteryDay:" + facid + "---" + maxLotteryNum + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="stroeid"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public bool CheckOpenidStore(string openid, string stroeid, string facid, int num)
        {
            Logger.AppLog.Write("BLLLottery:CheckOpenidStore [facid:" + facid + "] [stroeid:" + stroeid + "] [openid:" + openid + "] [num:" + num + "]  ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
            bool bRet = false;
            try
            {
                object obj = ldao.CheckOpenidStore(openid, stroeid, facid);

                if (obj != null)
                {
                    bRet = Convert.ToInt32(obj) > num;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:CheckOpenidStore:" + facid + "---" + stroeid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="verifyCode"></param>
        /// <param name="facid"></param>
        /// <returns></returns>
        public bool ModifyVerifyCodeByMobile(string mobile, string verifyCode, string facid)
        {
            Logger.AppLog.Write("BLLLottery:ModifyVerifyCodeByMobile [facid:" + facid + "] [mobile:" + mobile + "]  [verifyCode:" + verifyCode + "] ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            bool bRet = false;
            try
            {
                return ldao.ModifyVerifyCodeByMobile(mobile, verifyCode, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("BLLLottery:ModifyVerifyCodeByMobile:" + facid + "---" + mobile + "---" + verifyCode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }
        #endregion

        #region 9.1) 检查数码是否被注册过
        /// <summary>
        /// 检查数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="code">二次加密后的数码</param>
        /// <returns>true:被注册，false:未被注册</returns>
        public bool isRegisterByCode2(string facid, string digitcode, string code)
        {
            bool bRet = false;
            try
            {
                //object dbcode = ldao.IsRegisterFlagByCode2(facid, digitcode, code);

                RegisterUser userinfo = ldao.GetLotteryUserInfo(digitcode, facid);

                if (userinfo != null)
                {
                    if (!string.IsNullOrEmpty(userinfo.USER_NAME))
                    {
                        bRet = true;
                    }
                }

                //if (dbcode != null)
                //{
                //    if (!string.IsNullOrEmpty(dbcode.ToString()))
                //    {
                //        bRet = true;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:isRegisterByCode2:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 9.2) 检查数码是否被注册过
        /// <summary>
        /// 检查数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="code">二次加密后的数码</param>
        /// <returns>true:被注册，false:未被注册</returns>
        public bool isRegisterByCode3(string facid, string digitcode, string code)
        {
            bool bRet = false;
            try
            {
                object dbcode = ldao.IsRegisterFlagByCode3(facid, digitcode, code);
                if (dbcode != null)
                {
                    if (!string.IsNullOrEmpty(dbcode.ToString()))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:isRegisterByCode:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 9.3) 根据OPENID和数码检查数码中奖信息是否已经注册
        /// <summary>
        /// 检查数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="openid">openid</param>
        /// <returns>true:被注册，false:未被注册</returns>
        public bool isRegisterByCode4(string facid, string digitcode, string openid)
        {
            bool bRet = false;
            try
            {
                object dbcode = ldao.IsRegisterFlagByCode4(facid, digitcode, openid);
                if (dbcode != null)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:isRegisterByCode4:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 9.3) 根据OPENID和数码检查数码中奖信息是否已经注册
        /// <summary>
        /// 检查数码是否被注册过
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="openid">openid</param>
        /// <returns>true:被注册，false:未被注册</returns>
        public bool isRegisterByGroupID(string facid, string groupid, string openid)
        {
            bool bRet = false;
            try
            {
                object dbcode = ldao.IsRegisterFlagByCode4(facid, groupid, openid);
                if (dbcode != null)
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("ControlBLL:isRegisterByCode4:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
        }
        #endregion

        #region 10) 中奖用户正式注册
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="username">姓名</param>
        /// <param name="useraddress">地址</param>
        /// <param name="userzip">邮编</param>
        /// <param name="usermobile">手机号码</param>
        /// <param name="usercompany">公司名称</param>
        /// <param name="gift">选择礼品</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public bool ModifyRegisterUserByCode(string factoryid, string digitcode, string username, string useraddress, string userzip, string usermobile, string usercompany, string gift, string channel)
        {
            try
            {
                RegisterUser userinfo = new RegisterUser();

                userinfo.CHANGE_WAY = channel;
                userinfo.CHANGE_TYPE = channel;
                userinfo.USER_NAME = username;
                userinfo.USER_ADDRESS = useraddress;
                userinfo.USER_ZIPCODE = userzip;
                userinfo.USER_TELEPHONE = usermobile;
                userinfo.CHANGE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                userinfo.Lotteryname = gift;
                userinfo.Company = usercompany;
                userinfo.SPRO = digitcode;
                userinfo.Facid = factoryid;

                int row = ldao.ModifyRegisterUserByCode(userinfo, factoryid);
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:ModifyRegisterUserByCode:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11) 中奖用户正式注册(支持动态字段数据)
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public bool ModifyRegisterUserByCode(string factoryid, string digitcode, Hashtable userHash, string channel)
        {
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = ldao.GetTable(factoryid);

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

                #region 构造注册信息sql语句
                StringBuilder strFWSQL = new StringBuilder("UPDATE " + RegisterTalbe + " Q SET ");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 构造注册查询语句
                        //判断注册信息项是否包含在表中
                        if (colnumList.Contains(userinfo.Key.ToString().ToUpper()))
                        {
                            if (userinfo.Key.ToString().Contains("DATE"))
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = TO_DATE('" + userinfo.Value.ToString() + "','yyyy-mm-dd hh24:mi:ss'),");
                            }
                            else
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = '" + userinfo.Value.ToString() + "',");
                            }
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                strFWSQL.Append(" Q.CHANGE_DATE  = TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss'),");
                strFWSQL.Append(" Q.STATE  = '1'");
                strFWSQL.Append(" WHERE Q.FACID  = '" + factoryid + "'");
                strFWSQL.Append(" AND Q.SPRO = '" + digitcode + "'");
                #endregion

                //如表不存在则直接返回注册失败
                if (sqlfalg)
                {
                    return false;
                }

                int row = ldao.ModifyRegisterUserByCode(strFWSQL.ToString());
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:ModifyRegisterUserByCode:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11.1) 中奖用户正式注册(支持动态字段数据)
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public bool InsertRegisterUserByCode(string factoryid, string digitcode, Hashtable userHash, string channel)
        {
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = ldao.GetTable(factoryid);

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InsertRegisterUserByCode:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11.2) 中奖用户正式注册 - 马石油
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public bool ModifyRegisterUserByCodeSendMail(string factoryid, string digitcode, Hashtable userHash, string channel, string sendMailTemplate)
        {
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = ldao.GetTable(factoryid);

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

                string ecode = string.Empty;
                string epassword = string.Empty;
                string emial = string.Empty;

                #region 构造注册信息sql语句
                StringBuilder strFWSQL = new StringBuilder("UPDATE " + RegisterTalbe + " Q SET ");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 构造注册查询语句
                        //判断注册信息项是否包含在表中
                        if (colnumList.Contains(userinfo.Key.ToString().ToUpper()))
                        {
                            if (userinfo.Key.ToString().Contains("DATE"))
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = TO_DATE('" + userinfo.Value.ToString() + "','yyyy-mm-dd hh24:mi:ss'),");
                            }
                            else if (userinfo.Key.ToString().Contains("USER_EMAIL"))
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = '" + userinfo.Value.ToString() + "',");


                                //获取京东E卡数据,组织修改京东E卡卡密数据
                                if (ldao.GetNewJDECode(factoryid, 1, out ecode, out epassword))
                                {
                                    sendMailTemplate = sendMailTemplate.Replace("[卡号]", ecode);
                                    sendMailTemplate = sendMailTemplate.Replace("[卡密]", epassword);
                                }

                                //获取具体邮箱
                                emial = userinfo.Value.ToString();

                                //京东E卡卡密保持字段
                                strFWSQL.Append("F1 = '" + ecode + "',");
                            }
                            else
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = '" + userinfo.Value.ToString() + "',");
                            }
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                strFWSQL.Append(" Q.CHANGE_DATE  = TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss'),");
                strFWSQL.Append(" Q.STATE  = '1'");
                strFWSQL.Append(" WHERE Q.FACID  = '" + factoryid + "'");
                strFWSQL.Append(" AND Q.SPRO = '" + digitcode + "' AND Q.STATE  = '9'");
                #endregion


                //Logger.AppLog.Write("LotteryBLL:ModifyRegisterUserByCodeSendMail: [factoryid :" + factoryid + "] [digitcode:" + digitcode + "] [" + strFWSQL.ToString()+ "]", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);

                //如表不存在则直接返回注册失败
                if (sqlfalg)
                {
                    return false;
                }

                return ldao.ModifyRegisterUserByCode(factoryid, strFWSQL.ToString(), digitcode, ecode, emial, sendMailTemplate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:ModifyRegisterUserByCodeSendMail:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11) 中奖用户正式注册(支持动态字段数据) -- openid+数码+厂家编号为修改条件
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="openid">openid</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="channel">通道类型</param>
        /// <returns></returns>
        public bool ModifyRegisterUserByCodeAndOpenid(string factoryid, string digitcode, string openid, Hashtable userHash)
        {
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = ldao.GetTable(factoryid);

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

                #region 构造注册信息sql语句
                StringBuilder strFWSQL = new StringBuilder("UPDATE " + RegisterTalbe + " Q SET ");
                foreach (DictionaryEntry userinfo in userHash)
                {
                    if (colnumList != null && colnumList.Count > 0)
                    {
                        #region 构造注册查询语句
                        //判断注册信息项是否包含在表中
                        if (colnumList.Contains(userinfo.Key.ToString().ToUpper()))
                        {
                            if (userinfo.Key.ToString().Contains("DATE"))
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = TO_DATE('" + userinfo.Value.ToString() + "','yyyy-mm-dd hh24:mi:ss'),");
                            }
                            else
                            {
                                strFWSQL.Append(userinfo.Key.ToString().ToUpper() + "  = '" + userinfo.Value.ToString() + "',");
                            }
                        }
                        #endregion
                    }
                    else //如表不存在或者字段不存在跳出循环
                    {
                        sqlfalg = true;
                        break;
                    }
                }
                strFWSQL.Append(" Q.CHANGE_DATE  = TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss'),");
                strFWSQL.Append(" Q.STATE  = '1'");
                strFWSQL.Append(" WHERE Q.FACID  = '" + factoryid + "'");
                strFWSQL.Append(" AND Q.OPENID = '" + openid + "'");
                strFWSQL.Append(" AND Q.SPRO = '" + digitcode + "'  AND Q.STATE  = '9'");
                #endregion

                //如表不存在则直接返回注册失败
                if (sqlfalg)
                {
                    return false;
                }
                Logger.AppLog.Write("LotteryBLL:ModifyRegisterUserByCode:strFWSQL:" + strFWSQL.ToString(), KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);
                int row = ldao.ModifyRegisterUserByCode(strFWSQL.ToString());
                return (row > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:ModifyRegisterUserByCode:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11.2) 话费充值+信息收集
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertRechargeRegister(string factoryid, string digitcode, Hashtable userHash, out  string sql)
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
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                // int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                //return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InsertRechargeRegister:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11.3) 实物开奖+信息收集
        /// <summary>
        ///  实物开奖+信息收集
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertLotteryAndRegister(string factoryid, string digitcode, Hashtable userHash, out  string sql)
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
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                // int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                //return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InsertRechargeRegister:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11.2) 话费充值+信息收集
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertRechargeRegisterR5E(string factoryid, string digitcode, Hashtable userHash, out  string sql)
        {
            sql = "";
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = "T_SGM_WB_SHAKE_REGISTER_9999";

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                // int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                //return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InsertRechargeRegister:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion


        #region 11.2) 话费充值+信息收集
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
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
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                // int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                //return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InsertRechargeRegister:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 11.3) 实物，虚拟卡券抽奖+信息收集
        /// <summary>
        /// 中奖用户正式注册
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="digitcode">数码</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertRechargeRegisterXLCF(string factoryid, string digitcode, Hashtable userHash, out  string sql)
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
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                // int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                //return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL:InsertRechargeRegisterXLCF:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion


        #region 11.4) 基础用户注册
        /// <summary>
        /// 基础用户注册t_sgm_user_XXXX
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="userHash">注册内容</param>
        /// <param name="sql">输出:需要执行信息收集的sql语句</param>
        /// <returns></returns>
        public bool InsertUserRegister(string factoryid, Hashtable userHash, out  string sql)
        {
            sql = "";
            try
            {
                string newsql = string.Empty;
                bool sqlfalg = false;

                //获取该注册信息工厂所在表
                string RegisterTalbe = "T_SGM_USER_9999";

                //获取注册表结果
                List<String> colnumList = null;
                colnumList = Ddao.GetTableColmunList(factoryid, RegisterTalbe);

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

                // int row = ldao.InsertRegisterUserByCode(strFWSQL.ToString());
                //return (row > 0);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InsertRechargeRegister:" + factoryid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion


        #region 12) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,短信预警（杜邦）
        /// <summary>
        /// 添加参与,真码参与,中奖记录,短信预警（杜邦）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="isAddRegister">是否添加预注册信息</param>
        /// <returns>是否成功</returns>
        public bool AddLotteryDuPont(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int lotteryNumber, int totalTimes, out string systemstate)
        {
            systemstate = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                #endregion

                return ldao.AddLotteryDuPont(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, lotteryNumber, totalTimes, out systemstate);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryDuPont:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 12.1) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动）
        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLottery(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, out string systemstate)
        {
            systemstate = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                return ldao.AddLotteryRecharge(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 12.2) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）
        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLottery(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, Hashtable userHsah, out string systemstate)
        {
            systemstate = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                userHsah.Add("GUID", guid1);

                string registersql = string.Empty;

                if (!InsertRechargeRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryRecharge(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money, registersql, false);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 12.2) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）
        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryAtlas(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, Hashtable userHsah, string ecode, out string systemstate)
        {
            systemstate = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                userHsah.Add("GUID", guid1);

                string registersql = string.Empty;

                if (!InsertRechargeRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryRechargeAtlas(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money, registersql, false, ecode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 12.2) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）  -- 话费充值
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryYD(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, Hashtable userHsah, string systemstate, bool isspcode, out string lid)
        {
            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryRechargeYD(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money, registersql, isspcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）  -- 话费充值
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <param name="systemstate">输出:系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddEcodeLotteryR5E(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, Hashtable userHsah, string systemState, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                lid = guid1;
                userHsah.Add("GUID", guid1);
                userHsah.Add("F10", systemState);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryEcode(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        #endregion


        #region 流量充值+话费充值+实物礼品+虚拟券

        #region 12.1) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）  --实物礼品
        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）--实物礼品
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotterySW(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, Hashtable userHsah, string systemstate, bool isspcode, out string lid)
        {
            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryRechargeSW(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, registersql, isspcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 12.2) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）  --话费充值
        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对话费充值类抽奖活动+信息收集录入）--话费充值
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryHH(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, Hashtable userHsah, string systemstate, bool isspcode, out string lid)
        {
            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryRechargeHH(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money, registersql, isspcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 12.3) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对流量类抽奖活动+信息收集录入）--流量充值

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对流量类抽奖活动+信息收集录入）  -- 流量充值
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="packCode">流量包订购代码</param>
        /// <param name="packNum">流量包大小</param>
        /// <param name="packMoney">流量包大小</param>
        /// <param name="activityid">监控平台分配的活动ID</param>
        /// <param name="token">数据加密Token</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryLL(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, Hashtable userHsah, string systemstate, bool isspcode, string packCode, string packNum, string packMoney, string activityid, string token, out string lid)
        {
            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();
                string guid4 = "AAAAAA" + getGuid();
                string guid5 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[systemstate:" + systemstate + "]");
                sb.Append("[isspcode:" + isspcode.ToString() + "]");
                sb.Append("[packCode:" + packCode.ToString() + "]");
                sb.Append("[packNum:" + packNum.ToString() + "]");
                sb.Append("[packMoney:" + packMoney.ToString() + "]");
                sb.Append("[activityid:" + activityid.ToString() + "]");
                sb.Append("[token:" + token.ToString() + "]");
                #endregion


                /// 
                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }
                //Utility.ValueSort(orderid, packcode, mobile, createdate).MD5();
                string sign = Utility.ValueSort(guid4, packCode, ip, token).MD5();
                sb.Append("[sign:" + sign.ToString() + "]");

                return ldao.AddLotteryRechargeLL(guid1, guid2, guid3, guid4, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, registersql, isspcode, packCode, packNum, packMoney, activityid, sign);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 12.4 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对虚拟卡类抽奖活动+信息收集录入）--虚拟卡充值 未正式上线使用
        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,充值明细（针对虚拟卡类抽奖活动+信息收集录入）  -- 虚拟卡充值 未正式上线使用
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <param name="systemstate">输出:系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddEcodeLotteryR5EEcode(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, int money, Hashtable userHsah, string systemState, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                lid = guid1;
                userHsah.Add("GUID", guid1);
                userHsah.Add("F10", systemState);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterR5E(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryEcode(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #endregion

        //---------  魏战朋新增开始

        #region 12.3) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（针对 实物派送，京东虚拟卡券抽奖活动+信息收集录入）   (壳牌喜力超凡项目)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（针对 实物派送，京东虚拟卡券抽奖活动+信息收集录入）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="ecode">京东E卡卡号</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryXLCF(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string systemstate, bool isspcode, string ecode, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[ecode:" + ecode + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterXLCF(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }


                return ldao.AddLotteryRechargeXLCF(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, isspcode, ecode);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region   12.4 获取京东卡券  壳牌喜力超凡
        //获取京东卡券
        /// <summary>
        ///   获取京东卡券
        /// </summary>
        /// <param name="facid"></param>
        /// <returns></returns>
        public DataTable GetJDKQ(string facid)
        {
            return ldao.GetJDKQ(facid);
        }
        #endregion

        #region   12.5 获取京东卡券  壳牌喜力超凡  (采用)
        /// <summary>
        /// 获取一个未使用的新京东E卡卡密  
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="number"></param>
        /// <param name="ecode"></param>
        /// <param name="epassword"></param>
        /// <returns></returns>
        public bool GetNewJDECode(string facid, int number, out string ecode, out string epassword)
        {
            return ldao.GetNewJDECode(facid, number, out  ecode, out  epassword);
        }

        #endregion


        #region   12.5 获取一号店卡券  (阿特拉斯采用)
        /// <summary>
        /// 获取一号店卡券  (阿特拉斯采用)
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="number"></param>
        /// <param name="ecode"></param>
        /// <param name="epassword"></param>
        /// <returns></returns>
        public bool GetNewYHDECodeAtlas(string facid, int number, string level, out string ecode, out string epassword)
        {
            return ldao.GetNewJDECodeAtlas(facid, number, level, out  ecode, out  epassword);
        }

        #endregion

        //---------  魏战朋新增结束

        #region 12.3) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（针对 实物派送，京东虚拟卡券抽奖活动+信息收集录入）   (壳牌加油站)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（针对 实物派送，京东虚拟卡券抽奖活动+信息收集录入）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="ecode">京东E卡卡号</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryShellStation(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string systemstate, bool isspcode, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");

                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterXLCF(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }


                return ldao.AddLotteryRechargeShellStation(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, isspcode);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion



        #region 12.5) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（话费充值+信息收集录入）   (壳牌-WT项目)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（话费充值+信息收集录入）  
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="mobilebill">话费充值金额</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryWT(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string systemstate, bool isspcode, string mobilebill, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                // sb.Append("[ecode:" + ecode + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterXLCF(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }


                return ldao.AddLotteryRechargeWT(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, isspcode, mobilebill);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLotteryWT:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 12.6) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（话费充值+信息收集录入）   (壳牌-通行证项目)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（话费充值+信息收集录入）   (壳牌-通行证项目)
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="userguid">用户注册表guid</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="mobilebill">话费充值金额</param>
        /// <param name="miletype">奖励里程类别</param>
        /// <param name="mile">奖励里程</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryTXZ(string facid, string ip, string userguid, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string systemstate, bool isspcode, string mobilebill, string miletype, int mile, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                // sb.Append("[ecode:" + ecode + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                string registersql = string.Empty;

                if (!InsertRechargeRegisterXLCF(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }


                return ldao.AddLotteryRechargeTXZ(guid1, guid2, guid3, userguid, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, isspcode, mobilebill, miletype, mile);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLotteryTXZ:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion





        #region 13)  查看检查手机号码是否已经被注册过
        /// <summary>
        /// 检查手机号码是否已经被注册
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool CheckLotteryRegisterByMobile(string mobile, string facid, string tableName)
        {
            return ldao.GetLotteryRegisterByMobile(mobile, facid, tableName);
        }
        #endregion

        #region 14) 依据配置的注册信息字段进行注册
        public bool AddLotteryRegisterByDBConfig(string channel, string ip, string awardsno, string facid, string[] dbconfigfieldlist, string[] smsmsglist, string tablename)
        {
            string code = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            string newcode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            int iRet = ldao.AddLotteryRegisterByDBConfig(code, channel, ip, awardsno, newcode, facid, dbconfigfieldlist, smsmsglist, tablename);
            return iRet > 0 ? true : false;
        }
        #endregion

        #region 15)  查看检查数码是否已经被注册过
        /// <summary>
        /// 查看检查数码是否已经被注册过
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByCode(string digigcode, string facid)
        {
            return ldao.GetLotteryCodeByCode(digigcode, facid);
        }
        #endregion

        #region 16)  查看检查数码是否被发送手机被注册过
        /// <summary>
        /// 查看检查数码是否被发送手机被注册过
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="digigcode">数码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckLotteryCodeByCodeAndMobile(string mobile, string digigcode, string facid)
        {
            return ldao.GetLotteryCodeByCodeAndMobile(mobile, digigcode, facid);
        }
        #endregion

        #region 17) 查询手机号码是否进行过防伪查询
        /// <summary>
        /// 查询手机号码是否进行过防伪查询
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool GetLotteryCodeByMobile(string mobile, string facid)
        {
            return ldao.GetLotteryCodeByMobile(mobile, facid);
        }
        #endregion

        #region 18) 添加参与,真码参与,中奖记录,已中奖人数（针对红包抽奖活动）
        /// <summary>
        /// 添加参与,真码参与,中奖记录,已中奖人数（针对红包抽奖活动）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRedEnvelopeLottery(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, string money, out string systemstate)
        {
            systemstate = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                return ldao.AddRedEnvelopeLottery(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, money);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion


        #region 18) 添加参与,真码参与,中奖记录,已中奖人数（针对红包抽奖活动 + 信息收集）
        /// <summary>
        /// 添加参与,真码参与,中奖记录,已中奖人数（针对红包抽奖活动 + 信息收集）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="money">中奖充值金额</param>
        /// <param name="systemstate">系统状态</param>
        /// <returns>是否成功</returns>
        public bool AddRedEnvelopeLotteryRegister(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, string money, Hashtable userHsah, bool isspcode, out string systemstate)
        {
            systemstate = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[money:" + money.ToString() + "]");
                #endregion

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid1);
                }

                string registersql = string.Empty;

                if (!InsertRechargeRegister(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }

                return ldao.AddLotteryRedEnvelopeRegister(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, Convert.ToDouble(money), registersql, isspcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 19) 根据检测用户是否存在 (马石油)
        /// <summary>
        ///  根据检测用户是否存在
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="openid">微信OPENID</param>
        /// <param name="result">返回:答复</param>
        /// <param name="codeInfo">返回:此OPENID参与的抽奖记录明细</param>
        /// <param name="systemstate">返回:系统执行状态</param>
        /// <returns></returns>
        public bool CheckUserExist(string factoryid, string openid, out string result, out DataTable codeInfo, out  string systemstate)
        {
            systemstate = "000";
            result = "";
            codeInfo = null;
            try
            {
                codeInfo = ldao.CheckUserExist(factoryid, openid);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckUserExist:" + factoryid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 20) 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（针对 实物派送，虚拟卡券抽奖活动+信息收集录入）   (曼牌)

        /// <summary>
        /// 添加参与,真码参与,中奖记录,预注册信息,已中奖人数,（针对 实物派送，京东虚拟卡券抽奖活动+信息收集录入）
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="cardnum">奖品对应的卡券号</param>       
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryMannHummel(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string systemstate, bool isspcode, string cardnum, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            string carbrand = ""; //车型
            string cip = ""; //消费者ip

            string provinceName = "";//省份
            string cityName1 = "";//城市

            //追溯信息
            string productname = "";//产品名称
            string productid = "";//产品id
            string dealername = "";//经销商名称
            string dealerid = "";//经销商id
            string logisticscode = "";//物流码

            string openid = "";//微信用户关注号

            try
            {
                #region 参数组织
                string guid1 = getGuid();
                string guid2 = getGuid();
                string guid3 = getGuid();

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[cardnum:" + cardnum + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);
                if (userHsah.Contains("CIP"))
                {
                    cip = userHsah["CIP"].ToString();
                }

                if (userHsah.Contains("CARBRAND"))
                {
                    carbrand = userHsah["CARBRAND"].ToString();
                }
                if (userHsah.Contains("PROVINCENAME"))
                {
                    provinceName = userHsah["PROVINCENAME"].ToString();
                }
                if (userHsah.Contains("CITYNAME"))
                {
                    cityName1 = userHsah["CITYNAME"].ToString();
                }

                if (userHsah.Contains("PRODUCTID"))
                {
                    productid = userHsah["PRODUCTID"].ToString();
                }
                if (userHsah.Contains("PRODUCTNAME"))
                {
                    productname = userHsah["PRODUCTNAME"].ToString();
                }
                if (userHsah.Contains("DEALERNAME"))
                {
                    dealername = userHsah["DEALERNAME"].ToString();
                }
                if (userHsah.Contains("LOGISTICSCODE"))
                {
                    logisticscode = userHsah["LOGISTICSCODE"].ToString();
                }
                if (userHsah.Contains("DEALERID"))
                {
                    dealerid = userHsah["DEALERID"].ToString();
                }
                if (userHsah.Contains("OPENID"))
                {
                    openid = userHsah["OPENID"].ToString();
                }
                string registersql = string.Empty;

                if (!InsertRechargeRegisterXLCF(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }


                return ldao.AddLotteryRechargeMannHummel(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, isspcode, cardnum, carbrand, cip, provinceName, cityName1, dealername, logisticscode, dealerid, productname, productid, openid);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddRechargeLottery:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion


        #region 20.2) 添加参与,真码参与,中奖记录,注册信息,更新奖池信息,更新参与总人数，添加投保基础信息 壳牌太保

        /// <summary>
        /// 添加参与,真码参与,中奖记录,注册信息,更新奖池信息,更新参与总人数，添加投保基础信息
        /// </summary>
        /// <param name="facid">厂家编号</param>
        /// <param name="ip">IP地址/手机号码</param>
        /// <param name="digitCode">数码</param>
        /// <param name="channel">通道类型(W,M,S)</param>
        /// <param name="activityId">活动编号</param>
        /// <param name="poolid">奖池编码</param>
        /// <param name="area">参与地区</param>
        /// <param name="awardsno">奖项编号</param>
        /// <param name="proid">产品编号</param>
        /// <param name="result">答复信息</param>
        /// <param name="newcode">二次加密后的数码</param>
        /// <param name="smsflag">是否发送预警短信</param>
        /// <param name="smsMassage">短信发送内容</param>
        /// <param name="userHsah">注册信息项集合</param>
        /// <param name="systemstate">系统执行状态</param>
        /// <param name="isspcode">是否为特殊数码</param>
        /// <param name="orderid">投保订单号</param>    
        /// <param name="seriesid">系列ID</param>
        /// <param name="seriesname">系列名称</param>
        /// <param name="lid">输出:中奖纪录guid</param>
        /// <returns>是否成功</returns>
        public bool AddRechargeLotteryTB(string facid, string ip, string digitCode, string channel, string activityId, string poolid, string area, string awardsno, string proid, string result, string newcode, bool smsflag, string smsMassage, Hashtable userHsah, string systemstate, bool isspcode, string orderid, string seriesid, string seriesname, out string lid)
        {

            lid = "";
            StringBuilder sb = new StringBuilder();
            string productid = "";//产品id
            string productname = "";//产品名称
            string openid = "";//微信用户关注号
            try
            {
                #region 参数组织
                string guid1 = getGuid();//参与记录表guid
                string guid2 = getGuid();//参与真码记录表guid
                string guid3 = getGuid();//中奖guid

                sb.Append("[guid1:" + guid1 + "]");
                sb.Append("[guid2:" + guid2 + "]");
                sb.Append("[guid3:" + guid3 + "]");
                sb.Append("[ip:" + ip + "]");
                sb.Append("[facid:" + facid + "]");
                sb.Append("[digitCode:" + digitCode + "]");
                sb.Append("[channel:" + channel + "]");
                sb.Append("[activityId:" + activityId + "]");
                sb.Append("[awardsno:" + awardsno + "]");
                sb.Append("[proid:" + proid + "]");
                sb.Append("[result:" + result + "]");
                sb.Append("[area:" + area + "]");
                sb.Append("[poolid:" + result + "]");
                sb.Append("[newcode:" + newcode + "]");
                sb.Append("[smsflag:" + smsflag + "]");
                sb.Append("[smsMassage:" + smsMassage + "]");
                sb.Append("[orderid:" + orderid + "]");
                sb.Append("[seriesid:" + seriesid + "]");
                sb.Append("[seriesname:" + seriesname + "]");
                #endregion

                lid = guid3;
                userHsah.Add("GUID", guid3);
                userHsah.Add("F10", systemstate);

                if (userHsah.Contains("PRODUCTID"))
                {
                    productid = userHsah["PRODUCTID"].ToString();
                }
                if (userHsah.Contains("PRODUCTNAME"))
                {
                    productname = userHsah["PRODUCTNAME"].ToString();
                }
                if (userHsah.Contains("OPENID"))
                {
                    openid = userHsah["OPENID"].ToString();
                }
                string registersql = string.Empty;

                if (!InsertRechargeRegisterXLCF(facid, digitCode, userHsah, out registersql))
                {
                    registersql = "";
                }
                return ldao.AddLotteryRechargeTB(guid1, guid2, guid3, ip, facid, digitCode, channel, activityId, area, awardsno, proid, result, poolid, newcode, smsflag, smsMassage, registersql, isspcode, productid, productname, orderid, seriesid, seriesname, openid);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddLotteryRechargeTB:" + facid + "---" + sb.ToString() + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion


        #region 21) 根据检测用户是否存在 (曼牌)
        /// <summary>
        ///  根据检测用户是否存在
        /// </summary>
        /// <param name="factoryid">厂家编号</param>
        /// <param name="openid">微信OPENID</param>
        /// <param name="result">返回:答复</param>
        /// <param name="codeInfo">返回:此OPENID参与的抽奖记录明细</param>
        /// <param name="systemstate">返回:系统执行状态</param>
        /// <returns></returns>
        public bool CheckUserExistByOpenid(string factoryid, string openid, out string result, out DataTable codeInfo, out  string systemstate)
        {
            systemstate = "000";
            result = "";
            codeInfo = null;
            try
            {
                codeInfo = ldao.CheckUserExistByOpenid(factoryid, openid);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                {
                    systemstate = "001";
                    result = "该用户已注册";
                    return true;
                }
                else
                {
                    systemstate = "002";
                    result = "该用户还未注册";
                    return false;
                }

            }
            catch (Exception ex)
            {
                systemstate = "999";//程序异常
                Logger.AppLog.Write("LotteryBLL:CheckUserExistByOpenid:" + factoryid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion


        #region 22) 根据检测奖券号是否存在 (曼牌)
        /// <summary>
        ///  根据检测奖券号是否存在
        /// </summary>
        /// <param name="factoryid">厂家编号</param> 
        /// <param name="cardnum">生成的奖券号</param>
        /// <param name="codeInfo">返回:查询到的奖券</param>
        /// <param name="systemstate">返回:系统执行状态</param>      
        /// <returns></returns>
        public bool CheckCardId(string factoryid, string cardnum, out DataTable codeInfo, out  string systemstate)
        {
            systemstate = "000";
            codeInfo = null;
            try
            {
                codeInfo = ldao.CheckCardNumExist(factoryid, cardnum);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckCardId:" + factoryid + "---" + cardnum + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 23) 查询已中奖的核销信息 (曼牌)
        /// <summary>
        ///  根据factoryid、digitcode、mobile查询中奖核销信息
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="mobile">手机号</param> 
        /// <param name="codeInfo">返回：核销信息</param>        
        /// <returns></returns>
        public bool QueryConsumeInfo(string factoryid, string digitcode, out DataTable codeInfo)
        {

            codeInfo = null;
            try
            {
                codeInfo = ldao.QueryConsumeInfo(factoryid, digitcode);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:QueryConsumeInfo:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
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
        /// <param name="result">返回:答复</param>
        /// <param name="codeInfo">返回:此OPENID参与的抽奖核销信息</param>
        /// <param name="systemstate">返回:系统执行状态</param>
        /// <returns></returns>
        public bool GetConsumerEwardMann(string factoryid, string openid, string cardid, out string result, out DataTable codeInfo, out  string systemstate)
        {
            systemstate = "000";
            result = "";
            codeInfo = null;
            try
            {
                codeInfo = ldao.GetConsumerEwardMann(factoryid, openid, cardid);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                {
                    systemstate = "001";
                    result = "获取核销信息成功";
                    return true;
                }
                else
                {
                    systemstate = "002";
                    result = "未找到核销信息";
                    return false;
                }

            }
            catch (Exception ex)
            {
                systemstate = "000";//程序异常
                Logger.AppLog.Write("LotteryBLL:GetConsumerEwardMann:" + factoryid + "---" + openid + "---" + cardid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 24获取中奖数码信息 （通用版本）
        /// <summary>
        ///  根据factoryid、digitcode、mobile查询中奖核销信息
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="mobile">手机号</param> 
        /// <param name="codeInfo">中奖数码信息</param>        
        /// <returns></returns>
        public bool GetLotteryCodeInfo(string factoryid, string digitcode, out DataTable codeInfo)
        {
            codeInfo = null;
            try
            {
                codeInfo = ldao.GetLotteryCodeInfo(factoryid, digitcode);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:GetLotteryCodeInfo:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        ///  根据factoryid、digitcode、mobile查询中奖核销信息
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="mobile">手机号</param> 
        /// <param name="codeInfo">中奖数码信息</param>        
        /// <returns></returns>
        public bool GetLotteryCodeInfoNew(string factoryid, string digitcode, out DataTable codeInfo)
        {
            codeInfo = null;
            try
            {
                codeInfo = ldao.GetLotteryCodeInfoNew(factoryid, digitcode);

                if (codeInfo != null && codeInfo.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:GetLotteryCodeInfoNew:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion


        #region 查询真码首次参与的时间（曼牌）
        /// <summary>
        ///  查询真码首次参与的时间
        /// </summary>
        /// <param name="factoryid">厂家id</param>
        /// <param name="digitcode">防伪码</param>
        /// <param name="digitcode">返回：查询到的真码首次参与时间</param>
        /// <returns></returns>
        public bool CheckDigitCodeFirstQueryDateTime(string factoryid, string digitcode, out DataTable DateInfo)
        {
            DateInfo = null;
            try
            {
                DateInfo = ldao.CheckDigitCodeFirstQueryDateTime(factoryid, digitcode);

                if (DateInfo != null && DateInfo.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckDigitCodeFirstQueryDateTime:" + factoryid + "---" + digitcode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region 检查手机号码是否参与过活动

        /// <summary>
        /// 检查手机号码是否参与过活动
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckHaveActExist(string mobile, string facid)
        {
            try
            {
                return ldao.CheckLotteryCodeByMobile(mobile, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckHaveActExist:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }



        /// <summary>
        /// 检查手机号码是否参与过活动
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckRecentHasRecordByMobile(string mobile, string facid)
        {
            try
            {
                return ldao.CheckRecentHasRecordByMobile(mobile, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckHaveActExist:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 邀请好友
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lid"></param>
        /// <param name="systemstate"></param>
        /// <returns></returns>
        public bool InviteFriendJoin(string factoryid, string from, string to, string lid, string code, out string systemcode)
        {
            systemcode = "";
            try
            {
                return ldao.InviteFriendJoin(factoryid, from, to, lid, code, out systemcode);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InviteFriendJoin:" + factoryid + "---" + from + "----" + to + "------" + lid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #region 邀请好友
        /// <summary>
        /// 邀请好友
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool InviteFriendJoin(string factoryid, string from, string to, string lid, string code)
        {

            try
            {
                return ldao.InviteFriendJoin(factoryid, from, to, lid, code);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:InviteFriendJoin:" + factoryid + "---" + from + "----" + to + "------" + lid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion


        #region 检测数码是否已经邀请过好友

        /// <summary>
        /// 检测数码是否已经邀请过好友
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckCodeInvitedFriend(string factoryid, string from, string lid)
        {
            try
            {
                return ldao.CheckCodeInvitedFriend(factoryid, from, lid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:检测数码是否已经邀请过好友CheckCodeInvitedFriend() 异常【factoryid:" + factoryid + "】【from:" + from + "】【lid:" + lid + "】" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 检查OPENID是否参与过活动
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckOPENIDHaveActExist(string OPENID, string facid)
        {
            try
            {
                return ldao.CheckLotteryCodeByOPENID(OPENID, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckOPENIDHaveActExist:" + facid + "---" + OPENID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        /// <summary>
        /// 检查OPENID是否参与过活动 （重载）
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动ID</param>
        /// <returns></returns>
        public bool CheckOPENIDHaveActExist(string OPENID, string facid, string activityid)
        {
            try
            {
                return ldao.CheckLotteryCodeByOPENID(OPENID, facid, activityid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckOPENIDHaveActExist:" + facid + "---" + OPENID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }



        /// <summary>
        /// 检查OPENID是否参与过活动 
        /// </summary>
        /// <param name="OPENID">OPENID</param>
        /// <param name="facid">厂家编号</param>
        /// <param name="activityid">活动ID</param>
        /// <returns></returns>
        public bool CheckOPENIDHaveActExist2(string OPENID, string facid, string activityid)
        {
            try
            {
                return ldao.CheckLotteryCodeByOPENID2(OPENID, facid, activityid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckOPENIDHaveActExist2:" + facid + "---" + OPENID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }


        /// <summary>
        /// 检查用户是否通过微信授权过
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="facid"></param>
        /// <param name="validTime"></param>
        /// <returns></returns>
        public bool CheckOpenidIsAuth(string openid, string facid, string validTime)
        {
            Logger.AppLog.Write("BLLLottery:CheckOpenidIsAuth [facid:" + facid + "] [mobile:" + openid + "] ", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Info);

            bool bRet = false;

            try
            {
                object obj = ldao.CheckOpenidIsAuth(openid, facid, validTime);
                if (obj != null)
                {
                    bRet = Convert.ToInt32(obj) > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckOpenidIsAuth:" + facid + "---" + openid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
            return bRet;
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
            try
            {
                return ldao.CheckMobileIsJoinActivity(facid, mobile, activityid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckMobileIsJoinActivity:" + facid + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 检查门店是否存在

        /// <summary>
        /// 检查门店是否存在
        /// </summary>
        /// <param name="storeID">门店编号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckHaveStoreExist(string storeID, string facid)
        {
            try
            {
                return ldao.CheckLotteryCodeByStoreID(storeID, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckHaveStoreExist:" + facid + "---" + storeID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion

        #region 检查门店是否存在  喜力超凡

        /// <summary>
        /// 检查门店是否存在
        /// </summary>
        /// <param name="storeID">门店编号</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool CheckHaveStoreExistXLCF(string storeID, string facid)
        {
            try
            {
                return ldao.CheckLotteryCodeByStoreIDXLCF(storeID, facid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:CheckHaveStoreExistXLCF:" + facid + "---" + storeID + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// WT二期 查询门店中奖数量
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="facid"></param>
        /// <param name="lotterycount"></param>
        /// <returns></returns>
        public bool CheckHaveStoreLotteryCountWT(string storeID, string facid, out string lotterycount)
        {
            return ldao.CheckHaveStoreLotteryCountWT(storeID, facid, out lotterycount);
        }
        #endregion

        /// <summary>
        /// 获得当前微信用户本次活动获奖数据集合
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public DataTable GetLotteryListByOpenid(string facid, string openid)
        {

            try
            {
                return ldao.GetLotteryListByOpenid(facid, openid);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:GetLotteryListByOpenid:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        public DataTable SelectInviteRecord(string INVITELOTTERYGUID, string facid)
        {
            return ldao.SelectInviteRecord(INVITELOTTERYGUID, facid);
        }

        public string GetBaseDataValue(string facid, string datatypename)
        {
            return ldao.GetBaseDataValue(facid, datatypename);
        }

        #region 查询话费总额和话费当天总额
        public bool QueryLotteryTotalCount(string facid, string bTime, string eTime, out string LotterySum, out string curDaySum)
        {
            return ldao.QueryLotteryTotalCount(facid, bTime, eTime, out LotterySum, out curDaySum);
        }
        #endregion

        #region 查询话红包总额和当天总额
        public bool QueryRedTotalCount(string facid, string bTime, string eTime, out string RedSum, out string curDaySum)
        {
            return ldao.QueryRedTotalCount(facid, bTime, eTime, out RedSum, out curDaySum);
        }
        #endregion



        public int QueryCurrentDayCount(string facid, string awordno)
        {
            return ldao.QueryCurrentDayCount(facid, awordno);
        }



        public int QueryCurrentDayCountMobile(string facid, string mobile)
        {
            return ldao.SelectMobileJoinNum(facid, mobile);
        }



        public int QueryCurrentDayCountOpenid(string facid, string openid)
        {
            return ldao.SelectOpenidJoinNum(facid, openid);
        }


        #region 添加register表 德农专用
        public bool AddRegisterLog(string facid, string ip, string digitCode, string channel, string activityId, string area, string awardsno, string proid, string result, Hashtable userHsah, string systemState)
        {
            try
            {
                #region 参数组织
                string guid = getGuid();
                #endregion

                if (!userHsah.ContainsKey("GUID"))
                {
                    userHsah.Add("GUID", guid);
                }

                if (!userHsah.ContainsKey("F10"))
                {
                    userHsah.Add("F10", systemState);
                }


                if (!userHsah.ContainsKey("STATE"))
                {
                    userHsah.Add("STATE", "-1");
                }

                string registersql = string.Empty;

                if (!InsertRechargeRegister(facid, userHsah, out registersql))
                {
                    registersql = "";
                }
                return ldao.AddRegisterLog(registersql);

            }
            catch (Exception ex)
            {
                systemState = "000";
                Logger.AppLog.Write("LotteryBLL:AddLotteryLog:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        #endregion



        public DataTable SelectRegister(string facid, string where)
        {
            try
            {
                return ldao.SelectRegister(facid, where);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:SelectRegister:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }

        public string SelectRegister(string facid, string where, string field)
        {
            try
            {
                return ldao.SelectRegister(facid, where, field);

            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:SelectRegister:" + facid + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
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
            try
            {
                return ldao.AddCardInfoAndRegist(facid, userid, username, usermobile, cardid, userHash);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("LotteryBLL:AddCardInfoAndRegist:【facid：" + facid + "】【userid:" + userid + "】【username:" + username + "】【usermobile:" + usermobile + "】【cardid:" + cardid + "】【userHash:" + userHash + "】" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        #endregion

        #region  添加预约数据
        /// <summary>
        /// 添加预约数据
        /// </summary>
        /// <param name="factoryid"></param>
        /// <param name="mobile"></param>
        /// <param name="channel"></param>
        /// <param name="username"></param>
        /// <param name="storeid"></param>
        /// <param name="openid"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public bool AddReserve(string factoryid, string mobile, string channel, string username, string storeid, string openid, string f1)
        {
            bool bRet = false;
            try
            {
                return ldao.AddReserve(factoryid, mobile, channel, username, storeid, openid, f1);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.LotteryBLL  AddReserve[factoryid:" + factoryid + "] [mobile:" + mobile + "] [channel:" + channel + "] [username:" + username + "]  [storeid:" + storeid + " ] [openid:" + openid + "][f1:" + f1 + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
        }
        #endregion

        #region 添加用户注册
        /// <summary>
        /// 添加用户注册
        /// </summary>
        /// <param name="facid"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool UserRegist(string facid, Hashtable userHash)
        {
            bool bRet = false;
            try
            {
                userHash.Add("USERGUID", Guid.NewGuid().ToString().Replace("-", ""));
                string registersql = "";
                if (!InsertUserRegister(facid, userHash, out registersql))
                {
                    registersql = "";
                }
                return ldao.UserRegist(facid, registersql);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.LotteryBLL  UserRegist[facid:" + facid + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
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
        /// <param name="userHash">用户补全信息</param>
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
                return ldao.UpdateUserInfo(factoryid, userguid, userHash, firstcomplete, isreward, mile, isupdatebq);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.LotteryBLL  UpdateUserInfo[facid:" + factoryid + "] [userguid:" + userguid + "][userHash:" + userHash + "][firstcomplete:" + firstcomplete + "][isreward:" + isreward + "][mile:" + mile + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
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
                return ldao.IinviteFriend(facid, mobile, userguid, mile, inviyetype, mobilelists);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write(" KMSLotterySystemFront.BLLLottery.LotteryBLL  IinviteFriend[facid:" + facid + "] [mobile:" + mobile + "][userguid:" + userguid + "][mile:" + mile + "][inviyetype:" + inviyetype + "]----" + ex.Message, Logger.AppLog.LogMessageType.Fatal);
            }
            return bRet;
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
                return ldao.CheckIivited(facid, mobile, out dt);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  CheckIivited 参数[facid:" + facid + "][mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

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
                return ldao.CheckInviteByGroupid(facid, groupid);

            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  CheckInviteByGroupid 参数[facid:" + facid + "][groupid:" + groupid + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);

            }
            return bRet;
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
                bRet = ldao.AddUserMileNew(facid, inviteuserguid, rewardtype, code, lid, inviterewardmile, inviteguid);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  AddUserMileNew 参数[facid:" + facid + "][inviteuserguid:" + inviteuserguid + "][rewardtype:" + rewardtype + "][code:" + code + "][lid:" + lid + "][inviterewardmile:" + inviterewardmile + "][inviteguid:" + inviteguid + "][]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }
        #endregion

        #region 获取抽奖答复信息
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
                replay = ldao.GetShakeReplay(factoryid, replayid, channel);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  GetShakeReplay 参数[factoryid:" + factoryid + "][replayid:" + replayid + "][channel:" + channel + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
                replay = "";
            }
            return replay;

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
                bRet = ldao.AddMobileBill(facid, mobile, orderid, notes, citycode, cityname, statecode, statename, bill, cardid, resultid, lotteryguid, flag);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  AddMobileBill 参数[facid:" + facid + "][mobile:" + mobile + "][orderid:" + orderid + "][notes:" + notes + "][citycode:" + citycode + "][cityname:" + cityname + "][statecode:" + statecode + "][statename:" + statename + "] [bill:" + bill + "][cardid:" + cardid + "] [resultid:" + resultid + "][lotteryguid:" + lotteryguid + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
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
                bRet = ldao.UpdateInviteLotteryInfo(facid, inviteguid, lid);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  UpdateInviteLotteryInfo 参数[facid:" + facid + "][inviteguid:" + inviteguid + "][lid:" + lid + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }

        #endregion


        #region 注册t_sgm_user_9999表
        public bool InsertT_sgm_user(string facid, Hashtable userHsah)
        {
            bool bRet = false;
            try
            {
                bRet = ldao.InsertT_sgm_user(facid, userHsah);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  InsertT_sgm_user 参数[facid:" + facid + "][userHsah:" + userHsah + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }

        #endregion


        #region 查询t_sgm_user_9999表
        public DataTable SelectT_sgm_user(string facid, string mobile)
        {
            DataTable dtRet = null;
            try
            {
                dtRet = ldao.SelectT_sgm_user(facid, mobile);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  SelectT_sgm_user 参数[facid:" + facid + "][mobile:" + mobile + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return dtRet;
        }

        #endregion

        #region 修改t_sgm_user_9999表
        public bool UpdateT_sgm_user(string facid, Hashtable userHsah)
        {
            bool bRet = false;
            try
            {
                bRet = ldao.UpdateT_sgm_user(facid, userHsah);
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("KMSLotterySystemFront.BLLLottery.LotteryBLL  UpdateT_sgm_user 参数[facid:" + facid + "][userHsah:" + userHsah + "]----" + ex.Message + "--" + ex.TargetSite + "--" + ex.StackTrace + "--" + DateTime.Now.ToString() + "\n\r", KMSLotterySystemFront.Logger.AppLog.LogMessageType.Error);
            }
            return bRet;
        }
        #endregion

        public bool UpdateSgmRegister_kms(string facid, string digit, string mobile, string lid, Hashtable userHash)
        {
            return ldao.UpdateSgmRegister_kms(facid, digit, mobile, lid, userHash);
        }
    }
}
