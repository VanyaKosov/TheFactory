namespace Dev.Kosov.Factory.Core
{
    internal class Tile
    {
        private static int nextID = 0;

        internal Back BackType { get; set; }
        internal int EntityID { get; set; }
        internal Ore OreType { get; }
        internal int OreAmount { get; }

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
