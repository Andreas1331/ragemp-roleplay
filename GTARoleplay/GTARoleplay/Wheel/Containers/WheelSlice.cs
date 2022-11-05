namespace GTARoleplay.Wheel.Containers
{
    public abstract class WheelSlice
    {
        public string Text { get; }
        public string Description { get; }

        public WheelSlice(string text)
        {
            Text = text;
        }
    }
}
