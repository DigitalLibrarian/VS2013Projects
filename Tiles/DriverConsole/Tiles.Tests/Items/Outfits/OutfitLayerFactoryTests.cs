using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;
using Tiles.Items.Outfits;

namespace Tiles.Tests.Items.Outfits
{
    [TestClass]
    public class OutfitLayerFactoryTests
    {
        [TestMethod]
        public void Create()
        {
            var fact = new OutfitLayerFactory();
            Assert.IsNotNull(fact.Create<ArmorSlot>(null, null, null, null));
        }
    }
}
