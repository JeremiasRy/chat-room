using backend.Src.Models;
using backend.Src.Services;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Src.Controllers;
[Route("/api/v1/[controller]s")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet]
    public async Task<ChatUser?> GetUser()
    {
        if (HttpContext.Items.ContainsKey("UserId") && Guid.TryParse(HttpContext.Items["UserId"]!.ToString(), out Guid userId))
        {
            return await _userService.GetUserAsync(userId);
        }
        return null;
    }
}
