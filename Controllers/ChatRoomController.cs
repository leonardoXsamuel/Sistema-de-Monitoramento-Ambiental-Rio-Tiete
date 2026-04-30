using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.Services.ChatRoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApsMartChat.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatRoomController : ControllerBase
{
    private readonly IChatRoomService _service;

    public ChatRoomController(IChatRoomService service) => _service = service;

    [HttpPut("updateChatRoom/{roomId}")]
    public async Task<ActionResult<ChatRoomResponseDTO>> AlterarNomeChatRoom([FromRoute] int roomId, [FromBody] ChatRoomUpdateDTO chatRoomUpdateDTO)
    {
        var dto = await _service.AlterarNomeChatRoom(roomId, chatRoomUpdateDTO);

        if (dto is null)
            return BadRequest();

        return Ok(dto);
    }

    [HttpPost("createChatRoom")]
    public async Task<ActionResult<ChatRoomResponseDTO>> CriarChatRoom([FromBody] ChatRoomCreateDTO CreateDTO)
    {
        var dto = await _service.CriarChatRoomAsync(CreateDTO);

        if (dto is null)
            return BadRequest();

        return Ok(dto);
    }
}
