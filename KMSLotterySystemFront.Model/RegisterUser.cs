// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：RegisterUser.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：中奖注册信息实体类
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
    public class RegisterUser
    {
        #region　LotteryInfo构造
        public RegisterUser()
        { }
        public RegisterUser(string _spro, string _category, string _ip, string _vdate, string _lotterylevel, string _change_way, string _change_type, string _user_name, string _user_address, string _user_zipcode, string _user_telephone, string _change_date, string _post_date, string _state, string _closeing_date, string _memo, string _flag, string _lotteryname, string _company, string _newcode, string _facid)
        {
            this.SPRO = _spro;
            this.CATEGORY = _category;
            this.IP = _ip;
            this.VDATE = _vdate;
            this.LOTTERYLEVEL = _lotterylevel;
            this.CHANGE_WAY = _change_way;
            this.CHANGE_TYPE = _change_type;
            this.USER_NAME = _user_name;
            this.USER_ADDRESS = _user_address;
            this.USER_ZIPCODE = _user_zipcode;
            this.USER_TELEPHONE = _user_telephone;
            this.CHANGE_DATE = _change_date;
            this.POST_DATE = _post_date;
            this.STATE = _state;
            this.CLOSEING_DATE = _closeing_date;
            this.MEMO = _memo;
            this.FLAG = _flag;
            this.Lotteryname = _lotteryname;
            this.Company = _company;
            this.Newcoed = _newcode;
            this.Facid = _facid;
        }

        #region 注册信息实体
        //变量
        private string _spro = string.Empty;
        private string _category = string.Empty;
        private string _ip = string.Empty;
        private string _vdate = string.Empty;
        private string _lotterylevel = string.Empty;
        private string _change_way = string.Empty;
        private string _change_type = string.Empty;
        private string _user_name = string.Empty;
        private string _user_address = string.Empty;
        private string _user_zipcode = string.Empty;
        private string _user_telephone = string.Empty;
        private string _change_date = string.Empty;
        private string _post_date = string.Empty;
        private string _state = string.Empty;
        private string _closeing_date = string.Empty;
        private string _memo = string.Empty;
        private string _flag = string.Empty;
        private string _newcoed = string.Empty;
        private string _facid = string.Empty;
        private string _lotteryname = string.Empty;
        private string _company = string.Empty;

        /// <summary>
        /// 奖品名称
        /// </summary>
        public string Lotteryname
        {
            get { return _lotteryname; }
            set { _lotteryname = value; }
        }
       
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }

        /// <summary>
        /// 二次加密后的数码
        /// </summary>
        public string Newcoed
        {
            get { return _newcoed; }
            set { _newcoed = value; }
        }
      
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string Facid
        {
            get { return _facid; }
            set { _facid = value; }
        }

        /// <summary>
        /// 数码(标签上的数码)
        /// </summary>
        public string SPRO
        {
            get { return _spro; }
            set { _spro = value; }
        }

        /// <summary>
        /// 查询方式（Wap、手机短信、网站）
        /// </summary>
        public string CATEGORY
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// 来源号码(IP/电话号码/手机号码)
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        /// <summary>
        /// 数码被查询的时间
        /// </summary>
        public string VDATE
        {
            get { return _vdate; }
            set { _vdate = value; }
        }

        /// <summary>
        /// 奖项级别
        /// </summary>
        public string LOTTERYLEVEL
        {
            get { return _lotterylevel; }
            set { _lotterylevel = value; }
        }

        /// <summary>
        /// 兑奖途径
        /// </summary>
        public string CHANGE_WAY
        {
            get { return _change_way; }
            set { _change_way = value; }
        }

        /// <summary>
        /// 兑奖方式
        /// </summary>
        public string CHANGE_TYPE
        {
            get { return _change_type; }
            set { _change_type = value; }
        }

        /// <summary>
        /// 中奖姓名
        /// </summary>
        public string USER_NAME
        {
            get { return _user_name; }
            set { _user_name = value; }
        }

        /// <summary>
        /// 中奖邮寄地址
        /// </summary>
        public string USER_ADDRESS
        {
            get { return _user_address; }
            set { _user_address = value; }
        }

        /// <summary>
        /// 中奖邮编
        /// </summary>
        public string USER_ZIPCODE
        {
            get { return _user_zipcode; }
            set { _user_zipcode = value; }
        }

        /// <summary>
        /// 中奖联系电话
        /// </summary>
        public string USER_TELEPHONE
        {
            get { return _user_telephone; }
            set { _user_telephone = value; }
        }

        /// <summary>
        /// 兑奖日期
        /// </summary>
        public string CHANGE_DATE
        {
            get { return _change_date; }
            set { _change_date = value; }
        }

        /// <summary>
        /// 寄送日期
        /// </summary>
        public string POST_DATE
        {
            get { return _post_date; }
            set { _post_date = value; }
        }

        /// <summary>
        /// 兑奖状态（未兑、邮寄中、已寄出、未收到、兑奖成功、兑奖失败）
        /// </summary>
        public string STATE
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// 兑奖截止日期
        /// </summary>
        public string CLOSEING_DATE
        {
            get { return _closeing_date; }
            set { _closeing_date = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string MEMO
        {
            get { return _memo; }
            set { _memo = value; }
        }

        /// <summary>
        /// 可用标志(1:可用,0:不可用)
        /// </summary>
        public string FLAG
        {
            get { return _flag; }
            set { _flag = value; }
        }

        #endregion Model

        #endregion Model
    }
}
