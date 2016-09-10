using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

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
                new DfColorFactory(),
                new DfMaterialFactory(Store, new DfMaterialBuilderFactory()),
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
            Assert.AreEqual(60000, agent.Body.Size);
            Assert.AreEqual(89, agent.Body.Parts.Count());

            Assert.AreEqual(1, agent.Sprite.Symbol);
            var fg = agent.Sprite.Foreground;
            Assert.AreEqual(0x00, fg.R);
            Assert.AreEqual(0x8b, fg.B);
            Assert.AreEqual(0x8b, fg.G);
            Assert.AreEqual(0xff, fg.A);
            var bg = agent.Sprite.Background;
            Assert.AreEqual(0x0, bg.R);
            Assert.AreEqual(0x0, bg.B);
            Assert.AreEqual(0x0, bg.G);
            Assert.AreEqual(0xff, bg.A);

            var upperBody = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("upper body"));
            Assert.IsNotNull(upperBody);
            Assert.IsFalse(upperBody.IsInternal);

            var liver = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("liver"));
            Assert.IsNotNull(liver);
            Assert.AreEqual(300, liver.RelativeSize);
            Assert.IsTrue(liver.IsInternal);
            Assert.AreSame(upperBody, liver.Parent);
            

            var leftHand = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("left hand"));
            Assert.IsNotNull(leftHand);
            Assert.IsNotNull(leftHand.Tissue);
            Assert.IsTrue(leftHand.CanGrasp);
            Assert.IsFalse(leftHand.CanBeAmputated);
            Assert.AreEqual(80, leftHand.RelativeSize);

            Assert.IsTrue(agent.Body.Parts.Any(p => p.Moves.Any()));
            var leftHandParts = agent.Body.Parts.Where(p => p.Parent == leftHand);
            Assert.AreEqual(6, leftHandParts.Count());

            //[BODY_DETAIL_PLAN:VERTEBRATE_TISSUE_LAYERS:SKIN:FAT:MUSCLE:BONE:CARTILAGE]
	        //[BP_LAYERS:BY_CATEGORY:FINGER:ARG4:25:ARG3:25:ARG2:5:ARG1:1]
            var expectedTissueName = new string[]{
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
                var layerNames = finger.Tissue.Layers.Select(x => x.Material.Name);
                Assert.IsTrue(expectedTissueName.SequenceEqual(layerNames),
                    string.Format("Wrong layer names {0}", string.Join(",", layerNames)));

                Assert.IsFalse(finger.CanGrasp);
                Assert.IsTrue(finger.CanBeAmputated);

                Assert.IsTrue(finger.IsDigit);
                var moves = finger.Moves;
                Assert.IsNotNull(moves);
                Assert.AreEqual(1, moves.Where(m => m.Name.Equals("SCRATCH")).Count());
                var move = moves.Single(m => m.Name.Equals("SCRATCH"));
                Assert.AreEqual(ContactType.Other, move.ContactType);
            }
            var leftWrist = leftHandParts.SingleOrDefault(p => p.NameSingular.Equals("left wrist"));
            Assert.IsNotNull(leftWrist);
            Assert.AreEqual(2, leftWrist.Tissue.Layers.Count());

            Assert.IsTrue(
                new string[]{"bone", "muscle"}
                .SequenceEqual(
                    leftWrist.Tissue.Layers.Select(x => x.Material.Adjective))
                );

            var leftWristBoneLayer = leftWrist.Tissue.Layers.Single(x => x.Material.Name.Equals("bone"));
            Assert.AreEqual(200000, leftWristBoneLayer.Material.ImpactYield);


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
            Assert.AreEqual((int)'G', agent.Sprite.Symbol);
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

        [TestMethod]
        public void SoldierAntman()
        {
            var agent = DfAgentFactory.Create("ANT_MAN", "SOLDIER");
            Assert.IsNotNull(agent);
        }

        [TestMethod]
        public void CaveFloater()
        {
            var agent = DfAgentFactory.Create("CAVE_FLOATER");
            Assert.IsNotNull(agent);
        }

    }
}
