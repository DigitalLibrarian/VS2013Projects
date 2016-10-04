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
    public class GiantVsGiantTortoiseStrikeTests : DfContentTestBase
    {
        IAgent Giant { get; set; }
        IAgent GiantTortoise { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Giant = CreateAgent("GIANT", "MALE", Vector3.Zero);
            GiantTortoise = CreateAgent("GIANT TORTOISE", "MALE", Vector3.Zero);
        }


        [TestMethod]
        public void GiantVsGiantTortoise_PunchLeg()
        {
            //  Giant 1 punches Giant Tortoise 2 in the right front leg with his right hand, bruising the muscle!

            var attacker = Giant;
            var defender = GiantTortoise;

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("right front leg"));

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);
            
            AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.Impact_Bypass,
                MaterialStressResult.None);
        }
    }

}
