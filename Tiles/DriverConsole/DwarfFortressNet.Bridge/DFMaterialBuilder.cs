using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;
using Df = DwarfFortressNet.RawModels;

namespace DwarfFortressNet.Bridge
{
    public class DfMaterialBuilder
    {
        ObjectDb Db { get; set; }
        Df.Inorganic Material { get; set; }

        public DfMaterialBuilder(Df.Inorganic material, ObjectDb db)
        {
            Material = material;
            Db = db;
        }

        public IMaterial Build()
        {
            var adjective = Material.AllSolidAdjective;
            if (Material.UseMaterialTemplate != null)
            {
                var mt = Db.Get<MaterialTemplate>(Material.UseMaterialTemplate);
                if (mt.AllSolidAdjective != null)
                {
                    adjective = mt.AllSolidAdjective;
                }
            }

            if (Material.AllSolidAdjective != null)
            {
                adjective = Material.AllSolidAdjective;
            }

            return new Material(adjective: adjective);
        }
    }
}
