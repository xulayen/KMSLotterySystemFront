// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Logger
// *文件名称：AppLog.cs
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-07-22   
//
// *创建标识：日志记录
// *创建描述：
//
// *修改信息：
// *修改备注：
// ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace KMSLotterySystemFront.Logger
{
    /// <summary>
    /// 日志处理
    /// </summary>
    public class AppLog
    {
        /// <summary>
        /// 静态类
        /// </summary>
        private AppLog() { }
        private const String LOG_REPOSITORY = "Default"; // this should likely be set in the web config.
        private static ILog m_log;

        #region 初始化日志系统
        /// <summary>
        /// 初始化日志系统
        /// 在系统运行开始初始化
        /// Global.asax Application_Start内
        /// </summary>
        public static void Init()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        #endregion

        #region 写入日志1
        /// <summary>
        /// 写入日志1
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        public static void Write(String message, LogMessageType messageType)
        {
            DoLog(message, messageType, null, Type.GetType("System.Object"));
        }
        #endregion

        #region 写入日志2
        /// <summary>
        /// 写入日志2
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="type"></param>
        public static void Write(String message, LogMessageType messageType, Type type)
        {
            DoLog(message, messageType, null, type);
        }
        #endregion

        #region 写入日志3
        /// <summary>
        /// 写入日志2
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        public static void Write(String message, LogMessageType messageType, Exception ex)
        {
            DoLog(message, messageType, ex, Type.GetType("System.Object"));
        }
        #endregion

        #region 写入日志4
        /// <summary>
        /// 写入日志4
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        /// <param name="type"></param>
        public static void Write(String message, LogMessageType messageType, Exception ex,
                                 Type type)
        {
            DoLog(message, messageType, ex, type);
        }
        #endregion

        #region 断言1
        /// <summary>
        /// 断言1
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">日志信息</param>
        public static void Assert(bool condition, String message)
        {
            Assert(condition, message, Type.GetType("System.Object"));
        }
        #endregion

        #region 断言1
        /// <summary>
        /// 断言2
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">日志信息</param>
        /// <param name="type">日志类型</param>
        public static void Assert(bool condition, String message, Type type)
        {
            if (condition == false)
                Write(message, LogMessageType.Info);
        }
        #endregion

        #region 保存日志
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        /// <param name="type">日志类型</param>
        private static void DoLog(String message, LogMessageType messageType, Exception ex,
                                  Type type)
        {
            m_log = LogManager.GetLogger(type);

            switch (messageType)
            {
                case LogMessageType.Debug:
                    AppLog.m_log.Debug(message, ex);
                    break;

                case LogMessageType.Info:
                    AppLog.m_log.Info(message, ex);
                    break;

                case LogMessageType.Warn:
                    AppLog.m_log.Warn(message, ex);
                    break;

                case LogMessageType.Error:
                    AppLog.m_log.Error(message, ex);
                    break;

                case LogMessageType.Fatal:
                    AppLog.m_log.Fatal(message, ex);
                    break;
            }
        }
        #endregion

        #region 日志类型
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogMessageType
        {
            /// <summary>
            /// 调试 级别1　数字越高级别越高
            /// </summary>
            Debug,
            /// <summary>
            /// 信息 级别2　数字越高级别越高
            /// </summary>
            Info,
            /// <summary>
            /// 警告 级别3　数字越高级别越高
            /// </summary>
            Warn,
            /// <summary>
            /// 错误 级别4　数字越高级别越高
            /// </summary>
            Error,
            /// <summary>
            /// 致命错误 级别5　数字越高级别越高 
            /// </summary>
            Fatal
        }
        #endregion
    }
}
