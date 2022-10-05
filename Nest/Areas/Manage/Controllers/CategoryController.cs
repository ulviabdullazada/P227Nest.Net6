using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;
using Nest.Utlis.Constants;
using Nest.Utlis.Extensions;
using System.Reflection.Metadata;

namespace Nest.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        NestContext _context { get; }

        public CategoryController(NestContext context)
        {
            _context = context;
        }
        // GET: CategoryController
        public IActionResult Index()
        {
            return View(_context.Categories.Include(c => c.Products));
        }

        //// GET: CategoryController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: CategoryController/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.ImageFile is null) ModelState.AddModelError("ImageFile","Zəhmət olmasa faylı seçin");
            if (!ModelState.IsValid) return View();
            var file = category.ImageFile;
            if (!file.CheckFileExtension("image/"))
            {
                ModelState.AddModelError("ImageFile","Yüklədiyiniz fayl şəkil deyil");
                return View();
            }
            if (file.CheckFileSize(2))
            {
                ModelState.AddModelError("ImageFile","Yüklədiyiniz fayl 2mb-dan artıq olmamalıdır");
                return View();
            }
            string newFileName = Guid.NewGuid().ToString();
            newFileName += file.CutFileName(60);
            file.SaveFile(Path.Combine("imgs", "shop", newFileName));
            category.ImageUrl = newFileName;
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();
            Category category = _context.Categories.Include(c=>c.Products).FirstOrDefault(x=>x.Id == id);
            if (category is null) return NotFound("Kateqoriya tapılmadı");
            if (category.Products.Count >= 1) return BadRequest("Bu kateqoriyaya aid məhsullar mövcuddur");
            if (category.IsDeleted == true)
            {
                RemoveFile(Path.Combine("shop", category.ImageUrl));
                _context.Categories.Remove(category);
            }
            else
            {
                category.IsDeleted = true;
            }
            category.Modified = DateTime.UtcNow;
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult Update(int? id)
        {
            if (id is null) return BadRequest();
            Category item = _context.Categories.Find(id);
            if (item is null) return NotFound();
            return View(item);
        }
        [HttpPost]
        public IActionResult Update(int? id, Category category)
        {
            if (id != category.Id || id is null) return BadRequest();
            Category item = _context.Categories.Find(id);
            if (item is null) return NotFound();
            if (category.ImageFile != null)
            {
                IFormFile file = category.ImageFile;
                if (!file.CheckFileExtension("image/"))
                {
                    ModelState.AddModelError("ImageFile","Yüklədiyiniz fayl şəkil deyil");
                    return View();
                }
                if (file.CheckFileSize(2))
                {
                    ModelState.AddModelError("ImageFile", "Yüklədiyiniz fayl 2mb-dan artıq olmamalıdır");
                    return View();
                }
                string newFileName = Guid.NewGuid().ToString();
                newFileName += file.CutFileName();
                RemoveFile(Path.Combine("imgs",item.ImageUrl));
                file.SaveFile(Path.Combine("imgs","shop",newFileName));
                item.ImageUrl = newFileName;
            }
            item.Name = category.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public static void RemoveFile(string path)
        {
            path = Path.Combine(Constants.RootPath,"img",path);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        [HttpPost]
        public IActionResult SetStatus(int? id)
        {
            if (id is null) return BadRequest();
            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            category.IsDeleted = false;
            category.Modified = DateTime.UtcNow;
            _context.SaveChanges();
            return Ok();
        }
    }
}
