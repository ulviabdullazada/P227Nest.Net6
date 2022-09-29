using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;
using Nest.ViewModels;
using Z.EntityFramework.Plus;

namespace Nest.Controllers;

public class HomeController : Controller
{
    private NestContext _context { get; }

    public HomeController(NestContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        HomeVM home = new HomeVM
        {
            Sliders = _context.Sliders.OrderBy(s => s.Order),
            Categories = _context.Categories.Include(c=>c.Products).Where(c=>c.IsDeleted==false),
            Products = _context.Products.IncludeOptimized(p => p.ProductImages)
            .IncludeOptimized(p => p.Badge).IncludeOptimized(p => p.Category).IncludeOptimized(p => p.Vendor)
        };
        return View(home);
    }
}

