using GTANetworkAPI;
using GTARoleplay.Account.Data;
using GTARoleplay.Library.Extensions;
using System;

namespace GTARoleplay.Events
{
    public class EventsHandler
    {
        public event Action OnResourceStart;
        public event Action<string> OnResourceStartEx;
        public event Action<Player> OnPlayerConnected;
        public event Action<Player, DisconnectionType, string> OnPlayerDisconnected;
        public event Action<Vehicle> OnVehicleDeath;
        public event Action<Player, Vehicle, sbyte> OnPlayerEnteredVehicle;

        public event Action<Player, User> OnUserLoggedIn;

        public delegate void KeyPressed(Player player);
        public delegate void InteractionKeyPressed(Player player, int entityType, int entityHandle);

        public event KeyPressed CKeyPressed;
        public event KeyPressed YKeyPressed;
        public event KeyPressed LKeyPressed;
        public event InteractionKeyPressed EKeyPressed;

        public static EventsHandler Instance { get; private set; }

        public EventsHandler()
        {
            Instance = this;

            NAPI.ClientEvent.Register<Player>("CKeyPressed::Server", this, OnCKeyPressed);
            NAPI.ClientEvent.Register<Player>("YKeyPressed::Server", this, OnYKeyPressed);
            NAPI.ClientEvent.Register<Player>("LKeyPressed::Server", this, OnLKeyPressed);
            NAPI.ClientEvent.Register<Player, int, int>("EKeyPressed::Server", this, OnEKeyPressed);
        }

        public void ResourceStarted()
        {
            OnResourceStart?.Invoke();
        }

        public void ResourceStartedEx(string resourceName)
        {
            OnResourceStartEx?.Invoke(resourceName);
        }

        public void PlayerConnected(Player player)
        {
            OnPlayerConnected?.Invoke(player);
        }

        public void PlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            OnPlayerDisconnected?.Invoke(player, type, reason);
        }

        public void VehicleDeath(Vehicle vehicle)
        {
            OnVehicleDeath?.Invoke(vehicle);
        }
        public void PlayerEnteredVehicle(Player player, Vehicle vehicle, sbyte seat)
        {
            OnPlayerEnteredVehicle?.Invoke(player, vehicle, seat);
        }

        public void UserLoggedIn(Player player, User user)
        {
            OnUserLoggedIn?.Invoke(player, user);
        }

        private void OnCKeyPressed(Player player)
        {
            if (player == null)
                return;
            CKeyPressed?.Invoke(player);
        }

        private void OnYKeyPressed(Player player)
        {
            if (player == null)
                return;
            YKeyPressed?.Invoke(player);
        }

        private void OnLKeyPressed(Player player)
        {
            if (player == null)
                return;
            LKeyPressed?.Invoke(player);
        }

        private void OnEKeyPressed(Player player, int entityType, int entityHandle)
        {
            if (player == null)
                return;
            EKeyPressed?.Invoke(player, entityType, entityHandle);
        }
    }
}
