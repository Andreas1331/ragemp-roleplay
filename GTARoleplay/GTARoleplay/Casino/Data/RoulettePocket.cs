using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.Casino.Data
{
    public struct RoulettePocket
    {
        public int Index { get; }
        public string DisplayNumber { get; }
        public RouletteBets WinningFlags { get; }

        public static Dictionary<RouletteBets, int> PayoutRatios = new Dictionary<RouletteBets, int>()
        {
            { RouletteBets.StraightUp, 35},
            { RouletteBets.Split, 17},
            { RouletteBets.Basket, 6},
            { RouletteBets.Street, 11},
            { RouletteBets.Corner, 8},
            { RouletteBets.SixLine, 5},
            { RouletteBets.FirstColumn, 2},
            { RouletteBets.SecondColumn, 2},
            { RouletteBets.ThirdColumn, 2},
            { RouletteBets.FirstDozen, 2},
            { RouletteBets.SecondDozen, 2},
            { RouletteBets.ThirdDozen, 2},
            { RouletteBets.Odd, 1},
            { RouletteBets.Even, 1},
            { RouletteBets.Red, 1},
            { RouletteBets.Black, 1},
            { RouletteBets.OneToEigthteen, 1},
            { RouletteBets.NineteenToThirtySix, 1}
        };

        public RoulettePocket(int index, string displayNumber, RouletteBets winningFlags)
        {
            this.Index = index;
            this.DisplayNumber = displayNumber;
            this.WinningFlags = (winningFlags | RouletteBets.StraightUp);
        }
    }
}
