namespace CollectionHub.Domain.Converters
{
    public static class DictionaryConverter
    {
        public static List<string> ToItemPropertyNames(this Dictionary<string, string> collectionNames)
        {
            var result = new List<string>();

            foreach (var item in collectionNames.Keys)
            {
                result.Add(item.ToItemDbProperty());
            }

            return result;
        }
    }
}
