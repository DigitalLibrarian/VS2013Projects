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
        public void SpotCheckBodyDetailPlans()
        {
            var o = Store.Get(DfTags.BODY_DETAIL_PLAN, "STANDARD_MATERIALS");
            Assert.IsNotNull(o);
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
            AssertCastes(parsedLeo);

            Assert.AreEqual(expectedTags, parsedLeo.Tags.Count());
            AssertTagCount(parsedLeo, tag => tag.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION), 8);

            var headerTag = parsedLeo.Tags.First();
            Assert.AreEqual(DfTags.CREATURE, headerTag.Name);
            Assert.AreEqual(creatureType, headerTag.GetParam(0));

            var lastTag = parsedLeo.Tags.Last();
            Assert.AreEqual("COLOR", lastTag.Name);
            Assert.AreEqual(4, lastTag.NumWords);

            // interpretation

            var requiredCvs = DfCreatureVariationApplicator.FindRequiredCVs(parsedLeo);
            Assert.AreEqual(8, requiredCvs.Count());

            
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
            //var casteApp = new DfCasteApplicator(DfTags.MiscTags.MALE);
            DfObject result = parsedLeo;

            AssertCastes(result);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.SELECT_CASTE), 3);

            AssertSingleTag(result,
                t => t.Name.Equals("NAME")
                    && t.GetParam(0).Equals("leopard gecko man"));

            AssertTagCount(result, tag => tag.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION), 8);
            AssertNoTag(result, tag => tag.Name.Equals(DfTags.BODY));
            AssertNoTag(result, tag => tag.Name.Equals(DfTags.MiscTags.BP));
            

            
            // Apply copy to inheritance and creature variants
            ApplyPass(new DfCreatureApplicator(result), context);
            result = context.Create();

            Assert.AreEqual(DfTags.CREATURE, result.Type);
            Assert.AreEqual(creatureType, result.Name);

            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION));
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.BP_LAYERS));

            AssertSingleTag(result, tag => tag.Name.Equals(DfTags.BODY));

            AssertNoTag(result,
                t => t.GetWords()
                    .Any(word => word.Contains("QUADRUPED")));

            AssertSingleTag(result, 
                t => t.Name.Equals(DfTags.BODY) 
                        && t.GetWords().Any(word => word.Equals("HUMANOID_NECK")));

            AssertCastes(result,
                DfTags.MiscTags.FEMALE,
                DfTags.MiscTags.MALE
            );

            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.SELECT_CASTE), 5);

            ApplyPass(new DfCasteApplicator(DfTags.MiscTags.MALE), context);
            result = context.Create();            

            AssertSingleTag(result, t => t.Name.Equals("NAME"));

            AssertCastes(result);
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.SELECT_CASTE));
            AssertSingleTag(result, t => t.IsSingleWord(DfTags.MiscTags.MALE));
            AssertNoTag(result, t => t.IsSingleWord(DfTags.MiscTags.FEMALE));

            Assert.AreEqual(114, result.Tags.Count());

            Assert.AreEqual(DfTags.CREATURE, result.Type);
            Assert.AreEqual(creatureType, result.Name);

            AssertSingleTag(result, t => t.IsSingleWord(DfTags.MiscTags.MALE));
            AssertNoTag(result, t => t.IsSingleWord(DfTags.MiscTags.FEMALE));

            AssertNoTag(result, tag => tag.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION));

            AssertSingleTag(
                result,
                t => t.Name.Equals("GAIT")
                    && t.GetParam(0).Equals("CLIMB")
                    && t.GetParam(1).Equals("Scramble")
                    && t.GetParam(2).Equals("731")
                );

            AssertTagCount(result, t => t.Name.Equals(DfTags.BODY_DETAIL_PLAN), 5);
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.BP_LAYERS));
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.BP));

            // copy tags, creture variations and castes have been handled

            // what remains is body parts, tissues, and materials
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

        void ApplyPass(IContextApplicator app, IDfObjectContext context)
        {
            context.StartPass();
            app.Apply(Store, context);
            context.EndPass();
        }

        void AssertSingleTag(DfObject o, Predicate<DfTag> pred)
        {
            AssertTagCount(o, pred, 1);
        }

        void AssertNoTag(DfObject o, Predicate<DfTag> pred)
        {
            AssertTagCount(o, pred, 0);
        }

        void AssertTagCount(DfObject o, Predicate<DfTag> pred, int count)
        {
            var gaitTags = o.Tags.Where((tag) => pred(tag));

            Assert.AreEqual(count, gaitTags.Count());

        }

        void AssertCastes(DfObject o, params string[] casteNames)
        {
            var castes = DfCasteApplicator.FindCastes(o);
            Assert.IsTrue(castes.SequenceEqual(casteNames));
        }
    }
}
