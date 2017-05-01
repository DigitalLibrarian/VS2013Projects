﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class MaterialStrikeResultBuilder : IMaterialStrikeResultBuilder
    {
        double StrikerContactArea { get; set; }
        double StrickenContactArea { get; set; }
        double Momentum { get; set; }
        double LayerVolume { get; set; }
        double LayerThickness { get; set; }
        double RemainingPenetration { get; set; }
        double StrikerSharpness { get; set; }
        StressMode StressMode { get; set; }

        IMaterial StrikerMaterial { get; set; }
        IMaterial StrickenMaterial { get; set; }

        IMaterialStressCalc MaterialStressCalc { get; set; }

        public MaterialStrikeResultBuilder()
        {
            MaterialStressCalc = new MaterialStressCalc();
        }

        public MaterialStrikeResultBuilder(IMaterialStressCalc materialStressCalc)
        {
            MaterialStressCalc = materialStressCalc;
        }

        #region Builder State

        public void Clear()
        {
            StrikerContactArea = 0;
            Momentum = 0;
            StrikerSharpness = 0;
            StressMode = Materials.StressMode.None;
            StrikerMaterial = null;
            StrickenMaterial = null;
        }

        public void SetStressMode(StressMode mode)
        {
            StressMode = mode;
        }

        public void SetStrikerSharpness(double sharpness)
        {
            StrikerSharpness = sharpness;
        }
        public void SetStrikeMomentum(double momentum)
        {
            Momentum = momentum;
        }

        public void SetStrikerContactArea(double contactArea)
        {
            StrikerContactArea = contactArea;
        }

        public void SetStrickenContactArea(double contactArea)
        {
            StrickenContactArea = contactArea;
        }

        public void SetStrikerMaterial(IMaterial mat)
        {
            StrikerMaterial = mat;
        }

        public void SetStrickenMaterial(IMaterial mat)
        {
            StrickenMaterial = mat;
        }

        public void SetLayerVolume(double vol)
        {
            LayerVolume = vol;
            LayerVolume = System.Math.Max(1d, LayerVolume);
        }

        public void SetLayerThickness(double thick)
        {
            LayerThickness = thick;
        }

        public void SetRemainingPenetration(double penetrationLeft)
        {
            RemainingPenetration = penetrationLeft;
        }
        #endregion

        public MaterialStrikeResult Build()
        {
            double contactArea = System.Math.Min(StrikerContactArea, StrickenContactArea);
            
            var volDamaged = LayerVolume;
            var contactAreaRatio = (StrikerContactArea / StrickenContactArea);
            if (contactAreaRatio < 1d)
            {
                // If the striker is smaller than the strikee, then we can make the damaged area spread out a tad.
                // Think about how if you punched, then you are slightly sore around the impacted area to a degree.
                contactArea += contactArea * 0.09d;
                contactAreaRatio = (contactArea / StrickenContactArea);

                volDamaged *= contactAreaRatio;
            }
            else
            {
                contactAreaRatio = 1d;
                contactArea = StrickenContactArea - 1;
            }
            volDamaged = System.Math.Max(1d, volDamaged);

            var sharpness = StrikerSharpness;
            bool bluntBypass = false;

            double resultMom = -1;
            double stress = -1;
            bool defeated = false;
            var msr = StressResult.None;
            
            var shearCost1 = MaterialStressCalc.ShearCost1(StrikerMaterial, StrickenMaterial, sharpness);
            var shearCost2 = MaterialStressCalc.ShearCost2(StrikerMaterial, StrickenMaterial, sharpness);
            var shearCost3 = MaterialStressCalc.ShearCost3(StrikerMaterial, StrickenMaterial, sharpness, LayerVolume);

            var impactCost1 = MaterialStressCalc.ImpactCost1(StrickenMaterial, LayerVolume);
            var impactCost2 = MaterialStressCalc.ImpactCost2(StrickenMaterial, LayerVolume);
            var impactCost3 = MaterialStressCalc.ImpactCost3(StrickenMaterial, LayerVolume);
            
            int strainAtYield, yield, fractureForce;
            StrickenMaterial.GetModeProperties(StressMode, out yield, out fractureForce, out strainAtYield);

            stress = Momentum / volDamaged;
            if (StressMode == Materials.StressMode.Edge)
            {
                if (contactAreaRatio < 1d)
                {
                    stress -= (stress * contactAreaRatio);
                }

                var dentCost = (shearCost1);
                var cutCost = dentCost + System.Math.Max(0, shearCost2);
                var defeatCost = cutCost + System.Math.Max(0, shearCost3);

                if (stress > dentCost)
                {
                    msr = StressResult.Shear_Dent;
                    if (stress > cutCost)
                    {
                        msr = StressResult.Shear_Cut;
                        defeated = true;
                        if (stress > defeatCost
                            && StrikerContactArea >= StrickenContactArea
                            && RemainingPenetration >= LayerThickness)
                        {
                            msr = StressResult.Shear_CutThrough;
                        }
                    }
                }
            }
            else
            {
                if (contactAreaRatio < 1d)
                {
                    stress = (Momentum - (Momentum * contactAreaRatio));
                }

                // TODO - weapon deflection (soft meaty fists vs metal colossus)
                // bool deflection = layerWeight > (weaponVolume * weaponYield)/ (100d * 500d)
                // This should be visible in action log.  Probably happens before any other layer testing
                // use copy "glances away"

                var dentCost = (impactCost1);
                var cutCost = dentCost + System.Math.Max(0, impactCost2);
                var defeatCost = cutCost + System.Math.Max(0, impactCost3);

                if (StrickenMaterial.ImpactStrainAtYield >= 50000 || (cutCost == 0 && defeatCost == 0))
                {
                    //var impactStrain = MaterialStressCalc.CalculateStrain(strainAtYield, yield, stress);
                    //bluntBypass = impactStrain > strainAtYield;
                    bluntBypass = true;
                }

                if (stress > dentCost)
                {
                    msr = StressResult.Impact_Dent;
                    if (bluntBypass)
                    {
                        msr = StressResult.Impact_Bypass;
                    }
                    else
                    {
                        if (stress > cutCost)
                        {
                            msr = StressResult.Impact_InitiateFracture;
                            if (stress > defeatCost)
                            {
                                defeated = true;
                                msr = StressResult.Impact_CompleteFracture;
                            }
                        }
                    }
                }
            }

            if (!defeated)
            {

                /* If the layer was not defeated, reduced blunt damage is passed through to the layer below depending on layer strain/denting and flexibility. 
                 * For example if you punch someone in a steel helm,  [IMPACT_STRAIN_AT_YIELD:940], and the punch doesn't blunt fracture the steel helm, only 
                 * 940/50000=0.0188=1.88% of the momentum is passed to the skin layer */
                resultMom = Momentum * ((double)strainAtYield / 50000d);
                resultMom = System.Math.Max(0d, resultMom);
            }
            else
            {
                // After a layer has been defeated via cutting or blunt fracture, the momentum is reset to the original minus a portion of the "yield" cost(s). 
                double deduction = 0, deductPercent = 0.1d;
                if (StressMode == Materials.StressMode.Edge)
                {
                    var sharpFactor = 5000d / sharpness;
                    deduction = shearCost1 * volDamaged * LayerThickness * deductPercent * sharpFactor;
                }
                else
                {
                    deduction = impactCost1 * volDamaged * deductPercent;
                }

                resultMom = Momentum - deduction;
                resultMom = System.Math.Max(0d, resultMom);
            }

            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = contactArea,
                ContactAreaRatio = contactAreaRatio,

                StressResult = msr,
                IsDefeated = defeated,

                Stress = stress,
                ResultMomentum = resultMom
            };
        }
    }
}
