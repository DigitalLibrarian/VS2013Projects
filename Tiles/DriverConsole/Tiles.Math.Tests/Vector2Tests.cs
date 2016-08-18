using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Math.Tests
{
    [TestClass]
    public class Vector2Tests
    {
        [TestMethod]
        public void Constructor()
        {
            var v = new Vector2(1, 2);
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
        }

        [TestMethod]
        public void Addition()
        {
            var v1 = new Vector2(1, 2);
            var v2 = new Vector2(-1, -2);
            var sum = v1 + v2;
            Assert.AreEqual(0, sum.X);
            Assert.AreEqual(0, sum.Y);
            Asserter.AreEqual(sum, v2 + v1);
        }
        
        [TestMethod]
        public void Subtraction()
        {
            var v1 = new Vector2(1, 2);
            var v2 = new Vector2(2, 3);
            var diff = v1 - v2;
            Assert.AreEqual(-1, diff.X);
            Assert.AreEqual(-1, diff.Y);
            Asserter.AreNotEqual(diff, v2 - v1);
        }

        [TestMethod]
        public void VectorMultiplication()
        {
            var v1 = new Vector2(2, 3);
            var v2 = new Vector2(-2, 4);
            var product = v1 * v2;
            Asserter.AreEqual(new Vector2(-4, 12), product);
            Asserter.AreEqual(product, v2 * v1);
        }

        [TestMethod]
        public void ScalarMultiplication()
        {
            var v1 = new Vector2(2, 3);
            var s = 5.5;
            var product = v1 * s;
            Asserter.AreEqual(new Vector2(11, 16), product);
        }

        [TestMethod]
        public void Clamp()
        {
            var min = new Vector2(0, 0);
            var max = new Vector2(10, 10);

            var v = new Vector2(0, 10);
            var c = Vector2.Clamp(v, min, max);

            Assert.AreEqual(0, c.X);
            Assert.AreEqual(10, c.Y);


            v = new Vector2(-10,-10);
            c = Vector2.Clamp(v, min, max);

            Assert.AreEqual(0, c.X);
            Assert.AreEqual(0, c.Y);


            v = new Vector2(100, 100);
            c = Vector2.Clamp(v, min, max);

            Assert.AreEqual(10, c.X);
            Assert.AreEqual(10, c.Y);


            v = new Vector2(-100, 100);
            c = Vector2.Clamp(v, min, max);

            Assert.AreEqual(0, c.X);
            Assert.AreEqual(10, c.Y);

            v = new Vector2(100, -100);
            c = Vector2.Clamp(v, min, max);

            Assert.AreEqual(10, c.X);
            Assert.AreEqual(0, c.Y);


            v = new Vector2(5, 5);
            c = Vector2.Clamp(v, min, max);

            Assert.AreEqual(5, c.X);
            Assert.AreEqual(5, c.Y);
        }

        [TestMethod]
        public void ToString_Uniqueness()
        {
            var v1 = new Vector2(1, 1);
            var v2 = new Vector2(2, 2);
            var v3 = new Vector2(1, 1);

            Assert.AreNotEqual(v1.ToString(), v2.ToString());
            Assert.AreEqual(v1.ToString(), v3.ToString());
        }

        [TestMethod]
        public void Zero()
        {
            Asserter.AreEqual(new Vector2(0, 0), Vector2.Zero);
        }

        [TestMethod]
        public void Dot()
        {
            Assert.AreEqual(0, Vector2.Dot(new Vector2(0, 1), new Vector2(1, 0)));
            Assert.AreEqual(10, Vector2.Dot(new Vector2(2, 2), new Vector2(3, 2)));
        }

        [TestMethod]
        public void GetLength()
        {
            var v = new Vector2(3, 4);
            Assert.AreEqual(5, v.GetLength());
        }

        [TestMethod]
        public void Min()
        {
            var v1 = new Vector2(-1, -1);
            var v2 = new Vector2(0, 0);
            var v3 = new Vector2(-2, 2);

            Asserter.AreEqual(v1, Vector2.Min(v1, v1));
            Asserter.AreEqual(v2, Vector2.Min(v2, v2));
            Asserter.AreEqual(v3, Vector2.Min(v3, v3));

            Asserter.AreEqual(v1, Vector2.Min(v1, v2));

            Asserter.AreEqual(new Vector2(-2, -1), Vector2.Min(v1, v3));
        }

        [TestMethod]
        public void Max()
        {
            var v1 = new Vector2(-1, -1);
            var v2 = new Vector2(0, 0);
            var v3 = new Vector2(-2, 2);

            Asserter.AreEqual(v1, Vector2.Max(v1, v1));
            Asserter.AreEqual(v2, Vector2.Max(v2, v2));
            Asserter.AreEqual(v3, Vector2.Max(v3, v3));

            Asserter.AreEqual(v2, Vector2.Max(v1, v2));

            Asserter.AreEqual(new Vector2(-1, 2), Vector2.Max(v1, v3));
        }
        
        [TestMethod]
        public void Truncate()
        {
            var v = new Vector3(1, 2, 3);
            Asserter.AreEqual(new Vector2(1, 2), Vector2.Truncate(v));
        }
    }
}
