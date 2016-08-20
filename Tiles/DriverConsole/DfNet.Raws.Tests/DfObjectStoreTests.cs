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
    public class DfObjectStoreTests
    {
        [TestMethod]
        public void Storage()
        {
            var store = new DfObjectStore();

            DfObject o = new DfObject(new List<DfTag> { 
                new DfTag("type", "name")
            });

            store.Add(o);

            var result = store.Get("type", "name");
            Assert.AreSame(o, result);

            var byType = store.Get("type");
            Assert.AreSame(o, byType.Single());
        }

        [TestMethod]
        public void Retrieval()
        {
            DfObject o = new DfObject(new List<DfTag> { 
                new DfTag("type", "name")
            });
            var store = new DfObjectStore(new[] { o });

            var result = store.Get("type", "name");
            Assert.AreSame(o, result);

            var byType = store.Get("type");
            Assert.AreSame(o, byType.Single());
        }

        [TestMethod]
        public void DuplicateObjectNames()
        {
            DfObject o = new DfObject(new List<DfTag> { 
                new DfTag("type", "name")
            });
            var store = new DfObjectStore();

            store.Add(o);

            Asserter.AssertException<DuplicateDfObjectNameException>(() => store.Add(o));
        }

        [TestMethod]
        public void TypeMiss()
        {
            var store = new DfObjectStore();

            Assert.IsFalse(store.Get("nope").Any());
        }

        [TestMethod]
        public void NameMiss()
        {
            DfObject o = new DfObject(new List<DfTag> { 
                new DfTag("type", "name")
            });

            var store = new DfObjectStore(new[] { o });

            Assert.IsNull(store.Get("type", "wrong name"));
        }
    }
}
