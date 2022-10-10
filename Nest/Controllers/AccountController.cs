using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest.Models;
using Nest.Utlis.Enums;
using Nest.ViewModels;

namespace Nest.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, 
                                SignInManager<AppUser> signInManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index","Home");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginVM user, string ReturnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser existUser = await _userManager.FindByNameAsync(user.Username);
            if (existUser is null)
            {
                ModelState.AddModelError("","Username or password is wrong");
                return View();
            }
            var result  = await _signInManager.PasswordSignInAsync(existUser, user.Password,user.RememberMe, true);
            
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "You made too many attemps. Wait untill -> " + existUser.LockoutEnd?.AddHours(4).DateTime.ToString("MM/dd/yyyy HH:mm:ss"));
                    return View();
                }
                ModelState.AddModelError("", "Username or password is wrong");
                return View();
            }
            if (ReturnUrl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(ReturnUrl);
        }
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index","Home");
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
                return View();
            }
            
            if (user.Status.ToLower() != Roles.VendorPending.ToString().ToLower() && user.Status.ToLower() != Roles.Member.ToString().ToLower())
            {
                ModelState.AddModelError("", "Role-a deyme");
                return View();
            }
            AppUser appUser = new AppUser {
                FullName = user.Fullname,
                Email = user.Email,
                UserName = user.Username,
                ProfileImageUrl = "default.png"
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
            await _userManager.AddToRoleAsync(appUser,user.Status);
            await _signInManager.SignInAsync(appUser,true);
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        [Authorize]
        public async Task<IActionResult> UpdateUser()
        {
            ViewBag.ActiveTab = "Dashboard";
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return View(new UpdateUserVM { Fullname = user.FullName,Email=user.Email});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUser(UpdateUserVM user)
        {
            ViewBag.ActiveTab = "Account";
            if (!ModelState.IsValid) return View();
            AppUser currentUser = _userManager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            if (currentUser == null) RedirectToAction(nameof(Login));
            AppUser existUser = _userManager.FindByEmailAsync(user.Email).Result;
            if (existUser != null && existUser.Email != currentUser.Email)
            {
                ModelState.AddModelError("Email", "This email already exist");
                return View();
            }
            if (user.CurrentPassword != user.NewPassword)
            {
                var result = _userManager.ChangePasswordAsync(currentUser,user.CurrentPassword, user.NewPassword);
                if (!result.IsCompletedSuccessfully)
                {
                    if (! _userManager.CheckPasswordAsync(currentUser, user.CurrentPassword).Result)
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is not correct");
                        return View();
                    }
                }
            }
            currentUser.Email = user.Email;
            currentUser.FullName = user.Fullname;
            _userManager.UpdateAsync(currentUser);
            ViewBag.ActiveTab = "Dashboard";
            ViewBag.ToastrMessage = "Your changes is saved";
            return View();


            //string a = _userManager.GenerateEmailConfirmationTokenAsync();
            //_userManager.Verif
            //ConfirmEmail/UserId?token=a
            //Url.Action("ConfirmEmail", "Account", new { username = "", token = "" }, HttpContext.Request.Scheme);
        }
        //public async Task CreateRoles()
        //{
        //    foreach (var item in Enum.GetValues(typeof(Roles)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(item.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
        //        }
        //    }
        //}
        //public async Task<IActionResult> CreateSA()
        //{
        //    AppUser admin = new AppUser { FullName = "Ülvi Abdullazadə", UserName = "u1viii", Email = "ulvi.abdullazada@code.edu.az", IsAdmin = true };
        //    var result = await _userManager.CreateAsync(admin, "Admin123");
        //    if (!result.Succeeded)
        //    {
        //        var err = "";
        //        foreach (var item in result.Errors)
        //        {
        //            err += item;
        //        }
        //        return Content(err);
        //    }
        //    await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(admin.UserName), Roles.SuperAdmin.ToString());
        //    return Ok(admin);
        //}
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
