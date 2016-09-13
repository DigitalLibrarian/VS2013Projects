using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class MaterialStrikeResultBuilder : IMaterialStrikeResultBuilder
    {
        int ContactArea { get; set; }
        double Momentum { get; set; }
        double LayerVolume { get; set; }
        StressMode StressMode { get; set; }



        IMaterial StrikerMaterial { get; set; }
        IMaterial StrickenMaterial { get; set; }

        public void Clear()
        {
            ContactArea = 0;
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

        public void SetContactArea(int contactArea)
        {
            ContactArea = contactArea;
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

After a layer has been defeated via cutting or blunt fracture, the momentum is reset to the original minus a portion of the "yield" cost(s). If the layer was not defeated, reduced blunt damage is passed through to the layer below depending on layer strain/denting and flexibility.
             * 
    */
            
            var shearCost1 = MaterialStressCalc.ShearCost1(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier);
            var shearCost2 = MaterialStressCalc.ShearCost2(StrikerMaterial, StrickenMaterial, StrikerMaterial.SharpnessMultiplier);
            var shearCost3 = MaterialStressCalc.ShearCost3(StrikerMaterial, StrickenMaterial,StrikerMaterial.SharpnessMultiplier, 
                LayerVolume);

            var impactCost1 = MaterialStressCalc.ImpactCost1(StrickenMaterial, StrikerMaterial.SharpnessMultiplier, LayerVolume);
            var impactCost2 = MaterialStressCalc.ImpactCost2(StrickenMaterial, StrikerMaterial.SharpnessMultiplier, LayerVolume);
            var impactCost3 = MaterialStressCalc.ImpactCost3(StrickenMaterial, StrikerMaterial.SharpnessMultiplier, LayerVolume);

            /*
            var thresh = StressMode == Materials.StressMode.Edge
                ? MaterialStressCalc.GetEdgedBreakThreshold(ContactArea, StrikerMaterial, StrickenMaterial)
                : MaterialStressCalc.GetBluntBreakThreshold(ContactArea, StrickenMaterial);
            */
            double thresh = -1d;
            double resultMom = -1;
            double mom = Momentum;
            bool defeated = false;
            MaterialStressResult msr = MaterialStressResult.None;
            if (StressMode == Materials.StressMode.Edge)
            {
                resultMom = MaterialStressCalc.ShearMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);

                //mom = mom - shearCost1;
                thresh = shearCost1;
                if (mom >= thresh)
                {
                    msr = MaterialStressResult.Dent_Shear;
                    //mom = mom - shearCost2;
                    thresh = shearCost2;
                    if (mom >= thresh)
                    {
                        msr = MaterialStressResult.Cut_Shear;
                        //mom = mom - shearCost3;
                        thresh = shearCost3;
                        if (mom >= thresh)
                        {
                            msr = MaterialStressResult.CutThrough_Shear;
                            defeated = true;
                        }
                    }
                }
            }
            else
            {
                resultMom = MaterialStressCalc.ImpactMomentumAfterUnbrokenRigidLayer(
                    Momentum,
                    StrickenMaterial);

                //mom = mom - impactCost1;
                thresh = impactCost1;
                if (mom >= thresh)
                {
                    msr = MaterialStressResult.Dent_Impact;
                    //mom = mom - impactCost2;
                    thresh = impactCost2;
                    if (mom >= thresh)
                    {
                        msr = MaterialStressResult.InitiateFracture_Impact;
                        //mom = mom - impactCost3;
                        thresh = impactCost3;
                        if (mom >= thresh)
                        {
                            msr = MaterialStressResult.CompleteFracture_Impact;
                            defeated = true;
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


            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = ContactArea,
                MomentumThreshold = thresh,

                StressResult = msr,
                IsDefeated = defeated,

                ShearDentCost = shearCost1,
                ShearCutCost = shearCost2,
                ShearCutThroughCost = shearCost3,

                ImpactDentCost = impactCost1,
                ImpactInitiateFractureCost = impactCost2,
                ImpactCompleteFractureCost = impactCost3,

                ResultMomentum = resultMom
            };
        }
    }
}
