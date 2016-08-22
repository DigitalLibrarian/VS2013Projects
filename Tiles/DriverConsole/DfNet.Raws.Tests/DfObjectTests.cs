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
            var o = new DfObject(new DfTag("TURD", "SANDWICH"));

            Assert.AreEqual("TURD", o.Type);
            Assert.AreEqual("SANDWICH", o.Name);
        }

        [TestMethod]
        public void CloneDfObject()
        {
            var o = new DfObject(new List<DfTag>
            {
                new DfTag("TURD", "SANDWICH"),
                new DfTag("ONE", "1", "2"),
                new DfTag("TWO"),
                new DfTag("THREE", "kick", "kicks")
            });


            var clone = o.CloneDfObject();

            Assert.AreNotSame(o, clone);
            int i = 0;
            foreach (var tag in o.Tags)
            {
                Assert.AreNotSame(o.Tags[i], clone.Tags[i]);
                Assert.AreEqual(o.Tags.Count(), clone.Tags.Count());
                for (int j = 0; j < o.Tags[i].NumWords; j++)
                {
                    Assert.AreEqual(o.Tags[i].GetWord(j), clone.Tags[i].GetWord(j));
                }
                i++;
            }
        }

        [TestMethod]
        public void Next()
        {
            var firstTag = new DfTag("ONE", "a");
            var secondTag = new DfTag("TWO");
            var o = new DfObject(firstTag, secondTag);

            Assert.AreSame(secondTag, o.Next(firstTag));
        }

        [TestMethod]
        public void Next_LastOne()
        {
            var firstTag = new DfTag("ONE", "a");
            var o = new DfObject(firstTag);

            Assert.IsNull(o.Next(firstTag));
        }

        [TestMethod]
        public void Next_Unknown()
        {
            var firstTag = new DfTag("ONE", "a");
            var o = new DfObject(firstTag);

            Assert.IsNull(o.Next(new DfTag("UNKNOWN")));
        }
    }
}
