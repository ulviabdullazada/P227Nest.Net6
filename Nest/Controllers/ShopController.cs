using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;
using Nest.ViewModels;
using Newtonsoft.Json;
using Z.EntityFramework.Plus;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nest.Controllers
{
    public class ShopController : Controller
    {
        private readonly NestContext _context;
        public ShopController(NestContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page=1)
        {
            if (page < 0) page = 1;
            var products = _context.Products.Include(p => p.ProductImages).Include(p => p.Vendor)
                .Include(p => p.Badge);
            ViewBag.ActivePage = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 2);
            ShopIndexVM index = new ShopIndexVM
            {
                Products = products.Skip(((int)page - 1) * 2).Take(2),
                Categories = _context.Categories.Include(c => c.Products),
                Vendors = _context.Vendors.Include(v=>v.Products).OrderByDescending(v=>v.Products.Count).Take(6),
            };
            return View(index);
        }
        [HttpPost]
        public IActionResult Index(ShopAndFilterVM items) //List<int> p,List<int> VendorIds, int MaxPrice...
        {
            if (items is null) return NotFound();
            var products = _context.Products.IncludeOptimized(p => p.ProductImages).IncludeOptimized(p => p.Vendor)
                .IncludeOptimized(p => p.Badge).IncludeOptimized(p => p.Category);
            if (items.CategoryIds != null && items.VendorIds != null)
                products = products.Where(p=> items.CategoryIds.Contains(p.CategoryId) || items.VendorIds.Contains(p.VendorId));
            else
            {
                if (items.CategoryIds != null)
                    products = products.Where(p=> items.CategoryIds.Contains(p.CategoryId));
                if (items.VendorIds != null)
                    products = products.Where(p => items.VendorIds.Contains(p.VendorId));
            }
            if (items.MinPrice >= 0 && items.MinPrice < items.MaxPrice)
                products = products.Where(p => p.SellPrice >= items.MinPrice && p.SellPrice <= items.MaxPrice);
            return PartialView("_ProductListPartialView",products);
        }
        public IActionResult Product(int? id)
        {
            if (id is null) return BadRequest();
            var product = _context.Products
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .IncludeOptimized(p=>p.ProductImages)
                .IncludeOptimized(p=>p.Badge)
                .IncludeOptimized(p=>p.Vendor)
                .IncludeOptimized(p=>p.ProductComments)
                .IncludeOptimized(p=>p.Category).FirstOrDefault(p=>p.Id == id && p.IsDeleted == false);
            if (product is null) return NotFound();
            return View(product);
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Compare()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PostComment(ProductComment comment)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("ReturnUrl","/Shop/Product/"+comment.ProductId);
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account", routeValueDictionary);
            Product p = _context.Products.FirstOrDefault(p => p.Id == comment.ProductId && p.IsDeleted == false);
            if (p is null) return NotFound();
            if (!(comment.Rating >= 0 && comment.Rating <= 100) || comment is null) return RedirectToAction(nameof(Product), comment.ProductId);
            AppUser user = _context.AppUsers.FirstOrDefault(u=>u.UserName == User.Identity.Name);
            if (user is null) return NotFound();
            p.ReviewSum += comment.Rating;
            p.ReviewCount++;
            comment.AppUser = user;
            comment.Product = p;
            comment.CreatedTime = DateTime.UtcNow;
            _context.ProductComments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Product), new { id = p.Id });
        }
        public IActionResult AddBasket(int? id)
        {
            if (id is null) return BadRequest();
            if (!_context.Products.Any(p => p.Id == id)) return NotFound();
            //List<Product> products = HttpContext.Request.Cookies["Basket"];
            List<BasketItem> basketItems = new List<BasketItem>();
            string value = HttpContext.Request.Cookies["Basket"];
            if (value != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(value);
            }
            BasketItem item = basketItems.Find(bi=>bi.ProductId == id);
            if (item is null)
            {
                basketItems.Add(new BasketItem {ProductId=(int)id,Count=1 });
            }
            else
            {
                item.Count++;
            }
            HttpContext.Response.Cookies.Append("Basket",JsonConvert.SerializeObject(basketItems), new CookieOptions { MaxAge = TimeSpan.MaxValue });
            return Json(HttpContext.Request.Cookies["Basket"]);
        }

    }
}

