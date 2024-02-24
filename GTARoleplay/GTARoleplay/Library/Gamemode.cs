using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Library.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Timers;

namespace GTARoleplay.Library
{
    public class Gamemode : Script
    {
        public static readonly string VERSION = "0.0.2-alpha";
        public static readonly CultureInfo ServerCulture = new CultureInfo("en-us");

        private Timer hourlyTimer = null;
        private const double hourlyInterval = 3600000; // 1-hour

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);

            // Moved to Update event from ServerEvents to sync with hour
            hourlyTimer = new Timer();
            hourlyTimer.Elapsed += new ElapsedEventHandler(OnHourPassed);
            hourlyTimer.Interval = hourlyInterval;
            hourlyTimer.Start();
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnect(Player player, DisconnectionType type, string reason)
        {
            PlayerHandler.RemovePlayerFromPlayerList(player);
        }

        public static void OnHourPassed(object source, ElapsedEventArgs e)
        {
            // Give a payday to everyone
            List<GTACharacter> characters = new List<GTACharacter>();
            foreach(Player ply in NAPI.Pools.GetAllPlayers())
            {
                GTACharacter charData = ply.GetUserData()?.ActiveCharacter;
                if(charData != null)
                {
                    charData.Money += 2000;
                    characters.Add(charData);
                    NAPI.Task.Run(() =>
                    {
                        ply.SendChatMessage("Payday: You've been given $2000!");
                    });
                }
            }

            // Go and save their current data
            using (var db = new DbConn())
            {
                db.UpdateRange(characters);
                db.SaveChanges();
            }

        }
    }

    // Used for vehicles, items etc.
    public enum OwnerType { Player, Faction, Vehicle }
}
