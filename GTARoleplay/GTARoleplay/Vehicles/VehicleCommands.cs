using GTANetworkAPI;
using GTARoleplay.Events;
using GTARoleplay.Interactions;
using GTARoleplay.Library.Chat;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles.Data;

namespace GTARoleplay.Vehicles
{
    public class VehicleCommands : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            // When Y is pressed
            PlayerEvents.YKeyPressed += ToggleVehicleEngine;
            // When L is pressed
            PlayerEvents.LKeyPressed += ToggleVehicleLock;
        }

        [Command("engine", Alias = "vehengine,vehicleengine,vengine")]
        public void ToggleVehicleEngine(Player player)
        {
            if (player.Vehicle != null)
            {
                GTAVehicle vehData = player.Vehicle.GetVehicleData();
                if (vehData == null)
                    return;
                if (!vehData.HasVehiclePerms(player))
                    return;

                bool newEngineState = vehData.ChangeEngineState();
                string message = $"* {player.Name} turns on the engine of {player.Vehicle.DisplayName}.";
                if (!newEngineState)
                    message = $"* {player.Name} turns off the engine of {player.Vehicle.DisplayName}.";
                MessageFunctions.SendMessageToPlayersInRadiusColored(player, 30, message, ChatColors.ME_COLOR, excludingSelf: false);
            }
        }

        [Command("vehiclelock", Alias = "vehlock,vlock")]
        public void ToggleVehicleLock(Player player)
        {
            if (player == null)
                return;

            Vehicle veh = player.Vehicle ?? VehicleHandler.GetClosestVehicleToPlayer(player, 5);
            if (veh == null)
                return;

            GTAVehicle vehData = veh.GetVehicleData();
            if (vehData == null)
                return;
            if (!vehData.HasVehiclePerms(player))
                return;

            bool currentLock = veh.Locked;
            veh.Locked = !currentLock;

            string message = $"* {player.Name} locked {veh.DisplayName}.";
            if (currentLock)
                message = $"* {player.Name} unlocks {veh.DisplayName}.";
            MessageFunctions.SendMessageToPlayersInRadiusColored(player, 30, message, ChatColors.ME_COLOR, excludingSelf: false);
        }
    }
}
