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
        double MaxPenetration { get; set; }
        double RemainingPenetration { get; set; }
        double StrikerSharpness { get; set; }

        private bool ImplementIsSmall { get; set; }
        private double ImplementSize { get; set; }
        private bool ImplementIsEdged { get; set; }


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
            StrikerContactArea = StrickenContactArea = 0;
            Momentum = 0;
            LayerVolume = LayerThickness = 0;
            RemainingPenetration = 0;
            StrikerSharpness = 0;
            StressMode = Materials.StressMode.None;
            StrikerMaterial = null;
            StrickenMaterial = null;
            ImplementIsSmall = false;
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

        public void SetImplementIsSmall(bool wasSmall)
        {
            ImplementIsSmall = wasSmall;
        }

        public void SetMaxPenetration(double maxPenetration)
        {
            MaxPenetration = maxPenetration;
        }

        public void SetImplementSize(double size)
        {
            ImplementSize = size;
        }

        public void SetImplementIsEdged(bool isEdged)
        {
            ImplementIsEdged = isEdged;
        }
        #endregion

        public MaterialStrikeResult Build()
        {
            var contactArea = System.Math.Min(StrikerContactArea, StrickenContactArea);
            var contactAreaRatio = (contactArea / StrickenContactArea);
            if(contactAreaRatio >= 1d)
            {
                contactAreaRatio = 1d;
                contactArea = StrickenContactArea - 1;
            }

            var msr = StressResult.None;
            bool defeated = false;
            double stress = -1, resultMom = -1, 
                sharpness = StrikerSharpness,
                volDamaged = LayerVolume * contactAreaRatio,
                preDentRatio = 1d;

            double penetrationRatio = 0d;

            int strickenStrainAtYield, strickenYield, strickenFracture;
            StrickenMaterial.GetModeProperties(StressMode, out strickenYield, out strickenFracture, out strickenStrainAtYield);

            int strikerStrainAtYield, strikerYield, strikerFracture;
            StrikerMaterial.GetModeProperties(StressMode, out strikerYield, out strikerFracture, out strikerStrainAtYield);

            var shearCost1 = MaterialStressCalc.ShearCost1(strickenYield, strikerYield, sharpness) / LayerThickness;
            var shearCost2 = MaterialStressCalc.ShearCost2(strickenFracture, strikerFracture, sharpness);
            var shearCost3 = MaterialStressCalc.ShearCost3(strickenFracture, strikerFracture, sharpness, volDamaged);
           
            var bluntCost1 = MaterialStressCalc.BluntCost1(strickenYield, LayerVolume);
            var bluntCost2 = MaterialStressCalc.BluntCost2(strickenYield, strickenFracture, LayerVolume);
            var bluntCost3 = MaterialStressCalc.BluntCost3(strickenYield, strickenFracture, LayerVolume);
            
            var sharpFudge = sharpness / 1000d;
            stress = (Momentum / volDamaged) *sharpFudge;
            if (ImplementIsEdged)
            {
                var dentCost = (shearCost1);
                var cutCost = dentCost + System.Math.Max(0, shearCost2);
                var defeatCost = cutCost + System.Math.Max(0, shearCost3);

                if (stress > dentCost)
                {
                    msr = StressResult.Shear_Dent;
                    if (stress > cutCost && RemainingPenetration > 0)
                    {
                        if (LayerThickness > RemainingPenetration)
                        {
                            penetrationRatio = RemainingPenetration / LayerThickness;
                        }
                        else
                        {
                            penetrationRatio = (stress - cutCost) / (defeatCost - cutCost);
                        }
                        penetrationRatio = System.Math.Min(1d, penetrationRatio);
                        penetrationRatio = System.Math.Max(0d, penetrationRatio);

                        if (penetrationRatio > 0.01d)
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
                        else
                        {
                            penetrationRatio = 0d;
                        }
                    }
                }
                else 
                {
                    preDentRatio = stress / (dentCost);
                }
            }
            else
            {
                if (contactAreaRatio < 1d)
                {
                    stress = (Momentum - (Momentum * contactAreaRatio));
                }

                // TODO - weapon deflection (soft meaty fists vs metal colossus)
                
                //bool deflection = layerWeight > (weaponVolume * weaponYield) / (100d * 500d);
                //var layerWeight = StrickenMaterial.GetMassForUniformVolume(LayerVolume);
                //var deflectThreshold = (ImplementSize * (double)StrikerMaterial.ImpactYield) / (100d * 500d);
                //bool deflection = layerWeight > deflectThreshold;
                // This should be visible in action log.  Probably happens before any other layer testing
                // use copy "glances away".  This probably belongs at a higher level.

                var dentCost = (bluntCost1);
                var cutCost = dentCost + System.Math.Max(0, bluntCost2);
                var defeatCost = cutCost + System.Math.Max(0, bluntCost3);

                if (stress > dentCost)
                {
                    msr = StressResult.Impact_Dent;
                    if (strickenStrainAtYield >= 50000)
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
                                penetrationRatio = 1d;
                            }
                        }
                    }
                }
                else
                {
                    preDentRatio = contactAreaRatio;
                }
            }

            if (!defeated)
            {
                /* If the layer was not defeated, reduced blunt damage is passed through to the layer below depending on layer strain/denting and flexibility. 
                 * For example if you punch someone in a steel helm,  [IMPACT_STRAIN_AT_YIELD:940], and the punch doesn't blunt fracture the steel helm, only 
                 * 940/50000=0.0188=1.88% of the momentum is passed to the skin layer */
                resultMom = Momentum * ( (double)strickenStrainAtYield / 50000d);
                resultMom = System.Math.Max(0d, resultMom);

                if(!ImplementIsEdged && msr == StressResult.None)
                {
                    // the buck stops here
                    resultMom = 0d;
                }
            }
            else
            {
                // After a layer has been defeated via cutting or blunt fracture, the momentum is reset to the original minus a portion of the "yield" cost(s). 
                double deduction = 0;
                if (ImplementIsEdged)
                {
                    var sharpFactor = 5000d / sharpness;
                    deduction = shearCost1 * volDamaged * sharpFactor / 10d;
                }
                else
                {
                    deduction = bluntCost1 / 10d;
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
                PenetrationRatio = penetrationRatio,
                PreDentRatio = preDentRatio,

                ImplementMaxPenetration =(int) MaxPenetration,
                ImplementRemainingPenetration = (int) RemainingPenetration,
                ImplementWasSmall = ImplementIsSmall,
                ImplementContactArea = StrikerContactArea,
                ImplementSize = ImplementSize,

                StressResult = msr,
                IsDefeated = defeated,

                Stress = stress,
                ResultMomentum = resultMom
            };
        }
    }
}
