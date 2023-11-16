using backend.Src.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace backend.Src.Controllers;
[Route("/api/v1/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet("login")]
    public IActionResult Login() 
    {
        var redirectUrl = Url.Action("LoginSuccessCallback", "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }
    [HttpGet("login-success-callback")]
    public async Task<IActionResult> LoginSuccessCallback() 
    {
        var info = await HttpContext.AuthenticateAsync("Google");
        if (info.Failure is not null)
        {
            return Unauthorized();
        }
        var identity = info.Ticket!.Principal.Identity;

        if (identity is null)
        {
            return BadRequest();
        }

        var result = await _userService.CreateUser(identity.Name!);
        if (result is not null)
        {
            return Ok(new { Message = "Authentication succesfull and created user!" });
        }
        return Ok(new { Message = "Authentication Successful" });
    }
}
