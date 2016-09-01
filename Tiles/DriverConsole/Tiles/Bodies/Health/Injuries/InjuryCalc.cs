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
        IInjuryFactory InjuryFactory { get; set; }

        public InjuryCalc(IInjuryFactory injuryFactory)
        {
            InjuryFactory = injuryFactory;
        }

        public IEnumerable<IInjury> MaterialStrike(
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

                int armorThickness = part.Size; // TODO - times coverage ratio
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

                int momentum;
                ContactType contactType;
                injuries.AddRange(
                    PerformMaterialLayer(
                        part, layer,
                        layer.Material,
                        layer.Thickness,
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

        void GetModeProperties(ContactType contactType, IMaterial material,
            out int yield, out int fractureForce, out int strainAtYield)
        {
            switch (contactType)
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
                default:
                    throw new NotImplementedException();
            }
        }

        IEnumerable<IInjury> PerformMaterialLayer(
            IBodyPart part, ITissueLayer layer,
            IMaterial material, int thickness, 
            ContactType contactTypeStart, int force, int contactArea,
            out int momentum, out ContactType contactType)
        {
            int yield = 0;
            int fractureForce = 0;
            int strainAtYield = 0;

            GetModeProperties(contactTypeStart, material, out yield, out fractureForce, out strainAtYield);

            // TODO - make contact area affect unit force
            int momPerVol = (force * contactArea) /  thickness;

            int deformDist;
            var collideResult = MaterialStressCalc.StressLayer(
                momPerVol, 
                yield, fractureForce, strainAtYield, 
                out deformDist);

            // did not break surface
            if (collideResult == StressResult.Elastic)
            {
                contactTypeStart = ContactType.Blunt;
                force = 0;

                // all elastic collides stop weapon momentum
            }
            else
            {
                // lose 5%
                force = force - (force / 20);
            }

            var injuries = DetermineTissueInjury(
                contactArea, part, layer, contactTypeStart,
                collideResult, deformDist).ToList();

            momentum = force;
            contactType = contactTypeStart;

            return injuries;
        }

        IEnumerable<IInjury> DetermineTissueInjury(
            int contactArea, 
            IBodyPart part, ITissueLayer layer, 
            ContactType contactType, StressResult collisionResult, int deform)
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
            IBodyPart part, ITissueLayer layer, StressResult collisionResult, int deform)
        {
            // need to classify as piercing or slash, based on move data
            bool stab = contactArea <= 50; // magic from wiki

            if (collisionResult != StressResult.Elastic)
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
            IBodyPart part, ITissueLayer layer, StressResult collisionResult, int deform)
        {
            var injuryClass = StandardInjuryClasses.BruisedTissueLayer;

            yield return InjuryFactory.Create(injuryClass, part, layer);
        }

        public IEnumerable<IInjury> MeleeWeaponStrike(ICombatMoveClass moveClass, IAgent attacker, IAgent defender, IBodyPart targetPart, IItem weapon)
        {
            // TODO - this does not take into account attacker stats
            int weaponMomentum = weapon.Class.Size * weapon.Class.Material.SolidDensity;
            return MaterialStrike(
                moveClass.ContactType, 
                moveClass.ContactArea * weaponMomentum, 
                moveClass.ContactArea,
                defender, targetPart);
        }

        public IEnumerable<IInjury> UnarmedStrike(ICombatMoveClass moveClass, IAgent attacker, IAgent defender, IBodyPart targetPart)
        {
            int force = 1000;
            return MaterialStrike(
                moveClass.ContactType, 
                moveClass.ContactArea * force, 
                moveClass.ContactArea,
                defender, targetPart);
        }
    }
}
