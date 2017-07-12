// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：Userinfo
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 15:38:29  
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
    public class Userinfo
    {
        private string _userid = string.Empty;
        private int _userpointused = 0;
        private int _userpointvalid = 0;
        private int _userpointtotal = 0;
        private string _username = string.Empty;
        private string _usermobile = string.Empty;
        private string _userguid = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Userid
        {
            get { return _userid; }
            set { _userid = value; }
        }

        /// <summary>
        /// 用户GUID
        /// </summary>
        public string Userguid
        {
            get { return _userguid; }
            set { _userguid = value; }
        }

        /// <summary>
        /// 用户手机号码
        /// </summary>
        public string Usermobile
        {
            get { return _usermobile; }
            set { _usermobile = value; }
        }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        
        /// <summary>
        /// 用户总积分
        /// </summary>
        public int Userpointtotal
        {
            get { return _userpointtotal; }
            set { _userpointtotal = value; }
        }
        
        /// <summary>
        /// 用户可用积分
        /// </summary>
        public int Userpointvalid
        {
            get { return _userpointvalid; }
            set { _userpointvalid = value; }
        }
       
        /// <summary>
        /// 用户已用积分
        /// </summary>
        public int Userpointused
        {
            get { return _userpointused; }
            set { _userpointused = value; }
        }
    }

}
