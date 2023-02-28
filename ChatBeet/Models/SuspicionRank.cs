using ChatBeet.Data.Entities;

namespace ChatBeet.Models;

/// <summary>
/// Describes how sus a crewmate is
/// </summary>
public class SuspicionRank
{
    /// <summary>
    /// Nick of user
    /// </summary>
    public required User User { get; set; }

    /// <summary>
    /// Current suspicion level
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Color of crewmate
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Total suspicion ever raised
    /// </summary>
    public int LifetimeLevel { get; set; }
}