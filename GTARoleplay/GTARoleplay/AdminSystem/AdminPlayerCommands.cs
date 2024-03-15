using GTANetworkAPI;
using GTARoleplay.Account;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Library;
using GTARoleplay.Library.Chat;
using GTARoleplay.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.AdminSystem
{
    public class AdminPlayerCommands : ScriptExtended
    {
        public Dictionary<int, string> helpmes = new Dictionary<int, string>();

        [Command("adminweapon", Alias = "awep")]
        public void SpawnAdminVehicle(Player player, string weaponName, int ammo = 100)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Developer))
            {
                WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(weaponName);
                if (weaponHash > 0)
                {
                    NAPI.Player.GivePlayerWeapon(player, weaponHash, ammo);
                    player.SendChatMessage($"You gave yourself {weaponHash} with {ammo} ammo.");
                }
            }
        }

        [Command("goto", "~y~USAGE: ~w~/goto [Target Name / ID]", GreedyArg = true)]
        public void GotoPlayer(Player player, string target)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
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

        [Command("gethere", "~y~USAGE: ~w~/gethere [Target Name / ID]", GreedyArg = true)]
        public void GetPlayerHere(Player player, string target)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
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

        [Command("slap", "~y~USAGE: ~w~/slap [Target Name / ID]", GreedyArg = true)]
        public void SlapPlayer(Player player, string target)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
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

        [Command("arevive", "~y~USAGE: ~w~/arevive [Target Name / ID]", GreedyArg = true)]
        public void RevivePlayer(Player player, string target)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level2))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
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

        [Command("kick", "~y~USAGE: ~w~/kick [Target Name / ID] [reason]", Alias = "kickplayer", GreedyArg = true)]
        public void KickPlayer(Player player, string target, string reason)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    Staff adm = player.GetUserData()?.StaffData;
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

        [Command("silentkick", "~y~USAGE: ~w~/silentkick [Target Name / ID] [reason]", Alias = "skick", Group = "Level1", GreedyArg = true)]
        public void SilentlyKickPlayer(Player player, string target, string reason)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    Staff adm = player.GetUserData()?.StaffData;
                    if (adm == null)
                        return;

                    string kickMessage = $"~r~AdmCmd: {targetPly.Name} was kicked by {adm.StaffName} Reason: ({reason})";
                    targetPly.Kick(kickMessage);
                    // TODO: print to all admins or staff
                    //CMDUsedToAllAdmins("~y~AdmCmd: ~w~" + player.name.Replace("_", " ") + " ((" + player.handle + ")) silent kicked " + playerToKick.name.Replace("_", " ") + " ((" + playerToKick.handle + ")) Reason: " + reason);
                }
                else
                    player.SendChatMessage("~~r~ERROR: ~w~Player not found.");
            }
        }

        [Command("ooc", "~y~USAGE: ~w~/ooc [text]", Group = "Level2", Alias = "o", GreedyArg = true)]
        public void PrintOOC(Player player, string text)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level2))
            {
                Staff adm = player.GetUserData()?.StaffData;
                if (adm == null)
                    return;

                string message = $"{ChatColors.OOC_COLOR}OOC: {adm.StaffName}: {text}";
                NAPI.Chat.SendChatMessageToAll(message);
            }
        }

        [Command("spectate", "~y~USAGE: ~w~/(spec)tate [Target Name / ID]", Group = "Level1", Alias = "spec", GreedyArg = true)]
        public void SpectatePlayer(Player player, string target)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
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
                    player.SetData<Vector3>("OldSpectatePosition", player.Position);

                    player.SendAdminCommandMessage($"You're now spectating {targetPly.Name} ID {PlayerHandler.GetIDFromPlayer(targetPly)}!");
                    player.SendChatMessage("Use /unspectate to stop spectating this player.");
                }
            }
        }

        [Command("unspectate", Group = "Level1", Alias = "stopspec")]
        public void StopSpectatePlayer(Player player)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
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

        [Command("admins", Alias = "aonline,adminsonline")]
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

        [Command("ban", "~y~USAGE: ~w~/ban [Target Name / ID] [reason]", Group = "Level2", GreedyArg = true)]
        public void BanPlayer(Player player, string target, string reason)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level2))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    if (targetPly == player)
                    {
                        player.SendChatMessage("~r~ERROR: ~w~You're not able to ban yourself.");
                        return;
                    }

                    Staff adm = player.GetUserData()?.StaffData;
                    User targetUser = targetPly.GetUserData();
                    if (adm == null || targetUser == null)
                        return;

                    string ipAddress = NAPI.Player.GetPlayerAddress(targetPly);
                    BanRecord record = new BanRecord(targetUser.UserID, adm.StaffName, reason, ipAddress, targetPly.SocialClubName, DateTime.Now);
                    var db = DatabaseService.GetDatabaseContext();
                    db.BanRecords.Add(record);
                    db.SaveChanges();
                    string message = $"~r~AdmCmd: {targetPly.Name} (ID: {PlayerHandler.GetIDFromPlayer(targetPly)}) was banned by {player.Name}. Reason: {reason}";
                    targetPly.Kick(message);
                    NAPI.Chat.SendChatMessageToAll(message);
                }
            }
        }

        [Command("helpme", "~y~USAGE: ~w~/helpme [question] to submit a ticket to our staff team.", GreedyArg = true)]
        public void SubmitPlayerHelpme(Player player, string question)
        {
            if (player == null)
                return;

            int playerID = PlayerHandler.GetIDFromPlayer(player);
            if (!helpmes.ContainsKey(playerID))
            {
                if (String.IsNullOrEmpty(question))
                {
                    player.SendErrorMessage("Please submit a valid question.");
                    return;
                }

                string formattedHelpme = $"~g~HELPME: ~s~From ((ID:{playerID})) {player.Name}: {question}";
                // If there's no moderators online, show the helpme to the admin team
                if (AdminHandler.AllModerators.Count <= 0)
                    AdminHandler.PrintMessageToAdmins(formattedHelpme);
                else
                    AdminHandler.PrintMessageToModerators(formattedHelpme);
                helpmes.Add(playerID, formattedHelpme);
                player.SendChatMessage("~y~Your helpme has now been submitted, standby as a staff member responds.");
            } else
                player.SendErrorMessage("You already have a pending helpme.");
        }

        [Command("viewhelpmes", Alias = "vhm", Group = "Moderator")]
        public void PrintHelpmes(Player player)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Moderator))
            {
                player.SendChatMessage("~y~Displaying avaliable helpme's. Use /accepthelpme(ahm) [helpmeID] to accept, or /trashhelpme [helpmeID] to trash.");
                helpmes?.Values.ToList().ForEach(helpme =>
                {
                    player.SendChatMessage(helpme);
                });
                if (helpmes?.Count <= 0)
                {
                    player.SendErrorMessage("There's no pending helpme's!");
                }
            }
        }

        [Command("trashhelpme", "~y~USAGE: ~w~/trashhelpme [HelpmeID]", Group = "Moderator")]
        public void TrashHelpme(Player player, int helpmeid)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Moderator))
            {
                if (helpmes.ContainsKey(helpmeid))
                {
                    helpmes.Remove(helpmeid);
                    Player submittedBy = PlayerHandler.GetPlayer(helpmeid);

                    if (submittedBy != null)
                    {
                        Staff staff = player.GetUserData()?.StaffData;
                        if (staff != null)
                        {
                            submittedBy.SendErrorMessage($"Your helpme has been trashed by {staff.StaffName}", "INFO");
                        }
                    }

                    player.SendModeratorCommandMessage($"You successfully trashed the helpme with ID: {helpmeid}");
                }
                else
                    player.SendErrorMessage($"There's no helpme with the ID: {helpmeid}");
            }
        }

        [Command("accepthelpme", "~y~USAGE: ~w~/accepthelpme(/ahm) [HelpmeID] to accept a helpme.", Alias = "ahm", Group = "Moderator")]
        public void AcceptHelpme(Player player, int helpmeID)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Moderator))
            {
                if (helpmes.ContainsKey(helpmeID))
                {
                    Player submittedBy = PlayerHandler.GetPlayer(helpmeID);
                    if(submittedBy != null)
                    {
                        Staff staffData = player.GetUserData()?.StaffData;
                        if (staffData == null)
                            return;

                        submittedBy.SendChatMessage($"~g~INFO: ~w~Your helpme has been accepted by: {staffData.StaffName} ((ID:{PlayerHandler.GetIDFromPlayer(player)}))");
                        player.SendModeratorCommandMessage($"Helpme accepted! submitted by: {submittedBy.Name} ((ID:{helpmeID}))");
                        helpmes.Remove(helpmeID);
                    }
                }
            }
        }
    }
}
