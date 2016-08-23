using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Tests.Assertions;

namespace DfNet.Raws.Tests
{
    [TestClass]
    public class DfTagTests
    {
        [TestMethod]
        public void NoWordsException()
        {
            Asserter.AssertException<DfTag.NoWordsException>(() => new DfTag());
        }

        [TestMethod]
        public void OutOfRange()
        {
            var tag = new DfTag("TAG");
            Asserter.AssertException<IndexOutOfRangeException>(() => tag.GetWord(1));
            tag = new DfTag("TAG", "", "" , "");
            Asserter.AssertException<IndexOutOfRangeException>(() => tag.GetWord(4));
        }

        [TestMethod]
        public void SingleWord()
        {
            var tag = new DfTag("TAG");
            Assert.IsTrue(tag.IsSingleWord());
            Assert.IsFalse(tag.IsSingleWord("pwep"));
            Assert.IsTrue(tag.IsSingleWord("TAG"));
        }

        [TestMethod]
        public void WordWithParams()
        {
            var tag = new DfTag("TAG", new string[] { "p1", "p2" });

            Assert.IsFalse(tag.IsSingleWord());
            Assert.IsFalse(tag.IsSingleWord("TAG"));
            Assert.AreEqual(3, tag.NumWords);
            Assert.AreEqual("TAG", tag.GetWord(0));
            Assert.AreEqual("p1", tag.GetParam(0));
            Assert.AreEqual("p1", tag.GetWord(1));
            Assert.AreEqual("p2", tag.GetParam(1));
            Assert.AreEqual("p2", tag.GetWord(2));

            Asserter.AssertException<IndexOutOfRangeException>(() => tag.GetParam(2));
            Asserter.AssertException<IndexOutOfRangeException>(() => tag.GetWord(3));
        }

        [TestMethod]
        public void SeveralWords()
        {
            var words = new string[]
            {
                "Name",
                "Word1",
                "Word2",
                "Word3",
            };
            var tag = new DfTag(words);

            Assert.IsFalse(tag.IsSingleWord());
            Assert.AreEqual(words[0], tag.Name);

            int i = 0;
            foreach (var word in words)
            {
                if (i > 0)
                {
                    Assert.AreEqual(word, tag.GetParam(i - 1));
                }
                Assert.AreEqual(word, tag.GetWord(i++));
            }

            Assert.AreEqual(words.Length, tag.NumWords);

        }

        [TestMethod]
        public void CloneDfTag()
        {
            var words = new string[]
            {
                "Name",
                "Word1",
                "Word2",
                "Word3",
            };

            var tag = new DfTag(words);
            var clone = tag.CloneDfTag();

            Assert.AreEqual(tag.Name, clone.Name);

            for (int i = 0; i < words.Length;i++)
            {
                Assert.AreEqual(words[i], tag.GetWord(i));
                Assert.AreEqual(words[i], clone.GetWord(i));
            }
        }

        [TestMethod]
        public void CloneDfTagWithArgs()
        {
            string[] args = new string[]{
                "V1",
                "V2",
                "V3",
                "V4"
            };

            var tag = new DfTag(
                "TAG",
                "ARG1",
                "ARG2",
                "ARG3",
                "ARG4"
                );

            var clone = tag.CloneWithArgs("ARG", args);

            Assert.AreEqual("TAG", clone.GetWord(0));
            Assert.AreEqual("V1", clone.GetWord(1));
            Assert.AreEqual("V2", clone.GetWord(2));
            Assert.AreEqual("V3", clone.GetWord(3));
            Assert.AreEqual("V4", clone.GetWord(4));
        }

        [TestMethod]
        public void ToString_Formatting()
        {
            var tag = new DfTag("NAME", "W1", "W2", "W3");
            Assert.AreEqual("[NAME:W1:W2:W3]", tag.ToString());
        }
    }
}
