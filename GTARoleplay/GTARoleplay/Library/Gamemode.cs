using GTANetworkAPI;
using GTARoleplay.Character.Data;
using GTARoleplay.Database;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Money;
using System.Collections.Generic;
using System.Globalization;
using System.Timers;

namespace GTARoleplay.Library
{
    public class Gamemode 
    {
        public static readonly CultureInfo ServerCulture = new CultureInfo("en-us");

        private Timer hourlyTimer = null;
        private const double hourlyInterval = 3600000; // 1-hour

        private readonly DatabaseBaseContext dbx;

        public Gamemode(DatabaseBaseContext dbx)
        {
            this.dbx = dbx;

            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);

            // Moved to Update event from ServerEvents to sync with hour
            hourlyTimer = new Timer();
            hourlyTimer.Elapsed += new ElapsedEventHandler(OnHourPassed);
            hourlyTimer.Interval = hourlyInterval;
            hourlyTimer.Start();
        }

        public void OnHourPassed(object source, ElapsedEventArgs e)
        {
            // Give a payday to everyone
            var characters = new List<GTACharacter>();
            foreach(var ply in NAPI.Pools.GetAllPlayers())
            {
                var charData = ply.GetUserData()?.ActiveCharacter;
                if(charData != null)
                {
                    characters.Add(charData);
                    NAPI.Task.Run(() =>
                    {
                        MoneyHandler.GivePlayerMoney(charData, 2000);
                        ply.SendChatMessage("Payday: You've been given $2000!");
                    });
                }
            }

            // Go and save their current data
            dbx.UpdateRange(characters);
            dbx.SaveChanges();
        }
    }

    // Used for vehicles, items etc.
    public enum OwnerType { Player, Faction, Vehicle }
}
