namespace GTARoleplay.Wheel.Containers
{
    public class WheelSliceSubMenu : WheelSlice
    {
        public int LinkedMenuID { get; }
        public bool IsSubMenuBtn { get; private set; } = false;

        public WheelSliceSubMenu(string text, int linkedMenuID) : base(text)
        {
            LinkedMenuID = linkedMenuID;
            IsSubMenuBtn = true;
        }
    }
}
