using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Materials.Tests
{
    [TestClass]
    public class LayeredMaterialStrikeResultBuilderTests
    {
        Mock<ISingleLayerStrikeTester> TesterMock { get; set; }

        LayeredMaterialStrikeResultBuilder Builder { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            TesterMock = new Mock<ISingleLayerStrikeTester>();
            Builder = new LayeredMaterialStrikeResultBuilder(TesterMock.Object);
        }

    }
}
