using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChatBeet.Data
{
    public class SuspicionContext : DbContext
    {
        public SuspicionContext(DbContextOptions<SuspicionContext> opts) : base(opts) { }

        public virtual DbSet<Suspicion> Suspicions { get; set; }

        public Task<int> GetSuspicionLevelAsync(string suspect) => Suspicions.CountAsync(s => s.Suspect.ToLower() == suspect.ToLower());
    }
}
