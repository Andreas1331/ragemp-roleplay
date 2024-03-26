using GTANetworkAPI;
using GTARoleplay.Events;
using GTARoleplay.Library.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Library
{
    public class PlayerHandler 
    {
        private const string WELCOME_MESSAGE = "Welcome to GTA Roleplay!";

        public static readonly Dictionary<int, Player> PlayerList = new Dictionary<int, Player>();

        public PlayerHandler()
        {
            EventsHandler.Instance.OnPlayerDisconnected += OnPlayerDisconnected;
            EventsHandler.Instance.OnPlayerConnected += OnPlayerConnected;
        }

        public void OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            RemovePlayerFromPlayerList(player);
        }

        public void OnPlayerConnected(Player player)
        {
            player.SendChatMessage(WELCOME_MESSAGE);
            player.TriggerEvent("ShowLogin::Client");
            player.TriggerEvent("EnableHUD::Client", false);
            player.Freeze(true);
            player.Transparency = 0;
        }

        public static void AddPlayerToList(Player player)
        {
            // Lock the playerlist while a free id is being found
            lock (PlayerList)
            {
                int freeID = GetFreeID();
                PlayerList.Add(freeID, player);
            }
        }

        public static Player GetPlayer(int id)
        {
            return PlayerList.ContainsKey(id) ? PlayerList[id] : null;
        }

        public static Player GetPlayer(string name)
        {
            if (int.TryParse(name, out int id))
                return GetPlayer(id);

            return PlayerList.Values.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        public static int GetIDFromPlayer(Player player)
        {
            var entry = PlayerList.Where(e => e.Value == player)
                        .Select(e => (KeyValuePair<int, Player>?)e)
                        .FirstOrDefault();
            return entry == null ? -1 : (int)entry?.Key;
        }

        public static void RemovePlayerFromPlayerList(Player player)
        {
            // Lock the playerlist while the player is removed
            lock (PlayerList)
            {
                int playerID = GetIDFromPlayer(player);
                if(playerID != -1)
                {
                    PlayerList.Remove(playerID);
                }
            }
        }

        private static int GetFreeID()
        {
            List<int> currentIds = PlayerList.Keys.ToList();
            int freeID = 1;
            for (int i = 0; i < currentIds.Count; i++)
            {
                if (freeID != currentIds[i])
                    break;
                else freeID++;
            }

            return freeID;
        }
    }
}
