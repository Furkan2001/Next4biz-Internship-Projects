using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using PasswordManagementSystem.Data.Repositories;
using PasswordManagementSystem.Data.Interfaces;
using log4net;
using log4net.Config;
using PasswordManagementSystem.Business.Services;
using PasswordManagementSystem.Core.Helpers;
using PasswordManagementSystem.Core.Configurations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using PasswordManagementSystem.Business.Services.Impl;
using PasswordManagementSystem.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<EmailHelper>();

// Registering controllers with views
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<DataProtectionHelper>();

// Database configuration for dependency injection
builder.Services.AddTransient<Database>(); // Ensure this class is properly defined to handle DB operations
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();
builder.Services.AddScoped<IPasswordRoleRepository, PasswordRoleRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IPasswordRoleService, PasswordRoleService>();
builder.Services.AddScoped<ILabelService, LabelService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout süresini belirleyin
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// log4net configuration
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
var configFileInfo = new FileInfo("log4net.config");
if (configFileInfo.Exists)
{
    XmlConfigurator.Configure(logRepository, configFileInfo);
    Console.WriteLine("log4net configuration loaded successfully.");
}
else
{
    Console.WriteLine("log4net configuration file not found.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>(); // Şirket adı çıkarma middleware'ini kullan

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
