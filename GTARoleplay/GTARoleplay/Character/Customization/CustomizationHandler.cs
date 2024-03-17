using GTANetworkAPI;

namespace GTARoleplay.Character.Customization
{
    public class CustomizationHandler
    {
        public static void ShowClothesWindow(Player player)
        {
            if (player == null)
                return;

            player.TriggerEvent("EnableHUD::Client", false);
            player.TriggerEvent("ShowClothingSelector::Client");
        }
    }
}
