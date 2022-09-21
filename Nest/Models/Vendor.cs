namespace Nest.Models
{
    public class Vendor:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
        public string ProfileImageUrl { get; set; }
        public string BackgroundImageUrl { get; set; }
        public int CreatedYear { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
