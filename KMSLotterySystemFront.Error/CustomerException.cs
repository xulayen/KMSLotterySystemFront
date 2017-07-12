using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using KMSLotterySystemFront.Common;

namespace KMSLotterySystemFront.Error
{
    [Serializable]
    public class CustomerException : Exception, ISerializable
    {
        public CustomerException() { }

        public CustomerException(string message, CustomerSystemState type)
            : base(message)
        {
            this.ErrorType = type;
            this.ErrorCode = Utility.GetEnumDescription(type);
        }

        public CustomerException(string logContent, string message, CustomerSystemState type, Exception innerException)
            : base(message)
        {
            this.ErrorType = type;
            this.ErrorCode = Utility.GetEnumDescription(type);
            KMSLotterySystemFront.Logger.AppLog.Write("【" + message + "】" + logContent, Logger.AppLog.LogMessageType.Error, innerException);
        }

        public CustomerException(string logContent, string message, CustomerSystemState type, Logger.AppLog.LogMessageType lt = Logger.AppLog.LogMessageType.Error)
            : base(message)
        {
            this.ErrorType = type;
            this.ErrorCode = Utility.GetEnumDescription(type);
            KMSLotterySystemFront.Logger.AppLog.Write("【" + message + "】" + logContent, lt);
        }

        public CustomerSystemState ErrorType { get; set; }

        public String ErrorCode { get; set; }

    }



    public enum CustomerSystemState
    {
        [Description("0X0001")]
        N非法提交,

        [Description("000000")]
        N系统异常,

        [Description("001000")]
        Y成功,

        [Description("001001")]
        Y手机验证码验证成功,

        [Description("001002")]
        Y手机验证码发送成功,

        [Description("001003")]
        Y中奖SW,//实物

        [Description("001004")]
        Yopenid存在记录,

        [Description("001005")]
        Y已填写邮寄地址,






        [Description("002000")]
        NN失败,

        [Description("002001")]
        NN手机号码格式不正确,

        [Description("002002")]
        NN手机号码不能为空,

        [Description("002003")]
        NN手机验证码格式不正确,

        [Description("002004")]
        NN验证码验证失败,

        [Description("002005")]
        NN验证码发送失败,

        [Description("002006")]
        NN防伪数码不能为空,

        [Description("002007")]
        NNopenid为空,

        [Description("002008")]
        NN解码的厂家不是本活动厂家,

        [Description("002009")]
        NN二维码解码失败,

        [Description("002010")]
        NN防伪数码格式错误,

        [Description("002011")]
        NN抽奖活动不存在,

        [Description("002012")]
        NN活动答复未配置,

        [Description("002013")]
        NNIP地址格式不正确,

        [Description("002014")]
        NN不是指定活动参与的数码厂家,

        [Description("002015")]
        NN数码所属表不存在,

        [Description("002016")]
        NN数码已经过期,

        [Description("002017")]
        NN数码所属表信息表结构异常,

        [Description("002018")]
        NN数码不存在,

        [Description("002019")]
        NN产品不参与活动,

        [Description("002020")]
        NN生产时间不在许可范围,

        [Description("002021")]
        NN数码生产时间为空,

        [Description("002022")]
        NN未配置参与活动的产品,

        [Description("002023")]
        NN数码未激活,

        [Description("002024")]
        NN活动未开始,

        [Description("002025")]
        NN活动已结束,

        [Description("002026")]
        NN活动奖池为空,

        [Description("002027")]
        NN用户已达当日开奖上限,

        [Description("002028")]
        NN未填写邮寄信息,

        [Description("002029")]
        NN数码已经参与过活动,

        [Description("002030")]
        NN未中奖,

        [Description("002031")]
        NN未中奖_未命中奖池,

        [Description("002032")]
        NNopenid不存在记录,

        [Description("002033")]
        NN未查询到中奖记录,


    }
}
