using backend.Src.DTOs;
using backend.Src.Filters;
using backend.Src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Src.Controllers;
[ApiController]
[Route("/api/v1/[controller]s")]
[Authorize(Policy = "RequireGoogleAuthentication")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _service;
    public MessageController(IMessageService service)
    {
        _service = service;
    }
    [HttpGet]
    public async Task<List<DisplayMessageDTO>> GetMessages([FromQuery] DateTime? lastCreatedAt, [FromQuery] int pageSize = 20)
    {
        var filter = new Pagination() { PageSize = pageSize, LastCreatedAt = lastCreatedAt };
        return await _service.GetMessagesAsync(filter);
    }
    [HttpPost]
    public async Task<IActionResult> CreateMessage(MessageDTO request)
    {
        if (Guid.TryParse(User.FindFirstValue("user_id"), out Guid userId))
        {
            if (userId == request.UserId)
            {
                await _service.CreateMessageAsync(request);
                return Ok();
            }
        }
        return Unauthorized();
    }
}
