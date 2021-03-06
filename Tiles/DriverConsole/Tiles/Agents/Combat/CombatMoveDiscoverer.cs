﻿using System;
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
        public ICombatMoveFactory MoveFactory { get; private set; }
        public CombatMoveDiscoverer(ICombatMoveFactory moveFactory)
        {
            MoveFactory = moveFactory;
        }
        bool IsMeleeRange(IAgent attacker, IAgent defender)
        {
            return CompassVectors.IsCompassVector(attacker.Pos - defender.Pos);
        }

        public IEnumerable<ICombatMove> GetPossibleMoves(IAgent attacker, IAgent defender)
        {
            // TODO - a grabbed body part cannot be used to attack you
            if (IsMeleeRange(attacker, defender))
            {
                foreach (var mePart in attacker.Body.Parts)
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

                    if (mePart.IsWrestling)
                    {
                        foreach (var move in WrestlingMoves(attacker, defender, mePart))
                            yield return move;
                    }


                // TODO - armor, weapon, racial, magic, tech, etc.. other types of abilities
                }

                foreach (var move in BodyMoves(attacker, defender))
                    yield return move;
            }
        }

        private IEnumerable<ICombatMove> BodyMoves(IAgent attacker, IAgent defender)
        {
            foreach (var moveClass in attacker.Body.Moves)
            {
                if (moveClass.MeetsRequirements(attacker.Body))
                {
                    foreach (var youPart in defender.Body.Parts.Where(AcceptableTargetPart))
                    {
                        if(!youPart.IsWrestling)
                            yield return MoveFactory.BodyMove(attacker, defender, moveClass, youPart);
                    }
                }
            }
        }

        bool AcceptableTargetPart(IBodyPart part)
        {
            return !part.IsInternal && !part.Class.IsSmall;
        }


        IEnumerable<ICombatMove> WeaponMoves(IAgent attacker, IAgent defender, IItem weaponItem)
        {
            foreach (var attackMoveClass in weaponItem.Class.WeaponClass.AttackMoveClasses)
            {
                foreach (var youPart in defender.Body.Parts.Where(AcceptableTargetPart))
                {
                    yield return MoveFactory.AttackBodyPartWithWeapon(attacker, defender, attackMoveClass, youPart, weaponItem);
                }
            }
        }


        IEnumerable<ICombatMove> GraspMoves(IAgent attacker, IAgent defender, IBodyPart mePart)
        {
            foreach (var youPart in defender.Body.Parts.Where(AcceptableTargetPart))
            {
                if (!youPart.IsWrestling)
                {
                    yield return MoveFactory.GraspOpponentBodyPart(attacker, defender, mePart, youPart);
                }
            }
        }

        IEnumerable<ICombatMove> WrestlingMoves(IAgent attacker, IAgent defender, IBodyPart mePart)
        {
            if (mePart.IsGrasping && defender.Body.Parts.Contains(mePart.Grasped))
            {
                yield return MoveFactory.PullGraspedBodyPart(attacker, defender, mePart, mePart.Grasped);
                yield return MoveFactory.ReleaseGraspedPart(attacker, defender, mePart, mePart.Grasped);
            }

            if (defender.Body.IsWrestling)
            {
                foreach (var youPart in defender.Body.Parts)
                {
                    if (youPart.IsGrasping && youPart.Grasped == mePart)
                    {
                        yield return MoveFactory.BreakOpponentGrasp(attacker, defender, mePart, youPart);
                    }
                }
            }

        }
    }
}
