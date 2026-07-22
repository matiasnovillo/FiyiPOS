using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.DTOs
{
    public record PaginatedUnitDTO
    {
        public List<Unit> ListUnit { get; init; } = [];

        public long TotalRecordsInDatabase { get; set; }
        public int TotalRecordsInTheList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecordsInTheList / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
