using GTANetworkAPI;
using GTARoleplay.Database;
using GTARoleplay.Events;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GTARoleplay.Vehicles
{
    public class VehicleHandler 
    {
        public static readonly string VEHICLE_DATA = "VehicleData";

        private readonly DatabaseBaseContext dbx;

        public VehicleHandler(DatabaseBaseContext dbx)
        {
            this.dbx = dbx;

            EventsHandler.Instance.OnVehicleDeath += OnVehicleDeath;
            EventsHandler.Instance.OnPlayerEnteredVehicle += OnPlayerEnterVehicle;
        }

        public void OnVehicleDeath(Vehicle vehicle)
        {
            if (vehicle == null)
                return;

            // The event VehicleDeath is broken
            GTAVehicle vehData = vehicle.GetVehicleData();
            if (vehData != null)
                vehData.IsSpawned = false;
        }

        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatID)
        {
            if (vehicle == null || player == null)
                return;

            GTAVehicle vehData = vehicle.GetVehicleData();
            vehData?.SendVehicleStatsToPlayer(player);
        }

        public void Save(GTAVehicle gtaVehicle)
        {
            dbx.Vehicles.Update(gtaVehicle);
            dbx.SaveChanges();
        }

        public void Save(GTAVehicleMod gtaVehicleMod)
        {
            var alreadyExists = dbx.VehicleMods.Any(x => x.VehicleModID == gtaVehicleMod.VehicleModID);
            dbx.Entry(gtaVehicleMod).State = (alreadyExists ? EntityState.Modified : EntityState.Added);
            dbx.SaveChanges();
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
