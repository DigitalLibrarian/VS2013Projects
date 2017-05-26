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
using Tiles.Bodies.Injuries;
using Tiles.Materials;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class StrikeTests_UnicornVsUnicorn : DfContentTestBase
    {
        IAgent Unicorn { get; set; }

        [TestInitialize]
        public override void Initialize()
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

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);


            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.11d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.01d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().DentFraction.Numerator);
        }
    }
}
