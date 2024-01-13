using CollectionHub.DataManagement;
using CollectionHub.Models;
using CollectionHub.Models.Enums;

namespace CollectionHub.Domain.Converters
{
    public static class StringConverter
    {
        public static UserManageActions ToUserManageActions(this string action) =>
            action switch
            {
                "block" => UserManageActions.Block,
                "unblock" => UserManageActions.Unblock,
                "delete" => UserManageActions.Delete,
                "makeadmin" => UserManageActions.MakeAdmin,
                "makeuser" => UserManageActions.MakeUser,
                _ => throw new NotSupportedException()
            };

        public static DataType ToDataType(this string dataType) =>
            dataType switch
            {
                "String" or nameof(CollectionDb.String1Name)
                or nameof(CollectionDb.String2Name) or nameof(CollectionDb.String3Name) or nameof(ItemDb.Name) or nameof(ItemDb.Tags) => DataType.String,

                "Integer" or nameof(CollectionDb.Int1Name) or nameof(CollectionDb.Int2Name) or nameof(CollectionDb.Int3Name) => DataType.Integer,

                "Text" or nameof(CollectionDb.Text1Name) or nameof(CollectionDb.Text2Name) or nameof(CollectionDb.Text3Name) => DataType.Text,

                "Bool" or nameof(CollectionDb.Bool1Name) or nameof(CollectionDb.Bool2Name) or nameof(CollectionDb.Bool3Name) => DataType.Bool,

                "Date" or nameof(CollectionDb.Date1Name) or nameof(CollectionDb.Date2Name) or nameof(CollectionDb.Date3Name) => DataType.Date,

                _ => throw new NotSupportedException()
            };

        public static string ToItemDbProperty(this string propertyName) =>
            propertyName switch
            {
                nameof(CollectionDb.String1Name) => nameof(ItemDb.String1Value),
                nameof(CollectionDb.String2Name) => nameof(ItemDb.String2Value),
                nameof(CollectionDb.String3Name) => nameof(ItemDb.String3Value),
                nameof(CollectionDb.Int1Name) => nameof(ItemDb.Int1Value),
                nameof(CollectionDb.Int2Name) => nameof(ItemDb.Int2Value),
                nameof(CollectionDb.Int3Name) => nameof(ItemDb.Int3Value),
                nameof(CollectionDb.Text1Name) => nameof(ItemDb.Text1Value),
                nameof(CollectionDb.Text2Name) => nameof(ItemDb.Text2Value),
                nameof(CollectionDb.Text3Name) => nameof(ItemDb.Text3Value),
                nameof(CollectionDb.Bool1Name) => nameof(ItemDb.Bool1Value),
                nameof(CollectionDb.Bool2Name) => nameof(ItemDb.Bool2Value),
                nameof(CollectionDb.Bool3Name) => nameof(ItemDb.Bool3Value),
                nameof(CollectionDb.Date1Name) => nameof(ItemDb.Date1Value),
                nameof(CollectionDb.Date2Name) => nameof(ItemDb.Date2Value),
                nameof(CollectionDb.Date3Name) => nameof(ItemDb.Date3Value),
                _ => propertyName
            };

        public static dynamic ToDynamicType(this string value, Type type)
        {
            return type switch
            {
                _ when type == typeof(string) => value == string.Empty ? "-" : value,
                _ when type == typeof(long?) => value == string.Empty ? ulong.MinValue : long.Parse(value),
                _ when type == typeof(bool?) => value != string.Empty,
                _ when type == typeof(DateTimeOffset?) => DateTimeOffset.TryParse(value, out var dateTimeOffsetValue) ? (DateTimeOffset?)dateTimeOffsetValue : DateTimeOffset.MinValue,

                _ => value
            };
        }

        public static string[] GetCollectionFieldNames() =>
            [
                nameof(CollectionDb.String1Name),
                nameof(CollectionDb.String2Name),
                nameof(CollectionDb.String3Name),
                nameof(CollectionDb.Int1Name),
                nameof(CollectionDb.Int2Name),
                nameof(CollectionDb.Int3Name),
                nameof(CollectionDb.Text1Name),
                nameof(CollectionDb.Text2Name),
                nameof(CollectionDb.Text3Name),
                nameof(CollectionDb.Bool1Name),
                nameof(CollectionDb.Bool2Name),
                nameof(CollectionDb.Bool3Name),
                nameof(CollectionDb.Date1Name),
                nameof(CollectionDb.Date2Name),
                nameof(CollectionDb.Date3Name)];
    }
}
