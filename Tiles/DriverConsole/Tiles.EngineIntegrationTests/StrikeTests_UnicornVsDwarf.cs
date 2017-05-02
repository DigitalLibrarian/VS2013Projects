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
    public class StrikeTests_UnicornVsDwarf : DfContentTestBase
    {
        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Attacker = CreateAgent("UNICORN", "MALE");
            Defender = CreateAgent("DWARF", "MALE"); 
        }


        [TestMethod]
        public void UnicornVsDwarf_StabUpperBody()
        {
            var targetBodyPart = Defender.Body.Parts.Single(p => p.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }


        [TestMethod]
        public void UnicornVsDwarf_StabLeftUpperLeg()
        {
            var targetBodyPart = Defender.Body.Parts.Single(p => p.Name.Equals("left upper leg"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveBuilder.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }
    }
}
