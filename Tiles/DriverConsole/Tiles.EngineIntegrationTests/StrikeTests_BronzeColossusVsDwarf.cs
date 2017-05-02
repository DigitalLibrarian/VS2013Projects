﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class StrikeTests_BronzeColossusVsDwarf : DfContentTestBase
    {
        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Attacker = CreateAgent("COLOSSUS_BRONZE", "MALE", Vector3.Zero);
            Defender = CreateAgent("DWARF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void BronzeColossus_PunchStrikeMomentum()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = Attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            var strikeMom = move.Attacker.GetStrikeMomentum(move);
            Assert.AreEqual(236984, strikeMom, 1d);
        }

        [TestMethod]
        public void BronzeColossus_PunchContactArea()
        {
            var moveClass = Attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            Assert.AreEqual(931, moveClass.ContactArea, 1d);
        }

        [TestMethod]
        public void BronzeColossusVsDwarf_PunchRightUpperArm()
        {
            var targetBodyPart = Defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = Attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);
        }
    }
}