using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Library.Chat;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.FactionSystem
{
    public class FactionCommands : Script
    {
        [Command("faction", Alias = "f,fchat", GreedyArg = true)]
        public void SendFactionMessage(Player player, string message)
        {
            GTACharacter charData = player.GetUserData()?.ActiveCharacter;
            if (charData == null || charData.FactionMemberData == null)
                return;

            Faction faction = FactionHandler.GetFactionByID(charData.FactionMemberData.FactionID);
            if (faction == null)
                return;

            string rankName = faction.GetRankNameByIndex(charData.FactionMemberData.Rank);
            string messageRevamp = $"* (( {rankName} {player.Name}: {message} ))";

            foreach (Player ply in NAPI.Pools.GetAllPlayers())
            {
                if (ply == null)
                    continue;
                GTACharacter tmpCharData = ply.GetUserData()?.ActiveCharacter;
                if (tmpCharData == null || tmpCharData.FactionMemberData == null || tmpCharData.FactionMemberData == null)
                    continue;

                if (tmpCharData.FactionMemberData.FactionID == charData.FactionMemberData.FactionID)
                {
                    ply.SendChatMessage(ChatColors.FACTION_CHAT_COLOR + messageRevamp);
                }
            }
        }
    }
}
