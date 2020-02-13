using Microsoft.EntityFrameworkCore;

namespace BukBot.Models.DbModels
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Sound> Sounds { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql("User ID =postgres;Password=1234;Server=localhost;Port=5432;Database=BukBotDb;Integrated Security = true; Pooling=true;");
        
    }
}
