using GTANetworkAPI;
using System.Collections.Generic;

namespace GTARoleplay.Library.Chat
{
    public class MessageFunctions : Script
    {
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Player player, string message)
        {
            SendMessageToPlayersInRadius(player, 50, $"{player.Name} says: {message}");
        }

        public static void SendMessageToPlayersInRadiusColored(Player player, float radius, string message, string color, bool excludingSelf = true)
        {
            var playersNearby = NAPI.Player.GetPlayersInRadiusOfPlayer(radius, player);
            if (excludingSelf)
                playersNearby.RemoveAt(playersNearby.IndexOf(player));

            playersNearby.ForEach(ply =>
            {
                ply.SendChatMessage(color + message);
            });
        }

        public static void SendMessageToPlayersInRadius(Player player, float radius, string message, bool excludingSelf = false)
        {
            var playersNearby = NAPI.Player.GetPlayersInRadiusOfPlayer(radius, player);
            if (excludingSelf)
                playersNearby.RemoveAt(playersNearby.IndexOf(player));

            playersNearby.ForEach(ply =>
            {
                ply.SendChatMessage(message);
            });
        }
    }
}
