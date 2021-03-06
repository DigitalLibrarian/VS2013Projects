﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var move = CombatMoveFactory.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void UnicornVsDwarf_StabLeftUpperLeg()
        {
            var targetBodyPart = Defender.Body.Parts.Single(p => p.Name.Equals("left upper leg"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveFactory.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(690, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(9, layerResult.PainContribution, 4, "bone pain");
        }


        [TestMethod]
        public void UnicornVsDwarf_StabRightFoot()
        {
            var targetBodyPart = Defender.Body.Parts.Single(p => p.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var moveClass = Attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveFactory.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "skin pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "fat pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "muscle pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5, layerResult.PainContribution, 2, "bone pain");
        }
    }
}
