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
    public class DwarfVsAlligatorStrikeTests : DfContentTestBase
    {
        IAgent Dwarf { get; set; }
        IAgent Alligator { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
            Alligator = CreateAgent("ALLIGATOR", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void DwarfVsAlligator_FootWithSteelAxeHack()
        {
            var attacker = Dwarf;
            var defender = Alligator;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left rear foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));
            Assert.IsNotNull(moveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var scaleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("scale"));
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

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(scaleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

    }

}
