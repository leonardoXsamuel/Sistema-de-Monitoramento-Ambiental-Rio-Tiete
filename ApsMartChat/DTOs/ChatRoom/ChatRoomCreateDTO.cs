using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.ChatRoom;

public record ChatRoomCreateDTO([Required][StringLength(155, MinimumLength = 1)] string Name);
