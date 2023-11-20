using backend.Src.Services;
using backend.Src.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using backend.Src.Models;
using System.Collections.Concurrent;

namespace backend.Src.Connections;

[Authorize("RequireGoogleAuthentication")]
[Route("/chat")]
public class ChatHub : Hub
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    public ChatHub(IHttpContextAccessor httpContextAccessor, IMessageService messageService, IUserService userService)
    {
        _messageService = messageService;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }
    public async Task SendMessage(string message)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return;
        }

        if (await _messageService.CreateMessageAsync(new MessageDTO() { Content = message, UserId = (Guid)httpContext.Items["Id"]! }))
        {
            await Clients.All.SendAsync("ReceiveMessage", new DisplayMessageDTO() { Content = message, Name = (string)httpContext.Items["Name"]! });
        }
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }
        await _userService.LoginUserAsync((Guid)httpContext.Items["Id"]!, Context.ConnectionId);
        await base.OnConnectedAsync();
    }
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }
        await _userService.LogoutUserAsync((Guid)httpContext.Items["Id"]!);
        await base.OnDisconnectedAsync(exception);
    }
    public async Task NotifyConnectedUsers()
    {
        var result = await _userService.GetLoggedInUsersAsync();
        await Clients.All.SendAsync("ConnectedUsers", result);
    }
}
