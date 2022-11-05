using GTANetworkAPI;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Vehicles.Streaming;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.AdminSystem
{
    public class AdminVehicleCommands : Script
    {
        [Command("spawnadminvehicle", Alias ="acar,aveh")]
        public void SpawnAdminVehicle(Player player, string vehicleName)
        {
            if(AdminAuthentication.HasPermission(player, StaffRank.Level2))
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

        [Command("mypos")]
        public void PrintMyPosition(Player player)
        {
            if (AdminAuthentication.HasPermission(player, StaffRank.Level1))
            {
                Vector3 pos = player.Position;
                player.SendChatMessage($"Your position ({pos.X}, {pos.Y}, {pos.Z})");
            }
        }
    }
}
