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

        [TestMethod]
        public void DwarfVsDwarf_ScratchLeftFoot()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.31d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(3190, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(3190, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.31d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(3190, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(3190, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.31d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(240, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(3190, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchLeftLowerArm()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.22d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2270, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2270, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.22d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(2270, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2270, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.22d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2270, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchRightUpperLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.12d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1230, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1230, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.73d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.12d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(900, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1230, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.12d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(1230, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchRightLowerLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.14d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1430, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1430, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.79d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.14d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1140, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1430, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.14d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(1430, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.17d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(1730, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1730, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.17d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(1650, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1730, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.17d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(1730, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchUpperBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(8d, layerResult.StrikeResult.ContactArea, 0.2d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(770, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(770, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(8d, layerResult.StrikeResult.ContactArea, 0.2d);
            Assert.AreEqual(0.56d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(430, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(770, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.07d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_ScratchThirdFinger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("third finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Impact_CompleteFracture // not so sure about this one
                );

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0.84d, layerResult.StrikeResult.PenetrationRatio, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(8450, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            Assert.AreEqual(47d, targetBodyPart.GetContactArea(), 0.1d);
            Assert.AreEqual(19.5d, move.Class.ContactArea, 0.1d);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(21.2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.45d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(4510, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(21.2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.45d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(4510, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(21.2d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.45d, layerResult.StrikeResult.ContactAreaRatio, 0.1d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(4510, layerResult.GetDamage().EffectFraction.Numerator);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_PunchUpperBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("upper body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(104, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(21.26d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(0.2, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(21.26d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(0.2, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(21.26d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(0.2, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchRightUpperArm_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(35, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(60, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
            Assert.AreEqual(0d, skinInjury.StrikeResult.PenetrationRatio);
        }


        [TestMethod]
        public void DwarfVsDwarf_PunchRightLowerLeg_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(19, (int)move.Class.ContactArea);
            Assert.AreEqual(56, (int)targetBodyPart.GetContactArea());

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(0.38d, skinInjury.StrikeResult.ContactAreaRatio, 0.01d);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_PunchLeftLowerArm_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left lower arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            Assert.AreEqual(35, (int)targetBodyPart.GetContactArea());
            Assert.AreEqual(19, (int)move.Class.ContactArea);

            var skinInjury = report.BodyPartInjuries.First().TissueLayerInjuries.First();
            Assert.AreEqual(21, (int)skinInjury.StrikeResult.ContactArea);
            Assert.AreEqual(60, System.Math.Ceiling(skinInjury.StrikeResult.ContactAreaRatio * 100));
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchNose_ContactAreas()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("nose"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var report = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            Assert.AreEqual(4.8d, targetBodyPart.GetContactArea(), 0.1d);
            Assert.AreEqual(19.5d, move.Class.ContactArea, 0.01d);

            var layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(3.8d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);

            layerResult = report.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("cartilage", layerResult.Layer.Name);
            Assert.AreEqual(3.8d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
        }


        [TestMethod]
        public void DwarfVsDwarf_PunchRightFoot()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.83d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(8320, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.83d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(8320, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.83d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(8320, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchLowerBody()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.2d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(2020, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_PunchFinger()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first finger"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("punch"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_CompleteFracture,
                StressResult.Shear_CutThrough,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(10000d, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000d, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0d, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().EffectFraction.Numerator, 0.1d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().EffectFraction.Numerator, 0.1d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_KickHead()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.59d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5920, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.59d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5920, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.59d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5920, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }
        
        [TestMethod]
        public void DwarfVsDwarf_BiteRightUpperArm()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);

            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.28d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(2870, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2870, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.18d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.28d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(520, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(2870, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_BiteRightUpperLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);

            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.15d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1560, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1560, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0.1, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.15d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(160, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1560, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_BiteRightLowerLeg()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);

            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.18d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1810, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1810, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0.12, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.18d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(220, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(1810, layerResult.GetDamage().DentFraction.Numerator);

        }

        [TestMethod]
        public void DwarfVsDwarf_BiteRightHand()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.52d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(5290, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5290, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0.28d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.52d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1500, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5290, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(10.3d, layerResult.StrikeResult.ContactArea, 0.1d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.52d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(5290, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashLeftUpperLegCopperSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left upper leg"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.98d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);

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

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.51d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.51d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.51d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().DentFraction.Numerator);
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

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }


        [TestMethod]
        public void DwarfVsDwarf_SlashRightHandSteelSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
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

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("nail", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut,
                StressResult.None);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(520, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }


        [TestMethod]
        public void DwarfVsDwarf_SlashRightUpperLegWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right upper leg"));

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, slashMove,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(StressResult.Shear_Cut, layerResult.StrikeResult.StressResult);
            Assert.AreEqual(0.63d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(6390, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(StressResult.Shear_Dent, layerResult.StrikeResult.StressResult);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabRightFootWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            
            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.35d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(3560, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_StabRightLowerLegWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.StrikeResult.ContactAreaRatio, 0.02d);
            Assert.AreEqual(0.86d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(8240, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(9560, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.95d, layerResult.StrikeResult.ContactAreaRatio, 0.02d);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(9560, layerResult.GetDamage().DentFraction.Numerator);
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

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.22d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.5d, layerResult.StrikeResult.ContactAreaRatio, 0.05d);
            Assert.AreEqual(55d, layerResult.StrikeResult.ContactArea, 1d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1170, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().DentFraction.Numerator);
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

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.22d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0.5d, layerResult.StrikeResult.ContactAreaRatio, 0.05d);

            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1170, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(5190, layerResult.GetDamage().DentFraction.Numerator);
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

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(46.04d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(46.04d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(320, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashHeadWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(46.04d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(46.04d, layerResult.StrikeResult.ContactArea, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0.05d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(520, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
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

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0.1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(1080, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_SlashRightFootWithWoodSword()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right foot"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Cut);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0.46d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(4680, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }


        [TestMethod]
        public void DwarfVsDwarf_LashRightLowerArmWithWoodWhip()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right lower arm"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_WHIP, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("lash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(300, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(300, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.03d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(300, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_LashLeftFootWithWoodWhip()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("left foot"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_WHIP, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("lash"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_Bypass,
                StressResult.Impact_CompleteFracture);


            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(420, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(420, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(420, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(0.04d, layerResult.StrikeResult.ContactAreaRatio, 0.01d);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(420, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(420, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeRightHandWithWoodPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "WOOD_TEMPLATE");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_Dent);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(0.0d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(0, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);
        }

        [TestMethod]
        public void DwarfVsDwarf_StrikeRightHandWithSteelPick()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right hand"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "STEEL");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("strike"));

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);
            var result = AssertTissueStrikeResults(attacker, defender, targetBodyPart, move,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough,
                StressResult.Shear_CutThrough);

            var layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual("skin", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual("fat", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio, 0.01d);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual("muscle", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            layerResult = result.BodyPartInjuries.First().TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual("bone", layerResult.Layer.Name);
            Assert.AreEqual(1d, layerResult.StrikeResult.PenetrationRatio);
            Assert.AreEqual(1d, layerResult.StrikeResult.ContactAreaRatio);
            Assert.AreEqual(0, layerResult.GetDamage().EffectFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().CutFraction.Numerator);
            Assert.AreEqual(10000, layerResult.GetDamage().DentFraction.Numerator);

            Assert.AreEqual(18.5d, layerResult.StrikeResult.ContactArea, 0.1d);
        }

        [TestMethod]
        public void DwarfKickMomentum()
        {
            var attacker = Dwarf;
            var defender = Dwarf;

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("head"));
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("kick"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var mom = attacker.GetStrikeMomentum(move);

            Assert.AreEqual(82, (int)mom);
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

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            //"Iron has [MAX_EDGE:10000], so a no-quality iron short sword has a sharpness of 5000." - http://www.bay12forums.com/smf/index.php?topic=131995.105

            Assert.AreEqual(5000, (int)slashMove.Sharpness);
        }
    }

}
