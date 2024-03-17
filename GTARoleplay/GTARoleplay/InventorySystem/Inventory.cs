using GTANetworkAPI;
using GTARoleplay.InventorySystem.Data;
using GTARoleplay.ItemSystem.Items;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.InventorySystem
{
    public class Inventory
    {
        public List<Item> Items = new List<Item>();
        public float CurrentWeight = 0;

        protected virtual float MaxWeight { get; } = 10000;

        public Inventory(List<Item> itms)
        {
            Items = itms;
            CurrentWeight = Items.Sum(x => x.Weight);
        }

        private List<InventoryItem> GetInventoryItemsFormatted()
        {
            // Prepare the items for viewing
            List<InventoryItem> itmsToView = new List<InventoryItem>();
            List<Item> itemsAlreadyAdded = new List<Item>();
            foreach (Item itm in Items)
            {
                // If we already checked this item, just skip it
                if (itemsAlreadyAdded.Contains(itm))
                    continue;

                // If the item can stack, find the other items to stack with
                if (itm.IsStackable && itm is ICompatible itmCompatible)
                {
                    InventoryItem invItem = new InventoryItem();
                    invItem.Amount = 1;
                    Items.Where(x => x != itm).ToList().ForEach(x =>
                    {
                        if (x.IsStackable)
                        {
                            if (itmCompatible.IsCompatible(x))
                            {
                                invItem.Amount += 1;
                                itemsAlreadyAdded.Add(x);
                            }
                        }
                    });
                    invItem.Identifier = itm.ItemID;
                    invItem.WeightPerItem = itm.Weight;
                    invItem.Name = itm.Name;
                    itemsAlreadyAdded.Add(itm);
                    itmsToView.Add(invItem);
                }
                else
                {
                    // The item is not stackable, so it's safe to assume we only have one
                    itmsToView.Add(new InventoryItem(itm.Name, itm.ItemID, 1, itm.Weight));
                }
            }
            return itmsToView;
        }

        public void PrintInventory(Player player)
        {
            player.SendChatMessage($"Showing the inventory of {player.Name}");
            player.SendChatMessage("___________________________________________");
            var itms = GetInventoryItemsFormatted();
            itms?.ForEach(itm =>
                {
                    player.SendChatMessage($"({itms.IndexOf(itm) + 1}) {itm.Name} - Amount: {itm.Amount}");
                });
            player.SendChatMessage("___________________________________________");
        }

        public void ShowInventory(Player player)
        {
            if (player == null)
                return;

            var itmsToView = GetInventoryItemsFormatted();
            player.TriggerEvent("ShowInventory::Client", MaxWeight, NAPI.Util.ToJson(itmsToView));
        }

        public void RefreshInventory(Player player)
        {
            if (player == null)
                return;

            List<InventoryItem> itmsToView = GetInventoryItemsFormatted();
            player.TriggerEvent("RefreshInventory::Client", MaxWeight, NAPI.Util.ToJson(itmsToView));
        }

        public void AddItem(Item itm)
        {
            if(CurrentWeight + itm.Weight <= MaxWeight)
                Items.Add(itm);
        }

        public void AddItems(List<Item> itms)
        {
            if (CurrentWeight + itms.Sum(x => x.Weight) <= MaxWeight)
                Items.AddRange(itms);
        }

        public void RemoveItem(Item itm, bool deleteFromDB = true)
        {
            if(Items.Contains(itm))
                Items.Remove(itm);
            if(deleteFromDB)
                itm.DeleteFromDatabase();
        }
    }
}
