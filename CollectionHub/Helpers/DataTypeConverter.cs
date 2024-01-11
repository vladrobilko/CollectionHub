using CollectionHub.Models.Enums;

namespace CollectionHub.Helpers
{
    public static class DataTypeConverter
    {
        public static string[] ToPropertyNames(this DataType type) =>
            type switch
            {
                DataType.String => new[] { "String1Name", "String2Name", "String3Name" },
                DataType.Integer => new[] { "Int1Name", "Int2Name", "Int3Name" },
                DataType.Text => new[] { "Text1Name", "Text2Name", "Text3Name" },
                DataType.Bool => new[] { "Bool1Name", "Bool2Name", "Bool3Name" },
                DataType.Date => new[] { "Date1Name", "Date2Name", "Date3Name" },
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
