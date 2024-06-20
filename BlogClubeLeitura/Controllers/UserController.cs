using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogClubeLeitura.Models;
using Microsoft.EntityFrameworkCore;
using BlogClubeLeitura.Data;

namespace BlogClubeLeitura.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                users = users.Where(u => u.UserName.Contains(searchQuery) || u.Email.Contains(searchQuery));
            }

            var allUsers = await users.ToListAsync();
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    userRoles[user.Id] = roles.ToList();
                }
            }

            var filteredUsers = allUsers.Where(u => userRoles.ContainsKey(u.Id)).ToList();

            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(filteredUsers);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["FeedbackMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                TempData["FeedbackMessage"] = "Failed to remove user roles.";
                return RedirectToAction(nameof(Index));
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                TempData["FeedbackMessage"] = "Failed to add new user role.";
                return RedirectToAction(nameof(Index));
            }

            TempData["FeedbackMessage"] = "User role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["FeedbackMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Remove posts associated with the user
            var posts = _context.Posts.Where(p => p.UserId == userId).ToList();
            _context.Posts.RemoveRange(posts);
            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["FeedbackMessage"] = "Failed to delete user.";
            }
            else
            {
                TempData["FeedbackMessage"] = "User deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}