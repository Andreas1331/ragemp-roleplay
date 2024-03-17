using GTANetworkAPI;
using GTARoleplay.Account.Data;
using GTARoleplay.Character.Customization;
using GTARoleplay.Database;
using GTARoleplay.FactionSystem.Data;
using GTARoleplay.InventorySystem;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.Library;
using GTARoleplay.Money;
using GTARoleplay.Vehicles.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GTARoleplay.Character.Data
{
    [Table("characters")]
    public class GTACharacter
    {
        [Key]
        [Column("id")]
        public int CharacterID { get; set; }

        [Column("firstname")]
        public string Firstname { get; set; }

        [Column("lastname")]
        public string Lastname { get; set; }

        [Column("last_known_x")]
        public float LastX { get; set; }

        [Column("last_known_y")]
        public float LastY { get; set; }

        [Column("last_known_z")]
        public float LastZ { get; set; }

        [Column("money")]
        public int Money { get; set; }

        // One-to-many relationship
        [ForeignKey("UserRef")]
        [Column("userid")]
        public int UserID { get; set; }
        public User UserRef { get; set; }

        public FactionMember FactionMemberData { get; set; }
        public CharacterOutfit OutfitData { get; set; }

        public string Fullname => Firstname + " " + Lastname;
        public Vector3 LastKnownPos => new Vector3(LastX, LastY, LastZ);

        public List<GTAVehicle> Vehicles;
        public Inventory Inventory;
  
        public static event Action<GTACharacter, Player> OnCharacterSpawned;

        public void SpawnCharacter(User user)
        {
            UserRef = user;
            UserRef.ActiveCharacter = this;
            UserRef.PlayerData.Position = LastKnownPos;
            UserRef.PlayerData.Name = Fullname;
            UserRef.PlayerData.Transparency = 255;

            // Load the players information
            var db = DatabaseService.GetDatabaseContext();
            List<Item> items = db
                .Items
                .Where(x => x.OwnerID.Equals(CharacterID) && x.OwnerType.Equals(OwnerType.Player))
                .ToList();

            if (OutfitData != null)
            {
                ApplyClothes();
            }

            List<GTAVehicle> vehicles = db.Vehicles
                .Include(x => x.Mods)
                .Where(x => x.Owner.Equals(CharacterID) && x.OwnerType == OwnerType.Player)
                .ToList();
            Vehicles = vehicles;

            List<Item> itms = db
                .Items
                .Where(x => x.OwnerID.Equals(CharacterID) && x.OwnerType.Equals(OwnerType.Player))
                .ToList();
            Inventory = new Inventory(itms);

            PlayerHandler.AddPlayerToList(UserRef.PlayerData);
            MoneyHandler.SendUpdatedCashToPlayer(this);

            OnCharacterSpawned?.Invoke(this, UserRef.PlayerData);
        }

        public void ApplyClothes()
        {
            if (OutfitData != null)
            {
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 3, OutfitData.Torso, OutfitData.TorsoTexture);
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 4, OutfitData.Leg, OutfitData.LegTexture);
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 6, OutfitData.Feet, OutfitData.FeetTexture);
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 7, OutfitData.Accessories, OutfitData.AccessoriesTexture);
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 8, OutfitData.Undershirt, OutfitData.UndershirtTexture);
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 9, OutfitData.BodyArmor, OutfitData.BodyArmorTexture);
                NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 11, OutfitData.Top, OutfitData.TopTexture);

                NAPI.Player.SetPlayerAccessory(UserRef.PlayerData, 0, OutfitData.Hat, OutfitData.HatTexture);
                NAPI.Player.SetPlayerAccessory(UserRef.PlayerData, 1, OutfitData.Glasses, OutfitData.GlassesTexture);
                NAPI.Player.SetPlayerAccessory(UserRef.PlayerData, 2, OutfitData.Ears, OutfitData.EarsTexture);
            }
        }
    }
}
