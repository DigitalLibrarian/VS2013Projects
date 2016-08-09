using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles;
using System.Collections.Generic;

namespace Tiles.Tests
{
    [TestClass]
    public class ActionLogTests
    {
        [TestMethod]
        public void MaxLines()
        {
            var maxLines = 5;
            var log = new ActionLog(maxLines);
            Assert.AreEqual(maxLines, log.MaxLines);
        }

        [TestMethod]
        public void Capping()
        {
            var maxLines = 2;
            var log = new ActionLog(maxLines);

            string[] lines = new string[]{
                "line1",
                "line2"
            };

            IEnumerable<string> logged = null;
            for (int i = 0; i < maxLines; i++)
            {
                Assert.AreEqual(i, log.GetLines().Count());

                log.AddLine(lines[i]);

                logged = log.GetLines();
                Assert.AreEqual(i + 1, logged.Count());
                for (int j = 0; j < i; j++)
                {
                    logged.Contains(lines[j]);
                }
            }

            var overflowLine = "line3";
            log.AddLine(overflowLine);
            logged = log.GetLines();
            Assert.AreEqual(maxLines, logged.Count());
            Assert.IsFalse(logged.Contains(lines[0]));
            Assert.IsTrue(logged.Contains(lines[1]));
            Assert.IsTrue(logged.Contains(overflowLine));

        }
    }
}
