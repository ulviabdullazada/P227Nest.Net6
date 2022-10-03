using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nest.Models
{
    public class Product:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public int? ReviewSum { get; set; } = 0;
        public int? ReviewCount { get; set; } = 0;
        [Required]
        public decimal CostPrice { get; set; }
        [Required]
        public decimal SellPrice { get; set; }
        [Range(0,100)]
        public int DiscountPercent { get; set; }
        public string? SubTitle { get; set; }
        public string? SKU { get; set; }
        [Required]
        public int LifeDay { get; set; }
        [Required]
        public int StockCount { get; set; }
        public string? Desription { get; set; }
        public int? BadgeId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public Category? Category { get; set; }
        public Badge? Badge { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }

        [NotMapped]
        public IFormFile? MainImage { get; set; }
        [NotMapped]
        public IFormFile? HoverImage { get; set; }
        [NotMapped]
        public IFormFileCollection? Images { get; set; }
    }
}
