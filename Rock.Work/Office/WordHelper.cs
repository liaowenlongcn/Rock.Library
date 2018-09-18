using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Work.Office
{
    public class WordHelper
    {
        public static bool ExportWordByTemplete<T>(T model, string templeteFilePath, string targetFilePath)
        {
            try
            {
                Document doc = new Document();
                doc.LoadFromFile(templeteFilePath);
                BookmarksNavigator bookmarkNavigator = new BookmarksNavigator(doc);
                var bookmarks = doc.Bookmarks;

                while (bookmarks.Count > 0)
                {
                    var bookmark = bookmarks[0];
                    bookmarkNavigator.MoveToBookmark(bookmark.Name, true, true);    //移动到书签并清空书签原有内容
                    bookmarkNavigator.DeleteBookmarkContent(true);

                    var paras = bookmark.Name.Split('_');
                    if (paras.Length < 3)
                    {
                        doc.Bookmarks.Remove(bookmark);
                        continue;
                    }
                    var type = paras[0].ToLower();
                    var name = paras[1].ToLower();
                    var num = paras[2].ToLower();

                    switch (type)
                    {
                        case "table":
                            var dataTable = GetModelValue(name, model);
                            bookmarkNavigator.InsertTable(GetTable(doc, (DataTable)dataTable));
                            break;
                        default:
                            var text = GetModelValue(name, model);
                            if (text != null)
                                bookmarkNavigator.InsertText(text.ToString());
                            break;
                    }

                    doc.Bookmarks.Remove(bookmark);
                }
                doc.Properties.FormFieldShading = false;
                doc.SaveToFile(targetFilePath, FileFormat.Docx);
                doc.Close();

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }

        private static object GetModelValue(string fieldName, object model)
        {
            try
            {
                Type type = model.GetType();
                object value = type.GetProperty(fieldName).GetValue(model, null);
                return value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static Table GetTable(Document doc, DataTable dataTable)
        {
            var columnsCount = dataTable.Columns.Count;
            var rowsCount = dataTable.Rows.Count;

            Table table = new Table(doc);
            table.ResetCells(rowsCount, columnsCount);
            table.TableFormat.Borders.BorderType = BorderStyle.Hairline;
            table.TableFormat.Borders.Color = Color.Gray;


            // ***************** DataTable Header *************************
            TableRow row = table.Rows[0];
            row.IsHeader = true;
            row.Height = 20;
            row.HeightType = TableRowHeightType.Exactly;
            row.RowFormat.BackColor = Color.LightGray;
            for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
            {
                row.Cells[columnIndex].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p = row.Cells[columnIndex].AddParagraph();
                p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                TextRange txtRange = p.AppendText(dataTable.Rows[0][columnIndex].ToString());
                txtRange.CharacterFormat.Bold = true;
            }

            // ***************** DataTable Data *************************
            for (int rowIndex = 1; rowIndex < rowsCount; rowIndex++)
            {
                TableRow dataRow = table.Rows[rowIndex];
                dataRow.Height = 20;
                dataRow.HeightType = TableRowHeightType.Exactly;
                dataRow.RowFormat.BackColor = Color.Empty;
                for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    dataRow.Cells[columnIndex].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    dataRow.Cells[columnIndex].AddParagraph().AppendText(dataTable.Rows[rowIndex][columnIndex].ToString());
                }
            }


            return table;
        }
    }
}
