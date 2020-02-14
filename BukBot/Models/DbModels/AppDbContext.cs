using Microsoft.EntityFrameworkCore;

namespace BukBot.Models.DbModels
{
    /// <summary>
    /// FOR FUTURE DEV NEEDS
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql("User ID =postgres;Password=1234;Server=localhost;Port=5432;Database=BukBotDb;Integrated Security = true; Pooling=true;");
        
    }
}
