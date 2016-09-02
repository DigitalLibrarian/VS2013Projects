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
        public void ZeroDivide_Constructor()
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
        public void ZeroDivide_DenominatorSet()
        {
            var f = new Fraction(1, 1);
            Asserter.AssertException<DivideByZeroException>(() => f.Denominator = 0);
        }

        [TestMethod]
        public void SettersChangeFraction()
        {
            var f = new Fraction(1, 1);
            Assert.AreEqual(1, f.AsDouble());

            f.Numerator = 2;
            f.Denominator = 4;

            Assert.AreEqual(0.5, f.AsDouble());
        }

        [TestMethod]
        public void FibonacciPair()
        {
            var f = new Fraction(89, 144);
            Assert.AreEqual(0.6180555555555556, f.AsDouble());
        }
    }
}
