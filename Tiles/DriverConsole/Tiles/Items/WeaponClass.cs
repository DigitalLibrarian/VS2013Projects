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
        public IList<ICombatMoveClass> AttackMoveClasses { get; private set; }

        public WeaponClass(
            WeaponSlot[] slots,
            ICombatMoveClass[] attackMoveClasses
            )
        {
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
