﻿using System;
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
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.01d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.01d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "pain");
        }
    }
}
