using ClosedXML.Excel;
using IronPdf;
using CsvHelper;
using FiyiPOS.Areas.System.FailureBack.Entities;
using FiyiPOS.Areas.System.FailureBack.Interfaces;
using System.Data;
using System.Globalization;

namespace FiyiPOS.Areas.System.FailureBack.Services
{
    public class FailureExportationService : IFailureExportationService
    {
        public void ExportToExcel(string path, DataTable dataTableFailure)
        {
            using XLWorkbook XLWorkbook = new();

            DataTable DataTableFailureOriginal = new()
            {
                TableName = "Failure"
            };

            //We define another DataTable DataTableFailureCopy to avoid issue related to DateTime conversion
            DataTable DataTableFailureCopy = new()
            {
                TableName = "Failure"
            };

            #region Define columns for DataTableFailureCopy
            DataColumn DataTableColumnFailureIdForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "FailureId"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnFailureIdForDataTableFailureCopy);

            DataColumn DataTableColumnDateTimeCreationForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "DateTimeCreation"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnDateTimeCreationForDataTableFailureCopy);

            DataColumn DataTableColumnDateTimeLastModificationForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "DateTimeLastModification"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnDateTimeLastModificationForDataTableFailureCopy);

            DataColumn DataTableColumnUserCreationIdForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "UserCreationId"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnUserCreationIdForDataTableFailureCopy);

            DataColumn DataTableColumnUserLastModificationIdForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "UserLastModificationId"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnUserLastModificationIdForDataTableFailureCopy);

            DataColumn DataTableColumnActiveForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "Active"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnActiveForDataTableFailureCopy);

            DataColumn DataTableColumnMessageForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "Message"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnMessageForDataTableFailureCopy);

            DataColumn DataTableColumnEmergencyLevelForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "EmergencyLevel"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnEmergencyLevelForDataTableFailureCopy);

            DataColumn DataTableColumnStackTraceForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "StackTrace"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnStackTraceForDataTableFailureCopy);

            DataColumn DataTableColumnSourceForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "Source"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnSourceForDataTableFailureCopy);

            DataColumn DataTableColumnCommentForDataTableFailureCopy = new()
            {
                DataType = typeof(string),
                ColumnName = "Comment"
            };
            DataTableFailureCopy.Columns.Add(DataTableColumnCommentForDataTableFailureCopy);

            
            #endregion

            DataTableFailureOriginal = dataTableFailure;

            foreach (DataRow DataRow in DataTableFailureOriginal.Rows)
            {
                DataTableFailureCopy.Rows.Add(DataRow.ItemArray);
            }

            IXLWorksheet XLWorksheet = XLWorkbook.Worksheets.Add(DataTableFailureCopy);

            XLWorksheet.ColumnsUsed().AdjustToContents();

            XLWorkbook.SaveAs(path);
        }

        public void ExportToCSV(string path, List<Failure> ListFailure)
        {
            StreamWriter StreamWriter = new(path);

            using CsvWriter CsvWriter = new(StreamWriter, CultureInfo.InvariantCulture);

            CsvWriter.WriteRecords(ListFailure);
        }

        public void ExportToPDF(string path, List<Failure> ListFailure)
        {
            string ProjectName = "FiyiPOS";

            string Table = "Failure";

            ChromePdfRenderer ChromePdfRenderer = new();

            string RowsAsHTML = "";

            foreach (Failure? Failure in ListFailure)
            {
                RowsAsHTML += $@"
<tr>
    
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.FailureId}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.DateTimeCreation}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.DateTimeLastModification}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.UserCreationId}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.UserLastModificationId}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.Active}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.Message}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.EmergencyLevel}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.StackTrace}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.Source}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
    <td align=""left"" valign=""top"">
        <div style=""height: 12px; line-height: 12px; font-size: 10px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px;"">{Failure.Comment}</span>
        </font>
        <div style=""height: 40px; line-height: 40px; font-size: 38px;"">&nbsp;</div>
    </td>
</tr>";
            }

            string FullHTMLContent = $@"
<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
    <tr>
    <td align=""left"" valign=""top"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#1a1a1a"" style=""font-size: 52px; line-height: 55px; font-weight: 300; letter-spacing: -1.5px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 52px; line-height: 55px; font-weight: 300; letter-spacing: -1.5px;"">{ProjectName}</span>
        </font>
        <div style=""height: 25px; line-height: 25px; font-size: 23px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#4c4c4c"" style=""font-size: 36px; line-height: 45px; font-weight: 300; letter-spacing: -1px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #4c4c4c; font-size: 36px; line-height: 45px; font-weight: 300; letter-spacing: -1px;"">Registers of {Table}</span>
        </font>
        <div style=""height: 35px; line-height: 35px; font-size: 33px;"">&nbsp;</div>
    </td>
    </tr>
</table>
<br>
<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""width: 100% !important; min-width: 100%; max-width: 100%;"">
    <tr>
        
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">FailureId</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">DateTimeCreation</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">DateTimeLastModification</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">UserCreationId</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">UserLastModificationId</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">Active</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">Message</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">EmergencyLevel</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">StackTrace</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">Source</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">Comment</span>
        </font>
        <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
    </th>
    
    </tr>
    {RowsAsHTML}
</table>
<br>
<font face=""'Source Sans Pro', sans-serif"" color=""#868686"" style=""font-size: 17px; line-height: 20px;"">
    <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #868686; font-size: 17px; line-height: 20px;"">Printed on: {DateTime.Now}</span>
</font>
";

            ChromePdfRenderer.RenderHtmlAsPdf(FullHTMLContent).SaveAs(path);
        }
    }
}