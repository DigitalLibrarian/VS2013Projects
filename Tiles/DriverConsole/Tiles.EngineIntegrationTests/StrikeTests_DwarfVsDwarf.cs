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
    public class StrikeTests_DwarfVsDwarf : DfContentTestBase
    {
        IAgent Dwarf { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
        }

        #region Scratching
        [TestMethod]
        public void DwarfVsDwarf_ScratchFoot()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(8.15d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.31d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(3190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(8.15d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.31d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(3190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(8.15d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.07d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.31d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(240, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchLowerArm()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.22d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2270, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2270, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.22d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2270, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2270, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.22d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2270, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchUpperLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.12d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1230, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1230, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.73d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.12d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(900, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1230, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.12d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1230, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchLowerLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.14d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1430, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.79d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.14d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1140, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1430, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.14d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1430, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(1730, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1730, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(1650, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1730, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.17d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1730, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchUpperBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(8d, layerResult.ContactArea, 0.2d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(770, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(770, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(8d, layerResult.ContactArea, 0.2d);
            Assert.AreEqual(0.56d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(770, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchThirdFinger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("third finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Impact_CompleteFracture
                );

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0.84d, layerResult.PenetrationRatio, 0.1d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(8450, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.1d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }
        #endregion

        #region Punching
        [TestMethod]
        public void DwarfVsDwarf_PunchHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            Assert.AreEqual(47d, targetBodyPart.ContactArea, 0.1d);
            Assert.AreEqual(19.5d, move.Class.ContactArea, 0.1d);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(21.2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.45d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(4510, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(21.2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.45d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(4510, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(21.2d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.45d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(4510, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_PunchUpperBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(104, (int)targetBodyPart.ContactArea);
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(21.26d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.2, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(21.26d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.2, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(21.26d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.2, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchUpperArm_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(35, (int)targetBodyPart.ContactArea);
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.ContactArea);
            Assert.AreEqual(60, System.Math.Ceiling(skinInjury.ContactAreaRatio * 100));
            Assert.AreEqual(0d, skinInjury.PenetrationRatio);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_PunchLowerLeg_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(19, (int)move.Class.ContactArea);
            Assert.AreEqual(56, (int)targetBodyPart.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.ContactArea);
            Assert.AreEqual(0.38d, skinInjury.ContactAreaRatio, 0.01d);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_PunchLowerArm_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(35, (int)targetBodyPart.ContactArea);
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.ContactArea);
            Assert.AreEqual(60, System.Math.Ceiling(skinInjury.ContactAreaRatio * 100));
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchNose()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("nose"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            Assert.AreEqual(4.8d, targetBodyPart.ContactArea, 0.1d);
            Assert.AreEqual(19.5d, move.Class.ContactArea, 0.01d);

            var layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(3.8d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("cartilage", layerResult.Layer.Name);
            Assert.AreEqual(3.8d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_Punchoot()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.83d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(8320, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.83d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(8320, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.83d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(8320, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchLowerBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchFinger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_CompleteFracture,
                StressResult.Shear_CutThrough,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000d, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000d, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.EffectFraction.Numerator, 0.1d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.EffectFraction.Numerator, 0.1d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }
        #endregion

        #region Kicking
        [TestMethod]
        public void DwarfVsDwarf_KickHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.59d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5920, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.59d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5920, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.59d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5920, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
        #endregion

        #region Biting
        [TestMethod]
        public void DwarfVsDwarf_BiteUpperBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            Assert.AreEqual(9.48d, move.Class.ContactArea, 0.01d);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(980, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(980, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.06d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(60, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(980, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_BiteUpperArm()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);

            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2870, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2870, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.18d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.28d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(520, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2870, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_BiteUpperLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);

            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.15d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1560, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1560, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0.1, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.15d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(160, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1560, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_BiteLowerLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);

            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.18d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1810, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1810, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0.12, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.18d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(220, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1810, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

        }

        [TestMethod]
        public void DwarfVsDwarf_BiteHand()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.52d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5290, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5290, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0.28d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.52d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1500, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5290, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.52d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5290, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
        #endregion

        #region Daggers
        [TestMethod]
        public void DwarfVsDwarf_SlashUpperBodyWithWoodDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.05, layerResult.PenetrationRatio, 0.001);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(500, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(104.8, targetBodyPart.ContactArea, 0.1);
            Assert.AreEqual(103.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(10, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithWoodDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.05d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45, layerResult.ContactArea, 0.1);
            Assert.AreEqual(5.45, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.078d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(0.05d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(40, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabRightHandWithWoodDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2790, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45, layerResult.ContactArea, 0.1);
            Assert.AreEqual(5.45, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1, layerResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2790, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0, layerResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2790, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithSilverDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.05d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45, layerResult.ContactArea, 0.1);
            Assert.AreEqual(5.45, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(0.05d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45, layerResult.ContactArea, 0.1);
            Assert.AreEqual(5.45, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.56d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(0.05d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(290, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(510, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperBodyWithSilverDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(103.8, layerResult.WoundArea, 0.1);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(103.8, layerResult.WoundArea, 0.1);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.03d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(330, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.Inconclusive("Need example of extreme low penetration on muscle");
            //Assert.AreEqual(15, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithWoodDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.11d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1150, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1150, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.11d, layerResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0.49d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(570, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1150, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(5.45d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWithWoodDagger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(46, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.02d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(230, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(46, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(46, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
        #endregion

        #region Spears
        [TestMethod]
        public void DwarfVsDwarf_StabLowerArmWithWoodSpear()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SPEAR, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.60d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(6070, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(6070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(21.8, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.27d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(0.60d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1650, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(6070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithWoodSpear()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SPEAR, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.91d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1900, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithSilverSpear()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SPEAR, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(21.8, layerResult.WoundArea, 0.1);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(21.8, layerResult.WoundArea, 0.1);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.15d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(320, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea, 0.1);
            Assert.AreEqual(2, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithWoodSpear()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SPEAR, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(4630, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(4630, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(3, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0.17d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(820, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(4630, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.46d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }
        #endregion

        #region Pikes
        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithWoodPike()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PIKE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(4630, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(4630, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(3, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0.31d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(4630, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.46d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHandWithWoodPike()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PIKE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLowerArmWithWoodPike()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PIKE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.60d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(6070, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(6070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.47d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(0.60d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2860, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(6070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(2, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithWoodPike()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PIKE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.04d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.20d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(90, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(2070, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabFootWithWoodPike()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PIKE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.85d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(8530, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(8530, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.96d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.85d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(8200, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(8530, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLowerLegWithWoodPike()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PIKE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.38d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3820, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3820, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(21.8d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(3, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.38d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0.17d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(670, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3820, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(2, layerResult.PainContribution, 1);
        }
        #endregion

        #region Swords
        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithCopperSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(46.04d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(46.04d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(46.04d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0.62d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(6240, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperLegCopperSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(layerResult.WoundArea + 1d, targetBodyPart.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(66.1, targetBodyPart.ContactArea, 0.1);
            Assert.AreEqual(65.1, layerResult.WoundArea, 0.1);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.98d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperLegSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.39d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.Inconclusive("Need to determine proper expectation");
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLowerBodyWithSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperRightArmWithSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.Inconclusive("bone pain");
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHandSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5, layerResult.ContactArea, 0.1);
            Assert.AreEqual(19.5, targetBodyPart.ContactArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(15, layerResult.PainContribution, 4);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashToeSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first toe"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(6, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.05d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(520, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperLegWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(StressResult.Shear_Cut, layerResult.StressResult);
            Assert.AreEqual(0.63d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(6390, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(StressResult.Shear_Dent, layerResult.StressResult);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabFootWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(25, layerResult.WoundArea, 0.5);
            Assert.AreEqual(3, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.35d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3560, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(3, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLowerLegWithWoodSword()
        {
            var attacker = Dwarf; 
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.ContactAreaRatio, 0.02d);
            Assert.AreEqual(0.86d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(8240, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(9560, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(9, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.ContactAreaRatio, 0.02d);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(9560, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.22d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.5d, layerResult.ContactAreaRatio, 0.05d);
            Assert.AreEqual(55d, layerResult.ContactArea, 1d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1170, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(5, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLowerBodyWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.22d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.5d, layerResult.ContactAreaRatio, 0.05d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1170, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(5, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(46.04d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(46.04d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0.03d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(320, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Need expectation");
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHandWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left hand"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0.89d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(8980, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashLowerBodyWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(103.89d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1080, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(10, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperBodyWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            Assert.AreEqual(104.89d, targetBodyPart.ContactArea, 0.01d);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(103.89d, layerResult.ContactArea, 0.01d);
            Assert.AreEqual(0.1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1080, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(10, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashFootWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(3, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(4680, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(3, layerResult.PainContribution, 1);
        }
        #endregion

        #region Whips
        [TestMethod]
        public void DwarfVsDwarf_LashLowerArmWithWoodWhip()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_WHIP, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("lash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(300, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(300, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(300, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_LashFootWithWoodWhip()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_WHIP, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("lash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);


            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(420, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(420, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(420, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(420, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(420, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);
        }
        #endregion

        #region Picks
        [TestMethod]
        public void DwarfVsDwarf_StrikeHandWithWoodPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeUpperLegWithWoodPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.05d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(430, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Need expectation for p < 5% penetration");
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeHeadWithWoodPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.ContactArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.19d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(5, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeHandWithSteelPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "STEEL");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(18.5d, layerResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.Inconclusive("need experiment");
        }
        #endregion Picks

        #region Axes
        [TestMethod]
        public void DwarfVsDwarf_HackHandWithWoodenAxe()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_HackUpperBodyWithWoodenAxe()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(.31d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3090, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_HackUpperArmWithWoodenAxe()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.33d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3370, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_HackHandWithSilverAxe()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.92d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(9240, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.Inconclusive("Need to understand partial bone pain");
        }

        [TestMethod]
        public void DwarfVsDwarf_HackUpperArmWithSilverAxe()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.12d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1240, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Figure out how much pain this bone wound should generate");
        }

        [TestMethod]
        public void DwarfVsDwarf_HackUpperLegWithSilverAxe()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("hack"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.68d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(6860, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(13, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Figure out how much pain this bone wound should generate");
        }
        #endregion

        #region War Hammers
        [TestMethod]
        public void DwarfVsDwarf_BashUpperArmWithSilverWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.3d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(3030, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.3d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(3030, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.3d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(3030, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.3d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3030, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(3030, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(13, layerResult.PainContribution, 2);
        }

        [TestMethod]
        public void DwarfVsDwarf_BashLowerLegWithSilverWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(15, layerResult.PainContribution, 5);
        }

        [TestMethod]
        public void DwarfVsDwarf_BashLowerLegWithWoodWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
        }

        [TestMethod]
        public void DwarfVsDwarf_BashLowerLegWithTinWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "TIN");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
        }

        [TestMethod]
        public void DwarfVsDwarf_BashLowerLegWithAluminumWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "ALUMINUM");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
        }

        [TestMethod]
        public void DwarfVsDwarf_BashLowerLegWithAdamantineWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "ADAMANTINE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
        }

        [TestMethod]
        public void DwarfVsDwarf_BashLowerLegWithCopperWarHammer()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR, "COPPER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(1910, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.19d, layerResult.ContactAreaRatio, 0.01);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(1910, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
        }
        #endregion

        #region Halberds
        [TestMethod]
        public void DwarfVsDwarf_StabHandWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(18.5d, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabLeftUpperArmWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.33d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3370, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 2);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.WoundArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.15, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1550, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(5, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(.69d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(3580, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(54.5d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHandWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(18.5d, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.011d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(110, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Need expectation");
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.WoundArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.2d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(2060, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperArmWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.43d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(4370, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(4, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperBodyWithWoodHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.41, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(4090, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.9d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 1d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(0, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHandWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(18.5d, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(18.5d, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.74d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(7420, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Need to understand partial bone fracture pain");
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperArmWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.1d, layerResult.PenetrationRatio, 0.01);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(950, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Need to understand partial bone fracture pain");
        }

        [TestMethod]
        public void DwarfVsDwarf_StabHeadWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.WoundArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.WoundArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.44d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(4460, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 2);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabUpperBodyWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(54.5d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.06d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(.51d, layerResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(340, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(5, layerResult.PainContribution);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHandWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(18.5d, layerResult.WoundArea, 0.1);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(1, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.94d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(9470, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("need to understand partial bone pain");
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.WoundArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(46, layerResult.WoundArea, 0.1);
            Assert.AreEqual(7, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.55d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(5590, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.AreEqual(7, layerResult.PainContribution, 1);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperArmWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(35, layerResult.WoundArea, 0.1);
            Assert.AreEqual(4, layerResult.PainContribution, 1);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0.12d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(1280, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.WoundArea);
            Assert.Inconclusive("Need to understand partial bone pain");
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashUpperBodyWithSilverHalberd()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_HALBERD, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.9d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.9d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(layerResult.ContactArea, layerResult.WoundArea);
            Assert.AreEqual(15, layerResult.PainContribution);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.04d, layerResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.Damage.EffectFraction.Numerator);
            Assert.AreEqual(410, layerResult.Damage.CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.Damage.DentFraction.Numerator);
            Assert.AreEqual(103.9d, layerResult.ContactArea, 0.1);
            Assert.AreEqual(0d, layerResult.WoundArea);
            Assert.Inconclusive("Need data for p < 5% penetration");
        }
        #endregion

        [TestMethod]
        public void DwarfKickMomentum()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveFactory.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var mom = attacker.GetStrikeMomentum(move);

            Assert.AreEqual(82, (int)mom);
        }

        [TestMethod]
        public void DwarfWoodDaggerMomentum()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            var move = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            Assert.AreEqual(14.3, attacker.GetStrikeMomentum(move), 0.1);
        }

        // TODO - move to its own class
        [TestMethod]
        public void IronShortSwordSharpness()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "IRON");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.FirstOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveFactory.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            //"Iron has [MAX_EDGE:10000], so a no-quality iron short sword has a sharpness of 5000." - http://www.bay12forums.com/smf/index.php?topic=131995.105

            Assert.AreEqual(5000, (int)slashMove.Sharpness);
        }
    }

}
