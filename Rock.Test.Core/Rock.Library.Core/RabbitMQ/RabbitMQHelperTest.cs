using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Library.Core.Helper;
using Rock.Library.Core.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Test.Rock.Library.Core.RabbitMQ
{
    [TestClass]
    public class RabbitMQHelperTest
    {
        [TestMethod]
        public void ProductorTest()
        {
            RabbitMQHelper.Productor("hahah");
        }

        [TestMethod]
        public void LogHelperTest()
        {
            try
            {
                int y = 0;
                int i = 10 / y;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex); 
            }

            //LogHelper.Info("日志测试1q");
            //LogHelper.Debug("日志测试3");
            //LogHelper.Info("日志测试4");

            Thread.Sleep(2000);
        }
    }
}
