using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;

namespace Nest.Areas.Manage.Controllers
{
    [Area("Manage")]
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
            if (!ModelState.IsValid) return View();
            var file = category.ImageFile;
            if (file.ContentType != "image/jpeg" || file.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile","Fayl formatı jpg,jpeg və ya png olmalıdır.");
            }
            if (file.Length < 1024 * 2)
            {
                ModelState.AddModelError("ImageFile", "Fayl ölçüsü 200kb-dan az olmalıdır");
            }
            string fileName = Guid.NewGuid().ToString();
            if (file.FileName.Length > 64)
            {
                fileName += file.FileName.Substring(0, 63) + file.FileName.Substring(file.FileName.LastIndexOf("."));
            }
            else
            {
                fileName += file.FileName;
            }
            using (var writer = new FileStream(@"/assets/imgs/shop", FileMode.Create))
            {
                file.CopyTo(writer);
            }
            category.ImageUrl = fileName;
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
            if (category.IsDeleted)
            {
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
    }
}
