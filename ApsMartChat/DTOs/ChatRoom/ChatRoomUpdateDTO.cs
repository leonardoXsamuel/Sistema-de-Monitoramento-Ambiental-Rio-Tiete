using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.ChatRoom;

public class ChatRoomUpdateDTO
{
    [Required]
    [StringLength(155, MinimumLength = 4)]
    public string Name { get; set; }
}