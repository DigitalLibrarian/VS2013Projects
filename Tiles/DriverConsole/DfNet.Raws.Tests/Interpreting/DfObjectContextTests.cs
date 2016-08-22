using DfNet.Raws.Interpreting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Tests.Assertions;

namespace DfNet.Raws.Tests.Interpreting
{
    [TestClass]
    public class DfObjectContextTests
    {
        [TestMethod]
        public void ClassInvariants()
        {
            var source = new DfObject(new DfTag("A", "B"));
            int cursor = 0;
            var b = new DfObjectContext(source, cursor);

            Assert.AreSame(source, b.Source);
            Assert.AreEqual(cursor, b.Cursor);
        }

        [TestMethod]
        public void Pass_RemoveTagsByName()
        {
            var source = new DfObject(
                new DfTag("A", "B"),        // 0 
                new DfTag("TARGET2"),       // 1
                new DfTag("TARGET3"),       // 2
                new DfTag("INIT_CURSOR"),   // 3
                new DfTag("TARGET1")        // 4
                );
            var b = new DfObjectContext(source, 2);

            b.StartPass();
            b.InsertTags(b.Source.Tags.ToArray());
            b.GoToTag("TARGET3");
            Assert.AreEqual(0, b.RemoveTagsByName("NOT_THERE"));
            Assert.AreEqual(2, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("TARGET1"));
            Assert.AreEqual(2, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("TARGET2"));
            Assert.AreEqual(1, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("TARGET3"));
            Assert.AreEqual(0, b.Cursor);

            Assert.AreEqual(1, b.RemoveTagsByName("INIT_CURSOR"));
            Assert.AreEqual(0, b.Cursor);
            b.EndPass();

            var result = b.Create();
            Assert.AreEqual(1, result.Tags.Count());
            Assert.AreEqual(2, result.Tags.First().NumWords);
            Assert.AreEqual("A", result.Tags.First().GetWord(0));
            Assert.AreEqual("B", result.Tags.First().GetWord(1));
        }

        [TestMethod]
        public void Pass_NoInserts()
        {
            var source = new DfObject(new DfTag("CREATURE", "MUFFIN_MAN"));
            var context = new DfObjectContext(source, 1);

            Assert.AreEqual(1, context.Cursor);

            context.StartPass();
            Assert.AreEqual(0, context.Cursor);

            Asserter.AssertException<DfObject.InvalidHeaderException>(() => context.EndPass());
        }

        [TestMethod]
        public void Pass_BasicCopy()
        {
            var headerTag = new DfTag("CREATURE", "MUFFIN_MAN");
            var source = new DfObject(headerTag);
            var context = new DfObjectContext(source, 1);

            Assert.AreEqual(1, context.Cursor);

            context.StartPass();
            Assert.AreEqual(0, context.Cursor);

            context.InsertTags(source.Tags.ToArray());

            Assert.AreEqual(1, context.Cursor);
            Assert.AreEqual(1, context.WorkingSet.Count());
            Assert.AreSame(headerTag, context.WorkingSet.First());

            context.EndPass();
            Assert.AreEqual(0, context.Cursor);
            Assert.IsFalse(context.WorkingSet.Any());

            var result = context.Create();
            Assert.IsNotNull(result);
            Assert.AreNotSame(source, result);

            Assert.AreEqual(1, result.Tags.Count());
            Assert.AreEqual(2, result.Tags.First().NumWords);
            Assert.AreEqual("CREATURE", result.Tags.First().GetWord(0));
            Assert.AreEqual("MUFFIN_MAN", result.Tags.First().GetWord(1));
        }

        [TestMethod]
        public void GoToStart()
        {
            var context = new DfObjectContext(new DfObject(new DfTag("CREATURE", "MUFFIN_MAN")), 2);

            Assert.AreEqual(2, context.Cursor);

            context.GoToStart();
            Assert.AreEqual(1, context.Cursor);
        }

        [TestMethod]
        public void GoToEnd()
        {
            var context = new DfObjectContext(new DfObject(new DfTag("CREATURE", "MUFFIN_MAN")), 2);

            Assert.AreEqual(2, context.Cursor);

            context.GoToEnd();
            Assert.AreEqual(0, context.Cursor);

            context.InsertTags(new DfTag("A"), new DfTag("B"), new DfTag("C"));
            Assert.AreEqual(3, context.Cursor);

            context.GoToStart();
            Assert.AreEqual(1, context.Cursor);

            context.GoToEnd();
            Assert.AreEqual(3, context.Cursor);
        }

        [TestMethod]
        public void GoToTag()
        {
            var context = new DfObjectContext(new DfObject(
                new DfTag("CREATURE", "MUFFIN_MAN"),
                new DfTag("DELICIOUS"),
                new DfTag("RUNS_FAST")
                ));

            context.StartPass();
            context.InsertTags(context.Source.Tags.ToArray());
            Assert.AreEqual(3, context.Cursor);

            var result = context.GoToTag("DELICIOUS");
            Assert.AreEqual(1, context.Cursor);
            Assert.IsTrue(result);

            result = context.GoToTag("NOT_THERE");
            Assert.AreEqual(1, context.Cursor);
            Assert.IsFalse (result);

            result = context.GoToTag("CREATURE");
            Assert.AreEqual(0, context.Cursor);
            Assert.IsTrue(result);
            context.EndPass();
        }

