using GTANetworkAPI;
using GTARoleplay.InventorySystem;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.ItemSystem.Items
{
    public class WaterBottle : Item, ICompatible
    {
        public WaterBottle(int ownerID, OwnerType ownerType) : base(ownerID, ownerType) { }

        public override string Name => "Water bottle";

        public override float Weight => 500;

        public override bool IsStackable => true;

        public override ItemFlags ItemFlag => ItemFlags.IsOneTimeUsage | ItemFlags.IsUsable;

        public bool IsCompatible(Item itm)
        {
            return itm.GetType().Equals(this.GetType());
        }

        public override void Use(Player player)
        {
            if (player == null)
                return;

            if (ItemFlag.HasFlag(ItemFlags.IsUsable))
            {
                player.SendChatMessage("You drank a water bottle!");

                // If the item is a one-time usage, then get the players inventory and remove the item
                if (ItemFlag.HasFlag(ItemFlags.IsOneTimeUsage))
                {
                    Inventory inv = player.GetUserData()?.ActiveCharacter?.Inventory;
                    if (inv == null)
                        return;
                    inv.RemoveItem(this);
                }
            }
        }
    }
}
