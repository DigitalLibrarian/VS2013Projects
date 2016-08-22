using DfNet.Raws.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Tests.Parsing
{
    [TestClass]
    public class DfObjectParserTests
    {
        Mock<IDfTagParser> TagParserMock { get; set; }

        DfObjectParser Parser { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            TagParserMock = new Mock<IDfTagParser>();
            Parser = new DfObjectParser(TagParserMock.Object);
        }

        [TestMethod]
        public void NoInput()
        {
            var result = Parser.Parse(new string[0], new string[0]).ToList();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void NoObjectTags()
        {
            var testInput = "bloop";
            SetParsedTags(testInput,
                new DfTag[]{
                new DfTag("A"),
                new DfTag("B"),
                new DfTag("C"),
                new DfTag("D"),
                new DfTag("A"),
                new DfTag("B"),
                new DfTag("C"),
                new DfTag("D"),
            });

            var result = Parser.Parse(new string[0], "A", "B", "C", "D").ToList();
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void SimpleCase()
        {
            var testInput = "bloop";
            SetParsedTags(testInput,
                new DfTag[]{
                new DfTag("OBJECT", "A"),
                new DfTag("A", "1"),
                new DfTag("B", "2"),
                new DfTag("C", "3"),
                new DfTag("D", "4"),
                new DfTag("A", "5"),
                new DfTag("B", "6"),
                new DfTag("C", "7"),
                new DfTag("D", "8")
            });


            var result = Parser.Parse(new string[] { testInput }, "A").ToList();
            TagParserMock.Verify(x => x.Parse(testInput), Times.Once());

            Assert.IsTrue(result.Any());
            Assert.AreEqual(2, result.Count());

        }


        void SetParsedTags(string line, IEnumerable<DfTag> tags)
        {
            TagParserMock.Setup(x => x.Parse(line)).Returns(tags.ToList());
        }
    }
}
