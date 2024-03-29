﻿using GTANetworkAPI;
using GTARoleplay.Library.Extensions;
using System.Collections.Generic;

namespace GTARoleplay.Character.Customization
{
    public class CustomizationEvents 
    {
        private readonly CustomizationHandler customizationHandler;

        public CustomizationEvents(CustomizationHandler customizationHandler)
        {
            this.customizationHandler = customizationHandler;

            NAPI.ClientEvent.Register<Player, object>("PurchaseOutfit::Server", this, PurchaseOutfit);
            NAPI.ClientEvent.Register<Player>("RequestResyncOfPlayerClothes::Server", this, RequestResyncOfPlayerClothes);
        }

        public void PurchaseOutfit(Player player, object outfitObj)
        {
            if (outfitObj == null)
                return;

            var charData = player.GetUserData()?.ActiveCharacter;
            var currentOutfit = charData?.OutfitData;

            var outfit = NAPI.Util.FromJson<OutfitContainer>(outfitObj);
            foreach (KeyValuePair<int, OutfitObject> kp in outfit.ClothesAndAccessories)
            {
                switch (kp.Key)
                {
                    case 0:
                        currentOutfit.Hat = kp.Value.Drawable;
                        currentOutfit.HatTexture = kp.Value.Texture;
                        break;
                    case 1:
                        currentOutfit.Glasses = kp.Value.Drawable;
                        currentOutfit.GlassesTexture = kp.Value.Texture;
                        break;
                    case 2:
                        currentOutfit.Ears = kp.Value.Drawable;
                        currentOutfit.EarsTexture = kp.Value.Texture;
                        break;
                    case 3:
                        currentOutfit.Torso = kp.Value.Drawable;
                        currentOutfit.TorsoTexture = kp.Value.Texture;

                        break;
                    case 4:
                        currentOutfit.Leg = kp.Value.Drawable;
                        currentOutfit.LegTexture = kp.Value.Texture;

                        break;
                    case 6:
                        currentOutfit.Feet = kp.Value.Drawable;
                        currentOutfit.FeetTexture = kp.Value.Texture;

                        break;
                    case 7:
                        currentOutfit.Accessories = kp.Value.Drawable;
                        currentOutfit.AccessoriesTexture = kp.Value.Texture;

                        break;
                    case 8:
                        currentOutfit.Undershirt = kp.Value.Drawable;
                        currentOutfit.UndershirtTexture = kp.Value.Texture;

                        break;
                    case 9:
                        currentOutfit.BodyArmor = kp.Value.Drawable;
                        currentOutfit.BodyArmorTexture = kp.Value.Texture;

                        break;
                    case 11:
                        currentOutfit.Top = kp.Value.Drawable;
                        currentOutfit.TopTexture = kp.Value.Texture;

                        break;
                }
            }
            customizationHandler.SaveCharacterOutfit(currentOutfit);
            player.SendChatMessage("You've purchased new items for your outfit!");
        }

        public void RequestResyncOfPlayerClothes(Player player)
        {
            if (player == null)
                return;

            player.TriggerEvent("EnableHUD::Client", true);
            var charData = player.GetUserData()?.ActiveCharacter;
            charData.ApplyClothes();
        }
    }
}
