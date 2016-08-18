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
    public class DfMaterialFactory
    {
        ObjectDb Db { get; set; }

        public DfMaterialFactory(ObjectDb db)
        {
            Db = db;
        }

        public IMaterial Create(Df.Inorganic material)
        {
            var adjective = material.AllSolidAdjective;
            if (material.UseMaterialTemplate != null)
            {
                var mt = Db.Get<MaterialTemplate>(material.UseMaterialTemplate);
                if (mt.AllSolidAdjective != null)
                {
                    adjective = mt.AllSolidAdjective;
                }
            }

            if (material.AllSolidAdjective != null)
            {
                adjective = material.AllSolidAdjective;
            }

            return new Material(adjective: adjective);
        }

        public IMaterial Create(Df.MaterialTemplate mt)
        {
            var adjective = mt.AllSolidAdjective;
            return new Material(adjective: adjective);
        }

        public IMaterial Create(Df.TissueTemplate tt)
        {
            var adjective = tt.GameName;
            return new Material(adjective: adjective);
        }
    }
}
