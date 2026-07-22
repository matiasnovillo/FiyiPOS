namespace FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.DTOs
{
    public class InvoiceListItemDTO
    {
        public long      InvoiceId         { get; set; }
        public long      SaleId            { get; set; }
        public string    InvoiceNumber     { get; set; } = string.Empty;
        public int       TipoComprobante   { get; set; }
        public int       PuntoVenta        { get; set; }
        public long      NumeroComprobante { get; set; }
        public string?   CAE               { get; set; }
        public DateTime? CAEFechaVencimiento { get; set; }
        public DateTime  InvoiceDate       { get; set; }
        public decimal   Subtotal          { get; set; }
        public decimal   TaxAmount         { get; set; }
        public decimal   Total             { get; set; }
        public string    Status            { get; set; } = "active";
        public string?   AfipResultado     { get; set; }
        public string    Ambiente          { get; set; } = "Homologacion";
        public string?   TicketNumber      { get; set; }
        public string?   ClientName        { get; set; }
    }
}
