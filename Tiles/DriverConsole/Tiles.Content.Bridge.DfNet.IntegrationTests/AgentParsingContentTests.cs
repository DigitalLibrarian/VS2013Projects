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
    public class AgentParsingContentTests
    {
        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            DfAgentFactory = new DfAgentFactory(Store, new DfAgentBuilderFactory());
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
                   
            
            var leftHandParts = agent.Body.Parts.Where(p => p.Parent == leftHand);
            Assert.AreEqual(6, leftHandParts.Count());


            //[BODY_DETAIL_PLAN:VERTEBRATE_TISSUE_LAYERS:SKIN:FAT:MUSCLE:BONE:CARTILAGE]
	        //[BP_LAYERS:BY_CATEGORY:FINGER:ARG4:25:ARG3:25:ARG2:5:ARG1:1]
            var expectedTissueAdj = new string[]{
                "cartilage",
                "bone",
                "muscle",
                "fat",
                "skin",
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
                Assert.AreEqual(expectedTissueAdj.Count(), leftHandPart.Tissue.Layers.Count());
            }

            Assert.IsNotNull(leftHandParts.SingleOrDefault(p => p.NameSingular.Equals("left wrist")));
        }
    }
}
