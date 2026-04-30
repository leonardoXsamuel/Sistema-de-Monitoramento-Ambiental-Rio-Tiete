
using ApsMartChat.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(155, MinimumLength = 3)]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(155, MinimumLength = 3)]
    public string DisplayName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.Inspetor; // inspetor | coordenador
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<FileTransfer> FileTransfers { get; set; } = new List<FileTransfer>();
}