using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PasswordManagementSystem.Dtos;
using System.Data.SqlClient;

namespace PasswordManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _db.QueryAsync<User>("SELECT * FROM [User]");
        }

        public async Task<User> GetById(int id)
        {
            var user = await _db.QueryFirstOrDefaultAsync<User>("SELECT * FROM [User] WHERE UserId = @Id", new { Id = id });
            Console.WriteLine(id);
            if (user == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı.");
            }

            return user;
        }

        //public async task<user> getbyemailandpassword(string email, string password)
        //{
        //    var user = await _db.queryfirstordefaultasync<user>("select * from [user] where email = @email", new { email = email });

        //    /*if (user != null && user.password == hashpassword(password))
        //    {
        //        return user;
        //    }*/

        //    if (user != null && user.password == password)
        //    {
        //        return user;
        //    }

        //    return null;
        //}

        public async Task<User> GetByEmailAndPassword(string email, string password)
        {
            // İlk olarak, verilen email adresine sahip kullanıcıyı veritabanından alır.
            var user = await _db.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM [User] WHERE Email = @Email",
                new { Email = email }
            );

            // Kullanıcı bulunduysa ve şifresi doğruysa...
            if (user != null && user.Password == (password))
            {
                // Kullanıcının rollerini almak için bir SQL sorgusu çalıştırır.
                var roles = await _db.QueryAsync<Role>(
                    "SELECT r.* FROM [Role] r INNER JOIN UserRole ur ON r.RoleId = ur.RoleId WHERE ur.UserId = @UserId",
                    new { UserId = user.UserId }
                );

                // Alınan rolleri user nesnesinin Roles listesini doldurur.
                user.Roles = roles.ToList();
                return user;
            }

            // Kullanıcı bulunamazsa veya şifre yanlışsa, null döner.
            return null;
        }

        public async Task<IEnumerable<Role>> GetUserRoles(int userId)
        {
            return await _db.QueryAsync<Role>("SELECT r.* FROM [Role] r INNER JOIN [UserRole] ur ON r.RoleId = ur.RoleId WHERE ur.UserId = @UserId", new { UserId = userId });
        }

        public async Task<Company> GetUserCompany(int userId)
        {
            return await _db.QueryFirstOrDefaultAsync<Company>("SELECT c.* FROM [Company] c INNER JOIN [User] u ON c.CompanyId = u.CompanyId WHERE u.UserId = @UserId", new { UserId = userId });
        }

        public async Task<IEnumerable<User>> GetUserInCompanyId(int companyId)
        {
            return await _db.QueryAsync<User>("SELECT * FROM [User] WHERE CompanyId = @cId", new { cId = companyId });
        }

        public async Task<List<User>> GetUsersByRoleId(int roleId)
        {
            var sql = @"
                SELECT u.*
                FROM [User] u
                INNER JOIN [UserRole] ur ON u.UserId = ur.UserId
                WHERE ur.RoleId = @RoleId";

            return (await _db.QueryAsync<User>(sql, new { RoleId = roleId })).ToList();
        }

        public async Task<bool> InsertUser(User user, List<int> roleIds)
        {
            Console.WriteLine("InsertUser Method Called");
            Console.WriteLine("UserID: " + user.UserId);
            Console.WriteLine("User Name: " + user.Name);
            Console.WriteLine("User Email: " + user.Email);
            Console.WriteLine("User Password: " + user.Password);
            Console.WriteLine("User CompanyId: " + user.CompanyId);
            Console.WriteLine("Roles: " + string.Join(", ", roleIds));

            // Bağlantının açık olduğundan emin olun
            if (_db.State != ConnectionState.Open)
            {
                Console.WriteLine("Opening Database Connection");
                _db.Open(); // Senkron bağlantı açma işlemi
            }

            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    var sql = @"
                INSERT INTO [User] (Name, Email, Password, CompanyId) 
                VALUES (@Name, @Email, @Password, @CompanyId);
                SELECT CAST(SCOPE_IDENTITY() as int)";
                    var userId = await _db.ExecuteScalarAsync<int>(sql, user, transaction);
                    user.UserId = userId;

                    Console.WriteLine("Inserted User ID: " + userId);

                    var insertRolesSql = @"
                INSERT INTO [UserRole] (UserId, RoleId) 
                VALUES (@UserId, @RoleId)";
                    foreach (var roleId in roleIds)
                    {
                        Console.WriteLine("Inserting Role ID: " + roleId);
                        await _db.ExecuteAsync(insertRolesSql, new { UserId = userId, RoleId = roleId }, transaction);
                    }

                    transaction.Commit();
                    Console.WriteLine("Transaction Committed");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    transaction.Rollback();
                    Console.WriteLine("Transaction Rolled Back");
                    return false;
                }
            }
        }


        public async Task<bool> UpdateUser(User user, List<int> roleIds)
        {
            // Bağlantının açık olduğundan emin olun
            if (_db.State != ConnectionState.Open)
            {
                _db.Open(); // Senkron bağlantı açma işlemi
            }

            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    var sql = @"
                        UPDATE [User] 
                        SET Name = @Name, Email = @Email, Password = @Password, CompanyId = @CompanyId
                        WHERE UserId = @UserId";
                    await _db.ExecuteAsync(sql, user, transaction);

                    Console.WriteLine("userrepo2");

                    var deleteRolesSql = "DELETE FROM [UserRole] WHERE UserId = @UserId";
                    await _db.ExecuteAsync(deleteRolesSql, new { UserId = user.UserId }, transaction);

                    Console.WriteLine("userrepo3");

                    var insertRolesSql = @"
                        INSERT INTO [UserRole] (UserId, RoleId) 
                        VALUES (@UserId, @RoleId)";
                    foreach (var roleId in roleIds)
                    {
                        await _db.ExecuteAsync(insertRolesSql, new { UserId = user.UserId, RoleId = roleId }, transaction);
                    }

                    Console.WriteLine("userrepo4");

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            // Bağlantının açık olduğundan emin olun
            if (_db.State != ConnectionState.Open)
            {
                _db.Open(); // Senkron bağlantı açma işlemi
            }

            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    // Delete roles of the user from UserRole table
                    var sqlDeleteRoles = "DELETE FROM UserRole WHERE UserId = @UserId";
                    await _db.ExecuteAsync(sqlDeleteRoles, new { UserId = userId }, transaction);

                    // Delete the user from User table
                    var sqlDeleteUser = "DELETE FROM [User] WHERE UserId = @UserId";
                    var rowsAffected = await _db.ExecuteAsync(sqlDeleteUser, new { UserId = userId }, transaction);

                    transaction.Commit();
                    return rowsAffected > 0;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task Add(User entity)
        {
            // Şifreyi hashle
            entity.Password = HashPassword(entity.Password);

            // SQL sorgusunu tanımla
            var sql = "INSERT INTO [User] (Name, Email, Password, CompanyId) VALUES (@Name, @Email, @Password, @CompanyId)";

            // Veritabanına ekle
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(User entity)
        {
            // Şifrenin değişip değişmediğini kontrol edin ve değişmişse hashleyin
            var existingUser = await _db.QueryFirstOrDefaultAsync<User>("SELECT Password FROM [User] WHERE UserId = @Id", new { Id = entity.UserId });
            if (existingUser != null && existingUser.Password != entity.Password)
            {
                entity.Password = HashPassword(entity.Password);
            }

            // SQL sorgusunu tanımla
            var sql = "UPDATE [User] SET Name = @Name, Email = @Email, Password = @Password, CompanyId = @CompanyId WHERE UserId = @Id";

            // Veritabanında güncelleme yap
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM [User] WHERE UserId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            return await _db.QueryAsync<Role>("SELECT * FROM [Role]");
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            return await _db.QueryAsync<Company>("SELECT * FROM [Company]");
        }

        public async Task<IEnumerable<Password>> GetAllPasswords()
        {
            return await _db.QueryAsync<Password>("SELECT * FROM [Password]"); // Örnek olarak eklenmiş
        }
    }
}
