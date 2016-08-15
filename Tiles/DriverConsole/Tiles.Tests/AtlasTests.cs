using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Math;
using Tiles.Structures;
using Moq;
using Tiles.Tests.Assertions;

namespace Tiles.Tests
{
    [TestClass]
    public class AtlasTests
    {
        Vector3 SiteSize { get; set;}
        Mock<ISiteFactory> SiteFactoryMock { get; set; }

        Atlas Atlas { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            SiteFactoryMock = new Mock<ISiteFactory>();
            SiteSize = new Vector3(10, 10, 10);
            Atlas = new Atlas(SiteFactoryMock.Object, SiteSize);
        }

        [TestMethod]
        public void PostConstruction()
        {
            Assert.IsFalse(Atlas.GetTiles().Any());
            Assert.IsFalse(Atlas.GetSites().Any());
        }

        [TestMethod]
        public void SiteIndex()
        {
            Asserter.AreEqual(new Vector3(0, 0, 0), Atlas.SiteIndex(new Vector3(0, 0, 0)));
            Asserter.AreEqual(new Vector3(0, 0, 0), Atlas.SiteIndex(new Vector3(1, 1, 0)));
            Asserter.AreEqual(new Vector3(0, 0, 0), Atlas.SiteIndex(new Vector3(9, 9, 0)));


            Asserter.AreEqual(new Vector3(1, 1, 1), Atlas.SiteIndex(new Vector3(10, 10, 10)));
            Asserter.AreEqual(new Vector3(1, 1, 1), Atlas.SiteIndex(new Vector3(11, 11, 11)));
            Asserter.AreEqual(new Vector3(1, 1, 1), Atlas.SiteIndex(new Vector3(19, 19, 10)));


            Asserter.AreEqual(new Vector3(-1, -1, -1), Atlas.SiteIndex(new Vector3(-9, -9, -8)));
            Asserter.AreEqual(new Vector3(-1, -1, -1), Atlas.SiteIndex(new Vector3(-1, -1, -1)));
            Asserter.AreEqual(new Vector3(-1, -1, -1), Atlas.SiteIndex(new Vector3(-10, -10, -10)));
        }

        [TestMethod]
        public void ToSitePos()
        {

            Asserter.AreEqual(new Vector3(0, 0, 0), Atlas.ToSitePos(new Vector3(0, 0, 0)));
            Asserter.AreEqual(new Vector3(1, 1, 1), Atlas.ToSitePos(new Vector3(1, 1, 1)));
            Asserter.AreEqual(new Vector3(9, 9, 9), Atlas.ToSitePos(new Vector3(9, 9, 9)));

            Asserter.AreEqual(new Vector3(0, 0, 0), Atlas.ToSitePos(new Vector3(10, 10, 10)));
            Asserter.AreEqual(new Vector3(1, 1, 1), Atlas.ToSitePos(new Vector3(11, 11, 11)));
            Asserter.AreEqual(new Vector3(9, 9, 9), Atlas.ToSitePos(new Vector3(19, 19, 19)));

            Asserter.AreEqual(new Vector3(0, 0, 0), Atlas.ToSitePos(new Vector3(-10, -10, -10)));
            Asserter.AreEqual(new Vector3(1, 1, 1), Atlas.ToSitePos(new Vector3(-9, -9, -9)));
            Asserter.AreEqual(new Vector3(9, 9, 9), Atlas.ToSitePos(new Vector3(-1, -1, -1)));
        }

        [TestMethod]
        public void GetTileAtPos_PositiveTileIndex()
        {
            var pos = new Vector3(24, 23, 25);
            var siteIndex = new Vector3(2, 2, 2);
            var box = new Box3(siteIndex * SiteSize, (siteIndex * SiteSize) + SiteSize);
            var sitePos = new Vector3(4, 3, 5);

            var tileMock = new Mock<ITile>();
            var siteMock = new Mock<ISite>();
            siteMock.Setup(x => x.GetTileAtSitePos(sitePos)).Returns(tileMock.Object);;
            siteMock.Setup(x => x.GetTiles()).Returns(new List<ITile> { tileMock.Object });

            SiteFactoryMock.Setup(x => x.Create(Atlas, siteIndex, box)).Returns(siteMock.Object);

            var result = Atlas.GetTileAtPos(pos);

            SiteFactoryMock.Verify(x => x.Create(Atlas, siteIndex, box), Times.Once());
            siteMock.Verify(x => x.GetTileAtSitePos(sitePos), Times.Once());

            Assert.AreSame(tileMock.Object, result);

            var sites = Atlas.GetSites();
            Assert.AreEqual(1, sites.Count());
            Assert.AreSame(siteMock.Object, sites.Single());

            var tiles = Atlas.GetTiles();
            Assert.AreEqual(1, tiles.Count());
            Assert.AreSame(tileMock.Object, tiles.Single());

            var result2 = Atlas.GetTileAtPos(pos);

            SiteFactoryMock.Verify(x => x.Create(Atlas, siteIndex, box), Times.Once());
            siteMock.Verify(x => x.GetTileAtSitePos(sitePos), Times.Exactly(2));
        }



