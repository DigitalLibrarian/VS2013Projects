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
    public class StrikeTests_CrabVsCrab : DfContentTestBase
    {
        IAgent Crab { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Crab = CreateAgent("CRAB", "MALE", Vector3.Zero);
        }

        [Ignore]
        [TestMethod]
        public void CrabVsCrab_SnatchesFoot()
        {
            var attacker = Crab;
            var defender = Crab;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left first foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("snatch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("chitin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2.89d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2.89d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.96d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 1d);
            Assert.AreEqual(9670, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2.89d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [Ignore]
        [TestMethod]
        public void CrabVsCrab_SnatchesLeg()
        {
            var attacker = Crab;
            var defender = Crab;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left first leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("snatch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var parts = move.Class.GetRelatedBodyParts(attacker.Body);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("chitin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.89d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(8990, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(8990, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(13.98d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(9, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.89d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(13.98d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [Ignore]
        [TestMethod]
        public void CrabVsCrab_SnatchesCephalothorax()
        {
            var attacker = Crab;
            var defender = Crab;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("cephalothorax"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("snatch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var parts = move.Class.GetRelatedBodyParts(attacker.Body);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("chitin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.48d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(4880, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(4880, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(13.98d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(6, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.48d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(13.98d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
    }
}
