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
using Tiles.Materials;

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
            var damage = DamageVector.CreateUnit();

            return Core(damage, defender, targetPart);
        }

        IEnumerable<IInjury> Core(
            IDamageVector damage,
            IAgent defender,
            IBodyPart targetPart
            )
        {
            var armorItems = defender.Outfit.GetItems(targetPart).Where(x => x.IsArmor);
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

    public class BetterInjuryCalc : IInjuryCalc
    {
        IInjuryFactory InjuryFactory { get; set; }

        public BetterInjuryCalc(IInjuryFactory injuryFactory)
        {
            InjuryFactory = injuryFactory;
        }

        public IEnumerable<IInjury> Find(
            ContactType attackType, int forcePerArea, int contactArea,
            IAgent defender, IBodyPart part)
        {
            var armorItems = defender.Outfit.GetItems(part).Where(x => x.IsArmor);
            var tissueLayers = part.Tissue.TissueLayers.Reverse();
            var totalTissueThick = part.Tissue.TotalThickness;

            var injuries = new List<IInjury>();
            int force = forcePerArea;
            foreach (var armor in armorItems)
            {
                if (force <= 0)
                {
                    break;
                }

                int armorThickness = 100;
                int momentum;
                ContactType contactType;
                injuries.AddRange(
                    PerformMaterialLayer(
                        part, null,
                        armor.Class.Material,
                        armorThickness,
                        attackType, force, contactArea,
                        out momentum, out contactType
                        ));

                force = momentum;
                attackType = contactType;
            }

            foreach (var layer in tissueLayers)
            {
                if (force <= 0)
                {
                    break;
                }

                int armorThickness = 100;
                int momentum;
                ContactType contactType;
                injuries.AddRange(
                    PerformMaterialLayer(
                        part, layer,
                        layer.Material,
                        armorThickness,
                        attackType, force, contactArea,
                        out momentum, out contactType
                        ));

                force = momentum;
                attackType = contactType;
            }

            // TODO - check for conditional injuries on entire body part
            // * pulping
            // * instant death organs

            if (force > 0)
            {
                injuries.Add(InjuryFactory.Create(
                    StandardInjuryClasses.MissingBodyPart,
                    part));
            }


            return injuries;
        }

        IEnumerable<IInjury> PerformMaterialLayer(
            IBodyPart part, ITissueLayer layer,
            IMaterial material, int thickness, 
            ContactType contactTypeStart, int forcePerArea, int contactArea,
            out int momentum, out ContactType contactType)
        {
            int yield = 0;
            int fractureForce = 0;
            int strainAtYield = 0;

            switch (contactTypeStart)
            {
                case ContactType.Blunt:
                    yield = material.ImpactYield;
                    fractureForce = material.ImpactFracture;
                    strainAtYield = material.ImpactStrainAtYield;
                    break;
                case ContactType.Edge:
                    yield = material.ShearYield;
                    fractureForce = material.ShearFracture;
                    strainAtYield = material.ShearStrainAtYield;
                    break;
            }

            int deformDist;
            var collideResult = CollideLayer(forcePerArea, yield, fractureForce, strainAtYield, out deformDist);

            // did not break surface
            if (collideResult == CollisionResult.Elastic)
            {
                contactTypeStart = ContactType.Blunt;
                forcePerArea = 0;
            }
            else
            {
                forcePerArea = forcePerArea - (forcePerArea / 20);
            }

            var injuries = DetermineTissueInjury(
                contactArea, part, layer, contactTypeStart,
                collideResult, deformDist).ToList();

            momentum = forcePerArea * thickness;
            contactType = contactTypeStart;

            return injuries;
        }

        IEnumerable<IInjury> DetermineTissueInjury(
            int contactArea, 
            IBodyPart part, ITissueLayer layer, 
            ContactType contactType, CollisionResult collisionResult, int deform)
        {
            if (layer != null)
            {
                switch (contactType)
                {
                    case ContactType.Edge:
                        return DetermineEdgedInjury(contactArea, part, layer, collisionResult, deform);
                    case ContactType.Blunt:
                        return DetermineBluntInjury(contactArea, part, layer, collisionResult, deform);
                    case ContactType.Other:
                        break;
                }
                throw new NotImplementedException();
            }
            return Enumerable.Empty<IInjury>();
        }

        IEnumerable<IInjury> DetermineEdgedInjury(
            int contactArea,
            IBodyPart part, ITissueLayer layer, CollisionResult collisionResult, int deform)
        {
            // need to classify as piercing or slash, based on move data
            bool stab = contactArea <= 50; // magic from wiki

            if (collisionResult != CollisionResult.Elastic)
            {
                // light injury
                var injuryClass = stab
                    ? StandardInjuryClasses.PuncturedTissueLayer
                    : StandardInjuryClasses.TornTissueLayer;

                yield return InjuryFactory.Create(injuryClass, part, layer);
            }
            else
            {
                var injuryClass = StandardInjuryClasses.BruisedTissueLayer;

                yield return InjuryFactory.Create(injuryClass, part, layer);
            }
        }

        IEnumerable<IInjury> DetermineBluntInjury(
            int contactArea,
            IBodyPart part, ITissueLayer layer, CollisionResult collisionResult, int deform)
        {
            var injuryClass = StandardInjuryClasses.BruisedTissueLayer;

            yield return InjuryFactory.Create(injuryClass, part, layer);
        }

        enum CollisionResult
        {
            Elastic,
            Plastic,
            Fracture
        }

        CollisionResult CollideLayer(
            int forcePerArea,
            int yieldForce, int fractureForce, int strainAtYield,
            out int deformDistance)
        {
            deformDistance = strainAtYield * forcePerArea;
            var result = CollisionResult.Elastic;

            if (forcePerArea > fractureForce)
            {
                // broken through
                result = CollisionResult.Fracture;

            }
            else if (forcePerArea > yieldForce)
            {
                // inelastic (plastic) deformation
                result = CollisionResult.Plastic;

            }

            return result;
        }

        public IEnumerable<IInjury> MeleeWeaponStrike(ICombatMoveClass moveClass, IAgent attacker, IAgent defender, IBodyPart targetPart, IItem weapon)
        {
            int force = 1000;
            return Find(
                moveClass.ContactType, moveClass.ContactArea * force, moveClass.ContactArea,
                defender, targetPart);
        }

        public IEnumerable<IInjury> UnarmedStrike(ICombatMoveClass moveClass, IAgent attacker, IAgent defender, IBodyPart targetPart)
        {
            int force = 1000;
            return Find(
                moveClass.ContactType, moveClass.ContactArea * force, moveClass.ContactArea,
                defender, targetPart);
        }
    }
}
