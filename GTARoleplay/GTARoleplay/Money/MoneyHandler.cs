using GTARoleplay.Character;
using GTARoleplay.Library;

namespace GTARoleplay.Money
{
    public static class MoneyHandler
    {
        public static bool HasEnoughMoney(GTACharacter gtaCharacter, int amount, bool printMsg = true)
        {
            bool hasEnough = gtaCharacter.Money >= amount;
            if (!hasEnough && printMsg)
                gtaCharacter.UserRef?.PlayerData?.SendChatMessage("~r~You don't have enough money to perform this action");

            return hasEnough;
        }

        public static void TakePlayerMoney(GTACharacter gtaCharacter, int amount)
        {
            gtaCharacter.Money -= amount;
            SendUpdatedCashToPlayer(gtaCharacter);
        }

        public static void GivePlayerMoney(GTACharacter gtaCharacter, int amount)
        {
            gtaCharacter.Money += amount;
            SendUpdatedCashToPlayer(gtaCharacter);
        }

        public static void SendUpdatedCashToPlayer(GTACharacter gtaCharacter)
        {
            // TODO: Add bank details
            gtaCharacter.UserRef?.PlayerData.TriggerEvent(
                "UpdateCashStats::Client",
                gtaCharacter.Money.ToString("C2", Gamemode.ServerCulture),
                1000);
        }
    }
}
