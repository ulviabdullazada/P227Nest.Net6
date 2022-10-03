using Microsoft.AspNetCore.Mvc;
using Nest.DAL;
using Nest.Models;
using Nest.Utlis.Extensions;
using Z.EntityFramework.Plus;

namespace Nest.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        NestContext _context { get; }

        public ProductController(NestContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            return View(_context.Products.IncludeOptimized(p => p.ProductImages.Take(1)).IncludeOptimized(p=>p.Category));
        }
        public IActionResult Create()
        {
            ViewBag.Vendors = _context.Vendors.Where(v => v.IsDeleted == false);
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            ViewBag.Badges = _context.Badges;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product prod)
        {
            ViewBag.Vendors = _context.Vendors.Where(v => v.IsDeleted == false);
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            ViewBag.Badges = _context.Badges;
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (prod.MainImage is null)
            {
                ModelState.AddModelError("MainImage","Zəhmət olmasa şəkil seçin");
                return View();
            }

            #region Images
            var productImgs = prod.Images;
            foreach (var img in productImgs)
            {
                if (!img.CheckFileExtension("image/"))
                {
                    ModelState.AddModelError("MainImage", "Yüklədiyiniz fayl şəkil deyil");
                    return View();
                }
                if (img.CheckFileSize(2))
                {
                    ModelState.AddModelError("MainImage", "Yüklədiyiniz şəkil 2mb-dan artıq olmamalıdır");
                    return View();
                }
            }

            List<ProductImage> images = new List<ProductImage>();

            foreach (var img in productImgs)
            {
                string imgUrl = Guid.NewGuid() + img.CutFileName();
                img.SaveFile(Path.Combine("imgs", "shop",imgUrl));
                images.Add(new() {
                    ImageUrl = imgUrl,
                    Status = null,
                    Product = prod
                });
            }
            #endregion

            #region Main Image
            var mainImg = prod.MainImage;
            if (!mainImg.CheckFileExtension("image/"))
            {
                ModelState.AddModelError("MainImage", "Yüklədiyiniz fayl şəkil deyil");
                return View();
            }
            if (mainImg.CheckFileSize(2))
            {
                ModelState.AddModelError("MainImage", "Yüklədiyiniz şəkil 2mb-dan artıq olmamalıdır");
                return View();
            }
            string mainImgName = Guid.NewGuid() + mainImg.CutFileName();
            mainImg.SaveFile(Path.Combine("imgs","shop",mainImgName));
            images.Add(new()
            {
                ImageUrl = mainImgName,
                Status = true,
                Product = prod
            });
            #endregion

            #region Hover Image
            var hoverImg = prod.HoverImage;
            if (hoverImg != null)
            {
                if (!hoverImg.CheckFileExtension("image/"))
                {
                    ModelState.AddModelError("HoverImage", "Yüklədiyiniz fayl şəkil deyil");
                    return View();
                }
                if (hoverImg.CheckFileSize(2))
                {
                    ModelState.AddModelError("HoverImage", "Yüklədiyiniz şəkil 2mb-dan artıq olmamalıdır");
                    return View();
                }
                string hoverImgName = Guid.NewGuid() + hoverImg.CutFileName();
                hoverImg.SaveFile(Path.Combine("imgs", "shop", hoverImgName));
                images.Add(new()
                {
                    ImageUrl = hoverImgName,
                    Status = false,
                    Product = prod
                });
            }
            #endregion
            prod.SKU = prod.Name.Substring(0, 2) + prod.CategoryId.ToString();
            prod.ProductImages = images;
            _context.Products.Add(prod);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
