// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：MsgMdl.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：消息回复实体类
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
    /// 回复消息实体类
    /// </summary>
    public class MsgMdl
    {
        public MsgMdl()
        {

        }

        private string _ActivitiesHaveEnded = string.Empty;
        private string _ActivitiesNoStart = string.Empty;
        private string _TelecomMobile = string.Empty;
        private string _HasBeenWinning = string.Empty;
        private string _HasBeenWinningkq = string.Empty;
        private string _LotteryOver = string.Empty;
        private string _ExceedTheMaximumParticipation = string.Empty;
        private string _ExceedTheMaximumWinning = string.Empty;
        private string _DigitalHasBeenDrawing = string.Empty;
        private string _DigitalHasBeenWinning = string.Empty;
        private string _DigitalHasBeenQuery = string.Empty;
        private string _ErrorOfTheDigital = string.Empty;
        private string _DigitalFormatError = string.Empty;
        private string _SystemError = string.Empty;
        private string _DigitalIsNotWinning = string.Empty;
        private string _HasBeenWinning20 = string.Empty;
        private string _HasBeenWinning30 = string.Empty;
        private string _HasBeenWinning100 = string.Empty;
        private string _HasBeenWinning5 = string.Empty;
        private string _HasBeenWinning50 = string.Empty;
        private string _ProductNotActivities = string.Empty;
        private string _SysCodeNotActivation = string.Empty;

        private string _LottoSMSWarning = string.Empty;
        private string _ParticipateSMSWarning = string.Empty;
        private string _SysCodeLottoSMSWarning = string.Empty;

        private string _DigitCodeNotRegister = string.Empty;
        private string _DigitCodeReigster = string.Empty;
        private string _CodeRegisterSendMail = string.Empty;

        private string _MessageF1 = string.Empty;
        private string _MessageF2 = string.Empty;
        private string _MessageF3 = string.Empty;
        private string _MessageF4 = string.Empty;
        private string _MessageF5 = string.Empty;
        private string _MessageF6 = string.Empty;
        private string _MessageF7 = string.Empty;
        private string _MessageF8 = string.Empty;
        private string _MessageF9 = string.Empty;
        private string _MessageF10 = string.Empty;

        private string _HasBeenWinningF1 = string.Empty;
        private string _HasBeenWinningF2 = string.Empty;
        private string _HasBeenWinningF3 = string.Empty;
        private string _HasBeenWinningF4 = string.Empty;
        private string _HasBeenWinningF5 = string.Empty;
        private string _HasBeenWinningF6 = string.Empty;
        private string _HasBeenWinningF7 = string.Empty;
        private string _HasBeenWinningF8 = string.Empty;
        private string _HasBeenWinningF9 = string.Empty;
        private string _HasBeenWinningF10 = string.Empty;


        private string _StoreNoJoinActivity = string.Empty;
        private string _OpenidHasBeenJoin = string.Empty;
        private string _StoreInfoIsWrong = string.Empty;

        private string _MobileHasBeenWinning = string.Empty;
        private string _MobileHasBeenQuery = string.Empty;
        private string _MobileVerifyCodeNotExsit = string.Empty;



        private string _MobileTBSuccess = string.Empty;
        private string _MobileTBFail = string.Empty;
        private string _OpenidTBSuccess = string.Empty;
        private string _OpenidTBFail = string.Empty;
        private string _DigitTBSuccess = string.Empty;
        private string _DigitTBFail = string.Empty;
        private string _OpenidIsError = string.Empty;

        private string _UserNameIsError = string.Empty;
        private string _RedPackNumLimit = string.Empty;


        private string _HasBeenWinningHH = string.Empty;
        private string _HasBeenWinningSW = string.Empty;
        private string _HasBeenWinningHB = string.Empty;
        private string _HasBeenWinningLL = string.Empty;
        private string _HasBeenWinningXNQ = string.Empty;
        private string _HasBeenWinningLY = string.Empty;
      

        /// <summary>
        /// 发放红包次数超限
        /// </summary>
        public string RedPackNumLimit
        {
            get { return _RedPackNumLimit; }
            set { _RedPackNumLimit = value; }
        }
        

        /// <summary>
        /// 用户姓名格式不正确
        /// </summary>
        public string UserNameIsError
        {
            get { return _UserNameIsError; }
            set { _UserNameIsError = value; }
        }

        /// <summary>
        /// 手机号投保成功
        /// </summary>
        public string MobileTBSuccess
        {
            get { return _MobileTBSuccess; }
            set { _MobileTBSuccess = value; }
        }

        /// <summary>
        /// 手机号投保失败
        /// </summary>
        public string MobileTBFail
        {
            get { return _MobileTBFail; }
            set { _MobileTBFail = value; }
        }


        /// <summary>
        /// 微信号投保成功
        /// </summary>
        public string OpenidTBSuccess
        {
            get { return _OpenidTBSuccess; }
            set { _OpenidTBSuccess = value; }
        }

        /// <summary>
        ///  微信号投保失败
        /// </summary>
        public string OpenidTBFail
        {
            get { return _OpenidTBFail; }
            set { _OpenidTBFail = value; }
        }


        /// <summary>
        /// 数码投保失败
        /// </summary>
        public string DigitTBFail
        {
            get { return _DigitTBFail; }
            set { _DigitTBFail = value; }
        }


        /// <summary>
        /// 数码投保成功
        /// </summary>
        public string DigitTBSuccess
        {
            get { return _DigitTBSuccess; }
            set { _DigitTBSuccess = value; }
        }


        /// <summary>
        /// 手机验证码不存在
        /// </summary>
        public string MobileVerifyCodeNotExsit
        {
            get { return _MobileVerifyCodeNotExsit; }
            set { _MobileVerifyCodeNotExsit = value; }
        }

        /// <summary>
        /// 该手机号已经中过奖
        /// </summary>
        public string MobileHasBeenWinning
        {
            get { return _MobileHasBeenWinning; }
            set { _MobileHasBeenWinning = value; }
        }
        /// <summary>
        /// 该手机号已经参与过抽奖，但是没有中奖
        /// </summary>
        public string MobileHasBeenQuery
        {
            get { return _MobileHasBeenQuery; }
            set { _MobileHasBeenQuery = value; }
        }
        /// <summary>
        /// 该openid  已经参加过活动
        /// </summary>
        public string OpenidHasBeenJoin
        {
            get { return _OpenidHasBeenJoin; }
            set { _OpenidHasBeenJoin = value; }
        }

        /// <summary>
        /// 该门店没有参与资格活动或门店ID无效
        /// </summary>
        public string StoreNoJoinActivity
        {
            get { return _StoreNoJoinActivity; }
            set { _StoreNoJoinActivity = value; }
        }



        /// <summary>
        /// 门店ID-参数为空
        /// </summary>
        public string StoreInfoIsWrong
        {
            get { return _StoreInfoIsWrong; }
            set { _StoreInfoIsWrong = value; }
        }

        /// <summary>
        /// 活动已经结束
        /// </summary>
        public string ActivitiesHaveEnded
        {
            get { return _ActivitiesHaveEnded; }
            set { _ActivitiesHaveEnded = value; }
        }

        /// <summary>
        /// 活动还没有开始
        /// </summary>
        public string ActivitiesNoStart
        {
            get { return _ActivitiesNoStart; }
            set { _ActivitiesNoStart = value; }
        }

        /// <summary>
        /// 电信用户
        /// </summary>
        public string TelecomMobile
        {
            get { return _TelecomMobile; }
            set { _TelecomMobile = value; }
        }


        /// <summary>
        /// 已经中奖
        /// </summary>
        public string HasBeenWinning
        {
            get { return _HasBeenWinning; }
            set { _HasBeenWinning = value; }
        }

        /// <summary>
        /// 已经中奖 (京东卡券)
        /// </summary>
        public string HasBeenWinningkq
        {
            get { return _HasBeenWinningkq; }
            set { _HasBeenWinningkq = value; }
        }

        /// <summary>
        /// 奖项已兑完
        /// </summary>
        public string LotteryOver
        {
            get { return _LotteryOver; }
            set { _LotteryOver = value; }
        }


        /// <summary>
        ///  超过最大参与次数
        /// </summary>
        public string ExceedTheMaximumParticipation
        {
            get { return _ExceedTheMaximumParticipation; }
            set { _ExceedTheMaximumParticipation = value; }
        }
        /// <summary>
        ///  超过最大中奖次数
        /// </summary>
        public string ExceedTheMaximumWinning
        {
            get { return _ExceedTheMaximumWinning; }
            set { _ExceedTheMaximumWinning = value; }
        }
        /// <summary>
        /// 条码已经参与过抽奖
        /// </summary>
        public string DigitalHasBeenDrawing
        {
            get { return _DigitalHasBeenDrawing; }
            set { _DigitalHasBeenDrawing = value; }
        }

        /// <summary>
        /// 条码已经中过抽奖
        /// </summary>
        public string DigitalHasBeenWinning
        {
            get { return _DigitalHasBeenWinning; }
            set { _DigitalHasBeenWinning = value; }
        }

        /// <summary>
        /// 复查不可以参与活动
        /// </summary>
        public string DigitalHasBeenQuery
        {
            get { return _DigitalHasBeenQuery; }
            set { _DigitalHasBeenQuery = value; }
        }

        /// <summary>
        /// 数码错误或者非指定产品码
        /// </summary>
        public string ErrorOfTheDigital
        {
            get { return _ErrorOfTheDigital; }
            set { _ErrorOfTheDigital = value; }
        }


        /// <summary>
        /// 条码格式错误
        /// </summary>
        public string DigitalFormatError
        {
            get { return _DigitalFormatError; }
            set { _DigitalFormatError = value; }
        }

        /// <summary>
        /// 系统故障
        /// </summary>
        public string SystemError
        {
            get { return _SystemError; }
            set { _SystemError = value; }
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
        /// 成功充值20元
        /// </summary>
        public string HasBeenWinning20
        {
            get { return _HasBeenWinning20; }
            set { _HasBeenWinning20 = value; }
        }

        /// <summary>
        /// 成功充值30元
        /// </summary>
        public string HasBeenWinning30
        {
            get { return _HasBeenWinning30; }
            set { _HasBeenWinning30 = value; }
        }
        /// <summary>
        /// 成功充值100元
        /// </summary>
        public string HasBeenWinning100
        {
            get { return _HasBeenWinning100; }
            set { _HasBeenWinning100 = value; }
        }
        /// <summary>
        /// 成功充值5元
        /// </summary>
        public string HasBeenWinning5
        {
            get { return _HasBeenWinning5; }
            set { _HasBeenWinning5 = value; }
        }
        /// <summary>
        /// 成功充值50元
        /// </summary>
        public string HasBeenWinning50
        {
            get { return _HasBeenWinning50; }
            set { _HasBeenWinning50 = value; }
        }

        /// <summary>
        /// 产品未参与本次活动
        /// </summary>
        public string ProductNotActivities
        {
            get { return _ProductNotActivities; }
            set { _ProductNotActivities = value; }
        }

        /// <summary>
        /// 数码未参与防伪查询
        /// </summary>
        public string SysCodeNotActivation
        {
            get { return _SysCodeNotActivation; }
            set { _SysCodeNotActivation = value; }
        }


        /// <summary>
        /// 中奖短信预警
        /// </summary>
        public string LottoSMSWarning
        {
            get { return _LottoSMSWarning; }
            set { _LottoSMSWarning = value; }
        }

        /// <summary>
        /// 参与短信预警
        /// </summary>
        public string ParticipateSMSWarning
        {
            get { return _ParticipateSMSWarning; }
            set { _ParticipateSMSWarning = value; }
        }

        /// <summary>
        /// 指定数码中奖短信预警
        /// </summary>
        public string SysCodeLottoSMSWarning
        {
            get { return _SysCodeLottoSMSWarning; }
            set { _SysCodeLottoSMSWarning = value; }
        }

        /// <summary>
        /// 数码没有被注册
        /// </summary>
        public string DigitCodeNotRegister
        {
            get { return _DigitCodeNotRegister; }
            set { _DigitCodeNotRegister = value; }
        }

        /// <summary>
        /// 数码已经注册过
        /// </summary>
        public string DigitCodeReigster
        {
            get { return _DigitCodeReigster; }
            set { _DigitCodeReigster = value; }
        }

        /// <summary>
        /// 注册完成后邮件发送内容模板
        /// </summary>
        public string CodeRegisterSendMail
        {
            get { return _CodeRegisterSendMail; }
            set { _CodeRegisterSendMail = value; }
        }

        /// <summary>
        /// 备用答复1
        /// </summary>
        public string MessageF1
        {
            get { return _MessageF1; }
            set { _MessageF1 = value; }
        }

        /// <summary>
        /// 备用答复2
        /// </summary>
        public string MessageF2
        {
            get { return _MessageF2; }
            set { _MessageF2 = value; }
        }

        /// <summary>
        /// 备用答复3
        /// </summary>
        public string MessageF3
        {
            get { return _MessageF3; }
            set { _MessageF3 = value; }
        }

        /// <summary>
        /// 备用答复4
        /// </summary>
        public string MessageF4
        {
            get { return _MessageF4; }
            set { _MessageF4 = value; }
        }

        /// <summary>
        /// 备用答复5
        /// </summary>
        public string MessageF5
        {
            get { return _MessageF5; }
            set { _MessageF5 = value; }
        }

        /// <summary>
        /// 备用答复6
        /// </summary>
        public string MessageF6
        {
            get { return _MessageF6; }
            set { _MessageF6 = value; }
        }

        /// <summary>
        /// 备用答复7
        /// </summary>
        public string MessageF7
        {
            get { return _MessageF7; }
            set { _MessageF7 = value; }
        }

        /// <summary>
        /// 备用答复8
        /// </summary>
        public string MessageF8
        {
            get { return _MessageF8; }
            set { _MessageF8 = value; }
        }

        /// <summary>
        /// 备用答复9
        /// </summary>
        public string MessageF9
        {
            get { return _MessageF9; }
            set { _MessageF9 = value; }
        }

        /// <summary>
        /// 备用答复10
        /// </summary>
        public string MessageF10
        {
            get { return _MessageF10; }
            set { _MessageF10 = value; }
        }

        /// <summary>
        /// 中奖答复1
        /// </summary>
        public string HasBeenWinningF1
        {
            get { return _HasBeenWinningF1; }
            set { _HasBeenWinningF1 = value; }
        }
        /// <summary>
        /// 中奖答复2
        /// </summary>
        public string HasBeenWinningF2
        {
            get { return _HasBeenWinningF2; }
            set { _HasBeenWinningF2 = value; }
        }
        /// <summary>
        /// 中奖答复3
        /// </summary>
        public string HasBeenWinningF3
        {
            get { return _HasBeenWinningF3; }
            set { _HasBeenWinningF3 = value; }
        }
        /// <summary>
        /// 中奖答复4
        /// </summary>
        public string HasBeenWinningF4
        {
            get { return _HasBeenWinningF4; }
            set { _HasBeenWinningF4 = value; }
        }
        /// <summary>
        /// 中奖答复5
        /// </summary>
        public string HasBeenWinningF5
        {
            get { return _HasBeenWinningF5; }
            set { _HasBeenWinningF5 = value; }
        }
        /// <summary>
        /// 中奖答复6
        /// </summary>
        public string HasBeenWinningF6
        {
            get { return _HasBeenWinningF6; }
            set { _HasBeenWinningF6 = value; }
        }
        /// <summary>
        /// 中奖答复7
        /// </summary>
        public string HasBeenWinningF7
        {
            get { return _HasBeenWinningF7; }
            set { _HasBeenWinningF7 = value; }
        }
        /// <summary>
        /// 中奖答复8
        /// </summary>
        public string HasBeenWinningF8
        {
            get { return _HasBeenWinningF8; }
            set { _HasBeenWinningF8 = value; }
        }
        /// <summary>
        /// 中奖答复9
        /// </summary>
        public string HasBeenWinningF9
        {
            get { return _HasBeenWinningF9; }
            set { _HasBeenWinningF9 = value; }
        }
        /// <summary>
        /// 中奖答复10
        /// </summary>
        public string HasBeenWinningF10
        {
            get { return _HasBeenWinningF10; }
            set { _HasBeenWinningF10 = value; }
        }

        /// <summary>
        /// 微信号格式错误
        /// </summary>
        public string OpenidIsError
        {
            get { return _OpenidIsError; }
            set { _OpenidIsError = value; }
        }

        /// <summary>
        /// 中奖-话费
        /// </summary>
        public string HasBeenWinningHH
        {
            get { return _HasBeenWinningHH; }
            set { _HasBeenWinningHH = value; }
        }

        /// <summary>
        /// 中奖-旅游
        /// </summary>
        public string HasBeenWinningLY
        {
            get { return _HasBeenWinningLY; }
            set { _HasBeenWinningLY = value; }
        }

        /// <summary>
        /// 中奖-实物礼品
        /// </summary>
        public string HasBeenWinningSW
        {
            get { return _HasBeenWinningSW; }
            set { _HasBeenWinningSW = value; }
        }

        /// <summary>
        /// 中奖-红包
        /// </summary>
        public string HasBeenWinningHB
        {
            get { return _HasBeenWinningHB; }
            set { _HasBeenWinningHB = value; }
        }

        /// <summary>
        /// 中奖-流量
        /// </summary>
        public string HasBeenWinningLL
        {
            get { return _HasBeenWinningLL; }
            set { _HasBeenWinningLL = value; }
        }

        /// <summary>
        /// 中奖-虚拟券
        /// </summary>
        public string HasBeenWinningXNQ
        {
            get { return _HasBeenWinningXNQ; }
            set { _HasBeenWinningXNQ = value; }
        }

    }

   
}
