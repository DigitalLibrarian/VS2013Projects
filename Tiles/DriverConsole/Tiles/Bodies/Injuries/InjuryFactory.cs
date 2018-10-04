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
                    damage.EffectFraction.Numerator = Round(tissueResult.PreDentRatio * (double)damage.EffectFraction.Denominator);
                    break;
                default:
                    break;
            }

            var painContribution = GetPainContribution(targetBody, bodyPart, layer, tissueResult, damage);

            yield return
                new TissueLayerInjury(bodyPart, layer, tissueResult.StressResult, damage, woundArea, tissueResult.ContactArea, tissueResult.ContactAreaRatio, tissueResult.PenetrationRatio, painContribution, tissueResult.IsDefeated, isChip, isSoft, isVascular);
        }

        private double Min(params double[] nums)
        {
            return nums.Min();
        }

        private double Max(params double[] nums)
        {
            return nums.Max();
        }
        
        private int GetPainContribution(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
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

            var multiplier = 2d;
            {
                if (caRatio + penRatio > 1.2d)
                    multiplier += 1d;
            }

            var dmgRatio = Min(caRatio, penRatio);
            if (caRatio > 0.5d)                     dmgRatio = caRatio;
            if (penRatio < 0.05d)                   dmgRatio = penRatio;
            if (penRatio < 0.33 && caRatio < 0.33)  dmgRatio = Max(caRatio, penRatio);

            var preRounded = receptors * multiplier * dmgRatio;

            if (bodyPart.Class.IsSmall || woundRatio < 0.05d)
            {
                preRounded = 1;
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

        private int GetPainContribution15(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var invSizeRatio = tissueResult.ImplementSize / bodyPart.Size;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var invBpCaRatio = bodyPart.ContactArea / tissueResult.ImplementContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var invWoundRatio = bodyPart.ContactArea / tissueResult.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var normBpRatio = bodyPart.ContactArea / 50d;
            var invNormBpRatio = 50d / bodyPart.ContactArea;
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            var tissueMass = layer.Material.GetMassForUniformVolume(layer.Volume);
            var partMass = bodyPart.Tissue.TissueLayers.Sum(t => t.Material.GetMassForUniformVolume(t.Volume));
            var wepSizeCaRatio = tissueResult.ImplementSize / tissueResult.ImplementContactArea;
            var invWebSizeCaRatio = tissueResult.ImplementContactArea / tissueResult.ImplementSize;
            var vdEstimate = layer.Volume * tissueResult.ContactAreaRatio * tissueResult.PenetrationRatio;

            var moveSig = tissueResult.ImplementContactArea * tissueResult.ImplementMaxPenetration;
            var moveSigByWeaponSize = moveSig / tissueResult.ImplementSize;
            var moveSigByBpSize = moveSig / bodyPart.Size;
            var moveSigByBpVol = moveSig / partTotalVolume;

            var partSig = bodyPart.ContactArea / bodyPart.Size;
            var partVolSig = 500d / bodyPart.Size;
            var partRelVolSig = bodyPart.Class.RelativeSize / bodyPart.Size;

            var headGuess = bodyPart.Size / 650d;
            tVolRatio = sizeRatio;

            tVolRatio = System.Math.Max(0.1d, tVolRatio);
            invTVolRatio = System.Math.Max(0.1, invTVolRatio);

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNorm = 300d;
            var weaponNormRatio = tissueResult.ImplementSize / weaponNorm;
            var invWeaponNorm = weaponNorm / tissueResult.ImplementSize;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d) // body part is smaller
                {
                    if (normCaRatio > 0.5d)
                    {
                        factor += 1d;
                        if (penRatio >= 0.5d && caRatio >= 0.5d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }
                    }

                    receptors *= factor;
                    preRounded = receptors *
                            Min(Max(woundRatio, caRatio),
                            tVolRatio
                            );
                    preRounded = receptors * Max(caRatio, penRatio);
                }
                else
                {
                    if (cutRatio > 0.0d) factor += 1d;
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        preRounded = receptors * Min(invTVolRatio, penRatio, caRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.1d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0.01d && woundRatio > 0.95d) factor += 1d;
                if (!IsLowContactArea(tissueResult.ContactArea) || woundRatio > .95d)
                {
                    if (caRatio >= 1d && penRatio >= .25d)
                            factor += 1d;
                }

                receptors *= factor;
                if (tVolRatio <= 1d) // part is smaller by volume
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                    preRounded = receptors * caRatio;
                    //preRounded = receptors * caRatio * penRatio * partSig * woundRatio;
                    //preRounded = Min(receptors * partSig, preRounded);
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);
                    preRounded = receptors * invTVolRatio * penRatio;
                    preRounded = receptors * penRatio;

                    if (penRatio < 1d)
                    {
                        if (penRatio < 0.5d)
                        {
                            preRounded = receptors * (1d - penRatio);
                        }
                        else
                        {
                            preRounded = receptors * penRatio;
                        }

                        //preRounded = receptors * penRatio;
                    }
                }
                else
                {
                    preRounded = receptors
                        * System.Math.Max(woundRatio, penRatio)
                        ;
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall || caRatio < 0.05d)
            {
                preRounded = 1;
            }
            else if (headGuess < 1d)
            {
                var max = ((double)layer.Class.PainReceptors) * factor * headGuess;
                preRounded = Min(preRounded, max);
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
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
