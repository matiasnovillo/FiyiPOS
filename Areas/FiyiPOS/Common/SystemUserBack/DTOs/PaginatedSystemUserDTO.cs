using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities;


/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.DTOs
{
    public record PaginatedSystemUserDTO
    {
        public List<SystemUser> ListSystemUser { get; init; } = [];

        public long TotalRecordsInDatabase { get; set; }
        public int TotalRecordsInTheList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecordsInTheList / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
