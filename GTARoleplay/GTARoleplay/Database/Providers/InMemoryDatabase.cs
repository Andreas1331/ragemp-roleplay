using GTANetworkAPI;
using GTARoleplay.Account.Data;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Authentication;
using GTARoleplay.Character.Customization;
using GTARoleplay.Character.Data;
using GTARoleplay.Library;
using GTARoleplay.Money;
using GTARoleplay.Vehicles.Data;
using Microsoft.EntityFrameworkCore;

namespace GTARoleplay.Database.Providers
{
    /// <summary>
    /// If used, beaware that no data persists between each run.
    /// </summary>
    /// <remarks>
    /// An in-memory provider for EF Core with seeded data for testing.
    /// </remarks>
    public class InMemoryDatabase : DatabaseBaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("gta_roleplay");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var user = new User()
            {
                UserID = 1,
                Username = "andreas",
                Email = "myemail@google.com",
                Password = Authenticator.GetHashedPassword("123")
            };

            var character = new GTACharacter()
            {
                CharacterID = 1,
                Firstname = "Jack",
                Lastname = "McClane",
                LastX = 166.31f,
                LastY = -1274.69f,
                LastZ = 28.25f,
                UserID = user.UserID,
            };
            MoneyHandler.GivePlayerMoney(character, 999999);

            var outfit = new CharacterOutfit()
            {
                OutfitID = 1,
            };

            var staff = new Staff()
            {
                StaffID = 1,
                StaffName = "-Andreas",
                Rank = StaffRank.Developer,
            };

            var gtaVehicle = new GTAVehicle()
            {
                VehicleID = 1,
                Owner = 1,
                OwnerType = OwnerType.Player,
                Numberplate = "Awesome",
                VehicleModel = "sultan",
                LastX = 166.31f,
                LastY = -1274.69f,
                LastZ = 28.25f,
                Fuel = 100
            };

            builder.Entity<User>().HasData(user);
            builder.Entity<GTACharacter>().HasData(character);
            builder.Entity<CharacterOutfit>().HasData(outfit);
            builder.Entity<Staff>().HasData(staff);
            builder.Entity<GTAVehicle>().HasData(gtaVehicle);
        }
    }
}
