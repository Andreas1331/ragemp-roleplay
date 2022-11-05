using System;

namespace GTARoleplay.Wheel.Containers
{
    public class WheelSliceAction : WheelSlice
    {
        public bool WillCloseMenu { get; private set; } 
        public readonly Action PlayerClickedAction;

        public WheelSliceAction(string text, Action playerClickedAction, bool willCloseMenu = true) : base(text)
        {
            WillCloseMenu = willCloseMenu;
            PlayerClickedAction = playerClickedAction;
        }
    }
}
