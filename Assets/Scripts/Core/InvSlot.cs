using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Kosov.Factory.Core
{
    class InvSlot
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
