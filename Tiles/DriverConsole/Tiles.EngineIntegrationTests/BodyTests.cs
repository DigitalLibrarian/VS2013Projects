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
        public void KoboldSizes()
        {
            var kobold = DfTagsFascade.CreateCreatureAgent(Atlas, "KOBOLD", "MALE", Vector3.Zero);

            Assert.AreEqual(20000, kobold.Body.Size);
            Assert.AreEqual(2252, kobold.Body.Parts.Single(p => p.Name.Equals("upper body")).Size);
            Assert.AreEqual(2252, kobold.Body.Parts.Single(p => p.Name.Equals("lower body")).Size);
            Assert.AreEqual(226, kobold.Body.Parts.Single(p => p.Name.Equals("neck")).Size);
            Assert.AreEqual(676, kobold.Body.Parts.Single(p => p.Name.Equals("head")).Size);
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
        public void HumanBodyAttacks()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);

            var moves = human.Body.Moves;
            Assert.AreEqual(4, moves.Count());

            var punch = moves.Single(x => x.Name.Equals("punch"));
            Assert.AreEqual(StressMode.Blunt, punch.StressMode);
            Assert.AreEqual(339, punch.ContactArea);
        }
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
            Assert.AreEqual(3, layer.Thickness);
            //        FAT     261     128     43      fat     5       10000   10000
            layer = layers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(17, layer.Thickness);
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

            //left upper arm
            //        SKIN    4       43      2       skin    0       20000   20000
            //        FAT     52      43      25      fat     1       10000   10000
            //        MUSCLE  130     43      63      muscle  2       20000   20000
            //        BONE    130     43      63      bone    52      115000  130000
            //right lower arm
            //        SKIN    4       43      2       skin    0       20000   20000
            //        FAT     52      43      25      fat     1       10000   10000
            //        MUSCLE  130     43      63      muscle  2       20000   20000
            //        BONE    130     43      63      bone    52      115000  130000
            //left lower arm
            //        SKIN    4       43      2       skin    0       20000   20000
            //        FAT     52      43      25      fat     1       10000   10000
            //        MUSCLE  130     43      63      muscle  2       20000   20000
            //        BONE    130     43      63      bone    52      115000  130000
            //right hand
            //        SKIN    1       23      1       skin    0       20000   20000
            //        FAT     20      23      18      fat     0       10000   10000
            //        MUSCLE  52      23      46      muscle  1       20000   20000
            //        BONE    52      23      46      bone    20      115000  130000
            //left hand
            //        SKIN    1       23      1       skin    0       20000   20000
            //        FAT     20      23      18      fat     0       10000   10000
            //        MUSCLE  52      23      46      muscle  1       20000   20000
            //        BONE    52      23      46      bone    20      115000  130000
            //right upper leg
            //        SKIN    12      80      3       skin    0       20000   20000
            //        FAT     130     80      34      fat     2       10000   10000
            //        MUSCLE  327     80      86      muscle  6       20000   20000
            //        BONE    327     80      86      bone    130     115000  130000
            //left upper leg
            //        SKIN    12      80      3       skin    0       20000   20000
            //        FAT     130     80      34      fat     2       10000   10000
            //        MUSCLE  327     80      86      muscle  6       20000   20000
            //        BONE    327     80      86      bone    130     115000  130000
            //right lower leg
            //        SKIN    9       69      3       skin    0       20000   20000
            //        FAT     104     69      31      fat     2       10000   10000
            //        MUSCLE  261     69      79      muscle  5       20000   20000
            //        BONE    261     69      79      bone    104     115000  130000
            //left lower leg
            //        SKIN    9       69      3       skin    0       20000   20000
            //        FAT     104     69      31      fat     2       10000   10000
            //        MUSCLE  261     69      79      muscle  5       20000   20000
            //        BONE    261     69      79      bone    104     115000  130000
            //right foot
            //        SKIN    2       31      2       skin    0       20000   20000
            //        FAT     31      31      21      fat     0       10000   10000
            //        MUSCLE  78      31      53      muscle  1       20000   20000
            //        BONE    78      31      53      bone    31      115000  130000
            //left foot
            //        SKIN    2       31      2       skin    0       20000   20000
            //        FAT     31      31      21      fat     0       10000   10000
            //        MUSCLE  78      31      53      muscle  1       20000   20000
            //        BONE    78      31      53      bone    31      115000  130000
            //skull
            //        BONE    292     43      142     bone    116     115000  130000
        }
    }
}
