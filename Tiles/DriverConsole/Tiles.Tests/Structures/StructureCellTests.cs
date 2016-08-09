using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Tiles.Structures;
using Tiles.Bodies;

namespace Tiles.Tests.Structures
{
    [TestClass]
    public class StructureCellTests
    {
        Mock<IStructure> StructureMock { get; set; }
        Mock<ISprite> SpriteMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            StructureMock = new Mock<IStructure>();
            SpriteMock = new Mock<ISprite>();
        }

        [TestMethod]
        public void ConstructorDefaults()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object);

            Assert.AreSame(StructureMock.Object, cell.Structure);
            Assert.AreSame(SpriteMock.Object, cell.Sprite);
            Assert.AreEqual(cellType, cell.Type);
            Assert.IsFalse(cell.IsOpen);
            Assert.IsFalse(cell.CanClose);
            Assert.IsFalse(cell.CanOpen);
            Assert.IsTrue(cell.CanPass);
        }

        [TestMethod]
        public void ConfigConstructor()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canOpen: true, canClose: true, canPass: false, isOpen: true
                );

            Assert.AreSame(StructureMock.Object, cell.Structure);
            Assert.AreSame(SpriteMock.Object, cell.Sprite);
            Assert.AreEqual(cellType, cell.Type);
            Assert.IsTrue(cell.IsOpen);
            Assert.IsTrue(cell.CanClose);
            Assert.IsTrue(cell.CanOpen);
            Assert.IsFalse(cell.CanPass);
        }

        [TestMethod]
        public void Open_CanOpen_NotOpen()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canOpen: true, isOpen: false
                );

            var result = cell.Open();
            Assert.IsTrue(result);
            Assert.IsTrue(cell.IsOpen);
        }

        [TestMethod]
        public void Open_CanOpen_Open()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canOpen: true, isOpen: true
                );

            var result = cell.Open();
            Assert.IsFalse(result);
            Assert.IsTrue(cell.IsOpen);
        }

        [TestMethod]
        public void Open_CanNotOpen_NotOpen()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canOpen: false, isOpen: false
                );

            var result = cell.Open();
            Assert.IsFalse(result);
            Assert.IsFalse(cell.IsOpen);
        }

        [TestMethod]
        public void Open_CanNotOpen_Open()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canOpen: false, isOpen: true
                );

            var result = cell.Open();
            Assert.IsFalse(result);
            Assert.IsTrue(cell.IsOpen);
        }

        [TestMethod]
        public void Close_CanClose_Open()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canClose: true, isOpen: true
                );

            var result = cell.Close();
            Assert.IsTrue(result);
            Assert.IsFalse(cell.IsOpen);
        }

        [TestMethod]
        public void Close_CanClose_NotOpen()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canClose: true, isOpen: false
                );

            var result = cell.Close();
            Assert.IsFalse(result);
            Assert.IsFalse(cell.IsOpen);
        }

        [TestMethod]
        public void Close_CanNotClose_Open()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canClose: false, isOpen: true
                );

            var result = cell.Close();
            Assert.IsFalse(result);
            Assert.IsTrue(cell.IsOpen);
        }

        [TestMethod]
        public void Close_CanNotClose_NotOpen()
        {
            StructureCellType cellType = StructureCellType.None;
            var cell = new StructureCell(StructureMock.Object, cellType, SpriteMock.Object,
                canClose: false, isOpen: false
                );

            var result = cell.Close();
            Assert.IsFalse(result);
            Assert.IsFalse(cell.IsOpen);
        }
    }
}
