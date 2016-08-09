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
        WeaponSlot WeaponSlot { get; }
        ISprite Sprite { get; }

        // TODO - a single weapon class should be capable of many possible attack moves.  this is a just a stop gap
        DamageVector DamageVector { get; }
        string MeleeVerb { get; }
    }
}
