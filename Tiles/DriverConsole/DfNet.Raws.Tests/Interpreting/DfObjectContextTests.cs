using DfNet.Raws.Interpreting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Tests.Interpreting
{
    [TestClass]
    public class DfObjectContextTests
    {
        [TestMethod]
        public void ClassInvariants()
        {
            var source = new DfObject(new DfTag("A", "B"));
            int cursor = 0;
            var b = new DfObjectContext(source, cursor);

            Assert.AreSame(source, b.Source);
            Assert.AreEqual(cursor, b.Cursor);
        }

        [TestMethod]
        public void RemoveTag()
        {
            var source = new DfObject(
                new DfTag("A", "B"), 
                new DfTag("TARGET2"),
                new DfTag("TARGET3"),
                new DfTag("INIT_CURSOR"),
                new DfTag("TARGET1")
                );
            var b = new DfObjectContext(source, 2);

            Assert.AreEqual(0, b.RemoveTagsByName("NOT_THERE"));

            Assert.AreEqual(1, b.RemoveTagsByName("TARGET1"));
            Assert.AreEqual(2, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("TARGET2"));
            Assert.AreEqual(1, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("TARGET3"));
            Assert.AreEqual(1, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("INIT_CURSOR"));
            Assert.AreEqual(1, b.Cursor);
        }
    }
}
