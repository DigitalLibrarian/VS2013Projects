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
            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);
        }
    }
}
