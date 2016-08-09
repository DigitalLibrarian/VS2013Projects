using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public class WeaponClass : IWeaponClass
    {
        public DamageVector DamageVector { get; set; }

        public string Name { get; private set; }
        public ISprite Sprite { get; private set; }
        public string MeleeVerb { get; private set; }

        public WeaponClass(string name, ISprite sprite, DamageVector damage, string meleeVerb = "strikes", params WeaponSlot[] slots)
        {
            Name = name;
            Sprite = sprite;
            DamageVector = damage;
            MeleeVerb = meleeVerb;
            RequiredSlots = slots;
        }

        public IEnumerable<WeaponSlot> RequiredSlots
        {
            get;
            private set;
        }
    }
}
