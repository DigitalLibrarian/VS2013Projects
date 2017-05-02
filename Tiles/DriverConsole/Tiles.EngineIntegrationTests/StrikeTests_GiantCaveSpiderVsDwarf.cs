﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class StrikeTests_GiantCaveSpiderVsDwarf : DfContentTestBase
    {
        IAgent Spider { get; set; }
        IAgent Dwarf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Spider = CreateAgent("SPIDER_CAVE_GIANT", "MALE");
            Dwarf = CreateAgent("DWARF", "MALE");
        }

        [TestMethod]
        public void GiantCaveSpiderVsDwarf_BiteMaterial()
        {
            var attacker = Spider;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var strikeMat = attacker.GetStrikeMaterial(move);
            Assert.AreEqual("chitin", strikeMat.Name);
        }

        [TestMethod]
        public void GiantCaveSpiderVsDwarf_BiteHead()
        {
            var attacker = Spider;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }
    }
}