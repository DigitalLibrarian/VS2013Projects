﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents
{
    public class AgentReaper : IAgentReaper
    {
        IAtlas Atlas { get; set; }
        IActionReporter Reporter { get; set; }
        IItemFactory ItemFactory { get; set; }

        public AgentReaper(IAtlas atlas, IActionReporter reporter)
        {
            Atlas = atlas;
            Reporter = reporter;
            ItemFactory = new ItemFactory();
        }

        public IEnumerable<IItem> Reap(IAgent agent)
        {
            foreach (var bodyPart in agent.Body.Parts)
            {
                ClearGrasps(bodyPart);
            }
            var tile = Atlas.GetTileAtPos(agent.Pos);
            tile.RemoveAgent();
            Reporter.ReportDeath(agent);
            var newItems = CreateCorpse(agent);
            foreach (var item in newItems)
            {
                tile.Items.Add(item);
            }
            return newItems;
        }

        public IEnumerable<IItem> Reap(IAgent agent, IBodyPart bodyPart)
        {
            ClearGrasps(bodyPart);

            var newItems = CreateShedBodyPart(agent, bodyPart);
            var tile = Atlas.GetTileAtPos(agent.Pos);
            foreach (var item in newItems)
            {
                tile.Items.Add(item);
            }
            return newItems;
        }

        void ClearGrasps(IBodyPart part)
        {
            if (part.IsBeingGrasped)
            {
                part.GraspedBy.StopGrasp(part);
            }

            if (part.IsGrasping)
            {
                part.StopGrasp(part.Grasped);
            }
        }

        IEnumerable<IItem> CreateCorpse(IAgent defender)
        {
            var items = defender.Inventory.GetItems().Concat(defender.Inventory.GetWorn()).ToList();
            foreach (var item in items)
            {
                defender.Inventory.RemoveItem(item);
                yield return item;
            }

            yield return ItemFactory.CreateCorpse(defender);
        }
        IEnumerable<IItem> CreateShedBodyPart(IAgent defender, IBodyPart shedPart)
        {
            if (shedPart.Weapon != null)
            {
                var weaponItem = defender.Inventory.GetWorn(shedPart.Weapon);
                if (weaponItem != null)
                {
                    defender.Inventory.RemoveItem(weaponItem);
                    yield return weaponItem;
                }
            }

            if (shedPart.Armor != null)
            {
                var armorItem = defender.Inventory.GetWorn(shedPart.Armor);
                if (armorItem != null)
                {
                    defender.Inventory.RemoveItem(armorItem);
                    yield return armorItem;
                }
            }

            yield return CreateShedLimbItem(defender, shedPart);

        }

        IItem CreateShedLimbItem(IAgent defender, IBodyPart part)
        {
            return ItemFactory.CreateShedLimb(defender, part);;
        }
    }
}
