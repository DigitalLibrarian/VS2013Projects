using System;
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

        double CalculateStrain(double strainAtYield, double stressAtYield, double currentStress)
        {
            var say = strainAtYield;
            var youngs = stressAtYield / say;
            return (currentStress / youngs) * 100000d;
        }

        public IMaterialStrikeResult Build()
        {
            /*
            For edged defense, there is:
1. A small momentum cost to start denting the layer material, if the weapon has a higher shear yield than the layer.
2. A small momentum cost to start cutting the layer material, if the weapon has a higher shear fracture than the layer.
3. A large momentum cost to cut through the volume of the layer material, using the ratio of weapon to layer shear fractures and the weapon's sharpness.
             * 
For blunt defense, there is:
1. A check on the yielding of the weapon vs the attack momentum, to prevent soft meaty fists from punching bronze colossuses etc.
2. A momentum cost to dent the layer volume, using the layer's impact yield.
3. A momentum cost to initiate fracture in the layer volume, using the difference between the layer's impact fracture and impact yield.
4. A momentum cost to complete fracture in the layer volume, which is the same as step 3.

After a layer has been defeated via cutting or blunt fracture, the momentum is reset to the original minus a portion of the "yield" cost(s). 
If the layer was not defeated, reduced blunt damage is passed through to the layer below depending on layer strain/denting and flexibility.
    */
            double surfaceAreaRatio = 1;
            double contactArea = System.Math.Min(StrikerContactArea, StrickenContactArea);
            
            var volDamaged = LayerVolume;
            var caRatio = (StrikerContactArea / StrickenContactArea);
            if (caRatio < 1d)
            {
                // TODO - somehow the contact area is augmented here
                contactArea += contactArea * 0.09d;
                //contactArea = System.Math.Ceiling(contactArea);
                caRatio = ((contactArea) / StrickenContactArea);

                volDamaged *= caRatio;
                surfaceAreaRatio = ((contactArea) / StrickenContactArea);
            }
            else
            {
                surfaceAreaRatio = 1d;
                caRatio = 1d;
                contactArea = StrickenContactArea - 1;
            }
            volDamaged = System.Math.Max(1d, volDamaged);

            var sharpness = StrikerSharpness;
            
            var shearCost1 = MaterialStressCalc.ShearCost1(StrikerMaterial, StrickenMaterial, sharpness);
            var shearCost2 = MaterialStressCalc.ShearCost2(StrikerMaterial, StrickenMaterial, sharpness);
            var shearCost3 = MaterialStressCalc.ShearCost3(StrikerMaterial, StrickenMaterial, sharpness, LayerVolume);
                        
            var impactCost1 = MaterialStressCalc.ImpactCost1(StrickenMaterial, LayerVolume);
            var impactCost2 = MaterialStressCalc.ImpactCost2(StrickenMaterial, LayerVolume);
            var impactCost3 = MaterialStressCalc.ImpactCost3(StrickenMaterial, LayerVolume);

            double cutFraction = 0, dentFraction = 0, effectFraction = 0;
            double maxCut = 10000, maxDent = 10000 * surfaceAreaRatio, maxEffect = 25000;
            bool bluntBypass = false;

            double woundArea = 1d;
            double thresh = -1d;
            double resultMom = -1;
            double stress = (Momentum) * caRatio;
            bool defeated = false;
            bool partialPuncture = false;
            var msr = MaterialStressResult.None;
            if (StressMode == Materials.StressMode.Edge)
            {
                stress = Momentum / volDamaged;

                if (caRatio < 1d)
                {
                    stress = (Momentum / volDamaged) - ((Momentum / volDamaged) * caRatio);
                }
                else
                {
                    stress = Momentum / volDamaged;

                }
                resultMom = MaterialStressCalc.ShearMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);

                
                var dentCost = (shearCost1);
                var cutCost = dentCost + System.Math.Max(0, shearCost2);
                var defeatCost = cutCost + System.Math.Max(0, shearCost3);
                shearCost1 = dentCost;
                shearCost2 = cutCost;
                shearCost3 = defeatCost;
                if (stress > dentCost)
                {
                    msr = MaterialStressResult.Shear_Dent;
                    // strain follows hooke's law here
                    if (stress > cutCost)
                    {
                        msr = MaterialStressResult.Shear_Cut;
                        partialPuncture = true;
                        if ((stress > defeatCost)
                            && (StrikerContactArea >= StrickenContactArea
                            && RemainingPenetration >= LayerThickness)
                            )
                        {
                            woundArea = contactArea;
                            msr = MaterialStressResult.Shear_CutThrough;
                            defeated = true;
                        }
                    }
                }
                switch (msr)
                {
                    case MaterialStressResult.Shear_Dent:
                        dentCost = ((stress - dentCost) / (cutCost - dentCost)) * maxDent;
                        break;
                    case MaterialStressResult.Shear_Cut:
                        dentCost = maxDent;
                        cutCost = ((stress - cutCost) / (defeatCost - cutCost)) * maxCut;
                        break;
                    case MaterialStressResult.Shear_CutThrough:
                        dentCost = maxDent;
                        cutCost = maxCut;
                        break;
                }

            }
            else
            {
                if (caRatio < 1d)
                {
                    stress = (Momentum - (Momentum * caRatio));
                }
                else
                {
                    stress = Momentum / volDamaged;

                }
                resultMom = MaterialStressCalc.ImpactMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);
                // TODO - weapon deflection (soft meaty fists vs metal colossus) -  NEED TEST
                // bool deflection = layerWeight > (weaponVolume * weaponYield)/ (100d * 500d)

                var dentCost = (impactCost1);
                var cutCost = dentCost + System.Math.Max(0, impactCost2);
                var defeatCost = cutCost + System.Math.Max(0, impactCost3);

                impactCost1 = dentCost;
                impactCost2 = cutCost;
                impactCost3 = defeatCost;


                if (StrickenMaterial.ImpactStrainAtYield >= 50000 
                    || (cutCost == 0 && defeatCost == 0)
                    )
                {
                    var impactSay = StressMode == Materials.StressMode.Blunt
                        ? StrickenMaterial.ImpactStrainAtYield
                        : StrickenMaterial.ShearStrainAtYield;
                    var impactY = StressMode == Materials.StressMode.Blunt
                        ? StrickenMaterial.ImpactYield
                        : StrickenMaterial.ShearYield;
                    var impactStrain = CalculateStrain(impactSay, impactY, stress);

                    bluntBypass = impactStrain > impactSay;
                }

                if (stress > dentCost)
                {
                    woundArea = contactArea;
                    msr = MaterialStressResult.Impact_Dent;
                    if (bluntBypass)
                    {
                        msr = MaterialStressResult.Impact_Bypass;
                    }
                    else
                    {
                        if (stress > cutCost)
                        {
                            msr = MaterialStressResult.Impact_InitiateFracture;
                            if (stress > defeatCost)
                            {
                                defeated = true;
                                msr = MaterialStressResult.Impact_CompleteFracture;
                            }
                        }
                    }
                }
            }

            var say = StressMode == Materials.StressMode.Blunt
                ? StrickenMaterial.ImpactStrainAtYield
                : StrickenMaterial.ShearStrainAtYield;
            var y = StressMode == Materials.StressMode.Blunt
                ? StrickenMaterial.ImpactYield
                : StrickenMaterial.ShearYield;
            var strain = CalculateStrain(say, y, stress);


            // For example if you punch someone in a steel helm, 
            //[IMPACT_STRAIN_AT_YIELD:940], and the punch doesn't blunt 
            //fracture the steel helm, only 940/50000=0.0188=1.88% of the momentum 
            //is passed to the skin layer
            if (!defeated)
            {
                resultMom = Momentum * ((double)say / 50000d);
                resultMom = System.Math.Max(0d, resultMom);
            }
            else
            {
                resultMom = Momentum;
            }

            // flat deduction of 10% of yield cost, regardless of outcome
            double deduction = 0;
            var deductPercent = 0.1d;
            if (StressMode == Materials.StressMode.Edge)
            {
                var sharpFactor = 5000d / sharpness;
                deduction = ((shearCost1*volDamaged*LayerThickness) * deductPercent) * sharpFactor;
            }
            else
            {
                deduction = ((impactCost1 * volDamaged)) * deductPercent;
            }

            resultMom -= deduction;
            resultMom = System.Math.Max(0d, resultMom);
            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = contactArea,
                SurfaceAreaRatio = surfaceAreaRatio,
                MomentumThreshold = thresh,

                StressResult = msr,
                IsDefeated = defeated || partialPuncture,

                ShearDentCost = shearCost1,
                ShearCutCost = shearCost2,
                ShearCutThroughCost = shearCost3,

                ImpactDentCost = impactCost1,
                ImpactInitiateFractureCost = impactCost2,
                ImpactCompleteFractureCost = impactCost3,
                Stress = stress,
                ResultMomentum = resultMom,

                RemainingPenetration = RemainingPenetration,
                LayerThickness = LayerThickness,
                Sharpness = StrikerSharpness,
                WoundArea = System.Math.Max(1d, woundArea)
                
            };
        }


        public void SetRemainingPenetration(double penetrationLeft)
        {
            RemainingPenetration = penetrationLeft;
        }
    }
}
