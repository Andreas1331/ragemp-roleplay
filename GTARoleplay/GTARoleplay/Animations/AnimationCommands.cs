using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.Animations
{
    public class AnimationCommands : Script
    {
        [Command("anim", "~y~USAGE: ~w~/anim [animation] to play an animation", Alias = "an")]
        public void PlayPlayerAnimation(Player player, string animation)
        {
            bool foundAnim = AnimationHandler.StartAnimation(player, animation);
            if (!foundAnim)
                player.SendChatMessage($"The animation {animation} was not found!");
            else
                player.SendChatMessage("Use /stopanim, /sa or use the interaction wheel to stop the animation.");
        }

        [Command("stopanim", Alias = "stopanimation,sa")]
        public void StopPlayerAnimation(Player player)
        {
            AnimationHandler.StopAnimation(player);
        }
    }
}
