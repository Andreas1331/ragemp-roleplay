using GTARoleplay.Database;
using GTARoleplay.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GTARoleplay.Character.Customization
{
    [Table("outfits")]
    public class CharacterOutfit
    {
        [Key]
        [Column("id")]
        public int OutfitID { get; set; }

        [Column("owner")]
        public int Owner { get; set; }

        [Column("owner_type")]
        public OwnerType OwnerType { get; set; }
        
        [Column("torso")]
        public int Torso { get; set; }
        [Column("torso_texture")]
        public int TorsoTexture { get; set; }

        [Column("leg")]
        public int Leg { get; set; }
        [Column("leg_texture")]
        public int LegTexture { get; set; }

        [Column("feet")]
        public int Feet { get; set; }
        [Column("feet_texture")]
        public int FeetTexture { get; set; }

        [Column("accessories")]
        public int Accessories { get; set; }
        [Column("accessories_texture")]
        public int AccessoriesTexture { get; set; }

        [Column("undershirt")]
        public int Undershirt { get; set; }
        [Column("undershirt_texture")]
        public int UndershirtTexture { get; set; }

        [Column("body_armor")]
        public int BodyArmor { get; set; }
        [Column("body_armor_texture")]
        public int BodyArmorTexture { get; set; }

        [Column("top")]
        public int Top { get; set; }
        [Column("top_texture")]
        public int TopTexture { get; set; }

        [Column("hat")]
        public int Hat { get; set; }
        [Column("hat_texture")]
        public int HatTexture { get; set; }

        [Column("glasses")]
        public int Glasses { get; set; }
        [Column("glasses_texture")]
        public int GlassesTexture { get; set; }

        [Column("ears")]
        public int Ears { get; set; }
        [Column("ears_texture")]
        public int EarsTexture { get; set; }

        public void Save()
        {
            using (var db = new DbConn())
            {
                db.Outfits.Update(this);
                db.SaveChanges();
            }
        }
    }
}
