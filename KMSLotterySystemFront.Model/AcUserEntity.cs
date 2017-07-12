using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class AcUserEntity
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 授权帐号
        /// </summary>
        public string userid { get; set; }
        
        /// <summary>
        /// 授权密码
        /// </summary>
        public string userpwd { get; set; }

        /// <summary>
        /// 厂家编号
        /// </summary>
        public string factoryid { get; set; }



        /// <summary>
        /// 内部（自定义）厂家编号
        /// </summary>
        public string infactoryid { get; set; }


        /// <summary>
        /// 用户标识
        /// </summary>
        public string oid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; }

        /// <summary>
        /// 可用标志(0：不可用，1可用)
        /// </summary>
        public string flag { get; set; }

        /// <summary>
        /// 返回信息模板
        /// </summary>
        public string reply { get; set; }

        /// <summary>
        /// 授权开始时间
        /// </summary>
        public string begintime { get; set; }

        /// <summary>
        /// 授权结束时间
        /// </summary>
        public string endtime { get; set; }

        /// <summary>
        /// 是否记录防伪查询日志 (0:记录,1:不记录)
        /// </summary>
        public string iscustomer { get; set; }


        /// <summary>
        /// 活动名称（客户名称）
        /// </summary>
        public string activename { get; set; }

        /// <summary>
        /// 允许被查询的厂家产品编号
        /// </summary>
        public string facproductid { get; set; }



        /// <summary>
        /// 对应项目编号（针对短信发送平台的PID，便于统计短信计费）
        /// </summary>
        public string projectid { get; set; }

        /// <summary>
        /// 需要发送的短信内容
        /// </summary>
        public string sendmessage { get; set; }

        /// <summary>
        /// 平台分配的短信发送账号
        /// </summary>
        public string sendmessageuserid { get; set; }

        /// <summary>
        /// 平台分配的短信发送密码
        /// </summary>
        public string sendmessageuserpwd { get; set; }

        /// <summary>
        /// 加密TOKENKEY参数
        /// </summary>
        public string tokenkey { get; set; }

        /// <summary>
        /// 接口调用频率限制
        /// </summary>
        public string freq_limit { get; set; }

        public string f1 { get; set; }
        public string f2 { get; set; }
        public string f3 { get; set; }
        public string f4 { get; set; }
        public string f5 { get; set; }

        /// <summary>
        /// 授权IP表记录集合
        /// </summary>
        public List<AcQueryIpEntity> QueryIPInfo { get; set; }

    }
}
