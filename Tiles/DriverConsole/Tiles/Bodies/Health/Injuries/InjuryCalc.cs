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
            IMaterial weaponMat,  int weaponSize,
            ContactType attackType, double forcePerArea, int contactArea, 
            IAgent defender, IBodyPart part)
        {
            var armorItems = defender.Outfit.GetItems(part).Where(x => x.IsArmor);
            var tissueLayers = part.Tissue.TissueLayers.Reverse();
            var totalTissueThick = part.Tissue.TotalThickness;

            // TODO - Attack contact area is the minimum of weapon contact area and armor/layer contact area

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
                            weaponMat, weaponSize,
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
                            weaponMat, weaponSize,
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
            // TODO - Wrestling moves are special: breaking bones uses [BENDING_*] values, pinching utilizes [COMPRESSIVE_*] properties, and biting can deal [TENSILE] or [TORSION] damage depending on whether the attack is edged. Those attacks generally ignore armor.
            switch (contactType)
            {
                case ContactType.Edge:
                    yield = material.ShearYield;
                    fractureForce = material.ShearFracture;
                    strainAtYield = material.ShearStrainAtYield;
                    break;
                default:
                    yield = material.ImpactYield;
                    fractureForce = material.ImpactFracture;
                    strainAtYield = material.ImpactStrainAtYield;
                    break;
            }
        }

        IEnumerable<IInjury> PerformMaterialLayer(
            IMaterial weaponMat, int weaponSize,
            IBodyPart part, ITissueLayer layer,
            IMaterial material, int thicknessMm, 
            ContactType contactTypeStart, double force, int contactArea,
            out double momentum, out ContactType contactType)
        {
            int yield = 0;
            int fractureForce = 0;
            int strainAtYield = 0;

            GetModeProperties(contactTypeStart, material, out yield, out fractureForce, out strainAtYield);
            
            int wYield = 0;
            int wFractureForce = 0;
            int wStrainAtYield = 0;

            GetModeProperties(contactTypeStart, weaponMat, out wYield, out wFractureForce, out wStrainAtYield);
            var injuries = new List<IInjury>();

            if (contactTypeStart == ContactType.Edge)
            {
                if(MaterialStressCalc.EdgedStress(force, contactArea, thicknessMm,
                    wYield, wFractureForce, wStrainAtYield,
                    yield, fractureForce, strainAtYield
                    ))
                {

                    double deformDist;
                    var collideResult = MaterialStressCalc.StressLayer(
                        force, contactArea, thicknessMm,
                        yield, fractureForce, strainAtYield,
                        out deformDist);

                    switch (collideResult)
                    {
                        case StressResult.Plastic:
                            injuries.Add(InjuryFactory.Create(StandardInjuryClasses.CutBodyPart, part));
                            break;
                        case StressResult.Fracture:
                            injuries.Add(InjuryFactory.Create(StandardInjuryClasses.BadlyGashedBodyPart, part));
                            break;
                    }
                }

            }
            else
            {
                if (MaterialStressCalc.BluntStress(
                    weaponSize,
                    force, contactArea, thicknessMm,
                    wYield, wFractureForce, wStrainAtYield,
                    yield, fractureForce, strainAtYield
                    ))
                {

                    double deformDist;
                    var collideResult = MaterialStressCalc.StressLayer(
                        force, contactArea, thicknessMm,
                        yield, fractureForce, strainAtYield,
                        out deformDist);

                    switch (collideResult)
                    {
                        case StressResult.Plastic:
                            injuries.Add(InjuryFactory.Create(StandardInjuryClasses.BruisedBodyPart, part));
                            break;
                        case StressResult.Fracture:
                            injuries.Add(InjuryFactory.Create(StandardInjuryClasses.BatteredBodyPart, part));
                            break;
                    }
                }


                /*
                double deformDist;
                var collideResult = MaterialStressCalc.StressLayer(
                    force, contactArea, thicknessMm,
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

                injuries.AddRange(DetermineTissueInjury(
                    weaponMat,
                    contactArea, part, layer, contactTypeStart,
                    collideResult, deformDist).ToList());
                 * */
            }



            momentum = force;
            contactType = contactTypeStart;

            return injuries;
        }

        IEnumerable<IInjury> DetermineTissueInjury(
            IMaterial weaponMat, int contactArea, 
            IBodyPart part, ITissueLayer layer,
            ContactType contactType, StressResult collisionResult, double deform)
        {
            if (layer != null)
            {
                switch (contactType)
                {
                    case ContactType.Edge:
                        return DetermineEdgedInjury(weaponMat, contactArea, part, collisionResult, deform);
                    default:
                        return DetermineBluntInjury(contactArea, part, collisionResult, deform);
                }
            }
            return Enumerable.Empty<IInjury>();
        }

        IEnumerable<IInjury> DetermineEdgedInjury(
            IMaterial weaponMat, int contactArea,
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
            IInjuryClass injuryClass;
            switch (collisionResult)
            {
                case StressResult.Elastic:
                    injuryClass = StandardInjuryClasses.BruisedBodyPart;
                    break;
                case StressResult.Plastic:
                    injuryClass = StandardInjuryClasses.BatteredBodyPart;
                    break;
                case StressResult.Fracture:
                    injuryClass = StandardInjuryClasses.BrokenBodyPart;
                    break;
                default:
                    throw new NotImplementedException();
            }

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
                weapon.Class.Material,
                weapon.Class.Size,
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


            return Enumerable.Empty<IInjury>();
            /*
            return MaterialStrike(
                moveClass.ContactType, 
                forcePerArea, 
                moveClass.ContactArea,
                defender, targetPart);
             * */
        }
    }
}
  