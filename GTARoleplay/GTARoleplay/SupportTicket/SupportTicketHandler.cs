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
    public class SupportTicketHandler 
    {
        public Dictionary<int, string> tickets = new Dictionary<int, string>();

        public SupportTicketHandler()
        {
            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("ticket", "~y~USAGE: ~w~/ticket [question] to submit a ticket to our staff team.")
                {
                    GreedyArg = true,
                    ClassInstance = this
                }, SubmitTicket);

            NAPI.Command.Register<Player>(
                new RuntimeCommandInfo("viewtickets")
                {
                    Alias = "vt,tickets",
                    Group = "Moderator",
                    ClassInstance = this
                }, ViewTickets);

            NAPI.Command.Register<Player, int>(
                new RuntimeCommandInfo("trashticket", "~y~USAGE: ~w~/trashtticket [ticketID]")
                {
                    Alias = "tt",
                    Group = "Moderator",
                    ClassInstance = this
                }, TrashTicket);

            NAPI.Command.Register<Player, int>(
                new RuntimeCommandInfo("acceptticket", "~y~USAGE: ~w~/acceptticket(/tt) [ticketID] to accept a ticket.")
                {
                    Alias = "at",
                    Group = "Moderator",
                    ClassInstance = this
                }, AcceptTicket);
        }

        public void SubmitTicket(Player player, string question)
        {
            if (player == null)
                return;

            int playerID = PlayerHandler.GetIDFromPlayer(player);
            if (!tickets.ContainsKey(playerID))
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
                tickets.Add(playerID, formattedHelpme);
                player.SendChatMessage("~y~Your helpme has now been submitted, standby as a staff member responds.");
            }
            else
                player.SendErrorMessage("You already have a pending helpme.");
        }

        public void ViewTickets(Player player)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Moderator))
            {
                player.SendChatMessage("~y~Displaying avaliable tickets. Use /acceptticket(at) [ticketID] to accept, or /trashticket(tt) [ticketID] to trash.");
                tickets?.Values.ToList().ForEach(helpme =>
                {
                    player.SendChatMessage(helpme);
                });
                if (tickets?.Count <= 0)
                {
                    player.SendErrorMessage("There's no pending tickets");
                }
            }
        }

        public void TrashTicket(Player player, int ticketID)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Moderator))
            {
                if (tickets.ContainsKey(ticketID))
                {
                    tickets.Remove(ticketID);
                    var submittedBy = PlayerHandler.GetPlayer(ticketID);

                    if (submittedBy != null)
                    {
                        Staff staff = player.GetUserData()?.StaffData;
                        if (staff != null)
                        {
                            submittedBy.SendErrorMessage($"Your ticket has been trashed by {staff.StaffName}", "INFO");
                        }
                    }

                    player.SendModeratorCommandMessage($"You successfully trashed the ticket with ID: {ticketID}");
                }
                else
                    player.SendErrorMessage($"There's no ticket with the ID: {ticketID}");
            }
        }

        public void AcceptTicket(Player player, int ticketID)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Moderator))
            {
                if (tickets.ContainsKey(ticketID))
                {
                    Player submittedBy = PlayerHandler.GetPlayer(ticketID);
                    if (submittedBy != null)
                    {
                        Staff staffData = player.GetUserData()?.StaffData;
                        if (staffData == null)
                            return;

                        submittedBy.SendChatMessage($"~g~INFO: ~w~Your ticket has been accepted by: {staffData.StaffName} ((ID:{PlayerHandler.GetIDFromPlayer(player)}))");
                        player.SendModeratorCommandMessage($"Ticket accepted! submitted by: {submittedBy.Name} ((ID:{ticketID}))");
                        tickets.Remove(ticketID);
                    }
                }
            }
        }
    }
}
