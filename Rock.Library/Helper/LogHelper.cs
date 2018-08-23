using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log/log.config", ConfigFileExtension = "config", Watch = true)]

namespace Rock.Library.Helper
{
    public static class LogHelper
    { 
        #region 信息日志

        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        public static Task Info(params string[][] values)
        {
            return Task.Factory.StartNew(() =>
            {
                if (loginfo.IsInfoEnabled)
                {
                    if (values != null && values.Length > 0)
                    {
                        loginfo.Info("\r\n" + string.Join("\r\n", values.Select(p => "【" + p[0] + "】:" + p[1])));
                    }
                }
            });
        }
        #endregion

        #region 调试日志
        private static readonly log4net.ILog logdebug = log4net.LogManager.GetLogger("logdebug");

        public static Task Debug(params string[][] values)
        {
            return Task.Factory.StartNew(() =>
            {
                if (loginfo.IsDebugEnabled)
                {
                    if (values != null && values.Length > 0)
                    {
                        loginfo.Debug("\r\n" + string.Join("\r\n", values.Select(p => "【" + p[0] + "】:" + p[1])));
                    }
                }
            });
        }
        #endregion

        #region 错误日志

        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        public static Task Error(Exception ex)
        {
            return Task.Factory.StartNew(() =>
            {
                if (loginfo.IsErrorEnabled)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("");
                    sb.AppendLine("异常信息：" + ex.Message);
                    sb.AppendLine("异常对象：" + ex.Source);
                    sb.AppendLine("调用堆栈：\n" + ex.StackTrace.Trim());
                    sb.AppendLine("触发方法：" + ex.TargetSite);

                    loginfo.Error(sb.ToString());
                }
            });
        }
         
        #endregion
    }
}
