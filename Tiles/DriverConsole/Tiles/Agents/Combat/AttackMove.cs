using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Agents.Combat
{
    public class AttackMove : IAttackMove
    {
        public AttackMove(IAttackMoveClass attackMoveClass, string name, IAgent attacker, IAgent defender, IBodyPart targetBodyPart, uint damage)
        {
            AttackMoveClass = attackMoveClass;
            Name = name;
            Attacker = attacker;
            Defender = defender;
            TargetBodyPart = targetBodyPart;
            CalculatedDamage = damage;
        }


        public IAttackMoveClass AttackMoveClass { get; private set; }
        public string Name { get; set; }
        public string Verb { get; set; }
        public bool IsCritical { get; set; }
        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
        public IBodyPart TargetBodyPart { get; set; }
        public IItem Weapon { get; set; }
        public uint CalculatedDamage { get; set; }
    }
}
