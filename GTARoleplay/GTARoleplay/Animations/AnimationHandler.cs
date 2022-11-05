using GTANetworkAPI;
using GTARoleplay.Animations.Data;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTARoleplay.Animations
{
    public class AnimationHandler : Script
    {
        public static readonly List<Animation> AllAnimations = new List<Animation>()
        {
            new Animation("Fuck you", "fuckyou", "mp_player_intfinger", "mp_player_int_finger", null, AnimationFlags.StopOnLastFrame),
            new Animation("Guitar", "", "anim@mp_player_intcelebrationmale@air_guitar", "air_guitar", null, AnimationFlags.StopOnLastFrame),
            new Animation("Shagging", "", "anim@mp_player_intcelebrationmale@air_shagging", "air_shagging", null, AnimationFlags.StopOnLastFrame),
            new Animation("Synth", "", "anim@mp_player_intcelebrationmale@air_synth", "air_synth", null, AnimationFlags.Loop),
            new Animation("Kiss", "", "anim@mp_player_intcelebrationmale@blow_kiss", "blow_kiss", null, AnimationFlags.StopOnLastFrame),
            new Animation("Bro", "", "anim@mp_player_intcelebrationmale@bro_love", "bro_love", null, AnimationFlags.StopOnLastFrame),
            new Animation("Chicken", "", "anim@mp_player_intcelebrationmale@chicken_taunt", "chicken_taunt", null, AnimationFlags.StopOnLastFrame),
            new Animation("Chin", "", "anim@mp_player_intcelebrationmale@chin_brush", "chin_brush", null, AnimationFlags.StopOnLastFrame),
            new Animation("Dj", "", "anim@mp_player_intcelebrationmale@dj", "dj", null, AnimationFlags.Loop),
            new Animation("Dock", "", "anim@mp_player_intcelebrationmale@dock", "dock", null, AnimationFlags.StopOnLastFrame),
            new Animation("Facepalm", "", "anim@mp_player_intcelebrationmale@face_palm", "face_palm", null, AnimationFlags.StopOnLastFrame),
            new Animation("Fingerkiss", "", "anim@mp_player_intcelebrationmale@finger_kiss", "finger_kiss", null, AnimationFlags.StopOnLastFrame),
            new Animation("Freakout", "", "anim@mp_player_intcelebrationmale@freakout", "freakout", null, AnimationFlags.StopOnLastFrame),
            new Animation("Jazzhands", "", "anim@mp_player_intcelebrationmale@jazz_hands", "jazz_hands", null, AnimationFlags.StopOnLastFrame),
            new Animation("Knuckle", "", "anim@mp_player_intcelebrationmale@knuckle_crunch", "knuckle_crunch", null, AnimationFlags.StopOnLastFrame),
            new Animation("Pick nose", "picknose,nose", "anim@mp_player_intcelebrationmale@nose_pick", "nose_pick", null, AnimationFlags.StopOnLastFrame),
            new Animation("No", "", "anim@mp_player_intcelebrationmale@no_way", "no_way", null, AnimationFlags.StopOnLastFrame),
            new Animation("Peace", "", "anim@mp_player_intcelebrationmale@peace", "peace", null, AnimationFlags.StopOnLastFrame),
            new Animation("Photo", "", "anim@mp_player_intcelebrationmale@photography", "photography", null, AnimationFlags.StopOnLastFrame),
            new Animation("Rock", "", "anim@mp_player_intcelebrationmale@rock", "rock", null, AnimationFlags.StopOnLastFrame),
            new Animation("Salute", "", "anim@mp_player_intcelebrationmale@salute", "salute", null, AnimationFlags.StopOnLastFrame),
            new Animation("Shush", "", "anim@mp_player_intcelebrationmale@shush", "shush", null, AnimationFlags.StopOnLastFrame),
            new Animation("Slow clap", "slowclap,clap", "anim@mp_player_intcelebrationmale@slow_clap", "slow_clap", null, AnimationFlags.Loop),
            new Animation("Surrender", "", "anim@mp_player_intcelebrationmale@surrender", "surrender", null, AnimationFlags.Loop),
            new Animation("Wank", "", "anim@mp_player_intcelebrationmale@wank", "wank", null, AnimationFlags.Loop),
            new Animation("Wave", "", "anim@mp_player_intcelebrationmale@wave", "wave", null, AnimationFlags.Loop),
            new Animation("Hands up", "handsup", "missminuteman_1ig_2", "handsup_base", null, AnimationFlags.Loop),
            new Animation("SMG aim", "smgaim", "weapons@submg@", "aim_med_static", null, AnimationFlags.Loop),
            new Animation("Thumbs", "", "anim@mp_player_intcelebrationmale@thumbs_up", "thumbs_up", null, AnimationFlags.StopOnLastFrame),
            new Animation("Taunt", "", "anim@mp_player_intcelebrationmale@thumb_on_ears", "thumb_on_ears", null, AnimationFlags.StopOnLastFrame),
            new Animation("Peace sign", "peacesign,vsign", "anim@mp_player_intcelebrationmale@v_sign", "v_sign", null, AnimationFlags.StopOnLastFrame),
            new Animation("Loco", "", "anim@mp_player_intcelebrationmale@you_loco", "you_loco", null, AnimationFlags.StopOnLastFrame),
            new Animation("Rifleaim", "", "weapons@rifle@hi@", "aim_med_loop", null, (AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody)),
            new Animation("Sit 1", "sit1", "amb@world_human_seat_steps@male@elbows_on_knees@base", "base", new Vector3(0,0,-0.5), AnimationFlags.Loop),
            new Animation("Sit 2", "sit2", "amb@world_human_seat_steps@female@hands_by_sides@base", "base", new Vector3(0,0,-0.5), AnimationFlags.Loop),
            new Animation("Sit 3", "sit3", "amb@lo_res_idles@", "generic_seating_lo_res_base", new Vector3(0,0,-0.5), AnimationFlags.Loop),
            new Animation("Laydown", "", "amb@lo_res_idles@", "lying_face_down_lo_res_base", new Vector3(0,0,-0.5), AnimationFlags.Loop),
            new Animation("Flashlight", "", "amb@world_human_security_shine_torch@male@idle_a", "idle_c", null, AnimationFlags.StopOnLastFrame),
            new Animation("Smokefem", "", "amb@world_human_smoking@male@male_a@base", "base", null, AnimationFlags.Loop),
            new Animation("Smokeman", "", "amb@world_human_smoking@female@base", "base", null, AnimationFlags.Loop),
            new Animation("Smokeweed 1", "smokeweed1", "amb@world_human_smoking_pot@female@base", "base", null, AnimationFlags.Loop),
            new Animation("Smokeweed 2", "smokeweed2", "amb@world_human_smoking_pot@male@base", "base", null, AnimationFlags.Loop),
            new Animation("Crosshands", "", "amb@world_human_stand_guard@male@base", "base", null, AnimationFlags.Loop),
            new Animation("Traffic 1", "traffic1", "missheist_agency2aig_1", "direct_traffic_a", null, AnimationFlags.StopOnLastFrame),
            new Animation("Traffic 2", "traffic2", "missheist_agency2aig_1", "direct_traffic_d", null, AnimationFlags.StopOnLastFrame),
            new Animation("Traffic 3", "traffic3", "missheist_agency2aig_1", "direct_traffic_b", null, AnimationFlags.StopOnLastFrame),
            new Animation("Traffic 4", "traffic4", "missheist_agency2aig_1", "direct_traffic_c", null, AnimationFlags.StopOnLastFrame),
            new Animation("Traffic 5", "traffic5", "missheist_agency2aig_1", "direct_traffic_e", null, AnimationFlags.StopOnLastFrame),
            new Animation("Pistolaim 1", "pistolaim1", "weapons@pistol@", "aim_med_loop", null, AnimationFlags.Loop),
            new Animation("Pistolaim 2", "pistolaim2", "weapons@pistol_1h@gang", "aim_med_loop", null, AnimationFlags.Loop),
            new Animation("Pistolaim 3", "pistolaim3", "weapons@pistol_1h@hillbilly", "aim_med_loop", null, AnimationFlags.Loop),
            new Animation("Pistolaim 4", "pistolaim4", "weapons@pistol_1h@", "aim_med_loop", null, AnimationFlags.Loop),
            new Animation("Trafficjam 1", "trafficjam1", "switch@michael@stuckintraffic", "stuckintraffic_aggitated", null, AnimationFlags.StopOnLastFrame),
            new Animation("Trafficjam 2", "trafficjam2", "switch@michael@stuckintraffic", "stuckintraffic_hitwheel", null, AnimationFlags.StopOnLastFrame),
            new Animation("Handcuff", "", "mp_arresting", "idle", null, (AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl)),
            new Animation("Crouch", "", "move_crouch_proto", "idle_intro", null, (AnimationFlags.Loop | AnimationFlags.Cancellable)),
            new Animation("Sit from left", "sitfromleft", "anim_casino_b@amb@casino@games@shared@player@", "sit_enter_left", null, (AnimationFlags.StopOnLastFrame)),
            new Animation("Sit 4", "sit4", "anim_casino_b@amb@casino@games@shared@player@", "idle_a", null, (AnimationFlags.Loop))
        };

        public static void StopAnimation(Player player)
        {
            // TODO: In the future, delete animation objects
            player.StopAnimation();
            player.Freeze(false);
        }

        public static bool StartAnimation(Player player, string animName)
        {
            if (player == null || String.IsNullOrEmpty(animName))
                return false;

            animName = animName.ToLower();
            Animation animToStart = AllAnimations.FirstOrDefault(x => x.AnimDisplayName.ToLower().Equals(animName) || x.AlternativeNames.ToLower().Split(",").Contains(animName.ToLower()));
            if(!animToStart.Equals(default(Animation)))
            {
                // If the anim has an offset, freeze him and set his position accordingly
                if (animToStart.Offset != null) { 
                    // Get the players position before he started the animation if it exists
                    Vector3 oldPos = player.HasData("AnimationOldPosition") ? player.GetData<Vector3>("AnimationOldPosition") : null;
                    if (oldPos != null)
                        player.Position = oldPos;
                    player.Position += animToStart.Offset;
                    player.Freeze(true);

                    // Save the players old position
                    oldPos = player.Position - animToStart.Offset;
                    player.SetData<Vector3>("AnimationOldPosition", oldPos);
                }

                player.PlayAnimation(animToStart.AnimDictionary, animToStart.AnimName, (int)animToStart.Flags);
                return true;
            }

            return false;
        }
    }
}
