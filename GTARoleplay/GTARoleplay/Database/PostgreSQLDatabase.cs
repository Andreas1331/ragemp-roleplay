﻿using Microsoft.EntityFrameworkCore;

namespace GTARoleplay.Database
{
    public class PostgreSQLDatabase : DatabaseBaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                @"host=localhost;database=gta_roleplay;username=postgres;password=root;");
        }
    }
}
