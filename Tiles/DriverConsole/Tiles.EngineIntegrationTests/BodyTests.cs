using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DfNet.Raws;
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;
using Tiles.ScreensImpl.SiteFactories;
using Tiles.Ecs;
using Tiles.Random;
using Tiles.Agents;
using Tiles.Math;
using Moq;
using Tiles.Items;
using Tiles.Agents.Combat;
using Tiles.Injuries;
using Tiles.Materials;
using Tiles.Bodies;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class BodyTests
    {
        IRandom Random { get; set; }

        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IContentMapper ContentMapper { get; set; }

        IEntityManager EntityManager { get; set; }
        DfTagsFascade DfTagsFascade { get; set; }

        IAtlas Atlas { get; set; }

        ICombatMoveBuilder CombatMoveBuilder { get; set; }
        IInjuryReportCalc InjuryReportCalc { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            DfAgentFactory = new DfAgentFactory(Store,
                new DfAgentBuilderFactory(),
                new DfColorFactory(),
                new DfMaterialFactory(Store, new DfMaterialBuilderFactory()),
                new DfCombatMoveFactory(),
                new DfBodyAttackFactory()
                );

            ContentMapper = new ContentMapper();

            EntityManager = new EntityManager();
            DfTagsFascade = new DfTagsFascade(Store, EntityManager, Random);

            Atlas = new Mock<IAtlas>().Object;
            CombatMoveBuilder = new CombatMoveBuilder();
            InjuryReportCalc = new InjuryReportCalc(new LayeredMaterialStrikeResultBuilder(new MaterialStrikeResultBuilder()));
        }


        [TestMethod]
        public void DwarfHead_InternalParts()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var head = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.IsNotNull(head);

            var skull = dwarf.Body.Parts.First(x => x.Name.Equals("skull"));
            Assert.IsNotNull(skull);

            Assert.AreSame(skull.Parent, head);

            var brain = dwarf.Body.Parts.First(x => x.Name.Equals("brain"));
            Assert.IsNotNull(brain);

            var internalParts = dwarf.Body.GetInternalParts(head);
            Assert.AreEqual(2, internalParts.Count());

            Assert.IsTrue(internalParts.SequenceEqual(new []{skull, brain}));
        }

        [TestMethod]
        public void HumanBodyAttack_Punch()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var punch = human.Body.Moves.Single(x => x.Name.Equals("punch"));
            Assert.AreEqual(StressMode.Blunt, punch.StressMode);

            var move = CombatMoveBuilder.BodyMove(human, human, punch, human.Body.Parts.First());
            var punchMom = human.GetStrikeMomentum(move);
            Assert.AreEqual(48d, (int)punchMom);
        }

        [TestMethod]
        public void HumanTotalBodyPartRelativeSize()
        {
            var expected = 6791;

            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            
            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(expected, totRel);
        }

        [TestMethod]
        public void UnicornTotalBodyPartRelativeSize()
        {
            var expected = 5546;

            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);

            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(expected, totRel);
        }

        [TestMethod]
        public void UnicornFrontLeg()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            Assert.AreEqual(900, part.Class.RelativeSize);
            Assert.AreEqual(392, (int)part.GetContactArea());

            Assert.AreEqual(5, part.Tissue.TissueLayers.Count());

            var skinLayer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(7, (int)skinLayer.Thickness);
            Assert.AreEqual(137, (int)skinLayer.Volume);

        }


        /*
         * Human data
         * Volume/Contact/Thickness/Material/Blunt_Momentum_Resist/Shear_Yield/Frac/PSize/PThick/PRelSize/BodyTotRelSize
upper body
        SKIN    20      112     3       skin    0       20000   20000   1197    227     1000    5546
        FAT     213     112     40      fat     4       10000   10000   1197    227     1000    5546
        MUSCLE  1069    112     202     muscle  21      20000   20000   1197    227     1000    5546
lower body
        SKIN    20      112     3       skin    0       20000   20000   1197    227     1000    5546
        FAT     213     112     40      fat     4       10000   10000   1197    227     1000    5546
        MUSCLE  1069    112     202     muscle  21      20000   20000   1197    227     1000    5546
neck
        SKIN    2       24      1       skin    0       20000   20000   119     105     100     5546
        FAT     21      24      18      fat     0       10000   10000   119     105     100     5546
        MUSCLE  106     24      93      muscle  2       20000   20000   119     105     100     5546
head
        EYEBROW 11      50      4       hair    0       60000   120000  359     152     300     5546
        EYEBROW 11      50      4       hair    0       60000   120000  359     152     300     5546
        HAIR    5       50      2       hair    0       60000   120000  359     152     300     5546
        HAIR    5       50      2       hair    0       60000   120000  359     152     300     5546
        SKIN    5       50      2       skin    0       20000   20000   359     152     300     5546
        FAT     57      50      24      fat     1       10000   10000   359     152     300     5546
        MUSCLE  289     50      122     muscle  5       20000   20000   359     152     300     5546
right upper arm
        SKIN    4       38      2       skin    0       20000   20000   239     133     200     5546
        FAT     42      38      23      fat     0       10000   10000   239     133     200     5546
        MUSCLE  106     38      59      muscle  2       20000   20000   239     133     200     5546
        BONE    106     38      59      bone    42      115000  130000  239     133     200     5546
left upper arm
        SKIN    4       38      2       skin    0       20000   20000   239     133     200     5546
        FAT     42      38      23      fat     0       10000   10000   239     133     200     5546
        MUSCLE  106     38      59      muscle  2       20000   20000   239     133     200     5546
        BONE    106     38      59      bone    42      115000  130000  239     133     200     5546
right lower arm
        SKIN    4       38      2       skin    0       20000   20000   239     133     200     5546
        FAT     42      38      23      fat     0       10000   10000   239     133     200     5546
        MUSCLE  106     38      59      muscle  2       20000   20000   239     133     200     5546
        BONE    106     38      59      bone    42      115000  130000  239     133     200     5546
left lower arm
        SKIN    4       38      2       skin    0       20000   20000   239     133     200     5546
        FAT     42      38      23      fat     0       10000   10000   239     133     200     5546
        MUSCLE  106     38      59      muscle  2       20000   20000   239     133     200     5546
        BONE    106     38      59      bone    42      115000  130000  239     133     200     5546
right hand
        SKIN    1       20      1       skin    0       20000   20000   95      97      80      5546
        FAT     16      20      17      fat     0       10000   10000   95      97      80      5546
        MUSCLE  42      20      43      muscle  0       20000   20000   95      97      80      5546
        BONE    42      20      43      bone    16      115000  130000  95      97      80      5546
left hand
        SKIN    1       20      1       skin    0       20000   20000   95      97      80      5546
        FAT     16      20      17      fat     0       10000   10000   95      97      80      5546
        MUSCLE  42      20      43      muscle  0       20000   20000   95      97      80      5546
        BONE    42      20      43      bone    16      115000  130000  95      97      80      5546
right upper leg
        SKIN    10      70      3       skin    0       20000   20000   598     180     500     5546
        FAT     106     70      32      fat     2       10000   10000   598     180     500     5546
        MUSCLE  267     70      80      muscle  5       20000   20000   598     180     500     5546
        BONE    267     70      80      bone    106     115000  130000  598     180     500     5546
left upper leg
        SKIN    10      70      3       skin    0       20000   20000   598     180     500     5546
        FAT     106     70      32      fat     2       10000   10000   598     180     500     5546
        MUSCLE  267     70      80      muscle  5       20000   20000   598     180     500     5546
        BONE    267     70      80      bone    106     115000  130000  598     180     500     5546
right lower leg
        SKIN    8       60      2       skin    0       20000   20000   478     167     400     5546
        FAT     85      60      29      fat     1       10000   10000   478     167     400     5546
        MUSCLE  213     60      74      muscle  4       20000   20000   478     167     400     5546
        BONE    213     60      74      bone    85      115000  130000  478     167     400     5546
left lower leg
        SKIN    8       60      2       skin    0       20000   20000   478     167     400     5546
        FAT     85      60      29      fat     1       10000   10000   478     167     400     5546
        MUSCLE  213     60      74      muscle  4       20000   20000   478     167     400     5546
        BONE    213     60      74      bone    85      115000  130000  478     167     400     5546
right foot
        SKIN    2       27      1       skin    0       20000   20000   143     112     120     5546
        FAT     25      27      19      fat     0       10000   10000   143     112     120     5546
        MUSCLE  63      27      50      muscle  1       20000   20000   143     112     120     5546
        BONE    63      27      50      bone    25      115000  130000  143     112     120     5546
left foot
        SKIN    2       27      1       skin    0       20000   20000   143     112     120     5546
        FAT     25      27      19      fat     0       10000   10000   143     112     120     5546
        MUSCLE  63      27      50      muscle  1       20000   20000   143     112     120     5546
        BONE    63      27      50      bone    25      115000  130000  143     112     120     5546
skull
        BONE    239     38      133     bone    95      115000  130000  239     133     200     5546
         * */

        [TestMethod]
        public void Human()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            IBodyPart part = null;
            ITissueLayer layer = null;

            //            BODY PART DEFENSE
            //Volume/Contact/Thickness/Material/Blunt_Momentum_Resist/Shear_Yield/Frac
            //upper body

            part = agent.Body.Parts.Single(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(part);
            var layers = part.Tissue.TissueLayers;
            Assert.AreEqual(3, layers.Count());

            //        SKIN    24      128     4       skin    0       20000   20000

            layer = layers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(4, layer.Thickness);
            Assert.AreEqual(24, layer.Volume);

            //        FAT     261     128     43      fat     5       10000   10000
            layer = layers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(43, layer.Thickness);
            
            //        MUSCLE  1308    128     217     muscle  26      20000   20000



            //lower body
            //        SKIN    24      128     4       skin    0       20000   20000
            //        FAT     261     128     43      fat     5       10000   10000
            //        MUSCLE  1308    128     217     muscle  26      20000   20000
            //neck
            //        SKIN    2       27      1       skin    0       20000   20000
            //        FAT     26      27      19      fat     0       10000   10000
            //        MUSCLE  130     27      100     muscle  2       20000   20000
            //head
            //        EYEBROW 14      57      5       hair    0       60000   120000
            //        EYEBROW 14      57      5       hair    0       60000   120000
            //        HAIR    7       57      2       hair    0       60000   120000
            //        HAIR    7       57      2       hair    0       60000   120000
            //        SKIN    7       57      2       skin    0       20000   20000
            //        FAT     70      57      25      fat     1       10000   10000
            //        MUSCLE  354     57      130     muscle  7       20000   20000
            //right upper arm
            
            part = agent.Body.Parts.Single(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(part);
            layers = part.Tissue.TissueLayers;
            Assert.AreEqual(4, layers.Count());

            //        SKIN    4       43      2       skin    0       20000   20000

            layer = layers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(2, layer.Thickness);
            //        FAT     52      43      25      fat     1       10000   10000
            //        MUSCLE  130     43      63      muscle  2       20000   20000
            //        BONE    130     43      63      bone    52      115000  130000
            layer = layers.Single(x => x.Material.Name.Equals("bone"));
            Assert.AreEqual(51, layer.Thickness);

        }
    }
}
