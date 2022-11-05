using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.InventorySystem
{
    public class InventoryHandler : Script
    {
        public static void ShowInventory(Player player)
        {
            // if the player is restricted (cuffed etc.) then return
            if (player == null || player.GetRestrictedState())
                return;

            Inventory inv = player.GetUserData()?.ActiveCharacter?.Inventory;
            if (inv == null)
                return;

            inv.ShowInventory(player);
        }
    }
}
