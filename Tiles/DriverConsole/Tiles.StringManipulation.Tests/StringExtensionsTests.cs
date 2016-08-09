using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tiles.StringManipulation.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void WrapLine_SmallString()
        {
            var result = "1234567890".WrapLine(10).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1234567890", result.First());
        }

        [TestMethod]
        public void WrapLine_BigString()
        {
            var result = "1234567890".WrapLine(3).ToList();

            Assert.AreEqual(4, result.Count());
            Assert.AreEqual("123", result[0]);
            Assert.AreEqual("456", result[1]);
            Assert.AreEqual("789", result[2]);
            Assert.AreEqual("0", result[3]);
        }

        [TestMethod]
        public void WrapText()
        {
            var input = new List<string>{
                "1234567890",
                "a", "b", 
                "cadabra",
                "d"
            };

            var result = input.WrapText(3).ToList();

            Assert.AreEqual(10, result.Count());
            Assert.AreEqual("123", result[0]);
            Assert.AreEqual("456", result[1]);
            Assert.AreEqual("789", result[2]);
            Assert.AreEqual("0", result[3]);
            Assert.AreEqual("a", result[4]);
            Assert.AreEqual("b", result[5]);
            Assert.AreEqual("cad", result[6]);
            Assert.AreEqual("abr", result[7]);
            Assert.AreEqual("a", result[8]);
            Assert.AreEqual("d", result[9]);
        }
    }
}
