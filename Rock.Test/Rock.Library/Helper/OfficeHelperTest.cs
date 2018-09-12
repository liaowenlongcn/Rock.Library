using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Library.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Rock.Test.Rock.Library.Helper
{
    [TestClass]
    public class OfficeHelperTest
    {
        [TestMethod]
        public void TestExportExcel()
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("姓名");
            dt1.Columns.Add("性别");
            dt1.Columns.Add("年龄");

            DataRow dr1 = dt1.NewRow();
            dr1["姓名"] = "张三";
            dr1["性别"] = "男";
            dr1["年龄"] = "12";
            dt1.Rows.Add(dr1);

            DataRow dr2 = dt1.NewRow();
            dr2["姓名"] = "李四";
            dr2["性别"] = "女";
            dr2["年龄"] = "20";
            dt1.Rows.Add(dr2);

            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            dic.Add("教师名单", dt1);
            dic.Add("学生名单", dt1);
            OfficeHelper.ExportExcel(@"D:\Download\excel.xlsx", dic);
        }
    }
}
