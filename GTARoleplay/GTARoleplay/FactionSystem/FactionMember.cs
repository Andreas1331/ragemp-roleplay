using GTARoleplay.Character.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTARoleplay.FactionSystem
{
    [Table("faction_members")]
    public class FactionMember
    {
        [Key]
        [Column("id")]
        [ForeignKey("Character")]
        public int CharacterID { get; set; }

        // 1-to-many relationship
        [ForeignKey("FactionRef")]
        [Column("factionid")]
        public int FactionID { get; set; }
        // in most cases wont be loaded
        public Faction FactionRef { get; set; }

        [Column("rank")]
        public int Rank { get; set; }

        public virtual GTACharacter Character { get; set; }
    }
}
