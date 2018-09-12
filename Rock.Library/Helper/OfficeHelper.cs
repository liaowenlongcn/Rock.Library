using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Rock.Library.Helper
{
    public class OfficeHelper
    {

        #region Excel
        public static bool ExportExcel(string filePath, Dictionary<string, DataTable> tables)
        {
            var workbook = new HSSFWorkbook();

            foreach (var table in tables)
            {
                var sheet = workbook.CreateSheet(table.Key);
                var dt = table.Value;

                for (var rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                {
                    IRow row = sheet.CreateRow(rowIndex);
                    for (int columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
                    {
                        ICell cell = row.CreateCell(columnIndex, CellType.String);
                        cell.SetCellValue(dt.Rows[rowIndex][columnIndex].ToString());
                    } 
                }

                for (int columnIndex = 0; columnIndex <= dt.Columns.Count; columnIndex++)
                    sheet.AutoSizeColumn(columnIndex, true);          //自动列宽
            } 

            using (var fs = File.OpenWrite(filePath))
            {
                workbook.Write(fs);
            }

            return true;
        } 
        #endregion

    }
}
