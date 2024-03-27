using GTANetworkAPI;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Database;
using GTARoleplay.Library;
using GTARoleplay.Library.Chat;
using GTARoleplay.Library.Extensions;
using System;
using System.Linq;

namespace GTARoleplay.AdminSystem
{
    public class AdminPlayerCommands 
    {
        private readonly DatabaseBaseContext dbx;

        public AdminPlayerCommands(DatabaseBaseContext dbx)
        {
            this.dbx = dbx;

            NAPI.Command.Register<Player, string, int>(
                new RuntimeCommandInfo("adminweapon")
                {
                    Alias = "awep",
                    ClassInstance = this
                }, SpawnAdminVehicle);

            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("goto", "~y~USAGE: ~w~/goto [Target Name / ID]")
                {
                    GreedyArg = true,
                    ClassInstance = this
                }, GotoPlayer);

            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("gethere", "~y~USAGE: ~w~/gethere [Target Name / ID]")
                {
                    GreedyArg = true,
                    ClassInstance = this
                }, GotoPlayer);

            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("slap", "~y~USAGE: ~w~/slap [Target Name / ID]")
                {
                    GreedyArg = true,
                    ClassInstance = this
                }, SlapPlayer);

            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("arevive", "~y~USAGE: ~w~/arevive [Target Name / ID]")
                {
                    GreedyArg = true,
                    ClassInstance = this
                }, RevivePlayer);

            NAPI.Command.Register<Player, string, string>(
                new RuntimeCommandInfo("kick", "~y~USAGE: ~w~/kick [Target Name / ID] [reason]")
                {
                    Alias = "kickplayer",
                    GreedyArg = true,
                    ClassInstance = this
                }, KickPlayer);

            NAPI.Command.Register<Player, string, string>(
                new RuntimeCommandInfo("silentkick", "~y~USAGE: ~w~/silentkick [Target Name / ID] [reason]")
                {
                    Alias = "skick",
                    GreedyArg = true,
                    ClassInstance = this,
                    Group = "Level1"
                }, SilentlyKickPlayer);

            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("ooc", "~y~USAGE: ~w~/ooc [text]")
                {
                    Alias = "o",
                    GreedyArg = true,
                    ClassInstance = this,
                    Group = "Level2"
                }, PrintOOC);

            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("spectate", "~y~USAGE: ~w~/(spec)tate [Target Name / ID]")
                {
                    Alias = "spec",
                    GreedyArg = true,
                    ClassInstance = this,
                    Group = "Level1"
                }, SpectatePlayer);

            NAPI.Command.Register<Player>(
                new RuntimeCommandInfo("unspectate")
                {
                    Alias = "stopspec,unspec",
                    ClassInstance = this,
                }, StopSpectatePlayer);

            NAPI.Command.Register<Player>(
                new RuntimeCommandInfo("admins")
                {
                    Alias = "aonline,adminsonline",
                    ClassInstance = this,
                }, PrintAdminsOnline);

            NAPI.Command.Register<Player, string, string>(
                new RuntimeCommandInfo("ban", "~y~USAGE: ~w~/ban [Target Name / ID] [reason]")
                {
                    Group = "Level2",
                    GreedyArg = true,
                    ClassInstance = this,
                }, BanPlayer);
        }

        public void SpawnAdminVehicle(Player player, string weaponName, int ammo = 100)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Developer))
            {
                var weaponHash = NAPI.Util.WeaponNameToModel(weaponName);
                if (weaponHash > 0)
                {
                    NAPI.Player.GivePlayerWeapon(player, weaponHash, ammo);
                    player.SendChatMessage($"You gave yourself {weaponHash} with {ammo} ammo.");
                }
            }
        }

        public void GotoPlayer(Player player, string target)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    // TODO: If the dimension is bigger than 0, load the property
                    player.Position = targetPly.Position.Around(1);
                    player.Dimension = targetPly.Dimension;
                    player.SendChatMessage($"You've teleported to {targetPly.Name}.");
                }
                else
                    player.SendChatMessage("~r~ERROR: ~w~Player not found.");
            }
        }

        public void GetPlayerHere(Player player, string target)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    targetPly.Position = player.Position.Around(1);
                    targetPly.Dimension = player.Dimension;
                    targetPly.SendChatMessage("You have been teleported by an admin.");
                    player.SendChatMessage($"You have teleported {targetPly.Name} to your location.");
                }
                else
                    player.SendChatMessage("~r~ERROR: ~w~Player not found.");
            }
        }

        public void SlapPlayer(Player player, string target)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    // Throw the player x meters up in the air
                    targetPly.Position = targetPly.Position + new Vector3(0, 0, 2);
                    targetPly.SendChatMessage("You were slapped by an admin!");
                    player.SendChatMessage($"You slapped {targetPly.Name}!");
                }
                else
                    player.SendChatMessage("~r~ERROR: ~w~Player not found.");
            }
        }

        public void RevivePlayer(Player player, string target)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level2))
            {
                var targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    if (targetPly.Dead)
                    {
                        NAPI.Player.SpawnPlayer(targetPly, targetPly.Position);
                        player.SendAdminCommandMessage($"You've respawned {targetPly.Name}.");
                        targetPly.SendChatMessage("~y~You were revived by an admin.");
                    }
                }
            }
        }

        public void KickPlayer(Player player, string target, string reason)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                var targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    var adm = player.GetUserData()?.StaffData;
                    if (adm == null)
                        return;

                    string kickMessage = $"~r~AdmCmd: {targetPly.Name} was kicked by {adm.StaffName} Reason: ({reason})";
                    targetPly.Kick(kickMessage);
                    NAPI.Chat.SendChatMessageToAll(kickMessage);
                }
                else
                    player.SendChatMessage("~r~ERROR: ~w~Player not found.");
            }
        }

        public void SilentlyKickPlayer(Player player, string target, string reason)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    var adm = player.GetUserData()?.StaffData;
                    if (adm == null)
                        return;

                    var kickMessage = $"~r~AdmCmd: {targetPly.Name} was kicked by {adm.StaffName} Reason: ({reason})";
                    targetPly.Kick(kickMessage);
                    // TODO: print to all admins or staff
                    //CMDUsedToAllAdmins("~y~AdmCmd: ~w~" + player.name.Replace("_", " ") + " ((" + player.handle + ")) silent kicked " + playerToKick.name.Replace("_", " ") + " ((" + playerToKick.handle + ")) Reason: " + reason);
                }
                else
                    player.SendChatMessage("~~r~ERROR: ~w~Player not found.");
            }
        }

        public void PrintOOC(Player player, string text)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level2))
            {
                Staff adm = player.GetUserData()?.StaffData;
                if (adm == null)
                    return;

                string message = $"{ChatColors.OOC_COLOR}OOC: {adm.StaffName}: {text}";
                NAPI.Chat.SendChatMessageToAll(message);
            }
        }

        public void SpectatePlayer(Player player, string target)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    if (targetPly == player)
                    {
                        player.SendChatMessage("~r~ERROR: ~w~You're not able to spectate yourself.");
                        return;
                    }

                    player.TriggerEvent("StartSpectatingTarget::Client", targetPly);
                    NAPI.Entity.SetEntityTransparency(player, 0);
                    player.SetData("OldSpectatePosition", player.Position);

                    player.SendAdminCommandMessage($"You're now spectating {targetPly.Name} ID {PlayerHandler.GetIDFromPlayer(targetPly)}!");
                    player.SendChatMessage("Use /unspectate to stop spectating this player.");
                }
            }
        }

        public void StopSpectatePlayer(Player player)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                player.TriggerEvent("StopSpectatingTarget::Client");
                NAPI.Entity.SetEntityTransparency(player, 255);
                // Set the players position back to the place before the spectate began
                if (player.HasData("OldSpectatePosition"))
                {
                    player.Position = player.GetData<Vector3>("OldSpectatePosition");
                    player.ResetData("OldSpectatePosition");
                }

                player.SendAdminCommandMessage("You're nolonger spectating anyone.");
            }
        }

        public void PrintAdminsOnline(Player player)
        {
            player.SendChatMessage("~y~======== Administrators available ========");
            AdminHandler.AllAdmins.ToList().ForEach(kp =>
            {
                if (kp.Key != null && kp.Value != null)
                {
                    player.SendChatMessage($"~y~{kp.Value.Rank}: ~w~{kp.Value.StaffName} ((ID:{PlayerHandler.GetIDFromPlayer(kp.Key)}))");
                }
            });
            player.SendChatMessage("~y~=====================================");
        }

        public void BanPlayer(Player player, string target, string reason)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level2))
            {
                var targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    if (targetPly == player)
                    {
                        player.SendChatMessage("~r~ERROR: ~w~You're not able to ban yourself.");
                        return;
                    }

                    var adm = player.GetUserData()?.StaffData;
                    var targetUser = targetPly.GetUserData();
                    if (targetUser == null)
                        return;

                    var ipAddress = NAPI.Player.GetPlayerAddress(targetPly);
                    var record = new BanRecord(targetUser.UserID, adm.StaffName, reason, ipAddress, targetPly.SocialClubName, DateTime.Now);
                    dbx.BanRecords.Add(record);
                    dbx.SaveChanges();
                    string message = $"~r~AdmCmd: {targetPly.Name} (ID: {PlayerHandler.GetIDFromPlayer(targetPly)}) was banned by {player.Name}. Reason: {reason}";
                    targetPly.Kick(message);
                    NAPI.Chat.SendChatMessageToAll(message);
                }
            }
        }
    }
}
