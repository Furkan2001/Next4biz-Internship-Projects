using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using PasswordManagementSystem.Models;

public class Database
{
    private readonly string _connectionString;

    public Database(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    // User tablosu ile ilgili işlemler
    public IEnumerable<User> GetUsers()
    {
        using (var connection = GetConnection())
        {
            return connection.Query<User>("SELECT * FROM [User]");
        }
    }

    public User GetUserById(int userId)
    {
        using (var connection = GetConnection())
        {
            var user = connection.QueryFirstOrDefault<User>("SELECT * FROM [User] WHERE UserId = @UserId", new { UserId = userId });
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }
            return user;
        }
    }

    public void AddUser(User user)
    {
        using (var connection = GetConnection())
        {
            var sql = "INSERT INTO [User] (CompanyId, Name, Email, Password) VALUES (@CompanyId, @Name, @Email, @Password)";
            connection.Execute(sql, user);
        }
    }

    // Diğer CRUD işlemleri burada tanımlanabilir

    // Label tablosu ile ilgili işlemler
    public IEnumerable<Label> GetLabelsByUserId(int userId)
    {
        using (var connection = GetConnection())
        {
            return connection.Query<Label>("SELECT * FROM Label WHERE UserId = @UserId", new { UserId = userId });
        }
    }

    public void AddLabel(Label label)
    {
        using (var connection = GetConnection())
        {
            var sql = "INSERT INTO Label (UserId, PasswordId, LabelText) VALUES (@UserId, @PasswordId, @LabelText)";
            connection.Execute(sql, label);
        }
    }

    // Diğer tablolara yönelik işlemler de burada tanımlanabilir
}
