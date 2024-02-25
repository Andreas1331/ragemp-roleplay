using GTANetworkAPI;
using GTARoleplay.Database;
using GTARoleplay.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTARoleplay.ItemSystem.Items
{
    [Table("items")]

    public abstract class Item
    {
        [Key]
        [Column("id")]
        public int ItemID { get; set; }

        [Column("owner_id")]
        public int OwnerID { get; set; }

        [Column("owner_type")]
        public OwnerType OwnerType { get; set; }

        public abstract string Name { get; }
        public abstract float Weight { get; }
        public abstract bool IsStackable { get; }
        public abstract ItemFlags ItemFlag { get; }

        public abstract void Use(Player player);

        public Item(int ownerID, OwnerType ownerType)
        {
            OwnerID = ownerID;
            OwnerType = ownerType;
        }

        public void DeleteFromDatabase()
        {
            using (var db = DatabaseService.GetDatabaseContext())
            {
                db.Items.Remove(this);
                db.SaveChanges();
            }
        }

        public static void AddItemToDatabase(Item itm)
        {
            using (var db = DatabaseService.GetDatabaseContext())
            {
                db.Items.Add(itm);
                db.SaveChanges();
            }
        }

        public static void AddItemsToDatabase(List<Item> itms)
        {
            using (var db = DatabaseService.GetDatabaseContext())
            {
                db.Items.AddRange(itms);
                db.SaveChanges();
            }
        }
    }

    [Flags]
    public enum ItemFlags
    {
        IsOneTimeUsage = 0,
        IsUsable = 1 << 3,
    }
}
