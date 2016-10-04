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
    public class DwarfVsDwarfStrikeTests : DfContentTestBase
    {
        IAgent Dwarf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void DwarfVsDwarf_ToeWithSteelSwordSlash()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first toe"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var nailLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("nail"));
            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                slashMoveClass.StressMode,
                strikeMomentum,
                slashMoveClass.ContactArea,
                slashMoveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            //Assert.AreSame(BodyPartInjuryClasses.Severed, partInjury.Class);

            Assert.AreEqual(5, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(nailLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(boneLayer, tInjury.Layer);
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
            Assert.IsNotNull(moveClass);

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(move);

            var context = new CombatMoveContext(attacker, defender, move);

            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                strikeMomentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

           // Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
 
            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);
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
            Assert.IsNotNull(moveClass);

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(move);

            var context = new CombatMoveContext(attacker, defender, move);

            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                strikeMomentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            // Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);
        }
    }

}
