using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Tests.Assertions;

namespace Tiles.Math.Tests
{
    [TestClass]
    public class FractionTests
    {
        [TestMethod]
        public void ZeroDivide()
        {
            Asserter.AssertException<DivideByZeroException>(() => new Fraction(0, 0));
        }

        [TestMethod]
        public void Zero()
        {
            var f = new Fraction(0, 1);
            Assert.AreEqual(0, f.AsDouble());
        }

        [TestMethod]
        public void FibonacciPair()
        {
            var f = new Fraction(89, 144);
            Assert.AreEqual(0.6180555555555556, f.AsDouble());
        }
    }
}
