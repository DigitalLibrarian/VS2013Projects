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
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass);
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
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.None);
        }

        [TestMethod]
        public void ElfVsElf_PunchFinger()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("fourth finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Impact_CompleteFracture,
                MaterialStressResult.Shear_CutThrough,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_CompleteFracture);
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
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.None);
        }
    }
}
