using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest.DAL;
using Nest.Models;
using Nest.ViewModels;

namespace Nest.ViewComponents
{
    public class LayoutHeaderViewComponent:ViewComponent
    {
        UserManager<AppUser> _userManager { get; }
        public LayoutHeaderViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _userManager.FindByNameAsync(HttpContext.User.Identity.Name??""));
        }

    }
}
