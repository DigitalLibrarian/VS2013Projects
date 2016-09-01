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
using Tiles.Math;

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
            ContactType attackType, double forcePerArea, int contactArea,
            IAgent defender, IBodyPart part)
        {
            var armorItems = defender.Outfit.GetItems(part).Where(x => x.IsArmor);
            var tissueLayers = part.Tissue.TissueLayers.Reverse();
            var totalTissueThick = part.Tissue.TotalThickness;

            var injuries = new List<IInjury>();
            double force = forcePerArea;
            foreach (var armor in armorItems)
            {
                if (force > 0)
                {
                    int armorThickness = part.Size; // TODO - times coverage ratio
                    double momentum;
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
            }

            foreach (var layer in tissueLayers)
            {
                if (force > 0)
                {
                    double momentum;
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
            }

            // TODO - check for conditional injuries on entire body part
            // * pulping
            // * instant death organs
            /*
            if (force > 0)
            {
                injuries.Add(InjuryFactory.Create(
                    StandardInjuryClasses.MissingBodyPart,
                    part));
            }
            */

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
                case ContactType.Other:
                    yield = material.ImpactYield;
                    fractureForce = material.ImpactFracture;
                    strainAtYield = material.ImpactStrainAtYield;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        IEnumerable<IInjury> PerformMaterialLayer(
            IBodyPart part, ITissueLayer layer,
            IMaterial material, int thicknessMm, 
            ContactType contactTypeStart, double force, int contactArea,
            out double momentum, out ContactType contactType)
        {
            int yield = 0;
            int fractureForce = 0;
            int strainAtYield = 0;

            GetModeProperties(contactTypeStart, material, out yield, out fractureForce, out strainAtYield);
            /*
            double forceD = (double)force;
            double contactAreaD = (double)contactArea/ 10000d;
            double thickD = (double)thicknessMm / 1000d;

            // contact area in mm3
            double momD = (forceD / contactAreaD) / thickD;
            int momPerVol = (int)momD;
            */
            double deformDist;
            var collideResult = MaterialStressCalc.StressLayer(
                force,  contactArea, thicknessMm,
                yield, fractureForce, strainAtYield, 
                out deformDist);

            // did not break surface
            if (collideResult == StressResult.Elastic)
            {
                //contactTypeStart = ContactType.Blunt;
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
            ContactType contactType, StressResult collisionResult, double deform)
        {
            if (layer != null)
            {
                switch (contactType)
                {
                    case ContactType.Edge:
                        return DetermineEdgedInjury(contactArea, part, collisionResult, deform);
                    case ContactType.Blunt:
                        return DetermineBluntInjury(contactArea, part, collisionResult, deform);
                    case ContactType.Other:
                        break;
                }
            }
            return Enumerable.Empty<IInjury>();
        }

        IEnumerable<IInjury> DetermineEdgedInjury(
            int contactArea,
            IBodyPart part, StressResult collisionResult, double deform)
        {
            // need to classify as piercing or slash, based on move data

            IInjuryClass injuryClass;
            switch (collisionResult)
            {
                case StressResult.Elastic:
                    injuryClass = StandardInjuryClasses.CutBodyPart;
                    break;
                case StressResult.Plastic:
                    injuryClass = StandardInjuryClasses.BadlyGashedBodyPart;
                    break;
                case StressResult.Fracture:
                    injuryClass = StandardInjuryClasses.MangledBodyPart;
                    break;
                default:
                    throw new NotImplementedException();
            }

            yield return InjuryFactory.Create(injuryClass, part);
        }

        IEnumerable<IInjury> DetermineBluntInjury(
            int contactArea,
            IBodyPart part, StressResult collisionResult, double deform)
        {
            var injuryClass = StandardInjuryClasses.BruisedBodyPart;

            yield return InjuryFactory.Create(injuryClass, part);
        }

        public IEnumerable<IInjury> MeleeWeaponStrike(
            ICombatMoveClass moveClass, double weaponVelo, 
            IAgent attacker, IAgent defender, IBodyPart targetPart, IItem weapon)
        {
            // TODO - this does not take into account attacker stats

            var weaponMass = weapon.GetMass();
            double force = ((double)weaponVelo * ((double)weaponMass));

            return MaterialStrike(
                moveClass.ContactType, 
                force, 
                moveClass.ContactArea,
                defender, targetPart);
        }

        public IEnumerable<IInjury> UnarmedStrike(
            ICombatMoveClass moveClass, double force, 
            IAgent attacker, IAgent defender, IBodyPart targetPart)
        {
            int forcePerArea = 1250;

            return MaterialStrike(
                moveClass.ContactType, 
                forcePerArea, 
                moveClass.ContactArea,
                defender, targetPart);
        }
    }
}
  