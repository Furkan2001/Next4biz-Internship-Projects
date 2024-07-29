using Microsoft.Extensions.DependencyInjection;
using log4net;
using log4net.Config;
using System.IO;

namespace PasswordManagementSystem.Core.Extensions
{
    public static class Log4netExtensions
    {
        public static void AddLog4net(this IServiceCollection services)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            services.AddSingleton(LogManager.GetLogger(typeof(Log4netExtensions)));
        }
    }
}
