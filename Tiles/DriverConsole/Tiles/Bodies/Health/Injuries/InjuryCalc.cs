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
        IMaterialStrikeResultBuilder Builder { get; set; }

        public InjuryCalc(IInjuryFactory injuryFactory)
        {
            InjuryFactory = injuryFactory;
            Builder = new MaterialStrikeResultBuilder();
        }

        public IEnumerable<IInjury> MaterialStrike(
            IMaterial weaponMat,  int weaponSize,
            StressMode attackType, double forcePerArea, int contactArea, 
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
                    StressMode contactType;
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
                    StressMode contactType;
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

        IEnumerable<IInjury> PerformMaterialLayer(
            IMaterial weaponMat, int weaponSize,
            IBodyPart part, ITissueLayer layer,
            IMaterial material, int thicknessMm, 
            StressMode stressModeIn, double force, int contactArea,
            out double momentumOut, out StressMode stressModeOut)
        {
            Builder.Clear();
            Builder.SetStressMode(stressModeIn);
            Builder.SetStrickenMaterial(material);
            Builder.SetStrikerMaterial(weaponMat);
            Builder.SetStrikeMomentum(force);
            Builder.SetContactArea(contactArea);

            //If an attack connects, the target will be wounded in some part of the body. 
            //The severity of the wound depends on 
            //  1) the strength of the attack, 
            //  2) the protective value of any armor or other protection available for that body part, 
            //  3) a (large) random factor. 

            //Wounds are cumulative: when an already wounded body part is hit the wound will worsen, 
            //  even if in adventure mode it produces the same message about the condition of the 
            //  body part more than once.

            var result = Builder.Build();

            if (result.BreaksThrough)
            {

            }
            else
            {
                if (stressModeIn != StressMode.Blunt)
                {
                    // convert to blunt
                    Builder.SetStressMode(StressMode.Blunt);
                    result = Builder.Build();
                    if (result.BreaksThrough)
                    {


                    }
                    else
                    {
                        // TODO - 
                        //If both edged and blunt momenta thresholds haven't been met, 
                        //attack is permanently converted to blunt and its momentum may be 
                        //greatly reduced. Specifically, it is multiplied by 
                        //SHEAR_STRAIN_AT_YIELD/50000 for edged attacks 
                        //or IMPACT_STRAIN_AT_YIELD/50000 otherwise. 
                        //I.e., most metals reduce blocked attacks by 98%-99%
                    }
                }

            }

            if (result.BreaksThrough)
            {
                momentumOut = force - (force * (5d / 100d));
            }
            throw new NotImplementedException();
        }

        void AccumulateStrikeDamage(IDamageVector damageVector, ITissueLayer layer, DamageType damageType, IMaterialStrikeResult strikeResult)
        {
            double damageValue = strikeResult.ExcessMomentum * (double)layer.Class.RelativeThickness;
            damageVector.Set(damageType, (int)damageValue);
        }


        public IEnumerable<IInjury> MeleeWeaponStrike(
            ICombatMoveClass moveClass, double weaponVelo, 
            IAgent attacker, IAgent defender, IBodyPart targetPart, IItem weapon)
        {
            
            var massGrams = weapon.GetMass();
            double force = ((double)weaponVelo * ((double) massGrams/1000d) );

            return MaterialStrike(
                weapon.Class.Material,
                weapon.Class.Size,
                moveClass.StressMode, 
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
  