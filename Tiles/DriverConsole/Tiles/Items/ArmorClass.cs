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
        //Dictionary<DamageType, uint> BaseResist { get; set; }
        public DamageVector DamageVector { get; private set; }
        public string Name { get; private set; }
        public ISprite Sprite { get; private set; }

        // TODO - maintain interface and constructor api, but make this an IList
        public ArmorSlot ArmorSlot { get; private set; }

        public ArmorClass(string name, ISprite sprite, ArmorSlot slot, DamageVector damage)
        {
            Name = name;
            Sprite = sprite;
            ArmorSlot = slot;
            DamageVector = damage;
        }

    }
}
