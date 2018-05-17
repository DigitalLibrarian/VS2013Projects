using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Material
    {
        public Material()
        {
            StateProps = new List<MaterialStateProp>();
        }

        public string Name { get; set; }
        public string Adjective { get; set; }

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

        public IList<MaterialStateProp> StateProps { get; set; }
    }

    public class MaterialStateProp
    {
        public string PropertyName { get; set; }
        public string State { get; set; }
        public string Value { get; set; }
    }
}
