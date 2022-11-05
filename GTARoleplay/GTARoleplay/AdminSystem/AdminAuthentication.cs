using GTANetworkAPI;
using GTARoleplay.Account;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.AdminSystem
{
    public static class AdminAuthentication
    {
        private const string NO_PERM_STR = "~r~You don't have permission to use this command!";

        public static bool HasPermission(Player player, StaffRank rankRequired, bool printMsg = true)
        {
            User pUser = player.GetUserData();
            if (pUser == null || pUser.StaffData == null)
            {
                player.SendChatMessage(NO_PERM_STR);
                return false;
            }

            bool hasPermission = pUser.StaffData.Rank >= rankRequired;
            if(!hasPermission && printMsg)
                player.SendChatMessage(NO_PERM_STR);
            
            return hasPermission;
        }

        public static bool HasPermission(Player player, StaffRank rankRequired, ExtraCommand extraPermissions, bool printMsg = true)
        {
            User pUser = player.GetUserData();
            if (pUser == null || pUser.StaffData == null)
            {
                player.SendChatMessage(NO_PERM_STR);
                return false;
            }

            bool hasPermission = pUser.StaffData.Rank >= rankRequired;
            if (!hasPermission)
                hasPermission = pUser.StaffData.SpecialPermissions.HasFlag(extraPermissions);

            if (!hasPermission && printMsg)
                player.SendChatMessage(NO_PERM_STR);

            return hasPermission;
        }
    }
}
