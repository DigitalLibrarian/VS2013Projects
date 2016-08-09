using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Tiles.Structures;
using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Tests.Structures
{
    [TestClass]
    public class StructureTests
    {
        [TestMethod]
        public void Constructor()
        {
            string name = "name";
            var size = new Vector2(2, 2);
            var structure = new Structure(name, size);

            Assert.AreEqual(name, structure.Name);
            Asserter.AreEqual(size, structure.Size);

            Assert.IsFalse(structure.Cells.Any());
        }

        [TestMethod]
        public void Add()
        {
            string name = "name";
            var size = new Vector2(2, 2);
            var structure = new Structure(name, size);

            Assert.IsFalse(structure.Cells.Any());

            var cellMock = new Mock<IStructureCell>();
            var relPos1 = new Vector2(0, 0);

            structure.Add(relPos1, cellMock.Object);

            Assert.AreEqual(1, structure.Cells.Count());

            Assert.AreSame(cellMock.Object, structure.Cells[relPos1]);
        }
    }
}
