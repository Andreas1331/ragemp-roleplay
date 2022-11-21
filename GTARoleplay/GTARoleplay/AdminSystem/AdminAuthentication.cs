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
            bool hasPermission = false;
            if (pUser == null || pUser.StaffData == null)
                return hasPermission;

            hasPermission = pUser.StaffData.Rank >= rankRequired;
            if(!hasPermission && printMsg)
                player.SendChatMessage(NO_PERM_STR);
            
            return hasPermission;
        }

        public static bool HasPermission(Player player, StaffRank rankRequired, ExtraCommand extraPermissions, bool printMsg = true)
        {
            User pUser = player.GetUserData();
            bool hasPermission = false;
            if (pUser == null || pUser.StaffData == null)
                return hasPermission;

            hasPermission = pUser.StaffData.Rank >= rankRequired;
            // Check if the user has special permissions bypassing the regular ranks
            if (!hasPermission)
                hasPermission = pUser.StaffData.SpecialPermissions.HasFlag(extraPermissions);

            if (!hasPermission && printMsg)
                player.SendChatMessage(NO_PERM_STR);

            return hasPermission;
        }
    }
}
