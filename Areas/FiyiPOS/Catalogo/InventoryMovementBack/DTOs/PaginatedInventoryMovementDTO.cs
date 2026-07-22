namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.DTOs
{
    public record PaginatedInventoryMovementDTO
    {
        public List<InventoryMovementLineDTO> ListMovements   { get; init; } = [];

        public long TotalRecordsInDatabase { get; set; }
        public int  TotalRecordsInTheList  { get; set; }
        public long TotalFilteredRecords   { get; set; }
        public int  PageIndex              { get; set; }
        public int  PageSize               { get; set; }
        public int  TotalPages             => (int)Math.Ceiling(TotalFilteredRecords / (double)PageSize);
        public bool HasPreviousPage        => PageIndex > 1;
        public bool HasNextPage            => PageIndex < TotalPages;
    }
}
