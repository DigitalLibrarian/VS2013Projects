using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Tests.Assertions;

namespace DfNet.Raws.Tests
{
    [TestClass]
    public class DfObjectTests
    {
        [TestMethod]
        public void NotEnoughTags()
        {
            Asserter.AssertException<DfObject.InvalidHeaderException>(() =>
            {
                new DfObject(new List<DfTag>());
            });
        }

        [TestMethod]
        public void NotEnoughWords()
        {
            Asserter.AssertException<DfObject.InvalidHeaderException>(() =>
            {
                new DfObject(new List<DfTag>(){
                    new DfTag("")
                });
            });
        }

        [TestMethod]
        public void TypeAndName()
        {
            var o = new DfObject(new List<DfTag>
            {
                new DfTag("TURD", "SANDWICH")
            });

            Assert.AreEqual("TURD", o.Type);
            Assert.AreEqual("SANDWICH", o.Name);
        }
    }
}
