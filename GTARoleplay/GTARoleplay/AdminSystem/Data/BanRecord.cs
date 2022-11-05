using GTARoleplay.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GTARoleplay.AdminSystem.Data
{
    [Table("ban_records")]
    public class BanRecord
    {
        [Key]
        [Column("id")]
        [ForeignKey("User")] // The string value is the property name, named 'User' of type 'User' as seen down below
        public int UserID { get; set; }

        [Column("banned_by")]
        public string BannedBy { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("ip")]
        public string IpAddress { get; set; }

        [Column("social_club_name")]
        public string SocialClubName { get; set; }

        [Column("banned_date")]
        public DateTime BannedDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        public virtual User User { get; set; }

        public BanRecord()
        {

        }

        public BanRecord(int userID, string bannedBy, string reason, string ipAddress, string socialClubname, DateTime date, bool isActive = true)
        {
            this.UserID = userID;
            this.BannedBy = bannedBy;
            this.Reason = reason;
            this.IpAddress = ipAddress;
            this.SocialClubName = socialClubname;
            this.BannedDate = date;
            this.IsActive = isActive;
        }
    }
}
