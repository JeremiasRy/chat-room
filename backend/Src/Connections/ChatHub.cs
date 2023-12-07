using backend.Src.Services;
using backend.Src.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;

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

    private static string MakeResponseToProperJson(object responseObj)
    {
        return JsonSerializer.Serialize(responseObj, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });
    }
    public async Task SendMessage(string message)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return;
        }

        if (httpContext.Items.ContainsKey("UserId") && Guid.TryParse(httpContext.Items["UserId"]!.ToString(), out Guid userId))
        {
            if (await _messageService.CreateMessageAsync(new MessageDTO() { Content = message, UserId = userId }))
            {
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var response = MakeResponseToProperJson(new DisplayMessageDTO() { Content = message, Name = ((Claim)httpContext.Items["Name"]!).Value });
                await Clients.All.SendAsync("ReceiveMessage", response);
            }
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
        await Clients.All.SendAsync("ConnectedUsers", MakeResponseToProperJson(result));
    }
}