        [TestMethod]
        public void GetTileAtPos_NegativeTileIndex()
        {
            var pos = new Vector3(-24, -23, -22);
            var siteIndex = new Vector3(-3, -3, -3);
            var box = new Box3((siteIndex * SiteSize) + SiteSize, siteIndex * SiteSize);
            var sitePos = new Vector3(6, 7, 8);

            var tileMock = new Mock<ITile>();
            var siteMock = new Mock<ISite>();
            siteMock.Setup(x => x.GetTileAtSitePos(sitePos)).Returns(tileMock.Object); ;
            siteMock.Setup(x => x.GetTiles()).Returns(new List<ITile> { tileMock.Object });

            SiteFactoryMock.Setup(x => x.Create(Atlas, siteIndex, box)).Returns(siteMock.Object);

            var result = Atlas.GetTileAtPos(pos);

            SiteFactoryMock.Verify(x => x.Create(Atlas, siteIndex, box), Times.Once());
            siteMock.Verify(x => x.GetTileAtSitePos(sitePos), Times.Once());

            Assert.AreSame(tileMock.Object, result);

            var sites = Atlas.GetSites();
            Assert.AreEqual(1, sites.Count());
            Assert.AreSame(siteMock.Object, sites.Single());

            var tiles = Atlas.GetTiles();
            Assert.AreEqual(1, tiles.Count());
            Assert.AreSame(tileMock.Object, tiles.Single());

            var result2 = Atlas.GetTileAtPos(pos);

            SiteFactoryMock.Verify(x => x.Create(Atlas, siteIndex, box), Times.Once());
            siteMock.Verify(x => x.GetTileAtSitePos(sitePos), Times.Exactly(2));
        }

        [TestMethod]
        public void InsertStructure()
        {
            var siteMock = new Mock<ISite>();
            var siteIndex = new Vector3(0, 0, 0);
            var box = new Box3(Vector3.Zero, SiteSize);
            SiteFactoryMock.Setup(x => x.Create(Atlas, siteIndex, box)).Returns(siteMock.Object);
            var tileMock11 = new Mock<ITile>();
            var tileMock12 = new Mock<ITile>();
            var tileMock21 = new Mock<ITile>();
            var tileMock22 = new Mock<ITile>();
            siteMock.Setup(x => x.GetTileAtSitePos(new Vector3(1, 1, 1))).Returns(tileMock11.Object);
            siteMock.Setup(x => x.GetTileAtSitePos(new Vector3(1, 2, 1))).Returns(tileMock12.Object);
            siteMock.Setup(x => x.GetTileAtSitePos(new Vector3(2, 1, 1))).Returns(tileMock21.Object);
            siteMock.Setup(x => x.GetTileAtSitePos(new Vector3(2, 2, 2))).Returns(tileMock22.Object);

            var cells = new Dictionary<Vector3, IStructureCell>
            {
                {new Vector3(0, 0, 0), new Mock<IStructureCell>().Object},
                {new Vector3(0, 1, 0), new Mock<IStructureCell>().Object},
                {new Vector3(1, 0, 0), new Mock<IStructureCell>().Object},
                {new Vector3(1, 1, 1), new Mock<IStructureCell>().Object},
            };
            var structureMock = new Mock<IStructure>();
            structureMock.Setup(x => x.Cells).Returns(cells);
            structureMock.Setup(x => x.Size).Returns(new Vector3(2, 2, 2));

            var insertionPoint = new Vector3(1, 1, 1);
            var atlas = Atlas;

            atlas.GetTileAtPos(new Vector3(0, 0, 0));// prime it to generate a site
            foreach (var tile in atlas.GetTiles())
            {
                tile.Terrain = Terrain.Tree;
                tile.IsTerrainPassable = false;
            }

            atlas.InsertStructure(insertionPoint, structureMock.Object);

            foreach (var tile in atlas.GetTiles())
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
