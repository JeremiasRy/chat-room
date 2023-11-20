using System.Security.Claims;

namespace backend.Src.Middleware;

public class UserInfoMiddleware
{
    private readonly RequestDelegate _next;
    public UserInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        var userId = httpContext.User.FindFirstValue("user_id");
        var name = httpContext.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

        if (name is not null && Guid.TryParse(userId, out Guid userGuid)) 
        {
            httpContext.Items["Id"] = userGuid;
            httpContext.Items["Name"] = name;
        }

        await _next(httpContext);
    }
}

public static class UserInfoMiddlewareExtensions
{
    public static IApplicationBuilder UseUserInformationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserInfoMiddleware>();
    }
}
