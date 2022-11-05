using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTARoleplay.Interactions.Data
{
    public class PlayerActionStorage
    {
        private Player player;
        private readonly List<PlayerAction> actions = new List<PlayerAction>();

        public PlayerActionStorage(Player player)
        {
            this.player = player;
        }

        public void AddNewAction(PlayerAction newAction)
        {
            if(actions != null)
            {
                actions.Add(newAction);
                // Setup event to destroy the action if the player leaves the colshape
                newAction.shape.OnEntityExitColShape += (shape, ply) =>
                {
                    RemoveAction(newAction);
                };
            }
        }

        public void InvokeChildren()
        {
            actions.ForEach(x =>
            {
                x?.action?.Invoke(player);
            });
        }

        public void RemoveAction(PlayerAction playerAction)
        {
            if (actions.Contains(playerAction))
                actions.Remove(playerAction);
        }
    }
}
