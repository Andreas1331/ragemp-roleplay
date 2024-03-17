using Microsoft.EntityFrameworkCore;
using GTARoleplay.Vehicles.Data;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.Character.Customization;
using GTARoleplay.FactionSystem.Data;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Account.Data;
using GTARoleplay.Character.Data;

namespace GTARoleplay.Database
{
    public abstract class DatabaseBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GTACharacter> Characters { get; set; }
        public DbSet<Staff> Admins { get; set; }
        public DbSet<BanRecord> BanRecords { get; set; }
        public DbSet<GTAVehicle> Vehicles { get; set; }
        public DbSet<GTAVehicleMod> VehicleMods { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CharacterOutfit> Outfits { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<FactionMember> FactionMembers { get; set; }
        public DbSet<FactionRank> FactionRanks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Cocaine>();
            builder.Entity<WaterBottle>();
            base.OnModelCreating(builder);
        }

        public virtual bool IsRelational() { return true; }
    }
}
