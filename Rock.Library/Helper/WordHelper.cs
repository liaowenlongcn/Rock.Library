using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Reflection;

namespace Rock.Library.Helper
{
    /// <summary>
    /// Word 帮助类
    /// </summary>
    public class WordHelper
    {
        #region 属性
        /// <summary>
        /// 数据源
        /// </summary>
        public object DataSource { get; set; }
        /// <summary>
        /// 模板路径
        /// </summary>
        public string TemplatePath { get; set; }
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get; set; }
        #endregion

        #region 生成文档
        public void Generate()
        {
            if (File.Exists(TargetPath)) File.Delete(TargetPath);
            File.Copy(TemplatePath, TargetPath, true);

            object oMissing = Missing.Value;
            Application app = new Application();
            app.Visible = false;
            object fileName = TargetPath;
            var doc = app.Documents.Open(ref fileName,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            //遍历书签                 
            foreach (Bookmark bookmark in doc.Bookmarks)
            {
                var paras = bookmark.Name.Split('_');
                if (paras.Length < 3) continue;
                var type = paras[0].ToLower();
                var name = paras[1].ToLower();
                var num = paras[2].ToLower();

                Range range = bookmark.Range;
                switch (type)
                {
                    case "table":
                        makeTable(doc, app, name, range, oMissing);
                        break;
                    default:
                        range.Text = GetValue(name);
                        break;
                }

                bookmark.Delete();
            }

            doc.Save();
            doc.Close();
            app.Quit();
        }
        #endregion

        #region 获取属性值
        public dynamic GetValue(object name, object obj = null)
        {
            try
            {
                BindingFlags flag = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance;
                if (obj == null)
                    return DataSource.GetType().GetProperty(name.ToString(), flag).GetValue(DataSource);
                else
                    return obj.GetType().GetProperty(name.ToString(), flag).GetValue(obj);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 生成表格
        private void makeTable(Document doc, Application app, object name, Range range, Object Nothing)
        {
            var dt = (System.Data.DataTable)GetValue(name);
            int tableColumn = dt.Columns.Count;
            int tableRow = dt.Rows.Count;
            Table table = doc.Tables.Add(range, tableRow, tableColumn, ref Nothing, ref Nothing);
            table.Borders.Enable = 1;

            for (int i = 0; i < tableRow; i++)
            {
                for (int j = 0; j < tableColumn; j++)
                {
                    Range cellRange = table.Cell(i + 1, j + 1).Range;
                    cellRange.Text = dt.Rows[i][j].ToString();
                    if (i == 0)
                    {
                        cellRange.Shading.ForegroundPatternColor = WdColor.wdColorGray20;
                        cellRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        cellRange.Font.Bold = 20;
                    }
                }
            }
        }
        #endregion
    }
}
