using GTARoleplay.Account;
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

            builder.Entity<User>().HasData(user);
        }
    }
}
