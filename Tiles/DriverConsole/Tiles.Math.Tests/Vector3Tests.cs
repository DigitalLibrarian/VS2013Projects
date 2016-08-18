using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Math.Tests
{
    [TestClass]
    public class Vector3Tests
    {
        [TestMethod]
        public void Constructor()
        {
            var v = new Vector3(1, 2, 3);
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(3, v.Z);
        }

        [TestMethod]
        public void Addition()
        {
            var v1 = new Vector3(1, 2, 3);
            var v2 = new Vector3(-1, -2, -3);
            var sum = v1 + v2;
            Assert.AreEqual(0, sum.X);
            Assert.AreEqual(0, sum.Y);
            Asserter.AreEqual(sum, v2 + v1);
        }

        [TestMethod]
        public void Subtraction()
        {
            var v1 = new Vector3(1, 2, 3);
            var v2 = new Vector3(2, 3, 4);
            var diff = v1 - v2;
            Assert.AreEqual(-1, diff.X);
            Assert.AreEqual(-1, diff.Y);
            Assert.AreEqual(-1, diff.Z);
            Asserter.AreNotEqual(diff, v2 - v1);
        }

        [TestMethod]
        public void VectorMultiplication()
        {
            var v1 = new Vector3(2, 3, 4);
            var v2 = new Vector3(-2, 4, 5);
            var product = v1 * v2;
            Asserter.AreEqual(new Vector3(-4, 12, 20), product);
            Asserter.AreEqual(product, v2 * v1);
        }

        [TestMethod]
        public void ScalarMultiplication()
        {
            var v1 = new Vector3(2, 3, 4);
            var s = 5.5;
            var product = v1 * s;
            Asserter.AreEqual(new Vector3(11, 16, 22), product);
        }

        [TestMethod]
        public void ToString_Uniqueness()
        {
            var v1 = new Vector3(1, 1, 1);
            var v2 = new Vector3(2, 2, 2);
            var v3 = new Vector3(1, 1, 1);

            Assert.AreNotEqual(v1.ToString(), v2.ToString());
            Assert.AreEqual(v1.ToString(), v3.ToString());
        }

        [TestMethod]
        public void Zero()
        {
            Asserter.AreEqual(new Vector3(0, 0, 0), Vector3.Zero);
        }

        [TestMethod]
        public void Dot()
        {
            Assert.AreEqual(0, Vector3.Dot(new Vector3(0, 1, 0), new Vector3(1, 0, 0)));
            Assert.AreEqual(14, Vector3.Dot(new Vector3(2, 2, 2), new Vector3(3, 2, 2)));
        }

        [TestMethod]
        public void Min()
        {
            var v1 = new Vector3(-1, -1, -1);
            var v2 = new Vector3(0, 0, 0);
            var v3 = new Vector3(-2, 2, -2);

            Asserter.AreEqual(v1, Vector3.Min(v1, v1));
            Asserter.AreEqual(v2, Vector3.Min(v2, v2));
            Asserter.AreEqual(v3, Vector3.Min(v3, v3));

            Asserter.AreEqual(v1, Vector3.Min(v1, v2));

            Asserter.AreEqual(new Vector3(-2, -1, -2), Vector3.Min(v1, v3));
        }

        [TestMethod]
        public void Max()
        {
            var v1 = new Vector3(-1, -1, -1);
            var v2 = new Vector3(0, 0, 0);
            var v3 = new Vector3(-2, 2, -2);

            Asserter.AreEqual(v1, Vector3.Max(v1, v1));
            Asserter.AreEqual(v2, Vector3.Max(v2, v2));
            Asserter.AreEqual(v3, Vector3.Max(v3, v3));

            Asserter.AreEqual(v2, Vector3.Max(v1, v2));

            Asserter.AreEqual(new Vector3(-1, 2, -1), Vector3.Max(v1, v3));
        }
    }
}
