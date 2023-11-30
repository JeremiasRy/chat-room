using backend.Src.Services;
using backend.Src.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Src.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

if (builder.Environment.IsDevelopment())
{
    services.AddControllers();
    services.AddSwaggerGen();
}

services
    .AddScoped<IDb, Db>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IMessageService, MessageService>()
    .AddScoped<IGoogleVerifierService, GoogleVerifierService>()
    .AddScoped<IJwtTokenService, JwtTokenService>();

services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

services.AddSignalR();
services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseUserInformationMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(options =>
{
    options
        .AllowAnyOrigin()
        .AllowAnyHeader();
});

app.MapControllers();

app.Run();