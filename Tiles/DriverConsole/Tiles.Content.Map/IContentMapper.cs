using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

using ContentModel = Tiles.Content.Models;
using EngineMaterials = Tiles.Materials;
using EngineCombat = Tiles.Agents.Combat;
using EngineItems = Tiles.Items;
using EngineAgents = Tiles.Agents;

namespace Tiles.Content.Map
{
    public interface IContentMapper
    {
        EngineMaterials.IMaterial Map(ContentModel.Material material);
        
        EngineItems.IArmorClass Map(ContentModel.Armor armor);
        EngineItems.IWeaponClass Map(ContentModel.Weapon weapon);
        EngineItems.IItemClass Map(ContentModel.Item item);

        EngineItems.ArmorSlot Map(ContentModel.ArmorSlot slot);
        EngineItems.WeaponSlot Map(ContentModel.WeaponSlot slot);

        EngineCombat.ICombatMoveClass Map(ContentModel.CombatMove move);
        EngineAgents.IAgentClass Map(ContentModel.Agent agent);
    }

}
