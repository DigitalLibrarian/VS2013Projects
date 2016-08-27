using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Health;
using Tiles.Items;

namespace Tiles.Bodies.Health.Injuries
{
    public class InjuryCalc : IInjuryCalc
    {
        IInjuryResultBuilderFactory BuilderFactory { get; set; }
        IDamageResistorFactory ResistorFactory { get; set; }

        public InjuryCalc(
            IInjuryResultBuilderFactory builderFactory,
            IDamageResistorFactory resistorFactory)
        {
            BuilderFactory = builderFactory;
            ResistorFactory = resistorFactory;
        }

        public IEnumerable<IInjury> MeleeWeaponStrike(
            ICombatMoveClass moveClass, 
            IAgent attacker, IAgent defender, 
            IBodyPart targetPart, 
            IItem weapon)
        {
            // TODO - use move class properties like contact type,
            // peneration depth, and contact area to scale the base
            // damage vector
            // Ideally we can be completely creative and come up with our
            // own damage vector (DamageVectorFactory) as the starting point for
            // the injury calculations.  Then we can take it out of injury class
            // all together.

            return Core(moveClass.DamageVector, defender, targetPart);
        }

        public IEnumerable<IInjury> UnarmedStrike(
            ICombatMoveClass moveClass, 
            IAgent attacker, IAgent defender, 
            IBodyPart targetPart)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IInjury> Core(
            IDamageVector damage,
            IAgent defender,
            IBodyPart targetPart
            )
        {
            var armorItems = defender.Outfit.GetItems(targetPart);
            var tissueLayers = targetPart.Tissue.TissueLayers;
            var totalTissueThick = targetPart.Tissue.TotalThickness;

            var b = BuilderFactory.Create();
            b.SetTargetBodyPart(targetPart);
            b.AddDamage(damage);
            foreach (var armor in armorItems)
            {
                b.AddArmorResistor(ResistorFactory.Create(armor));
            }

            foreach (var layer in tissueLayers)
            {
                b.AddTissueResistor(layer, ResistorFactory.Create(layer));
            }

            return b.Build();

        }
    }
}
