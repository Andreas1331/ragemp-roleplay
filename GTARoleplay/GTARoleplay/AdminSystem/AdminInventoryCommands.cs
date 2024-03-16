using GTANetworkAPI;
using GTARoleplay.AdminSystem.Data;
using GTARoleplay.Character;
using GTARoleplay.InventorySystem;
using GTARoleplay.ItemSystem.Items;
using GTARoleplay.ItemSystem.Items.Factory;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using System.Collections.Generic;

namespace GTARoleplay.AdminSystem
{
    public class AdminInventoryCommands : Script
    {
        [Command("admingiveitem", Alias = "agive,agiveitem", GreedyArg = true)]
        public void AdminGivePlayerItem(Player player, string target, int amount, string item)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level2))
            {
                if(amount <= 0)
                {
                    player.SendChatMessage("~r~Please provide a valid amount!");
                    return;
                }

                // Find the target 
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    // Get the targets inventory
                    GTACharacter targetCharData = targetPly.GetUserData()?.ActiveCharacter;
                    if (targetCharData == null)
                        return;
                    Inventory inv = targetCharData.Inventory;
                    if(inv != null)
                    {
                        ItemFactory itmFactory = new ItemFactory(targetCharData.CharacterID, OwnerType.Player);
                        if(amount == 1)
                        {
                            Item itm = itmFactory.GetItem(item);
                            if(itm != null)
                            {
                                inv.AddItem(itm);
                                Item.AddItemToDatabase(itm);
                                targetPly.SendChatMessage($"You were given {amount} {itm.Name}");
                                player.SendChatMessage($"You gave {amount} {itm.Name} to {targetPly.Name}");
                            }
                            else
                            {
                                player.SendChatMessage("~r~The specified item does not exist!");
                            }
                        }
                        else
                        {
                            List<Item> itms = itmFactory.GetItems(item, amount);
                            if (itms != null)
                            {
                                inv.AddItems(itms);
                                Item.AddItemsToDatabase(itms);
                                targetPly.SendChatMessage($"You were given {amount} {itms[0].Name}");
                                player.SendChatMessage($"You gave {amount} {itms[0].Name} to {targetPly.Name}");
                            }
                            else
                            {
                                player.SendChatMessage("~r~The specified item does not exist!");
                            }
                        }
                    }
                }
            }
        }

        [Command("checkitems", Alias = "acheckitem,admincheckitems", GreedyArg = true)]
        public void CheckPlayerItems(Player player, string target)
        {
            if (AdminAuthorization.HasPermission(player, StaffRank.Level1))
            {
                Player targetPly = PlayerHandler.GetPlayer(target);
                if (targetPly != null)
                {
                    player.SendChatMessage($"Showing the inventory of {targetPly.Name}:");
                    player.SendChatMessage("___________________________________________");
                    Inventory inv = targetPly.GetUserData()?.ActiveCharacter?.Inventory;
                    if (inv != null)
                        inv.PrintInventory(player);
                    player.SendChatMessage("___________________________________________");
                }
                else 
                    player.SendChatMessage("~r~ERROR: ~w~Player not found.");
            }
        }
    }
}
