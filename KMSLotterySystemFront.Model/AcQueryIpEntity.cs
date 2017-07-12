using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class AcQueryIpEntity
    {
        /// <summary>
        /// 对应用户授权表ID
        /// </summary>
        public string aid { get; set; }

        /// <summary>
        /// 企业服务器IP
        /// </summary>
        public string vip { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createdate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 使用状态(0失效,1有效)
        /// </summary>
        public string flag { get; set; }

        /// <summary>
        /// 匹配状态 (1:完全匹配  0:段匹配)
        /// </summary>
        public string type { get; set; }
    }
}
