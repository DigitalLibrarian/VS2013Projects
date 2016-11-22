using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Materials;
using Tiles.Random;

namespace Tiles.Injuries
{
    public interface IInjuryReportCalc
    {
        IInjuryReport CalculateMaterialStrike(
            ICombatMoveContext context,
            StressMode stressMode,
            double momentum, double contactArea, int maxPenetration,
            IBodyPart targetPart,
            IMaterial strikerMat,
            double sharpness
            );
    }

    public class InjuryReportCalc : IInjuryReportCalc
    {
        IDamageVector GetUnitDamage(StressMode mode, double contactArea, int penetration)
        {
            if (mode == StressMode.Edge)
            {
                if (IsLowContactArea(contactArea) && IsHighPenetration(penetration))
                {
                    return new DamageVector(new Dictionary<DamageType, int>
                    {
                        {DamageType.Bludgeon, 1},
                        {DamageType.Slash, 1},
                    });
                }
                else
                {
                    return new DamageVector(new Dictionary<DamageType, int>
                    {
                        {DamageType.Slash, 1},
                    });
                }

            }
            else if (mode == StressMode.Blunt || mode == StressMode.Other)
            {
                return new DamageVector(new Dictionary<DamageType, int>
                    {
                        {DamageType.Bludgeon, 1},
                    });
            }
            throw new NotImplementedException();
        }
        bool IsHighContactArea(double contactArea)
        {
            return contactArea > 50;
        }

        bool IsHighPenetration(int maxPenetation)
        {
            return maxPenetation > 2000;
        }
        bool IsLowContactArea(double contactArea)
        {
            return contactArea <= 50;
        }

        bool IsLowPenetration(int maxPenetation)
        {
            return maxPenetation < 2000;
        }

        ILayeredMaterialStrikeResultBuilder CreateBuilder()
        {
            // TODO - inject factory
            return new LayeredMaterialStrikeResultBuilder(new MaterialStrikeResultBuilder()); 
        }
        

        public IInjuryReport CalculateMaterialStrike(ICombatMoveContext context, StressMode stressMode, double momentum, double contactArea, int maxPenetration, IBodyPart targetPart, IMaterial strikerMat, double sharpness)
        {
            var builder = CreateBuilder();

            // resize contact area if the body part is smaller
            // body part size in cm3, contact area in mm3
            double originalContactArea = contactArea;

            var partCa = targetPart.GetContactArea();
            var weaponCa = contactArea;

            builder.SetMomentum(momentum);
            builder.SetStrikerContactArea(contactArea);
            builder.SetStrickenContactArea(partCa);
            builder.SetStrikerSharpness(sharpness);
            builder.SetMaxPenetration(maxPenetration);
            builder.SetStressMode(stressMode);
            builder.SetStrikerMaterial(strikerMat);

            var armorItems = context.Defender.Outfit.GetItems(targetPart).Where(x => x.IsArmor);
            var tissueLayers = targetPart.Tissue.TissueLayers.Reverse();

            var tlParts = new Dictionary<ITissueLayer, IBodyPart>();

            foreach (var armorItem in armorItems)
            {
                builder.AddLayer(armorItem.Class.Material);
            }

            foreach (var tissueLayer in tissueLayers)
            {
                if (!tissueLayer.Class.IsCosmetic)
                {
                    builder.AddLayer(tissueLayer.Material, tissueLayer.Thickness, tissueLayer.Volume, tissueLayer);
                    tlParts.Add(tissueLayer, targetPart);
                }
            }

            // TODO - it should not be possible to sever internal parts
            foreach(var internalPart in context.Defender.Body.GetInternalParts(targetPart))
            {
                foreach (var tissueLayer in internalPart.Tissue.TissueLayers.Reverse())
                {
                    if (!tissueLayer.Class.IsCosmetic)
                    {
                        builder.AddLayer(tissueLayer.Material, tissueLayer.Thickness, tissueLayer.Volume, tissueLayer);
                        tlParts.Add(tissueLayer, internalPart);
                    }
                }
            }
            
            var result = builder.Build();
            
            var tissueInjuries = new Dictionary<IBodyPart, List<ITissueLayerInjury>>();
            foreach (var taggedResult in result.TaggedResults)
            {
                var layerInjuries = Enumerable.Empty<ITissueLayerInjury>();
                var tissueLayer = taggedResult.Key as ITissueLayer;
                var tissueResult = taggedResult.Value;

                var tlBodyPart = tlParts[tissueLayer];

                var totalThick = tlBodyPart.GetThickness();
                double ttFact = (double)(tissueLayer.Thickness) / (double)(totalThick);
                var tissueDamage = GetUnitDamage(
                    tissueResult.StressMode,
                    originalContactArea,
                    maxPenetration
                    );

                var momComp = 100d;
                if (!tissueResult.IsDefeated)
                {
                    double momLeft = (tissueResult.MomentumThreshold - tissueResult.Momentum) / tissueResult.MomentumThreshold;

                    // if momLeft = 0, we want 100% dmg
                    // if momLeft = .5, we want 50 pts
                    // if momLeft = 100, we want 0 pts

                    momComp = (1d - momLeft) * 100;
                }

                double newDamage = System.Math.Max(1d, momComp * ttFact);
                tissueDamage.ScalarMultiply(newDamage);
                var tlInjuries = CreateTissueInjury(tlBodyPart, tissueLayer, tissueResult, tissueDamage, tlBodyPart.Damage);

                if (!tissueInjuries.ContainsKey(tlBodyPart))
                {
                    tissueInjuries[tlBodyPart] = new List<ITissueLayerInjury>();
                }
                tissueInjuries[tlBodyPart].AddRange(tlInjuries);
            }

            var bpInjuries = new List<IBodyPartInjury>();

            if (tissueInjuries.ContainsKey(targetPart))
            {
                var rootInjury = CreateBodyPartInjury(targetPart, contactArea, result, tissueInjuries[targetPart]);
                bpInjuries.Add(rootInjury);
            }
            foreach (var bp in tissueInjuries.Keys)
            {
                if (bp != targetPart)
                {
                    bpInjuries.Add(CreateBodyPartInjury(bp, contactArea, result, tissueInjuries[bp]));
                }
            }
            return new InjuryReport(bpInjuries);
        }

