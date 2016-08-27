﻿using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet.IntegrationTests
{
    [TestClass]
    public class ContentParsing_AgentTests
    {
        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            DfAgentFactory = new DfAgentFactory(Store, 
                new DfAgentBuilderFactory(),
                new DfMaterialFactory(Store),
                new DfCombatMoveFactory()
                );
        }

        [TestMethod]
        public void Dwarf()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            Assert.IsNotNull(agent);
            Assert.IsNotNull(agent.Body);
            Assert.AreNotSame("dwarf", agent.Name);
            Assert.AreEqual(89, agent.Body.Parts.Count());

            Assert.AreEqual(1, agent.Symbol);

            var leftHand = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("left hand"));
            Assert.IsNotNull(leftHand);
            Assert.IsNotNull(leftHand.Tissue);
            Assert.IsTrue(leftHand.CanGrasp);
            Assert.IsFalse(leftHand.CanBeAmputated);

            Assert.IsTrue(agent.Body.Parts.Any(p => p.Moves.Any()));
            var leftHandParts = agent.Body.Parts.Where(p => p.Parent == leftHand);
            Assert.AreEqual(6, leftHandParts.Count());

            //[BODY_DETAIL_PLAN:VERTEBRATE_TISSUE_LAYERS:SKIN:FAT:MUSCLE:BONE:CARTILAGE]
	        //[BP_LAYERS:BY_CATEGORY:FINGER:ARG4:25:ARG3:25:ARG2:5:ARG1:1]
            var expectedTissueAdj = new string[]{
                "bone",
                "muscle",
                "fat",
                "skin",
                "nail"
            };

            foreach (var partName in new string[]{
                "first finger",
                "second finger",
                "third finger",
                "fourth finger",
                "thumb",
            })
            {
                var finger = leftHandParts.SingleOrDefault(p => p.NameSingular.Equals(partName));
                Assert.IsNotNull(finger);
                Assert.IsNotNull(finger.Tissue);
                Assert.IsTrue(expectedTissueAdj.SequenceEqual(finger.Tissue.Layers.Select(x => x.Material.Adjective)));

                Assert.IsFalse(finger.CanGrasp);
                Assert.IsTrue(finger.CanBeAmputated);

                Assert.IsTrue(finger.IsDigit);
                var moves = finger.Moves;
                Assert.IsNotNull(moves);
                Assert.AreEqual(1, moves.Where(m => m.Name.Equals("SCRATCH")).Count());
            }
            var leftWrist = leftHandParts.SingleOrDefault(p => p.NameSingular.Equals("left wrist"));
            Assert.IsNotNull(leftWrist);
            Assert.AreEqual(2, leftWrist.Tissue.Layers.Count());

            Assert.IsTrue(
                new string[]{"bone", "muscle"}
                .SequenceEqual(
                    leftWrist.Tissue.Layers.Select(x => x.Material.Adjective))
                );

            var brain = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("brain"));
            Assert.IsNotNull(brain);
            Assert.IsTrue(brain.IsNervous);

            var spleen = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("spleen"));
            Assert.IsNotNull(spleen);
            Assert.IsTrue(spleen.IsInternal);
            Assert.IsFalse(spleen.IsNervous);
        }

        [TestMethod]
        public void GreenDevourer()
        {
            var agent = DfAgentFactory.Create("GREEN_DEVOURER");
            Assert.IsNotNull(agent);
            Assert.AreEqual((int)'G', agent.Symbol);
            Assert.IsTrue(
                agent.Body.Parts
                .Where(p => p.Tissue.Layers.Any())
                .Any());

            Assert.IsFalse(
                agent.Body.Parts
                .Where(p => p.Tissue.Layers.Any(
                    layer => layer.Material.Adjective.Equals("bone")))
                .Any());

            Assert.IsTrue(
                agent.Body.Parts
                .Where(p => p.Tissue.Layers.Any(
                    layer => layer.Material.Adjective.Equals("skin")))
                .Any());
        }
    }
}
