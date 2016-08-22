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
using DfNet.Raws.Interpreting.Applicators;
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
            {DfTags.CREATURE, 781},
            {DfTags.CREATURE_VARIATION, 32},
            {DfTags.BODY, 625},
            {DfTags.BODY_DETAIL_PLAN, 2252},
            {DfTags.MATERIAL_TEMPLATE, 70},
            {DfTags.ITEM_WEAPON, 24},
            {DfTags.TISSUE_TEMPLATE, 38}
        };


        IDfObjectStore Store { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            Parser = new DfObjectParser(new DfTagParser());
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

        DfObject RunCreaturePipeline(string creatureType, string caste = null)
        {
            var parsedLeo = Store.Get(DfTags.CREATURE, creatureType);

            var context = new DfObjectContext(parsedLeo);
            ApplyPass(new DfCreatureApplicator(parsedLeo), context);

            if (caste != null)
            {
                ApplyPass(new DfCasteApplicator(caste), context);
            }

            ApplyPass(new DfBodyApplicator(), context);
            ApplyPass(new DfMaterialApplicator(), context);
            ApplyPass(new DfTissueApplicator(), context);
            
            return context.Create();
        }

        [TestMethod]
        public void PipelineAllCreatures()
        {
            DfObject o = null;
            foreach (var creatureDf in Store.Get(DfTags.CREATURE))
            {
                var castes = DfCasteApplicator.FindCastes(creatureDf);
                if (!castes.Any())
                {
                    o = RunCreaturePipeline(creatureDf.Name);
                    Assert.AreEqual(creatureDf.Name, o.Name);
                }
                else
                {
                    foreach (var caste in castes)
                    {
                        o = RunCreaturePipeline(creatureDf.Name, caste);
                        Assert.AreEqual(creatureDf.Name, o.Name);
                    }
                }
            }
        }

        [TestMethod]
        public void Worm()
        {
            var result = RunCreaturePipeline("WORM");

            AssertNoTag(result, t => t.IsSingleWord(DfTags.MiscTags.FEMALE));
            AssertNoTag(result, t => t.IsSingleWord(DfTags.MiscTags.MALE));
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.CASTE));

            var casteNameTag = AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.CASTE_NAME));
            Assert.AreEqual(casteNameTag.GetParam(0), "worm");
            Assert.AreEqual(casteNameTag.GetParam(1), "worms");

            AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.BP_LAYERS)
                && t.GetParam(0).Equals(DfTags.MiscTags.BY_CATEGORY)
                && t.GetParam(1).Equals("PINCER"));
        }

        [TestMethod]
        public void PenquinWoman()
        {
            var result = RunCreaturePipeline("PENGUIN MAN", DfTags.MiscTags.FEMALE);

            AssertSingleTag(result, t => t.IsSingleWord(DfTags.MiscTags.FEMALE));
            AssertNoTag(result, t => t.IsSingleWord(DfTags.MiscTags.MALE));

            var casteNameTag = AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.CASTE_NAME));
            Assert.AreEqual(casteNameTag.GetParam(0), "penguin woman");
            Assert.AreEqual(casteNameTag.GetParam(1), "penguin women");
        }


        [TestMethod]
        public void BushmasterMan()
        {
            var result = RunCreaturePipeline("BUSHMASTER_MAN", DfTags.MiscTags.MALE);

            AssertSingleTag(result, t => t.IsSingleWord(DfTags.MiscTags.MALE));
            AssertNoTag(result, t => t.IsSingleWord(DfTags.MiscTags.FEMALE));

            var casteNameTag = AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.CASTE_NAME));
            Assert.AreEqual(casteNameTag.GetParam(0), "bushmaster man");
            Assert.AreEqual(casteNameTag.GetParam(1), "bushmaster men");

            var selfImmunityTag = AssertSingleTag(result, t => t.Name.Equals("SYN_IMMUNE_CREATURE"));
            Assert.AreEqual("BUSHMASTER_MAN", selfImmunityTag.GetParam(0));
            Assert.AreEqual("ALL", selfImmunityTag.GetParam(1));


            var venomStart = AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL)
                    && t.GetParam(0).Equals("VENOM"));

            var syndromeTag = AssertSingleTag(result, t => t.IsSingleWord("SYNDROME"));
            Assert.AreEqual("bushmaster bite", result.Next(syndromeTag).GetParam(0));
            AssertSingleTag(result, t => t.IsSingleWord("ENTERS_BLOOD"));
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

            // copy tags, creture variations and castes have been handled

            // what remains is body parts, tissues, and materials

            var bdpTags = result.Tags.Where(t => t.Name.Equals(DfTags.BODY_DETAIL_PLAN));
            var dbpNames = bdpTags.Select(x => x.GetParam(0));
            Assert.IsTrue(dbpNames.SequenceEqual<string>(new string[]{
                "STANDARD_MATERIALS",
                "STANDARD_TISSUES",
                "VERTEBRATE_TISSUE_LAYERS",
                "STANDARD_HEAD_POSITIONS",
                "HUMANOID_RIBCAGE_POSITIONS"
            }));


            AssertTagCount(result, t => t.Name.Equals(DfTags.BODY_DETAIL_PLAN), 5);
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.BP_LAYERS));
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.BP));

            ApplyPass(new DfBodyApplicator(), context);
            result = context.Create();


            AssertTagCount(result, t => t.Name.Equals(DfTags.BODY_DETAIL_PLAN), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.BP_LAYERS), 65);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.BP), 16);

            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.END_MATERIAL), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE), 4);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.START_TISSUE), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.END_TISSUE), 0);

            ApplyPass(new DfMaterialApplicator(), context);
            result = context.Create();

            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL), 23);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.END_MATERIAL), 23);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.START_TISSUE), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.END_TISSUE), 0);

            // this one should have been removed in base class
            AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL)
                    && t.GetParam(0).Equals("SKIN"));

            // added by base class
            AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL)
                    && t.GetParam(0).Equals("SCALE"));

            ApplyPass(new DfTissueApplicator(), context);
            result = context.Create();


            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL), 23);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.END_MATERIAL), 23);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE), 0);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.START_TISSUE), 18);
            AssertTagCount(result, t => t.Name.Equals(DfTags.MiscTags.END_TISSUE), 18);


            AssertSingleTag(result,
                t => t.Name.Equals(DfTags.MiscTags.BP)
                    && t.GetParam(0).Equals("LLL"));
            
            var bpLayers = AssertSingleTag(result, 
                t => t.Name.Equals(DfTags.MiscTags.BP_LAYERS)
                    && t.GetParam(0).Equals(DfTags.MiscTags.BY_CATEGORY)
                    && t.GetParam(1).Equals("LEG_UPPER"));
            
	        //[BODY_DETAIL_PLAN:VERTEBRATE_TISSUE_LAYERS:SCALE:FAT:MUSCLE:BONE:CARTILAGE]
            //[BP_LAYERS:BY_CATEGORY:LEG_UPPER:ARG4:25:ARG3:25:ARG2:5:ARG1:1]
            Assert.IsTrue(bpLayers.GetParams().Skip(2).SequenceEqual
               (
               new string[] { 
                        "BONE", "25",
                        "MUSCLE", "25",
                        "FAT", "5",
                        "SCALE", "1"
                    }
               ));

            foreach (var tissueName in new[] { "BONE", "MUSCLE", "FAT", "SCALE" })
            {
                AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.START_TISSUE)
                    && t.GetParam(0).Equals(tissueName));

                AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.END_TISSUE)
                    && t.GetParam(0).Equals(tissueName));

                AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL)
                        && t.GetParam(0).Equals(tissueName));

                AssertSingleTag(result, t => t.Name.Equals(DfTags.MiscTags.END_MATERIAL)
                        && t.GetParam(0).Equals(tissueName));
            }

            foreach (var tissueName in new[] { "SKIN", "HAIR", "LEATHER", "PARCHMENT" })
            {
                AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.START_TISSUE)
                    && t.GetParam(0).Equals(tissueName));

                AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.END_TISSUE)
                    && t.GetParam(0).Equals(tissueName));

                AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL)
                        && t.GetParam(0).Equals(tissueName));

                AssertNoTag(result, t => t.Name.Equals(DfTags.MiscTags.END_MATERIAL)
                        && t.GetParam(0).Equals(tissueName));
            }

            AssertSingleTag(
                result,
                t => t.Name.Equals("GAIT")
                    && t.GetParam(0).Equals("CLIMB")
                    && t.GetParam(1).Equals("Scramble")
                    && t.GetParam(2).Equals("731")
                );

            var pipeLined = RunCreaturePipeline(creatureType, DfTags.MiscTags.MALE);
            Assert.AreEqual(pipeLined.Tags.Count(), result.Tags.Count());
            for(int i = 0; i < result.Tags.Count(); i++)
            {
                Assert.IsTrue(
                    result.Tags[i].GetWords().SequenceEqual(pipeLined.Tags[i].GetWords()));
            }
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

        DfTag AssertSingleTag(DfObject o, Predicate<DfTag> pred)
        {
            var result = AssertTagCount(o, pred, 1);
            return result.Single();
        }

        void AssertNoTag(DfObject o, Predicate<DfTag> pred)
        {
            AssertTagCount(o, pred, 0);
        }

        IEnumerable<DfTag> AssertTagCount(DfObject o, Predicate<DfTag> pred, int count)
        {
            var gaitTags = o.Tags.Where((tag) => pred(tag));

            Assert.AreEqual(count, gaitTags.Count());
            return gaitTags;
        }

        void AssertCastes(DfObject o, params string[] casteNames)
        {
            var castes = DfCasteApplicator.FindCastes(o);
            Assert.IsTrue(castes.SequenceEqual(casteNames));
        }
    }
}
