using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;
public class IrcLinkContext : DbContext
{
    public IrcLinkContext(DbContextOptions<IrcLinkContext> optsBuilder) : base(optsBuilder) { }

    public virtual DbSet<IrcLink> Links { get; set; }
}