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
        public AttackMove(string name, IAgent attacker, IAgent defender, IBodyPart targetBodyPart, uint damage)
        {
            Name = name;
            Attacker = attacker;
            Defender = defender;
            TargetBodyPart = targetBodyPart;
            CalculatedDamage = damage;
        }

        public string Name { get; set; }
        public string Verb { get; set; }
        public bool IsCritical { get; set; }
        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
        public IBodyPart TargetBodyPart { get; set; }
        public IWeapon Weapon { get; set; }
        public uint CalculatedDamage { get; set; }
    }
}
