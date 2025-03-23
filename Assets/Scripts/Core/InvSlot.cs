namespace Dev.Kosov.Factory.Core
{
    public class InvSlot
    {
        internal ItemType Type { get; set; }
        internal int Amount { get; set; }

        internal InvSlot(ItemType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
