﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Math;

namespace Tiles.Agents.Combat
{
    public class AttackMoveFactory : IAttackMoveFactory
    {
        public IAttackMoveBuilder MoveBuilder { get; private set; }
        public AttackMoveFactory(IAttackMoveBuilder moveBuilder)
        {
            MoveBuilder = moveBuilder;
        }

        public IEnumerable<IAttackMove> GetPossibleMoves(IAgent attacker, IAgent defender)
        {
            var diffVector = attacker.Pos - defender.Pos;
            foreach (var meBP in attacker.Body.Parts)
            {
                if (meBP.Weapon != null)
                {
                    foreach (var youBP in defender.Body.Parts)
                    {
                        if (CompassVectors.IsCompassVector(diffVector))
                        {
                            yield return MoveBuilder.WeaponStrike(attacker, defender, youBP, meBP.Weapon);
                        }
                    }
                }
                // TODO - armor, weapon, racial, magic, tech, etc.. other types of abilities
            }
        }
    }
}
