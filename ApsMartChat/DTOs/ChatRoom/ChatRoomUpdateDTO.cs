using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.ChatRoom;

public record ChatRoomUpdateDTO(

    [Required]
    [StringLength(155, MinimumLength = 4)]
    string Name

);