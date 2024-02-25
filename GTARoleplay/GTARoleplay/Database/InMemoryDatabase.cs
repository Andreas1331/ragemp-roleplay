using Microsoft.EntityFrameworkCore;

namespace GTARoleplay.Database
{
    public class InMemoryDatabase : DatabaseBaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("gta_roleplay");
        }
    }
}