        [TestMethod]
        public void Pass_CopyTagsFrom()
        {
            var source = new DfObject(new DfTag("CREATURE", "MUFFIN_MAN"));
            var context = new DfObjectContext(source);

            var copySource = new DfObject(
                new DfTag("CREATURE", "GINGERBREAD_MAN"),
                new DfTag("SOME_ARBITRARY_TAG1"),
                new DfTag(DfTags.MiscTags.APPLY_CREATURE_VARIATION),
                new DfTag("SOME_ARBITRARY_TAG2")
                );

            context.StartPass();
            Assert.AreEqual(0, context.Cursor);
            context.InsertTags(source.Tags.First());
            Assert.AreEqual(1, context.Cursor);
            context.CopyTagsFrom(copySource);
            Assert.AreEqual(3, context.Cursor);
            context.EndPass();

            var result = context.Create();

            Assert.AreEqual(3, result.Tags.Count());

            Assert.AreEqual(2, result.Tags[0].NumWords);
            Assert.AreEqual("CREATURE", result.Tags[0].GetWord(0));
            Assert.AreEqual("MUFFIN_MAN", result.Tags[0].GetWord(1));

            Assert.AreEqual(1, result.Tags[1].NumWords);
            Assert.AreEqual("SOME_ARBITRARY_TAG1", result.Tags[1].GetWord(0));

            Assert.AreEqual(1, result.Tags[2].NumWords);
            Assert.AreEqual("SOME_ARBITRARY_TAG2", result.Tags[2].GetWord(0));
        }

        [TestMethod]
        public void Pass_Remove()
        {
            var headerTag = new DfTag("CREATURE", "MUFFIN_MAN");
            var tag1 = new DfTag("ONE");
            var tag2 = new DfTag("TWO");
            var tag3 = new DfTag("THREE");
            var tag4 = new DfTag("FOUR");
            var source = new DfObject(headerTag, tag1, tag2, tag3, tag4);
            var context = new DfObjectContext(source);

            context.StartPass();
            context.InsertTags(source.Tags.ToArray());
            Assert.AreEqual(5, context.Cursor);

            Assert.IsTrue(context.WorkingSet.Contains(headerTag));
            Assert.IsTrue(context.WorkingSet.Contains(tag1));
            Assert.IsTrue(context.WorkingSet.Contains(tag2));
            Assert.IsTrue(context.WorkingSet.Contains(tag3));
            Assert.IsTrue(context.WorkingSet.Contains(tag4));

            context.Remove(tag2);
            Assert.AreEqual(4, context.Cursor);

            Assert.IsTrue(context.WorkingSet.Contains(headerTag));
            Assert.IsTrue(context.WorkingSet.Contains(tag1));
            Assert.IsFalse(context.WorkingSet.Contains(tag2));
            Assert.IsTrue(context.WorkingSet.Contains(tag3));
            Assert.IsTrue(context.WorkingSet.Contains(tag4));

            var unknownTag = new DfTag("UNKNOWN");
            context.Remove(unknownTag);
            Assert.AreEqual(4, context.Cursor);

            Assert.IsTrue(context.WorkingSet.Contains(headerTag));
            Assert.IsTrue(context.WorkingSet.Contains(tag1));
            Assert.IsFalse(context.WorkingSet.Contains(tag2));
            Assert.IsTrue(context.WorkingSet.Contains(tag3));
            Assert.IsTrue(context.WorkingSet.Contains(tag4));
        }

        [TestMethod]
        public void Pass_ReplaceTag()
        {
            var headerTag = new DfTag("CREATURE", "MUFFIN_MAN");
            var tag1 = new DfTag("ONE");
            var tag2 = new DfTag("TWO");
            var tag3 = new DfTag("THREE");
            var tag4 = new DfTag("FOUR");
            var source = new DfObject(headerTag, tag1, tag2, tag3, tag4);
            var context = new DfObjectContext(source);

            context.StartPass();
            context.InsertTags(source.Tags.ToArray());
            Assert.AreEqual(5, context.Cursor);

            Assert.IsTrue(context.WorkingSet.Contains(headerTag));
            Assert.IsTrue(context.WorkingSet.Contains(tag1));
            Assert.IsTrue(context.WorkingSet.Contains(tag2));
            Assert.IsTrue(context.WorkingSet.Contains(tag3));
            Assert.IsTrue(context.WorkingSet.Contains(tag4));

            var newTag = new DfTag("NEW", "TAG");

            var result = context.ReplaceTag(newTag, newTag);
            Assert.IsFalse(result);
            Assert.AreEqual(5, context.Cursor);

            result = context.ReplaceTag(tag2, newTag);
            Assert.AreEqual(5, context.Cursor);

            Assert.IsTrue(context.WorkingSet.Contains(headerTag));
            Assert.IsTrue(context.WorkingSet.Contains(tag1));
            Assert.IsFalse(context.WorkingSet.Contains(tag2));
            Assert.IsTrue(context.WorkingSet.Contains(tag3));
            Assert.IsTrue(context.WorkingSet.Contains(tag4));
            Assert.IsTrue(context.WorkingSet.Contains(newTag));

            context.EndPass();

            var creation = context.Create();

            Assert.AreEqual(5, creation.Tags.Count());

            Assert.IsFalse(creation.Tags.Any(t => t.Name.Equals("TWO")));
            Assert.AreEqual(1, creation.Tags.Count(t => t.Name.Equals("NEW") && t.GetParam(0).Equals("TAG")));
        }
    }
}
