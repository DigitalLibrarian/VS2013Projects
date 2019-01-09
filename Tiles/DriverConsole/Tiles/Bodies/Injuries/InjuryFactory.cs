using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;
using Tiles.Random;

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
        private IRandom Random { get; set; }

        public InjuryFactory(IRandom random)
        {
            Random = random;
        }

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
            //var isChip = tissueResult.ContactArea <= bodyPart.ContactArea;

            // TODO - this should be checked after the fact by looking at the layer's wounds.  That way
            // hitting a chipped bone can "fracture" it.
            var isChip = tissueResult.ContactAreaRatio < 0.25d;
            var isSoft = layer.Material.IsSoft(tissueResult.StressMode);
            var isVascular = layer.IsVascular;

            var damage = new DamageVector();
            switch (tissueResult.StressResult)
            {
                case StressResult.Impact_Dent:
                    damage.DentFraction.Numerator = Round(tissueResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
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
                case StressResult.None:
                    damage.EffectFraction.Numerator = Round(Min(tissueResult.ContactAreaRatio, tissueResult.PreDentRatio) * (double)damage.EffectFraction.Denominator);
                    break;
                default:
                    break;
            }

            bool arteryOpened = WasArteryOpened(layer, tissueResult);
            var painContribution = GetPainContribution(targetBody, bodyPart, layer, tissueResult, damage);
            var bleedingContribution = GetBleedingContribution(bodyPart, layer, tissueResult, damage, arteryOpened);

            yield return
                new TissueLayerInjury(
                    bodyPart, layer, 
                    tissueResult.StressResult, damage, 
                    woundArea, tissueResult.ContactArea, 
                    tissueResult.ContactAreaRatio, tissueResult.PenetrationRatio, 
                    painContribution, bleedingContribution, arteryOpened,
                    tissueResult.IsDefeated, isChip, isSoft, isVascular);
        }

        private double Min(params double[] nums)
        {
            return nums.Min();
        }

        private double Max(params double[] nums)
        {
            return nums.Max();
        }

        private bool WasArteryOpened(ITissueLayer layer, MaterialStrikeResult tissueResult)
        {
            if (!layer.Class.HasArteries) return false;
            return Random.NextDouble() < tissueResult.ContactAreaRatio * tissueResult.PenetrationRatio;
        }
        
        private int GetPainContribution(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0
                && tissueResult.StressResult != StressResult.Shear_Dent) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var invSizeRatio = tissueResult.ImplementSize / bodyPart.Size;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var invBpCaRatio = bodyPart.ContactArea / tissueResult.ImplementContactArea;
            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var partNormalizer = 650d;
            var partSig = bodyPart.Size / partNormalizer;
            var invPartSig = partNormalizer / bodyPart.Size;
            var partChar = bodyPart.Class.RelativeSize / bodyPart.Thickness;
            var relSizeRatio = bodyPart.Class.RelativeSize / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;

            var multiplier = 1d;
            if(!(partSig > 10 && partRatio <= 0.02))
            {
                multiplier += 1d;
                if (caRatio + penRatio > 1.25d)
                    multiplier += 1d;
            }

            var dmgRatio = Min(caRatio, penRatio);
            if (caRatio > 0.5d)                     dmgRatio = caRatio;
            if (penRatio == 0d)                     dmgRatio = caRatio;
            if (penRatio < 1 && caRatio < 1)        dmgRatio = woundRatio;

            var preRounded = receptors * multiplier * dmgRatio;
            
            var upperBound = Max(partRatio * 10d) * receptors * multiplier;
            preRounded = Min(upperBound, preRounded);

            if (bodyPart.Class.IsSmall || woundRatio < 0.05d)
            {
                preRounded = 1d;
            }
            else if (partSig < 1d)
            {
                var max = receptors * multiplier * partSig * dmgRatio;
                max -= 0.5d;
                max = Max(0.5d, max);
                preRounded = Min(preRounded, max);
            }
            else if (partSig >= 2d && woundRatio > 0.25d)
            {
                preRounded *= woundRatio;
            }

            return (int) System.Math.Round(preRounded, 0, MidpointRounding.AwayFromZero);
        }

        public int GetBleedingContribution(IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult strikeResult, IDamageVector damage, bool arteryOpened)
        {
            var multiplier = (double)layer.Class.VascularRating;
            if (arteryOpened) multiplier *= 5d;

            var bleed = strikeResult.ContactAreaRatio * strikeResult.PenetrationRatio * multiplier;

            if (layer.Class.HasArteries)
                bleed *= 3d;

            return (int)System.Math.Round(bleed, 0, MidpointRounding.AwayFromZero);
        }

        private long Round(double d)
        {
            return ((long)((double)d / 10d)) * 10L;
        }
    }
}
