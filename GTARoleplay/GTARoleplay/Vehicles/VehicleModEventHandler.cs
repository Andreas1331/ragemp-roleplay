using GTANetworkAPI;
using GTARoleplay.Database;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Money;
using GTARoleplay.Vehicles.Data;

namespace GTARoleplay.Vehicles
{
    public class VehicleModEventHandler 
    {
        private const int MOD_PRICE = 1000;

        private readonly DatabaseBaseContext dbx;
        private readonly VehicleHandler vehicleHandler;

        public VehicleModEventHandler(DatabaseBaseContext dbx, VehicleHandler vehicleHandler)
        {
            this.dbx = dbx;
            this.vehicleHandler = vehicleHandler;

            NAPI.ClientEvent.Register<Player, float>(
                "VehicleTravelled::Server", this, UpdateVehicleStats);
            NAPI.ClientEvent.Register<Player, Vehicle, int, int, int>(
                "PurchasePrimaryVehicleColor::Server", this, PurchasePrimaryVehicleColor);
            NAPI.ClientEvent.Register<Player, Vehicle, int, int, int>(
                "PurchaseSecondaryVehicleColor::Server", this, PurchaseSecondaryVehicleColor);
            NAPI.ClientEvent.Register<Player, Vehicle, int, int, int>(
                "PurchaseVehicleWheel::Server", this, PurchaseVehicleWheel);
            NAPI.ClientEvent.Register<Player, Vehicle, int, int>(
                "PurchaseVehicleMod::Server", this, PurchaseVehicleMod);
            NAPI.ClientEvent.Register<Player, Vehicle>(
                "RequestResyncOfVehicle::Server", this, RequestResyncOfVehicle);
        }

        public void UpdateVehicleStats(Player player, float distance)
        {
            if (player == null || player.Vehicle == null)
                return;

            if(distance > 0)
            {
                var vehData = player.Vehicle.GetVehicleData();
                if(vehData != null)
                {
                    // Update the vehicles distance travelled, and deduct fuel
                    vehData.DistanceTravelled += distance;
                    vehData.SendVehicleStatsToPlayer(player);
                    vehicleHandler.Save(vehData);
                }
            }
        }

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
                        vehicleHandler.Save(vehData);
                        // Deduct money .. 
                    }
                }
            }
        }

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
                        vehicleHandler.Save(vehData);
                        // Deduct money .. 
                    }
                }
            }
        }

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
                        vehicleHandler.Save(mods);
                        // Deduct money .. 
                    }
                }
            }
        }

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
                    vehicleHandler.Save(mods);
                    // Deduct money .... 
                }
            }
        }

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
