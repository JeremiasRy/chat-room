using System.ComponentModel.DataAnnotations;

namespace backend.Src.DTOs;

public class MessageDTO
{
    [Required]
    public string Content { get; set; } = null!;
    [Required]
    public Guid UserId { get; set; }
}
