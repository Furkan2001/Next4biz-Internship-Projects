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
        private readonly PasswordService _passwordService;
        private readonly LabelService _labelService;
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));

        public AdminController(UserService userService, RoleService roleService, LogService logService, PasswordService passwordService, LabelService labelService)
        {
            _userService = userService;
            _roleService = roleService;
            _logService = logService;
            _passwordService = passwordService;
            _labelService = labelService;
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
                log.Warn("Attempt to access Index page without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");

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

        public async Task<IActionResult> UserListing()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                log.Warn("Attempt to access UserListing page without a valid session.");
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
                log.Warn("Attempt to access AddUpdateUser page without a valid session.");
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
                log.Warn("Attempt to save user without a valid session.");
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
                log.Warn("Attempt to delete user without a valid session.");
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
                log.Warn("Attempt to get user without a valid session.");
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
                log.Warn("Attempt to access Profile page without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
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

        public async Task<IActionResult> CompanyLogs()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                log.Warn("Attempt to access CompanyLogs page without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            if (currentUser == null)
            {
                log.Warn($"User not found with session ID: {sessionUserId}");
                return RedirectToAction("Index", "Login");
            }

            var companyLogs = await _logService.GetLogsByCompanyId(currentUser.CompanyId);

            log.Info($"{GetCurrentUserEmail()} accessed the CompanyLogs page.");

            return View(companyLogs);
        }

        public async Task<IActionResult> RoleListing()
        {
            bool loginStatus = await controlForLoginStatus();
            if (!loginStatus)
            {
                log.Warn("Attempt to access RoleListing page without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                log.Warn("Attempt to access Role List page without a valid session.");
                return RedirectToAction("Index", "Login");
            }
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
                log.Warn("Attempt to access AddUpdateRole page without a valid session.");
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
                log.Warn("Attempt to save role without a valid session.");
                return RedirectToAction("Index", "Login");
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _userService.GetUserById(sessionUserId.Value);
            roleDto.CompanyId = currentUser.CompanyId;

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
                log.Warn("Attempt to delete role without a valid session.");
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
                log.Warn("Attempt to get role without a valid session.");
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
                log.Warn($"User not found with session ID: {userId}");
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
            var minUserRoleId = userRoles.Min(r => r.RoleId);

            var roles = await _roleService.GetRolesByCompanyId(companyId);
            var filteredRoles = roles.Where(r => r.RoleId >= minUserRoleId).ToList();

            var passwords = await _passwordService.GetCompanyPasswords(companyId);

            var passwordViewModels = passwords.Select(p => new PasswordViewModel
            {
                PasswordId = p.PasswordId,
                PasswordName = p.PasswordName,
                EncryptedPassword = p.EncryptedPassword,
                UserId = p.UserId,
                CreatedBy = p.CreatedBy,
                Labels = p.Labels?.Select(l => new LabelViewModel
                {
                    LabelId = l.LabelId,
                    PasswordId = l.PasswordId,
                    UserId = l.UserId,
                    LabelText = l.LabelText
                }).ToList(),
                Roles = p.Roles?.Select(r => new RoleViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                }).ToList(),
                RoleIds = p.Roles?.Select(r => r.RoleId).ToList(),
                Label = p.Labels?.FirstOrDefault()?.LabelText
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
                UserId = userId
            };

            log.Info($"{GetCurrentUserEmail()} accessed the PasswordListing page.");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPasswordDetails(int passwordId)
        {
            if (!await controlForLoginStatus())
            {
                log.Warn($"User not found");
                return RedirectToAction("Index", "Login");
            }

            var password = await _passwordService.GetPasswordDetailsByIdAsync(passwordId);
            var labels = password.Labels?.Select(l => l.LabelText).ToList() ?? new List<string>();
            var creatorEmail = password.CreatedBy;

            var details = new
            {
                passwordName = password.PasswordName,
                encryptedPassword = password.EncryptedPassword,
                labels,
                creatorEmail
            };

            log.Info($"{GetCurrentUserEmail()} accessed details for password with ID {passwordId}.");

            return Json(details);
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
                log.Warn($"User not found with session ID: {userId}");
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
            var minUserRoleId = userRoles.Min(r => r.RoleId);

            var roles = await _roleService.GetRolesByCompanyId(companyId);
            var filteredRoles = roles.Where(r => r.RoleId >= minUserRoleId).ToList();

            var passwords = await _passwordService.GetCompanyPasswords(companyId);

            var passwordViewModels = passwords.Select(p => new PasswordViewModel
            {
                PasswordId = p.PasswordId,
                PasswordName = p.PasswordName,
                EncryptedPassword = p.EncryptedPassword,
                UserId = p.UserId,
                CreatedBy = p.CreatedBy,
                Labels = p.Labels?.Select(l => new LabelViewModel
                {
                    LabelId = l.LabelId,
                    PasswordId = l.PasswordId,
                    UserId = l.UserId,
                    LabelText = l.LabelText
                }).ToList(),
                Roles = p.Roles?.Select(r => new RoleViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                }).ToList(),
                RoleIds = p.Roles?.Select(r => r.RoleId).ToList(),
                Label = p.Labels?.FirstOrDefault()?.LabelText
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
                UserId = userId
            };

            log.Info($"{GetCurrentUserEmail()} accessed the AddUpdatePassword page.");

            return View(model);
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
        public async Task<IActionResult> CreatePassword(PasswordViewModel passwordViewModel)
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
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            log.Info("Creating password with the following details:");
            log.Info($"UserId: {userId}");
            log.Info($"PasswordName: {passwordViewModel.Password.PasswordName}");
            log.Info($"EncryptedPassword: {passwordViewModel.Password.EncryptedPassword}");
            log.Info($"Label: {passwordViewModel.Label}");

            passwordViewModel.Password.UserId = userId;
            var success = await _passwordService.AddPasswordAsync(passwordViewModel.Password);

            if (success)
            {
                var newPassword = await _passwordService.GetPasswordByIdAsync(passwordViewModel.Password.PasswordId);

                if (newPassword == null)
                {
                    log.Error("Newly added password could not be found.");
                    return Json(new { success = false, message = "Newly added password could not be found." });
                }

                if (newPassword.PasswordId == 0)
                {
                    log.Error("Newly added password has invalid PasswordId.");
                    return Json(new { success = false, message = "Newly added password has invalid PasswordId." });
                }

                if (passwordViewModel.RoleIds != null && passwordViewModel.RoleIds.Any())
                {
                    await _passwordService.UpdatePasswordRolesAsync(newPassword.PasswordId, passwordViewModel.RoleIds);
                }

                if (!string.IsNullOrEmpty(passwordViewModel.Label))
                {
                    var label = new Label
                    {
                        UserId = userId,
                        PasswordId = newPassword.PasswordId,
                        LabelText = passwordViewModel.Label
                    };

                    log.Info("Adding or updating label:");
                    log.Info($"Label UserId: {label.UserId}");
                    log.Info($"Label PasswordId: {label.PasswordId}");
                    log.Info($"Label Text: {label.LabelText}");

                    var labelSuccess = await _labelService.AddOrUpdateLabelAsync(label);

                    if (!labelSuccess)
                    {
                        log.Error("Error adding or updating label");
                        return Json(new { success = false, message = "Error adding or updating label" });
                    }
                }

                log.Info("Password created successfully.");

                return Json(new { success = true });
            }

            log.Warn("Failed to create password.");

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> SavePassword(PasswordViewModel passwordViewModel)
        {
            var userId = (int)HttpContext.Session.GetInt32("UserId");

            if (!await controlForLoginStatus())
            {
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            if (passwordViewModel == null || passwordViewModel.Password == null)
            {
                log.Warn("Invalid password data received.");
                return BadRequest("Invalid password data.");
            }

            passwordViewModel.Password.UserId = userId;
            var success = await _passwordService.UpdatePasswordAsync(passwordViewModel.Password);
            if (success)
            {
                await _passwordService.UpdatePasswordRolesAsync(passwordViewModel.Password.PasswordId, passwordViewModel.RoleIds);

                if (string.IsNullOrEmpty(passwordViewModel.Label))
                {
                    var deleteSuccess = await _labelService.DeleteLabelByPasswordIdAndUserIdAsync(passwordViewModel.Password.PasswordId, userId);

                    if (!deleteSuccess)
                    {
                        log.Error("Error deleting label");
                        return Json(new { success = false, message = "Error deleting label" });
                    }
                }
                else
                {
                    var label = new Label
                    {
                        UserId = userId,
                        PasswordId = passwordViewModel.Password.PasswordId,
                        LabelText = passwordViewModel.Label
                    };

                    var labelSuccess = await _labelService.AddOrUpdateLabelAsync(label);

                    if (!labelSuccess)
                    {
                        log.Error("Error adding or updating label");
                        return Json(new { success = false, message = "Error adding or updating label" });
                    }
                }

                log.Info($"{GetCurrentUserEmail()} saved a password with ID {passwordViewModel.Password.PasswordId}.");

                return Json(new { success = true });
            }

            log.Warn($"{GetCurrentUserEmail()} failed to save a password with ID {passwordViewModel.Password.PasswordId}.");

            return Json(new { success = false });
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePassword(int passwordId)
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
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            var success = await _passwordService.DeletePasswordAsync(passwordId);
            if (success)
            {
                log.Info($"{GetCurrentUserEmail()} deleted a password with ID {passwordId}.");
            }
            else
            {
                log.Warn($"{GetCurrentUserEmail()} failed to delete a password with ID {passwordId}.");
            }
            return Json(new { success });
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

        [HttpGet]
        public async Task<IActionResult> GetAllPasswords()
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
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                log.Warn($"User not found with session ID: {userId}");
                return RedirectToAction("Index", "Login");
            }

            var companyId = user.CompanyId;
            var passwords = await _passwordService.GetCompanyPasswords(companyId);

            var result = passwords.Select(p => new
            {
                p.PasswordId,
                p.PasswordName,
                Labels = p.Labels?.Where(l => l.UserId == userId).Select(l => new { l.LabelText })
            }).ToList();

            log.Info($"{GetCurrentUserEmail()} accessed all passwords.");

            return Json(result);
        }

        public async Task<bool> controlForLoginStatus()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                log.Warn("No valid session found during controlForLoginStatus check.");
                return false; // return RedirectToAction("Index", "Login"); // Login sayfasına yönlendirme
            }

            var user = await _userService.GetUserAndRolesById(sessionUserId.Value);
            if (user == null)
            {
                log.Warn($"User not found with session ID: {sessionUserId}");
                return false; // return RedirectToAction("Index", "Login"); // Kullanıcı bulunamazsa login sayfasına yönlendirme
            }

            var roles = user.Roles.Select(r => r.RoleName).ToList();

            if (!roles.Contains("Admin"))
            {
                log.Warn($"User with session ID: {sessionUserId} does not have admin role.");
                return false; // return RedirectToAction("Index", "User", new { userId = user.UserId });
            }

            log.Info($"User with session ID: {sessionUserId} passed controlForLoginStatus check.");
            return true;
        }
    }
}
