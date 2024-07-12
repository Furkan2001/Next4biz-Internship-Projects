using log4net;
using Microsoft.AspNetCore.Mvc;
using PasswordManagementSystem.Models;
using PasswordManagementSystem.Services;
using PasswordManagementSystem.ViewModels;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController));

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                return RedirectToAction("Index", "Login"); // Login sayfasına yönlendirme
            }

            var user = await _userService.GetUserById((int)sessionUserId);
            if (user == null)
            {
                return RedirectToAction("Index", "Login"); // Login sayfasına yönlendirme
            }

            log.Info($"{GetCurrentUserEmail()} accessed the Index page.");

            ViewBag.AdminName = user.Name;
            ViewBag.AdminId = user.UserId;
            return View(user);
        }

        private string GetCurrentUserEmail()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                return "Unknown User";
            }
            var user = _userService.GetUserById(sessionUserId.Value).Result;
            return user?.Email ?? "Unknown User";
        }

        public async Task<IActionResult> Profile()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(sessionUserId.Value);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var company = await _userService.GetUserCompany(user.UserId);
            var roles = await _userService.GetUserRoles(user.UserId);

            var model = new UserProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                CompanyName = company?.CompanyName,
                Roles = roles.ToList()
            };

            log.Info($"{GetCurrentUserEmail()} accessed their profile.");

            return View(model);
        }
    }
}
