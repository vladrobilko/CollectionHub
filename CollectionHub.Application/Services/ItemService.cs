using CollectionHub.DataManagement;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CollectionHub.Domain.Converters;

namespace CollectionHub.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<UserDb> _userManager;

        public ItemService(ApplicationDbContext context, UserManager<UserDb> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateItem(string userName, IFormCollection formCollection)
        {
            long collectionId;
            // Extract the collection view id from the form data
            formCollection.TryGetValue("id", out var collectionIdString);
            long.TryParse(collectionIdString, out collectionId);

            var fieldsWithValues = new Dictionary<string, string>();

            foreach (var key in formCollection.Keys)
            {
                fieldsWithValues[key] = formCollection[key];
            }

            await CheckUserHasCollection(userName, collectionId);

            var itemDbProperties = typeof(ItemDb).GetProperties();
            var newItem = new ItemDb();

            foreach (var item in fieldsWithValues)
            {
                //change keys here from Name to Value
                var property = itemDbProperties.FirstOrDefault(p => p.Name == item.Key.ToItemDbProperty());

                if (item.Key == "Tags")
                {
                    newItem.Tags = ParseTags(item.Value);
                }
                else
                {
                    if (property != null)
                    {
                        var value = item.Value.ToDynamicType(property.PropertyType);
                        var valueRes = ChangeType(value, property.PropertyType);

                        property.SetValue(newItem, valueRes);
                    }
                }
            }

            newItem.CollectionId = collectionId;
            newItem.CreationDate = DateTimeOffset.Now;

            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
        private List<TagDb> ParseTags(string tags)
        {
            char[] separators = { ',', ' ' };

            string[] tagArray = tags.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return tagArray
                .Where(tag => !string.IsNullOrWhiteSpace(tag) && ContainsLettersOrNumbers(tag))
                .Select(tag => new TagDb { Name = tag })
                .ToList();
        }

        private bool ContainsLettersOrNumbers(string input)
        {
            return input.Any(char.IsLetterOrDigit);
        }

        public async Task CheckUserHasCollection(string userName, long collectionId)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstOrDefaultAsync(x => x.Id == collectionId) ??
                throw new AccessViolationException();
        }
    }
}
