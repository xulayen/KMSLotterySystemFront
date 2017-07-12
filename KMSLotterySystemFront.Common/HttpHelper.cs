using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace KMSLotterySystemFront.Common
{
    public class HttpHelper
    {

        public static string SendRequest(string serviceUrl, string method, Dictionary<string, object> dicParam)
        {
            try
            {
                string param = string.Empty;
                if (dicParam != null && dicParam.Count > 0)
                {
                    foreach (var item in dicParam)
                    {
                        param += item.Key + "=" + item.Value + "&";
                    }
                }

                Encoding myEncode = Encoding.GetEncoding("UTF-8");
                byte[] postBytes = Encoding.UTF8.GetBytes(param);

                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(serviceUrl);
                req.Method = method;
                req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                req.ContentLength = postBytes.Length;

                try
                {
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(postBytes, 0, postBytes.Length);
                    }
                    using (WebResponse res = req.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream(), myEncode))
                        {
                            string strResult = sr.ReadToEnd();
                            return strResult;
                        }
                    }
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                KMSLotterySystemFront.Logger.AppLog.Write("HttpHelper:SendRequest:" + ex.Message + "--" + ex.Source + "--" + ex.StackTrace + "--" + ex.TargetSite, KMSLotterySystemFront.Logger.AppLog.LogMessageType.Fatal);
                return "无法连接到服务器\r\n错误信息：" + ex.Message;
            }
        }
    }


    public class HttpMethod
    {
        public const string Post = "POST";
        public const string Get = "GET";
    }

    public enum StatusCode
    {
         
        /// <summary>
        /// 等效于 HTTP 状态 202。 Accepted 指示请求已被接受做进一步处理。
        /// </summary>
        Accepted = 202,

        /// <summary>
        /// 等效于 HTTP 状态 300。 Ambiguous 指示请求的信息有多种表示形式。默认操作是将此状态视为重定向，并遵循与此响应关联的 Location 标头的内容。
        /// </summary>
        Ambiguous = 300,

        /// <summary>
        /// 等效于 HTTP 状态 502。 BadGateway 指示中间代理服务器从另一代理或原始服务器接收到错误响应。
        /// </summary>
        BadGateway = 502,

        /// <summary>
        /// 等效于 HTTP 状态 400。 BadRequest 指示服务器未能识别请求。如果没有其他适用的错误，或者不知道准确的错误或错误没有自己的错误代码，则发送 BadRequest。
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 等效于 HTTP 状态 409。 Conflict 指示由于服务器上的冲突而未能执行请求。
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// 等效于 HTTP 状态 100。 Continue 指示客户端可能继续其请求。
        /// </summary>
        Continue = 100,

        /// <summary>
        /// 等效于 HTTP 状态 201。 Created 指示请求导致在响应被发送前创建新资源。
        /// </summary>
        Created = 201,

        /// <summary>
        /// 等效于 HTTP 状态 417。 ExpectationFailed 指示服务器未能符合 Expect 头中给定的预期值。
        /// </summary>
        ExpectationFailed = 417,

        /// <summary>
        /// 等效于 HTTP 状态 403。 Forbidden 指示服务器拒绝满足请求。
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 等效于 HTTP 状态 302。 Found 指示请求的信息位于 Location 头中指定的 URI 处。接收到此状态时的默认操作为遵循与响应关联的 Location 头。原始请求方法为 POST 时，重定向的请求将使用 GET 方法。
        /// </summary>
        Found = 302,

        /// <summary>
        /// 等效于 HTTP 状态 504。 GatewayTimeout 指示中间代理服务器在等待来自另一个代理或原始服务器的响应时已超时。
        /// </summary>
        GatewayTimeout = 504,

        /// <summary>
        /// 等效于 HTTP 状态 410。 Gone 指示请求的资源不再可用。
        /// </summary>
        Gone = 410,

        /// <summary>
        /// 等效于 HTTP 状态 505。 HttpVersionNotSupported 指示服务器不支持请求的 HTTP 版本。
        /// </summary>
        HttpVersionNotSupported = 505,

        /// <summary>
        /// 等效于 HTTP 状态 500。 InternalServerError 指示服务器上发生了一般错误。
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// 等效于 HTTP 状态 411。 LengthRequired 指示缺少必需的 Content-length 头。
        /// </summary>
        LengthRequired = 411,

        /// <summary>
        /// 等效于 HTTP 状态 405。 MethodNotAllowed 指示请求的资源上不允许请求方法（POST 或 GET）。
        /// </summary>
        MethodNotAllowed = 405,

        /// <summary>
        /// 等效于 HTTP 状态 301。 Moved 指示请求的信息已移到 Location 头中指定的 URI 处。接收到此状态时的默认操作为遵循与响应关联的 Location 头。原始请求方法为 POST 时，重定向的请求将使用 GET 方法。
        /// </summary>
        Moved = 301,

        /// <summary>
        /// 等效于 HTTP 状态 301。 MovedPermanently 指示请求的信息已移到 Location 头中指定的 URI 处。接收到此状态时的默认操作为遵循与响应关联的 Location 头。
        /// </summary>
        MovedPermanently = 301,

        /// <summary>
        /// 等效于 HTTP 状态 300。 MultipleChoices 指示请求的信息有多种表示形式。默认操作是将此状态视为重定向，并遵循与此响应关联的 Location 标头的内容。
        /// </summary>
        MultipleChoices = 300,

        /// <summary>
        /// 等效于 HTTP 状态 204。 NoContent 指示已成功处理请求并且响应已被设定为无内容。
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// 等效于 HTTP 状态 203。 NonAuthoritativeInformation 指示返回的元信息来自缓存副本而不是原始服务器，因此可能不正确。
        /// </summary>
        NonAuthoritativeInformation = 203,

        /// <summary>
        /// 等效于 HTTP 状态 406。 NotAcceptable 指示客户端已用 Accept 头指示将不接受资源的任何可用表示形式。
        /// </summary>
        NotAcceptable = 406,

        /// <summary>
        /// 等效于 HTTP 状态 404。 NotFound 指示请求的资源不在服务器上。
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 等效于 HTTP 状态 501。 NotImplemented 指示服务器不支持请求的函数。
        /// </summary>
        NotImplemented = 501,

        /// <summary>
        /// 等效于 HTTP 状态 304。 NotModified 指示客户端的缓存副本是最新的。未传输此资源的内容。
        /// </summary>
        NotModified = 304,

        /// <summary>
        /// 等效于 HTTP 状态 200。 OK 指示请求成功，且请求的信息包含在响应中。这是最常接收的状态代码。
        /// </summary>
        OK = 200,

        /// <summary>
        /// 等效于 HTTP 状态 206。 PartialContent 指示响应是包括字节范围的 GET 请求所请求的部分响应。
        /// </summary>
        PartialContent = 206,

        /// <summary>
        /// 等效于 HTTP 状态 402。保留 PaymentRequired 以供将来使用。
        /// </summary>
        PaymentRequired = 402,

        /// <summary>
        /// 等效于 HTTP 状态 412。 PreconditionFailed 指示为此请求设置的条件失败，且无法执行此请求。条件是用条件请求标头（如 If-Match、If-None-Match 或 If-Unmodified-Since）设置的。
        /// </summary>
        PreconditionFailed = 412,

        /// <summary>
        /// 等效于 HTTP 状态 407。 ProxyAuthenticationRequired 指示请求的代理要求身份验证。Proxy-authenticate 头包含如何执行身份验证的详细信息。
        /// </summary>
        ProxyAuthenticationRequired = 407,

        /// <summary>
        /// 等效于 HTTP 状态 302。 Redirect 指示请求的信息位于 Location 头中指定的 URI 处。接收到此状态时的默认操作为遵循与响应关联的 Location 头。原始请求方法为 POST 时，重定向的请求将使用 GET 方法。
        /// </summary>
        Redirect = 302,

        /// <summary>
        /// 等效于 HTTP 状态 307。 RedirectKeepVerb 指示请求信息位于 Location 头中指定的 URI 处。接收到此状态时的默认操作为遵循与响应关联的 Location 头。原始请求方法为 POST 时，重定向的请求还将使用 POST 方法。
        /// </summary>
        RedirectKeepVerb = 307,

        /// <summary>
        /// 等效于 HTTP 状态 303。作为 POST 的结果，RedirectMethod 将客户端自动重定向到 Location 头中指定的 URI。用 GET 生成对 Location 标头所指定的资源的请求。
        /// </summary>
        RedirectMethod = 303,

        /// <summary>
        /// 等效于 HTTP 状态 416。 RequestedRangeNotSatisfiable 指示无法返回从资源请求的数据范围，因为范围的开头在资源的开头之前，或因为范围的结尾在资源的结尾之后。
        /// </summary>
        RequestedRangeNotSatisfiable = 416,

        /// <summary>
        /// 等效于 HTTP 状态 413。 RequestEntityTooLarge 指示请求太大，服务器无法处理。
        /// </summary>
        RequestEntityTooLarge = 413,

        /// <summary>
        /// 等效于 HTTP 状态 408。 RequestTimeout 指示客户端没有在服务器期望请求的时间内发送请求。
        /// </summary>
        RequestTimeout = 408,

        /// <summary>
        /// 等效于 HTTP 状态 414。 RequestUriTooLong 指示 URI 太长。
        /// </summary>
        RequestUriTooLong = 414,

        /// <summary>
        /// 等效于 HTTP 状态 205。 ResetContent 指示客户端应重置（或重新加载）当前资源。
        /// </summary>
        ResetContent = 205,

        /// <summary>
        /// 等效于 HTTP 状态 303。作为 POST 的结果，SeeOther 将客户端自动重定向到 Location 头中指定的 URI。用 GET 生成对 Location 标头所指定的资源的请求。
        /// </summary>
        SeeOther = 303,

        /// <summary>
        /// 等效于 HTTP 状态 503。 ServiceUnavailable 指示服务器暂时不可用，通常是由于过多加载或维护。
        /// </summary>
        ServiceUnavailable = 503,

        /// <summary>
        /// 等效于 HTTP 状态 101。 SwitchingProtocols 指示正在更改协议版本或协议。
        /// </summary>
        SwitchingProtocols = 101,

        /// <summary>
        /// 等效于 HTTP 状态 307。 TemporaryRedirect 指示请求信息位于 Location 头中指定的 URI 处。接收到此状态时的默认操作为遵循与响应关联的 Location 头。原始请求方法为 POST 时，重定向的请求还将使用 POST 方法。
        /// </summary>
        TemporaryRedirect = 307,

        /// <summary>
        /// 等效于 HTTP 状态 401。 Unauthorized 指示请求的资源要求身份验证。WWW-Authenticate 头包含如何执行身份验证的详细信息。
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 等效于 HTTP 状态 415。 UnsupportedMediaType 指示请求是不支持的类型。
        /// </summary>
        UnsupportedMediaType = 415,

        /// <summary>
        /// 等效于 HTTP 状态 306。 Unused 是未完全指定的 HTTP/1.1 规范的建议扩展。
        /// </summary>
        Unused = 306,

        /// <summary>
        /// 等效于 HTTP 状态 426。 UpgradeRequired 指示客户端应切换为诸如 TLS/1.0 之类的其他协议。
        /// </summary>
        UpgradeRequired = 426,

        /// <summary>
        /// 等效于 HTTP 状态 305。 UseProxy 指示请求应使用位于 Location 头中指定的 URI 的代理服务器。
        /// </summary>
        UseProxy = 305,

    }
}
