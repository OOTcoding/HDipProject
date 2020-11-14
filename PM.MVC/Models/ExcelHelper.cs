using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace PM.MVC.Models
{
    public static class ExcelHelper
    {
        public static byte[] GetContentAsBytes(params DataTable[] tables)
        {
            using (var workbook = new XLWorkbook())
            {
                foreach (DataTable dataTable in tables)
                {
                    IXLWorksheet ws = workbook.Worksheets.Add(dataTable);
                    ws.RangeUsed().ColumnsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.ColumnsUsed().AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}