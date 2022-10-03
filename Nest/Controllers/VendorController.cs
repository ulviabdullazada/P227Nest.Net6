using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;
using Nest.ViewModels;
using Z.EntityFramework.Plus;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nest.Controllers
{
    public class VendorController : Controller
    {
        private NestContext _context { get; set; }

        public VendorController(NestContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View(_context.Vendors.Include(v=>v.Products).Where(v=>v.IsDeleted==false));
        }

        public IActionResult Details(int? id)
        {
            if (id is null) return RedirectToAction(nameof(Index));
            Vendor vendor = _context.Vendors.FirstOrDefault(v=>v.Id == id);
            if (vendor is null) return NotFound();
            VendorDetailVM detailVM = new VendorDetailVM
            {
                Vendor = _context.Vendors.FirstOrDefault(v => v.Id == id),
                Products = _context.Products.IncludeOptimized(p => p.ProductImages).IncludeOptimized(p => p.Vendor)
                .IncludeOptimized(p => p.Badge).Where(p=>p.IsDeleted==false && p.VendorId == id)
            };
            return View(detailVM);
        }
        public IActionResult Create()
        {

            return View();
        }


    }
}

