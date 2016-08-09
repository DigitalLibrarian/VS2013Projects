using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles.Tests.Assertions;

namespace Tiles.Math.Tests
{
    [TestClass]
    public class CompassVectorsTests
    {
        [TestMethod]
        public void FromDirection()
        {
            var d = new Dictionary<CompassDirection, Vector2>{
                {CompassDirection.North, CompassVectors.North},
                {CompassDirection.South, CompassVectors.South},
                {CompassDirection.East, CompassVectors.East},
                {CompassDirection.West, CompassVectors.West},
                {CompassDirection.NorthEast, CompassVectors.NorthEast},
                {CompassDirection.NorthWest, CompassVectors.NorthWest},
                {CompassDirection.SouthEast, CompassVectors.SouthEast},
                {CompassDirection.SouthWest, CompassVectors.SouthWest},
            };

            foreach (var p in d)
            {
                Asserter.AreEqual(p.Value, CompassVectors.FromDirection(p.Key));
            }
        }

        [TestMethod]
        public void CombinatoricDirections()
        {
            Asserter.AreEqual(CompassVectors.North + CompassVectors.East, CompassVectors.NorthEast);
            Asserter.AreEqual(CompassVectors.North + CompassVectors.West, CompassVectors.NorthWest);

            Asserter.AreEqual(CompassVectors.South + CompassVectors.East, CompassVectors.SouthEast);
            Asserter.AreEqual(CompassVectors.South + CompassVectors.West, CompassVectors.SouthWest);
        }

        [TestMethod]
        public void GetAll()
        {
            Assert.AreEqual(8, CompassVectors.GetAll().Count());
        }

        [TestMethod]
        public void IsCompassVector()
        {
            foreach (var v in CompassVectors.GetAll())
            {
                Assert.IsTrue(CompassVectors.IsCompassVector(v));
            }

            Assert.IsFalse(CompassVectors.IsCompassVector(new Vector2(2, 2)));
        }
    }
}
