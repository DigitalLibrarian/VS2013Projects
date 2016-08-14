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
    public class CombatMoveDiscoverer : ICombatMoveDiscoverer
    {
        public ICombatMoveBuilder MoveBuilder { get; private set; }
        public CombatMoveDiscoverer(ICombatMoveBuilder moveBuilder)
        {
            MoveBuilder = moveBuilder;
        }
        bool IsMeleeRange(IAgent attacker, IAgent defender)
        {
            var diffVector = attacker.Pos - defender.Pos;
            return CompassVectors.IsCompassVector(diffVector);
        }

        public IEnumerable<ICombatMove> GetPossibleMoves(IAgent attacker, IAgent defender)
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

                    if (attacker.Body.IsWrestling)
                    {
                        foreach (var move in WrestlingMoves(attacker, defender, mePart))
                            yield return move;
                    }
                }

                // TODO - armor, weapon, racial, magic, tech, etc.. other types of abilities
            }
        }



        IEnumerable<ICombatMove> WeaponMoves(IAgent attacker, IAgent defender, IItem weaponItem)
        {
            foreach (var attackMoveClass in weaponItem.WeaponClass.AttackMoveClasses)
            {
                foreach (var youPart in defender.Body.Parts)
                {
                    yield return MoveBuilder.AttackBodyPartWithWeapon(attacker, defender, attackMoveClass, youPart, weaponItem);
                }
            }
        }


        IEnumerable<ICombatMove> GraspMoves(IAgent attacker, IAgent defender, IBodyPart mePart)
        {
            foreach (var youPart in defender.Body.Parts)
            {
                if (!youPart.IsWrestling)
                {
                    yield return MoveBuilder.GraspOpponentBodyPart(attacker, defender, mePart, youPart);
                }
            }
        }

        IEnumerable<ICombatMove> WrestlingMoves(IAgent attacker, IAgent defender, IBodyPart mePart)
        {
            if (mePart.IsGrasping && defender.Body.Parts.Contains(mePart.Grasped))
            {
                yield return MoveBuilder.PullGraspedBodyPart(attacker, defender, mePart, mePart.Grasped);
                yield return MoveBuilder.ReleaseGraspedPart(attacker, defender, mePart, mePart.Grasped);
                // TODO - more mixable verbs
                // twist, if can
                // bend, if can

            }

            if (defender.Body.IsWrestling)
            {
                foreach (var youPart in defender.Body.Parts)
                {
                    if (youPart.IsGrasping && youPart.Grasped == mePart)
                    {
                        yield return MoveBuilder.BreakOpponentGrasp(attacker, defender, mePart, youPart);
                    }
                }
            }

        }
    }
}
