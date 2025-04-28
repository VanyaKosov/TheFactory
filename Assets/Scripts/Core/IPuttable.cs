using System.Collections.Generic;

namespace Dev.Kosov.Factory.Core
{
    internal interface IPuttable
    {
        internal int Put(InvSlot item); // Returns remainder

        internal List<InvSlot> GetWantedItems();
    }
}
