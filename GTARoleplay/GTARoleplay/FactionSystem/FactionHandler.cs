using GTANetworkAPI;
using GTARoleplay.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.FactionSystem
{
    public class FactionHandler : Script
    {
        // Permanent, AKA reserved faction names
        public static readonly string LSPD_FACTION_NAME = "Los Santos Police Department";
        public static readonly List<string> ALL_PERMANENT_FACTION_NAMES = new List<string>()
        {
            LSPD_FACTION_NAME
        };

        public static List<Faction> AllFactions = new List<Faction>();

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            using (var db = new DbConn())
            {
                AllFactions = db.Factions.Include(x => x.Ranks).ToList();
                Console.WriteLine($"FactionHandler: all factions have been loaded ({AllFactions.Count})");
            }
        }

        public static Faction GetFactionByID(int id)
        {
            return AllFactions?.FirstOrDefault(x => x.FactionID == id);
        }
    }
}
