using GTANetworkAPI;
using GTARoleplay.Casino;
using GTARoleplay.Character.Customization;
using GTARoleplay.Events;
using GTARoleplay.Interactions.Data;
using GTARoleplay.InventorySystem;
using GTARoleplay.Library;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Vehicles;
using GTARoleplay.Vehicles.Data;
using GTARoleplay.Vehicles.Streaming;
using GTARoleplay.Wheel;
using GTARoleplay.Wheel.Containers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Interactions
{
    public class InteractionHandler 
    {
        public InteractionHandler()
        {
            EventsHandler.Instance.EKeyPressed += ShowWheelOnE;
            EventsHandler.Instance.CKeyPressed += ShowGenericWheel;
            EventsHandler.Instance.YKeyPressed += DoPlayerAction;
        }

        private void DoPlayerAction(Player player)
        {
            if (player == null)
                return;

            /* Check if the player has any player actions stored */
            if (player.HasData(PlayerActionHandler.PLAYER_ACTIONS))
            {
                var storage = player.GetData<PlayerActionStorage>(PlayerActionHandler.PLAYER_ACTIONS);
                storage?.InvokeChildren();
            }
        }

        private void ShowWheelOnE(Player player, int entityType, int entityValue)
        {
            switch (entityType)
            {
                case 2: // Vehicles
                    CreateVehicleInteractionWheel(player, entityValue);
                    break;
                case 3: // Objects
                    // figure out which object the player is interacting with
                    var obj = NAPI.Pools.GetAllObjects().FirstOrDefault(x => x.Value.Equals(entityValue));
                    if(obj != null)
                    {
                        if (obj.Model == 3021682919) // vw_prop_casino_roulette_01b
                            RouletteHandler.CreateRouletteInteractionWheel(player, obj);
                    }
                    break;
                default:
                    return;
            }
        }

        private void ShowGenericWheel(Player player)
        {
            var character = player.GetUserData()?.ActiveCharacter;
            if (character == null)
                return;

            var primaryTitle = $"{player.Name} [{PlayerHandler.GetIDFromPlayer(player)}]";
            var primaryWheel = new PrimaryInteractionWheel(0, primaryTitle, ConsoleKey.C);
            var vehicleWheel = new InteractionWheel(1, "Your vehicle(s)");
            var extraWheel = new InteractionWheel(2, "Walkingstyle");
            primaryWheel.AddSubWheel(vehicleWheel);
            primaryWheel.AddSubWheel(extraWheel);

            List<object> slices = new List<object>()
                {
                    new WheelSliceAction("icon.inventory", () => InventoryHandler.ShowInventory(player)),
                    new WheelSliceAction("Animations", () => {
                        player.SendChatMessage("Will show animations in the future..");
                        CustomizationHandler.ShowClothesWindow(player);
                    }),
                    new WheelSliceSubMenu("icon.car", vehicleWheel.ID),
                    new WheelSliceSubMenu("icon.list", extraWheel.ID),
                };
            primaryWheel.Slices = slices;

            // Fill the vehicle slices list based on the players available vehicles
            // TODO: Create a submenu for each vehicle, where there's options like: Spawn/Despawn
            // TODO: Don't let a player spawn a vehicle that is already spawned
            List<object> vehicleSlices = new List<object>();
            if (character.Vehicles != null)
            {
                int vehicleWheelID = 3;
                foreach (GTAVehicle veh in character.Vehicles)
                {
                    InteractionWheel vehWheel = new InteractionWheel(vehicleWheelID, "Vehicle");
                    primaryWheel.AddSubWheel(vehWheel);
                    var spawnSlice = new WheelSliceAction("icon.palm", () => veh.SpawnVehicle());
                    var despawnSlice = new WheelSliceAction("icon.cross", () => veh.DespawnVehicle());
                    var backSlice = new WheelSliceSubMenu("icon.arrowleft", vehicleWheel.ID);
                    vehWheel.Slices = new List<object>();
                    vehWheel.Slices.Add(spawnSlice);
                    vehWheel.Slices.Add(despawnSlice);

                    var slice = new WheelSliceSubMenu(veh.VehicleModel, vehicleWheelID);
                    vehicleSlices.Add(slice);
                    vehicleWheelID++;
                }
            }
            vehicleSlices.Add(new WheelSliceSubMenu("icon.arrowleft", primaryWheel.ID));
            vehicleWheel.Slices = vehicleSlices;

            List<object> extraSlices = new List<object>()
            {
                new WheelSliceAction("Normal", () => player.SetSharedData("walkingStyle", "Normal")),
                new WheelSliceAction("Brave", () => player.SetSharedData("walkingStyle", "Brave")),
                new WheelSliceAction("Confident", () => player.SetSharedData("walkingStyle", "Confident")),
                new WheelSliceAction("Drunk", () => player.SetSharedData("walkingStyle", "Drunk")),
                new WheelSliceAction("Fat", () => player.SetSharedData("walkingStyle", "Fat")),
                new WheelSliceAction("Gangster", () => player.SetSharedData("walkingStyle", "Gangster")),
                new WheelSliceAction("Hurry", () => player.SetSharedData("walkingStyle", "Hurry")),
                new WheelSliceAction("Injured", () => player.SetSharedData("walkingStyle", "Injured")),
                new WheelSliceAction("Intimidated", () => player.SetSharedData("walkingStyle", "Intimidated")),
                new WheelSliceAction("Quick", () => player.SetSharedData("walkingStyle", "Quick")),
                new WheelSliceAction("Sad", () => player.SetSharedData("walkingStyle", "Sad")),
                new WheelSliceAction("Tough", () => player.SetSharedData("walkingStyle", "Tough"))
            };
            extraSlices.Add(new WheelSliceSubMenu("icon.arrowleft", primaryWheel.ID));
            extraWheel.Slices = extraSlices;

            primaryWheel.Display(player);
        }

        private void CreateVehicleInteractionWheel(Player ply, int entityVal)
        {
            // Get a hold of the vehicle being interacted with
            Vehicle veh = NAPI.Pools.GetAllVehicles().FirstOrDefault(x => x.Value.Equals(entityVal));
            if (veh == null)
                return;

            PrimaryInteractionWheel wheel = new PrimaryInteractionWheel(0, "Vehicle", ConsoleKey.E);
            List<object> slices = new List<object>()
                {
                    new WheelSliceAction("icon.key", () => {
                        veh.Locked = !veh.Locked;
                        NAPI.Chat.SendChatMessageToAll("Changed lock status of vehicle..");
                    }),
                    new WheelSliceAction("icon.power", () => {
                        VehicleStreaming.SetEngineState(veh, !VehicleStreaming.GetEngineState(veh));
                        NAPI.Chat.SendChatMessageToAll("Changed engine status of vehicle..");
                    }),
                    new WheelSliceAction("Trunk", () =>
                    {
                        DoorState currentTrunk = VehicleStreaming.GetDoorState(veh, DoorID.DoorTrunk);
                        if(currentTrunk == DoorState.DoorOpen)
                            VehicleStreaming.SetDoorState(veh, DoorID.DoorTrunk, DoorState.DoorClosed);
                        else
                            VehicleStreaming.SetDoorState(veh, DoorID.DoorTrunk, DoorState.DoorOpen);
                        NAPI.Chat.SendChatMessageToAll("Changed trunk status of vehicle..");
                    }),
                    new WheelSliceAction("Hood", () =>
                    {
                        DoorState currentHood = VehicleStreaming.GetDoorState(veh, DoorID.DoorHood);
                        if(currentHood == DoorState.DoorOpen)
                            VehicleStreaming.SetDoorState(veh, DoorID.DoorHood, DoorState.DoorClosed);
                        else
                            VehicleStreaming.SetDoorState(veh, DoorID.DoorHood, DoorState.DoorOpen);
                        NAPI.Chat.SendChatMessageToAll("Changed hood status of vehicle..");
                    }),
                    new WheelSliceAction("icon.wrench", () =>
                    {
                        VehicleModHandler.ShowModWindow(ply, veh);
                        NAPI.Chat.SendChatMessageToAll("Opening mod browser for vehicle..");
                    })
                };
            wheel.Slices = slices;
            wheel.Display(ply);
        }
    }
}
