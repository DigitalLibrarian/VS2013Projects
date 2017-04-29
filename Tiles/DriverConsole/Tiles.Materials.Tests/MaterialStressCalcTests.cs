using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Materials.Tests
{
    [TestClass]
    public class MaterialStressCalcTests
    {
        void AssertRoughly(double expected, double actual, string message)
        {
            Assert.AreEqual(
                (int)(expected * 100) / 100d,
                (int)(actual * 100) / 100d,
                message);
        }
    }
}
