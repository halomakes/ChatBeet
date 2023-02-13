using ChatBeet.Data.Entities;

namespace ChatBeet.Models;

public struct TagStat
{
    public string Tag;
    public int Total;
    public StatMode Mode;
    public User? User;

    public enum StatMode
    {
        User,
        Tag
    }
}