        private IBodyPartInjury CreateBodyPartInjury(
            IBodyPart part, double contactArea,
            ILayeredMaterialStrikeResult result,
            List<ITissueLayerInjury> tissueInjuries)
        {
            // TODO - if the body part only has a single tissue, then the
            // body part injury can just mirror it (look at throat injuries in df combat logs)
            var damage = new DamageVector();
            foreach (var ti in tissueInjuries)
            {
                damage.Add(ti.GetTotal());
            }

            if (WillSever(part, damage, contactArea))
            {
                return new BodyPartInjury(
                    BodyPartInjuryClasses.Severed, part, tissueInjuries);
            }
            else if (!IsBroken(part.Damage) && WillBreak(part.Damage, damage))
            {
                return new BodyPartInjury(
                    BodyPartInjuryClasses.ExplodesIntoGore, part, tissueInjuries);
            }
            else if (!IsMangled(part.Damage) && WillMangle(part.Damage, damage))
            {
                return new BodyPartInjury(
                    BodyPartInjuryClasses.Severed, part, tissueInjuries);
            }
            {
                return new BodyPartInjury(
                    BodyPartInjuryClasses.JustTissueDamage, part, tissueInjuries);
            }
        }


        bool WillSever(IBodyPart part, IDamageVector d, double contactArea)
        {
            if (!part.CanBeAmputated) return false;

            var p = part.Damage;
            var dSlash = d.GetFraction(DamageType.Slash).AsDouble();
            if (dSlash <= 0) return false;

            var pSlash = p.GetFraction(DamageType.Slash).AsDouble();
            if (pSlash + dSlash >= 1)
            {
                if (contactArea >= part.Size * 10)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsBroken(IDamageVector d)
        {
            return d.GetFraction(DamageType.Bludgeon).AsDouble() >= 1d;
        }

        public bool WillBreak(IDamageVector p, IDamageVector d)
        {
            return d.GetFraction(DamageType.Bludgeon).AsDouble()
                + p.GetFraction(DamageType.Bludgeon).AsDouble()
                >= 1f;
        }

        public bool IsMangled(IDamageVector d)
        {
            return new DamageType[]{
                DamageType.Slash,
            }.Select(x => d.GetFraction(x).AsDouble())
            .Sum() >= 1d;
        }

        public bool WillMangle(IDamageVector p, IDamageVector d)
        {
            return new DamageType[]{
                DamageType.Slash,
            }.Select(x =>
                d.GetFraction(x).AsDouble()
                + p.GetFraction(x).AsDouble()
                )
            .Sum() >= 1d;
        }


        private IEnumerable<ITissueLayerInjury> CreateTissueInjury(
            IBodyPart bodyPart, 
            ITissueLayer layer,
            IMaterialStrikeResult tissueResult,
            IDamageVector tissueDamage,
            IDamageVector existingDamage)
        {
            var injuryClasses = new ITissueLayerInjuryClass[]{
                TissueLayerInjuryClasses.Bruise,
                TissueLayerInjuryClasses.Fracture,
                TissueLayerInjuryClasses.Tear,
                TissueLayerInjuryClasses.TearApart,
            };
            foreach (var dt in tissueDamage.GetTypes())
            {

                var injuryDamage = new DamageVector();
                injuryDamage.Set(dt, tissueDamage.Get(dt));
                yield return
                new TissueLayerInjury(
                    new MsrTissueLayerInjuryClass(bodyPart, layer, tissueResult), layer, injuryDamage, tissueResult);
            }
        }
    }
}
