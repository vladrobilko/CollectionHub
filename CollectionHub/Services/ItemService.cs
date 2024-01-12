using CollectionHub.DataManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CollectionHub.Services
{
    public class ItemService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<UserDb> _userManager;

        public ItemService(ApplicationDbContext context, UserManager<UserDb> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateItem(Dictionary<string, string> fieldsWithValues)
        {
            //check user find collection 
            // Assuming itemDbProperties is a list of PropertyInfo objects for ItemDb
            var itemDbProperties = typeof(ItemDb).GetProperties();
            var newItem = new ItemDb();
            // Set property values using reflection
            foreach (var item in fieldsWithValues)
            {
                var property = itemDbProperties.First(p => p.Name == item.Key);

                property?.SetValue(newItem, Convert.ChangeType(item.Value, property.PropertyType));
            }

            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();
        }
    }
}
