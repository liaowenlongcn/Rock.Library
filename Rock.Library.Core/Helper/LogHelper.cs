using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Core.Helper
{
    public static class LogHelper
    {
        #region 属性
        private static readonly ILog log = GetLog();
        private static ILog GetLog()
        {
            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("Log/log.config"));
            return LogManager.GetLogger(repository.Name, "NETCorelog4net");
        }
        #endregion

        #region 信息日志
        public static Task Info(string message)
        {
            return Task.Factory.StartNew(() =>
            {
                if (log.IsInfoEnabled)
                {
                    log.Info(message + "\r\n");
                }
            });
        }
        #endregion

        #region 调试日志 
        public static Task Debug(string message)
        {
            return Task.Factory.StartNew(() =>
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(message + "\r\n");
                }
            });
        }
        #endregion

        #region 错误日志 
        public static Task Error(Exception ex)
        {
            return Task.Factory.StartNew(() =>
            {
                if (log.IsErrorEnabled)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("");
                    sb.AppendLine("异常信息：" + ex.Message);
                    sb.AppendLine("异常对象：" + ex.Source);
                    sb.AppendLine("调用堆栈：" + "\r\n" + ex.StackTrace);
                    sb.AppendLine("触发方法：" + ex.TargetSite);

                    log.Error(sb.ToString());
                }
            });
        }

        #endregion
    }
}
