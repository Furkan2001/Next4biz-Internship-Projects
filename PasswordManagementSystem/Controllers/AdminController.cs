using Microsoft.AspNetCore.Mvc;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.ViewModels;
using PasswordManagementSystem.Services;
using System.Threading.Tasks;
using PasswordManagementSystem.Models;
using PasswordManagementSystem.Dtos;
using PasswordManagementSystem.Repositories;
using log4net;
using System;

namespace PasswordManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly LogService _logService;
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));

        public AdminController(UserService userService, RoleService roleService, LogService logService)
        {
            _userService = userService;
            _roleService = roleService;
            _logService = logService;
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

        public async Task<IActionResult> Index()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");

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

        public async Task<IActionResult> UserListing()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value); //companyId yi içinden almak için

            var users = await _userService.getUsersInCompanyId(currentUser.CompanyId);

            log.Info($"{GetCurrentUserEmail()} accessed the UserListing page.");

            return View(users);
        }

        public async Task<IActionResult> AddUpdateUser()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            var companyId = currentUser.CompanyId;

            var users = (await _userService.GetAllUsers()).Where(u => u.CompanyId == companyId).ToList();
            var allRoles = await _userService.GetAllRoles(HttpContext); // Sadece aynı şirketin rolleri
            var userRoles = new List<List<Role>>();
            var userCompanies = new List<Company>();

            foreach (var user in users)
            {
                var roles = (await _userService.GetUserRoles(user.UserId)).ToList();
                var company = await _userService.GetUserCompany(user.UserId);

                userRoles.Add(roles);
                userCompanies.Add(company);
            }

            log.Info($"{GetCurrentUserEmail()} accessed the AddUpdateUser page.");

            var model = new Tuple<List<User>, List<List<Role>>, List<Company>, List<Role>, int?>(users, userRoles, userCompanies, allRoles, sessionUserId);
            return View(model);
        }

        public async Task<IActionResult> SaveUser(UserDto userDto)
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            userDto.CompanyId = currentUser.CompanyId;

            bool success = await _userService.SaveUser(userDto);
            if (success)
            {
                log.Info($"{GetCurrentUserEmail()} saved a user with email {userDto.Email}.");
                return Json(new { success = true });
            }
            log.Warn($"{GetCurrentUserEmail()} failed to save a user with email {userDto.Email}.");
            return Json(new { success = false });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            bool success = await _userService.DeleteUser2(userId);
            if (success)
            {
                log.Info($"{GetCurrentUserEmail()} deleted a user with ID {userId}.");
                return Json(new { success = true });
            }
            log.Warn($"{GetCurrentUserEmail()} failed to delete a user with ID {userId}.");
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int userId)
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                log.Warn($"{GetCurrentUserEmail()} attempted to access a non-existent user with ID {userId}.");
                return NotFound();
            }

            var roles = await _userService.GetUserRoles(userId);
            var allRoles = await _userService.GetAllRoles(HttpContext); // Sadece aynı şirketin rolleri
            var company = await _userService.GetUserCompany(userId);

            var model = new
            {
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.Password
                },
                roles = roles.Select(r => new {
                    r.RoleId,
                    r.RoleName
                }).ToList(),
                allRoles = allRoles.Select(r => new {
                    r.RoleId,
                    r.RoleName
                }).ToList(),
                company = new
                {
                    company.CompanyName
                }
            };

            log.Info($"{GetCurrentUserEmail()} accessed details for user with email {user.Email}.");

            return Json(model);
        }

        public async Task<IActionResult> Profile()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
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

        public async Task<IActionResult> CompanyLogs()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var companyLogs = await _logService.GetLogsByCompanyId(currentUser.CompanyId);
            return View(companyLogs);
        }

        public async Task<IActionResult> RoleListing()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            var roles = await _roleService.GetRolesByCompanyId(currentUser.CompanyId);

            foreach (var role in roles)
            {
                role.Users = await _userService.GetUsersByRoleId(role.RoleId);
            }

            log.Info($"{GetCurrentUserEmail()} accessed the RoleListing page.");

            return View(roles);
        }

        public async Task<IActionResult> AddUpdateRole()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            var companyId = currentUser.CompanyId;

            var roles = (await _roleService.GetAllRoles()).Where(r => r.CompanyId == companyId).ToList();

            log.Info($"{GetCurrentUserEmail()} accessed the AddUpdateRole page.");

            var model = new Tuple<List<Role>, int?>(roles, sessionUserId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleDto roleDto)
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            roleDto.CompanyId = currentUser.CompanyId;

            Console.WriteLine("roleDto.CompanyId: " + roleDto.CompanyId);
            Console.WriteLine("roleDto.roleId: " + roleDto.RoleId);

            bool success = await _roleService.SaveRole(roleDto);
            if (success)
            {
                log.Info($"{GetCurrentUserEmail()} saved a role with name {roleDto.RoleName}.");
                return Json(new { success = true });
            }
            log.Warn($"{GetCurrentUserEmail()} failed to save a role with name {roleDto.RoleName}.");
            return Json(new { success = false });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            bool success = await _roleService.DeleteRole2(roleId);
            if (success)
            {
                log.Info($"{GetCurrentUserEmail()} deleted a role with ID {roleId}.");
                return Json(new { success = true });
            }
            log.Warn($"{GetCurrentUserEmail()} failed to delete a role with ID {roleId}.");
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int roleId)
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                return RedirectToAction("Index", "Login");
            }

            var role = await _roleService.GetRoleById(roleId);
            if (role == null)
            {
                log.Warn($"{GetCurrentUserEmail()} attempted to access a non-existent role with ID {roleId}.");
                return NotFound();
            }

            var model = new
            {
                role = new
                {
                    role.RoleId,
                    role.RoleName,
                    role.CompanyId
                }
            };

            log.Info($"{GetCurrentUserEmail()} accessed details for role with name {role.RoleName}.");

            return Json(model);
        }

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
