namespace Dev.Kosov.Factory.Core
{
    public struct InvSlot
    {
        public ItemType Type { get; internal set; }
        public int Amount { get; internal set; }

        internal InvSlot(ItemType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
