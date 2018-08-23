using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Model
{
    public class DataBaseConnect
    { 
        /// <summary>
        /// 数据库地址
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
