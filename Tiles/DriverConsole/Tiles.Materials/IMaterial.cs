using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterial
    {
        string Name { get; }
        string Adjective { get; }

        int ImpactYield { get; }
        int ImpactFracture { get; }
        int ImpactStrainAtYield { get; }

        int ShearYield { get; }
        int ShearFracture { get; }
        int ShearStrainAtYield { get; }

        int CompressiveYield { get; }
        int CompressiveFracture { get; }
        int CompressiveStrainAtYield { get; }

        int TensileYield { get; }
        int TensileFracture { get; }
        int TensileStrainAtYield { get; }

        int TorsionYield { get; }
        int TorsionFracture { get; }
        int TorsionStrainAtYield { get; }

        int BendingYield { get; }
        int BendingFracture { get; }
        int BendingStrainAtYield { get; }

        /// <summary>
        /// Density of material, in solid state, measured in kg/m^3
        /// </summary>
        int SolidDensity { get; }

        double SharpnessMultiplier { get; }

        bool IsSoft(StressMode stressMode);

        /// <summary>
        /// Get mass of a solid uniform density volume
        /// </summary>
        /// <param name="volumeCubicCm"></param>
        /// <returns>mass in kg</returns>
        double GetMassForUniformVolume(double volumeCubicCm);

        void GetModeProperties(StressMode contactType, out int yield, out int fractureForce, out int strainAtYield);

        string GetStateProperty(string name, string state);
    }
}
