using GTANetworkAPI;
using GTARoleplay.Library;
using GTARoleplay.Library.Chat;

namespace GTARoleplay.Players
{
    public class PlayerCommands : Script
    {
        [Command("b", GreedyArg = true)]
        public void PrintBMessage(Player player, string text)
        {
            string message = $"(( [{PlayerHandler.GetIDFromPlayer(player)}] {player.Name}: {text} ))";
            MessageFunctions.SendMessageToPlayersInRadiusColored(player, 30, message, "~#FFFFFF~", excludingSelf: false);
        }

        [Command("players", Alias = "playersonline,online")]
        public void PrintPlayersOnline(Player player)
        {
            // TODO: In the future, make an HTML list to display lots of players
            var playerLst = PlayerHandler.PlayerList;
            player.SendChatMessage($"There are {playerLst.Count} player(s) online");
            foreach (var kp in playerLst)
                player.SendChatMessage($"Player: {kp.Value.Name}, ID: {kp.Key}");
        }

        [Command("me", GreedyArg = true)]
        public void PrintMeAction(Player player, string text)
        {
            string message = $"* {player.Name} {text}";
            MessageFunctions.SendMessageToPlayersInRadiusColored(player, ChatDistances.ME_DST, message, ChatColors.ME_COLOR, excludingSelf: false);
        }

        [Command("do", GreedyArg = true)]
        public void PrintDoAction(Player player, string text)
        {
            string message = $"* {text} (( {player.Name} ))";
            MessageFunctions.SendMessageToPlayersInRadiusColored(player, 30, message, ChatColors.DO_COLOR, excludingSelf: false);
        }

        [Command("shout", Alias = "s", GreedyArg = true)]
        public void PrintShoutMessage(Player player, string text)
        {
            string message = $"~h~{player.Name} shouts: {text}!";
            MessageFunctions.SendMessageToPlayersInRadiusColored(player, 60, message, ChatColors.SHOUT_COLOR, excludingSelf: false);
        }

        [Command("melow", GreedyArg = true)]
        public void PrintMeLowAction(Player player, string text)
        {
            string message = $"* {player.Name} {text}";
            MessageFunctions.SendMessageToPlayersInRadiusColored(player, 15, message, ChatColors.ME_COLOR, excludingSelf: false);
        }

        [Command("low", GreedyArg = true)]
        public void PrintLowMessage(Player player, string text)
        {
            string message = player.Name + $" says [LOW]: {text}";
            MessageFunctions.SendMessageToPlayersInRadius(player, 10, message);
        }

        [Command("pm", Alias = "privatemessage", GreedyArg = true)]
        public void SendPrivateMessage(Player player, string target, string message)
        {
            Player targetPly = PlayerHandler.GetPlayer(target);
            if (targetPly != null)
            {
                if(targetPly == player)
                {
                    player.SendChatMessage("You're not able to perform this action on yourself!");
                    return;
                }

                int targetID = PlayerHandler.GetIDFromPlayer(targetPly);
                string messageSender = $"(( PM sent to [{targetID}] {targetPly.Name}: {message} ))";
                string messageTarget = $"(( PM from [{PlayerHandler.GetIDFromPlayer(player)}] {player.Name}: {message} ))";
                NAPI.Chat.SendChatMessageToPlayer(player, ChatColors.PM_TO_COLOR + messageSender);
                NAPI.Chat.SendChatMessageToPlayer(targetPly, ChatColors.PM_FROM_COLOR + messageTarget);
            }
            else
            {
                player.SendChatMessage($"No player found with the name or id {target}");
            }
        }
    }
}
