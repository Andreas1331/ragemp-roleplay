using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Account
{
    public class AccountHandler : Script
    {
        public static readonly string USER_DATA = "UserData";

        [RemoteEvent("OnLoginSubmitted::Server")]
        public void AuthenticateLogin(Player player, string username, string password)
        {
            // The user didn't type anything valid in the login form
            if(String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                // Invalid input
                player.SendChatMessage("Invalid input!");
                return;
            }

            // Find the user and validate the password
            // Include the users Admin profile, if he's an admin
            User user = DatabaseService.GetDatabaseContext()
                .Users
                .Include(x => x.StaffData)
                .Where(x => string.Equals(username, x.Username) || string.Equals(username, x.Email))
                .FirstOrDefault();
            if (user == null)
            {
                // User doesn't exist
                player.SendChatMessage("User doesn't exist!");
                return;
            }
            else
            {
                if (!user.VerifyPassword(player, password))
                {
                    // Incorrect password
                    // TODO: Limit the amount of attempts to 3 and then kick him
                    player.SendChatMessage("Wrong password!");
                    return;
                }
                else
                {
                    // The player is logged in!
                    player.TriggerEvent("DestroyLogin::Client");
                    user.PlayerData = player;
                    // Don't track this list of characters as the purpose for now is just to display the current information, not change it
                    List<GTACharacter> characters = DatabaseService.GetDatabaseContext()
                        .Characters
                        .Include(x => x.FactionMemberData)
                        .Include(x => x.OutfitData)
                        .Where(x => x.UserID.Equals(user.UserID))
                        .AsNoTracking()
                        .ToList();
                    if (!characters.Any())
                    {
                        // No characters found
                        player.SendChatMessage("You don't have any characters to play with!");
                        return;
                    }
                    else
                    {
                        user.Characters = characters;
                        var characterNames = new List<string>
                            (from character in user.Characters select character.Fullname);
                        player.TriggerEvent("ShowCharSelector::Client", NAPI.Util.ToJson(characterNames));
                    }
                }
            }
        }

        [RemoteEvent("OnCharacterSelected::Server")]
        public void OnCharacterSelected(Player player, string characterName)
        {
            // The user didn't send a valid character
            if (String.IsNullOrWhiteSpace(characterName))
            {
                // Invalid input
                player.SendChatMessage("Invalid input!");
                return;
            }

            User pUser = player.GetUserData();
            if(pUser == null)
            {
                // Something went wrong when the player logged in
                return;
            }

            GTACharacter selectedChar = pUser
                .Characters
                .FirstOrDefault(x =>
                x.Fullname.Equals(characterName)
                && x.UserID.Equals(pUser.UserID));
            if (selectedChar == null)
            {
                // Somehow the user managed to send a character that he doesn't own
                return;
            }

            // Enable the HUD
            player.TriggerEvent("EnableHUD::Client", true);
            player.TriggerEvent("SetVersion::Client", Gamemode.VERSION);

            selectedChar.SpawnCharacter(pUser);
            pUser.Characters.Clear();
            pUser.Characters = null;

            // Unfreeze the player after the spawning is done
            player.TriggerEvent("DestroyCharSelector::Client");
            player.TriggerEvent("PlayerLoggedIn::Client");
            player.Freeze(false);
        }
    }
}
