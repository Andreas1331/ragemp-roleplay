using GTANetworkAPI;
using GTARoleplay.Account;
using GTARoleplay.Character.Customization;
using GTARoleplay.Database;
using GTARoleplay.FactionSystem;
using GTARoleplay.InventorySystem;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.Library;
using GTARoleplay.Vehicles.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GTARoleplay.Character
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

        [Column("current_outfit")]
        public int CurrentOutfitID { get; set; } = -1;

        // The faction member table has a foreignkey to the character if the character is in a faction
        public FactionMember FactionMemberData { get; set; }

        public string Fullname => Firstname + " " + Lastname;
        public Vector3 LastKnownPos => new Vector3(LastX, LastY, LastZ);

        public List<GTAVehicle> Vehicles;
        public Inventory Inventory;
        public CharacterOutfit CurrentOutfit;

        public static event Action<GTACharacter, Player> OnCharacterSpawned;

        public void SpawnCharacter(User user)
        {
            UserRef = user;
            UserRef.ActiveCharacter = this;
            UserRef.PlayerData.Position = LastKnownPos;
            UserRef.PlayerData.Name = Fullname;

            // Load the players information
            using (var db = new DbConn())
            {
                List<Item> items = db.Items.Where(x => x.OwnerID.Equals(CharacterID) && x.OwnerType.Equals(OwnerType.Player)).ToList();

                if(CurrentOutfitID > -1)
                {
                    CharacterOutfit outfit = db.Outfits.Where(x => x.OutfitID == CurrentOutfitID).FirstOrDefault();
                    if (outfit != null)
                    {
                        CurrentOutfit = outfit;
                        ApplyClothes(outfit);
                    }
                }

                List<GTAVehicle> vehicles = db.Vehicles.Include(x => x.Mods).Where(x => x.Owner.Equals(CharacterID) && x.OwnerType == OwnerType.Player).ToList();
                Vehicles = vehicles;

                List<Item> itms = db.Items.Where(x => x.OwnerID.Equals(CharacterID) && x.OwnerType.Equals(OwnerType.Player)).ToList();
                Inventory = new Inventory(itms);
            }

            PlayerHandler.AddPlayerToList(UserRef.PlayerData);
            SendUpdatedCashToPlayer();

            OnCharacterSpawned?.Invoke(this, UserRef.PlayerData);
        }

        public bool HasEnoughMoney(int amount, bool printMsg = true)
        {
            bool hasEnough = Money >= amount;
            if (!hasEnough && printMsg)
                UserRef?.PlayerData?.SendChatMessage("~r~You don't have enough money to perform this action");

            return hasEnough;
        }

        public void TakePlayerMoney(int amount)
        {
            Money -= amount;
            SendUpdatedCashToPlayer();
        }

        public void GivePlayerMoney(int amount, bool saveToDatabase = true)
        {
            Money += amount;
            SendUpdatedCashToPlayer();
        }

        private void SendUpdatedCashToPlayer()
        {
            // TODO: Add bank details
            UserRef?.PlayerData.TriggerEvent("UpdateCashStats::Client", Money.ToString("C2", Gamemode.ServerCulture), 1000);
        }

        public void ApplyClothes(CharacterOutfit outfit)
        {
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 3, outfit.Torso, outfit.TorsoTexture);
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 4, outfit.Leg, outfit.LegTexture);
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 6, outfit.Feet, outfit.FeetTexture);
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 7, outfit.Accessories, outfit.AccessoriesTexture);
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 8, outfit.Undershirt, outfit.UndershirtTexture);
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 9, outfit.BodyArmor, outfit.BodyArmorTexture);
            NAPI.Player.SetPlayerClothes(UserRef.PlayerData, 11, outfit.Top, outfit.TopTexture);

            NAPI.Player.SetPlayerAccessory(UserRef.PlayerData, 0, outfit.Hat, outfit.HatTexture);
            NAPI.Player.SetPlayerAccessory(UserRef.PlayerData, 1, outfit.Glasses, outfit.GlassesTexture);
            NAPI.Player.SetPlayerAccessory(UserRef.PlayerData, 2, outfit.Ears, outfit.EarsTexture);
        }

        public void ApplyClothes()
        {
            if (CurrentOutfit != null)
                ApplyClothes(CurrentOutfit);
        }
    }
}
