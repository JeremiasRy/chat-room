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
