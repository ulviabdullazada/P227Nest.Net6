using Nest.Models;

namespace Nest.ViewModels
{
    public class BasketVM
    {
        public List<Product> Products { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
