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
    public class GiantVsGiantTortoiseStrikeTests : DfContentTestBase
    {
        IAgent Giant { get; set; }
        IAgent GiantTortoise { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Giant = CreateAgent("GIANT", "MALE", Vector3.Zero);
            GiantTortoise = CreateAgent("GIANT TORTOISE", "MALE", Vector3.Zero);
        }


        [TestMethod]
        public void GiantVsGiantTortoise_PunchLeg()
        {
            //  Giant 1 punches Giant Tortoise 2 in the right front leg with his right hand, bruising the muscle!

            var attacker = Giant;
            var defender = GiantTortoise;

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("right front leg"));

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var momentum = attacker.GetStrikeMomentum(move);
            var context = new CombatMoveContext(attacker, defender, move);

            //  TODO - most dense tissue layer
            var relatedParts = move.Class.GetRelatedBodyParts(attacker.Body);
            var weaponMat = relatedParts.First().Tissue.TissueLayers.Last().Material;

            var scaleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("scale"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                momentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                weaponMat
                );

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());
            Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(scaleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);

            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }
    }

}
