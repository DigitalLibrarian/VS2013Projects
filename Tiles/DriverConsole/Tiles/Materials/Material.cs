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
    }
}
