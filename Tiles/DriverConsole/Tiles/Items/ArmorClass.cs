using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public class ArmorClass : IArmorClass
    {
        public DamageVector DamageVector { get; private set; }
        public string Name { get; private set; }
        public ISprite Sprite { get; private set; }
        public IReadOnlyList<ArmorSlot> RequiredSlots { get; private set; }

        public ArmorClass(string name, ISprite sprite, DamageVector damage, params ArmorSlot[] slots)
        {
            Name = name;
            Sprite = sprite;
            RequiredSlots = slots;
            DamageVector = damage;
        }

    }
}
