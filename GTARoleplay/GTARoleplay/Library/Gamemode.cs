using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Provider;
using System.Collections.Generic;
using System.Globalization;
using System.Timers;

namespace GTARoleplay.Library
{
    public class Gamemode : Script
    {
        public static readonly string VERSION = "0.0.2-alpha";
        private const string WELCOME_MESSAGE = "Welcome to GTA Roleplay!";
        public static readonly CultureInfo ServerCulture = new CultureInfo("en-us");

        private Timer hourlyTimer = null;
        private const double hourlyInterval = 3600000; // 1-hour

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            ServicesContainer.Init();
            DatabaseService.EnsureDatabaseIsCreated();

            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);

            // Moved to Update event from ServerEvents to sync with hour
            hourlyTimer = new Timer();
            hourlyTimer.Elapsed += new ElapsedEventHandler(OnHourPassed);
            hourlyTimer.Interval = hourlyInterval;
            hourlyTimer.Start();
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            PlayerHandler.RemovePlayerFromPlayerList(player);
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            player.SendChatMessage(WELCOME_MESSAGE);
            player.TriggerEvent("ShowLogin::Client");
            player.TriggerEvent("EnableHUD::Client", false);
            player.Freeze(true);
            player.Transparency = 0;
        }

        public static void OnHourPassed(object source, ElapsedEventArgs e)
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
                        charData.GivePlayerMoney(2000);
                        ply.SendChatMessage("Payday: You've been given $2000!");
                    });
                }
            }

            // Go and save their current data
            var db = DatabaseService.GetDatabaseContext();
            db.UpdateRange(characters);
            db.SaveChanges();
        }
    }

    // Used for vehicles, items etc.
    public enum OwnerType { Player, Faction, Vehicle }
}
