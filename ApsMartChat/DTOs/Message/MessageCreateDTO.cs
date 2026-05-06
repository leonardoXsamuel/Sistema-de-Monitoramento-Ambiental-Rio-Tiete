using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.Message;

public record MessageCreateDTO(
    [Required]
    [StringLength(400, MinimumLength = 1)]
    string Content,

    [Required]
    UserCreateDTO Sender,

    [Required]
    ChatRoomCreateDTO Room
);
    
