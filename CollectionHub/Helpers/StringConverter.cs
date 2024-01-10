using CollectionHub.Models;
using CollectionHub.Models.Enums;

namespace CollectionHub.Helpers
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
                "String" or "String1Name" or "String2Name" or "String3Name" or "Name" or "Tags" => DataType.String,
                "Integer" or "Int1Name" or "Int2Name" or "Int3Name" => DataType.Integer,
                "Text" or "Text1Name" or "Text2Name" or "Text3Name" => DataType.Text,
                "Bool" or "Bool1Name" or "Bool2Name" or "Bool3Name" => DataType.Bool,
                "Date" or "Date1Name" or "Date2Name" or "Date3Name" => DataType.Date,
                _ => throw new NotSupportedException()
            };
    }
}
