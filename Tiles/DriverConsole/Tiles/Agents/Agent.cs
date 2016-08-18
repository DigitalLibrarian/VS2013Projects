﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Agents.Behaviors;
namespace Tiles.Agents
{
    public class Agent : IAgent
    {
        public string Name { get; private set; }
        public IAtlas Atlas { get; private set; }
        public ISprite Sprite { get; private set; }
        public IBody Body { get; private set; }
        public Vector3 Pos { get; protected set; }
        public IInventory Inventory { get; private set; }
        public virtual bool IsPlayer { get { return false; } }
        public IAgentBehavior AgentBehavior { get; set; }
        public IOutfit Outfit { get; private set; }
        public bool IsUndead { get; set; }

        public IAgentCommandQueue CommandQueue { get; private set; }

        public bool IsDead { 
            get 
            {  
                return !Body.Parts.Where(x => x.IsLifeCritical).Any() 
                    || Body.Parts.Where(x => x.IsLifeCritical).Any(x => x.Health.OutOfHealth);
            } 
        }

        public Agent(IAtlas atlas, ISprite sprite, Vector3 pos, 
            IBody body, string name, 
            IInventory inventory, 
            IOutfit outfit,
            IAgentCommandQueue commandQueue)
        {
            Atlas = atlas;
            Sprite = sprite;
            Pos = pos;
            Body = body;
            Name = name;
            Inventory = inventory;
            Outfit = outfit;

            CommandQueue = commandQueue;
        }

        public virtual void Update(IGame game)
        {
            if (AgentBehavior != null)
            {
                AgentBehavior.Update(game, this);
            }

        }

        public bool CanMove(Vector3 delta)
        {
            if (Body.IsWrestling) return false;

            var newTile = Atlas.GetTileAtPos(Pos + delta);
            if (newTile == null) return false;
            if (newTile.HasAgent) return false;

            if (newTile.HasStructureCell)
            {
                return newTile.StructureCell.CanPass;
            }

            return newTile.IsTerrainPassable;
        }

        public bool Move(Vector3 move)
        {
            if (CanMove(move))
            {
                var startTile = Atlas.GetTileAtPos(Pos);
                startTile.RemoveAgent();
                Pos += move;
                var newTile = Atlas.GetTileAtPos(Pos);
                newTile.SetAgent(this);
                return true;
            }
            return false;
        }
    }
}
