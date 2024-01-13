using CollectionHub.DataManagement;
using CollectionHub.Models.Enums;

namespace CollectionHub.Domain.Converters
{
    public static class DataTypeConverter
    {
        public static string[] ToCollectionProperty(this DataType type) =>
            type switch
            {
                DataType.String => new[] { nameof(CollectionDb.String1Name), nameof(CollectionDb.String2Name), nameof(CollectionDb.String3Name) },
                DataType.Integer => new[] { nameof(CollectionDb.Int1Name), nameof(CollectionDb.Int2Name), nameof(CollectionDb.Int3Name) },
                DataType.Text => new[] { nameof(CollectionDb.Text1Name), nameof(CollectionDb.Text2Name), nameof(CollectionDb.Text3Name) },
                DataType.Bool => new[] { nameof(CollectionDb.Bool1Name), nameof(CollectionDb.Bool2Name), nameof(CollectionDb.Bool3Name) },
                DataType.Date => new[] { nameof(CollectionDb.Date1Name), nameof(CollectionDb.Date2Name), nameof(CollectionDb.Date3Name) },
                _ => throw new NotSupportedException(),
            };

        public static string ToInputType(this DataType type) =>
             type switch
             {
                 DataType.String => "text",
                 DataType.Integer => "number",
                 DataType.Bool => "checkbox",
                 DataType.Date => "date",
                 _ => throw new NotSupportedException(),
             };
    }
}
