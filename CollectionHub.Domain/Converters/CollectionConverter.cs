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

                WriteCsvHeader(csv, collection.AllHeaders.Values);

                WriteCsvData(csv, collection.Items);

                return Encoding.UTF8.GetBytes(writer.ToString());
            }
        }

        private static void WriteCsvHeader(CsvWriter csv, IEnumerable<string> headers)
        {
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }

            csv.NextRecord();
        }

        private static void WriteCsvData(CsvWriter csv, List<List<string>> items)
        {
            foreach (var row in items)
            {
                foreach (var value in row.Skip(1))
                {
                    csv.WriteField(value);
                }

                csv.NextRecord();
            }
        }
    }
}
