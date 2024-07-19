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
                log.Warn("Attempt to access Index page without a valid session.");
                return RedirectToAction("Index", "Login"); // Login sayfasına yönlendirme
            }

            var user = await _userService.GetUserById((int)sessionUserId);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {sessionUserId}");
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
                log.Warn("Attempt to access Profile page without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(sessionUserId.Value);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {sessionUserId}");
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

        public async Task<IActionResult> PasswordListing()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (!sessionUserId.HasValue)
            {
                log.Warn("Attempt to access AddUpdatePassword without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var userId = sessionUserId.Value;

            if (!await controlForLoginStatus())
            {
                log.Warn("Unauthorized attempt to access PasswordListing.");
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            var companyId = user.CompanyId;
            var userRoles = await _roleService.GetRolesByUserIdAsync(userId);
            var roleIds = userRoles.Select(r => r.RoleId).ToList();
            var minUserRoleId = userRoles.Min(r => r.RoleId);

            var roles = await _roleService.GetRolesByCompanyId(companyId);
            var filteredRoles = roles.Where(r => r.RoleId >= minUserRoleId).ToList();

            var passwords = await _passwordService.getPasswordsForUser(userId);

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

            log.Info($"{GetCurrentUserEmail()} accessed the PasswordListing page.");

            return View(model);
        }

        public async Task<IActionResult> AddUpdatePassword()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (!sessionUserId.HasValue)
            {
                log.Warn("Attempt to access AddUpdatePassword without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var userId = sessionUserId.Value;

            if (!await controlForLoginStatus())
            {
                log.Warn("Unauthorized attempt to access AddUpdatePassword.");
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            var companyId = user.CompanyId;
            var userRoles = await _roleService.GetRolesByUserIdAsync(userId);
            var roleIds = userRoles.Select(r => r.RoleId).ToList();
            var minUserRoleId = userRoles.Min(r => r.RoleId);

            var roles = await _roleService.GetRolesByCompanyId(companyId);
            var filteredRoles = roles.Where(r => r.RoleId >= minUserRoleId).ToList();

            var passwords = await _passwordService.getPasswordsForUser(userId);

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

            log.Info($"{GetCurrentUserEmail()} accessed the AddUpdatePassword page.");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRequiredPasswords()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (!sessionUserId.HasValue)
            {
                log.Warn("Attempt to access AddUpdatePassword without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var userId = sessionUserId.Value;

            if (!await controlForLoginStatus())
            {
                log.Warn("Unauthorized attempt to access GetRequiredPasswords.");
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            var passwords = await _passwordService.getPasswordsForUser(userId);

            var result = passwords.Select(p => new
            {
                p.PasswordId,
                p.PasswordName,
                Labels = p.Labels?.Where(l => l.UserId == userId).Select(l => new { l.LabelText })
            }).ToList();

            log.Info($"{GetCurrentUserEmail()} accessed the GetRequiredPasswords action.");

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPassword(int passwordId)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (!sessionUserId.HasValue)
            {
                log.Warn("Attempt to access AddUpdatePassword without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var userId = sessionUserId.Value;
            var password = await _passwordService.GetPasswordByIdAsync(passwordId, userId);

            if (!await controlForLoginStatus())
            {
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            if (password == null)
            {
                log.Warn($"{GetCurrentUserEmail()} attempted to access a non-existent password with ID {passwordId}.");
                return NotFound();
            }

            var passwordViewModel = new PasswordViewModel
            {
                PasswordId = password.PasswordId,
                PasswordName = password.PasswordName,
                EncryptedPassword = password.EncryptedPassword,
                UserId = password.UserId,
                CreatedBy = password.CreatedBy,
                CanEdit = password.CanEdit,
                Labels = password.Labels?.Select(l => new LabelViewModel
                {
                    LabelId = l.LabelId,
                    PasswordId = l.PasswordId,
                    UserId = l.UserId,
                    LabelText = l.LabelText
                }).ToList(),
                Roles = password.Roles?.Select(r => new RoleViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                }).ToList(),
                RoleIds = password.Roles?.Select(r => r.RoleId).ToList(),
                Label = password.Labels?.FirstOrDefault()?.LabelText
            };

            log.Info($"{GetCurrentUserEmail()} accessed details for password with ID {passwordId}.");

            return Json(passwordViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CheckUserPassword(string userPassword)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (!sessionUserId.HasValue)
            {
                log.Warn("Attempt to access AddUpdatePassword without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var userId = sessionUserId.Value;

            if (!await controlForLoginStatus())
            {
                log.Warn($"User not found");
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(sessionUserId.Value);
            if (user != null && user.Password == userPassword) // Şifreyi burada hash'lenmiş şekilde kontrol etmelisiniz
            {
                log.Info($"{GetCurrentUserEmail()} successfully verified password.");
                return Json(new { success = true });
            }

            log.Warn($"{GetCurrentUserEmail()} failed to verify password.");
            return Json(new { success = false });
        }

        public async Task<bool> controlForLoginStatus()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                log.Warn("No valid session found during controlForLoginStatus check.");
                return false;
            }

            var user = await _userService.GetUserAndRolesById(sessionUserId.Value);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {sessionUserId}");
                return false;
            }

            log.Info($"User with session ID: {sessionUserId} passed controlForLoginStatus check.");
            return true;
        }
    }
}
