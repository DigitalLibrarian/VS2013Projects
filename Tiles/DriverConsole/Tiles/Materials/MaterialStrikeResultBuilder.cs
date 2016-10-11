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
        StressMode StressMode { get; set; }

        IMaterial StrikerMaterial { get; set; }
        IMaterial StrickenMaterial { get; set; }

        public void Clear()
        {
            StrikerContactArea = 0;
            Momentum = 0;
            StressMode = Materials.StressMode.None;
            StrikerMaterial = null;
            StrickenMaterial = null;
        }

        public void SetStressMode(StressMode mode)
        {
            StressMode = mode;
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


            double contactArea = System.Math.Min(StrikerContactArea, StrickenContactArea);
            var volDamaged = LayerVolume;
            var caRatio = (StrikerContactArea / StrickenContactArea);
            if (StrikerContactArea < StrickenContactArea)
            {
                volDamaged *= caRatio;
            }

            var shearCost1 = MaterialStressCalc.ShearCost1(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier);
            var shearCost2 = MaterialStressCalc.ShearCost2(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier);
            var shearCost3 = MaterialStressCalc.ShearCost3(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier, volDamaged);
                        
            var impactCost1 = MaterialStressCalc.ImpactCost1(StrickenMaterial, volDamaged);
            var impactCost2 = MaterialStressCalc.ImpactCost2(StrickenMaterial, volDamaged);
            var impactCost3 = MaterialStressCalc.ImpactCost3(StrickenMaterial, volDamaged);


            bool yieldOnly = false;
            bool bluntBypass = false;

            double woundArea = 0;
            double thresh = -1d;
            double resultMom = -1;
            double mom = (Momentum);
            double stress = Momentum / contactArea;// / volDamaged;
            bool defeated = false;
            bool partialPuncture = false;
            var msr = MaterialStressResult.None;
            double totalUsed = 0;
            if (StressMode == Materials.StressMode.Edge)
            {
                var edgeStress = Momentum / contactArea;// / volDamaged;
                resultMom = MaterialStressCalc.ShearMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);

                var dentCost = (shearCost1);
                var cutCost = dentCost + (shearCost2);
                var defeatCost = cutCost + (shearCost3);
                shearCost1 = dentCost;
                shearCost2 = cutCost;
                shearCost3 = defeatCost;

                if (edgeStress > dentCost)
                {
                    totalUsed = dentCost;
                    msr = MaterialStressResult.Shear_Dent;
                    // strain follows hooke's law here
                    yieldOnly = true;
                    if (edgeStress > cutCost)
                    {
                        totalUsed = cutCost;
                        msr = MaterialStressResult.Shear_Cut;
                        // TODO - It looks like the attack is stopped unless at least 10% damage
                        // is done here.....  I think that is what we were trying to get at
                        // with woundRat
                        var woundRat = (stress-cutCost) / (shearCost3 - cutCost);
                        woundArea = contactArea * woundRat;
                        partialPuncture = true;
                        if ((edgeStress > defeatCost)
                            && (StrikerContactArea >= StrickenContactArea
                            && RemainingPenetration >= LayerThickness))
                        {
                            msr = MaterialStressResult.Shear_CutThrough;
                            defeated = true;
                            yieldOnly = false;
                            totalUsed = defeatCost;
                        }
                    }
                }
            }
            else
            {
                var bluntStress = Momentum / contactArea;// / volDamaged;
                resultMom = MaterialStressCalc.ImpactMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);
                // TODO - weapon deflection (soft meaty fists vs metal colossus)
                // bool deflection = layerWeight > (weaponVolume * weaponYield)/ (100d * 500d)

                var dentCost = (impactCost1);
                var cutCost = dentCost + System.Math.Max(0, impactCost2);
                var defeatCost = cutCost + System.Math.Max(0, impactCost3);

                impactCost1 = dentCost;
                impactCost2 = cutCost;
                impactCost3 = defeatCost;


                if (StrickenMaterial.ImpactStrainAtYield >= 50000 ||
                    (cutCost == 0 && defeatCost == 0))
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
                /*
                if (cutCost <= 0)
                {
                    bluntBypass = true;
                }
                 * */

                if (bluntStress > dentCost)
                {
                    totalUsed = dentCost;
                    msr = MaterialStressResult.Impact_Dent;
                    yieldOnly = true;
                    if (bluntBypass)
                    {
                        msr = MaterialStressResult.Impact_Bypass;
                    }
                    else
                    {
                        if (bluntStress > cutCost)
                        {
                            totalUsed = cutCost;
                            msr = MaterialStressResult.Impact_InitiateFracture;
                            if (bluntStress > defeatCost)
                            {
                                yieldOnly = false;
                                defeated = true;
                                totalUsed = defeatCost;
                                msr = MaterialStressResult.Impact_CompleteFracture;
                            }
                        }
                    }
                }
            }


            // For example if you punch someone in a steel helm, 
            //[IMPACT_STRAIN_AT_YIELD:940], and the punch doesn't blunt 
            //fracture the steel helm, only 940/50000=0.0188=1.88% of the momentum 
            //is passed to the skin layer


            var say = StressMode == Materials.StressMode.Blunt
                ? StrickenMaterial.ImpactStrainAtYield
                : StrickenMaterial.ShearStrainAtYield;
            var y = StressMode == Materials.StressMode.Blunt
                ? StrickenMaterial.ImpactYield
                : StrickenMaterial.ShearYield;
            var strain = CalculateStrain(say, y, stress);

            double deduction = totalUsed + (Momentum * 0.05d);
            if (defeated)
            {
                deduction += MaterialStressCalc.DefeatedLayerMomentumDeduction(
                    StrikerMaterial, StrickenMaterial,
                    StrikerMaterial.SharpnessMultiplier, volDamaged);

                if (StressMode == Materials.StressMode.Edge)
                {
                    deduction = shearCost1 * 0.1d;
                }
                else
                {
                    deduction = impactCost1 * 0.1d * contactArea;
                }

            }
            else if(yieldOnly)
            {
                var passRat = (double)strain / (double) say;
                passRat = System.Math.Min(passRat, 1d);
                passRat = System.Math.Max(passRat, 0d);
                deduction += (1d - passRat) * Momentum;
            }

            deduction = System.Math.Min(Momentum, deduction);
            resultMom = Momentum - deduction;

            if (bluntBypass || partialPuncture)
            {
                defeated = true;
                //resultMom = Momentum;
            }

            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = contactArea,
                MomentumThreshold = thresh,

                StressResult = msr,
                IsDefeated = defeated,

                ShearDentCost = shearCost1,
                ShearCutCost = shearCost2,
                ShearCutThroughCost = shearCost3,

                ImpactDentCost = impactCost1,
                ImpactInitiateFractureCost = impactCost2,
                ImpactCompleteFractureCost = impactCost3,
                Stress = stress,
                ResultMomentum = resultMom,

                WoundArea = System.Math.Max(1d, woundArea)
            };
        }


        public void SetRemainingPenetration(double penetrationLeft)
        {
            RemainingPenetration = penetrationLeft;
        }
    }
}
