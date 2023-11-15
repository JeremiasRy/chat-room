using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Src.Controllers;
[Route("/api/v1/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login() 
    {
        var redirectUrl = Url.Action("loginSuccess", "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }
    [HttpGet("login-success-callback")]
    public async Task<IActionResult> LoginSuccessCallback() 
    {
        var info = await HttpContext.AuthenticateAsync("Google");
        // Do some work here
        return Ok(new { Message = "Authentication Successful" });
    }
}
