// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：PointsResultMsg
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-31 11:44:18  
//
// *创建标识：
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
    /// <summary>
    /// 积分抽奖活动默认答复实体类
    /// </summary>
    public class PointsResultMsg
    {
        public PointsResultMsg()
        {

        }

        private string _ErrorOfUser = string.Empty;
        private string _ActivitiesNoStart = string.Empty;
        private string _UserPointsNotLack = string.Empty;
        private string _ParticipateSMSWarning = string.Empty;
        private string _ExceedTheMaximumWinning = string.Empty;
        private string _ExceedTheMaximumParticipation = string.Empty;
        private string _HasBeenWinning = string.Empty;
        private string _DigitalIsNotWinning = string.Empty;
        private string _LottoSMSWarning = string.Empty;
        private string _Ohter = string.Empty;
        private string _SystemException = string.Empty;
        private string _SuccessOfParticipation = string.Empty;
        private string _FailOfParticipation = string.Empty;


        /// <summary>
        /// 用户不存在
        /// </summary>
        public string ErrorOfUser
        {
            get { return _ErrorOfUser; }
            set { _ErrorOfUser = value; }
        }
        
        /// <summary>
        /// 活动还没有开始或已结束！
        /// </summary>
        public string ActivitiesNoStart
        {
            get { return _ActivitiesNoStart; }
            set { _ActivitiesNoStart = value; }
        }
       
        /// <summary>
        /// 用户可用积分分值不够
        /// </summary>
        public string UserPointsNotLack
        {
            get { return _UserPointsNotLack; }
            set { _UserPointsNotLack = value; }
        }
        
        /// <summary>
        /// 参与次数预警短信提醒
        /// </summary>
        public string ParticipateSMSWarning
        {
            get { return _ParticipateSMSWarning; }
            set { _ParticipateSMSWarning = value; }
        }
        
        /// <summary>
        /// 超过中奖次数限制
        /// </summary>
        public string ExceedTheMaximumWinning
        {
            get { return _ExceedTheMaximumWinning; }
            set { _ExceedTheMaximumWinning = value; }
        }
        
        /// <summary>
        /// 超过参与次数限制
        /// </summary>
        public string ExceedTheMaximumParticipation
        {
            get { return _ExceedTheMaximumParticipation; }
            set { _ExceedTheMaximumParticipation = value; }
        }
       
        /// <summary>
        /// 中奖
        /// </summary>
        public string HasBeenWinning
        {
            get { return _HasBeenWinning; }
            set { _HasBeenWinning = value; }
        }
       
        /// <summary>
        /// 未中奖
        /// </summary>
        public string DigitalIsNotWinning
        {
            get { return _DigitalIsNotWinning; }
            set { _DigitalIsNotWinning = value; }
        }
       
        /// <summary>
        /// 中奖预警短信提醒
        /// </summary>
        public string LottoSMSWarning
        {
            get { return _LottoSMSWarning; }
            set { _LottoSMSWarning = value; }
        }
        
        /// <summary>
        /// 其他答复
        /// </summary>
        public string Ohter
        {
            get { return _Ohter; }
            set { _Ohter = value; }
        }

        /// <summary>
        /// 系统异常答复
        /// </summary>
        public string SystemException
        {
            get { return _SystemException; }
            set { _SystemException = value; }
        }

        /// <summary>
        /// 参与积分抽奖活动成功
        /// </summary>
        public string SuccessOfParticipation
        {
            get { return _SuccessOfParticipation; }
            set { _SuccessOfParticipation = value; }
        }

        /// <summary>
        /// 参与积分抽奖活动失败
        /// </summary>
        public string FailOfParticipation
        {
            get { return _FailOfParticipation; }
            set { _FailOfParticipation = value; }
        }
    }
}
