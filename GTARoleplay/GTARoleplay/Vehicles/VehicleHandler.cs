using GTANetworkAPI;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles.Data;

namespace GTARoleplay.Vehicles
{
    public class VehicleHandler : Script
    {
        public static readonly string VEHICLE_DATA = "VehicleData";

        [ServerEvent(Event.VehicleDeath)]
        public void OnVehicleDeath(Vehicle vehicle)
        {
            if (vehicle == null)
                return;

            // The event VehicleDeath is broken
            GTAVehicle vehData = vehicle.GetVehicleData();
            if (vehData != null)
                vehData.IsSpawned = false;
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatID)
        {
            if (vehicle == null || player == null)
                return;

            GTAVehicle vehData = vehicle.GetVehicleData();
            vehData?.SendVehicleStatsToPlayer(player);
        }

        public static Vehicle GetClosestVehicleToPlayer(Player player)
        {
            Vehicle closestVehicle = null;
            float previousDistance = 0;
            NAPI.Pools.GetAllVehicles().ForEach(tmpVeh =>
            {
                float distToVeh = player.Position.DistanceTo(tmpVeh.Position);

                if (closestVehicle == null)
                {
                    previousDistance = distToVeh;
                    closestVehicle = tmpVeh;
                }
                else if (distToVeh < previousDistance)
                {
                    previousDistance = distToVeh;
                    closestVehicle = tmpVeh;
                }
            });

            return closestVehicle;
        }

        public static Vehicle GetClosestVehicleToPlayer(Player player, float maxDistance)
        {
            Vehicle closestVehicle = null;
            float previousDistance = 0;
            NAPI.Pools.GetAllVehicles().ForEach(tmpVeh =>
            {
                float distToVeh = player.Position.DistanceTo(tmpVeh.Position);
                if (distToVeh <= maxDistance)
                {
                    // The vehicle is within max distance, so proceed
                    if (closestVehicle == null)
                    {
                        closestVehicle = tmpVeh;
                        previousDistance = distToVeh;
                    }
                    else if (distToVeh < previousDistance)
                    {
                        previousDistance = distToVeh;
                        closestVehicle = tmpVeh;
                    }
                }
            });

            return closestVehicle;
        }
    }
}
