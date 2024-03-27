using GTANetworkAPI;
using GTARoleplay.InventorySystem;
using GTARoleplay.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.Animations
{
    public class AnimationCommands : Script
    {

        public AnimationCommands()
        {
            NAPI.Command.Register<Player, string>(new RuntimeCommandInfo("anim", "~y~USAGE: ~w~/anim [animation] to play an animation")
            {
                Alias = "an",
                ClassInstance = this
            }, PlayPlayerAnimation);

            NAPI.Command.Register<Player>(new RuntimeCommandInfo("stopanim")
            {
                Alias = "stopanimation,sa",
                ClassInstance = this
            }, StopPlayerAnimation);
        }

        public void PlayPlayerAnimation(Player player, string animation)
        {
            var foundAnim = AnimationHandler.StartAnimation(player, animation);
            if (!foundAnim)
                player.SendChatMessage($"The animation {animation} was not found!");
            else
                player.SendChatMessage("Use /stopanim, /sa or use the interaction wheel to stop the animation.");
        }

        public void StopPlayerAnimation(Player player)
        {
            AnimationHandler.StopAnimation(player);
        }
    }
}
