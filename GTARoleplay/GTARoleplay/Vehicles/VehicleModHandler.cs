using GTANetworkAPI;

namespace GTARoleplay.Vehicles
{
    public static class VehicleModHandler
    {
        public static void ShowModWindow(Player player, Vehicle veh)
        {
            if (player == null || veh == null)
                return;

            player.TriggerEvent("EnableHUD::Client", false);
            player.TriggerEvent("ShowVehicleModding::Client", veh.Handle);
        }
    }
}
