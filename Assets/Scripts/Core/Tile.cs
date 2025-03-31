namespace Dev.Kosov.Factory.Core
{
    public class Tile
    {
        private static int nextID = 0;

        public BackType BackType { get; internal set; }
        public int EntityID { get; internal set; }
        public int OreAmount { get; internal set; }
        public OreType OreType { get; internal set; }

        internal Tile(BackType backType, OreType oreType, int oreAmount)
        {
            BackType = backType;
            OreType = oreType;
            OreAmount = oreAmount;
            EntityID = -1;
        }

        internal Tile(BackType backType, OreType oreType, int oreAmount, int entityID)
        {
            BackType = backType;
            OreType = oreType;
            OreAmount = oreAmount;
            EntityID = entityID;
        }

        internal static int GenEntityID()
        {
            nextID++;
            return nextID - 1;
        }
    }
}
