using CollectionHub.Models.ViewModels;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace CollectionHub.Domain.Converters
{
    public static class CollectionConverter
    {
        public static byte[] ToCsvBytes(this CollectionViewModel collection)
        {
            using (var writer = new StringWriter())
            {
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                foreach (var header in collection.AllHeaders.Values)
                {
                    csv.WriteField(header);
                }
                csv.NextRecord();

                for (int i = 0; i < collection.Items.Count; i++)
                {
                    for (int j = 0; j < collection.AllHeaders.Count; j++)
                    {
                        var value = collection.Items[i][j + 1];
                        csv.WriteField(value);
                    }
                    csv.NextRecord();
                }

                return Encoding.UTF8.GetBytes(writer.ToString());
            }
        }
    }
}
