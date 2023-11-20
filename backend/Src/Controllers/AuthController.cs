using backend.Src.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Src.Controllers;
[Route("/api/v1/[controller]s")]
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
        var userFromDb = await _userService.GetUserAsync(null, identity.Name);
        if (userFromDb is null)
        {
            await _userService.CreateUserAsync(identity.Name!);
            userFromDb = await _userService.GetUserAsync(null, identity.Name) ?? throw new Exception("Things shouldn't fail here?");
        }
        
        if (!((ClaimsIdentity)identity).Claims.Any(claim => claim.Type == "user_id"))
        {
            ((ClaimsIdentity)identity).AddClaim(new Claim("user_id", userFromDb.Id.ToString()));
        }
        await HttpContext.SignInAsync("Cookies", info.Principal!, info.Ticket.Properties);

        return Ok(new { Message = "Authentication Successful" });
    }
}
