using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Tests
{
    [TestClass]
    public class DfTagsTests
    {
        [TestMethod]
        public void AllObjectTypes()
        {
            var types = DfTags.GetAllObjectTypes();
            Assert.IsTrue(types.Any());
        }
    }
}
