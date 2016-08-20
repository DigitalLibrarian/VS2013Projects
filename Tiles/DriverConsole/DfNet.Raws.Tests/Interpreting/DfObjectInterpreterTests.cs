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
    public class DfObjectInterpreterTests
    {
        Mock<IDfObjectStore> StoreMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            StoreMock = new Mock<IDfObjectStore>();
        }



    }
}
