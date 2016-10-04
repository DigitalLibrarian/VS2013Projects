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


            var volDamaged = LayerVolume;
            if (StrikerContactArea < StrickenContactArea)
            {
                volDamaged *= (StrikerContactArea / StrickenContactArea);
            }

            double contactArea = System.Math.Min(StrikerContactArea, StrickenContactArea);
            var shearCost1 = MaterialStressCalc.ShearCost1(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier) / contactArea;
            var shearCost2 = MaterialStressCalc.ShearCost2(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier) / contactArea;
            var shearCost3 = MaterialStressCalc.ShearCost3(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier,
                volDamaged);

            if (StrikerMaterial.ShearYield <= StrickenMaterial.ShearYield)
            {
                //shearCost1 = 0d;
            }

            if (StrikerMaterial.ShearFracture <= StrickenMaterial.ShearFracture)
            {
                //shearCost2 = 0d;
            }

            
            //vbca=layervolume*matdata.yield.IMPACT/100/500/10
            //vbcb=layervolume*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
            //vbcc=layervolume*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/1

            var impactCost1 = MaterialStressCalc.ImpactCost1(StrickenMaterial, volDamaged);
            var impactCost2 = MaterialStressCalc.ImpactCost2(StrickenMaterial, volDamaged);
            var impactCost3 = MaterialStressCalc.ImpactCost3(StrickenMaterial, volDamaged);

            //impactCost1 = volDamaged * StrickenMaterial.ImpactYield / 100d / 500d / 10d;
            //impactCost2 = volDamaged * (StrickenMaterial.ImpactFracture - StrickenMaterial.ImpactYield) / 100d / 500d / 10d;
            //impactCost2 = volDamaged * (StrickenMaterial.ImpactFracture - StrickenMaterial.ImpactYield) / 100d / 500d / 1d;


            bool bluntBypass = false;

            double thresh = -1d;
            double resultMom = -1;
            double mom = (Momentum);
            double stress = Momentum / volDamaged;
            bool defeated = false;
            MaterialStressResult msr = MaterialStressResult.None;
            if (StressMode == Materials.StressMode.Edge)
            {
                resultMom = MaterialStressCalc.ShearMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);

                var dentCost = (shearCost1);
                var cutCost = dentCost + (shearCost2);
                var defeatCost = cutCost + (shearCost3);
                shearCost1 = dentCost;
                shearCost2 = cutCost;
                shearCost3 = defeatCost;

                if (stress > dentCost)
                {
                    msr = MaterialStressResult.Shear_Dent;
                    // strain follows hooke's law

                    if (stress > cutCost)
                    {
                        msr = MaterialStressResult.Shear_Cut;
                        defeated = true;
                        if (stress > defeatCost)
                        {
                            msr = MaterialStressResult.Shear_CutThrough;
                        }
                    }
                }
            }
            else
            {
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

                bluntBypass = StrickenMaterial.ImpactStrainAtYield >= 50000;

                if (stress > dentCost)
                {
                    msr = MaterialStressResult.Impact_Dent;
                    if (bluntBypass)
                    {
                        if (stress >= 0)
                        {
                            msr = MaterialStressResult.Impact_Bypass;
                            //defeated = true;
                        }
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
            

            if(defeated)
            {
                var deduction = MaterialStressCalc.DefeatedLayerMomentumDeduction(
                    StrikerMaterial, StrickenMaterial,
                    StrikerMaterial.SharpnessMultiplier, LayerVolume);
                deduction = System.Math.Min(Momentum, deduction);
                resultMom = Momentum - deduction;
            }

            if (bluntBypass)
            {
                defeated = true;
            }

            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = StrikerContactArea,
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
                ResultMomentum = resultMom
            };
        }
    }
}
