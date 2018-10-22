using RabbitMQ.Client;
using Rock.Library.Core.Helper;
using System.Text;
using System.Configuration;

namespace Rock.Library.Core.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 帮助类
    /// </summary>
    public class RabbitMQHelper
    {
        #region 属性 
        /// <summary>
        /// RabbitMQ 服务器地址
        /// </summary>
        private static string rabbitMQHostName = ConfigHelper.GetSectionValue("RabbitMQHostName");
        /// <summary>
        /// RabbitMQ 用户名
        /// </summary>
        private static string rabbitMQUserName = ConfigHelper.GetSectionValue("RabbitMQUserName");
        /// <summary>
        /// RabbitMQ 密码
        /// </summary>
        private static string rabbitMQPassword = ConfigHelper.GetSectionValue("RabbitMQPassword");
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary> 
        public static void Productor(string message)
        {
            ConnectionFactory factory = GetConnectionFactory();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var exchangeName = "exchangeName";
                    string queueName = "alfresco";
                    string routeKey = "";
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false, null); //定义一个Direct类型交换机
                    channel.QueueDeclare(queueName, false, false, false, null); //定义一个队列 
                    channel.QueueBind(queueName, exchangeName, routeKey, null); //将队列绑定到交换机  

                    //发布消息
                    var sendBytes = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
                    LogHelper.Info("RabbitMQ 发送成功：" + message);
                }
            }
        }
        #endregion

        #region 获取连接工厂
        /// <summary>
        /// 获取连接工厂
        /// </summary> 
        private static ConnectionFactory GetConnectionFactory()
        {
            var RabbitMQHostName1 = rabbitMQHostName;
            return new ConnectionFactory
            {
                HostName = rabbitMQHostName, 
                UserName = rabbitMQUserName, 
                Password = rabbitMQPassword,
            };
        }
        #endregion
    }
}
