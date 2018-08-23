using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Helper.Http
{
    public class HttpResponse
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public string Result { get; set; }
    }
}
