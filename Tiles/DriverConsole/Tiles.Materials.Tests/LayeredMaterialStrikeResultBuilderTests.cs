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
        Mock<IMaterialStrikeResultBuilder> MatStrikeBuilderMock { get; set; }

        LayeredMaterialStrikeResultBuilder Builder { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            MatStrikeBuilderMock = new Mock<IMaterialStrikeResultBuilder>();
            Builder = new LayeredMaterialStrikeResultBuilder(MatStrikeBuilderMock.Object);
        }

    }
}
