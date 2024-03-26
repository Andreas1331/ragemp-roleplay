using GTARoleplay.Account.Data;
using GTARoleplay.Character.Data;
using GTARoleplay.Database;
using GTARoleplay.InventorySystem;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.Library;
using GTARoleplay.Money;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTARoleplay.Character
{
    public class CharacterHandler
    {
        private readonly DatabaseBaseContext dbx;

        public CharacterHandler(DatabaseBaseContext dbx)
        {
            this.dbx = dbx;
        }

        public void SpawnCharacter(User user, GTACharacter gtaCharacter)
        {
            gtaCharacter.UserRef = user;
            gtaCharacter.UserRef.ActiveCharacter = gtaCharacter;
            gtaCharacter.UserRef.PlayerData.Position = gtaCharacter.LastKnownPos;
            gtaCharacter.UserRef.PlayerData.Name = gtaCharacter.Fullname;
            gtaCharacter.UserRef.PlayerData.Transparency = 255;

            if (gtaCharacter.OutfitData != null)
            {
                gtaCharacter.ApplyClothes();
            }

            var vehicles = dbx.Vehicles
                .Include(x => x.Mods)
                .Where(x => x.Owner.Equals(gtaCharacter.CharacterID) && x.OwnerType == OwnerType.Player)
                .ToList();
            gtaCharacter.Vehicles = vehicles;

            var itms = dbx
                .Items
                .Where(x => x.OwnerID.Equals(gtaCharacter.CharacterID) && x.OwnerType.Equals(OwnerType.Player))
                .ToList();
            gtaCharacter.Inventory = new Inventory(itms);

            PlayerHandler.AddPlayerToList(gtaCharacter.UserRef.PlayerData);
            MoneyHandler.SendUpdatedCashToPlayer(gtaCharacter);
        }
    }
}
