namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.DTOs
{
    public class ReturnSaleLineDTO
    {
        public long     ReturnSaleId { get; set; }
        public string   TicketNumber { get; set; } = string.Empty;
        public DateTime ReturnDate   { get; set; }
        public string?  Reason       { get; set; }
        public decimal  Total        { get; set; }
        public int      ItemCount    { get; set; }
    }
}
