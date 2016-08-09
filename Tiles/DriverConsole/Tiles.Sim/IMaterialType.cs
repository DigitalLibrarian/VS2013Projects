using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;

namespace Tiles.Sim
{
    /* TODO 
     * 
     *  1. Create a poco for material type. 
     *  2. Then create impl for the store capable of loading from a strongly typed data structure.
     *  3. Hard code a database and plug it into a world generator
     *  4. Then separately write a loader bound to a specific file format for persistence.
     *  5. Profit from the ability to persist material type databases and supply the store to a world generator along with a similar pattern for ITerrainTile generation
     *  */

    interface IMaterialType
    {
        //double SpecificHeat { get; }

        //double SolidToLiquidTemp { get; }
        //double LiquidToGasTemp { get; }

        //double Density { get; }
        //double EnergyDensity { get; }
        Color Color { get; }

        bool IsTerrainMaterial { get; }
    }

    class MaterialType : IMaterialType
    {
        public Color Color { get; set; }
        public bool IsTerrainMaterial { get; set; }
    }

    interface IMaterialTypeStore
    {
        IMaterialType Get(int id);
        IEnumerable<IMaterialType> Get(Predicate<IMaterialType> predicate);
    }

    class MaterialTypeStore : IMaterialTypeStore
    {
        Dictionary<int, IMaterialType> Data { get; set; }
        public MaterialTypeStore()
        {
            Data = new Dictionary<int, IMaterialType>
            {
                {1, new MaterialType{
                    Color = Color.DarkGray,
                    IsTerrainMaterial = false
                }},
                {2, new MaterialType{
                    Color = Color.DarkBlue,
                    IsTerrainMaterial = true
                }},
                {3, new MaterialType{
                    Color = Color.White,
                    IsTerrainMaterial = false
                }},
                {4, new MaterialType{
                    Color = Color.Green,
                    IsTerrainMaterial = true
                }},
                {5, new MaterialType{
                    Color = Color.White,
                    IsTerrainMaterial = true
                }},
            };
        }

        public MaterialTypeStore(Dictionary<int, IMaterialType> data)
        {
            Data = data;
        }

        public IMaterialType Get(int id)
        {
            if (Data.ContainsKey(id))
            {
                return Data[id];
            }
            return null;
        }

        public IEnumerable<IMaterialType> Get(Predicate<IMaterialType> predicate)
        {
            foreach (var mt in Data.Values)
            {
                if (predicate(mt))
                {
                    yield return mt;
                }
            }
        }
    }

}
