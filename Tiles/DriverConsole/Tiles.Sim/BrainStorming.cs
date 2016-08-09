using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Sim
{
    enum EntitySize
    {
        None = 0,
        Tiny = 1,
        Medium = 3,
        Large = 5,
        Huge = 7
    }

    enum MaterialState
    {
        Solid,
        Liquid,
        Gas,
        Phantasmal
    }

    interface IEntity
    {
        Vector2 Position { get; set; }
        double Temperature { get; }
        EntitySize Size { get; }

        MaterialState StateOfMatter { get; }
        IMaterialType MaterialType { get; }

        double Weight { get; }
    }

 
    
    interface ITerrainTile
    {
        MaterialState StateOfMatter { get; }
        IMaterialType MaterialType { get; }

        bool IsEnterable(MaterialState stateOfMatter);

        // TODO - figure out how to represent half filled water tiles
    }
    
    
    interface ITerrainTileFactory
    {
        ITerrainTile FromMaterialType(IMaterialType material);
    }
}
