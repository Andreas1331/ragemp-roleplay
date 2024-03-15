using GTARoleplay.Account;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Character;
using Microsoft.EntityFrameworkCore;

namespace GTARoleplay.Database.Providers
{
    /// <summary>
    /// If used, beaware that no data persists between each run.
    /// </summary>
    /// <remarks>
    /// An in-memory provider for EF Core.
    /// </remarks>
    public class InMemoryDatabase : DatabaseBaseContext
    {
        public override bool IsRelational() { return false; }

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
                Password = BCrypt.Net.BCrypt.HashPassword("test")
            };

            var character = new GTACharacter() {
                CharacterID = 1,
                Firstname = "Jack",
                Lastname = "McClane",
                LastX = 166.31f,
                LastY = -1274.69f,
                LastZ = 28.25f,
                UserID = user.UserID,
            };
            character.GivePlayerMoney(999999);

            var staff = new Staff()
            {
                StaffID = 1,
                StaffName = "-Andreas",
                Rank = StaffRank.Developer,
            };

            builder.Entity<User>().HasData(user);
            builder.Entity<GTACharacter>().HasData(character);
            builder.Entity<Staff>().HasData(staff);
        }
    }
}
