using GTANetworkAPI;
using GTARoleplay.Account;

namespace GTARoleplay.Library.Extensions
{
    public static class PlayerExtensions
    {
        private static readonly string PLAYER_RESTRICTED_STATE = "RestrictedState";

        public static void SendErrorMessage(this Player player, string msg, string errorPrefix = "ERROR")
        {
            player?.SendChatMessage($"~r~{errorPrefix}: ~s~{msg}");
        }

        public static void SendAdminCommandMessage(this Player player, string msg)
        {
            player?.SendChatMessage($"~y~AdmCmd: ~s~{msg}");
        }

        public static void SendModeratorCommandMessage(this Player player, string msg)
        {
            player?.SendChatMessage($"~y~ModCmd: ~s~{msg}");
        }

        public static User GetUserData(this Player player)
        {
            if (player == null || !player.HasData(AccountHandler.USER_DATA))
                return null;

            return player.GetData<User>(AccountHandler.USER_DATA);
        }

        public static void Freeze(this Player player, bool state)
        {
            player?.TriggerEvent("SetFreezeState::Client", state);
        }

        public static void ChangeRestrictedState(this Player player, bool state)
        {
            player?.SetData<bool>(PLAYER_RESTRICTED_STATE, state);
        }

        public static bool GetRestrictedState(this Player player)
        {
            return player != null ? player.GetData<bool>(PLAYER_RESTRICTED_STATE) : false;
        }
    }
}
