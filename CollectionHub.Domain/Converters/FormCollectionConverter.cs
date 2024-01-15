using Microsoft.AspNetCore.Http;

namespace CollectionHub.Domain.Converters
{
    public static class FormCollectionConverter
    {
        public static Dictionary<string, string> ToDictionary(this IFormCollection formCollection)
        {
            var fieldsWithValues = new Dictionary<string, string>();

            foreach (var key in formCollection.Keys)
            {
                fieldsWithValues[key] = formCollection[key];
            }

            return fieldsWithValues;
        }

        public static long ToId(this IFormCollection formCollection)
        {
            if (formCollection.TryGetValue("id", out var idString) && long.TryParse(idString, out var collectionId))
            {
                return collectionId;
            }

            throw new FormatException();
        }
    }
}
