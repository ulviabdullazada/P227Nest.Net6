using Nest.Models;

namespace Nest.ViewModels
{
    public class ShopIndexVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
