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
                "",
                MapSprite(Symbol.MiscItem),
                weapon.SlotRequirements.Select(Map).ToArray(),
                weapon.Moves.Select(Map).ToArray());
        }

        public EngineItems.IItemClass Map(ContentModel.Item item)
        {
            return new ItemClass(
                string.Format("{0} {1}", item.Material.Adjective, item.NameSingular),
                MapSprite(Symbol.MiscItem),
                Map(item.Weapon),
                Map(item.Armor),
                Map(item.Material));
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
    }

}
