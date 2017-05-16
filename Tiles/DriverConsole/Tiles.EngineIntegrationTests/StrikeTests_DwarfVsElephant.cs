using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Materials;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class StrikeTests_DwarfVsElephant : DfContentTestBase
    {
        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Attacker = CreateAgent("DWARF", "MALE");
            Defender = CreateAgent("ELEPHANT", "MALE");
        }
        
        [TestMethod]
        public void DwarfVsElephant_ScratchLeftRearFoot()
        {
            var targetBodyPart = Defender.Body.Parts.Single(p => p.Name.Equals("left rear foot"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }

        [TestMethod]
        public void DwarfVsElephant_SlashLeftRearFootWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("left rear foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);
        }

        [TestMethod]
        public void DwarfVsElephant_SlashHeadWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);
        }

        [TestMethod]
        public void DwarfVsElephant_SlashUpperBodyWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);
        }


        [TestMethod]
        public void DwarfVsElephant_StabUpperBodyWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);
        }

        [TestMethod]
        public void DwarfVsElephant_StabHeadWithWoodHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Dent);
        }

        [TestMethod]
        public void DwarfVsElephant_StabHeadWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }

        [TestMethod]
        public void DwarfVsElephant_StrikeLeftFrontFootWithSteelPick()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("left front foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);
        }

        [TestMethod]
        public void DwarfVsElephant_StrikeLeftFrontFootWithWoodPick()
        {
            var targetBodyPart = Defender.Body.Parts.FirstOrDefault(x => x.Name.Equals("left front foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut);
        }

        [TestMethod]
        public void DwarfVsElephant_StrikeLeftRearLegWithWoodPick()
        {
            var targetBodyPart = Defender.Body.Parts.FirstOrDefault(x => x.Name.Equals("left rear leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Dent);
        }
    }
}
