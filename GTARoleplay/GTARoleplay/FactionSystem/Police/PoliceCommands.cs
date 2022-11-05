using GTANetworkAPI;
using GTARoleplay.Animations;
using GTARoleplay.Character;
using GTARoleplay.Library;
using GTARoleplay.Library.Chat;
using GTARoleplay.Library.Extensions;

namespace GTARoleplay.FactionSystem.Police
{
    public class PoliceCommands : Script
    {
        [Command("handcuff", "~y~USAGE: ~w~/hand(cuff) [Target Name / ID]", Alias = "cuff")]
        public void HandcuffPlayer(Player player, string target)
        {
            if (player == null)
                return;

            GTACharacter charData = player.GetUserData()?.ActiveCharacter;
            if (charData == null || charData.FactionMemberData == null)
                return;

            Faction faction = FactionHandler.GetFactionByID(charData.FactionMemberData.FactionID);
            if (faction == null)
                return;

            // The player is LSPD
            if (FactionHandler.LSPD_FACTION_NAME.Equals(faction.Name))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if(targetPly != null)
                {
                    // Cuff the target
                    AnimationHandler.StartAnimation(targetPly, "handcuff");
                    targetPly.ChangeRestrictedState(true);
                    targetPly.TriggerEvent("ChangeHandcuffsState::Client", true);
                    targetPly.TriggerEvent("SetCurrentPedWeapon::Client", WeaponHash.Unarmed);

                    string message = $"* {player.Name} puts handcuffs on {targetPly.Name}.";
                    MessageFunctions.SendMessageToPlayersInRadiusColored(player, ChatDistances.ME_DST, message, ChatColors.ME_COLOR, excludingSelf: false);
                    // freemode male
                    if (targetPly.Model == 1885233650)
                        targetPly.SetClothes(7, 41, 0);
                    else 
                        targetPly.SetClothes(7, 25, 0);
                }
            }
        }

        [Command("unhandcuff", "~y~USAGE: ~w~/unhandcuff [Target Name / ID]", Alias = "uncuff")]
        public void UnHandcuffPlayer(Player player, string target)
        {
            if (player == null)
                return;

            GTACharacter charData = player.GetUserData()?.ActiveCharacter;
            if (charData == null || charData.FactionMemberData == null)
                return;

            Faction faction = FactionHandler.GetFactionByID(charData.FactionMemberData.FactionID);
            if (faction == null)
                return;

            // The player is LSPD
            if (FactionHandler.LSPD_FACTION_NAME.Equals(faction.Name))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    // Uncuff the target
                    AnimationHandler.StopAnimation(targetPly);
                    targetPly.ChangeRestrictedState(false);
                    targetPly.TriggerEvent("ChangeHandcuffsState::Client", false);

                    string message = $"* {player.Name} takes handcuffs off {targetPly.Name}.";
                    MessageFunctions.SendMessageToPlayersInRadiusColored(player, ChatDistances.ME_DST, message, ChatColors.ME_COLOR, excludingSelf: false);

                    // Remove the cuff object, by re-applying the players entire outfit
                    GTACharacter targetCharData = targetPly.GetUserData()?.ActiveCharacter;
                    targetCharData?.ApplyClothes();
                }
            }
        }
    }
}
