using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents.Combat
{
    public class AttackMoveDiscoverer : IAttackMoveDiscoverer
    {
        public IAttackMoveBuilder MoveBuilder { get; private set; }
        public AttackMoveDiscoverer(IAttackMoveBuilder moveBuilder)
        {
            MoveBuilder = moveBuilder;
        }
        bool IsMeleeRange(IAgent attacker, IAgent defender)
        {
            var diffVector = attacker.Pos - defender.Pos;
            return CompassVectors.IsCompassVector(diffVector);
        }

        public IEnumerable<IAttackMove> GetPossibleMoves(IAgent attacker, IAgent defender)
        {
            bool meleeRange = IsMeleeRange(attacker, defender);
            foreach (var mePart in attacker.Body.Parts)
            {
                if (meleeRange)
                {
                    var weaponItem = attacker.Outfit.GetWeaponItem(mePart);
                    if (weaponItem != null)
                    {
                        foreach (var move in WeaponMoves(attacker, defender, weaponItem))
                            yield return move;
                    }

                    if (mePart.CanGrasp)
                    {
                        foreach (var move in GraspMoves(attacker, defender, mePart))
                            yield return move;
                    }

                    if (attacker.Body.IsGrasping)
                    {
                        foreach (var move in WrestlingMoves(attacker, defender, mePart))
                            yield return move;
                    }

                }

                // TODO - armor, weapon, racial, magic, tech, etc.. other types of abilities
            }
        }



        IEnumerable<IAttackMove> WeaponMoves(IAgent attacker, IAgent defender, IItem weaponItem)
        {
            foreach (var attackMoveClass in weaponItem.WeaponClass.AttackMoveClasses)
            {
                foreach (var youPart in defender.Body.Parts)
                {
                    yield return MoveBuilder.AttackBodyPartWithWeapon(attacker, defender, attackMoveClass, youPart, weaponItem);
                }
            }
        }


        IEnumerable<IAttackMove> GraspMoves(IAgent attacker, IAgent defender, IBodyPart mePart)
        {
            foreach (var youPart in defender.Body.Parts)
            {
                yield return MoveBuilder.GraspOpponent(attacker, defender, mePart, youPart);
            }
        }

        IEnumerable<IAttackMove> WrestlingMoves(IAgent attacker, IAgent defender, IBodyPart mePart)
        {
            if (mePart.IsGrasping)
            {
                var youPart = mePart.Grasped;
                if (!youPart.IsGrasping)
                {
                    yield return MoveBuilder.WrestlingPull(attacker, defender, mePart, youPart);
                }
            }

        }
    }
}
