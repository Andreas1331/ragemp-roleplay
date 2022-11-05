using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.Animations.Data
{
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    public struct Animation
    {
        public string AnimDisplayName { get; }
        // comma separated names
        public string AlternativeNames { get; }
        public string AnimDictionary { get; }
        public string AnimName { get; }
        public Vector3 Offset { get; }
        public AnimationFlags Flags { get; }

        public Animation(string animDisplayName, string alternativeNames, string animDict, string animName, Vector3 offset, AnimationFlags flags)
        {
            AnimDisplayName = animDisplayName;
            AlternativeNames = alternativeNames;
            AnimDictionary = animDict;
            AnimName = animName;
            Offset = offset;
            Flags = flags;
        }
    }
}
