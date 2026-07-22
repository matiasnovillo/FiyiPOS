using FiyiPOS.Areas.System.FailureBack.Entities;
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

namespace FiyiPOS.Areas.System.FailureBack.Interfaces
{
    public interface IFailureExportationService
    {
        void ExportToExcel(string path, DataTable dataTableFailure);

        void ExportToCSV(string path, List<Failure> listFailure);

        void ExportToPDF(string path, List<Failure> listFailure);
    }
}