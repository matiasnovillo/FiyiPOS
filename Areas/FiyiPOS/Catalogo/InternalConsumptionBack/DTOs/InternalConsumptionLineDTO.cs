namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.DTOs
{
    public class InternalConsumptionLineDTO
    {
        public long     InternalConsumptionId { get; set; }
        public DateTime ConsumptionDate       { get; set; }
        public string   Reason                { get; set; } = string.Empty;
        public string   OperatorName           { get; set; } = string.Empty;
        public decimal  TotalCost              { get; set; }
        public int      ItemCount              { get; set; }
        public decimal  TotalQuantity          { get; set; }
    }
}
