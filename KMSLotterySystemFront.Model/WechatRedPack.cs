using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class WechatRedPack
    {
        private string _WEIXIN_ACT_ID = "act_id";
        private string _WEIXIN_ACT_NAME = "act_name";
        private string _WEIXIN_APPID = "wxappid";
        private string _WEIXIN_APPSECRET = "appSecret";
        private string _WEIXIN_CERTIFICATEPATH = "CertificatePath";
        private string _WEIXIN_CERTIFICATESECRET = "CertificateSecret";
        private string _WEIXIN_CLIENT_IP = "client_ip";
        private string _WEIXIN_LOGO_IMGURL = "logo_imgurl";
        private string _WEIXIN_MAX_VALUE = "max_value";
        private string _WEIXIN_MCH_BILLNO = "mch_billno";
        private string _WEIXIN_MCH_ID = "mch_id";
        private string _WEIXIN_MIN_VALUE = "min_value";
        private string _WEIXIN_NICK_NAME = "nick_name";
        private string _WEIXIN_RE_OPENID = "re_openid";
        private string _WEIXIN_REMARK = "remark";
        private string _WEIXIN_SEND_NAME = "send_name";
        private string _WEIXIN_SHARE_CONTENT = "share_content";
        private string _WEIXIN_SHARE_IMGURL = "share_imgurl";
        private string _WEIXIN_SHARE_URL = "share_url";
        private string _WEIXIN_TOTAL_AMOUNT = "total_amount";
        private string _WEIXIN_TOTAL_NUM = "total_num";
        private string _WEIXIN_WISHING = "wishing";
        private string _WEIXIN_FACID = string.Empty;
        private string _WEIXIN_ACTIVITYID = string.Empty;
        private string _WEIXIN_ACTIVITYNAME = string.Empty;
        private string _WEIXIN_CODEID = string.Empty;
        private string _WEIXIN_GUID = string.Empty;
        private string _WEIXIN_STATE = string.Empty;
        private string _WEIXIN_SENDMESSAGE = string.Empty;
        private string _WEIXIN_MCHBILLNO = "";//商户订单号

        private string _WEIXIN_CERTPASS = string.Empty;
        private string _WEIXIN_HB_TYPE = string.Empty;
        private string _WEIXIN_LOTTERYID = string.Empty;
        private string _WEIXIN_STATEDETAIL = string.Empty;
        private string _WEIXIN_SEND_LISTID = string.Empty;//红包订单的微信单号

        /// <summary>
        /// 红包订单的微信单号
        /// </summary>
        public string WEIXIN_SEND_LISTID
        {
            get { return _WEIXIN_SEND_LISTID; }
            set { _WEIXIN_SEND_LISTID = value; }
        }



        /// <summary>
        /// 微信发放状态描述
        /// </summary>
        public string WEIXIN_STATEDETAIL
        {
            get { return _WEIXIN_STATEDETAIL; }
            set { _WEIXIN_STATEDETAIL = value; }
        }



        /// <summary>
        /// 中奖guid
        /// </summary>
        public string WEIXIN_LOTTERYID
        {
            get { return _WEIXIN_LOTTERYID; }
            set { _WEIXIN_LOTTERYID = value; }
        }


        /// <summary>
        /// 发放红包类型（0普通红包，1 裂变红包）
        /// </summary>
        public string WEIXIN_HB_TYPE
        {
            get { return _WEIXIN_HB_TYPE; }
            set { _WEIXIN_HB_TYPE = value; }
        }


        /// <summary>
        /// 证书密码（默认为商户号ID）
        /// </summary>
        public string WEIXIN_CERTPASS
        {
            get { return _WEIXIN_CERTPASS; }
            set { _WEIXIN_CERTPASS = value; }
        }

        /// <summary>
        /// 红包发放状态
        /// </summary>
        public string WEIXIN_STATE
        {
            get { return _WEIXIN_STATE; }
            set { _WEIXIN_STATE = value; }
        }

        /// <summary>
        /// 微信平台反馈消息结果
        /// </summary>
        public string WEIXIN_SENDMESSAGE
        {
            get { return _WEIXIN_SENDMESSAGE; }
            set { _WEIXIN_SENDMESSAGE = value; }
        }



        /// <summary>
        /// 数码
        /// </summary>
        public string WEIXIN_CODEID
        {
            get { return _WEIXIN_CODEID; }
            set { _WEIXIN_CODEID = value; }
        }
        /// <summary>
        /// 红包充值记录GUID
        /// </summary>
        public string WEIXIN_GUID
        {
            get { return _WEIXIN_GUID; }
            set { _WEIXIN_GUID = value; }
        }
        /// <summary>
        /// 活动ID(内部)
        /// </summary>
        public string WEIXIN_ACTIVITYID
        {
            get { return _WEIXIN_ACTIVITYID; }
            set { _WEIXIN_ACTIVITYID = value; }
        }
        /// <summary>
        /// 活动名称(内部)
        /// </summary>
        public string WEIXIN_ACTIVITYNAME
        {
            get { return _WEIXIN_ACTIVITYNAME; }
            set { _WEIXIN_ACTIVITYNAME = value; }
        }
        /// <summary>
        /// CCN厂家编号
        /// </summary>
        public string WEIXIN_FACID
        {
            get { return _WEIXIN_FACID; }
            set { _WEIXIN_FACID = value; }
        }
        /// <summary>
        /// 外部记录ID(用于匹配和查询外部系统记录的充值情况)
        /// </summary>
        public string WEIXIN_ACT_ID
        {
            get { return _WEIXIN_ACT_ID; }
            set { _WEIXIN_ACT_ID = value; }
        }
        /// <summary>
        /// 红包发送方昵称
        /// </summary>
        public string WEIXIN_NICK_NAME
        {
            get { return _WEIXIN_NICK_NAME; }
            set { _WEIXIN_NICK_NAME = value; }
        }
        /// <summary>
        /// 红包接收用户OPENID
        /// </summary>
        public string WEIXIN_RE_OPENID
        {
            get { return _WEIXIN_RE_OPENID; }
            set { _WEIXIN_RE_OPENID = value; }
        }
        /// <summary>
        /// 红包接收用户，打开红包后的祝福语
        /// </summary>
        public string WEIXIN_WISHING
        {
            get { return _WEIXIN_WISHING; }
            set { _WEIXIN_WISHING = value; }
        }
        public string WEIXIN_TOTAL_NUM
        {
            get { return _WEIXIN_TOTAL_NUM; }
            set { _WEIXIN_TOTAL_NUM = value; }
        }
        /// <summary>
        /// 红包金额
        /// </summary>
        public string WEIXIN_TOTAL_AMOUNT
        {
            get { return _WEIXIN_TOTAL_AMOUNT; }
            set { _WEIXIN_TOTAL_AMOUNT = value; }
        }
        /// <summary>
        /// 分享链接URL地址（没有填一个格式就可以，但不能空，如为空了校验不过去）
        /// </summary>
        public string WEIXIN_SHARE_URL
        {
            get { return _WEIXIN_SHARE_URL; }
            set { _WEIXIN_SHARE_URL = value; }
        }
        /// <summary>
        /// 分享链接URL头部图片（没有填一个格式就可以，但不能空，如为空了校验不过去）
        /// </summary>
        public string WEIXIN_SHARE_IMGURL
        {
            get { return _WEIXIN_SHARE_IMGURL; }
            set { _WEIXIN_SHARE_IMGURL = value; }
        }
        /// <summary>
        /// 分享内容
        /// </summary>
        public string WEIXIN_SHARE_CONTENT
        {
            get { return _WEIXIN_SHARE_CONTENT; }
            set { _WEIXIN_SHARE_CONTENT = value; }
        }
        /// <summary>
        /// 红包发送方名称
        /// </summary>
        public string WEIXIN_SEND_NAME
        {
            get { return _WEIXIN_SEND_NAME; }
            set { _WEIXIN_SEND_NAME = value; }
        }
        /// <summary>
        /// 发送语(打开红包后的祝福语)
        /// </summary>
        public string WEIXIN_REMARK
        {
            get { return _WEIXIN_REMARK; }
            set { _WEIXIN_REMARK = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WEIXIN_MIN_VALUE
        {
            get { return _WEIXIN_MIN_VALUE; }
            set { _WEIXIN_MIN_VALUE = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WEIXIN_MCH_BILLNO
        {
            get { return _WEIXIN_MCH_BILLNO; }
            set { _WEIXIN_MCH_BILLNO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WEIXIN_MAX_VALUE
        {
            get { return _WEIXIN_MAX_VALUE; }
            set { _WEIXIN_MAX_VALUE = value; }
        }
        /// <summary>
        /// 发送红包微信公众号LOGO图片URL地址
        /// </summary>
        public string WEIXIN_LOGO_IMGURL
        {
            get { return _WEIXIN_LOGO_IMGURL; }
            set { _WEIXIN_LOGO_IMGURL = value; }
        }
        /// <summary>
        /// 提交发送红包的URL地址
        /// </summary>
        public string WEIXIN_CLIENT_IP
        {
            get { return _WEIXIN_CLIENT_IP; }
            set { _WEIXIN_CLIENT_IP = value; }
        }
        public string WEIXIN_APPSECRET
        {
            get { return _WEIXIN_APPSECRET; }
            set { _WEIXIN_APPSECRET = value; }
        }
        /// <summary>
        /// 打开红包后活动名称 
        /// </summary>
        public string WEIXIN_ACT_NAME
        {
            get { return _WEIXIN_ACT_NAME; }
            set { _WEIXIN_ACT_NAME = value; }
        }
        /// <summary>
        /// *商户ID(微信支付平台)
        /// </summary>
        public string WEIXIN_MCH_ID
        {
            get { return _WEIXIN_MCH_ID; }
            set { _WEIXIN_MCH_ID = value; }
        }/// <summary>
        /// *支付秘钥(微信支付平台)
        /// </summary>
        public string WEIXIN_CERTIFICATESECRET
        {
            get { return _WEIXIN_CERTIFICATESECRET; }
            set { _WEIXIN_CERTIFICATESECRET = value; }
        }
        /// <summary>
        /// *微信支付证书名称(微信支付平台)
        /// </summary>
        public string WEIXIN_CERTIFICATEPATH
        {
            get { return _WEIXIN_CERTIFICATEPATH; }
            set { _WEIXIN_CERTIFICATEPATH = value; }
        }
        /// <summary>
        /// *微信支付APPID
        /// </summary>
        public string WEIXIN_APPID
        {
            get { return _WEIXIN_APPID; }
            set { _WEIXIN_APPID = value; }
        }

        /// <summary>
        /// *微信返回的订单号
        /// </summary>
        public string WEIXIN_MCHBILLNO
        {
            get { return _WEIXIN_MCHBILLNO; }
            set { _WEIXIN_MCHBILLNO = value; }
        }
    }
}
