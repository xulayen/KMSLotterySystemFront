using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.DAL;
using KMSLotterySystemFront.Model;
using System.Data;

namespace KMSLotterySystemFront.BLLLottery
{
    public class smsBLL
    {
        #region 公共变量
        public readonly static smsDao smsdao = new smsDao();
        #endregion

        /// <summary>
        /// 添加短信验证码至验证码表
        /// </summary>
        /// <param name="userid">发送主体</param>
        /// <param name="mobile">接受信息的手机号码</param>
        /// <param name="vifycode">验证码</param>
        /// <param name="ip">提交的IP</param>
        /// <param name="facid">厂家编号</param>
        /// <returns></returns>
        public bool AddVerifyCode(string userid, string mobile, string vifycode, string ip, string facid)
        {
            try
            {
                if (smsdao.AddVerifyCode(userid, mobile, vifycode, ip, facid) > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:AddVerifyCode:" + facid + "---" + mobile + "---" + vifycode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }

        /// <summary>
        /// 获取手机发送的验证码
        /// </summary>
        /// <param name="facId">厂家编号</param>
        /// <param name="mobile">接受信息的手机号码</param>
        /// <param name="vifycode">验证码</param>
        /// <returns></returns>
        public bool GetVerifyMobile(string facId, string mobile, string vifycode)
        {
            bool bRet = false;
            try
            {
                object obj = smsdao.GetVerifyMobile(facId, mobile, vifycode);

                if (obj != null)
                {
                    if (obj.ToString().Equals(mobile))
                    {
                        bRet = true; 
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:GetVerifyMobile:" + facId + "---" + mobile + "---" + vifycode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return bRet;
            }
        }

        /// <summary>
        /// 验证手机短信验证码是否通过
        /// </summary>
        /// <param name="facId">厂家编号</param>
        /// <param name="mobile">接受信息的手机号码</param>
        /// <param name="vifycode">验证码</param>
        /// <param name="validTime">有效期时间</param>
        /// <returns></returns>
        public bool VerifyMobile(string facId, string mobile, string vifycode, int validTime = 5)
        {
            try
            {
                return smsdao.VerifyMobile(facId, mobile, vifycode, validTime);
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:VerifyMobile:" + facId + "---" + mobile + "---" + vifycode + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return false;
            }
        }
        /// <summary>
        /// 获取R5E短信发送情况
        /// </summary>
        /// <param name="facId"></param>
        /// <param name="mobile"></param>
        /// <param name="systemState"></param>
        /// <returns></returns>
        public DataTable GetVerifyCode(string facId, string mobile, out string systemState)
        {
            systemState = "002";
            DataTable dt = null;
            try
            {
                dt = smsdao.GetVerifyMobileByMobile(facId, mobile);
                if (dt != null)
                {
                    systemState = "001";
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.AppLog.Write("smsBLL:GetVerifyCode:" + facId + "---" + mobile + "---" + ex.StackTrace + "---" + ex.Source + "---" + ex.Message, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return null;
            }
        }
    }
}
