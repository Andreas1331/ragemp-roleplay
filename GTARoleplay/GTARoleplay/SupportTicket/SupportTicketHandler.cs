using GTANetworkAPI;
using GTARoleplay.AdminSystem;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.SupportTicket
{
    public class SupportTicketHandler : Script
    {
        public Dictionary<int, string> helpmes = new Dictionary<int, string>();

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
            }
            else
                player.SendErrorMessage("You already have a pending helpme.");
        }

        [Command("viewhelpmes", Alias = "vhm", Group = "Moderator")]
        public void PrintHelpmes(Player player)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Moderator))
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
            if (AdminAuthorization.HasPermission(player, StaffRank.Moderator))
            {
                if (helpmes.ContainsKey(helpmeid))
                {
                    helpmes.Remove(helpmeid);
                    var submittedBy = PlayerHandler.GetPlayer(helpmeid);

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
            if (AdminAuthorization.HasPermission(player, StaffRank.Moderator))
            {
                if (helpmes.ContainsKey(helpmeID))
                {
                    Player submittedBy = PlayerHandler.GetPlayer(helpmeID);
                    if (submittedBy != null)
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
