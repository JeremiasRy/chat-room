﻿namespace backend.Src.DTOs;

public class DisplayMessageDTO
{
    public string Content { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool Online { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}