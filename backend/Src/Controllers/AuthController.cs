using backend.Src.DTOs;
using backend.Src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Src.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IGoogleVerifierService _googleVerifierService;
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;
    public AuthController(IGoogleVerifierService googleVerifierService, IUserService userService, IJwtTokenService jwtTokenService)
    {
        _googleVerifierService = googleVerifierService;
        _userService = userService;
        _jwtTokenService = jwtTokenService;
    }
    [HttpPost]
    public async Task<IActionResult> AuthorizeToken([FromBody] CredentialDTO credential) 
    {
        var result = await _googleVerifierService.VerifyTokenAsync(credential.Credential);
        if (result is null)
        {
            return Unauthorized();
        }
        var userCheck = await _userService.GetUserAsync(name: result.Name);
        if (userCheck is null)
        {
            await _userService.CreateUserAsync(result.Name);
            userCheck = await _userService.GetUserAsync(name: result.Name) ?? throw new Exception("Things went south");
        }
        return Ok(_jwtTokenService.CreateToken(userCheck));
    }
}
