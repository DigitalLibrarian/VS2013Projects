using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public class ArmorClass : IArmorClass
    {
        public IReadOnlyList<ArmorSlot> RequiredSlots { get; private set; }

        public ArmorClass(params ArmorSlot[] slots)
        {
            RequiredSlots = slots;
        }

    }
}
