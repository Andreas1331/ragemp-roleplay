using GTANetworkAPI;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles.Streaming;

namespace GTARoleplay.AdminSystem
{
    public class AdminVehicleCommands
    {
        public AdminVehicleCommands()
        {
            NAPI.Command.Register<Player, string>(
                new RuntimeCommandInfo("spawnadminvehicle")
                {
                    Alias = "acar,aveh",
                    ClassInstance = this
                }, SpawnAdminVehicle);
        }

        public void SpawnAdminVehicle(Player player, string vehicleName)
        {
            if(AdminAuthorization.HasPermission(player, StaffRank.Level2))
            {
                VehicleHash vehicle = NAPI.Util.VehicleNameToModel(vehicleName);
                if(vehicle > 0)
                {
                    Vehicle veh = NAPI.Vehicle.CreateVehicle(vehicle, player.Position, 0, 10, 5);
                    VehicleStreaming.SetEngineState(veh, true);
                    player.SendChatMessage("Vehicle created!");
                }
            }
        }
    }
}
