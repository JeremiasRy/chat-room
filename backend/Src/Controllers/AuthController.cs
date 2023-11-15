using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace backend.Src.Controllers;
[Route("/api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login() 
    {
        var redirectUrl = Url.Action("loginSuccess", "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }
    [HttpGet("loginSuccess")]
    public async Task<IActionResult> ExternalLoginCallback() 
    {
        var info = await HttpContext.AuthenticateAsync("Google");
        return Ok(new { Message = "Authentication Successful" });
    }
}
