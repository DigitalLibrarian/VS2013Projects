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
using Tiles.Bodies.Injuries;
using Tiles.Materials;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class StrikeTests_DwarfVsDwarf : DfContentTestBase
    {
        IAgent Dwarf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchLeftFoot()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }


        [TestMethod]
        public void DwarfVsDwarf_ScratchHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchUpperBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.None);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

             var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

             var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
             Assert.AreEqual("skin", layerResult.Layer.Name);
             Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

             layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
             Assert.AreEqual("fat", layerResult.Layer.Name);
             Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

             layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
             Assert.AreEqual("muscle", layerResult.Layer.Name);
             Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchHead_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(47, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(46, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
        }


        [TestMethod]
        public void DwarfVsDwarf_PunchUpperBody_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(104, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(21, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchRightUpperArm_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(35, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(60, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
            Assert.AreEqual(0d, skinInjury.StrikeResult.PenetrationRatio);
        }


        [TestMethod]
        public void DwarfVsDwarf_PunchRightLowerLeg_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(19, (int)move.Class.ContactArea);
            Assert.AreEqual(56, (int)targetBodyPart.GetContactArea());

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(38, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
        }
        
        [TestMethod]
        public void DwarfVsDwarf_PunchLeftLowerArm_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(35, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(60, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchNose_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("nose"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            Assert.AreEqual(4, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(3, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(100, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
        }


        [TestMethod]
        public void DwarfVsDwarf_PunchRightFoot()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchLowerBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchFinger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_CompleteFracture,
                StressResult.Shear_CutThrough,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var nailResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", nailResult.Layer.Name);
            Assert.AreEqual(1d, nailResult.StrikeResult.PenetrationRatio);

            var skinResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", skinResult.Layer.Name);
            Assert.AreEqual(1d, skinResult.StrikeResult.PenetrationRatio);

            var fatResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", fatResult.Layer.Name);
            Assert.AreEqual(0d, fatResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_KickHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_BiteRightUpperArm()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashLeftUpperLegCopperSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlabLowerBodyWithSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperRightArmWithSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);
        }


        [TestMethod]
        public void DwarfVsDwarf_SlashRightHandSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashToeSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first toe"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.PenetrationRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
        }


        [TestMethod]
        public void DwarfVsDwarf_SlashRightUpperLegWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(StressResult.Shear_Cut, layerResult.StrikeResult.StressResult);
            Assert.AreEqual(0.63d, layerResult.StrikeResult.PenetrationRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(StressResult.Shear_Dent, layerResult.StrikeResult.StressResult);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabRightFootWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);           
        }

        [TestMethod]
        public void DwarfVsDwarf_StabRightLowerLegWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.StrikeResult.ContactAreaRatio, 0.02d);
            Assert.AreEqual(0.86d, layerResult.StrikeResult.PenetrationRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.StrikeResult.ContactAreaRatio, 0.02d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLowerBodyWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.22d, layerResult.StrikeResult.PenetrationRatio, 0.01d);

            Assert.AreEqual(0.5d, layerResult.StrikeResult.ContactAreaRatio, 0.05d);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashLowerBodyWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashRightFootWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
        }


        [TestMethod]
        public void DwarfVsDwarf_LashRightLowerArmWithWoodWhip()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_WHIP, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("lash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);
        }

        [TestMethod]
        public void DwarfVsDwarf_LashLeftFootWithWoodWhip()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_WHIP, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("lash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeRightHandWithWoodPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeRightHandWithSteelPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "STEEL");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);
        }

        [TestMethod]
        public void DwarfKickMomentum()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var mom = attacker.GetStrikeMomentum(move);

            Assert.AreEqual(82, (int)mom);
        }

        // TODO - move to its own class
        [TestMethod]
        public void IronShortSwordSharpness()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "IRON");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.FirstOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            //"Iron has [MAX_EDGE:10000], so a no-quality iron short sword has a sharpness of 5000." - http://www.bay12forums.com/smf/index.php?topic=131995.105

            Assert.AreEqual(5000, (int)slashMove.Sharpness);
        }
    }

}
