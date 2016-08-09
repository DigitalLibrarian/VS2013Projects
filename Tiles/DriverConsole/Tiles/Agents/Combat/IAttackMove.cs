using System;
using Tiles.Bodies;
using Tiles.Items;
namespace Tiles.Agents.Combat
{
    public interface IAttackMove
    {
        
        IAgent Attacker { get; set; }
        uint CalculatedDamage { get; set; }
        IAgent Defender { get; set; }
        bool IsCritical { get; set; }
        string Name { get; set; }
        IBodyPart TargetBodyPart { get; set; }
        string Verb { get; set; }
        IWeapon Weapon { get; set; }
    }
}
