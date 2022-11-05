using GTANetworkAPI;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Character;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTARoleplay.Account
{
    [Table("accounts")]
    public class User
    {
        [Key]
        [Column("id")]
        public int UserID { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        public ICollection<GTACharacter> Characters { get; set; }

        // The staff table has a foreignkey to the user if the user is an admin
        public Staff StaffData { get; set; }

        [NotMapped]
        public GTACharacter ActiveCharacter { get; set; }

        [NotMapped]
        public Player PlayerData
        {
            get
            {
                return playerData;
            }
            set
            {
                value.SetData<User>(AccountHandler.USER_DATA, this);
                playerData = value;
            }
        }
        private Player playerData;

        public static event Action<Player, User> OnUserLoggedIn;

        public bool VerifyPassword(Player player, string password)
        {
            bool isPassValid = BCrypt.Net.BCrypt.Verify(password, Password);
            if (isPassValid)
                OnUserLoggedIn?.Invoke(player, this);
            return isPassValid;
        }
    }
}
