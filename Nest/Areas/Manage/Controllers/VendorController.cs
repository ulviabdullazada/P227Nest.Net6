using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.DAL;
using Nest.Models;
using Nest.Utlis.Enums;
using Nest.Utlis.Extensions;
using System.Net;
using System.Net.Mail;

namespace Nest.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class VendorController : Controller
    {
        private readonly NestContext _context;
        private readonly UserManager<AppUser> _userManager;

        public VendorController(NestContext context,
                                UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: VendorController
        public ActionResult Index()
        {
            return View(_context.Vendors.Include(v=>v.Products));
        }

        // GET: VendorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VendorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VendorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vendor vendor)
        {
            if (!ModelState.IsValid) return View();
            if (vendor.ProfileImageFile is null)
            {
                ModelState.AddModelError("ProfileImageFile", "Zəhmət olmasa profil şəklinizi yükləyin");
                return View();
            }
            var profileImg = vendor.ProfileImageFile;
            if (!profileImg.CheckFileExtension("image/"))
            {
                ModelState.AddModelError("ProfileImageFile", "Yüklədiyiniz fayl şəkil deyil");
                return View();
            }
            if (profileImg.CheckFileSize(1))
            {
                ModelState.AddModelError("ProfileImageFile", "Yüklədiyiniz şəkil 2mb-dan artıq olmamalıdır");
                return View();
            }
            string newProfileImgName = Guid.NewGuid() + profileImg.CutFileName();
            profileImg.SaveFile(Path.Combine("imgs", "vendor", newProfileImgName));
            vendor.ProfileImageUrl = newProfileImgName;
            var backImg = vendor.BackgroundImageFile;
            vendor.BackgroundImageUrl = "vendor-header-bg.png";
            if (backImg != null)
            {
                if (!backImg.CheckFileExtension("image/"))
                {
                    ModelState.AddModelError("BackgroundImageUrl", "Yüklədiyiniz fayl şəkil deyil");
                    return View();
                }
                if (backImg.CheckFileSize(1))
                {
                    ModelState.AddModelError("BackgroundImageUrl", "Yüklədiyiniz şəkil 2mb-dan artıq olmamalıdır");
                    return View();
                }
                string newBackImgName = Guid.NewGuid() + backImg.CutFileName();
                backImg.SaveFile(Path.Combine("imgs", "vendor", newBackImgName));
                vendor.BackgroundImageUrl = newBackImgName;
            }
            vendor.IsDeleted = false;
            vendor.Modified = DateTime.UtcNow;
            _context.Vendors.Add(vendor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: VendorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VendorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VendorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VendorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<IActionResult> PendingVendors()
        {
            return View(await _userManager.GetUsersInRoleAsync(Roles.VendorPending.ToString()));
        }
        public async Task<IActionResult> SendInfoMail(string email, bool? isAccepted)
        {
            if (string.IsNullOrWhiteSpace(email) || isAccepted == null) return BadRequest();
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return NotFound();
            await _userManager.RemoveFromRoleAsync(user, Roles.VendorPending.ToString());
            if (isAccepted == true)
            {
                await _userManager.AddToRoleAsync(user, Roles.Vendor.ToString());
                SendMail(email,"Info about vendor program","<p style='color:green'>Congragulations</p>");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
                SendMail(email,"Info about vendor program","<p style='color:red'>We are so sorry</p>");
            }
            return RedirectToAction(nameof(Index));
        }
        private void SendMail(string email, string subject, string body)
        {
            string myEmail = "nestmvcproject@gmail.com";
            string pass = "euvmxxtbpinoytxt";

            var from = new MailAddress(myEmail,"Nest support");
            var to = new MailAddress(email);

            SmtpClient smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(myEmail,pass)
            };
            using (var message = new MailMessage(from, to) { Subject = subject, Body = body,IsBodyHtml=true})
            {
                smtp.Send(message);
            }

        }
    }
}
