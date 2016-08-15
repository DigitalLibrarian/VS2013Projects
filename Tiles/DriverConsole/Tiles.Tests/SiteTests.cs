using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Math;
using Tiles.Structures;
using Moq;

namespace Tiles.Tests
{
    [TestClass]
    public class SiteTests
    {
        [TestMethod]
        public void Dimensions()
        {
            var site =new Site(new Box2(Vector2.Zero, new Vector2(1, 2)));

            Assert.AreEqual(1, site.Size.X);
            Assert.AreEqual(2, site.Size.Y);
        }

        [TestMethod]
        public void GetTileAt_OutOfBounds()
        {
            var site =new Site(new Box2(Vector2.Zero, new Vector2(1, 1)));

            Assert.IsNull(site.GetTileAtIndex(-1, -1));
            Assert.IsNull(site.GetTileAtIndex(2, 2));
            Assert.IsNull(site.GetTileAtIndex(0, -1));
            Assert.IsNull(site.GetTileAtIndex(0, 1));
            Assert.IsNull(site.GetTileAtIndex(-1, 0));
            Assert.IsNull(site.GetTileAtIndex(1, 0));

            Assert.IsNotNull(site.GetTileAtIndex(0, 0));
        }

        [TestMethod]
        public void GetTileAt_Persistence()
        {
            var site = new Site(new Box2(new Vector2(-10, -10), new Vector2(-8, -9)));
            var firstCall = site.GetTileAtIndex(0, 0);
            var secondCall = site.GetTileAtIndex(0, 0);

            Assert.AreSame(firstCall, secondCall);

            firstCall = site.GetTileAtIndex(1, 0);
            secondCall = site.GetTileAtIndex(1, 0);

            Assert.AreSame(firstCall, secondCall);

            firstCall = site.GetTileAtIndex(0, 0);
            secondCall = site.GetTileAtIndex(1, 0);

            Assert.AreNotSame(firstCall, secondCall);

            Assert.AreSame(site.GetTileAtIndex(0, 0), site.GetTileAtSitePos(new Vector2(0, 0)));
        }

        [TestMethod]
        public void GetTiles()
        {
            var w = 4;
            var h = 4;
            var site =new Site(new Box2(Vector2.Zero, new Vector2(w, h)));
            var tiles = site.GetTiles().ToList();
            Assert.AreEqual(w * h, tiles.Count());

            int i = 0;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var t = tiles[i];

                    Assert.AreEqual(x, t.Index.X);
                    Assert.AreEqual(y, t.Index.Y);

                    i++;
                }
            }
        }

        [TestMethod]
        public void InBounds()
        {
            var site =new Site(new Box2(Vector2.Zero, new Vector2(1, 1)));

            Assert.IsFalse(site.InBounds(-1, -1));
            Assert.IsFalse(site.InBounds(new Vector2(-1, -1)));

            Assert.IsFalse(site.InBounds(2, 2));
            Assert.IsFalse(site.InBounds(new Vector2(2, 2)));

            Assert.IsFalse(site.InBounds(0, -1));
            Assert.IsFalse(site.InBounds(new Vector2(0, -1)));

            Assert.IsFalse(site.InBounds(0, 1));
            Assert.IsFalse(site.InBounds(new Vector2(0, 1)));

            Assert.IsFalse(site.InBounds(-1, 0));
            Assert.IsFalse(site.InBounds(new Vector2(-1, 0)));

            Assert.IsFalse(site.InBounds(1, 0));
            Assert.IsFalse(site.InBounds(new Vector2(1, 0)));

            Assert.IsTrue(site.InBounds(0, 0));
            Assert.IsTrue(site.InBounds(new Vector2(0, 0)));
        }

        [TestMethod]
        public void InsertStructure()
        {
            var cells = new Dictionary<Vector2, IStructureCell>
            {
                {new Vector2(0, 0), new Mock<IStructureCell>().Object},
                {new Vector2(0, 1), new Mock<IStructureCell>().Object},
                {new Vector2(1, 0), new Mock<IStructureCell>().Object},
                {new Vector2(1, 1), new Mock<IStructureCell>().Object},
            };
            var structureMock = new Mock<IStructure>();
            structureMock.Setup(x => x.Cells).Returns(cells);
            structureMock.Setup(x => x.Size).Returns(new Vector2(1, 1));

            var insertionPoint = new Vector2(1, 1);
            var site =new Site(new Box2(Vector2.Zero, new Vector2(3, 3)));
            foreach (var tile in site.GetTiles())
            {
                tile.Terrain = Terrain.Tree;
                tile.IsTerrainPassable = false;
            }

            site.InsertStructure(insertionPoint, structureMock.Object);

            foreach (var tile in site.GetTiles())
            {
                var cellKey = tile.Index - insertionPoint;
                if (cells.ContainsKey(cellKey))
                {
                    var expectedCell = cells[cellKey];
                    Assert.IsNotNull(tile.StructureCell);
                    Assert.AreSame(expectedCell, tile.StructureCell);
                    Assert.AreEqual(Terrain.None, tile.Terrain);
                    Assert.IsTrue(tile.IsTerrainPassable);
                }
                else
                {
                    Assert.IsNull(tile.StructureCell);
                    Assert.AreEqual(Terrain.Tree, tile.Terrain);
                    Assert.IsFalse(tile.IsTerrainPassable);
                }
            }

        }
    }
}
