using GTARoleplay.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GTARoleplay.Vehicles.Data
{
    [Table("vehicle_mods")]
    public class GTAVehicleMod
    {
        [Key]
        [Column("id")]
        [ForeignKey("VehicleData")]
        public int VehicleModID { get; set; }

        [Column("spoiler")]
        public int Spoiler { get; set; } = -1;

        [Column("front_bumper")]
        public int FrontBumper { get; set; } = -1;

        [Column("rear_bumper")]
        public int RearBumper { get; set; } = -1;

        [Column("side_skirt")]
        public int SideSkirt { get; set; } = -1;

        [Column("exhaust")]
        public int Exhaust { get; set; } = -1;

        [Column("frame")]
        public int Frame { get; set; } = -1;

        [Column("grille")]
        public int Grille { get; set; } = -1;

        [Column("hood")]
        public int Hood { get; set; } = -1;

        [Column("fender")]
        public int Fender { get; set; } = -1;

        [Column("right_fender")]
        public int RighFender { get; set; } = -1;

        [Column("roof")]
        public int Roof { get; set; } = -1;

        [Column("engine")]
        public int Engine { get; set; } = -1;

        [Column("brakes")]
        public int Brakes { get; set; } = -1;

        [Column("transmission")]
        public int Transmission { get; set; } = -1;

        [Column("horns")]
        public int Horns { get; set; } = -1;

        [Column("suspension")]
        public int Suspension { get; set; } = -1;

        [Column("armor")]
        public int Armor { get; set; } = -1;

        [Column("turbo")]
        public int Turbo { get; set; } = -1;

        [Column("xenon")]
        public int Xenon { get; set; } = -1;

        [Column("wheel_type")]
        public int WheelType { get; set; } = 0;

        [Column("front_wheels")]
        public int FrontWheels { get; set; } = -1;

        [Column("back_wheels")]
        public int BackWheels { get; set; } = -1;

        [Column("plate_holders")]
        public int PlateHolders { get; set; } = -1;

        [Column("trim_design")]
        public int TrimDesign { get; set; } = -1;

        [Column("ornaments")]
        public int Ornaments { get; set; } = -1;

        [Column("dial_design")]
        public int DialDesign { get; set; } = -1;

        [Column("steering_wheel")]
        public int SteeringWheel { get; set; } = -1;

        [Column("shift_lever")]
        public int ShiftLever { get; set; } = -1;

        [Column("plaques")]
        public int Plaques { get; set; } = -1;

        [Column("hydraulics")]
        public int Hydraulics { get; set; } = -1;

        [Column("boost")]
        public int Boost { get; set; } = -1;

        [Column("window_tint")]
        public int WindowTint { get; set; } = -1;

        [Column("livery")]
        public int Livery { get; set; } = -1;

        [Column("plate")]
        public int Plate { get; set; } = 0;

        public virtual GTAVehicle VehicleData { get; set; }

        public void Save()
        {
            using (var db = DatabaseService.GetDatabaseContext())
            {
                bool alreadyExists = db.VehicleMods.Any(x => x.VehicleModID == this.VehicleModID);
                db.Entry(this).State = (alreadyExists ? EntityState.Modified : EntityState.Added);
                db.SaveChanges();
            }
        }
    }
}
