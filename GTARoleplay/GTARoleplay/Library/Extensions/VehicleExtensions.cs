using GTANetworkAPI;
using GTARoleplay.Vehicles;
using GTARoleplay.Vehicles.Data;

namespace GTARoleplay.Library.Extensions
{
    public static class VehicleExtensions
    {
        public static GTAVehicle GetVehicleData(this Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.HasData(VehicleHandler.VEHICLE_DATA))
                return null;

            return vehicle.GetData<GTAVehicle>(VehicleHandler.VEHICLE_DATA);
        }
    }
}
