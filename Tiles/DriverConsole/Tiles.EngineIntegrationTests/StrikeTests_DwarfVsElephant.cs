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
            Assert.AreEqual(0.01d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.23d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.01d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(40, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(190, layerResult.Damage.DentFraction.Numerator);
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
            Assert.AreEqual(0, layerResult.PainContribution);
            
            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.07d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(750, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(6, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.3d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(3080, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(770.44, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(6, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(770.44, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(0.693d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(6930, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0d, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(310, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(310, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.7d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(220, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(310, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(700, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(700, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.03d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(20, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.Damage.DentFraction.Numerator);
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
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(146.4d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(0.01d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.36d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(60, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3690, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
            Assert.AreEqual(0.26d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2600, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.26d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2600, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.43d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.26d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1130, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.26d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
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
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.02d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.26d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(40, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);
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
            Assert.AreEqual(670, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
    }
}
