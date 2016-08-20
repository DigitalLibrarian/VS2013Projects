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
        public void CloneTag()
        {
            var words = new string[]
            {
                "Name",
                "Word1",
                "Word2",
                "Word3",
            };

            var tag = new DfTag(words);
            var clone = tag.CloneTag();

            Assert.AreEqual(tag.Name, clone.Name);

            for (int i = 0; i < words.Length;i++)
            {
                Assert.AreEqual(words[i], tag.GetWord(i));
                Assert.AreEqual(words[i], clone.GetWord(i));
            }
        }
    }
}
