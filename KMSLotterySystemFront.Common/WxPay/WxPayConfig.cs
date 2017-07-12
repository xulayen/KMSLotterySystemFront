using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Common.WxPay
{
    public class WxPayConfig
    {

        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        //public const string APPID = "wx2428e34e0e7dc6ef";
        //public const string MCHID = "1233410002";
        //public const string KEY = "e10adc3849ba56abbe56e056f20f883e";
        //public const string APPSECRET = "51c56b886b5be869567dd389b3e5d1d6";

        //中商官方微信
        public const string APPID = "wx050f2eb2da69e7d5";
        public const string MCHID = "1247115001";
        public const string KEY = "38B4510AAC2C456194663C95FC5C2D15";
        public const string APPSECRET = "e818a97c64b41c9da88edc68189003b2";


        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        public const string SSLCERT_PATH = "cert/apiclient_cert.p12";   //需要中商的证书
        public const string SSLCERT_PASSWORD = "1247115001";            //中商的商户号


        //=======【红包发送请求url】===================================== 

        public const string CashRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";  //现金红包

        public const string QueryRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo";  //查询现金红包/裂变红包

        public const string GroupRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendgroupredpack";  //裂变红包

        public const string QYPayUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";    //企业付款


        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public const string NOTIFY_URL = "http://paysdk.weixin.qq.com/example/ResultNotifyPage.aspx";


        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public const string IP = "8.8.8.8";


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public const string PROXY_URL = "http://10.152.18.220:8080";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public const int REPORT_LEVENL = 1;

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 0;
    }
}
