using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class WechatClickEntity
    {
        /// <summary>
        /// 点击GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 点击用户OPENID
        /// </summary>
        public string OPENID { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string COUNTRY { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string PROVINCE { get; set; }

        /// <summary>
        /// 市区
        /// </summary>
        public string CITY { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NICKNAME { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string SEX { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 中奖记录GUID
        /// </summary>
        public string LOTTERYGUID { get; set; }

        /// <summary>
        /// 分享人OPENID
        /// </summary>
        public string SHAREOPENID { get; set; }

        /// <summary>
        /// 被转发门店ID
        /// </summary>
        public string SHARESTOREID { get; set; }

        /// <summary>
        /// 类别(1:海报link、分享link)
        /// </summary>
        public string TYPE { get; set; }

        /// <summary>
        /// 厂家编号
        /// </summary>
        public string FACID { get; set; }

        /// <summary>
        /// 厂家编号标识
        /// </summary>
        public string FACIDFLAG { get; set; }


        /// <summary>
        /// 进入homepage 类型  线上/线下 homepage 
        /// </summary>
        public string LINETYPE { get; set; }


        /// <summary>
        /// 进入通道 ：0 代表线下 ；线上：  1:广告 2:短信推送 3:微信公众号 4:朋友圈转发 )
        /// </summary>
        public string CHANNEL { get; set; }



        public string F1 { get; set; }

        public string F2 { get; set; }

        public string F3 { get; set; }

        public string F4 { get; set; }

        public string F5 { get; set; }

    }
}
