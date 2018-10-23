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
    public class StrikeTests_DwarfVsElf : DfContentTestBase
    {
        IAgent Dwarf { get; set; }
        IAgent Elf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
            Elf = CreateAgent("ELF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void DwarfVsElf_StabUpperLegWithWoodSpear()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SPEAR, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.30d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3020, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3020, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(20d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1, "skin pain");

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.14d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.30d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(440, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3020, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(20d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1, "fat pain");

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.30d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(3020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(20d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution, "muscle pain");
        }

    }
}
