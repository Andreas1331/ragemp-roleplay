using GTANetworkAPI;
using GTARoleplay.Interactions.Data;
using System;

namespace GTARoleplay.Interactions
{
    public static class PlayerActionHandler
    {
        public static readonly string PLAYER_ACTIONS = "PlayerActions";

        public static void SetupNewAction(Action<Player> action, ColShape shape)
        {
            shape.OnEntityEnterColShape += (shape, ply) =>
            {
                PlayerAction plyAction = new PlayerAction(action, shape);
                AddNewAction(ply, plyAction);
            };
        }

        private static void AddNewAction(Player player, PlayerAction action)
        {
            if (player.HasData(PLAYER_ACTIONS))
            {
                // Get the current storage of actions
                PlayerActionStorage storage = player.GetData<PlayerActionStorage>(PLAYER_ACTIONS);
                if (storage == null)
                    storage = new PlayerActionStorage(player);
                storage.AddNewAction(action);
            }
            else
            {
                PlayerActionStorage storage = new PlayerActionStorage(player);
                storage.AddNewAction(action);
                player.SetData<PlayerActionStorage>(PLAYER_ACTIONS, storage);
            }
        }
    }
}
