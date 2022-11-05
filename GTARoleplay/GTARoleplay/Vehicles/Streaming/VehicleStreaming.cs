using GTANetworkAPI;
using Newtonsoft.Json.Linq;

namespace GTARoleplay.Vehicles.Streaming
{
    #region Enums
    //Enums for ease of use
    public enum WindowID
    {
        WindowFrontRight,
        WindowFrontLeft,
        WindowRearRight,
        WindowRearLeft
    }
    public enum WindowState
    {
        WindowFixed,
        WindowDown,
        WindowBroken
    }
    public enum DoorID
    {
        DoorFrontLeft,
        DoorFrontRight,
        DoorRearLeft,
        DoorRearRight,
        DoorHood,
        DoorTrunk
    }
    public enum DoorState
    {
        DoorClosed,
        DoorOpen,
        DoorBroken,
    }
    public enum WheelID
    {
        Wheel0,
        Wheel1,
        Wheel2,
        Wheel3,
        Wheel4,
        Wheel5,
        Wheel6,
        Wheel7,
        Wheel8,
        Wheel9
    }
    public enum WheelState
    {
        WheelFixed,
        WheelBurst,
        WheelOnRim,
    }
    #endregion

    public class VehicleStreaming : Script
    {
        public static void SetDoorState(Vehicle veh, DoorID door, DoorState state)
        {
            VehicleSyncData data = GetVehicleSyncData(veh);
            if (data == default(VehicleSyncData))
                data = new VehicleSyncData();

            data.Door[(int)door] = (int)state;
            UpdateVehicleSyncData(veh, data);
            NAPI.ClientEvent.TriggerClientEventInDimension(veh.Dimension, "VehStream_SetVehicleDoorStatus_Single", veh, (int)door, (int)state);
        }

        public static DoorState GetDoorState(Vehicle veh, DoorID door)
        {
            VehicleSyncData data = GetVehicleSyncData(veh);
            if (data == default(VehicleSyncData))
            {
                data = new VehicleSyncData();
                UpdateVehicleSyncData(veh, data);
            }
            return (DoorState)data.Door[(int)door];
        }

        public static void SetEngineState(Vehicle veh, bool status)
        {
            if (veh == null)
                return;
            if (veh.GetType() != typeof(Vehicle))
                return;

            VehicleSyncData data = GetVehicleSyncData(veh);
            if (data == default(VehicleSyncData))
                data = new VehicleSyncData();

            data.Engine = status;
            UpdateVehicleSyncData(veh, data);
            NAPI.ClientEvent.TriggerClientEventInDimension(veh.Dimension, "VehStream_SetEngineStatus", veh, status);
        }

        public static bool GetEngineState(Vehicle veh)
        {
            VehicleSyncData data = GetVehicleSyncData(veh);
            if (data == default(VehicleSyncData))
            {
                data = new VehicleSyncData();
                UpdateVehicleSyncData(veh, data);
            }
            return data.Engine;
        }

        private static VehicleSyncData GetVehicleSyncData(Vehicle veh)
        {
            try
            {
                if (veh != null)
                {
                    if (NAPI.Entity.DoesEntityExist(veh))
                    {
                        if (NAPI.Data.HasEntitySharedData(veh, "VehicleSyncData"))
                        {
                            if (NAPI.Data.GetEntitySharedData(veh, "VehicleSyncData") is int)
                                return default(VehicleSyncData);
                            JObject obj = (JObject)NAPI.Data.GetEntitySharedData(veh, "VehicleSyncData");
                            return obj.ToObject<VehicleSyncData>();
                        }
                    }
                }

                return default(VehicleSyncData); //null
            }
            catch
            {
                return default(VehicleSyncData); //null
            }
        }

        public static bool UpdateVehicleSyncData(Vehicle veh, VehicleSyncData data)
        {
            if (veh != null)
            {
                if (NAPI.Entity.DoesEntityExist(veh))
                {
                    if (data != null)
                    {
                        data.Position = veh.Position;
                        data.Rotation = veh.Rotation;
                        veh.SetSharedData("VehicleSyncData", data);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
