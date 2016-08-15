using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Structures;

namespace Tiles
{
    public interface ITile
    {
        Vector3 Index { get;}
        IList<IItem> Items { get; }
        Terrain Terrain { get; set; }
        ISprite TerrainSprite { get; set; }
        IAgent Agent { get; }
        bool HasAgent { get; }

        IStructureCell StructureCell { get; set; }
        bool HasStructureCell { get; }

        // TODO - move trees and buildings out of Terrain and into their own Structure type that is separate from the Terrain

        bool IsTerrainPassable { get; set; }

        IItem GetTopItem();
        void RemoveAgent();
        void SetAgent(IAgent agent);
    }
}
