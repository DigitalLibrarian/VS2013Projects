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
    public class StrikeTests_DwarfVsUnicorn : DfContentTestBase
    {
        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Attacker = CreateAgent("DWARF", "MALE");
            Defender = CreateAgent("UNICORN", "MALE");
        }


        [TestMethod]
        public void DwarfVsUnicorn_SlashRightFrontLegWithSteelSword()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            Attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, sword);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(780, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }


        [TestMethod]
        public void DwarfVsUnicorn_SlashRightFrontLegWithCopperSword()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            Attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, sword);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.18d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1830, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsUnicorn_StabFrontLegWithSteelSword()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            Attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, sword);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1370, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1370, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1370, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1370, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(640, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1370, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1370, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsUnicorn_StabLowerBodyWithSteelSword()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            Attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, sword);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1280, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1280, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1280, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1280, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.13d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(130, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1280, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsUnicorn_StabHeadWithCopperSword()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            Attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, sword);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(2860, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2860, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(2860, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2860, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.28d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2860, layerResult.GetDamage().DentFraction.Numerator);
        }


        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithSilverMace()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "SILVER");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(550, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(550, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0d);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(550, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsUnicorn_BashHoofWithSilverMace()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right front hoof"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "SILVER");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.None);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("hoof", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.21d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithCopperMace()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "COPPER");
            Attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(Attacker, Defender, moveClass, targetBodyPart, weapon);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);


            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(550, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(550, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0d);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(550, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }
    }
}
