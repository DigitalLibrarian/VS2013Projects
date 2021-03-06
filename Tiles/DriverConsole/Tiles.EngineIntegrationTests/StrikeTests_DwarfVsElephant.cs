﻿using DfNet.Raws;
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
        public void DwarfVsElephant_ScratchRearFoot()
        {
            var targetBodyPart = Defender.Body.Parts.Single(p => p.Name.Equals("left rear foot"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.02d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(200, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(200, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.23d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.02d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(40, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(200, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsElephant_SlashLeftRearFootWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("left rear foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(418d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "skin pain");
            
            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "fat pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.38d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(3860, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "muscle pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, "bone pain");
        }

        [TestMethod]
        public void DwarfVsElephant_SlashHeadWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(770.44, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(6, layerResult.PainContribution, "skin pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(770.44, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(6, layerResult.PainContribution, "fat pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.01d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(150, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(770.44, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, "muscle pain");
        }

        [TestMethod]
        public void DwarfVsElephant_SlashUpperBodyWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(15, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.13d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1390, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10, layerResult.PainContribution, "pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StabUpperBodyWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(290, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(290, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(290, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(290, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(290, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(290, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StabHeadWithWoodHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.07d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(50, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(640, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");
        }

        [TestMethod]
        public void DwarfVsElephant_SlashUpperBodyWithWoodHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10, layerResult.PainContribution, "skin pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StabHeadWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(640, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(640, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(640, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(640, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(640, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(640, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsElephant_SlashTuskWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right tusk"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("ivory", layerResult.Layer.Name);
            Assert.AreEqual(0.04d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(146.4d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(410, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, "pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StabTuskWithSteelHalberd()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right tusk"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("ivory", layerResult.Layer.Name);
            Assert.AreEqual(0.29d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.33d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(980, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3390, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, "pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StrikeLeftFrontFootWithSteelPick()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("left front foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "STEEL");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.23d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2380, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2380, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "skin pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.23d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2380, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2380, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "fat pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.23d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2380, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2380, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "muscle pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.23d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.23d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(560, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2380, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2, layerResult.PainContribution, 7, "bone pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StrikeFrontFootWithWoodPick()
        {
            var targetBodyPart = Defender.Body.Parts.FirstOrDefault(x => x.Name.Equals("left front foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.03d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.23d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(80, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2380, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2, layerResult.PainContribution, 1, "pain");
        }

        [TestMethod]
        public void DwarfVsElephant_StrikeRearLegWithWoodPick()
        {
            var targetBodyPart = Defender.Body.Parts.FirstOrDefault(x => x.Name.Equals("left rear leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.06d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(620, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");
        }
    }
}
