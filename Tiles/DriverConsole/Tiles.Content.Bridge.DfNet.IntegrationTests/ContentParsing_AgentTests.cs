using DfNet.Raws;
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
                new DfMaterialFactory(Store)
                );
        }

        [TestMethod]
        public void Dwarf()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            Assert.IsNotNull(agent);
            Assert.IsNotNull(agent.Body);
            Assert.AreEqual(89, agent.Body.Parts.Count());

            var leftHand = agent.Body.Parts.SingleOrDefault(p => p.NameSingular.Equals("left hand"));
            Assert.IsNotNull(leftHand);
            Assert.IsNotNull(leftHand.Tissue);
            Assert.IsTrue(leftHand.CanGrasp);
            Assert.IsFalse(leftHand.CanBeAmputated);
            
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
                var leftHandPart = leftHandParts.SingleOrDefault(p => p.NameSingular.Equals(partName));
                Assert.IsNotNull(leftHandPart);
                Assert.IsNotNull(leftHandPart.Tissue);
                Assert.IsTrue(expectedTissueAdj.SequenceEqual(leftHandPart.Tissue.Layers.Select(x => x.Material.Adjective)));

                Assert.IsFalse(leftHandPart.CanGrasp);
                Assert.IsTrue(leftHandPart.CanBeAmputated);
            }
            var leftWrist = leftHandParts.SingleOrDefault(p => p.NameSingular.Equals("left wrist"));
            Assert.IsNotNull(leftWrist);
            Assert.AreEqual(2, leftWrist.Tissue.Layers.Count());

            Assert.IsTrue(
                new string[]{"bone", "muscle"}.SequenceEqual(leftWrist.Tissue.Layers.Select(x => x.Material.Adjective))
                );
        }
    }
}
