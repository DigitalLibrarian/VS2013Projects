﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class SurvivorAgentCommandPlanner : BaseAgentCommandPlanner
    {
        private static int SightRange = 25;

        public SurvivorAgentCommandPlanner(IRandom random, IAgentCommandFactory commandFactory) 
            : base(random, commandFactory, new CombatMoveDiscoverer(new CombatMoveFactory()), new PositionFinder()) { }

        public override IEnumerable<IAgentCommand> PlanBehavior(IGame game, IAgent agent)
        {
            if (agent.IsDead) return Dead(agent);

            if (!HaveWeaponEquipped(agent))
            {
                return HuntLoot(game, agent);
            }
            else
            {
                var target = FindTarget(game, agent);
                if (target != null)
                {
                    var attackMoves = AttackMoves(agent, target);
                    if (attackMoves.Any())
                    {
                        var attackMove = Random.NextElement(attackMoves.ToList());
                        return CommandFactory.MeleeAttack(agent, target, attackMove);
                    }
                    else
                    {
                        return Seek(agent, target.Pos);
                    }
                }
            }
            
            return Nothing(agent);
        }

        private IEnumerable<IAgentCommand> HuntLoot(IGame game, IAgent agent)
        {
            if (CanEquipAny(agent))
            {
                var items = GetEquippableWeapons(agent);
                if (items.Any())
                {
                    return CommandFactory.WieldWeapon(agent, items.First());
                }
                items = GetEquippableArmors(agent);
                if (items.Any())
                {
                    return CommandFactory.WearArmor(agent, items.First());
                }
            }
            else if(game.Atlas.GetTileAtPos(agent.Pos).Items.Any())
            {
                return CommandFactory.PickUpItemsOnAgentTile(game, agent);
            }

            var itemPos = FindNearestItem(game, agent);
            if (itemPos.HasValue)
            {
                return Seek(agent, itemPos.Value);
            }

            return Wander(agent);
        }

        Vector3? FindNearestItem(IGame game, IAgent agent)
        {
            return FindNearbyPos(agent.Pos, (worldPos) => game.Atlas.GetTileAtPos(worldPos).Items.Any(), halfBoxSize : SightRange);
        }

        bool CanEquipAny(IAgent agent)
        {
            var weapons = GetEquippableWeapons(agent); 
            var armors = GetEquippableArmors(agent);
            
            return weapons.Any() || armors.Any();
        }

        IEnumerable<IItem> GetEquippableWeapons(IAgent agent)
        {
            return agent.Inventory.GetItems().Where(item => agent.Outfit.CanWield(item));
        }

        IEnumerable<IItem> GetEquippableArmors(IAgent agent)
        {
            return agent.Inventory.GetItems().Where(item => agent.Outfit.CanWear(item));
        }

        bool HaveWeaponEquipped(IAgent agent)
        {
            return agent.Outfit.GetItems().Where(x => x.IsWeapon).Any();
        }

        private IAgent FindTarget(IGame game, IAgent agent)
        {
            var pos = FindNearbyPos(agent.Pos, worldPos =>
            {
                var tile = game.Atlas.GetTileAtPos(worldPos);
                if (tile.HasAgent && !tile.Agent.IsDead && tile.Agent != agent)
                {
                    return true;
                }
                return false;
            }, SightRange);

            if (pos.HasValue)
            {
                return game.Atlas.GetTileAtPos(pos.Value).Agent;
            }
            return null;
        }
    }
}
