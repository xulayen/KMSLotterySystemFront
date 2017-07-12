using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class SendRedPackModel
    {
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string FactoryId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 活动编号
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 调用接口的机器Ip地址
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 商户logo的url 
        /// </summary>
        public Uri LogoImgUrl { get; set; }


        /// <summary>
        /// 商户名称
        /// </summary>
        public string MchName { get; set; }

        /// <summary>
        /// 付款金额，单位分
        /// </summary>
        public float Money { get; set; }

        /// <summary>
        /// 提供方名称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 接收者编号
        /// </summary>
        public string ReceiverOpenId { get; set; }

        /// <summary>
        /// 备注:在微信提供给用户的领取提醒中显示
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 分享内容
        /// </summary>
        public string ShareContent { get; set; }

        /// <summary>
        /// 分享图片
        /// </summary>
        public Uri ShareImgUrl { get; set; }

        /// <summary>
        /// 分享地址
        /// </summary>
        public Uri ShareUrl { get; set; }

        /// <summary>
        /// 红包祝福语
        /// </summary>
        public string Wish { get; set; }

    }

}
