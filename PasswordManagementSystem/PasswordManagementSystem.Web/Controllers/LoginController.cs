using log4net;
using Microsoft.AspNetCore.Mvc;
using PasswordManagementSystem.Business.Services;
using PasswordManagementSystem.Core.Helpers;
using PasswordManagementSystem.Core.Dtos;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private static readonly ILog log = LogManager.GetLogger(typeof(LoginController));
        private readonly EmailHelper _emailSender;

        public LoginController(IUserService userService, EmailHelper emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId.HasValue)
            {
                var user = await _userService.GetUserById(sessionUserId.Value);
                if (user == null)
                {
                    return RedirectToAction("Index", "Login"); // Kullanıcı bulunamazsa login sayfasına yönlendirme
                }

                var roles = user.Roles.Select(r => r.RoleName).ToList();

                // Kullanıcı ve rollerini loglayın
                Console.WriteLine("User: " + user.Email);
                Console.WriteLine("Roles: " + string.Join(", ", roles));

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin", new { userId = user.UserId }); // Admin sayfasına yönlendirme
                }
                else
                {
                    return RedirectToAction("Index", "User", new { userId = user.UserId }); // Kullanıcı sayfasına yönlendirme
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            var user = await _userService.GetUserByEmailAndPassword(email, password);

            if (user != null)
            {
                var requestedCompany = HttpContext.Items["CompanyName"] as string;
                if (user.CompanyName.Equals(requestedCompany, StringComparison.OrdinalIgnoreCase))
                {
                    var verificationCode = new Random().Next(100000, 999999).ToString();
                    HttpContext.Session.SetString("VerificationCode", verificationCode);
                    HttpContext.Session.SetString("VerificationEmail", email);

                    await _emailSender.SendEmailAsync(user.Email, "Your Verification Code",
                        $"Your verification code is {verificationCode}");

                    return RedirectToAction("VerifyCode", new { userId = user.UserId });
                }
            }

            ViewBag.ErrorMessage = "Invalid login attempt";
            return View();
        }

        [HttpGet]
        public IActionResult VerifyCode(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(int userId, string code)
        {
            var sessionCode = HttpContext.Session.GetString("VerificationCode");
            var sessionEmail = HttpContext.Session.GetString("VerificationEmail");

            if (code == sessionCode)
            {
                // Kullanıcıyı giriş sayfasına yönlendirme
                var user = await _userService.GetUserAndRolesById(userId); // Kullanıcıyı userId ile al
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", userId);
                    GlobalContext.Properties["UserId"] = userId;

                    var roles = user.Roles.Select(r => r.RoleName).ToList();
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin", new { userId = user.UserId });
                    }
                    else
                    {
                        return RedirectToAction("Index", "User", new { userId = user.UserId });
                    }
                }
            }

            ViewBag.ErrorMessage = "Invalid verification code";
            ViewBag.UserId = userId;
            return View();
        }

        public IActionResult Logout()
        {
            log.Info("cikis yapildi.");
            HttpContext.Session.Clear(); // Oturum verilerini temizler
            return RedirectToAction("Index", "Login"); // Giriş sayfasına yönlendirir
        }
    }
}
