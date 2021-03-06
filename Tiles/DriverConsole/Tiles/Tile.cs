﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;
using Tiles.Structures;
using Tiles.Splatter;

namespace Tiles
{
    public enum Terrain
    { 
        None,
        Rock,
        Lava,
        Dirt,
        Mud,
        // TODO - tree and building should be moved into a different Structure type
        Tree,
        Building
    }

    public class Tile : ITile
    {
        public IList<IItem> Items { get; set; }

        public Terrain Terrain { get; set; }
        public Sprite TerrainSprite { get; set; }

        public Vector3 Index { get; private set; }

        public IAgent Agent { get; set; }
        public bool HasAgent { get { return Agent != null; } }
        
        public IStructureCell StructureCell { get; set; }
        public bool HasStructureCell { get { return StructureCell != null; } }
        public bool IsTerrainPassable { get; set; }

        public int LiquidDepth { get; set; }

        public IMaterial SplatterMaterial { get; set; }
        public SplatterAmount SplatterAmount { get; set; }

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


        public bool HasRoomForAgent
        {
            get {
                if (!HasAgent)
                {
                    if (IsTerrainPassable)
                    {
                        if (HasStructureCell)
                        {
                            if (StructureCell.CanPass)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

    }
}
