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
        public void CloneWithArgs_CvNewTag()
        {
            var args = new string[]{
                "V1", "V2", "V3"
            };

            var cvExample = new DfObject(
                new DfTag(
                    "CV_NEW_TAG",
                    "GAIT",
                    "WALK",
                    "Fast Crawl",
                    "!ARG2",
                    "NO_BUILD_UP"
                ),

                new DfTag(
                    "CV_NEW_TAG",
                    "GAIT",
                    "WALK",
                    "Crawl",
                    "!ARG1",
                    "NO_BUILD_UP"
                    ));

            var clone = cvExample.CloneDfObjectWithArgs("!ARG", args);

            int diffIndex = 4;

            Assert.AreNotSame(cvExample, clone);
            Assert.AreEqual(cvExample.Tags.Count(), clone.Tags.Count());
            Assert.AreNotSame(cvExample, clone);
            for (int i = 0; i < cvExample.Tags.Count();i++ )
            {
                Assert.AreNotSame(cvExample.Tags[i], clone.Tags[i]);
                for (int j = 0; j < cvExample.Tags[i].NumWords; j++)
                {
                    if (j != diffIndex)
                    {
                        Assert.AreEqual(
                            cvExample.Tags[i].GetWord(j),
                            clone.Tags[i].GetWord(j));
                    }
                    else
                    {
                        Assert.AreNotEqual(
                            cvExample.Tags[i].GetWord(j),
                            clone.Tags[i].GetWord(j));
                    }
                }
            }


            Assert.AreEqual(
                clone.Tags[0].GetWord(diffIndex),
                args[1]);

            Assert.AreEqual(
                clone.Tags[1].GetWord(diffIndex),
                args[0]);
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
