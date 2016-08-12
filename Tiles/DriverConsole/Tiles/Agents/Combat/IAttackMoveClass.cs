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
        bool IsReleasePart { get; }
        bool IsGraspBreak { get; }
        bool TakeDamageProducts { get; }

        IVerb Verb { get; }
        DamageVector DamageVector { get; }
    }
}
