using DfNet.Raws.Interpreting;
using DfNet.Raws.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Tests.Interpreting
{
    [TestClass]
    public class DfCreatureVariationTests
    {
        Mock<IDfObjectStore> StoreMock { get; set; }
        Mock<IDfObjectContext> ContextMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            StoreMock = new Mock<IDfObjectStore>();
            ContextMock = new Mock<IDfObjectContext>();
        }

        [TestMethod]
        public void NoVariation()
        {
            var cvDefn = new DfObject(
                CreatureVariationTag("NO_VARIATION")
                );

            var source = new DfObject(CreatureTag("POOP_MONSTER"));
            ContextMock.Setup(x => x.Source).Returns(source);

            var cv = new DfCreatureVariation(cvDefn);

            cv.Apply(StoreMock.Object, ContextMock.Object);

            ContextMock.Verify(x => x.InsertTags(It.IsAny<DfTag[]>()), Times.Never());
            ContextMock.Verify(x => x.ReplaceTag(It.IsAny<DfTag>(), It.IsAny<DfTag>()), Times.Never());
            ContextMock.Verify(x => x.RemoveTagsByName(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void AddTag()
        {
            var cvDefn = new DfObject(
                CreatureVariationTag("NO_VARIATION"),
                CvNewTag("CHICKEN", "LITTLE"),
                CvNewTag("ODDBALL")
                );

            var source = new DfObject(
                CreatureTag("POOP_MONSTER")
            );
            ContextMock.Setup(x => x.Source).Returns(source);

            var cv = new DfCreatureVariation(cvDefn);

            cv.Apply(StoreMock.Object, ContextMock.Object);

            ContextMock.Verify(x => x.ReplaceTag(It.IsAny<DfTag>(), It.IsAny<DfTag>()), Times.Never());
            ContextMock.Verify(x => x.RemoveTagsByName(It.IsAny<string>()), Times.Never());

            ContextMock.Verify(x => x.InsertTags(It.IsAny<DfTag[]>()), Times.Exactly(2));
            ContextMock.Verify(x => x.InsertTags(It.Is<DfTag[]>(newTags =>
                newTags.Count() == 1
                && newTags.ElementAt(0).GetWords().SequenceEqual(new[] { "CHICKEN", "LITTLE" })
            )), Times.Once());
            
            ContextMock.Verify(x => x.InsertTags(It.Is<DfTag[]>(newTags =>
                newTags.Count() == 1
                && newTags.ElementAt(0).GetWords().SequenceEqual(new[] { "ODDBALL" })
            )), Times.Once());
        }


        [TestMethod]
        public void RemoveTag()
        {
            var cvDefn = new DfObject(
                CreatureVariationTag("NO_VARIATION"),
                CvRemove("CHICKEN"),
                CvRemove("ODDBALL")
                );

            var source = new DfObject(
                CreatureTag("POOP_MONSTER")
            );
            ContextMock.Setup(x => x.Source).Returns(source);

            var cv = new DfCreatureVariation(cvDefn);

            cv.Apply(StoreMock.Object, ContextMock.Object);

            ContextMock.Verify(x => x.ReplaceTag(It.IsAny<DfTag>(), It.IsAny<DfTag>()), Times.Never());
            ContextMock.Verify(x => x.InsertTags(It.IsAny<DfTag[]>()), Times.Never());

            ContextMock.Verify(x => x.RemoveTagsByName("CHICKEN"), Times.Once());
            ContextMock.Verify(x => x.RemoveTagsByName("ODDBALL"), Times.Once());
        }

        [TestMethod]
        public void ConvertTag()
        {
            var convertChickenTag = CvConvertTag();
            var convertOddballTag = CvConvertTag();
            var cvDefn = new DfObject(
                CreatureVariationTag("NO_VARIATION"),
                convertChickenTag,
                CvCt_Master("CHICKEN"),
                CvCt_Target("LITTLE"),
                CvCt_Replacement("PECK", "FAIL_TO_FLY"),
                convertOddballTag,
                CvCt_Master("ODDBALL"),
                CvCt_Target("ODDBALL"),
                CvCt_Replacement("COOLNESS", "SUPER")
                );

            var chickenTag = new DfTag("CHICKEN", "LITTLE");
            var oddBallTag = new DfTag("ODDBALL");
            var source = new DfObject(
                CreatureTag("POOP_MONSTER"),
                chickenTag,
                oddBallTag
            );
            ContextMock.Setup(x => x.Source).Returns(source);

            var cv = new DfCreatureVariation(cvDefn);

            cv.Apply(StoreMock.Object, ContextMock.Object);

            ContextMock.Verify(x => x.InsertTags(It.IsAny<DfTag[]>()), Times.Never());
            ContextMock.Verify(x => x.RemoveTagsByName(It.IsAny<string>()), Times.Never());

            ContextMock.Verify(
                x => x.ReplaceTag(It.IsAny<DfTag>(), It.IsAny<DfTag>()), Times.Exactly(2));
            ContextMock.Verify(
                x => x.ReplaceTag(
                    chickenTag, 
                    It.Is<DfTag>(t => t.GetWords().SequenceEqual(
                        new []{"CHICKEN", "PECK", "FAIL_TO_FLY"}
                        ))), 
                Times.Once());

            ContextMock.Verify(
                x => x.ReplaceTag(
                    oddBallTag,
                    It.Is<DfTag>(t => t.GetWords().SequenceEqual(
                        new []{"COOLNESS", "SUPER"}
                        ))),
                Times.Once());
        }


        #region Raws
        // TODO - move into some kind of factory
        DfTag CreatureTag(string name)
        {
            return new DfTag(DfTags.CREATURE, name);
        }
        DfTag CreatureVariationTag(string name)
        {
            return new DfTag(DfTags.CREATURE_VARIATION, name);
        }
        DfTag CvRemove(string name)
        {
            return new DfTag(DfTags.MiscTags.CV_REMOVE_TAG, name);
        }
        DfTag CvNewTag(params string[] paramWords)
        {
            return new DfTag(DfTags.MiscTags.CV_NEW_TAG, paramWords);
        }
        DfTag CvConvertTag()
        {
            return new DfTag(DfTags.MiscTags.CV_CONVERT_TAG);
        }
        DfTag CvCt_Master(string naster)
        {
            return new DfTag(DfTags.MiscTags.CVCT_MASTER, naster);
        }
        DfTag CvCt_Target(string target)
        {
            return new DfTag(DfTags.MiscTags.CVCT_TARGET, target);
        }
        DfTag CvCt_Replacement(params string[] replacements)
        {
            return new DfTag(DfTags.MiscTags.CVCT_REPLACEMENT, replacements);
        }
        #endregion
    }
}
