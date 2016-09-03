using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterialStrikeResult
    {
        StressMode StressMode { get; }
        StressResult StressResult { get; }

        double Stress { get; }
        double Momentum { get;}
        double MomentumThreshold { get; }

        bool BreaksThrough { get; }
        
    }

    public class MaterialStrikeResult : IMaterialStrikeResult
    {
        public StressMode StressMode { get; set; }
        public StressResult StressResult { get; set; }

        public double Stress { get; set; }
        public double Momentum { get; set; }
        public double MomentumThreshold { get; set; }

        public bool BreaksThrough { get { return Momentum >= MomentumThreshold; } }
    }

    public interface IMaterialStrikeBuilder
    {
        void SetStressMode(StressMode mode);

        void SetStrikeMomentum(double momentum);
        void SetContactArea(int contactArea);

        void SetStrikerMaterial(IMaterial mat);
        void SetStrickenMaterial(IMaterial mat);

        void SetStrickenThickness(int thickness);

        IMaterialStrikeResult Build();
    }

    public class MaterialStrikeResultBuilder : IMaterialStrikeBuilder
    {
        int ContactArea { get; set; }
        double Momentum { get; set; }
        StressMode StressMode { get; set; }

        IMaterial StrikerMaterial { get; set; }
        IMaterial StrickenMaterial { get; set; }

        int StrickenThickness { get; set; }

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

        public void SetStrickenThickness(int thickness)
        {
            StrickenThickness = thickness;
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
            int yield = 0;
            int fractureForce = 0;
            int strainAtYield = 0;

            StrickenMaterial.GetModeProperties(StressMode.Edge, out yield, out fractureForce, out strainAtYield);

            int wYield = 0;
            int wFractureForce = 0;
            int wStrainAtYield = 0;

            StrikerMaterial.GetModeProperties(StressMode.Edge, out wYield, out wFractureForce, out wStrainAtYield);
            double force = Momentum;
            int contactArea = ContactArea;
            int thicknessMm = StrickenThickness;

            StressResult sResult = StressResult.Elastic;
            var threshold = MaterialStressCalc.GetEdgedBreakThreshold(
                    contactArea, 
                    wYield, wFractureForce, wStrainAtYield,
                    yield, fractureForce, strainAtYield);

            double deformDist;
            if(MaterialStressCalc.EdgedStress(force, contactArea, thicknessMm,
                    wYield, wFractureForce, wStrainAtYield,
                    yield, fractureForce, strainAtYield
                    ))
            {
                sResult = MaterialStressCalc.StressLayer(
                    force, contactArea, thicknessMm,
                    yield, fractureForce, strainAtYield,
                    out deformDist);
            }

            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                MomentumThreshold = threshold,
                StressResult = sResult,
                Stress = MaterialStressCalc.CalcStress(force, contactArea, yield, fractureForce, strainAtYield)
            };
        }

        IMaterialStrikeResult BuildBlunt()
        {
            int yield = 0;
            int fractureForce = 0;
            int strainAtYield = 0;

            StrickenMaterial.GetModeProperties(StressMode.Blunt, out yield, out fractureForce, out strainAtYield);

            int wYield = 0;
            int wFractureForce = 0;
            int wStrainAtYield = 0;

            StrikerMaterial.GetModeProperties(StressMode.Blunt, out wYield, out wFractureForce, out wStrainAtYield);

            double force = Momentum;
            int contactArea = ContactArea;
            int thicknessMm = StrickenThickness;

            double deformDist = 0;
            StressResult sResult = StressResult.Elastic;
            if (MaterialStressCalc.BluntStress(
                    force, contactArea, thicknessMm,
                    wYield, wFractureForce, wStrainAtYield,
                    yield, fractureForce, strainAtYield
                    ))
            {
                sResult = StressResult.Fracture;
                deformDist = 1d;
            }

            return new MaterialStrikeResult
            {
                StressMode = StressMode,
                Momentum = Momentum,
                StressResult = sResult
            };
        }
    }
}
