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

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(180, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(180, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.06d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.01d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(10, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(180, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);


            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(750, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.3d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(3080, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.69d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(6930, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(270, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(310, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(310, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.7d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(220, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(310, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);

            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(310, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(700, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(700, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(20, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(700, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.26d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2600, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.26d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2600, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.43d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.26d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1130, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.26d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.02d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.26d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(40, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2600, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
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
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.06d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(670, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
        }
    }
}
