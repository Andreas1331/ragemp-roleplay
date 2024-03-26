using GTANetworkAPI;
using GTARoleplay.Database;

namespace GTARoleplay.Character.Customization
{
    public class CustomizationHandler
    {
        private readonly DatabaseBaseContext dbx;

        public CustomizationHandler(DatabaseBaseContext dbx)
        {
            this.dbx = dbx;
        }

        public void SaveCharacterOutfit(CharacterOutfit characterOutfit)
        {
            dbx.Outfits.Update(characterOutfit);
            dbx.SaveChanges();
        }

        public static void ShowClothesWindow(Player player)
        {
            if (player == null)
                return;

            player.TriggerEvent("EnableHUD::Client", false);
            player.TriggerEvent("ShowClothingSelector::Client");
        }
    }
}
