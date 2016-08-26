using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Tests
{
    [TestClass]
    public class PositionFinderTests
    {
        [TestMethod]
        public void FindNearbyPos_SearchOrder()
        {
            var finder = new PositionFinder();

            var startPos = new Vector3(3, 3, 3);
            int halfBoxSize = 2;

            List<Vector3> expectedTested= new List<Vector3>
            {
                new Vector3(3, 3, 3),

                new Vector3(2, 2, 3),
                new Vector3(2, 3, 3),
                new Vector3(2, 4, 3),
                new Vector3(3, 2, 3),
                new Vector3(3, 4, 3),
                new Vector3(4, 2, 3),
                new Vector3(4, 3, 3),
                new Vector3(4, 4, 3),

                new Vector3(1, 1, 3),
                new Vector3(1, 2, 3),
                new Vector3(1, 3, 3),
                new Vector3(1, 4, 3),
                new Vector3(1, 5, 3),
                
                new Vector3(2, 1, 3),
                new Vector3(2, 5, 3),
                
                new Vector3(3, 1, 3),
                new Vector3(3, 5, 3),
                
                new Vector3(4, 1, 3),
                new Vector3(4, 5, 3),

                new Vector3(5, 1, 3),
                new Vector3(5, 2, 3),
                new Vector3(5, 3, 3),
                new Vector3(5, 4, 3),
                new Vector3(5, 5, 3)
            };
            List<Vector3> tested = new List<Vector3>();

            var result = finder.FindNearbyPos(startPos, v => {
                tested.Add(v);
                return false;
            }, halfBoxSize);

            Assert.IsTrue(expectedTested.SequenceEqual(tested));
            Assert.IsFalse(result.HasValue);
        }

        [TestMethod]
        public void FindNearbyPos_HitStops()
        {
            var finder = new PositionFinder();

            var startPos = new Vector3(3, 3, 3);
            int halfBoxSize = 2;

            List<Vector3> expectedTested = new List<Vector3>
            {
                new Vector3(3, 3, 3),

                new Vector3(2, 2, 3),
                new Vector3(2, 3, 3),
                new Vector3(2, 4, 3)
            };
            List<Vector3> tested = new List<Vector3>();

            var result = finder.FindNearbyPos(startPos, v =>
            {
                tested.Add(v);
                return v.X == 2 && v.Y == 4 && v.Z == 3;
            }, halfBoxSize);

            Assert.IsTrue(expectedTested.SequenceEqual(tested));
            Assert.IsTrue(result.HasValue);
            Asserter.AreEqual(new Vector3(2, 4, 3), result.Value);
        }
    }
}
