using ClosedXML.Excel;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Interfaces;
using System.Data;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Services
{
    public class ProductExportationService : IProductExportationService
    {
        public byte[] ExportToExcel(DataTable dataTable)
        {
            DataTable copy = new() { TableName = "Productos" };

            foreach (DataColumn col in dataTable.Columns)
                copy.Columns.Add(col.ColumnName, typeof(string));

            foreach (DataRow row in dataTable.Rows)
                copy.Rows.Add(row.ItemArray);

            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add(copy);
            worksheet.ColumnsUsed().AdjustToContents();

            using MemoryStream ms = new();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
