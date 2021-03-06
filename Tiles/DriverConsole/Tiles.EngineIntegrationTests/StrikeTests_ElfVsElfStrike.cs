﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.18d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1850, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.18d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1850, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.18d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1850, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
        }

        [TestMethod]
        public void ElfVsElf_PunchLeftFoot()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.Single(x => x.Name.Equals("left foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.76d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(7630, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.76d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(7630, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.76d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(7630, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
        }

        [TestMethod]
        public void ElfVsElf_PunchFinger()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("fourth finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_CompleteFracture,
                StressResult.Shear_CutThrough,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(10000, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(10000, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
        }

        [TestMethod]
        public void ElfVsElf_KickRightUpperLeg()
        {
            var attacker = Elf;
            var defender = Elf;

            var targetBodyPart = defender.Body.Parts.Single(x => x.Name.Equals("right upper leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.38d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(3860, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.38d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(3860, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.38d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(3860, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
        }
    }
}
