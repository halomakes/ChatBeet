using DtellaRules.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DtellaRules.Data
{
    public class DtellaContext : DbContext
    {
        public DtellaContext() : base()
        {

        }

        public virtual DbSet<MemoryCell> MemoryCells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dtella.db");
        }
    }
}
