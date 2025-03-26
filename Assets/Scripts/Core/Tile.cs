namespace Dev.Kosov.Factory.Core
{
    public class Tile
    {
        private static int nextID = 0;

        public Back BackType { get; internal set; }
        public int EntityID { get; internal set; }
        public Ore OreType { get; }
        public int OreAmount { get; }

        internal Tile(Back backType, Ore oreType, int oreAmount)
        {
            BackType = backType;
            OreType = oreType;
            OreAmount = oreAmount;
            EntityID = -1;
        }

        internal static int GenEntityID()
        {
            nextID++;
            return nextID - 1;
        }
    }
}
