using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Areas.Admin.ViewModels;

using static NutriWeek.GCommon.ApplicationConstants;

namespace NutriWeek.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AdministratorRoleName)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;


        public UserManagementController(
            UserManager<IdentityUser> userManager)
        {
            _userManager =
                userManager;
        }


        public async Task<IActionResult> Index()
        {
            List<IdentityUser> users =
                _userManager
                    .Users
                    .ToList();


            List<UserViewModel> userViewModels =
                new List<UserViewModel>();


            foreach (IdentityUser user
                     in users)
            {
                IList<string> roles =
                    await _userManager
                        .GetRolesAsync(user);


                userViewModels.Add(
                    new UserViewModel
                    {
                        Id =
                            user.Id,

                        Email =
                            user.Email ??
                            user.UserName ??
                            "No email",

                        Roles =
                            roles.ToList()
                    });
            }


            return View(userViewModels);
        }
    }
}
