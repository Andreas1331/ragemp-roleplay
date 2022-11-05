using GTANetworkAPI;

namespace GTARoleplay.Events
{
    public class PlayerEvents : Script
    {
        //TODO: Make the delegate a func instead, so each method returns if they succeeded or not. If they didn't,
        // we'll continue the invoke of the next method in line. Also make each subscriber tell if it blocks functions.
        public delegate void KeyPressed(Player player);
        public delegate void InteractionKeyPressed(Player player, int entityType, int entityHandle);

        public static event KeyPressed CKeyPressed;
        public static event InteractionKeyPressed EKeyPressed;
        public static event KeyPressed YKeyPressed;
        public static event KeyPressed LKeyPressed;

        [RemoteEvent("CKeyPressed::Server")]
        public void OnCKeyPressed(Player player)
        {
            if (player == null)
                return;
            CKeyPressed?.Invoke(player);
        }

        [RemoteEvent("EKeyPressed::Server")]
        public void OnEKeyPressed(Player player, int entityType, int entityHandle)
        {
            if (player == null)
                return;
            EKeyPressed?.Invoke(player, entityType, entityHandle);
        }

        [RemoteEvent("YKeyPressed::Server")]
        public void OnYKeyPressed(Player player)
        {
            if (player == null)
                return;
            YKeyPressed?.Invoke(player);
        }

        [RemoteEvent("LKeyPressed::Server")]
        public void OnLKeyPressed(Player player)
        {
            if (player == null)
                return;
            LKeyPressed?.Invoke(player);
        }
    }
}
