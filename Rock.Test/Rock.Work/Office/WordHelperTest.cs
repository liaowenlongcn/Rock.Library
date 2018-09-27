using Rock.Test.Rock.Work.Office.Model;
using Rock.Work.Office;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Test.Rock.Work.Office
{
    public class WordHelperTest
    {
        public void ExportWordByTempleteTest()
        {
            Person person = new Person();
            person.name = "张三";
            person.age = "18";
            person.dt1 = GetDataTable();
            string templeteFilePath = @"D:\Download\temp.docx";
            string expFilePath = @"D:\Download\" + Guid.NewGuid().ToString() + ".docx";
            WordHelper.ExportWordByTemplete(person, templeteFilePath, expFilePath);
        }


        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("time");
            dt.Columns.Add("company");
            dt.Columns.Add("position");

            DataRow dr = dt.NewRow();
            dr["time"] = "时间";
            dr["company"] = "公司";
            dr["position"] = "职位";
            dt.Rows.Add(dr);

            DataRow dr1 = dt.NewRow();
            dr1["time"] = "2017-09-08";
            dr1["company"] = "百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度百度";
            dr1["position"] = "程序员";
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["time"] = "2018-05-18";
            dr2["company"] = "腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯腾讯";
            dr2["position"] = "软件工程师";

            dt.Rows.Add(dr2);

            return dt;
        }
    }
}
