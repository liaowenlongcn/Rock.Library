using Rock.Library.Model;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Helper
{
    public class IISHelper
    {
        #region 站点操作
        /// <summary>
        /// 站点操作
        /// </summary> 
        public static bool SetWebsite(string siteName, MethodType method)
        {
            try
            {
                var webManager = new Microsoft.Web.Administration.ServerManager();
                var site = webManager.Sites[siteName];
                if (site == null)
                {
                    LogHelper.Info(new string[] { "Can't not find site:{0}", siteName });
                    return false;
                }
                switch (method)
                {
                    case MethodType.Start:
                        site.Start();
                        break;
                    case MethodType.Stop:
                        site.Stop();
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }

        #endregion 

        #region 应用池操作
        public static bool SetAppPool(string appPoolName, string method)
        {
            try
            {
                DirectoryEntry appPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                DirectoryEntry findPool = appPool.Children.Find(appPoolName, "IIsApplicationPool");
                findPool.Invoke(method, null);
                appPool.CommitChanges();
                appPool.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
        #endregion
    }
}
