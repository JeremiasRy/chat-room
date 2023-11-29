using backend.Src.Services;
using backend.Src.Data;
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

services.AddSignalR();
services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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