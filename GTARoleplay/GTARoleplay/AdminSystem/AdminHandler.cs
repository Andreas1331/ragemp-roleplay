using GTANetworkAPI;
using GTARoleplay.Account;
using GTARoleplay.Account.Data;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Database;
using GTARoleplay.Library.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTARoleplay.AdminSystem
{
    public class AdminHandler : ScriptExtended
    {
        public static readonly Dictionary<Player, Staff> AllAdmins = new Dictionary<Player, Staff>();
        public static readonly Dictionary<Player, Staff> AllModerators = new Dictionary<Player, Staff>();

        public static readonly List<string> LEVEL_1_COMMANDS = new List<string>();
        public static readonly List<string> LEVEL_2_COMMANDS = new List<string>();

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            // TODO: Check if the player is Rockstar banned, IP banned etc.
            List<BanRecord> banRecords = DatabaseService.GetDatabaseContext()
                .BanRecords
                .Where(
                    x => x.IpAddress.Equals(NAPI.Player.GetPlayerAddress(player)) ||
                    x.SocialClubName.Equals(player.SocialClubName))
                .AsNoTracking().ToList();

            if (banRecords != null)
            {
                if (banRecords.Any(x => x.IsActive))
                {
                    player.KickSilent("~r~You're banned from this server!");
                }
            }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            if (player == null)
                return;

            if (AllAdmins.ContainsKey(player))
                AllAdmins.Remove(player);
            if (AllModerators.ContainsKey(player))
                AllModerators.Remove(player);
        }

        [ServerEvent(Event.ResourceStartEx)]
        public void OnResourceStartEx(string resourceName)
        {
            CommandInfo[] cmds = NAPI.Resource.GetResourceCommandInfos(resourceName);
            foreach (var cmd in cmds.Distinct())
            {
                if (String.IsNullOrEmpty(cmd.Group))
                    continue;

                if (cmd.Group.Equals("Level1"))
                    LEVEL_1_COMMANDS.Add(cmd.Command);
                else if (cmd.Group.Equals("Level2"))
                    LEVEL_2_COMMANDS.Add(cmd.Command);
            }
            LEVEL_2_COMMANDS.AddRange(LEVEL_1_COMMANDS);
        }

        public override void OnUserLoggedIn(Player player, User user)
        {
            // A new user has logged in, if he's an admin add him to the list
            if (user?.StaffData != null)
            {
                if (user.StaffData.Rank.Equals(StaffRank.Moderator))
                    AllModerators.Add(player, user.StaffData);
                else
                    AllAdmins.Add(player, user.StaffData);
            }

            // TODO: Check if the user is banned or not. 
            List<BanRecord> banRecords = DatabaseService.GetDatabaseContext()
                .BanRecords
                .Where(x => x.UserID.Equals(user.UserID))
                .AsNoTracking()
                .ToList();
            if (banRecords.Any(x => x.IsActive))
            {
                player.KickSilent("~r~You're banned from this server!");
            }
        }

        public static void PrintMessageToAdmins(string msg)
        {
            AllAdmins?.ToList().ForEach(kp =>
            {
                if (kp.Key != null && kp.Value != null)
                {
                    kp.Key.SendChatMessage(msg);
                }
            });
        }

        public static void PrintMessageToModerators(string msg)
        {
            AllModerators?.ToList().ForEach(kp =>
            {
                if (kp.Key != null && kp.Value != null)
                {
                    kp.Key.SendChatMessage(msg);
                }
            });
        }
    }
}
