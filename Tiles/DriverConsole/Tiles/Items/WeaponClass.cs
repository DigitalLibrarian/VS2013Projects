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
        public string Name { get; private set; }
        public ISprite Sprite { get; private set; }
        public IList<ICombatMoveClass> AttackMoveClasses { get; private set; }

        public WeaponClass(
            string name, ISprite sprite, 
            WeaponSlot[] slots,
            ICombatMoveClass[] attackMoveClasses
            )
        {
            Name = name;
            Sprite = sprite;
            RequiredSlots = slots;
            AttackMoveClasses = new List<ICombatMoveClass>(attackMoveClasses);
        }

        public IEnumerable<WeaponSlot> RequiredSlots
        {
            get;
            private set;
        }
    }
}
