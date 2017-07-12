// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：PointsActivity
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-10-30 10:57:50  
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
    public class PointsActivity
    {
        private string _activityid = string.Empty;

        /// <summary>
        /// 活动编码
        /// </summary>
        public string Activityid
        {
            get { return _activityid; }
            set { _activityid = value; }
        }

        private string _activityname = string.Empty;

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Activityname
        {
            get { return _activityname; }
            set { _activityname = value; }
        }
        private string _activityfacid = string.Empty;

        /// <summary>
        /// 活动工厂ID
        /// </summary>
        public string Activityfacid
        {
            get { return _activityfacid; }
            set { _activityfacid = value; }
        }
        private string _activityproid = string.Empty;
    }
}
