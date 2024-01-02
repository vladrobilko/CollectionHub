using CollectionHub.Models;

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
                _ => throw new ArgumentException()
            };
    }
}
