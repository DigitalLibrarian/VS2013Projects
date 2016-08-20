using DfNet.Raws.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Tests.Assertions;

namespace DfNet.Raws.Tests.Parsing
{
    [TestClass]
    public class DfObjectParserTests
    {
        private const string DirKey = @"DwarfFortressRawsDirectory";
        DfObjectParser Parser { get; set; }

        Dictionary<string, IEnumerable<string>> Raws { get; set; }
        Dictionary<string, int> ExpectedCounts = new Dictionary<string, int>
        {
            {DfTags.CREATURE, 780},
            {DfTags.CREATURE_VARIATION, 31},
            {DfTags.BODY, 624},
            {DfTags.BODY_DETAIL_PLAN, 2251},
            {DfTags.MATERIAL_TEMPLATE, 69},
            {DfTags.ITEM_WEAPON, 23},
            {DfTags.TISSUE_TEMPLATE, 37}
        };
        
        [TestInitialize]
        public void Initialize()
        {
            Parser = new DfObjectParser();
            var rawDirPath = ConfigurationManager.AppSettings.Get(DirKey);
            Raws = Directory.GetFiles(rawDirPath, "*", SearchOption.AllDirectories)
                .ToDictionary(
                    fileName => fileName,
                    fileName => File.ReadLines(fileName));
        }

        [TestMethod]
        public void TestDbCounts()
        {
            foreach (var objType in ExpectedCounts.Keys)
            {
                TestCount(objType);
            }
        }

        [TestMethod]
        public void ParseAllTypes()
        {
            var lines = Raws.SelectMany(x => x.Value);
            var all = Parser.Parse(lines, DfTags.GetAllObjectTypes()).ToList();
            Assert.IsTrue(all.Any());
        }

        
        void TestCount(string objectType)
        {
            var expected = ExpectedCounts[objectType];
            var lines = Raws.SelectMany(x => x.Value);
            var objects = Parser.Parse(lines, objectType);
            var actual = objects.Count();
            Assert.AreEqual(expected, actual,
                string.Format("{0} - expected {1}, actual {2}", 
                objectType, expected, actual));
        }

        [TestMethod]
        public void LeopardGeckoMan()
        {
            int expectedTags = 24;
            var file = Raws.Keys.Single(f => f.EndsWith("creature_desert_new.txt"));

            var creatureType = "LEOPARD_GECKO_MAN";
            var leo = Parser.Parse(File.ReadLines(file), DfTags.CREATURE)
                .Single(x => x.Tags.First().GetParam(0).Equals(creatureType));

            Assert.AreEqual(expectedTags, leo.Tags.Count());

            var headerTag = leo.Tags.First();
            Assert.AreEqual(DfTags.CREATURE, headerTag.Name);
            Assert.AreEqual(creatureType, headerTag.GetParam(0));

            var lastTag = leo.Tags.Last();
            Assert.AreEqual("COLOR", lastTag.Name);
            Assert.AreEqual(4, lastTag.NumWords);
        }
    }
}
