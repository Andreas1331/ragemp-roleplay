using GTANetworkAPI;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.Library.Extensions;
using System.Linq;

namespace GTARoleplay.InventorySystem
{
    public class InventoryEvents : Script
    {
        [RemoteEvent("UseItem::Server")]
        public void UseItemFromInventory(Player player, int itemIdentifier)
        {
            if (player == null)
                return;

            Inventory inv = player.GetUserData()?.ActiveCharacter?.Inventory;
            if (inv != null)
            {
                // Find the item
                Item itm = inv.Items.FirstOrDefault(x => x.ItemID.Equals(itemIdentifier));
                itm?.Use(player);
                inv.RefreshInventory(player);
            }
        }
    }
}
