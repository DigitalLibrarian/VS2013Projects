using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public interface IWeaponClass
    {
        string Name { get; }
        ISprite Sprite { get; }
        
        IEnumerable<WeaponSlot> RequiredSlots { get; }

        IList<IAttackMoveClass> AttackMoveClasses { get; }
    }
}
