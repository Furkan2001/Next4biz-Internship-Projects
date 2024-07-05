using Microsoft.AspNetCore.Mvc;
using log4net;
using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly Database _database; // Veritabanı bağlantısı için readonly alan
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController)); // Statik ve readonly logger

        public UserController(Database database)
        {
            _database = database; // Kurucuda veritabanı bağlantısını atama

            // Kullanıcı kimliğini ayarlayın (bu örnekte sabit bir değer kullanılıyor)
            GlobalContext.Properties["UserId"] = 2;

            log.Info("UserController initialized."); // Controller başlatıldığında log mesajı
        }

        public IActionResult Index()
        {
            log.Info("Getting all users."); // Bilgi loglama
            IEnumerable<User> users = _database.GetUsers(); // Veritabanından kullanıcıları çekme
            return View(users); // Kullanıcıları görünüme gönderme
        }

        public IActionResult Details(int id)
        {
            try
            {
                log.Info($"Getting details for user with ID {id}."); // Bilgi loglama
                User user = _database.GetUserById(id); // Veritabanından belirli kullanıcıyı çekme
                IEnumerable<Label> labels = _database.GetLabelsByUserId(id);
                ViewBag.Labels = labels;
                return View(user);
            }
            catch (Exception ex)
            {
                log.Error($"Error getting details for user with ID {id}: {ex.Message}", ex); // Hata loglama
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            try
            {
                log.Info("Creating a new user."); // Bilgi loglama
                _database.AddUser(user); // Veritabanına yeni kullanıcı ekleme
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error($"Error creating user: {ex.Message}", ex); // Hata loglama
                return BadRequest("Error creating user");
            }
        }

        // Diğer CRUD işlemleri için Action metotları oluşturabilirsiniz
    }
}
