using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Bodies.Health.Injuries
{
    public class MostBestInjuryCalc : IInjuryCalc
    {
        IInjuryFactory InjuryFactory { get; set; }
        ILayeredMaterialStrikeResultBuilder Builder { get; set; }
        public MostBestInjuryCalc(IInjuryFactory injuryFactory, ILayeredMaterialStrikeResultBuilder builder)
        {
            InjuryFactory = injuryFactory;
            Builder = builder;
        }

        public IEnumerable<IInjury> MeleeWeaponStrike(
            ICombatMoveClass moveClass, 
            double weaponVelo, 
            IAgent attacker, 
            IAgent defender, 
            IBodyPart targetPart, 
            IItem weapon)
        {
            Builder.Clear();

            var massGrams = weapon.GetMass();
            double momentum = ((double)weaponVelo * ((double) massGrams/1000d) );
            Builder.SetMomentum(momentum);
            Builder.SetContactArea(moveClass.ContactArea);
            Builder.SetStressMode(moveClass.StressMode);
            Builder.SetStrikerMaterial(weapon.Class.Material);

            var armorItems = defender.Outfit.GetItems(targetPart).Where(x => x.IsArmor);
            var tissueLayers = targetPart.Tissue.TissueLayers.Reverse();

            foreach (var armorItem in armorItems)
            {
                Builder.AddLayer(armorItem.Class.Material);
            }

            foreach (var tissueLayer in tissueLayers)
            {
                Builder.AddLayer(tissueLayer.Material, tissueLayer.Thickness, tissueLayer);
            }

            var result = Builder.Build();
            int contactArea = moveClass.ContactArea;
            int maxPen = moveClass.MaxPenetration;
            var totalThick = targetPart.Tissue.TotalThickness;
            var damage = new DamageVector();
            foreach (var taggedResult in result.TaggedResults)
            {
                var tissueLayer = taggedResult.Key as ITissueLayer;
                var tissueResult = taggedResult.Value;
                var dt = ClassifyDamageType(
                    tissueResult.StressMode,
                    contactArea,
                    maxPen
                    );

                double penFact = (double)((tissueLayer.Thickness+1) * 100)/ (double)(totalThick+1);

                if (tissueResult.BreaksThrough)
                {
                    var excess =  tissueResult.Momentum + 1d + tissueResult.ExcessMomentum;
                    var damageD = excess * penFact;
                    damage.Set(dt, (int)damageD);

                }
                else if (dt == DamageType.Bludgeon)
                {
                    double mom = tissueResult.Momentum;
                    double t = tissueResult.MomentumThreshold;
                    double damageD = (mom / t) * penFact;
                    damage.Set(dt, (int)damageD);
                }
            }

            // now that we know the damage, we check for new injuries that need to 
            // be reported

            return CreateInjuries(targetPart, damage);

            /*
            var damage = new DamageVector();
            foreach (var taggedResult in result.TaggedResults) 
            {
                var tissueLayer = taggedResult.Key as ITissueLayer;
                var tissueResult = taggedResult.Value;

                AccumulateTissueLayerDamage(damage, tissueLayer, tissueResult);
            }
             * */

            // now we have the total amount of multidimensional damage that should
            // be applied to the body part.

            //return CreateInjuries(targetPart, damage);
        }


        DamageType ClassifyDamageType(StressMode mode, int contactArea, int penetration)
        {

            //The contact area represents the area of contact of the weapon, 
            //and the penetration determines how deep the attack goes 
            //(and is apparently ignored entirely for BLUNT attacks
            //Large contact areas combined with low penetration represent slashing attacks, 
            //while small contact areas with high penetration behave as piercing attacks.
            
            if (mode == StressMode.Edge)
            {
                if (IsHighContactArea(contactArea) && IsLowPenetration(penetration))
                {
                    return DamageType.Slash;
                }

                if (IsLowContactArea(contactArea) && IsHighPenetration(penetration))
                {
                    return DamageType.Pierce;
                }
            }
            else if (mode == StressMode.Blunt)
            {
                return DamageType.Bludgeon;
            }

            throw new NotImplementedException();
        }

        private IEnumerable<IInjury> CreateInjuries(IBodyPart part, DamageVector damage)
        {

            foreach (var dti in _Dtis)
            {
                var partFraction = part.Damage.GetFraction(dti.DamageType);
                var newFraction = damage.GetFraction(dti.DamageType);

                if (dti.IsHit(part, damage))
                {
                    var injuryClass = dti.PickInjuryClass(part, damage);
                    yield return InjuryFactory.Create(injuryClass, part, damage);
                    break;
                }
            }
        }

        class DtiBinding
        {
            public DamageType DamageType { get; set; }
            public double Threshold_Low_Moderate { get; set; }
            public IInjuryClass LowClass { get; set;}
            public IInjuryClass ModerateClass { get; set;}

            public DtiBinding(DamageType dt, double lowMod, IInjuryClass lowClass, IInjuryClass moderateClass)
            {
                DamageType = dt;
                Threshold_Low_Moderate = lowMod;

                LowClass = lowClass;
                ModerateClass = moderateClass;
            }

            bool IsRelevant(IBodyPart part, DamageVector damage)
            {
                return (damage.GetFraction(DamageType).AsDouble() > 0);
            }

            public bool IsHit(IBodyPart part, DamageVector damage)
            {
                if (!IsRelevant(part, damage)) return false;

                var damageFraction = damage.GetFraction(DamageType);

                return (damageFraction.AsDouble() > 0);
            }

            public bool BreaksModerate(Fraction damageFraction)
            {
                var damageRatio = damageFraction.AsDouble();
                return (damageRatio > Threshold_Low_Moderate);
            }

            public IInjuryClass PickInjuryClass(IBodyPart part, DamageVector damage)
            {
                var bodyFraction = part.Damage.GetFraction(DamageType);
                var damageFraction = damage.GetFraction(DamageType);

                var num = bodyFraction.Numerator + (damageFraction.Numerator * (bodyFraction.Denominator / damageFraction.Denominator));
                var newFraction = new Fraction(
                    num,
                    bodyFraction.Denominator);

                if (BreaksModerate(newFraction))
                {
                    return ModerateClass;
                }
                else
                {
                    return LowClass;
                }
            }
        }

        static DtiBinding[] _Dtis = new DtiBinding[]
        {
            new DtiBinding(DamageType.Bludgeon, 0.1, 
                StandardInjuryClasses.BruisedBodyPart,
                StandardInjuryClasses.BatteredBodyPart),
            new DtiBinding(DamageType.Slash, 0.1,
                StandardInjuryClasses.CutBodyPart,
                StandardInjuryClasses.BadlyGashedBodyPart),
            new DtiBinding(DamageType.Pierce, 0.1,
                StandardInjuryClasses.PiercedBodyPart,
                StandardInjuryClasses.BadlyPiercedBodyPart),
            new DtiBinding(DamageType.Gore, 0.1,
                StandardInjuryClasses.TornBodyPart,
                StandardInjuryClasses.BadlyRippedBodyPart),
        };


        private void AccumulateTissueLayerDamage(DamageVector damage, ITissueLayer tissueLayer, IMaterialStrikeResult tissueResult)
        {
            if (tissueResult.BreaksThrough)
            {
                var excess = 1+(int)tissueResult.ExcessMomentum;
                var dt = ClassifyDamageType(tissueResult.StressMode, tissueResult);

                var dComp = damage.Get(dt);
                damage.Set(dt, dComp + (excess * tissueLayer.Class.RelativeThickness));
            }
        }

        DamageType ClassifyDamageType(StressMode mode, IMaterialStrikeResult tissueResult)
        {

            //The contact area represents the area of contact of the weapon, 
            //and the penetration determines how deep the attack goes 
            //(and is apparently ignored entirely for BLUNT attacks
            //Large contact areas combined with low penetration represent slashing attacks, 
            //while small contact areas with high penetration behave as piercing attacks.


            int contactArea = tissueResult.ContactArea;
            switch (mode)
            {
                case StressMode.Blunt:
                    return contactArea <= 9 ? DamageType.Gore : DamageType.Bludgeon;
                case StressMode.Edge:
                    return contactArea <= 50 ? DamageType.Pierce : DamageType.Slash;
            }
            throw new NotImplementedException();
        }

        bool IsHighContactArea(int contactArea)
        {
            return contactArea > 50;
        }

        bool IsHighPenetration(int maxPenetation)
        {
            return maxPenetation > 2000;
        }
        bool IsLowContactArea(int contactArea)
        {
            return contactArea <= 50;
        }

        bool IsLowPenetration(int maxPenetation)
        {
            return maxPenetation < 2000;
        }

        public IEnumerable<IInjury> UnarmedStrike(ICombatMoveClass moveClass, double force, Agents.IAgent attacker, Agents.IAgent defender, IBodyPart targetPart)
        {
            return Enumerable.Empty<IInjury>(); 
        }
    }
}
