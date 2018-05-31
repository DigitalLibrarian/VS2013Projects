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
            var move = CombatMoveFactory.BodyMove(Attacker, Defender, moveClass, targetBodyPart);

            var results = AssertTissueStrikeResults(Attacker, Defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(470, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(470, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(470, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(470, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.26d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(120, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(470, layerResult.GetDamage().DentFraction.Numerator);
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
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(50, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(750, layerResult.GetDamage().DentFraction.Numerator);
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
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.19d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.19d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.19d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = results.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.6d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.19d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1170, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1950, layerResult.GetDamage().DentFraction.Numerator);
        }
    }
}
