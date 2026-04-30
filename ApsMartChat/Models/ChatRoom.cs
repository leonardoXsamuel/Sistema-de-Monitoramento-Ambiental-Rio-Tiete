using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.Models;

public class ChatRoom
{
    public int Id { get; set; }
    
    [Required, StringLength(55, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<FileTransfer> FileTransfers { get; set; } = new List<FileTransfer>();
}
