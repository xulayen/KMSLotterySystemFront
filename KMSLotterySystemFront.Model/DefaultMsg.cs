// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：DefaultMsg.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：默认消息回复实体类
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class DefaultMsg
    {
        private string _NotActiivties = string.Empty;
        private string _NotMsg = string.Empty;
        private string _ErrorOfIp = string.Empty;
        private string _ErrorOfSystem = string.Empty;
        private string _ErrorOfDigit = string.Empty;

       

        /// <summary>
        /// 无抽奖活动
        /// </summary>
        public string NotActiivties
        {
            get { return _NotActiivties; }
            set { _NotActiivties = value; }
        }
       
        /// <summary>
        /// 获取活动消息配置错误
        /// </summary>
        public string NotMsg
        {
            get { return _NotMsg; }
            set { _NotMsg = value; }
        }
        
        /// <summary>
        /// IP地址或者手机号码格式错误
        /// </summary>
        public string ErrorOfIp
        {
            get { return _ErrorOfIp; }
            set { _ErrorOfIp = value; }
        }
       
        /// <summary>
        /// 系统错误
        /// </summary>
        public string ErrorOfSystem
        {
            get { return _ErrorOfSystem; }
            set { _ErrorOfSystem = value; }
        }

        /// <summary>
        /// 数码格式错误
        /// </summary>
        public string ErrorOfDigit
        {
            get { return _ErrorOfDigit; }
            set { _ErrorOfDigit = value; }
        }
    }
}
