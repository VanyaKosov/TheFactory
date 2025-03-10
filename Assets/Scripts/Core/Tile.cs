namespace Assets.Scripts.Core
{
    class Tile
    {
        private static int nextID = 0;

        public Back BackType { get; set; }
        public int FeatureID { get; set; }
        public Ore OreType { get; }
        public int OreAmount { get; }

        public Tile(Back backType, Ore oreType, int oreAmount)
        {
            BackType = backType;
            OreType = oreType;
            OreAmount = oreAmount;
            FeatureID = -1;
        }

        public static int genEntityID()
        {
            nextID++;
            return nextID - 1;
        }
    }
}
