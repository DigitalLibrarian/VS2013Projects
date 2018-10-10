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
    public class StrikeTests_GiantGrizzlyBearVsGiantGrizzlyBear : DfContentTestBase
    {
        IAgent Bear { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Bear = CreateAgent("GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);
        }

        [Ignore]
        [TestMethod]
        public void GiantGrizzlyBearVsGiantGrizzlyBear_ScratchesLowerBody()
        {
            var attacker = Bear;
            var defender = Bear;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.06d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(580, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(580, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(50.02d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [Ignore]
        [TestMethod]
        public void GiantGrizzlyBearVsGiantGrizzlyBear_ScratchesRearLeg()
        {
            var attacker = Bear;
            var defender = Bear;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right rear leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.06d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(580, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(580, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(50.02d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [Ignore]
        [TestMethod]
        public void GiantGrizzlyBearVsGiantGrizzlyBear_ScratchesHead()
        {
            var attacker = Bear;
            var defender = Bear;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.13d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(580, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(580, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(50.02d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [Ignore]
        [TestMethod]
        public void GiantGrizzlyBearVsGiantGrizzlyBear_BitesLowerBody()
        {
            var attacker = Bear;
            var defender = Bear;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.06d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(580, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(580, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(50.02d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }
    }
}
