﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
        string token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (token == "")
        {
            token = httpContext.Request.Query["access_token"];
        }

        if (token is not null && token != "")
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jsonToken is not null)
            {
                httpContext.Items.Add("UserId", jsonToken.Subject);
                httpContext.Items.Add("Name", jsonToken.Claims.SingleOrDefault(claim => claim.Type == "name")!);
            }
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
