// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Model 
// *文件名称：ActivityInfo.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：活动总控实体类
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
    /// 活动总控实体类
    /// </summary>
    public class ActivityInfo
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

        /// <summary>
        /// 活动产品编码
        /// </summary>
        public string Activityproid
        {
            get { return _activityproid; }
            set { _activityproid = value; }
        }
        private string _productmark = string.Empty;

        /// <summary>
        /// 活动是否区分产品
        /// </summary>
        public string Productmark
        {
            get { return _productmark; }
            set { _productmark = value; }
        }
    }

    public class ShakeBaseData
    {
        /// <summary>
        /// 键值对ID
        /// </summary>
        public string codeid { get; set; }
        /// <summary>
        /// 键值对名称
        /// </summary>
        public string codename { get; set; }
        /// <summary>
        /// 键值对分组ID
        /// </summary>
        public string datatypename { get; set; }
        /// <summary>
        /// 键值对描述
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public string lotterymoeny { get; set; }
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string facid { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string subtype { get; set; } 
    }
}
