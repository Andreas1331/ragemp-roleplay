using GTANetworkAPI;
using GTARoleplay.Character;
using GTARoleplay.Database;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles.Streaming;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTARoleplay.Vehicles.Data
{
    [Table("vehicles")]
    public class GTAVehicle
    {
        [Key]
        [Column("id")]
        public int VehicleID { get; set; }

        [Column("owner")]
        public int Owner { get; set; }

        [Column("owner_type")]
        public OwnerType OwnerType { get; set; }

        [Column("numberplate")]
        public string Numberplate { get; set; }

        [Column("model")]
        public string VehicleModel { get; set; }

        [Column("last_known_x")]
        public float LastX { get; set; }

        [Column("last_known_y")]
        public float LastY { get; set; }

        [Column("last_known_z")]
        public float LastZ { get; set; }

        [Column("last_known_rot")]
        public float LastRotation { get; set; }

        [Column("prim_color_r")]
        public int PrimColorR { get; set; }

        [Column("prim_color_g")]
        public int PrimColorG { get; set; }

        [Column("prim_color_b")]
        public int PrimColorB { get; set; }

        [Column("second_color_r")]
        public int SecondColorR { get; set; }

        [Column("second_color_g")]
        public int SecondColorG { get; set; }

        [Column("second_color_b")]
        public int SecondColorB { get; set; }

        [Column("fuel")]
        public float Fuel { get; set; }

        [Column("distance_travelled")]
        public float DistanceTravelled { get; set; }

        public virtual GTAVehicleMod Mods { get; set; }

        public Vector3 LastKnownPos => new Vector3(LastX, LastY, LastZ);
        public Color PrimaryColor => new Color(PrimColorR, PrimColorG, PrimColorB);
        public Color SecondaryColor => new Color(SecondColorR, SecondColorG, SecondColorB);

        public bool IsSpawned = false;
        public Vehicle Vehicle;

        public void SpawnVehicle()
        {
            if (IsSpawned)
                return;

            VehicleHash vehicleHash = NAPI.Util.VehicleNameToModel(VehicleModel);
            if (vehicleHash > 0)
            {
                // The constructor using Color() doesn't work, so set it manually after spawning
                Vehicle = NAPI.Vehicle.CreateVehicle(vehicleHash, LastKnownPos, LastRotation, 0, 0, Numberplate);
                Vehicle.SetData<GTAVehicle>(VehicleHandler.VEHICLE_DATA, this);
                VehicleStreaming.SetEngineState(Vehicle, true);
                ApplyVehicleMods();
                IsSpawned = true;
            }
        }

        public void DespawnVehicle()
        {
            if (IsSpawned && Vehicle != null)
            {
                Vehicle.Delete();
                IsSpawned = false;
            }
        }

        public void Save()
        {
            using (var db = new DbConn())
            {
                db.Vehicles.Update(this);
                db.SaveChanges();
            }
        }

        public void ApplyVehicleMods()
        {
            // The vehicle doesn't have any mods
            if (Mods == null)
                return;

            Vehicle.SetMod(0, Mods.Spoiler);
            Vehicle.SetMod(1, Mods.FrontBumper);
            Vehicle.SetMod(2, Mods.RearBumper);
            Vehicle.SetMod(3, Mods.SideSkirt);
            Vehicle.SetMod(4, Mods.Exhaust);
            Vehicle.SetMod(5, Mods.Frame);
            Vehicle.SetMod(6, Mods.Grille);
            Vehicle.SetMod(7, Mods.Hood);
            Vehicle.SetMod(8, Mods.Fender);
            Vehicle.SetMod(9, Mods.RighFender);
            Vehicle.SetMod(10, Mods.Roof);
            Vehicle.SetMod(11, Mods.Engine);
            Vehicle.SetMod(12, Mods.Brakes);
            Vehicle.SetMod(13, Mods.Transmission);
            Vehicle.SetMod(14, Mods.Horns);
            Vehicle.SetMod(15, Mods.Suspension);
            Vehicle.SetMod(16, Mods.Armor);
            Vehicle.SetMod(18, Mods.Turbo);
            Vehicle.SetMod(22, Mods.Xenon);
            Vehicle.WheelType = Mods.WheelType;
            Vehicle.SetMod(23, Mods.FrontWheels);
            Vehicle.SetMod(24, Mods.BackWheels);
            Vehicle.SetMod(25, Mods.PlateHolders); // Missing in UI
            Vehicle.SetMod(27, Mods.TrimDesign); // Missing in UI
            Vehicle.SetMod(28, Mods.Ornaments); // Missing in UI
            Vehicle.SetMod(30, Mods.DialDesign); // Missing in UI
            Vehicle.SetMod(33, Mods.SteeringWheel);
            Vehicle.SetMod(34, Mods.ShiftLever); // Missing in UI
            Vehicle.SetMod(35, Mods.Plaques); // Missing in UI
            Vehicle.SetMod(38, Mods.Hydraulics);
            Vehicle.SetMod(40, Mods.Boost);
            Vehicle.SetMod(55, Mods.WindowTint);
            Vehicle.SetMod(48, Mods.Livery);
            Vehicle.SetMod(53, Mods.Plate);

            Vehicle.CustomPrimaryColor = PrimaryColor;
            Vehicle.CustomSecondaryColor = SecondaryColor;
            NAPI.Chat.SendChatMessageToAll("Applied vehicle mods..");
        }

        public bool HasVehiclePerms(Player player)
        {
            // The player is either the direct owner, or part of the owning faction
            // TODO: Faction logic later
            GTACharacter charData = player.GetUserData()?.ActiveCharacter;
            return (charData != null) ? (charData.CharacterID.Equals(Owner) && OwnerType.Equals(OwnerType.Player)) : false;
        }

        public bool ChangeEngineState()
        {
            if (Vehicle == null)
                return false;
            bool currentEngine = VehicleStreaming.GetEngineState(Vehicle);
            if (currentEngine)
            {
                // Turn it off
                VehicleStreaming.SetEngineState(Vehicle, false);
                return false;
            }
            else
            {
                // Turn it on
                VehicleStreaming.SetEngineState(Vehicle, true);
                return true;
            }
        }

        public void SendVehicleStatsToPlayer(Player player)
        {
            player?.TriggerEvent("UpdateVehicleStats::Client", DistanceTravelled, Fuel);
        }
    }
}
