using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class Material : IMaterial
    {
        public string Adjective { get; private set; }
        //public int SolidDensity { get; private set; }
        //public int LiquidDensity { get; private set; }


        public Material(string adjective)
        {
            Adjective = adjective;
        }
    }
}
