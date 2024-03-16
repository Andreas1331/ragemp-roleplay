using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GTARoleplay.Wheel.Containers
{
    public class PrimaryInteractionWheel : InteractionWheel
    {
        [JsonIgnore]
        public List<InteractionWheel> SubWheels { get; } = new List<InteractionWheel>();

        [JsonIgnore]
        public ConsoleKey KeyToBind { get; }

        public PrimaryInteractionWheel(int id, string title, ConsoleKey keyToBind) : base(id, title)
        {
            KeyToBind = keyToBind;
        }

        public void AddSubWheel(InteractionWheel wheel) 
        {
            // The wheel can not be a subwheel of itself!
            if (wheel == this)
                return;

            SubWheels.Add(wheel);
        }

        public void Display(Player player)
        {
            // We make sure the primary wheel is the first element of the list
            var wheelsForDisplay = new List<object>() {
                this,
            };
            wheelsForDisplay.AddRange(this.SubWheels);
            var wheels = JsonSerializer.Serialize<object>(wheelsForDisplay);
            player.TriggerEvent("ShowInteractionWheel::Client", wheels, KeyToBind);
            player.SetData("WheelData", this);
        }

        public void InvokeSliceAction(int wheelID, int sliceIndex)
        {
            // The action is on a submenu
            if (wheelID > 0)
            {
                // Get a reference to the submenu
                InteractionWheel wheel = SubWheels.FirstOrDefault(x => x.ID.Equals(wheelID));
                if(wheel != null)
                {
                    // Get a reference to the slice
                    object slice = wheel.Slices.FirstOrDefault(x => wheel.Slices.IndexOf(x).Equals(sliceIndex));
                    if (slice is WheelSliceAction sliceAction)
                        sliceAction.PlayerClickedAction?.Invoke();
                }
            }
            else
            {
                // Get a reference to the slice
                object slice = Slices.FirstOrDefault(x => Slices.IndexOf(x).Equals(sliceIndex));
                if (slice is WheelSliceAction sliceAction)
                    sliceAction.PlayerClickedAction?.Invoke();
            }
        }
    }
}
