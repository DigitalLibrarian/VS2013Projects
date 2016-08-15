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
            var site =new Site(new Box3(Vector3.Zero, new Vector3(1, 2, 3)));

            Assert.AreEqual(1, site.Size.X);
            Assert.AreEqual(2, site.Size.Y);
            Assert.AreEqual(3, site.Size.Z);
        }

        [TestMethod]
        public void GetTileAt_OutOfBounds()
        {
            var site =new Site(new Box3(Vector3.Zero, new Vector3(1, 1, 1)));

            Assert.IsNull(site.GetTileAtIndex(-1, -1, -1));
            Assert.IsNull(site.GetTileAtIndex(2, 2, 2));
            Assert.IsNull(site.GetTileAtIndex(0, -1, 0));
            Assert.IsNull(site.GetTileAtIndex(0, 1, 0));
            Assert.IsNull(site.GetTileAtIndex(-1, 0, 0));
            Assert.IsNull(site.GetTileAtIndex(1, 0, 0));

            Assert.IsNotNull(site.GetTileAtIndex(0, 0, 0));
        }

        [TestMethod]
        public void GetTileAt_Persistence()
        {
            var site = new Site(new Box3(new Vector3(-10, -10, -10), new Vector3(-8, -9, -7)));
            var firstCall = site.GetTileAtIndex(0, 0, 0);
            var secondCall = site.GetTileAtIndex(0, 0, 0);

            Assert.AreSame(firstCall, secondCall);

            firstCall = site.GetTileAtIndex(1, 0, 0);
            secondCall = site.GetTileAtIndex(1, 0, 0);

            Assert.AreSame(firstCall, secondCall);

            firstCall = site.GetTileAtIndex(0, 0, 0);
            secondCall = site.GetTileAtIndex(1, 0, 0);

            Assert.AreNotSame(firstCall, secondCall);

            Assert.AreSame(site.GetTileAtIndex(0, 0, 0), site.GetTileAtSitePos(new Vector3(0, 0, 0)));
        }

        [TestMethod]
        public void GetTiles()
        {
            var w = 4;
            var h = 4;
            var d = 4;
            var site =new Site(new Box3(Vector3.Zero, new Vector3(w, h, d)));
            var tiles = site.GetTiles().ToList();
            Assert.AreEqual(w * h * d, tiles.Count());

            int i = 0;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int z = 0; z < d; z++)
                    {
                        var t = tiles[i];

                        Assert.AreEqual(x, t.Index.X);
                        Assert.AreEqual(y, t.Index.Y);
                        Assert.AreEqual(z, t.Index.Z);

                        i++;
                    }
                }
            }
        }

        [TestMethod]
        public void InBounds()
        {
            var site =new Site(new Box3(Vector3.Zero, new Vector3(1, 1, 1)));

            Assert.IsFalse(site.InBounds(-1, -1, -1));
            Assert.IsFalse(site.InBounds(new Vector3(-1, -1, -1)));

            Assert.IsFalse(site.InBounds(2, 2, 2));
            Assert.IsFalse(site.InBounds(new Vector3(2, 2, 2)));

            Assert.IsFalse(site.InBounds(0, -1, 0));
            Assert.IsFalse(site.InBounds(new Vector3(0, -1, 0)));

            Assert.IsFalse(site.InBounds(0, 1, 0));
            Assert.IsFalse(site.InBounds(new Vector3(0, 1, 0)));

            Assert.IsFalse(site.InBounds(-1, 0, 0));
            Assert.IsFalse(site.InBounds(new Vector3(-1, 0, 0)));

            Assert.IsFalse(site.InBounds(1, 0, 0));
            Assert.IsFalse(site.InBounds(new Vector3(1, 0, 0)));

            Assert.IsTrue(site.InBounds(0, 0, 0));
            Assert.IsTrue(site.InBounds(new Vector3(0, 0, 0)));
        }

        [Ignore]
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

            var insertionPoint = new Vector3(1, 1, 3);
            var site =new Site(new Box3(Vector3.Zero, new Vector3(3, 3, 3)));
            foreach (var tile in site.GetTiles())
            {
                tile.Terrain = Terrain.Tree;
                tile.IsTerrainPassable = false;
            }

            site.InsertStructure(insertionPoint, structureMock.Object);

            foreach (var tile in site.GetTiles())
            {
                var cellKey = tile.Index - insertionPoint;
                /*
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
                 * */
            }

        }
    }
}
