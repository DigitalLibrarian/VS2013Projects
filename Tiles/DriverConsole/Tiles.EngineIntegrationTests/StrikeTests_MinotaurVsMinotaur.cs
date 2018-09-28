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
    public class StrikeTests_MinotaurVsMinotaur : DfContentTestBase
    {
        IAgent Minotaur { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Minotaur = CreateAgent("MINOTAUR", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void MinotaurVsMinotar_ScratchesUpperBody()
        {
            var attacker = Minotaur;
            var defender = Minotaur;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }
    }
}
