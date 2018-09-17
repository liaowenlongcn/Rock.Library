using Rock.Test.Rock.Work.Office.Model;
using Rock.Work.Office;
using System;
using System.Collections.Generic;
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
            person.Name = "张三";
            person.Age = "18";
            string templeteFilePath = @"D:\Download\temp.docx";
            string expFilePath = @"D:\Download\" + Guid.NewGuid().ToString() + ".docx";
            WordHelper.ExportWordByTemplete(person, templeteFilePath, expFilePath);
        }
    }
}
