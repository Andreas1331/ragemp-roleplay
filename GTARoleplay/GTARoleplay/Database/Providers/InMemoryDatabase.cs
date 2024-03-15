using GTARoleplay.Account;
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

            var user = new User();
            user.UserID = 1;
            user.Username = "andreas";
            user.Email = "myemail@google.com";
            user.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var character = new GTACharacter() {
                CharacterID = 1,
                Firstname = "Jack",
                Lastname = "McClane",
                UserID = user.UserID,
            };
            character.GivePlayerMoney(999999);

            builder.Entity<User>().HasData(user);
            builder.Entity<GTACharacter>().HasData(character);
        }
    }
}
