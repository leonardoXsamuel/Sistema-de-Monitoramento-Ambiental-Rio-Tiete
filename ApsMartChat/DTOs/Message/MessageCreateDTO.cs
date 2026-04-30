using ApsMartChat.DTOs.ChatRoom;
using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.User;

public record MessageCreateDTO(
    [Required]
    [StringLength(400, MinimumLength = 1)]
    string Content,

    [Required]
    UserCreateDTO Sender,

    [Required]
    ChatRoomCreateDTO Room
);
    
