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
                new DfTag("D", "8"),
                new DfTag("E", "9")
            });


            var result = Parser.Parse(new string[] { testInput }, "A").ToList();
            TagParserMock.Verify(x => x.Parse(testInput), Times.Once());

            Assert.IsTrue(result.Any());
            Assert.AreEqual(2, result.Count());

            var firstA = result.ElementAt(0);
            Assert.AreEqual(4, firstA.Tags.Count());
            Assert.AreEqual("A", firstA.Tags.ElementAt(0).Name);
            Assert.AreEqual("1", firstA.Tags.ElementAt(0).GetParam(0));
            Assert.AreEqual(2, firstA.Tags.ElementAt(0).NumWords);


            Assert.AreEqual("B", firstA.Tags.ElementAt(1).Name);
            Assert.AreEqual("2", firstA.Tags.ElementAt(1).GetParam(0));
            Assert.AreEqual(2, firstA.Tags.ElementAt(1).NumWords);

            Assert.AreEqual("C", firstA.Tags.ElementAt(2).Name);
            Assert.AreEqual("3", firstA.Tags.ElementAt(2).GetParam(0));
            Assert.AreEqual(2, firstA.Tags.ElementAt(2).NumWords);
            
            Assert.AreEqual("D", firstA.Tags.ElementAt(3).Name);
            Assert.AreEqual("4", firstA.Tags.ElementAt(3).GetParam(0));
            Assert.AreEqual(2, firstA.Tags.ElementAt(3).NumWords);

            var secondA = result.ElementAt(1);
            Assert.AreEqual(5, secondA.Tags.Count());
            Assert.AreEqual("A", secondA.Tags.ElementAt(0).Name);
            Assert.AreEqual("5", secondA.Tags.ElementAt(0).GetParam(0));
            Assert.AreEqual(2, secondA.Tags.ElementAt(0).NumWords);

            Assert.AreEqual("B", secondA.Tags.ElementAt(1).Name);
            Assert.AreEqual("6", secondA.Tags.ElementAt(1).GetParam(0));
            Assert.AreEqual(2, secondA.Tags.ElementAt(1).NumWords);

            Assert.AreEqual("C", secondA.Tags.ElementAt(2).Name);
            Assert.AreEqual("7", secondA.Tags.ElementAt(2).GetParam(0));
            Assert.AreEqual(2, secondA.Tags.ElementAt(2).NumWords);

            Assert.AreEqual("D", secondA.Tags.ElementAt(3).Name);
            Assert.AreEqual("8", secondA.Tags.ElementAt(3).GetParam(0));
            Assert.AreEqual(2, secondA.Tags.ElementAt(3).NumWords);
        }

        void SetParsedTags(string line, IEnumerable<DfTag> tags)
        {
            TagParserMock.Setup(x => x.Parse(line)).Returns(tags.ToList());
        }
    }
}
