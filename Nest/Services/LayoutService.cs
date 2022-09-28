using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;
using Nest.ViewModels;
using Newtonsoft.Json;

namespace Nest.Services
{
    public class LayoutService
    {

        //public List<BasketItem> GetBasketItems() 
        //{

        //}
        NestContext _context { get; }
        IHttpContextAccessor _accessor { get; }
        public LayoutService(NestContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }


        public Dictionary<string,string> GetSettings()
        {
            return _context.Settings.ToDictionary(p=>p.Key,p=>p.Value);
        }
        public BasketVM GetBasket()
        {
            var basket = new BasketVM {Products = new(),TotalPrice =0 };
            var basketItems = new List<BasketItem>();
            string cookie = _accessor.HttpContext.Request.Cookies["Basket"];
            if (cookie != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(cookie);
            }
            foreach (var item in basketItems)
            {
                Product p = _context.Products.Include(p=>p.ProductImages).FirstOrDefault(p=>p.Id == item.ProductId);
                if (p != null)
                {
                    basket.Products.Add(new ProductBasketItemVM { 
                        Product = p,
                        Count = item.Count
                    });
                    basket.TotalPrice += p.SellPrice * (100 - p.DiscountPercent) / 100*item.Count;
                }
            }
            return basket;
        }
    }
}
