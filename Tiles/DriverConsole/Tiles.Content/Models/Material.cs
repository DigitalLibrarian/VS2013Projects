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

        }
        public string Adjective { get; set; }

        
    //[IMPACT_YIELD:10000]
    //[IMPACT_FRACTURE:10000]
    //[IMPACT_STRAIN_AT_YIELD:1000]

        public int ImpactYield { get; set; }
        public int ImpactFracture { get; set; }
        public int ImpactStrainAtYield { get; set; }
    }
}
