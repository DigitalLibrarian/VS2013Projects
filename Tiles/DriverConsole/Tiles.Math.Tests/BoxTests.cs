using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Math.Tests
{
    [TestClass]
    public class BoxTests
    {
        [TestMethod]
        public void PointBox()
        {
            var b = new Box2(Vector2.Zero, Vector2.Zero);

            Asserter.AreEqual(Vector2.Zero, b.Min);
            Asserter.AreEqual(Vector2.Zero, b.Max);
            Asserter.AreEqual(Vector2.Zero, b.Size);

        }

        [TestMethod]
        public void StandardInput()
        {
            var min = new Vector2(-1, -1);
            var max = new Vector2(1, 1);

            var b = new Box2(min, max);

            Asserter.AreEqual(min, b.Min);
            Asserter.AreEqual(max, b.Max);
            Asserter.AreEqual(new Vector2(2, 2), b.Size);
        }

        [TestMethod]
        public void MixedInput()
        {
            var min = new Vector2(-1, 1);
            var max = new Vector2(1, -1);

            var b = new Box2(min, max);

            Asserter.AreEqual(new Vector2(-1, -1), b.Min);
            Asserter.AreEqual(new Vector2(1, 1), b.Max);

            Asserter.AreEqual(new Vector2(2, 2), b.Size);
        }

        [TestMethod]
        public void Contains_Hit()
        {
            var b = new Box2(new Vector2(-1, -1), new Vector2(1, 1));

            Assert.IsTrue(b.Contains(Vector2.Zero));
            Assert.IsTrue(b.Contains(new Vector2(-1, -1)));
            Assert.IsTrue(b.Contains(new Vector2(1, 1)));
        }

        [TestMethod]
        public void Contains_Miss()
        {
            var b = new Box2(new Vector2(-1, -1), new Vector2(1, 1));

            Assert.IsFalse(b.Contains(new Vector2(100, 100)));
            Assert.IsFalse(b.Contains(new Vector2(-2, -2)));
            Assert.IsFalse(b.Contains(new Vector2(2, 2)));
        }
    }
}
