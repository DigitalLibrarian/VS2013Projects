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
            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var partSig = bodyPart.Size / 650d;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;

            var multiplier = 1d;
            if (caRatio >= .5d && woundRatio > 0.5d)
            {
                multiplier += 1d;
                if (caRatio + penRatio > 1.2d)
                    multiplier += 1d;
            }

            var dmgRatio = Min(caRatio, penRatio);
            if(caRatio > 0.5d)
            {
                dmgRatio = caRatio;
            }
            if (penRatio < 0.05d)
            {
                dmgRatio = penRatio;
            }

            var preRounded = receptors * multiplier * dmgRatio;

            if (    bodyPart.Class.IsSmall 
                ||  tissueResult.ImplementWasSmall 
                ||  caRatio < 0.06d)
            {
                preRounded = 1;
            }
            else if (partSig < 1d)
            {
                var max = receptors * multiplier * partSig;
                max -= 0.5d;
                max = Max(0.5d, max);
                preRounded = Min(preRounded, max);
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
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

        private int GetPainContribution14(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
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
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            var partMass = bodyPart.Tissue.TissueLayers.Sum(t => t.Material.GetMassForUniformVolume(t.Volume));
            var wepSizeCaRatio = tissueResult.ImplementSize / tissueResult.ImplementContactArea;
            var invWebSizeCaRatio = tissueResult.ImplementContactArea / tissueResult.ImplementSize;
            var vdEstimate = layer.Volume * tissueResult.ContactAreaRatio * tissueResult.PenetrationRatio;
            tVolRatio = sizeRatio;

            tVolRatio = System.Math.Max(0.1d, tVolRatio);
            invTVolRatio = System.Math.Max(0.1, invTVolRatio);

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNormRatio = tissueResult.ImplementSize / 300d;
            var vdEstimate2 = penThick
                * (bodyPart.ContactArea * tissueResult.ContactAreaRatio);
            var vde2Ratio = vdEstimate2 / bodyPart.Size;

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
                    if (penRatio >= 1d || caRatio >= 1d)
                    {
                        if (penRatio >= .25d)
                            factor += 1d;
                    }
                }

                receptors *= factor;
                if (tVolRatio <= 1d) // part is smaller by volume
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);
                    preRounded = receptors * invTVolRatio;

                    if (penRatio < 1d)
                    {
                        if (penRatio < 0.5d)
                            preRounded = receptors * (1d - penRatio);
                        else
                        {
                            preRounded = receptors * penRatio;
                        }
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

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution13(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
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
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            tVolRatio = System.Math.Max(0.1d, tVolRatio);
            invTVolRatio = System.Math.Max(0.1, invTVolRatio);

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
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
                        System.Math.Min(System.Math.Max(woundRatio, caRatio),
                        tVolRatio
                        );
                }
                else
                {
                    if (cutRatio > 0.0d) factor += 1d;
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        //preRounded = receptors * invTVolRatio * penRatio;
                        preRounded = receptors * System.Math.Min(invTVolRatio, penRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.1d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0.01d && woundRatio > 0.95d) factor += 1d;
                if (!IsLowContactArea(tissueResult.ContactArea) || woundRatio > .95d)
                {
                    if (penRatio >= 1d || caRatio >= 1d)
                    {
                        if (penThick >= 1d && penRatio >= .25d)
                            factor += 1d;
                    }
                }

                receptors *= factor;
                if (tVolRatio <= 1d) // part is smaller by volume
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);
                    preRounded = receptors * invTVolRatio;

                    if (penRatio < 1d)
                    {
                        if (penRatio < 0.5d)
                            preRounded = receptors * (1d - penRatio);
                        else
                        {
                            preRounded = receptors * penRatio;
                        }
                    }
                }
                else
                {
                    preRounded = receptors 
                        * System.Math.Max(woundRatio, penRatio)
                        ;
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall
                || caRatio < 0.05d)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution12(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            tVolRatio = System.Math.Max(0.1d, tVolRatio);
            invTVolRatio = System.Math.Max(0.1, invTVolRatio);

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
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
                        System.Math.Min(System.Math.Max(woundRatio, caRatio),
                        tVolRatio
                        );
                }
                else
                {
                    if (cutRatio > 0.0d) factor += 1d;
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        //preRounded = receptors * invTVolRatio * penRatio;
                        preRounded = receptors * System.Math.Min(invTVolRatio, penRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.1d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0.01d) factor += 1d;
                if (!IsLowContactArea(tissueResult.ContactArea) || woundRatio > .95d)
                {
                    if (penRatio >= 1d || caRatio >= 1d)
                    {
                        if (penThick >= 1d && penRatio >= .25d)
                            factor += 1d;
                    }
                }

                receptors *= factor;
                if (tVolRatio <= 1d) // part is smaller by volume
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                    //preRounded = -1;
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);

                    //preRounded = receptors * System.Math.Min(woundRatio, penRatio);

                    if (penRatio < 1d)
                    {
                        if (penRatio < 0.5d)
                            preRounded = receptors * (1d - penRatio);
                        else
                        {
                            preRounded = receptors * penRatio;
                        }
                    }
                }
                else
                {
                    preRounded = receptors *
                            System.Math.Max(woundRatio, penRatio)
                        ;
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall
                || caRatio < 0.05d)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution11(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            tVolRatio = System.Math.Max(0.1d, tVolRatio);
            invTVolRatio = System.Math.Max(0.1, invTVolRatio);

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
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
                        System.Math.Min(System.Math.Max(woundRatio, caRatio), 
                        tVolRatio
                        );
                }
                else
                {
                    if (cutRatio > 0d) factor += 1d;
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        //preRounded = receptors * invTVolRatio * penRatio;
                        preRounded = receptors * System.Math.Min(invTVolRatio, penRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.1d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0.01d) factor += 1d;
                if (!IsLowContactArea(tissueResult.ContactArea) || woundRatio > .95d)
                {
                    if (penRatio >= 1d || caRatio >= 1d)
                    {
                        if (penThick >= 1d && penRatio >= .25d)
                            factor += 1d;
                    }
                }

                receptors *= factor;
                if (tVolRatio <= 1d) // part is smaller by volume
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                    //preRounded = -1;
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);

                    //preRounded = receptors * System.Math.Min(woundRatio, penRatio);

                    if (penRatio < 0.5d) 
                        preRounded = receptors * (1d - penRatio);
                }
                else
                {
                    preRounded = receptors *
                            System.Math.Max(woundRatio, penRatio)
                        ;
                }

            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution10(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
                {
                    if (woundRatio > 0.75d)
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
                        System.Math.Min(System.Math.Max(woundRatio, caRatio), tVolRatio);
                }
                else
                {
                    if (cutRatio > 0d) factor += 1d;
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        //preRounded = receptors * invTVolRatio * penRatio;
                        preRounded = receptors * System.Math.Min(invTVolRatio, penRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.1d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0.01d) factor += 1d;
                if (!IsLowContactArea(tissueResult.ContactArea) || woundRatio > .95d)
                {
                    if (penRatio >= 1d || caRatio >= 1d)
                    {
                        if (penThick >= 1d && penRatio >= .25d)
                            factor += 1d;
                    }
                }

                receptors *= factor;
                if (tVolRatio <= 1d) // part is smaller by volume
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                    //preRounded = -1;
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);

                    preRounded = receptors
                       * System.Math.Min(woundRatio, penRatio);

                    if (penRatio < 0.5d)
                        preRounded = receptors * (1d - penRatio);
                }
                else
                {
                    preRounded = receptors *
                            System.Math.Max(woundRatio, penRatio)
                        ;
                }

            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution9(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;
            var penThickToTVolRatio = tVolRatio / penThick;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
                {
                    //if (cutRatio > 0d) factor += 1d;
                    //if (penRatio >= 0.5d && caRatio >= 0.5d)
                    //{
                    //    if (penThick >= 1d)
                    //        factor += 1d;
                    //}

                    if (woundRatio > 0.75d)
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
                        System.Math.Min(System.Math.Max(woundRatio, caRatio), tVolRatio);
                }
                else
                {
                    if (cutRatio > 0d) factor += 1d;
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        //preRounded = receptors * invTVolRatio * penRatio;
                        preRounded = receptors * System.Math.Min(invTVolRatio, penRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.5d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0) factor += 1d;
                if (!IsLowContactArea(tissueResult.ContactArea) || woundRatio > .95d)
                {
                    if (penRatio >= 1d || caRatio >= 1d)
                    {
                        if (penThick >= 1d && penRatio >= .25d)
                            factor += 1d;
                    }
                }

                receptors *= factor;
                if (tVolRatio <= 1d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                    //preRounded = -1;
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);

                    preRounded = receptors *
                       System.Math.Min(woundRatio, penRatio);
                    if(penRatio < 1d)
                        preRounded = receptors * (1d - penRatio);
                }
                else
                {
                    preRounded = receptors *
                            System.Math.Max(woundRatio, penRatio)
                        ;
                }
                
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }
            
            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution8(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            var factor = 1d;
            double preRounded = 0;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (cutRatio > 0d) factor += 1d;
                if (tVolRatio <= 1d)
                {
                    if (penRatio >= 0.5d && caRatio >= 0.5d)
                    {
                        if (penThick >= 1d)
                            factor += 1d;
                    }

                    receptors *= factor;
                    if(tVolRatio > 0.5d)
                        preRounded = receptors *
                            System.Math.Min(System.Math.Max(woundRatio, caRatio), tVolRatio);
                    else
                    {
                        preRounded = receptors * tVolRatio;
                    }

                    if (normCaRatio < 0.5d)
                    {
                        preRounded *= 0.25d;
                    }
                }
                else
                {
                    if (IsLowContactArea(tissueResult.ContactArea))
                    {
                        receptors *= factor;
                        //preRounded = receptors * invTVolRatio * penRatio;
                        preRounded = receptors * System.Math.Min(invTVolRatio, penRatio);
                    }
                    else
                    {
                        if (penRatio >= 0.5d && caRatio >= 0.5d)
                        {
                            if (penThick >= 1d)
                                factor += 1d;
                        }

                        receptors *= factor;
                        preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    }
                }
            }
            else
            {
                if (cutRatio > 0) factor += 1d;
                if (penRatio >= 1d || caRatio >= 1d)
                {
                    if (penThick >= 1d || tVolRatio < 1d)
                        factor += 1d;
                }

                receptors *= factor;
                if (tVolRatio <= 1d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                }
                else if (invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);
                }
                else
                {
                    preRounded = receptors *
                        System.Math.Max(woundRatio, penRatio);
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution7(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = ((double)layer.Volume) / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            var momRatio = tissueResult.ResultMomentum / tissueResult.Momentum;
            tVolRatio = sizeRatio;

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;

            var weaponNormRatio = tissueResult.ImplementSize / 2400d;

            double preRounded = 0;
            var factor = 1d;

            if (cutRatio > 0) factor += 1d;
            if (penRatio >= 1d || caRatio >= 1d)
            {
                if (penThick >= 1d)// && tVolRatio < 1d)
                    factor += 1d;
            }

            receptors *= factor;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, caRatio), tVolRatio); ;

                    if (normCaRatio < 1d)
                    {
                        preRounded *= normCaRatio;
                    }
                }
                else
                {
                    preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                }
            }
            else
            {
                if (tVolRatio <= 1d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), tVolRatio);
                }
                else if(invTVolRatio > 0.25d)
                {
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, penRatio), invTVolRatio);
                }
                else
                {
                    preRounded = receptors *
                        System.Math.Max(woundRatio, penRatio);
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution6(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = (double)layer.Volume / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            tVolRatio = sizeRatio;

            var penThick = (double)layer.Thickness* tissueResult.PenetrationRatio;
            var tVolToPenThickRatio = penThick / tVolRatio;

            double preRounded = 0;
            var factor = 1d;

            if (cutRatio > 0) factor += 1d;
            if (penRatio >= 1d || caRatio >= 1d)
            {
                if (penThick >= 1d)
                    factor += 1d;
            }

            receptors *= factor;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
                {
                    //preRounded = (woundRatio) * caRatio * tVolRatio * receptors;
                    preRounded = receptors *
                        System.Math.Min(System.Math.Max(woundRatio, caRatio), tVolRatio); ;

                    if (normCaRatio < 1d)
                    {
                        preRounded *= normCaRatio;
                    }
                }
                else
                {
                    preRounded = receptors * System.Math.Max(woundRatio, invTVolRatio);
                    //preRounded = receptors * woundRatio;
                }
            }
            else
            {
                preRounded = receptors * System.Math.Max(woundRatio, penRatio);
                if (tVolRatio <= 1d)
                {
                    preRounded *= tVolRatio;
                }
                else
                {
                    if (normCaRatio < 1d)
                    {
                        preRounded *= normCaRatio;
                    }
                }
            }


            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution5(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = (double)layer.Volume / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var invWeaponRatio = tissueResult.ImplementContactArea / tissueResult.ContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);
            var normCaRatio = (tissueResult.ContactArea / 50d);
            var invNormCaRatio = (50d / tissueResult.ContactArea);
            tVolRatio = sizeRatio;

            var penThick = (double)layer.Thickness * tissueResult.PenetrationRatio;

            double preRounded = 0;
            var factor = 1d;

            if (cutRatio > 0) factor += 1d;
            if (penRatio >= 1d || caRatio >= 1d)
            {
                if(penThick >= 1d
                    //&& !IsLowContactArea(tissueResult.ContactArea) 
                    )
                    factor += 1d;
            }

            receptors *= factor;

            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (tVolRatio <= 1d)
                {
                    preRounded = (woundRatio)
                        * caRatio
                        * receptors;
                }
                else
                {
                    preRounded = receptors * System.Math.Min(woundRatio, invTVolRatio);
                    preRounded = receptors * woundRatio;
                }
            }
            else
            {
                if (IsLowContactArea(tissueResult.ContactArea))
                {
                    if (tVolRatio <= 1d)
                    {
                        preRounded = tVolRatio * receptors;
                    }
                    else
                    {
                        preRounded = invTVolRatio * receptors; 
                    }
                }
                else
                {
                    preRounded = receptors * System.Math.Max(woundRatio, penRatio);
                    //preRounded = receptors * woundRatio * penRatio;
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution3(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var penRatio = tissueResult.PenetrationRatio;
            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = (double)layer.Volume / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var bpCaRatio = tissueResult.ImplementContactArea / bodyPart.ContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            sizeRatio = tVolRatio;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);

            var bonus = 0d;
            if (woundRatio > 0.95)
            {
                bonus = 1d;
            }

            var preRounded = woundRatio * receptors * (cutRatio + dentRatio + bonus);
            //if (sizeRatio > 1.08d)
            //{
            //    preRounded = receptors * (dentRatio + cutRatio + bonus);
            //}
            //else 

            if (sizeRatio < 0.6d)
            {
                preRounded = sizeRatio * receptors;
            }


            if (IsLowContactArea(tissueResult.ImplementContactArea))
            {
                if (bpCaRatio < 1d)
                {
                    //weapon is smaller
                    preRounded = bpCaRatio * receptors;
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution2(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            var caRatio = tissueResult.ContactAreaRatio;
            var cutRatio = damage.CutFraction.AsDouble();
            var dentRatio = damage.DentFraction.AsDouble();
            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var volRatio = (double)layer.Volume / tissueResult.ImplementSize;
            var partTotalVolume = (double)(bodyPart.Tissue.TissueLayers.Sum(x => x.Volume));
            var tVolRatio = (partTotalVolume / tissueResult.ImplementSize);
            var invTVolRatio = tissueResult.ImplementSize / partTotalVolume;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var penRatio = (double)bodyPart.Thickness / (double)tissueResult.ImplementMaxPenetration;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;
            sizeRatio = tVolRatio;
            var maxRat = System.Math.Max(caRatio, tissueResult.PenetrationRatio);

            var preRounded = sizeRatio * receptors * (dentRatio + cutRatio + maxRat );
            if (sizeRatio > 1.08d)
            {
                preRounded = receptors * (dentRatio + cutRatio + caRatio);
            }
            else if (sizeRatio < 0.6d)
            {
                if(!IsLowContactArea(tissueResult.ImplementContactArea))
                    preRounded = sizeRatio * receptors;
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            return sum;
        }

        private int GetPainContribution1(IBody body, IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult tissueResult, IDamageVector damage)
        {
            if (tissueResult.PenetrationRatio == 0) return 0;
            var receptors = (double)layer.Class.PainReceptors;
            if (receptors == 0) return 0;

            double volDamaged = layer.Volume * tissueResult.ContactAreaRatio;

            var sizeRatio = bodyPart.Size / tissueResult.ImplementSize;
            var layerRatio = (double)layer.Thickness / (double)bodyPart.Tissue.TotalThickness;
            var penRatio = (double)bodyPart.Thickness / (double)tissueResult.ImplementMaxPenetration;
            var partRatio = (double)bodyPart.Class.RelativeSize / (double)body.Class.TotalBodyPartRelSize;
            var weaponRatio = tissueResult.ContactArea / tissueResult.ImplementContactArea;
            var woundRatio = tissueResult.ContactArea / bodyPart.ContactArea;

            double preRounded = receptors *
                    (
                          damage.DentFraction.AsDouble()
                        + damage.CutFraction.AsDouble()
                        + tissueResult.ContactAreaRatio
                        );

            if (tissueResult.ContactAreaRatio >= 1d)// && sizeRatio < 5d)
            {
                if (sizeRatio > 1d)
                {
                    preRounded = receptors * sizeRatio;
                }
                else if (sizeRatio < 0.9d)
                {
                    preRounded = (damage.DentFraction.AsDouble() * receptors) +
                        (sizeRatio * receptors);
                }
            }
            
            if (
                tissueResult.PenetrationRatio > 0.0
                &&
                sizeRatio < 1.08d
                )
            {
                if (IsLowContactArea(tissueResult.ContactArea))
                {
                    if (IsLowContactArea(tissueResult.ImplementContactArea))
                    {
                        if (tissueResult.ContactArea < tissueResult.ImplementContactArea)
                        {
                            preRounded = receptors * (
                                tissueResult.ContactAreaRatio
                                +
                                sizeRatio
                                );
                        }
                        else
                        {
                            preRounded = receptors * (
                                tissueResult.ContactAreaRatio
                                );
                        }

                        if (weaponRatio < 0.9d)
                        {
                            preRounded *= weaponRatio;
                        }
                    }
                    else// if (weaponRatio > 0.01d)
                    {
                        if (tissueResult.ContactArea < tissueResult.ImplementContactArea)
                        {
                            preRounded = (sizeRatio * receptors * 3d);
                        }
                        else
                        {
                            preRounded = (damage.DentFraction.AsDouble() * receptors) +
                                (sizeRatio * receptors);
                        }
                    }
                }
            }

            if (bodyPart.Class.IsSmall || tissueResult.ImplementWasSmall)
            {
                preRounded = 1d;
            }

            var sum = (int)System.Math.Round(
                preRounded, 0, MidpointRounding.AwayFromZero);

            //sum = (int)preRounded;
            //sum = System.Math.Max(1, sum);

            var max = tissueResult.PenetrationRatio < 1 ? 2 : 3;

            return System.Math.Min(max*(int)receptors, sum);
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
