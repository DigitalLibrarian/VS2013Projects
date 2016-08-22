using DfNet.Raws.Interpreting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Tests
{
    [TestClass]
    public class DfObjectInterpreterTests
    {
        Mock<IDfTagInterpreter> TagInt1Mock { get; set; }
        Mock<IDfTagInterpreter> TagInt2Mock { get; set; }

        DfObjectInterpreter ObjectInterpreter { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            TagInt1Mock = new Mock<IDfTagInterpreter>();
            TagInt2Mock = new Mock<IDfTagInterpreter>();

            ObjectInterpreter = new DfObjectInterpreter(TagInt1Mock.Object, TagInt2Mock.Object);
        }


        [TestMethod]
        public void InsertMisses()
        {
            var tagA = new DfTag("A");
            var tagB = new DfTag("B");
            var tagC = new DfTag("C");
            var tags = new List<DfTag> {tagA, tagB, tagC};

            TagInt1Mock.Setup(x => x.TagName).Returns("A");
            TagInt2Mock.Setup(x => x.TagName).Returns("NOPE");

            var storeMock = new Mock<IDfObjectStore>();
            var contextMock = new Mock<IDfObjectContext>();

            ObjectInterpreter.Interpret(storeMock.Object, contextMock.Object, tags, insertMisses: true);

            contextMock.Verify(x => x.InsertTags(It.IsAny<DfTag[]>()), Times.Exactly(2));
            contextMock.Verify(x => x.InsertTags(It.Is<DfTag[]>(
                inTags => inTags.Count() == 1
                    && inTags.First() == tagB
                )), Times.Once);
            contextMock.Verify(x => x.InsertTags(It.Is<DfTag[]>(
                inTags => inTags.Count() == 1
                    && inTags.First() == tagC
                )), Times.Once);

            TagInt1Mock.Verify(x => x.Run(storeMock.Object, contextMock.Object, tagA, tags), Times.Once());
            TagInt2Mock.Verify(x => x.Run(
                It.IsAny<IDfObjectStore>(),
                It.IsAny<IDfObjectContext>(),
                It.IsAny<DfTag>(),
                It.IsAny<IList<DfTag>>()
                ), Times.Never());
        }

        [TestMethod]
        public void DoNotInsertMisses()
        {
            var tagA = new DfTag("A");
            var tagB = new DfTag("B");
            var tagC = new DfTag("C");
            var tags = new List<DfTag> { tagA, tagB, tagC };

            TagInt1Mock.Setup(x => x.TagName).Returns("A");
            TagInt2Mock.Setup(x => x.TagName).Returns("NOPE");

            var storeMock = new Mock<IDfObjectStore>();
            var contextMock = new Mock<IDfObjectContext>();

            ObjectInterpreter.Interpret(storeMock.Object, contextMock.Object, tags, insertMisses: false);

            contextMock.Verify(x => x.InsertTags(It.IsAny<DfTag[]>()), Times.Never());

            TagInt1Mock.Verify(x => x.Run(storeMock.Object, contextMock.Object, tagA, tags), Times.Once());
            TagInt2Mock.Verify(x => x.Run(
                It.IsAny<IDfObjectStore>(),
                It.IsAny<IDfObjectContext>(),
                It.IsAny<DfTag>(),
                It.IsAny<IList<DfTag>>()
                ), Times.Never());
        }

    }
}
