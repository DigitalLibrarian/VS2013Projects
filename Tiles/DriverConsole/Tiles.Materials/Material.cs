using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class Material : IMaterial
    {
        public class MaterialStateProperty 
        {
            public string Name { get; set;}
            public string State { get; set;}
            public string Value { get; set;}
        }

        public string Name { get; set; }
        public string Adjective { get; set; }

        public IEnumerable<MaterialStateProperty> StateProps { get; set; }
        
        public Material(string name, string adjective)
            : this(name, adjective, Enumerable.Empty<MaterialStateProperty>()) { }
        public Material(string name, string adjective, IEnumerable<MaterialStateProperty> stateProps)
        {
            Name = name;
            Adjective = adjective;
            StateProps = stateProps;
        }

        public int ImpactYield { get; set; }
        public int ImpactFracture { get; set; }
        public int ImpactStrainAtYield { get; set; }

        public int ShearYield { get; set; }
        public int ShearFracture { get; set; }
        public int ShearStrainAtYield { get; set; }

        public int CompressiveYield { get; set; }
        public int CompressiveFracture { get; set; }
        public int CompressiveStrainAtYield { get; set; }

        public int TensileYield { get; set; }
        public int TensileFracture { get; set; }
        public int TensileStrainAtYield { get; set; }

        public int TorsionYield { get; set; }
        public int TorsionFracture { get; set; }
        public int TorsionStrainAtYield { get; set; }

        public int BendingYield { get; set; }
        public int BendingFracture { get; set; }
        public int BendingStrainAtYield { get; set; }

        public int SolidDensity { get; set; }
        public double SharpnessMultiplier { get; set; }

        public bool IsSoft(StressMode stressMode)
        {
            if (stressMode == StressMode.Edge)
            {
                return ShearStrainAtYield >= 50000;
            }
            else
            {
                return ImpactStrainAtYield >= 50000;
            }
        }

        public double GetMassForUniformVolume(double volumeCubicCm)
        {
            //Weight (in Γ) = Density (in kg/m3) * Volume*10 (in cm3) / 1,000,000 (cm3/m3)
            var densityKg = (double)SolidDensity;
            double volumeCubicM = (volumeCubicCm);
            return (volumeCubicM * densityKg) / 1000d;// convert kg to grams
        }

        public void GetModeProperties(StressMode contactType,
            out int yield, out int fracture, out int strainAtYield)
        {
            // TODO - 
            // Wrestling moves are special: breaking bones uses [BENDING_*] values, 
            // pinching utilizes [COMPRESSIVE_*] properties, 
            // and biting can deal [TENSILE] or [TORSION] damage depending on whether the attack is edged. 
            // Those attacks generally ignore armor.

            switch (contactType)
            {
                case StressMode.Edge:
                    yield = ShearYield;
                    fracture = ShearFracture;
                    strainAtYield = ShearStrainAtYield;
                    break;
                case StressMode.Blunt:
                    yield = ImpactYield;
                    fracture = ImpactFracture;
                    strainAtYield = ImpactStrainAtYield;
                    break;
                case StressMode.Compressive:
                    yield = CompressiveYield;
                    fracture = CompressiveFracture;
                    strainAtYield = CompressiveStrainAtYield;
                    break;
                case StressMode.Tensile:
                    yield = TensileYield;
                    fracture = TensileFracture;
                    strainAtYield = TensileStrainAtYield;
                    break;
                case StressMode.Torsion:
                    yield = TorsionYield;
                    fracture = TorsionFracture;
                    strainAtYield = TorsionStrainAtYield;
                    break;
                case StressMode.Bending:
                    yield = BendingYield;
                    fracture = BendingFracture;
                    strainAtYield = BendingStrainAtYield;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The stress mode {0} is unknown.", contactType));
            }
        }

        private bool IsAllowedState(string state)
        {
            switch (state)
            {
                case "SOLID":
                    return true;
                case "LIQUID":
                    return true;
                case "GAS":
                    return true;
                case "POWDER":
                    return true;
                case "PASTE":
                    return true;
                case "PRESSED":
                    return true;
            }
            return false;
        }

        public string GetStateProperty(string name, string state)
        {
            if (!IsAllowedState(state))
            {
                throw new ArgumentException(string.Format("state={0} is not allowed", state));
            }

            // In our list, the state may be SOLID, LIQUID, GAS, 
            // POWDER/SOLID_POWDER, PASTE/SOLID_PASTE, PRESSED/SOLID_PRESSED, 
            // ALL_SOLID, or ALL.

            var prop = StateProps.LastOrDefault(p => p.Name.Equals(name) && p.State == "ALL");
            if (prop != null)
            {
                return prop.Value;
            }
            prop = StateProps.LastOrDefault(p => p.Name.Equals(name) && p.State.Contains(state));
            if (prop != null)
            {
                return prop.Value;
            }

            throw new InvalidOperationException("Need better property resolution");
        }
    }
}
