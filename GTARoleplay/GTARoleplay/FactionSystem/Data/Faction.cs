using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GTARoleplay.FactionSystem.Data
{
    [Table("factions")]
    public class Faction
    {
        [Key]
        [Column("id")]
        public int FactionID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("abbreviation")]
        public string Abbreviation { get; set; }

        public ICollection<FactionMember> Members { get; set; }
        public ICollection<FactionRank> Ranks { get; set; }

        public string GetRankNameByIndex(int rank)
        {
            if(Ranks != null)
            {
                if (Ranks.ToList().Count > rank)
                    return Ranks.ToList()[rank].Name;
            }
            return "MissingRankName";
        }
    }
}
