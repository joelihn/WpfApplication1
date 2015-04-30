#region Using References

using System;
using System.IO;
using log4net;
using log4net.Config;

#endregion

namespace WpfApplication1.LogModule
{
    /// <summary>
    ///   LogHelper类，用于日志文件的创建
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        ///   日志类的单例实例
        /// </summary>
        private static LogHelper _logHelper;

        /// <summary>
        ///   保证日志单例线程安全的锁
        /// </summary>
        private static readonly object Lockhelper = new object();

        /// <summary>
        ///   内部成员变量，主要用来书写日志信息
        /// </summary>
        private readonly ILog _log;

        private readonly ILog _logconsole;

        /// <summary>
        ///   创建Log实例，为了保证单例模式，将构造函数设为protected
        /// </summary>
        private LogHelper()
        {
            var fileInfo = new FileInfo("log4net.config");
            SetConfig(fileInfo);
            _log = LogManager.GetLogger("loginfo");
            _logconsole = LogManager.GetLogger("logtoconsole");
        }

        /// <summary>
        ///   返回单例日志实例
        /// </summary>
        /// <returns>返回单例日志实例类</returns>
        public static LogHelper GetInstance()
        {
            lock (Lockhelper)
            {
                if (_logHelper == null)
                {
                    _logHelper = new LogHelper();
                }
            }
            return _logHelper;
        }

        /// <summary>
        ///   写错误日志
        /// </summary>
        /// <param name = "errorLog">错误日志信息</param>
        public void WriteErrorLog(string errorLog)
        {
            _log.Error(errorLog);
        }

        public void WriteErrorLogAndConsole(string function, Exception ee)
        {
            WriteErrorLog("In " + function + ": " + ee.Message + "\n" + ee.StackTrace);
            LogToConsole("In " + function + ": " + ee.Message);
        }

        /// <summary>
        ///   重载错误日志书写方法，可以添加异常类
        /// </summary>
        /// <param name = "errLog">错误日志信息</param>
        /// <param name = "exception">异常类</param>
        public void WriteErrorLog(string errLog, Exception exception)
        {
            _log.Error(errLog, exception);
        }

        /// <summary>
        ///   写调试日志
        /// </summary>
        /// <param name = "debugLog">调试日志信息</param>
        public void WriteDebugLog(string debugLog)
        {
            _log.Debug(debugLog);
        }

        /// <summary>
        ///   重载调试日志书写方法，可以添加异常类
        /// </summary>
        /// <param name = "debugLog">调试日志信息</param>
        /// <param name = "exception">异常类</param>
        public void WriteDebugLog(string debugLog, Exception exception)
        {
            _log.Debug(debugLog, exception);
        }

        /// <summary>
        ///   写一般信息日志
        /// </summary>
        /// <param name = "infoLog">日常日志信息</param>
        public void WriteInfoLog(string infoLog)
        {
            _log.Info(infoLog);
        }

        /// <summary>
        ///   重载一般信息日志书写，可以添加异常类
        /// </summary>
        /// <param name = "infoLog">日常日志信息</param>
        /// <param name = "exception">异常类</param>
        public void WriteInfoLog(string infoLog, Exception exception)
        {
            _log.Info(infoLog, exception);
        }

        public void WriteInfoConsole(object infoLog)
        {
            _logconsole.Info(infoLog);
        }

        public void LogToConsole(string message)
        {
            WriteInfoConsole(message);
        }

        /// <summary>
        ///   设置外部设置文件
        /// </summary>
        /// <param name = "configFile">设置文件所在的路径</param>
        private static void SetConfig(FileInfo configFile)
        {
            //log4net.Config.DOMConfigurator.Configure(configFile);
            XmlConfigurator.Configure(configFile);
        }
    }
}