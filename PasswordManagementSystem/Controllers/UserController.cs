using log4net;
using Microsoft.AspNetCore.Mvc;
using PasswordManagementSystem.Models;
using PasswordManagementSystem.Services;
using PasswordManagementSystem.ViewModels;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly PasswordService _passwordService;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController));

        public UserController(UserService userService, RoleService roleService, PasswordService passwordService)
        {
            _userService = userService;
            _roleService = roleService;
            _passwordService = passwordService;
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

        public async Task<IActionResult> AddUpdatePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId").Value;

            await controlForLoginStatus();

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var companyId = user.CompanyId;
            var userRoles = await _roleService.GetRolesByUserIdAsync(userId);
            var roleIds = userRoles.Select(r => r.RoleId).ToList();
            var minUserRoleId = userRoles.Min(r => r.RoleId);

            var roles = await _roleService.GetRolesByCompanyId(companyId);
            var filteredRoles = roles.Where(r => r.RoleId >= minUserRoleId).ToList();

            var passwords = await _passwordService.getPasswordsForUser(userId);

            // Şirket şifrelerini ve rolleri uygun ViewModel'lere dönüştürme işlemi
            var passwordViewModels = passwords.Select(p => new PasswordViewModel
            {
                PasswordId = p.PasswordId,
                PasswordName = p.PasswordName,
                EncryptedPassword = p.EncryptedPassword,
                UserId = p.UserId,
                CreatedBy = p.CreatedBy,
                CanEdit = p.CanEdit,
                Labels = p.Labels.Select(l => new LabelViewModel
                {
                    LabelId = l.LabelId,
                    PasswordId = l.PasswordId,
                    UserId = l.UserId,
                    LabelText = l.LabelText
                }).ToList()
            }).ToList();

            var roleViewModels = filteredRoles.Select(r => new RoleViewModel
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            }).ToList();

            var model = new PasswordViewModel
            {
                Passwords = passwordViewModels,
                Roles = roleViewModels,
                UserId = userId,
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRequiredPasswords()
        {
            var userId = HttpContext.Session.GetInt32("UserId").Value;

            await controlForLoginStatus(); // Kullanıcının oturum durumunu kontrol et

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Login"); // Kullanıcı bulunamazsa giriş sayfasına yönlendir
            }

            var passwords = await _passwordService.getPasswordsForUser(userId); // Kullanıcının şifrelerini al

            var result = passwords.Select(p => new
            {
                p.PasswordId,
                p.PasswordName,
                Labels = p.Labels?.Where(l => l.UserId == userId).Select(l => new { l.LabelText })
            }).ToList();

            return Json(result); // Şifreleri JSON formatında dön
        }

        // ----------------------------------------------------------------------------------------------------
        public async Task<bool> controlForLoginStatus()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                return false; // return RedirectToAction("Index", "Login"); // Login sayfasına yönlendirme
            }

            var user = await _userService.GetUserAndRolesById(sessionUserId.Value);
            if (user == null)
            {
                return false; // return RedirectToAction("Index", "Login"); // Kullanıcı bulunamazsa login sayfasına yönlendirme
            }

            var roles = user.Roles.Select(r => r.RoleName).ToList();

            if (!roles.Contains("Admin"))
            {
                return false; // return RedirectToAction("Index", "User", new { userId = user.UserId });
            }

            return true;
        }
    }
}
