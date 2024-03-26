using GTANetworkAPI;
using GTARoleplay.Account.Data;
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

        public static EventsHandler Instance { get; private set; }

        public EventsHandler()
        {
            Instance = this;
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
    }
}
