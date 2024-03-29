﻿namespace ChatBeet.Data.Entities;

public class TopTag
{
    public Guid UserId { get; set; }
    public required string Tag { get; set; }
    public int Total { get; set; }
    
    public virtual User? User { get; set; }
}