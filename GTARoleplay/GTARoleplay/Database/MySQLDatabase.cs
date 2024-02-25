using Microsoft.EntityFrameworkCore;
using System;

namespace GTARoleplay.Database
{
    public class MySQLDatabase : DatabaseBaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                @"Server=localhost;database=gta_roleplay;user=root;password=root;", new MySqlServerVersion(new Version(8, 0, 36)));
        }
    }
}