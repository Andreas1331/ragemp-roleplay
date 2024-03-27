using GTANetworkAPI;
using GTARoleplay.Events;
using GTARoleplay.Library.Chat;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles.Data;

namespace GTARoleplay.Vehicles
{
    public class VehicleCommands 
    {
        public VehicleCommands()
        {
            NAPI.Command.Register<Player>(new RuntimeCommandInfo("engine")
            {
                Alias = "vehengine,vehicleengine,vengine",
                ClassInstance = this
            }, ToggleVehicleEngine);

            NAPI.Command.Register<Player>(new RuntimeCommandInfo("vehiclelock")
            {
                Alias = "vehlock,vlock",
                ClassInstance = this
            }, ToggleVehicleLock);

            EventsHandler.Instance.YKeyPressed += ToggleVehicleEngine;
            EventsHandler.Instance.LKeyPressed += ToggleVehicleLock;
        }

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
