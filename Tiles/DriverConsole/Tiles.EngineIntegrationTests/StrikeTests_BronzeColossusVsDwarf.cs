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
    public class StrikeTests_BronzeColossusVsDwarf : DfContentTestBase
    {
        IAgent Colossus { get; set; }
        IAgent Dwarf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Colossus = CreateAgent("COLOSSUS_BRONZE", "MALE", Vector3.Zero);
            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void BronzeColossus_PunchStrikeMomentum()
        {
            var attacker = Colossus;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var strikeMom = move.Attacker.GetStrikeMomentum(move);
            Assert.AreEqual(236984, strikeMom, 1d);
        }

        [TestMethod]
        public void BronzeColossus_PunchContactArea()
        {
            var moveClass = Colossus.Body.Moves.First(x => x.Name.Equals("punch"));
            Assert.AreEqual(931, moveClass.ContactArea, 1d);
        }

        [TestMethod]
        public void BronzeColossusVsDwarf_PunchRightUpperArm()
        {
            var attacker = Colossus;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);
        }
    }
}
