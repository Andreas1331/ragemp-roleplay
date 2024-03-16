using GTANetworkAPI;
using GTARoleplay.Authentication;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Account
{
    public class AccountHandler : Script
    {
        public static readonly string USER_DATA = "UserData";

        public static event Action<Player, User> OnUserLoggedIn;

        [RemoteEvent("OnLoginSubmitted::Server")]
        public void AuthenticateLogin(Player player, string username, string password)
        {
            // Find the user and validate the password
            // Include the users Admin profile, if he's an admin
            User user = DatabaseService.GetDatabaseContext()
                .Users
                .Include(x => x.StaffData)
                .Where(x => string.Equals(username, x.Username) || string.Equals(username, x.Email))
                .AsNoTracking()
                .FirstOrDefault();
            if (user == null)
            {
                // User doesn't exist
                player.SendChatMessage("User doesn't exist!");
                return;
            }
            if (Authenticator.VerfiyPasswordAgainstUser(user, password))
            {
                OnUserLoggedIn?.Invoke(player, user);
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

                user.Characters = characters;
                var characterNames = new List<string>
                    (from character in user.Characters select character.Fullname);
                player.TriggerEvent("ShowCharSelector::Client", NAPI.Util.ToJson(characterNames));

                if (!characters.Any())
                    player.SendChatMessage("You don't have any characters to play with!");
            }
            else
            {
                // Incorrect password
                // TODO: Limit the amount of attempts to 3 and then kick him
                player.SendChatMessage("Wrong password!");
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
            GTACharacter selectedChar = pUser
                .Characters
                .FirstOrDefault(x =>
                x.Fullname.Equals(characterName));
            if (selectedChar == null)
            {
                // Somehow the user managed to send a name of a character he does not own
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
