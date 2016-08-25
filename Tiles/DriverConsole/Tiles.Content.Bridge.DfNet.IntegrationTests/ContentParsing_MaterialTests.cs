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
    public class ContentParsing_MaterialTests
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialFactory DfMaterialFactory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            DfMaterialFactory = new DfMaterialFactory(Store);
        }

        [TestMethod]
        public void FlameTemplate()
        {
            var flame = DfMaterialFactory.CreateFromMaterialTemplate("FLAME_TEMPLATE");
            Assert.AreNotSame("flames", flame.Adjective);
        }
    }
}
