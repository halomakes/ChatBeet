using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class SuspicionContext : DbContext
    {
        public SuspicionContext(DbContextOptions<SuspicionContext> opts) : base(opts)
        {
        }

        public virtual DbSet<Suspicion> Suspicions { get; set; }
    }
}
