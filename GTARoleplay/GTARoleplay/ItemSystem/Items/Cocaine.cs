using GTANetworkAPI;
using GTARoleplay.InventorySystem;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTARoleplay.ItemSystem.Items
{
    public class Cocaine : Item, ICompatible
    {
        [Column("strength")]
        public float Strength { get; set; } = 100;

        public override string Name => "Cocaine";
        public override float Weight => 1;
        public override bool IsStackable => true;
        public override ItemFlags ItemFlag => (ItemFlags.IsOneTimeUsage | ItemFlags.IsUsable);

        public Cocaine(int ownerID, OwnerType ownerType) : base(ownerID, ownerType) { }

        public override void Use(Player player)
        {
            if (player == null)
                return;

            if (ItemFlag.HasFlag(ItemFlags.IsUsable))
            {
                player.SendChatMessage("You've used cocaine!");

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

        public bool IsCompatible(Item itm)
        {
            if(itm is Cocaine itmCocaine)
            {
                return (Strength.Equals(itmCocaine.Strength));
            }
            return false;
        }
    }
}
