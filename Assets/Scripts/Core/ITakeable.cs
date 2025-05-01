namespace Dev.Kosov.Factory.Core
{
    internal interface ITakeable
    {
        internal InvSlot Take();

        internal InvSlot Take(ItemType type); // Take specific item
    }
}
