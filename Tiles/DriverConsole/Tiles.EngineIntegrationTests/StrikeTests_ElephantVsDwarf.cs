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
    public class StrikeTests_ElephantVsDwarf : DfContentTestBase
    {
        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Attacker = CreateAgent("ELEPHANT", "MALE");
            Defender = CreateAgent("DWARF", "MALE"); 
        }

        [TestMethod]
        public void ElephantVsDwarf_GoreLeftUpperArm()
        {
            var targetBodyPart = Defender.Body.Parts.FirstOrDefault(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.First(mc => mc.Name.Equals("gore"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
        }

        [TestMethod]
        public void ElephantVsDwarf_KickLeftUpperLeg()
        {
            var targetBodyPart = Defender.Body.Parts.FirstOrDefault(x => x.Name.Equals("left upper leg"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.First(mc => mc.Name.Equals("kick"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);
            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
        }
    }
}
