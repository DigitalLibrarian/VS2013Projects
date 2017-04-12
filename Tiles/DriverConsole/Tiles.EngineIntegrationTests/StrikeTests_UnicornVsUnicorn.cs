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
    public class StrikeTests_UnicornVsUnicorn : DfContentTestBase
    {
        IAgent Unicorn { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            base.Initialize();

            Unicorn = CreateAgent("UNICORN", "MALE");
        }


        [TestMethod]
        public void UnicornVsUnicorn_StabLeftRearLeg()
        {
            var attacker = Unicorn;
            var defender = Unicorn;

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("left rear leg"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);
        }
    }
}
