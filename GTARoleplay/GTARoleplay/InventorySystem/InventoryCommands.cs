using GTANetworkAPI;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.InventorySystem
{
    public class InventoryCommands : Script
    {
        [Command("myitems", Alias = "myinventory,inventory,inv")]
        public void ShowPlayerInventory(Player player)
        {
            Inventory inv = player.GetUserData()?.ActiveCharacter?.Inventory;
            inv?.ShowInventory(player);
        }

        [Command("printitems")]
        public void PrintPlayerItems(Player player)
        {
            Inventory inv = player.GetUserData()?.ActiveCharacter?.Inventory;
            inv?.PrintInventory(player);
        }
    }
}
