using BukBot.Models.DbModels;
using BukBot.Repositories.Interfaces;

namespace BukBot.Repositories
{
    public class SoundRepository : GenericRepository<Sound>, ISoundRepository
    {
        public SoundRepository(AppDbContext appDbContext)
        : base(appDbContext)
        { }
    }
}
