using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Agents.Combat
{
    public interface ICombatMoveClass
    {
        string Name { get; }
        IVerb Verb { get; }
        IDamageVector DamageVector { get; }

        BodyStateChange AttackerBodyStateChange { get; }

        bool IsDefenderPartSpecific { get; set; }

        bool IsMartialArts { get; set; }

        bool IsStrike { get; set; }

        bool IsItem { get; set; }
    }

}
