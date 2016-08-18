using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Math.Tests
{
    [TestClass]
    public class Box3Tests
    {
        [TestMethod]
        public void PointBox()
        {
            var b = new Box3(Vector3.Zero, Vector3.Zero);

            Asserter.AreEqual(Vector3.Zero, b.Min);
            Asserter.AreEqual(Vector3.Zero, b.Max);
            Asserter.AreEqual(Vector3.Zero, b.Size);

        }

        [TestMethod]
        public void StandardInput()
        {
            var min = new Vector3(-1, -1, 0);
            var max = new Vector3(1, 1, 10);

            var b = new Box3(min, max);

            Asserter.AreEqual(min, b.Min);
            Asserter.AreEqual(max, b.Max);
            Asserter.AreEqual(new Vector3(2, 2, 10), b.Size);
        }

        [TestMethod]
        public void MixedInput()
        {
            var min = new Vector3(-1, 1, -10);
            var max = new Vector3(1, -1, 10);

            var b = new Box3(min, max);

            Asserter.AreEqual(new Vector3(-1, -1, -10), b.Min);
            Asserter.AreEqual(new Vector3(1, 1, 10), b.Max);

            Asserter.AreEqual(new Vector3(2, 2, 20), b.Size);
        }

        [TestMethod]
        public void Contains_Hit()
        {
            var b = new Box3(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));

            Assert.IsTrue(b.Contains(Vector3.Zero));
            Assert.IsTrue(b.Contains(new Vector3(-1, -1, -1)));
            Assert.IsTrue(b.Contains(new Vector3(1, 1, 1)));
        }

        [TestMethod]
        public void Contains_Miss()
        {
            var b = new Box3(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));

            Assert.IsFalse(b.Contains(new Vector3(100, 100, 100)));
            Assert.IsFalse(b.Contains(new Vector3(-2, -2, -2)));
            Assert.IsFalse(b.Contains(new Vector3(2, 2, 2)));
        }
    }
}
