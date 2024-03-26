using GTARoleplay.Database;
using GTARoleplay.ItemSystem.Items;
using System.Collections.Generic;

namespace GTARoleplay.InventorySystem
{
    public class ItemHandler
    {
        private readonly DatabaseBaseContext _dbx;

        public ItemHandler(DatabaseBaseContext dbx)
        {
            _dbx = dbx;
        }

        public void DeleteFromDatabase(Item itm)
        {
            _dbx.Items.Remove(itm);
            _dbx.SaveChanges();
        }

        public void AddItemToDatabase(Item itm)
        {
            _dbx.Items.Add(itm);
            _dbx.SaveChanges();
        }

        public void AddItemsToDatabase(List<Item> itms)
        {
            _dbx.Items.AddRange(itms);
            _dbx.SaveChanges();
        }
    }
}
