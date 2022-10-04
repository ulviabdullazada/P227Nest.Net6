using System.ComponentModel.DataAnnotations;

namespace Nest.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
