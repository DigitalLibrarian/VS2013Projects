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
        public IEnumerable<ICombatMoveClass> AttackMoveClasses { get; private set; }
        public int MinimumSize { get; private set; }

        public WeaponClass(
            WeaponSlot[] slots,
            ICombatMoveClass[] attackMoveClasses,
            int minimumSize)
        {
            RequiredSlots = slots;
            AttackMoveClasses = new List<ICombatMoveClass>(attackMoveClasses);
            MinimumSize = minimumSize;
        }

        public IEnumerable<WeaponSlot> RequiredSlots
        {
            get;
            private set;
        }
    }
}
