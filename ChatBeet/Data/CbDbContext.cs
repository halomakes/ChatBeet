using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public partial class CbDbContext : DbContext
{
    public CbDbContext(DbContextOptions<CbDbContext> opts) : base(opts) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigureBooru(modelBuilder);
        ConfigureKeywords(modelBuilder);
        ConfigureDefinitions(modelBuilder);
        ConfigureProgress(modelBuilder);
        ConfigureHighGround(modelBuilder);
        ConfigureKarma(modelBuilder);
    }
}