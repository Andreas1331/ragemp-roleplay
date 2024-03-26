using GTANetworkAPI;

namespace GTARoleplay.Events
{
    internal class EventsListener : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            EventsHandler.Instance.ResourceStarted();
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            EventsHandler.Instance.PlayerConnected(player);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            EventsHandler.Instance.PlayerDisconnected(player, type, reason);
        }

        [ServerEvent(Event.ResourceStartEx)]
        public void OnResourceStartEx(string resourceName)
        {
            EventsHandler.Instance.ResourceStartedEx(resourceName);
        }

        [ServerEvent(Event.VehicleDeath)]
        public void OnVehicleDeath(Vehicle vehicle)
        {
            EventsHandler.Instance.VehicleDeath(vehicle);
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seat)
        {
            EventsHandler.Instance.PlayerEnteredVehicle(player, vehicle, seat);
        }
    }
}
