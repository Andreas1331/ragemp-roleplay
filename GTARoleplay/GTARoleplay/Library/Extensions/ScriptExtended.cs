﻿using GTANetworkAPI;
using GTARoleplay.Account;
using GTARoleplay.Character;

namespace GTARoleplay.Library.Extensions
{
    public class ScriptExtended : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            GTACharacter.OnCharacterSpawned += OnCharacterSpawned;
            AccountHandler.OnUserLoggedIn += OnUserLoggedIn;
        }

        public virtual void OnCharacterSpawned(GTACharacter character, Player player) { }
        public virtual void OnUserLoggedIn(Player player, User user) { }

    }
}
