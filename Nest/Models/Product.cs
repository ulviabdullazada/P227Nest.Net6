using System.ComponentModel.DataAnnotations;

namespace Nest.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public int ReviewSum { get; set; }
        public int ReviewCount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        [Range(0,100)]
        public int DiscountPercent { get; set; }
        public string SubTitle { get; set; }
        public string SKU { get; set; }
        public int LifeDay { get; set; }
        public int StockCount { get; set; }
        public string Desription { get; set; }
        public int BadgeId { get; set; }
        public int CategoryId { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public Category Category { get; set; }
        public Badge Badge { get; set; }
    }
}
