using ClosedXML.Excel;
using FiyiPOS.Areas.System.FailureBack.Entities;
using FiyiPOS.Areas.System.FailureBack.Interfaces;

namespace FiyiPOS.Areas.System.FailureBack.Services
{
    public class FailureImportationService : IFailureImportationService
    {
        public List<Failure> ImportExcel(string path, long userId)
        {
            List<Failure> ListFailure = [];

            XLWorkbook WorkBook = new(path);

            var Rows = WorkBook.Worksheet(1).RangeUsed()!.RowsUsed();

            foreach (var row in Rows)
            {
                var rowNumber = row.RowNumber();

                if (rowNumber > 1)
                {
                    string Message = row.Cell(7).GetString();
                    int EmergencyLevel = Convert.ToInt32(row.Cell(8).GetString());
                    string StackTrace = row.Cell(9).GetString();
                    string Source = row.Cell(10).GetString();
                    string Comment = row.Cell(11).GetString();
                    

                    Failure Failure = new()
                    {
                        FailureId = 0,
                        Active = true,
                        DateTimeCreation = DateTime.Now,
                        DateTimeLastModification = DateTime.Now,
                        UserCreationId = userId,
                        UserLastModificationId = userId,
                        Message = Message,
                        EmergencyLevel = EmergencyLevel,
                        StackTrace = StackTrace,
                        Source = Source,
                        Comment = Comment,
                        
                    };

                    ListFailure.Add(Failure);
                }
            }

            return ListFailure;
        }
    }
}