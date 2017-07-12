using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class QueryHbResult
    {


        private string _guid = string.Empty;
        private string _ccnactivityid = string.Empty;
        private string _ccnactivityname = string.Empty;

        /// <summary>
        /// 查询红包答复guid
        /// </summary>
        public string guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        /// <summary>
        /// ccn内部活动编号
        /// </summary>
        public string ccnactivityid
        {
            get { return _ccnactivityid; }
            set { _ccnactivityid = value; }
        }
        /// <summary>
        /// ccn内部活动名称
        /// </summary>
        public string ccnactivityname
        {
            get { return _ccnactivityname; }
            set { _ccnactivityname = value; }
        }
        private string _facid = string.Empty;
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string facid
        {
            get { return _facid; }
            set { _facid = value; }
        }
        private string _mch_id = string.Empty;
        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string mch_id
        {
            get { return _mch_id; }
            set { _mch_id = value; }
        }
        private string _mch_billno = string.Empty;
        /// <summary>
        /// 商户使用查询API填写的商户单号的原路返回
        /// </summary>
        public string mch_billno
        {
            get { return _mch_billno; }
            set { _mch_billno = value; }
        }

        private string _detail_id = string.Empty;
        /// <summary>
        /// 使用API发放现金红包时返回的红包单号
        /// </summary>
        public string detail_id
        {
            get { return _detail_id; }
            set { _detail_id = value; }
        }
        private string _status = string.Empty;
        /// <summary>
        /// 红包状态：（SENDING:发放中 SENT:已发放待领取 FAILED：发放失败 RECEIVED:已领取 RFUND_ING:退款中 REFUND:已退款）
        /// </summary>
        public string status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _send_type = string.Empty;
        /// <summary>
        /// 发放类型：（API:通过API接口发放 UPLOAD:通过上传文件方式发放 ACTIVITY:通过活动方式发放）
        /// </summary>
        public string send_type
        {
            get { return _send_type; }
            set { _send_type = value; }
        }


        private string _hb_type = string.Empty;
        /// <summary>
        /// 红包类型：（GROUP:裂变红包 NORMAL:普通红包
        /// </summary>
        public string hb_type
        {
            get { return _hb_type; }
            set { _hb_type = value; }
        }
        private string _total_num = string.Empty;

        /// <summary>
        /// 红包个数
        /// </summary>
        public string total_num
        {
            get { return _total_num; }
            set { _total_num = value; }
        }

        private string _total_amount = string.Empty;
        /// <summary>
        /// 红包总金额（单位分）
        /// </summary>
        public string total_amount
        {
            get { return _total_amount; }
            set { _total_amount = value; }
        }


        private string _reason = string.Empty;
        /// <summary>
        /// 失败原因
        /// </summary>
        public string reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
        private string _send_time = string.Empty;
        /// <summary>
        /// 红包发送时间
        /// </summary>
        public string send_time
        {
            get { return _send_time; }
            set { _send_time = value; }
        }

        private string _refund_time = string.Empty;

        /// <summary>
        /// 红包退款时间（红包的退款时间（如果其未领取的退款））
        /// </summary>
        public string refund_time
        {
            get { return _refund_time; }
            set { _refund_time = value; }
        }

        private string _refund_amount = string.Empty;

        /// <summary>
        /// 红包退款金额
        /// </summary>
        public string refund_amount
        {
            get { return _refund_amount; }
            set { _refund_amount = value; }
        }


        private string _wishing = string.Empty;

        /// <summary>
        /// 祝福语
        /// </summary>
        public string wishing
        {
            get { return _wishing; }
            set { _wishing = value; }
        }


        private string _remark = string.Empty;

        /// <summary>
        /// 活动描述（低版本微信可见）
        /// </summary>
        public string remark
        {
            get { return _remark; }
            set { _remark = value; }
        }


        private string _act_name = string.Empty;

        /// <summary>
        /// 活动名称
        /// </summary>
        public string act_name
        {
            get { return _act_name; }
            set { _act_name = value; }
        }
        private string _hblist = string.Empty;
        /// <summary>
        /// 裂变红包领取列表
        /// </summary>
        public string hblist
        {
            get { return _hblist; }
            set { _hblist = value; }
        }
        private string _openid = string.Empty;

        /// <summary>
        /// 领取红包的openid
        /// </summary>
        public string openid
        {
            get { return _openid; }
            set { _openid = value; }
        }


        private string _amount = string.Empty;
        /// <summary>
        /// openid领取的金额
        /// </summary>
        public string amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        private string _rcv_time = string.Empty;
        /// <summary>
        /// openid领取红包的时间
        /// </summary>
        public string rcv_time
        {
            get { return _rcv_time; }
            set { _rcv_time = value; }
        }

        private string _return_code = string.Empty;
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code
        {
            get { return _return_code; }
            set { _return_code = value; }
        }

        private string _return_msg = string.Empty;
        /// <summary>
        /// 返回信息与return_code 对应
        /// </summary>
        public string return_msg
        {
            get { return _return_msg; }
            set { _return_msg = value; }
        }

        private string _result_code = string.Empty;
        /// <summary>
        /// 业务结果（SUCCESS/FAIL）
        /// </summary>
        public string result_code
        {
            get { return _result_code; }
            set { _result_code = value; }
        }

        private string _err_code = string.Empty;
        /// <summary>
        /// 错误码信息
        /// </summary>
        public string err_code
        {
            get { return _err_code; }
            set { _err_code = value; }
        }


        private string _err_code_des = string.Empty;

        /// <summary>
        /// 结果信息描述
        /// </summary>
        public string err_code_des
        {
            get { return _err_code_des; }
            set { _err_code_des = value; }
        }

        private string _solution = string.Empty;
        /// <summary>
        /// 针对错误码对应的解决方案
        /// </summary>
        public string solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

    }


    public enum HB_TYPE
    {
        NORMAL = 0,
        GROUP = 1,
        GETHBINFO = 2
    }


    public enum WxPayAPIType
    {
        发放红包 = 1,
    }

}
