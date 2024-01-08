using CollectionHub.DataManagement;
using CollectionHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectionHub.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ApplicationDbContext _context;

        public CollectionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetAllCategories() => await _context.Categories.Select(x=> x.Name).ToListAsync();

    }
}
