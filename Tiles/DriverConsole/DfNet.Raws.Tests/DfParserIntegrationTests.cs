using DfNet.Raws.Interpreting;
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
using DfNet.Raws.Parsing;
using DfNet.Raws.Interpreting;
namespace DfNet.Raws.Tests
{
    // TODO - label this as integration test and make true unit tests for the parser
    [TestClass]
    public class DfParserIntegrationTests
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


        IDfObjectStore Store { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            Parser = new DfObjectParser();
            var rawDirPath = ConfigurationManager.AppSettings.Get(DirKey);
            Raws = Directory.GetFiles(rawDirPath, "*", SearchOption.AllDirectories)
                .ToDictionary(
                    fileName => fileName,
                    fileName => File.ReadLines(fileName));

            Store = new DfObjectStore(
                Parser.Parse(Raws.SelectMany(x => x.Value), 
                DfTags.GetAllObjectTypes()));
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
            var allTypes = DfTags.GetAllObjectTypes();
            var lines = Raws.SelectMany(x => x.Value);
            var all = Parser.Parse(lines, allTypes).ToList();
            Assert.IsTrue(all.Any());

            int distinctTypes = allTypes.Count();
            Assert.AreEqual(distinctTypes, all.Select(x => x.Type).Distinct().Count());
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
            var lines = Raws.SelectMany(x => x.Value);

            var creatureType = "LEOPARD_GECKO_MAN";
            var parsedLeo = Store.Get(DfTags.CREATURE, creatureType);

            Assert.AreEqual(expectedTags, parsedLeo.Tags.Count());

            var headerTag = parsedLeo.Tags.First();
            Assert.AreEqual(DfTags.CREATURE, headerTag.Name);
            Assert.AreEqual(creatureType, headerTag.GetParam(0));

            var lastTag = parsedLeo.Tags.Last();
            Assert.AreEqual("COLOR", lastTag.Name);
            Assert.AreEqual(4, lastTag.NumWords);

            // interpretation

            var requiredCvs = DfCreatureVariationApplicator.FindRequiredCVs(parsedLeo);
            Assert.AreEqual(8, requiredCvs.Count());

            var variationNames = requiredCvs.Select(
                name => Parser.Parse(lines, DfTags.CREATURE_VARIATION)
                    .Single(x => x.Name.Equals(name)));
            Assert.AreEqual(8, variationNames.Count());


            var reqCreatureNames = DfCreatureVariationApplicator.FindRequiredCreature(parsedLeo);
            Assert.AreEqual(1, reqCreatureNames.Count());
            Assert.AreEqual("GECKO_LEOPARD", reqCreatureNames.First());

            var reqCreatures = reqCreatureNames.Select(reqType => Store.Get(DfTags.CREATURE, reqType));
            Assert.AreEqual(1, reqCreatures.Count());


            var castes = reqCreatures.SelectMany(x => DfCasteApplicator.FindCastes(x));
            Assert.AreEqual(2, castes.Count());
            Assert.IsTrue(castes.SequenceEqual(new string[]{
                DfTags.MiscTags.FEMALE,
                DfTags.MiscTags.MALE
            }));


            var context = new DfObjectContext(parsedLeo);
            var casteApp = new DfCasteApplicator(DfTags.MiscTags.MALE);
            DfObject result = parsedLeo;


            castes = DfCasteApplicator.FindCastes(result);
            Assert.AreEqual(0, castes.Count());
            
            context.StartPass();
            casteApp.Apply(Store, context);
            context.EndPass();

            result = context.Create();

            castes = DfCasteApplicator.FindCastes(result);
            Assert.AreEqual(0, castes.Count());
            Assert.AreEqual("leopard gecko man",
                result.Tags.Single(x => x.Name.Equals("NAME")).GetParam(0));


            var creatureApp = new DfCreatureApplicator(result);

            context.StartPass();
            creatureApp.Apply(Store, context);
            context.EndPass();

            result = context.Create();

            Assert.IsNotNull(result);
            Assert.AreEqual(DfTags.CREATURE, result.Type);
            Assert.AreEqual(creatureType, result.Name);

            castes = DfCasteApplicator.FindCastes(result);

            Assert.IsTrue(castes.SequenceEqual(new string[]{
                DfTags.MiscTags.FEMALE,
                DfTags.MiscTags.MALE
            }));


            var bodyTag = result.Tags.SingleOrDefault(tag => tag.Name.Equals(DfTags.BODY));
            Assert.IsNotNull(bodyTag);

            Assert.IsFalse(bodyTag.GetWords().Any(word => word.Equals("QUADRUPED_NECK")));
            Assert.IsTrue(bodyTag.GetWords().Any(word => word.Equals("HUMANOID_NECK")));


            context.StartPass();
            casteApp.Apply(Store, context);
            context.EndPass();
            result = context.Create();

            Assert.IsNotNull(result);
            Assert.AreEqual(DfTags.CREATURE, result.Type);
            Assert.AreEqual(creatureType, result.Name);


            Assert.AreEqual("leopard gecko man", 
                result.Tags.Single(x => x.Name.Equals("NAME")).GetParam(0));

            castes = DfCasteApplicator.FindCastes(result);
            Assert.AreEqual(0, castes.Count());


            Assert.IsTrue(result.Tags.Any(tag => tag.IsSingleWord(DfTags.MiscTags.MALE)));
            Assert.IsFalse(result.Tags.Any(tag => tag.IsSingleWord(DfTags.MiscTags.FEMALE)));

            Assert.AreEqual(DfTags.CREATURE, result.Type);
            Assert.AreEqual(creatureType, result.Name);
            Assert.AreEqual(118, result.Tags.Count());
        }


        [TestMethod]
        public void AnimalPeopleCastesAreInherited()
        {
            var variateds = Store.Get(DfTags.CREATURE).Where(
                x => x.Tags.Any(
                    tag => tag.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION)
                    && tag.GetParam(0).Equals("ANIMAL_PERSON")));

            Assert.AreEqual(166, variateds.Count());
            foreach (var v in variateds)
            {
                var castes = DfCasteApplicator.FindCastes(v);
                Assert.IsFalse(castes.Any(), 
                    string.Format("{0}, {1} has {2} castes.", v.Name, v.Tags.First().GetParam(0), castes.Count()));
            }
        }


        [TestMethod]
        public void AnimalPeopleCopyTagsFrom()
        {
            var animalPeople = Store.Get(DfTags.CREATURE).Where(
                x => x.Tags.Any(
                    tag => tag.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION)
                    && tag.GetParam(0).Equals("ANIMAL_PERSON")));

            Assert.AreEqual(166, animalPeople.Count());
            foreach (var v in animalPeople)
            {
                var copyTag = v.Tags.SingleOrDefault(tag => tag.NumWords == 2
                    && tag.Name.Equals(DfTags.MiscTags.COPY_TAGS_FROM));

                Assert.IsNotNull(copyTag);

                var parent = Store.Get(DfTags.CREATURE, copyTag.GetParam(0));
                Assert.IsNotNull(parent);
            }
        }
    }
}
