using ApsMartChat.Models;
using Microsoft.EntityFrameworkCore;

namespace ApsMartChat.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<FileTransfer> FileTransfers { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();

            e.Property(u => u.Role)
             .HasConversion<string>()
             .IsRequired();
        });

        // ChatRoom
        modelBuilder.Entity<ChatRoom>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).HasMaxLength(100).IsRequired();
        });

        // Message
        modelBuilder.Entity<Message>(e =>
        {
            e.HasKey(m => m.Id);
            e.HasOne(m => m.Sender)
             .WithMany(u => u.Messages)
             .HasForeignKey(m => m.SenderId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Room)
             .WithMany(r => r.Messages)
             .HasForeignKey(m => m.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // FileTransfer
        modelBuilder.Entity<FileTransfer>(e =>
        {
            e.HasKey(f => f.Id);
            e.HasOne(f => f.Uploader)
             .WithMany(u => u.FileTransfers)
             .HasForeignKey(f => f.UploaderId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(f => f.Room)
             .WithMany(r => r.FileTransfers)
             .HasForeignKey(f => f.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed: sala padrão (Monitoramento Rio Tietê)
        modelBuilder.Entity<ChatRoom>().HasData(
            new ChatRoom { Id = 1, Name = "Monitoramento Rio Tietê", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
