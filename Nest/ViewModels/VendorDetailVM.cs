using Nest.Models;
namespace Nest.ViewModels
{
    public class VendorDetailVM
    {
        public Vendor Vendor { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
