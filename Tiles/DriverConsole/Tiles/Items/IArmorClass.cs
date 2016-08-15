using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public interface IArmorClass
    {
        string Name { get; }
        ISprite Sprite { get; }

        IReadOnlyList<ArmorSlot> RequiredSlots { get;}
        DamageVector ResistVector { get; }
    }
}
