using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactBikes.Data;
using ReactBikes.Models;
using System.Data;

namespace ReactBikes.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ReactBikesUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ReactBikesUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UsersViewModel>();
            foreach (ReactBikesUser user in users)
            {
                var usersViewModel = new UsersViewModel();
                usersViewModel.User = user;
                usersViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(usersViewModel);
            }
            return View(userRolesViewModel);
        }
        private async Task<List<string>> GetUserRoles(ReactBikesUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Manage(string userId)
        {
            var model = new UsersViewModel();
            ViewBag.userId = userId;
            model.User = await _userManager.FindByIdAsync(userId);
            if (model.User == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            model.UserRolesViewModels = new List<UserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRoles = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,

                };
                if (await _userManager.IsInRoleAsync(model.User, role.Name))
                {
                    userRoles.Selected = true;
                }
                else
                {
                    userRoles.Selected = false;
                }
                model.UserRolesViewModels.Add(userRoles);
            }
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Manage(UsersViewModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.UserRolesViewModels.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        // GET: Bikes/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _userManager == null)
            {
                return NotFound();
            }
            UsersViewModel viewModel = new UsersViewModel();

            viewModel.User = await _userManager.FindByIdAsync(id);
            
            if (viewModel.User == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        
        // POST: Bikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Problem("user is null.");
            }
            else
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
        
    }
}

