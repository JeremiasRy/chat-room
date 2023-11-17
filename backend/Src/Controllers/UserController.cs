using backend.Src.Models;
using backend.Src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Src.Controllers;

[Authorize(Policy = "RequireGoogleAuthentication")]
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
        if (Guid.TryParse(User.FindFirstValue("user_id"), out Guid userId))
        {
            return await _userService.GetUser(userId);
        }
        return null;
    }
}
