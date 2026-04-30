using ApsMartChat.Services.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApsMartChat.Controllers;

[Authorize]
[ApiController]
[Route("api/rooms/{roomId:int}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messages;

    public MessagesController(IMessageService messages) => _messages = messages;

    /// Retorna histórico paginado de mensagens de uma sala.
    [HttpGet]
    public async Task<IActionResult> GetHistory(int roomId, [FromQuery] int page = 1)
    {
        var history = await _messages.GetHistoryOfMessagesAsync(roomId, page);
        return Ok(history);
    }
}

