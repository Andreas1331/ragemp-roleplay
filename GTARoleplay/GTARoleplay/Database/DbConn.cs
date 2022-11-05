using Microsoft.EntityFrameworkCore;
using GTARoleplay.Account;
using GTARoleplay.Character;
using GTARoleplay.Vehicles.Data;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.Character.Customization;
using GTARoleplay.FactionSystem;
using GTARoleplay.AdminSystem.Data;

namespace GTARoleplay.Database
{
    public class DbConn : DbContext
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=3306;database=gta_roleplay;user=root;password=1234;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Cocaine>();
            builder.Entity<WaterBottle>();
            base.OnModelCreating(builder);
        }
    }
}
