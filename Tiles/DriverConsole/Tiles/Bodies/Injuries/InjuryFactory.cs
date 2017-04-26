using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Bodies.Injuries
{
    public interface IInjuryFactory
    {
        IEnumerable<IBodyPartInjury> Create(
            IBodyPart targetPart,
            double contactArea,
            int maxPenetration,
            ILayeredMaterialStrikeResult result,
            Dictionary<ITissueLayer, IBodyPart> tlParts);
    }

    public class InjuryFactory : IInjuryFactory
    {
        public IEnumerable<IBodyPartInjury> Create(
            IBodyPart targetPart,
            double contactArea,
            int maxPenetration,
            ILayeredMaterialStrikeResult result,
            Dictionary<ITissueLayer, IBodyPart> tlParts)
        {
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
                    tissueResult.ContactArea,
                    maxPenetration
                    );

                double newDamage = System.Math.Max(1d, ttFact);
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
            // add the rest
            foreach (var bp in tissueInjuries.Keys)
            {
                if (bp != targetPart)
                {
                    bpInjuries.Add(CreateBodyPartInjury(bp, contactArea, result, tissueInjuries[bp]));
                }
            }

            return bpInjuries;
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
            return new BodyPartInjury(
                BodyPartInjuryClasses.TissueDamage, part, tissueInjuries);
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
            MaterialStrikeResult tissueResult,
            IDamageVector tissueDamage,
            IDamageVector existingDamage)
        {
            yield return
                new TissueLayerInjury(
                    new MsrTissueLayerInjuryClass(bodyPart, layer, tissueResult),
                    layer, tissueDamage, tissueResult);
        }

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
            else
            {
                return new DamageVector(new Dictionary<DamageType, int>
                    {
                        {DamageType.Bludgeon, 1},
                    });
            }
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
    }
}
