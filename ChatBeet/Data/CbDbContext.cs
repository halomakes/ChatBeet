using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public partial class CbDbContext : DbContext
{
    public CbDbContext(DbContextOptions<CbDbContext> opts) : base(opts)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyUtcDateTimeConverter();
        ConfigureUsers(modelBuilder);
        ConfigureBooru(modelBuilder);
        ConfigureDefinitions(modelBuilder);
        ConfigureProgress(modelBuilder);
        ConfigureSuspicion(modelBuilder);
        ConfigureHighGround(modelBuilder);
        ConfigureKarma(modelBuilder);
        ConfigureStats(modelBuilder);
        ConfigureQuotes(modelBuilder);
    }
}