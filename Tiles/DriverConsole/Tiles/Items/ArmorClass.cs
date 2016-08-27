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
        public IDamageVector ResistVector { get; private set; }
        public IReadOnlyList<ArmorSlot> RequiredSlots { get; private set; }

        public ArmorClass(IDamageVector damage, params ArmorSlot[] slots)
        {
            RequiredSlots = slots;
            ResistVector = damage;
        }

    }
}
