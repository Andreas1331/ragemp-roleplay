﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GTARoleplay.FactionSystem
{
    public enum FactionPerm { Core = 1, Overseer }

    [Table("faction_ranks")]
    public class FactionRank
    {
        [Key]
        [Column("id")]
        [ForeignKey("FactionData")]
        public int FactionID { get; set; }

        [Column("rank_name")]
        public string Name { get; set; }

        [Column("permission")]
        public FactionPerm Permission { get; set; }

        public virtual Faction FactionData { get; set; }
    }
}
