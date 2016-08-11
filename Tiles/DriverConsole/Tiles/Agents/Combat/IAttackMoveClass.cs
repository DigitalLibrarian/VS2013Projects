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
        bool IsMeleeStrike { get; }
        bool IsGraspPart { get; }
        IVerb Verb { get; }
        DamageVector DamageVector { get; }


        bool TakeDamageProducts { get; }
    }
}
