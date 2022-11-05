namespace GTARoleplay.InventorySystem.Data
{
    // Is used whenever the player wish to view an inventory. The struct serves as a data container for items.
    public struct InventoryItem
    {
        public string Name { get; set; }
        public int Identifier { get; set; }
        public int Amount { get; set; }
        public float WeightPerItem { get; set; }

        public InventoryItem(string name, int identifier, int amount, float weightPerItem)
        {
            Name = name;
            Identifier = identifier;
            Amount = amount;
            WeightPerItem = weightPerItem;
        }
    }
}
