using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class WxPayResult
    {

        private string _return_code = string.Empty;
        private string _return_msg = string.Empty;
        private string _result_code = string.Empty;
        private string _err_code = string.Empty;
        private string _err_code_des = string.Empty;
        private string _mch_billno = string.Empty;
        private string _mch_id = string.Empty;
        private string _wxappid = string.Empty;
        private string _re_openid = string.Empty;
        private string _total_amount = string.Empty;
        private string _total_num = string.Empty;//发放红包数量
        private string _hb_type = string.Empty;//红包类型0  普通红包，  1  裂变红包
        private string _send_listid = string.Empty;

        //额外信息
        private string _ccnactivityid = string.Empty;
        private string _ccnactivityname = string.Empty;
        private string _codeid = string.Empty;
        private string _facid = string.Empty;
        private string _ip = string.Empty;

        private string _guid = string.Empty;

        /// <summary>
        /// 中奖纪录
        /// </summary>
        public string guid
        {
            get { return _guid; }
            set { _guid = value; }
        }


        // 返回状态码 SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string return_code
        {
            get { return _return_code; }
            set { _return_code = value; }
        }

        /// <summary>
        /// 返回信息，与return_code对应
        /// </summary>
        public string return_msg
        {
            get { return _return_msg; }
            set { _return_msg = value; }
        }


        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code
        {
            get { return _result_code; }
            set { _result_code = value; }
        }

        /// <summary>
        /// 错误码信息
        /// </summary>
        public string err_code
        {
            get { return _err_code; }
            set { _err_code = value; }
        }


        /// <summary>
        /// 结果信息描述
        /// </summary>
        public string err_code_des
        {
            get { return _err_code_des; }
            set { _err_code_des = value; }
        }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string mch_billno
        {
            get { return _mch_billno; }
            set { _mch_billno = value; }
        }

        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id
        {
            get { return _mch_id; }
            set { _mch_id = value; }
        }

        /// <summary>
        /// 微信分配的公众账号ID
        /// </summary>
        public string wxappid
        {
            get { return _wxappid; }
            set { _wxappid = value; }
        }

        /// <summary>
        /// 接收充值的OPENID
        /// </summary>
        public string re_openid
        {
            get { return _re_openid; }
            set { _re_openid = value; }
        }

        /// <summary>
        /// 付款金额
        /// </summary>
        public string total_amount
        {
            get { return _total_amount; }
            set { _total_amount = value; }
        }
        /// <summary>
        /// 红包订单的微信单号
        /// </summary>
        public string send_listid
        {
            get { return _send_listid; }
            set { _send_listid = value; }
        }


        /// <summary>
        /// //红包类型0  普通红包，  1  裂变红包
        /// </summary>
        public string hb_type
        {
            get { return _hb_type; }
            set { _hb_type = value; }
        }



        /// <summary>
        /// 发放红包数量
        /// </summary>
        public string total_num
        {
            get { return _total_num; }
            set { _total_num = value; }
        }




        //额外信息

        /// <summary>
        /// 内部活动编号
        /// </summary>
        public string ccnactivityid
        {
            get { return _ccnactivityid; }
            set { _ccnactivityid = value; }
        }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ccnactivityname
        {
            get { return _ccnactivityname; }
            set { _ccnactivityname = value; }
        }


        /// <summary>
        /// 中奖防伪数码
        /// </summary>
        public string codeid
        {
            get { return _codeid; }
            set { _codeid = value; }
        }


        /// <summary>
        /// 厂家编号
        /// </summary>
        public string facid
        {
            get { return _facid; }
            set { _facid = value; }
        }


        /// <summary>
        /// 调用端IP
        /// </summary>
        public string ip
        {
            get { return _ip; }
            set { _ip = value; }
        }


    }

}
