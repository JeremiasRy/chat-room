using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using backend.Src.Services;
using backend.Src.Data;
using backend.Src.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Auth:client_id"];
        options.ClientSecret = builder.Configuration["Auth:client_secret"];
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("RequireGoogleAuthentication", policy =>
    {
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.AuthenticationSchemes.Add(GoogleDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
});

if (builder.Environment.IsDevelopment())
{
    services.AddControllers();
    services.AddSwaggerGen();
}

services
    .AddScoped<IDb, Db>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IMessageService, MessageService>();

services.AddSignalR();
services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
});

app.UseUserInformationMiddleware();

app.MapControllers();

app.Run();