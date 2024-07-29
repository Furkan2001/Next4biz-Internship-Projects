namespace PasswordManagementSystem.Web.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var host = context.Request.Host.Host;
            var subdomain = host.Split('.')[0]; // "company1" gibi subdomain adını alır.
            if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
            {
                context.Items["CompanyName"] = subdomain;
            }

            await _next(context);
        }
    }
}
