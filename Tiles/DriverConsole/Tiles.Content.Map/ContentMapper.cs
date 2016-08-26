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
using EngineBodies = Tiles.Bodies;
using EngineAgents = Tiles.Agents;
using Tiles.Math;

namespace Tiles.Content.Map
{
    public class ContentMapper : IContentMapper
    {
        public EngineMaterials.IMaterial Map(ContentModel.Material material)
        {
            return new EngineMaterials.Material(material.Adjective);
        }

        public EngineItems.IArmorClass Map(ContentModel.Armor armor)
        {
            if (armor == null) return null;
            return new ArmorClass(
                "",
                MapSprite(Symbol.MiscClothing),
                new EngineCombat.DamageVector(),
                armor.SlotRequirements.Select(Map).ToArray());
        }

        public EngineItems.IWeaponClass Map(ContentModel.Weapon weapon)
        {
            if (weapon == null) return null;
            return new WeaponClass(
                weapon.SlotRequirements.Select(Map).ToArray(),
                weapon.Moves.Select(Map).ToArray());
        }

        public EngineItems.IItemClass Map(ContentModel.Item item)
        {
            return new ItemClass(
                string.Format("{0} {1}", item.Material.Adjective, item.NameSingular), 
                MapSprite(Symbol.MiscItem), 
                Map(item.Material), 
                Map(item.Weapon), 
                Map(item.Armor));
        }

        public EngineItems.ArmorSlot Map(ContentModel.ArmorSlot slot)
        {
            return (EngineItems.ArmorSlot)(int)slot;
        }

        public EngineItems.WeaponSlot Map(ContentModel.WeaponSlot slot)
        {
            return (EngineItems.WeaponSlot)(int)slot;
        }

        public EngineCombat.ICombatMoveClass Map(ContentModel.CombatMove move)
        {
            return new EngineCombat.CombatMoveClass(
                move.Verb.SecondPerson,
                Map(move.Verb),
                new EngineCombat.DamageVector(),
                move.PrepTime,
                move.RecoveryTime)
                {
                    IsDefenderPartSpecific = move.IsDefenderPartSpecific,
                    IsMartialArts = move.IsMartialArts,
                    IsStrike = move.IsStrike,
                    IsItem = move.IsItem,
                    AttackerBodyStateChange = Map(move.BodyStateChange)
                };
        }

        public EngineBodies.BodyStateChange Map(ContentModel.BodyStateChange bodyStateChange)
        {
            return (EngineBodies.BodyStateChange)(int)bodyStateChange;
        }

        public Tiles.IVerb Map(ContentModel.Verb verb)
        {
            return new Verb(new Dictionary<VerbConjugation, string>
            {
                { VerbConjugation.SecondPerson, verb.SecondPerson },
                { VerbConjugation.ThirdPerson, verb.ThirdPerson}
            },
            verb.IsTransitive
            );
        }

        ISprite MapSprite(int c, Color fg = Color.White, Color bg = Color.Black)
        {
            return new Sprite(c, fg, bg);
        }

        public EngineAgents.IAgentClass Map(ContentModel.Agent agent)
        {
            return new EngineAgents.AgentClass(
                agent.Name,
                MapSprite(Symbol.Zombie),
                Map(agent.Body)
                );
        }

        public EngineBodies.IBodyClass Map(ContentModel.Body body)
        {
            var partMap = body.Parts.ToDictionary(x => x, x => Map(x) );
            foreach (var part in body.Parts)
            {
                if (part.Parent != null)
                {
                    partMap[part].Parent = partMap[part.Parent];
                }
            }

            return new EngineBodies.BodyClass(body.Parts.Select(x => partMap[x]));
        }

        EngineBodies.BodyPartClass Map(ContentModel.BodyPart bodyPart)
        {
            return new EngineBodies.BodyPartClass(
                bodyPart.NameSingular, 
                Map(bodyPart.Tissue), 
                Map(bodyPart.ArmorSlot), 
                Map(bodyPart.WeapnSlot), 
                bodyPart.Moves.Select(Map), 
                isCritical: true, 
                canGrasp: bodyPart.CanGrasp,
                canBeAmputated: bodyPart.CanBeAmputated,
                isNervous: bodyPart.IsNervous,
                isCirc: bodyPart.IsCirculatory,
                isSkeletal: bodyPart.IsSkeletal,
                isDigit: bodyPart.IsDigit,
                isBreathe: bodyPart.IsBreathe,
                isSight: bodyPart.IsSight,
                isStance: bodyPart.IsStance,
                isInternal: bodyPart.IsInternal
                );
        }

        EngineBodies.ITissueClass Map(ContentModel.Tissue tissue)
        {
            var layers = new List<EngineBodies.ITissueLayerClass>();
            foreach (var layer in tissue.Layers)
            {
                layers.Add(new EngineBodies.TissueLayerClass(
                    Map(layer.Material), 
                    layer.RelativeThickness
                    ));
            }

            return new EngineBodies.TissueClass(layers);
        }
    }

}
