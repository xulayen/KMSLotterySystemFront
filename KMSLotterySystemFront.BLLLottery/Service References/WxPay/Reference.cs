﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace KMSLotterySystemFront.BLLLottery.WxPay {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WxPay.WxPayServiceSoap")]
    public interface WxPayServiceSoap {
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 strIn 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetRedPack", ReplyAction="*")]
        KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackResponse GetRedPack(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequest request);
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 facId 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/RedPackSendInt", ReplyAction="*")]
        KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntResponse RedPackSendInt(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequest request);
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 facId 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/RedPackSendOut", ReplyAction="*")]
        KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutResponse RedPackSendOut(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequest request);
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 facId 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetRedPackInfo", ReplyAction="*")]
        KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoResponse GetRedPackInfo(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequest request);
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 facId 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/QYPaySendInt", ReplyAction="*")]
        KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntResponse QYPaySendInt(KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetRedPackRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetRedPack", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequestBody Body;
        
        public GetRedPackRequest() {
        }
        
        public GetRedPackRequest(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetRedPackRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string strIn;
        
        public GetRedPackRequestBody() {
        }
        
        public GetRedPackRequestBody(string strIn) {
            this.strIn = strIn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetRedPackResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetRedPackResponse", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackResponseBody Body;
        
        public GetRedPackResponse() {
        }
        
        public GetRedPackResponse(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetRedPackResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string GetRedPackResult;
        
        public GetRedPackResponseBody() {
        }
        
        public GetRedPackResponseBody(string GetRedPackResult) {
            this.GetRedPackResult = GetRedPackResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RedPackSendIntRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RedPackSendInt", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequestBody Body;
        
        public RedPackSendIntRequest() {
        }
        
        public RedPackSendIntRequest(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RedPackSendIntRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string facId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string ccnactivityid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string openId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string code;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string money;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string hbtype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string total_num;
        
        public RedPackSendIntRequestBody() {
        }
        
        public RedPackSendIntRequestBody(string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num) {
            this.facId = facId;
            this.ccnactivityid = ccnactivityid;
            this.openId = openId;
            this.code = code;
            this.money = money;
            this.hbtype = hbtype;
            this.total_num = total_num;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RedPackSendIntResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RedPackSendIntResponse", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntResponseBody Body;
        
        public RedPackSendIntResponse() {
        }
        
        public RedPackSendIntResponse(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RedPackSendIntResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public bool RedPackSendIntResult;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string systemState;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string msgresult;
        
        public RedPackSendIntResponseBody() {
        }
        
        public RedPackSendIntResponseBody(bool RedPackSendIntResult, string systemState, string msgresult) {
            this.RedPackSendIntResult = RedPackSendIntResult;
            this.systemState = systemState;
            this.msgresult = msgresult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RedPackSendOutRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RedPackSendOut", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequestBody Body;
        
        public RedPackSendOutRequest() {
        }
        
        public RedPackSendOutRequest(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RedPackSendOutRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string facId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string ccnactivityid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string openId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string money;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string lid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string codeId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string hbtype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string total_num;
        
        public RedPackSendOutRequestBody() {
        }
        
        public RedPackSendOutRequestBody(string facId, string ccnactivityid, string openId, string money, string lid, string codeId, string hbtype, string total_num) {
            this.facId = facId;
            this.ccnactivityid = ccnactivityid;
            this.openId = openId;
            this.money = money;
            this.lid = lid;
            this.codeId = codeId;
            this.hbtype = hbtype;
            this.total_num = total_num;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RedPackSendOutResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RedPackSendOutResponse", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutResponseBody Body;
        
        public RedPackSendOutResponse() {
        }
        
        public RedPackSendOutResponse(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RedPackSendOutResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public bool RedPackSendOutResult;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string systemState;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string msgresult;
        
        public RedPackSendOutResponseBody() {
        }
        
        public RedPackSendOutResponseBody(bool RedPackSendOutResult, string systemState, string msgresult) {
            this.RedPackSendOutResult = RedPackSendOutResult;
            this.systemState = systemState;
            this.msgresult = msgresult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetRedPackInfoRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetRedPackInfo", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequestBody Body;
        
        public GetRedPackInfoRequest() {
        }
        
        public GetRedPackInfoRequest(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetRedPackInfoRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string facId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string ccnactivityid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string mch_billno;
        
        public GetRedPackInfoRequestBody() {
        }
        
        public GetRedPackInfoRequestBody(string facId, string ccnactivityid, string mch_billno) {
            this.facId = facId;
            this.ccnactivityid = ccnactivityid;
            this.mch_billno = mch_billno;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetRedPackInfoResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetRedPackInfoResponse", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoResponseBody Body;
        
        public GetRedPackInfoResponse() {
        }
        
        public GetRedPackInfoResponse(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetRedPackInfoResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public bool GetRedPackInfoResult;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string systemState;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string msgresult;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string resultxml;
        
        public GetRedPackInfoResponseBody() {
        }
        
        public GetRedPackInfoResponseBody(bool GetRedPackInfoResult, string systemState, string msgresult, string resultxml) {
            this.GetRedPackInfoResult = GetRedPackInfoResult;
            this.systemState = systemState;
            this.msgresult = msgresult;
            this.resultxml = resultxml;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class QYPaySendIntRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="QYPaySendInt", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequestBody Body;
        
        public QYPaySendIntRequest() {
        }
        
        public QYPaySendIntRequest(KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class QYPaySendIntRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string facId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string ccnactivityid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string openId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string checkreceivenametype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string receivename;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string money;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string desc;
        
        public QYPaySendIntRequestBody() {
        }
        
        public QYPaySendIntRequestBody(string facId, string ccnactivityid, string openId, string checkreceivenametype, string receivename, string money, string desc) {
            this.facId = facId;
            this.ccnactivityid = ccnactivityid;
            this.openId = openId;
            this.checkreceivenametype = checkreceivenametype;
            this.receivename = receivename;
            this.money = money;
            this.desc = desc;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class QYPaySendIntResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="QYPaySendIntResponse", Namespace="http://tempuri.org/", Order=0)]
        public KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntResponseBody Body;
        
        public QYPaySendIntResponse() {
        }
        
        public QYPaySendIntResponse(KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class QYPaySendIntResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public bool QYPaySendIntResult;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string systemState;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string msgresult;
        
        public QYPaySendIntResponseBody() {
        }
        
        public QYPaySendIntResponseBody(bool QYPaySendIntResult, string systemState, string msgresult) {
            this.QYPaySendIntResult = QYPaySendIntResult;
            this.systemState = systemState;
            this.msgresult = msgresult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WxPayServiceSoapChannel : KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WxPayServiceSoapClient : System.ServiceModel.ClientBase<KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap>, KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap {
        
        public WxPayServiceSoapClient() {
        }
        
        public WxPayServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WxPayServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WxPayServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WxPayServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackResponse KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap.GetRedPack(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequest request) {
            return base.Channel.GetRedPack(request);
        }
        
        public string GetRedPack(string strIn) {
            KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequest inValue = new KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequest();
            inValue.Body = new KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackRequestBody();
            inValue.Body.strIn = strIn;
            KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackResponse retVal = ((KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap)(this)).GetRedPack(inValue);
            return retVal.Body.GetRedPackResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntResponse KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap.RedPackSendInt(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequest request) {
            return base.Channel.RedPackSendInt(request);
        }
        
        public bool RedPackSendInt(string facId, string ccnactivityid, string openId, string code, string money, string hbtype, string total_num, out string systemState, out string msgresult) {
            KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequest inValue = new KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequest();
            inValue.Body = new KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntRequestBody();
            inValue.Body.facId = facId;
            inValue.Body.ccnactivityid = ccnactivityid;
            inValue.Body.openId = openId;
            inValue.Body.code = code;
            inValue.Body.money = money;
            inValue.Body.hbtype = hbtype;
            inValue.Body.total_num = total_num;
            KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendIntResponse retVal = ((KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap)(this)).RedPackSendInt(inValue);
            systemState = retVal.Body.systemState;
            msgresult = retVal.Body.msgresult;
            return retVal.Body.RedPackSendIntResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutResponse KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap.RedPackSendOut(KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequest request) {
            return base.Channel.RedPackSendOut(request);
        }
        
        public bool RedPackSendOut(string facId, string ccnactivityid, string openId, string money, string lid, string codeId, string hbtype, string total_num, out string systemState, out string msgresult) {
            KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequest inValue = new KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequest();
            inValue.Body = new KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutRequestBody();
            inValue.Body.facId = facId;
            inValue.Body.ccnactivityid = ccnactivityid;
            inValue.Body.openId = openId;
            inValue.Body.money = money;
            inValue.Body.lid = lid;
            inValue.Body.codeId = codeId;
            inValue.Body.hbtype = hbtype;
            inValue.Body.total_num = total_num;
            KMSLotterySystemFront.BLLLottery.WxPay.RedPackSendOutResponse retVal = ((KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap)(this)).RedPackSendOut(inValue);
            systemState = retVal.Body.systemState;
            msgresult = retVal.Body.msgresult;
            return retVal.Body.RedPackSendOutResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoResponse KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap.GetRedPackInfo(KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequest request) {
            return base.Channel.GetRedPackInfo(request);
        }
        
        public bool GetRedPackInfo(string facId, string ccnactivityid, string mch_billno, out string systemState, out string msgresult, out string resultxml) {
            KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequest inValue = new KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequest();
            inValue.Body = new KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoRequestBody();
            inValue.Body.facId = facId;
            inValue.Body.ccnactivityid = ccnactivityid;
            inValue.Body.mch_billno = mch_billno;
            KMSLotterySystemFront.BLLLottery.WxPay.GetRedPackInfoResponse retVal = ((KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap)(this)).GetRedPackInfo(inValue);
            systemState = retVal.Body.systemState;
            msgresult = retVal.Body.msgresult;
            resultxml = retVal.Body.resultxml;
            return retVal.Body.GetRedPackInfoResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntResponse KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap.QYPaySendInt(KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequest request) {
            return base.Channel.QYPaySendInt(request);
        }
        
        public bool QYPaySendInt(string facId, string ccnactivityid, string openId, string checkreceivenametype, string receivename, string money, string desc, out string systemState, out string msgresult) {
            KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequest inValue = new KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequest();
            inValue.Body = new KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntRequestBody();
            inValue.Body.facId = facId;
            inValue.Body.ccnactivityid = ccnactivityid;
            inValue.Body.openId = openId;
            inValue.Body.checkreceivenametype = checkreceivenametype;
            inValue.Body.receivename = receivename;
            inValue.Body.money = money;
            inValue.Body.desc = desc;
            KMSLotterySystemFront.BLLLottery.WxPay.QYPaySendIntResponse retVal = ((KMSLotterySystemFront.BLLLottery.WxPay.WxPayServiceSoap)(this)).QYPaySendInt(inValue);
            systemState = retVal.Body.systemState;
            msgresult = retVal.Body.msgresult;
            return retVal.Body.QYPaySendIntResult;
        }
    }
}