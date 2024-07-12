using log4net;
using Microsoft.AspNetCore.Mvc;
using PasswordManagementSystem.Services;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        private static readonly ILog log = LogManager.GetLogger(typeof(LoginController));

        public LoginController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId.HasValue)
            {
                var user = await _userService.GetUserAndRolesById(sessionUserId.Value);
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
            log.Info("log girisi gonderildi.");
            var user = await _userService.GetUserByEmailAndPassword(email, password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                GlobalContext.Properties["UserId"] = user.UserId;

                var roles = user.Roles.Select(r => r.RoleName).ToList();
                if (roles.Contains("Admin"))
                {
                    Console.WriteLine("admin");
                    return RedirectToAction("Index", "Admin", new { userId = user.UserId });
                }
                else
                {
                    Console.WriteLine("admin değil");
                    return RedirectToAction("Index", "User", new { userId = user.UserId });
                }
            }
            Console.WriteLine("cıktı");

            ViewBag.ErrorMessage = "Invalid login attempt";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Oturum verilerini temizler
            return RedirectToAction("Index", "Login"); // Giriş sayfasına yönlendirir
        }

        public async Task<IActionResult> TestIndex()
        {
            return View();
        }
    }
}
