using Microsoft.AspNetCore.Mvc;
using PasswordManagementSystem.Services;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(int userId)
        {
            var user = _userService.GetUserById(userId);
            return View(user);
        }
    }
}
