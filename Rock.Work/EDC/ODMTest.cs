using Rock.Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Library;

namespace Rock.Work.EDC
{
    public class ODMTest
    {
        public void ImportDataTest()
        {
            var filePath = @"D:\Download\odm.xml";
            FileStream fs = new FileStream(filePath, FileMode.Open);//可以是其他重载方法 
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close(); 

            string url = "http://localhost:9735/api/ODMProvider/ImportData";
            RequestBO bo = new RequestBO
            {
                Filename = "test",
                Filedata = byData
            };
            string data = bo.ToJson();
            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("ApiHeader", "{\"SessionKey\":\"\",\"LangKey\":\"chs\",\"Ip\":\"\",\"AppKey\":\"U27BM34YCZWPWAG\"}");
            HttpHelper.Post(url,data,headers); 
        }

        public class RequestBO {
            public string Filename { get; set; }
            public byte[] Filedata { get; set; }
        }
    }
}
