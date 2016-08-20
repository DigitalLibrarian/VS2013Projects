using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace DwarfFortressNet.Bridge
{
    public class DfMaterialPallete
    {
        ObjectDb Db { get; set; }

        Dictionary<string, IMaterial> Materials { get; set; }

        public DfMaterialPallete()
        {
            Db = new ObjectDb();
            Materials = new Dictionary<string, IMaterial>();
        }

        public void AddMaterial(string name, IMaterial material)
        {
            Materials[name] = material;
        }
        public IMaterial GetMaterial(string name)
        {
            return Materials[name];
        }
    }
}
