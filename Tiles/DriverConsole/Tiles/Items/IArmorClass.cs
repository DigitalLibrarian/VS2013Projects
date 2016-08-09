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

        // TODO - Add the ability to cover multiple armor slots, but still only take up one
        // OR make armors take up multiple slots all together

        IReadOnlyList<ArmorSlot> RequiredSlots { get;}
        ISprite Sprite { get; }
        DamageVector DamageVector { get; }
    }
}
