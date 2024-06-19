using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogClubeLeitura.Models;

namespace BlogClubeLeitura.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(users);
        }

        [HttpPost]
        [Route("UpdateRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return RedirectToAction(nameof(Index));
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add new user role");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete user");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}