using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    public class MobileFlowInfoEntity
    {
        public string PlatFormType { get; set; }
        public string MobileType { get; set; }
        public List<FlowInfoEntity> FlowInfo { get; set; }
    }

    public class FlowInfoEntity
    {
        public string PackCode { get; set; }
        public int PackNum { get; set; }
        public double PackMoney { get; set; }

    }
}
