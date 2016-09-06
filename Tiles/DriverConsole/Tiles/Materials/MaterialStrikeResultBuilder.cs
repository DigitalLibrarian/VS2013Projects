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

        public IMaterialStrikeResult Build()
        {
            if (StressMode == StressMode.Edge)
            {
                return BuildEdged();
            }

            return BuildBlunt();
        }

        IMaterialStrikeResult BuildEdged()
        {
            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = ContactArea,
                MomentumThreshold = MaterialStressCalc
                   .GetEdgedBreakThreshold(ContactArea, StrikerMaterial, StrickenMaterial),
               
            };
        }

        IMaterialStrikeResult BuildBlunt()
        {
            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                ContactArea = ContactArea,

                MomentumThreshold = MaterialStressCalc.GetBluntBreakThreshold(ContactArea, StrickenMaterial),

            };
        }
    }
}
