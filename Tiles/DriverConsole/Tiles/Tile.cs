using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Agents;
using Tiles.Items;
using Tiles.Math;
using Tiles.Structures;

namespace Tiles
{
    public enum Terrain
    { 
        None,
        Rock,
        Water,
        Lava,
        Dirt,
        Mud,
        // TODO - tree and building should be moved into a different Structure type
        Tree,
        Building
    }

    public class Tile : ITile
    {
        public Symbol Background { get; set; }
        public IList<IItem> Items { get; set; }

        public Terrain Terrain { get; set; }
        public ISprite TerrainSprite { get; set; }

        public Vector3 Index { get; private set; }

        public IAgent Agent { get; set; }
        public bool HasAgent { get { return Agent != null; } }
        
        public IStructureCell StructureCell { get; set; }
        public bool HasStructureCell { get { return StructureCell != null; } }
        public bool IsTerrainPassable { get; set; }

        public Tile(int x, int y, int z)
        {
            Index = new Vector3(x, y, z);
            Items = new List<IItem>();
            IsTerrainPassable = true;
        }
        
        public IItem GetTopItem() 
        {
            if (!Items.Any()) return null;
            return Items.Last();
        }

        public void RemoveAgent()
        {
            Agent = null;
        }

        public void SetAgent(IAgent agent)
        {
            Agent = agent;
        }
    }
}
