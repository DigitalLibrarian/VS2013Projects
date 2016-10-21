using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DfNet.Raws;
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;
using Tiles.ScreensImpl.SiteFactories;
using Tiles.Ecs;
using Tiles.Random;
using Tiles.Agents;
using Tiles.Math;
using Moq;
using Tiles.Items;
using Tiles.Agents.Combat;
using Tiles.Injuries;
using Tiles.Materials;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class StrikeTests_DwarfVsUnicorn : DfContentTestBase
    {
        IAgent Dwarf { get; set; }
        IAgent Unicorn { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
            Unicorn = CreateAgent("UNICORN", "MALE", Vector3.Zero);
        }


        [TestMethod]
        public void DwarfVsUnicorn_SlashRightFrontLegWithSteelSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Shear_CutThrough,
                MaterialStressResult.Shear_CutThrough,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut);
        }


        [TestMethod]
        public void DwarfVsUnicorn_SlashLegWithCopperSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Shear_CutThrough,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut);
        }


        [TestMethod]
        public void DwarfVsUnicorn_StabFrontLegWithSteelSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut);
        }

        [TestMethod]
        public void DwarfVsUnicorn_StabLowerBodyWithSteelSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut);
        }


        [TestMethod]
        public void DwarfVsUnicorn_StabHeadWithCopperSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut);
        }


        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithSilverMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass);
        }

        [TestMethod]
        public void DwarfVsUnicorn_BashHoofWithSilverMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front hoof"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.None);
        }


        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithCopperMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "COPPER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass);
        }


        [TestMethod]
        public void UnicornVsDwarf_StabUpperBody()
        {
            var attacker = GetNewUnicorn();
            var defender = GetNewDwarf();

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut,
                MaterialStressResult.Shear_Cut);
        }

        IAgent GetNewDwarf()
        {
            return Dwarf;
        }

        IAgent GetNewUnicorn()
        {
            return Unicorn;
        }
    }
}
