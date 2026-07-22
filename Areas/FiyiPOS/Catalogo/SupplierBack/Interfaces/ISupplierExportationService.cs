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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Interfaces
{
    public interface ISupplierExportationService
    {
        byte[] ExportToExcel(DataTable dataTable);
    }
}
