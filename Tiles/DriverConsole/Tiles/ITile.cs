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
        Sprite TerrainSprite { get; set; }

        IStructureCell StructureCell { get; set; }
        bool HasStructureCell { get; }

        // TODO - move trees and buildings out of Terrain and into their own Structure type that is separate from the Terrain

        bool IsTerrainPassable { get; set; }

        IItem GetTopItem();

        IAgent Agent { get; }
        bool HasAgent { get; }
        bool HasRoomForAgent { get; }
        void RemoveAgent();
        void SetAgent(IAgent agent);

        /// <summary>
        /// [0 - 7] liquid depth
        /// </summary>
        int LiquidDepth { get; set; }
    }
}
