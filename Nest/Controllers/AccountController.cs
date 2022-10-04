using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest.Models;
using Nest.ViewModels;

namespace Nest.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationVM user)
        {
            if (!ModelState.IsValid) return View();
            var existUser = await _userManager.FindByNameAsync(user.Username);
            if (existUser != null)
            {
                ModelState.AddModelError("Username", "This username already exist");
            }
            AppUser appUser = new AppUser {
                FullName = user.Fullname,
                Email = user.Email,
                UserName = user.Username
            };
            var result = await _userManager.CreateAsync(appUser,user.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("",err.ToString());
                }
                return View();
            }
            await _signInManager.SignInAsync(appUser,true);
            return RedirectToAction("Index","Home");
        }
    }
}
