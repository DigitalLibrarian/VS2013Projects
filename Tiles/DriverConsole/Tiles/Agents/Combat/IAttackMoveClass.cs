using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public interface IAttackMoveClass
    {
        string Name { get; }
        IVerb MeleeVerb { get; }
        DamageVector DamageVector { get; }
    }
}
