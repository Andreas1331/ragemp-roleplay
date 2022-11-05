using GTANetworkAPI;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.InventorySystem
{
    public class InventoryCommands : Script
    {
        [Command("myitems", Alias = "myinventory,inventory,inv")]
        public void CheckPlayerItems(Player player)
        {
            player.SendChatMessage($"Showing the inventory of {player.Name}");
            player.SendChatMessage("___________________________________________");
            Inventory inv = player.GetUserData()?.ActiveCharacter?.Inventory;
            if (inv != null)
                inv.PrintInventory(player);
            player.SendChatMessage("___________________________________________");
        }
    }
}
