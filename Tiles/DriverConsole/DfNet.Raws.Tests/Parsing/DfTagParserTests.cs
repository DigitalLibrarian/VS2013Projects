using DfNet.Raws.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Tests.Parsing
{
    [TestClass]
    public class DfTagParserTests
    {
        DfTagParser Parser = new DfTagParser();

        [TestMethod]
        public void NoTags()
        {
            IEnumerable<DfTag> tags = Parser.Parse("").ToList();
            Assert.IsFalse(tags.Any());

            tags = Parser.Parse("gibbity gabbity doo");
            Assert.IsFalse(tags.Any());

            tags = Parser.Parse("\n\n\n\n]asdf:1:2:3[");
            Assert.IsFalse(tags.Any());
        }

        [TestMethod]
        public void SingleTag()
        {
            IEnumerable<DfTag> tags = Parser.Parse("[OBJECT:NINJA_DUCK]").ToList();
            Assert.AreEqual(1, tags.Count());

            var tag = tags.Single();
            Assert.AreEqual("OBJECT", tag.Name);
            Assert.AreEqual("NINJA_DUCK", tag.GetWord(1));
        }


        [TestMethod]
        public void MultipleTags()
        {
            string line = "\t[BENIGN][MEANDERER][NATURAL][PET_EXOTIC]";
            IEnumerable<DfTag> tags = Parser.Parse(line).ToList();

            Assert.AreEqual(4, tags.Count());
            Assert.IsTrue(tags.ElementAt(0).IsSingleWord("BENIGN"));
            Assert.IsTrue(tags.ElementAt(1).IsSingleWord("MEANDERER"));
            Assert.IsTrue(tags.ElementAt(2).IsSingleWord("NATURAL"));
            Assert.IsTrue(tags.ElementAt(3).IsSingleWord("PET_EXOTIC"));
        }

    }
}
