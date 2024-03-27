using GTANetworkAPI;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.AdminSystem
{
    internal class AdminUtilityCommands
    {
        public AdminUtilityCommands()
        {
            NAPI.Command.Register<Player>(
                new RuntimeCommandInfo("mypos")
                {
                    ClassInstance = this
                }, PrintMyPosition);
        }

        public void PrintMyPosition(Player player)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Vector3 pos = player.Position;
                player.SendChatMessage($"Your position ({pos.X}, {pos.Y}, {pos.Z})");
            }
        }
    }
}
