using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.Models;

public class Message
{
    public int Id { get; set; }
    [StringLength(400, MinimumLength = 1), Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(400, MinimumLength = 1), Required]
    public DateTime SentAt { get; set; }

    public int SenderId { get; set; }
    [Required]
    public User Sender { get; set; } = null!;

    public int RoomId { get; set; }
    [Required]
    public ChatRoom Room { get; set; } = null!;
}