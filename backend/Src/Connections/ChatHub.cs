using backend.Src.Services;
using backend.Src.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend.Src.Connections;
[Authorize]
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

        if (await _messageService.CreateMessageAsync(new MessageDTO() { Content = message, UserId = (Guid)httpContext.Items["UserId"]! }))
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
        if (httpContext.Items.ContainsKey("UserId") && Guid.TryParse(httpContext.Items["UserId"]!.ToString(), out Guid userId))
        {
            await _userService.LoginUserAsync(userId, Context.ConnectionId);
            await NotifyConnectedUsers();
            await base.OnConnectedAsync();
        }
    }
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }
        if (httpContext.Items.ContainsKey("UserId") && Guid.TryParse(httpContext.Items["UserId"]!.ToString(), out Guid userId))
        {
            await _userService.LogoutUserAsync(userId);
            await NotifyConnectedUsers();
            await base.OnConnectedAsync();
        }
    }
    public async Task NotifyConnectedUsers()
    {
        var result = await _userService.GetLoggedInUsersAsync();
        await Clients.All.SendAsync("ConnectedUsers", result);
    }
}
