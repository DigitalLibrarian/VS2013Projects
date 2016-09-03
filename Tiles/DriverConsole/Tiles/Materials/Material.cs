using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class Material : IMaterial
    {
        public string Name { get; set; }
        public string Adjective { get; set; }

        public Material(string name, string adjective)
        {
            Name = name;
            Adjective = adjective;
        }

        public int ImpactYield { get; set; }
        public int ImpactFracture { get; set; }
        public int ImpactStrainAtYield { get; set; }

        public int ShearYield { get; set; }
        public int ShearFracture { get; set; }
        public int ShearStrainAtYield { get; set; }

        public int SolidDensity { get; set; }

        public double GetMassForUniformVolume(int volumeCubicCm)
        {
            //Weight (in Γ) = Density (in kg/m3) * Volume*10 (in cm3) / 1,000,000 (cm3/m3)
            var density = (double)SolidDensity;
            double volumeCubicM = ((double)volumeCubicCm / 10000d);
            return  (volumeCubicM * density);
        }

        public void GetModeProperties(StressMode contactType,
            out int yield, out int fractureForce, out int strainAtYield)
        {
            // TODO - Wrestling moves are special: breaking bones uses [BENDING_*] values, pinching utilizes [COMPRESSIVE_*] properties, and biting can deal [TENSILE] or [TORSION] damage depending on whether the attack is edged. Those attacks generally ignore armor.
            switch (contactType)
            {
                case StressMode.Edge:
                    yield = ShearYield;
                    fractureForce = ShearFracture;
                    strainAtYield = ShearStrainAtYield;
                    break;
                default:
                    yield = ImpactYield;
                    fractureForce = ImpactFracture;
                    strainAtYield = ImpactStrainAtYield;
                    break;
            }
        }
    }
}
