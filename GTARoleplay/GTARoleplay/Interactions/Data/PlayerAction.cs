using GTANetworkAPI;
using System;

namespace GTARoleplay.Interactions.Data
{
    public class PlayerAction
    {
        public readonly Action<Player> action;
        public readonly ColShape shape;

        public PlayerAction(Action<Player> action, ColShape shape)
        {
            this.action = action;
            this.shape = shape;
        }
    }
}
