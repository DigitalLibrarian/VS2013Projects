using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles;
using Tiles.Items;
using Tiles.Agents;
using Moq;

namespace Tiles.Tests
{
    [TestClass]
    public class TileTests
    {
        [TestMethod]
        public void Index_Constructor()
        {
            var t = new Tile(1, 2);

            Assert.AreEqual(1, t.Index.X);
            Assert.AreEqual(2, t.Index.Y);

            Assert.IsFalse(t.HasStructureCell);
            Assert.IsNull(t.StructureCell);
            Assert.IsFalse(t.HasAgent);
            Assert.IsNull(t.Agent);
            Assert.AreEqual(Terrain.None, t.Terrain);
            Assert.IsNull(t.TerrainSprite);
        }

        [TestMethod]
        public void IsTerrainPassable_Default()
        {
            var t = new Tile(1, 1);
            Assert.IsTrue(t.IsTerrainPassable);
        }

        [TestMethod]
        public void ObjectPersistence()
        {
            var objMock1 = new Mock<IItem>();
            var objMock2 = new Mock<IItem>();

            var tile = new Tile(1, 1);

            Assert.AreEqual(0, tile.Items.Count());
            Assert.IsNull(tile.GetTopItem());

            tile.Items.Add(objMock1.Object);

            Assert.AreEqual(1, tile.Items.Count());
            Assert.IsTrue(tile.Items.Contains(objMock1.Object));
            Assert.AreSame(objMock1.Object, tile.GetTopItem());

            tile.Items.Add(objMock2.Object);

            Assert.AreEqual(2, tile.Items.Count());
            Assert.IsTrue(tile.Items.Contains(objMock1.Object));
            Assert.IsTrue(tile.Items.Contains(objMock2.Object));
            Assert.AreSame(objMock2.Object, tile.GetTopItem());
        }

        [TestMethod]
        public void HasAgent()
        {
            var tile = new Tile(1, 1);
            Assert.IsFalse(tile.HasAgent);
            Assert.IsNull(tile.Agent);

            tile.Agent = new Mock<IAgent>().Object;
            Assert.IsTrue(tile.HasAgent);
            Assert.IsNotNull(tile.Agent);

            tile.RemoveAgent();
            Assert.IsFalse(tile.HasAgent);
            Assert.IsNull(tile.Agent);

            tile.SetAgent(new Mock<IAgent>().Object);
            Assert.IsTrue(tile.HasAgent);
            Assert.IsNotNull(tile.Agent);
        }
    }
}
