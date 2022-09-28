using Microsoft.AspNetCore.Mvc;
using Nest.DAL;
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
            return View();
        }
    }
}
