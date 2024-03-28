using GTANetworkAPI;
using GTARoleplay.Account.Data;
using GTARoleplay.Authentication;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Events;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GTARoleplay.Account
{
    public class AccountHandler 
    {
        public static readonly string USER_DATA = "UserData";

        private readonly DatabaseBaseContext _dbx;
        private readonly CharacterHandler _characterHandler;

        public AccountHandler(DatabaseBaseContext dbx, CharacterHandler characterHandler)
        {
            _dbx = dbx;
            _characterHandler = characterHandler;

            NAPI.ClientEvent.Register<Player, string, string>("OnLoginSubmitted::Server", this, AuthenticateLogin);
            NAPI.ClientEvent.Register<Player, string>("OnCharacterSelected::Server", this, OnCharacterSelected);
        }

        public void AuthenticateLogin(Player player, string username, string password)
        {
            // Find the user and validate the password
            // Include the users Admin profile, if he's an admin
            User user = _dbx
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
                EventsHandler.Instance.UserLoggedIn(player, user);
                player.TriggerEvent("DestroyLogin::Client");
                user.PlayerData = player;
                // Don't track this list of characters as the purpose for now is just to display the current information, not change it
                var characters = _dbx
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

        public void OnCharacterSelected(Player player, string characterName)
        {
            var pUser = player.GetUserData();
            var selectedChar = pUser
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
            // TODO: Put the version into a config class and load everything we need once instead
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            player.TriggerEvent("SetVersion::Client", $"{version.Major}.{version.Minor}.{version.Build}");

            _characterHandler.SpawnCharacter(pUser, selectedChar);
            pUser.Characters.Clear();
            pUser.Characters = null;

            // Unfreeze the player after the spawning is done
            player.TriggerEvent("DestroyCharSelector::Client");
            player.TriggerEvent("PlayerLoggedIn::Client");
            player.Freeze(false);
        }
    }
}
