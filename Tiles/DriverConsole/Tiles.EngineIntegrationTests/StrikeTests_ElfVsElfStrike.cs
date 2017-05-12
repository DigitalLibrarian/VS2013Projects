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
    public class StrikeTests_ElfVsElfStrike : DfContentTestBase
    {
        IAgent Elf { get; set; }
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Elf = CreateAgent("ELF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void ElfVsElf_PunchUpperBody()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.Single(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);
        }
        [TestMethod]
        public void ElfVsElf_PunchLeftFoot()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.Single(x => x.Name.Equals("left foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);
        }

        [TestMethod]
        public void ElfVsElf_PunchFinger()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("fourth finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_CompleteFracture,
                StressResult.Shear_CutThrough,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
        }

        [TestMethod]
        public void ElfVsElf_KickRightUpperLeg()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.Single(x => x.Name.Equals("right upper leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);
        }
    }
}
