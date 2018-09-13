using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Work.Office;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Test.Rock.Work.Office
{
    [TestClass]
    public class WordHelperTest
    {
        [TestMethod]
        public void ExportWordByTempleteTest()
        {
            Person person = new Person();
            person.Name = "张三";
            person.Age = "18";

            var templeteFilePath = @"D:\Download\temp.docx";
            var expFilePath = @"D:\Download\" + Guid.NewGuid().ToString() + ".docx";
            WordHelper.ExportWordByTemplete(person, templeteFilePath, expFilePath);
        }

    }
}
