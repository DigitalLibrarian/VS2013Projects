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
            IBody targetBody,
            IBodyPart targetPart,
            double contactArea,
            double maxPenetration,
            ILayeredMaterialStrikeResult result,
            Dictionary<ITissueLayer, IBodyPart> tlParts);
    }

    public class InjuryFactory : IInjuryFactory
    {
        public IEnumerable<IBodyPartInjury> Create(
            IBody targetBody,
            IBodyPart targetPart,
            double contactArea,
            double maxPenetration,
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

                var totalThick = tlBodyPart.Thickness;
                double ttFact = (double)(tissueLayer.Thickness) / (double)(totalThick);
                var tlInjuries = CreateTissueInjury(targetBody, tlBodyPart, tissueLayer, tissueResult);

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
            
            return new BodyPartInjury(part, tissueInjuries);
        }

        private IEnumerable<ITissueLayerInjury> CreateTissueInjury(
            IBody targetBody,
            IBodyPart bodyPart,
            ITissueLayer layer,
            MaterialStrikeResult tissueResult)
        {
            var woundArea = tissueResult.PenetrationRatio >= 1d ? tissueResult.ContactArea : 0d;
            var isChip = tissueResult.ContactArea <= bodyPart.ContactArea;
            var isSoft = layer.Material.IsSoft(tissueResult.StressMode);
            var isVascular = layer.IsVascular;

            var damage = new DamageVector();
            switch (tissueResult.StressResult)
            {
                case StressResult.Impact_Bypass:
                    damage.EffectFraction.Numerator = Round(tissueResult.ContactAreaRatio * (double)damage.EffectFraction.Denominator);
                    break;
                case StressResult.Impact_CompleteFracture:
                    damage.CutFraction.Numerator = Round(tissueResult.PenetrationRatio * tissueResult.ContactAreaRatio * (double)damage.CutFraction.Denominator);
                    damage.DentFraction.Numerator = Round(tissueResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                case StressResult.Shear_Dent:
                    damage.DentFraction.Numerator = Round(tissueResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                case StressResult.Shear_Cut:
                    damage.CutFraction.Numerator = Round(tissueResult.PenetrationRatio * tissueResult.ContactAreaRatio * (double)damage.CutFraction.Denominator);
                    damage.DentFraction.Numerator = Round(tissueResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                case StressResult.Shear_CutThrough:
                    damage.CutFraction.Numerator = Round(tissueResult.PenetrationRatio * tissueResult.ContactAreaRatio * (double)damage.CutFraction.Denominator);
                    damage.DentFraction.Numerator = Round(tissueResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                default:
                    break;
            }

            var painContribution = GetPainContribution(targetBody, bodyPart, layer, tissueResult, damage);

            yield return
                new TissueLayerInjury(bodyPart, layer, tissueResult.StressResult, damage, woundArea, tissueResult.ContactArea, tissueResult.ContactAreaRatio, tissueResult.PenetrationRatio, painContribution, tissueResult.IsDefeated, isChip, isSoft, isVascular);
        }

        private int GetPainContribution(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            double volDamaged = layer.Volume * tissueResult.ContactAreaRatio;

            double preRounded = receptors *
                    (
                          damage.DentFraction.AsDouble()
                        + damage.CutFraction.AsDouble()
                        + tissueResult.ContactAreaRatio
                        );

            if (tissueResult.PenetrationRatio >= 1d)
            {
                var layerRatio = (double) layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
                var penRatio = (double)bodyPart.Thickness / (double)tissueResult.ImplementMaxPenetration;
                var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
                var weaponFactor = tissueResult.ContactArea / tissueResult.ImplementContactArea;
                if (IsLowContactArea(tissueResult.ContactArea)
                    && IsLowContactArea(tissueResult.ImplementContactArea))
                {
                    preRounded = receptors * (
                        tissueResult.ContactAreaRatio
                        ) * weaponFactor;
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            sum = (int)preRounded;
            sum = System.Math.Max(1, sum);
            return System.Math.Min(3*(int)receptors, sum);
        }

        private long Round(double d)
        {
            return ((long)((double)d / 10d)) * 10L;
        }

        #region Damage Classifiction

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
            return maxPenetation <= 2000;
        }
        #endregion
    }
}
