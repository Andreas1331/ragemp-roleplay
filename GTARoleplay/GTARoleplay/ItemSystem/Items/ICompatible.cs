namespace GTARoleplay.ItemSystem.Items
{
    public interface ICompatible
    {
        // Check if two items are compatible with each other, meaning they can stack etc.
        public bool IsCompatible(Item itm);
    }
}
