using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class MemoryCellContext : DbContext
    {
        public MemoryCellContext() : base(){}

        public virtual DbSet<MemoryCell> MemoryCells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db/memorycell.db");
        }
    }
}
