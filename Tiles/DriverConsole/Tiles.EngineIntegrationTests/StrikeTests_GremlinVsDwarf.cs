using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class StrikeTests_GremlinVsDwarf : DfContentTestBase
    {
        IAgent Gremlin { get; set; }
        IAgent Dwarf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Gremlin = CreateAgent("GREMLIN", "MALE", Vector3.Zero);
            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void GremlinVsDwarf_WoodDaggerSlashMomentum()
        {
            var attacker = Gremlin;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var mom = attacker.GetStrikeMomentum(move);
            Assert.AreEqual(6, mom);
        }

        [Ignore]
        [TestMethod]
        public void GremlinVsDwarf_StabFootWithWoodDagger()
        {
            var attacker = Gremlin;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Dent,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(5.45d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.21d, layerResult.ContactAreaRatio, 0.01d);
            //Assert.AreEqual(3190, layerResult.Damage.CutFraction.Numerator);
            //Assert.AreEqual(3190, layerResult.Damage.DentFraction.Numerator);
            //Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            //Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

        }

        [Ignore]
        [TestMethod]
        public void GremlinVsDwarf_SlashFootWithWoodDagger()
        {
            var attacker = Gremlin;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Dent,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(5.45d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.21d, layerResult.ContactAreaRatio, 0.01d);
            //Assert.AreEqual(3190, layerResult.Damage.CutFraction.Numerator);
            //Assert.AreEqual(3190, layerResult.Damage.DentFraction.Numerator);
            //Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            //Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

        }

        [Ignore]
        [TestMethod]
        public void GremlinVsDwarf_SlashUpperBodyWithWoodDagger()
        {
            var attacker = Gremlin;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Dent);
        }

        [Ignore]
        [TestMethod]
        public void GremlinVsDwarf_StabsUpperBodyWithWoodDagger()
        {
            var attacker = Gremlin;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Dent);
        }
    }
}
