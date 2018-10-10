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
        public void MinotaurVsMinotar_PunchesUpperBody()
        {
            var attacker = Minotaur;
            var defender = Minotaur;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, "skin pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, "fat pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.2d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution, "muscle pain");
        }

        [TestMethod]
        public void MinotaurVsMinotar_ScratchesUpperBody()
        {
            var attacker = Minotaur;
            var defender = Minotaur;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.08d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(830, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(830, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "skin pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.08d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(830, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(830, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "fat pain");

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.06d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.08d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(50, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(830, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution, "muscle pain");
        }
    }
}
