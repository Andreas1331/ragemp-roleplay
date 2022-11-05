using System.Collections.Generic;

namespace GTARoleplay.Character.Customization
{
    public struct OutfitContainer
    {
        // The key is the component, and the value contains the drawable and texture
        public Dictionary<int, OutfitObject> ClothesAndAccessories;
    }

    public struct OutfitObject
    {
        public int Drawable;
        public int Texture;
    }
}
