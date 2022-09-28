using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nest.Models
{
    public class Category:BaseEntity
    {
        [StringLength(40,ErrorMessage ="Uzunluq 40dan çox ola bilməz"), Required(ErrorMessage = "Yaz dana")]
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Product>? Products { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
