using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nest.Models
{
    public class Vendor:BaseEntity
    {
        [StringLength(45), Required]
        public string? Name { get; set; }
        [StringLength(1500), Required]
        public string? Description { get; set; }
        [StringLength(100), Required]
        public string? Address { get; set; }
        [StringLength(30)]
        public string Number { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public int CreatedYear { get; set; }
        public ICollection<Product>? Products { get; set; }
        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; }
        [NotMapped]
        public IFormFile? BackgroundImageFile { get; set; }
    }
}
