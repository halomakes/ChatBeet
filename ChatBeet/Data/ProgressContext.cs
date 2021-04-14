using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class ProgressContext : DbContext
    {
        public ProgressContext(DbContextOptions<ProgressContext> optsBuilder) : base(optsBuilder) { }

        public virtual DbSet<FixedTimeRange> FixedTimeRanges { get; set; }
    }
}
