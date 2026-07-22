namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.DTOs
{
    public class PromotionVM
    {
        public long   PromotionId { get; set; }
        public string Name        { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price      { get; set; }
        public bool   Active      { get; set; } = true;
        public List<PromotionDetailVM> Details { get; set; } = new();
    }

    public class PromotionDetailVM
    {
        public long    ProductId       { get; set; }
        public string  ProductName     { get; set; } = "";
        public decimal Quantity        { get; set; }
        public decimal Stock           { get; set; }
        public bool    AllowFractional { get; set; }
        public string  UnitAbbreviation { get; set; } = "";
    }
}
