using GTARoleplay.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GTARoleplay.AdminSystem.Data
{
    [Flags]
    public enum ExtraCommand
    {
        None = 0,
        KickAll = 1 << 0,
        AlertSlack = 1 << 1,
    };

    public enum StaffRank { Moderator = 1, Level1, Level2, Level3, Level4, Level5, Management, Developer }

    [Table("staff")]
    public class Staff
    {
        [Key]
        [Column("id")]
        [ForeignKey("User")] 
        public int StaffID { get; set; }

        [Column("staff_name")]
        public string StaffName { get; set; }

        [Column("rank")]
        public StaffRank Rank { get; set; }

        [Column("special_permissions")]
        public ExtraCommand SpecialPermissions { get; set; }

        public virtual User User { get; set; }
    }
}
