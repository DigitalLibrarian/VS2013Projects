using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfCombatMoveFactory
    {
        CombatMove Create(DfObject attackDf);
    }

    public class DfCombatMoveFactory : IDfCombatMoveFactory
    {
        public CombatMove Create(DfObject attackDf)
        {
            var attackTag = attackDf.Tags.First();
            CombatMove move = null;
            

            switch (attackTag.GetParam(0))
            {
                case "EDGE":
                    move = CreateWeaponAttack(attackTag);
                    break;
                case "BLUNT":
                    move = CreateWeaponAttack(attackTag);
                    break;
                default:
                    move = CreateBodyAttack(attackTag);
                    break;
            }

            foreach (var subTag in attackDf.Tags)
            {
                switch (subTag.Name)
                {
                    case DfTags.MiscTags.ATTACK_PREPARE_AND_RECOVER:
                        move.PrepTime = int.Parse(subTag.GetParam(0));
                        move.RecoveryTime = int.Parse(subTag.GetParam(1));
                        break;
                }
            }
            return move;
        }

        CombatMove CreateWeaponAttack(DfTag attackTag)
        {
            var name = attackTag.GetParam(3);
            return new CombatMove
            {
                Name = name,
                Verb = new Verb
                {
                    SecondPerson = name,
                    ThirdPerson = attackTag.GetParam(4),
                    IsTransitive = false
                },
                ContactArea = int.Parse(attackTag.GetParam(1)),
                MaxPenetration = int.Parse(attackTag.GetParam(2)),
                VelocityMultiplier = int.Parse(attackTag.GetParam(6))
            };
        }

        CombatMove CreateBodyAttack(DfTag attackTag)
        {
            return new CombatMove
            {
                Name = attackTag.GetParam(0),
                Verb = new Verb
                {
                    SecondPerson = attackTag.GetParam(0),
                    ThirdPerson = attackTag.GetParam(0),
                    IsTransitive = false
                },
                ContactArea = 0,
                MaxPenetration = 0,
                VelocityMultiplier = 0
            };
        }
    }
}
