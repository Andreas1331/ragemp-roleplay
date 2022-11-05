using GTANetworkAPI;

namespace GTARoleplay.Vehicles.Streaming
{
    //This is the data object which will be synced to vehicles
    public class VehicleSyncData
    {
        //Used to bypass some streaming bugs
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Rotation { get; set; } = new Vector3();

        //Basics
        public float Dirt { get; set; } = 0.0f;
        public bool Locked { get; set; } = true;
        public bool Engine { get; set; } = false;

        //(Not synced)
        public float BodyHealth { get; set; } = 1000.0f;
        public float EngineHealth { get; set; } = 1000.0f;

        //Doors 0-7 (0 = closed, 1 = open, 2 = broken) (This uses enums so don't worry about it)
        public int[] Door { get; set; } = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

        //Windows (0 = up, 1 = down, 2 = smashed) (This uses enums so don't worry about it)
        public int[] Window { get; set; } = new int[4] { 0, 0, 0, 0 };

        //Wheels 0-7, 45/47 (0 = fixed, 1 = flat, 2 = missing) (This uses enums so don't worry about it)
        public int[] Wheel { get; set; } = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
}

