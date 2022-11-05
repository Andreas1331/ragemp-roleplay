using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.Casino.Data
{
    public struct RouletteBet
    {
        public Player Better { get; }
        public int Amount { get; }
        public List<string> BettingFields { get; }
        public RouletteBets BetFlag { get; }

        public RouletteBet(Player better, int amount, List<string> bettingFields, RouletteBets betFlag)
        {
            this.Better = better;
            this.Amount = amount;
            this.BettingFields = bettingFields;
            this.BetFlag = betFlag;
        }
    }
}
