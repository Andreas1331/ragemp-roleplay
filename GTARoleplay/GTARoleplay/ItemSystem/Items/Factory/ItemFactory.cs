using GTARoleplay.Library;
using System.Collections.Generic;

namespace GTARoleplay.ItemSystem.Items.Factory
{
    public class ItemFactory
    {
        private int ownerID;
        private OwnerType ownerType;

        public ItemFactory(int ownerID, OwnerType ownerType)
        {
            this.ownerID = ownerID;
            this.ownerType = ownerType;
        }

        public Item GetItem(string itemName)
        {
            return (itemName.ToLower()) switch
            {
                "cocaine" => new Cocaine(ownerID, ownerType),
                "water bottle" => new WaterBottle(ownerID, ownerType),
                _ => null,
            };
        }

        public List<Item> GetItems(string itemName, int amount)
        {
            List<Item> itms = new List<Item>();
            for (int i = 0; i < amount; i++)
            {
                Item itm = GetItem(itemName);
                if (itm == null)
                    return null;
                itms.Add(itm);
            }
            return itms;
        }
    }
}
