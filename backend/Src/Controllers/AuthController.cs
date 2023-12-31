﻿using backend.Src.DTOs;
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
        if (!_jwtTokenService.CanReadToken(credential.Credential))
        {
            return Unauthorized();
        }
        var result = await _googleVerifierService.VerifyTokenAsync(credential.Credential);
        if (result is null)
        {
            return Unauthorized();
        }
        var userCheck = await _userService.GetUserAsync(email: result.Email);
        if (userCheck is null)
        {
            await _userService.CreateUserAsync(result.Name, result.Email);
            userCheck = await _userService.GetUserAsync(email: result.Email) ?? throw new Exception("Things went south");
        }
        return Ok(_jwtTokenService.CreateToken(userCheck));
    }
    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshToken() 
    {
        if (HttpContext.Items.ContainsKey("UserId") && Guid.TryParse(HttpContext.Items["UserId"]!.ToString(), out Guid userId))
        {
            var user = await _userService.GetUserAsync(userId);
            return user is not null ? Ok(_jwtTokenService.CreateToken(user)) : BadRequest();
        }
        return Unauthorized();
    }
}
