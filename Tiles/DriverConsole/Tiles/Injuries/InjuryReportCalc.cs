﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Injuries
{
    public interface IInjuryReportCalc
    {
        IInjuryReport CalculateMaterialStrike(
            ICombatMoveContext context,
            StressMode stressMode,
            double momentum, int contactArea, int maxPenetration,
            IBodyPart targetPart,
            IMaterial strikerMat
            );
    }

    public class InjuryReportCalc : IInjuryReportCalc
    {
        ILayeredMaterialStrikeResultBuilder Builder { get; set; }

        public InjuryReportCalc(ILayeredMaterialStrikeResultBuilder builder)
        {
            Builder = builder;
        }


        IDamageVector GetUnitDamage(StressMode mode, int contactArea, int penetration)
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
            else if (mode == StressMode.Blunt)
            {
                return new DamageVector(new Dictionary<DamageType, int>
                    {
                        {DamageType.Bludgeon, 1},
                    });
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
        public IInjuryReport CalculateMaterialStrike(ICombatMoveContext context, StressMode stressMode, double momentum, int contactArea, int maxPenetration, IBodyPart targetPart, IMaterial strikerMat)
        {
            Builder.Clear();

            // resize contact area if the body part is smaller
            // body part size in cm3, contact area in mm3
            int originalContactArea = contactArea;
            contactArea = System.Math.Min(targetPart.Size * 10, contactArea);

            Builder.SetMomentum(momentum);
            Builder.SetContactArea(contactArea);
            Builder.SetMaxPenetration(maxPenetration);
            Builder.SetStressMode(stressMode);
            Builder.SetStrikerMaterial(strikerMat);

            var totalThick = targetPart.Tissue.TotalThickness;
            var armorItems = context.Defender.Outfit.GetItems(targetPart).Where(x => x.IsArmor);
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

            var tissueInjuries = new List<ITissueLayerInjury>();
            foreach (var taggedResult in result.TaggedResults)
            {
                var tissueLayer = taggedResult.Key as ITissueLayer;
                var tissueResult = taggedResult.Value;

                double ttFact = (double)(tissueLayer.Thickness + 1) / (double)(totalThick + 1);
                var tissueDamage = GetUnitDamage(
                    tissueResult.StressMode,
                    originalContactArea,
                    maxPenetration
                    );

                var momComp = 100d;
                if (!tissueResult.BreaksThrough)
                {
                    double momLeft = (tissueResult.MomentumThreshold - tissueResult.Momentum) / tissueResult.MomentumThreshold;

                    // if momLeft = 0, we want 100% dmg
                    // if momLeft = .5, we want 50 pts
                    // if momLeft = 100, we want 0 pts

                    momComp = (1d - momLeft) * 100;
                }

                double newDamage = System.Math.Max(1d, momComp * ttFact);
                tissueDamage.ScalarMultiply(newDamage);

                tissueInjuries.AddRange(CreateTissueInjury(tissueLayer, tissueResult, tissueDamage, targetPart.Damage));
            }


            return new InjuryReport(new IBodyPartInjury[]{
                CreateBodyPartInjury(targetPart, contactArea, result, tissueInjuries)
            });
        }

        private IBodyPartInjury CreateBodyPartInjury(
            IBodyPart part, int contactArea,
            ILayeredMaterialStrikeResult result,
            List<ITissueLayerInjury> tissueInjuries)
        {
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
            else
            {
                return new BodyPartInjury(
                    BodyPartInjuryClasses.JustTissueDamage, part, tissueInjuries);
            }

        }


        bool WillSever(IBodyPart part, IDamageVector d, int contactArea)
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
                DamageType.Gore,
                DamageType.Pierce,
                DamageType.Slash,
            }.Select(x => d.GetFraction(x).AsDouble())
            .Sum() >= 1d;
        }

        public bool WillMangle(IDamageVector p, IDamageVector d)
        {
            return new DamageType[]{
                DamageType.Gore,
                DamageType.Pierce,
                DamageType.Slash,
            }.Select(x =>
                d.GetFraction(x).AsDouble()
                + p.GetFraction(x).AsDouble()
                )
            .Sum() >= 1d;
        }


        private IEnumerable<ITissueLayerInjury> CreateTissueInjury(
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

            foreach (var injuryClass in injuryClasses)
            {
                var newDVal = tissueDamage.GetFraction(injuryClass.DamageType).AsDouble();

                if (injuryClass.IsInRange(newDVal))
                {
                    var injuryDamage = new DamageVector();
                    injuryDamage.Set(injuryClass.DamageType, tissueDamage.Get(injuryClass.DamageType));
                    yield return new TissueLayerInjury(injuryClass, layer, injuryDamage, tissueResult);
                }
            }
        }
    }
}