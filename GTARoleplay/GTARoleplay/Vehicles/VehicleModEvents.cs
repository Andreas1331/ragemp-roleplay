using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Money;
using GTARoleplay.Vehicles.Data;

namespace GTARoleplay.Vehicles
{
    public class VehicleModEvents : Script
    {
        private const int MOD_PRICE = 1000;

        [RemoteEvent("VehicleTravelled::Server")]
        public void UpdateVehicleStats(Player player, float distance)
        {
            if (player == null || player.Vehicle == null)
                return;

            if(distance > 0)
            {
                GTAVehicle vehData = player.Vehicle.GetVehicleData();
                if(vehData != null)
                {
                    // Update the vehicles distance travelled, and deduct fuel
                    vehData.DistanceTravelled += distance;
                    vehData.SendVehicleStatsToPlayer(player);
                    vehData.Save();
                }
            }
        }

        [RemoteEvent("PurchasePrimaryVehicleColor::Server")]
        public void PurchasePrimaryVehicleColor(Player player, Vehicle veh, int r, int g, int b)
        {
            if (player == null || veh == null)
                return;

            var character = player.GetUserData()?.ActiveCharacter;
            if (character != null)
            {
                if (MoneyHandler.HasEnoughMoney(character, MOD_PRICE))
                {
                    // Get the vehicle data
                    GTAVehicle vehData = veh.GetVehicleData();
                    if (vehData != null)
                    {
                        vehData.PrimColorR = r;
                        vehData.PrimColorG = g;
                        vehData.PrimColorB = b;
                        veh.CustomPrimaryColor = vehData.PrimaryColor;
                        vehData.Save();
                        // Deduct money .. 
                    }
                }
            }
        }

        [RemoteEvent("PurchaseSecondaryVehicleColor::Server")]
        public void PurchaseSecondaryVehicleColor(Player player, Vehicle veh, int r, int g, int b)
        {
            if (player == null || veh == null)
                return;

            var character = player.GetUserData()?.ActiveCharacter;
            if (character != null)
            {
                if (MoneyHandler.HasEnoughMoney(character, MOD_PRICE))
                {
                    // Get the vehicle data
                    GTAVehicle vehData = veh.GetVehicleData();
                    if (vehData != null)
                    {
                        vehData.SecondColorR = r;
                        vehData.SecondColorG = g;
                        vehData.SecondColorB = b;
                        veh.CustomSecondaryColor = vehData.SecondaryColor;
                        vehData.Save();
                        // Deduct money .. 
                    }
                }
            }
        }

        [RemoteEvent("PurchaseVehicleWheel::Server")]
        public void PurchaseVehicleWheel(Player player, Vehicle veh, int modType, int wheelType, int wheelValue)
        {
            if (player == null || veh == null)
                return;

            var character = player.GetUserData()?.ActiveCharacter;
            if (character != null)
            {
                if (MoneyHandler.HasEnoughMoney(character, MOD_PRICE))
                {
                    // Get the vehicle data
                    GTAVehicle vehData = veh.GetVehicleData();
                    if (vehData != null)
                    {
                        GTAVehicleMod mods = vehData.Mods ?? new GTAVehicleMod();
                        mods.WheelType = wheelType;

                        if (modType == 23)
                            mods.FrontWheels = wheelValue;
                        else if (modType == 24)
                            mods.BackWheels = wheelValue;
                        mods.Save();
                        // Deduct money .. 
                    }
                }
            }
        }

        [RemoteEvent("PurchaseVehicleMod::Server")]
        public void PurchaseVehicleMod(Player player, Vehicle veh, int modType, int modValue)
        {
            if (player == null || veh == null)
                return;

            var character = player.GetUserData()?.ActiveCharacter;
            if (MoneyHandler.HasEnoughMoney(character, MOD_PRICE))
            {
                // Get the vehicle data
                GTAVehicle vehData = veh.GetVehicleData();
                if (vehData != null)
                {
                    GTAVehicleMod mods = vehData.Mods ?? new GTAVehicleMod();

                    switch (modType)
                    {
                        case 0:
                            mods.Spoiler = modValue;
                            break;
                        case 1:
                            mods.FrontBumper = modValue;
                            break;
                        case 2:
                            mods.RearBumper = modValue;
                            break;
                        case 3:
                            mods.SideSkirt = modValue;
                            break;
                        case 4:
                            mods.Exhaust = modValue;
                            break;
                        case 5:
                            mods.Frame = modValue;
                            break;
                        case 6:
                            mods.Grille = modValue;
                            break;
                        case 7:
                            mods.Hood = modValue;
                            break;
                        case 8:
                            mods.Fender = modValue;
                            break;
                        case 9:
                            mods.RighFender = modValue;
                            break;
                        case 10:
                            mods.Roof = modValue;
                            break;
                        case 11:
                            mods.Engine = modValue;
                            break;
                        case 12:
                            mods.Brakes = modValue;
                            break;
                        case 13:
                            mods.Transmission = modValue;
                            break;
                        case 14:
                            mods.Horns = modValue;
                            break;
                        case 15:
                            mods.Suspension = modValue;
                            break;
                        case 16:
                            mods.Armor = modValue;
                            break;
                        case 18:
                            mods.Turbo = modValue;
                            break;
                        case 33:
                            mods.SteeringWheel = modValue;
                            break;
                        case 38:
                            mods.Hydraulics = modValue;
                            break;
                        case 40:
                            mods.Boost = modValue;
                            break;
                        case 46:
                            mods.WindowTint = modValue;
                            break;
                        case 53:
                            mods.Plate = modValue;
                            break;
                    }
                    mods.Save();
                    // Deduct money .... 
                }
            }
        }

        [RemoteEvent("RequestResyncOfVehicle::Server")]
        public void RequestResyncOfVehicle(Player player, Vehicle veh)
        {
            if (player == null || veh == null)
                return;
            player.TriggerEvent("EnableHUD::Client", true);
            var character = player.GetUserData()?.ActiveCharacter;
            if (character != null)
            {
                // Get the vehicle data
                GTAVehicle vehData = veh.GetVehicleData();
                vehData?.ApplyVehicleMods();
            }
        }
    }
}
