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
    public class MaterialStrikeResultBuilderTests
    {
        Mock<IMaterialStressCalc> StressCalcMock { get; set; }

        MaterialStrikeResultBuilder Builder { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            StressCalcMock = new Mock<IMaterialStressCalc>();

            Builder = new MaterialStrikeResultBuilder(StressCalcMock.Object);
        }

        [Ignore]
        [TestMethod]
        public void Edged_PartialDent()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Edged_CompleteDent()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Edged_PartialCut()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Edged_CompleteCut()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_Deflection()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_PartialDent()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_CompleteDent()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_PartialFracture()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_CompleteFracture()
        {
            throw new NotImplementedException();
        }
    }
}
