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
            var dwarfAgent = DfAgentFactory.Create("DWARF", "MALE");
            Assert.IsNotNull(dwarfAgent);
            Assert.IsNotNull(dwarfAgent.Body);
            Assert.AreEqual(79, dwarfAgent.Body.Parts.Count());
        }
    }
}
