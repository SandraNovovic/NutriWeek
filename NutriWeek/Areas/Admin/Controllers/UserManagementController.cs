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
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserManagementController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager =
                userManager;

            _roleManager =
                roleManager;
        }


        public async Task<IActionResult> Index()
        {
            List<IdentityUser> users =
                _userManager
                    .Users
                    .ToList();


            List<string> availableRoles =
                _roleManager
                    .Roles
                    .Select(r =>
                        r.Name!)
                    .OrderBy(r =>
                        r)
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
                            roles.ToList(),

                        AvailableRoles =
                            availableRoles
                    });
            }


            return View(userViewModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(
            string userId,
            string roleName)
        {
            IdentityUser? user =
                await _userManager
                    .FindByIdAsync(userId);


            if (user == null)
            {
                TempData["Error"] =
                    "User not found.";

                return RedirectToAction(
                    nameof(Index));
            }


            bool roleExists =
                await _roleManager
                    .RoleExistsAsync(roleName);


            if (!roleExists)
            {
                TempData["Error"] =
                    "Role not found.";

                return RedirectToAction(
                    nameof(Index));
            }


            bool alreadyInRole =
                await _userManager
                    .IsInRoleAsync(
                        user,
                        roleName);


            if (alreadyInRole)
            {
                TempData["Error"] =
                    "The user already has this role.";

                return RedirectToAction(
                    nameof(Index));
            }


            IdentityResult result =
                await _userManager
                    .AddToRoleAsync(
                        user,
                        roleName);


            if (!result.Succeeded)
            {
                TempData["Error"] =
                    "The role could not be assigned.";

                return RedirectToAction(
                    nameof(Index));
            }


            TempData["Success"] =
                $"{roleName} role was assigned to {user.Email}.";


            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(
            string userId,
            string roleName)
        {
            IdentityUser? user =
                await _userManager
                    .FindByIdAsync(userId);


            if (user == null)
            {
                TempData["Error"] =
                    "User not found.";

                return RedirectToAction(
                    nameof(Index));
            }


            string? currentUserId =
                _userManager
                    .GetUserId(User);


            if (user.Id == currentUserId &&
                roleName == AdministratorRoleName)
            {
                TempData["Error"] =
                    "You cannot remove your own Administrator role.";

                return RedirectToAction(
                    nameof(Index));
            }


            bool isInRole =
                await _userManager
                    .IsInRoleAsync(
                        user,
                        roleName);


            if (!isInRole)
            {
                TempData["Error"] =
                    "The user does not have this role.";

                return RedirectToAction(
                    nameof(Index));
            }


            IdentityResult result =
                await _userManager
                    .RemoveFromRoleAsync(
                        user,
                        roleName);


            if (!result.Succeeded)
            {
                TempData["Error"] =
                    "The role could not be removed.";

                return RedirectToAction(
                    nameof(Index));
            }


            TempData["Success"] =
                $"{roleName} role was removed from {user.Email}.";


            return RedirectToAction(
                nameof(Index));
        }
    }
}
