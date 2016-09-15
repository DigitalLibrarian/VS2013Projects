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
        public void UnicornTotalBodyPartRelativeSize()
        {
            var expected = 5546;

            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);

            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(expected, totRel);
        }

        [TestMethod]
        public void UnicornFrontLeg_SkinThickness()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));


            Assert.AreEqual(5, part.Tissue.TissueLayers.Count());

            var skinLayer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(7, (int)skinLayer.Thickness);
        }
            
        [TestMethod]
        public void UnicornFrontLeg_SkinVolume()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            var skinLayer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(139, (int)skinLayer.Volume);
        }
        
        [TestMethod]
        public void UnicornFrontLeg_FatThickness()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            Assert.AreEqual(5, part.Tissue.TissueLayers.Count());

            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(71, (int)layer.Thickness);
        }

        [TestMethod]
        public void UnicornFrontLeg_FatVolume()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(1241, (int)layer.Volume);
        }

        [TestMethod]
        public void UnicornFrontLeg_BoneVolume()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));
            Assert.AreEqual(3487, (int)layer.Volume);
        }

        [TestMethod]
        public void Unicorn_FrontLeg_RelativeSize()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));
            Assert.AreEqual(900, part.Class.RelativeSize);
        }

        [TestMethod]
        public void Unicorn_FrontLeg_ContactArea()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            Assert.AreEqual(396, (int)part.GetContactArea());
        }

        [TestMethod]
        public void Human()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            Assert.AreEqual(7000d, agent.Body.Size);

            var part = agent.Body.Parts.Single(x => x.Name.Equals("upper body"));
            Assert.IsNotNull(part);
            var layers = part.Tissue.TissueLayers;
            Assert.AreEqual(3, layers.Count());

            part = agent.Body.Parts.Single(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(part);
            layers = part.Tissue.TissueLayers;
            Assert.AreEqual(4, layers.Count());

        }

        [TestMethod]
        public void HumanUpperBody_FatThickness()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("upper body"));
                        
            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(43, layer.Thickness);
        }


        [TestMethod]
        public void HumanUpperBody_FatVolume()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("upper body"));
            
            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(261, (int)layer.Volume);
        }


        [TestMethod]
        public void HumanBodyAttack_HandSize()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);

            var part = human.Body.Parts.Single(p => p.Name.Equals("right hand"));
            var handSize = part.Size;
            Assert.AreEqual(100, (int)handSize);
        }

        [TestMethod]
        public void HumanBodyAttack_HandMass()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);

            var part = human.Body.Parts.Single(p => p.Name.Equals("right hand"));

            var handMass = part.GetMass();
            Assert.AreEqual(504d, (int)handMass);
        }

        [TestMethod]
        public void HumanBodyAttack_Punch()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var punch = human.Body.Moves.Single(x => x.Name.Equals("punch"));
            Assert.AreEqual(StressMode.Blunt, punch.StressMode);

            var parts = punch.GetRelatedBodyParts(human.Body);
            Assert.AreEqual(1, parts.Count());
            var part = parts.First();
            Assert.AreEqual("right hand", part.Name);

            var move = CombatMoveBuilder.BodyMove(human, human, punch, human.Body.Parts.First());
            var punchMom = human.GetStrikeMomentum(move);
            Assert.AreEqual(36d, (int)punchMom);
        }

        [TestMethod]
        public void HumanBodyAttack_Bite()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var bite = human.Body.Moves.Single(x => x.Name.Equals("bite"));
            Assert.AreEqual(StressMode.Edge, bite.StressMode);

            var move = CombatMoveBuilder.BodyMove(human, human, bite, human.Body.Parts.First());

            var mom = human.GetStrikeMomentum(move);
            Assert.AreEqual(4d, (int)mom);
        }


        [TestMethod]
        public void HumanBodyAttack_Scratch()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var scratch = human.Body.Moves.Single(x => x.Name.Equals("scratch"));
            Assert.AreEqual(StressMode.Edge, scratch.StressMode);

            var move = CombatMoveBuilder.BodyMove(human, human, scratch, human.Body.Parts.First());
            var mom = human.GetStrikeMomentum(move);
            Assert.AreEqual(15d, (int)mom);
        }

        [TestMethod]
        public void HumanBodyAttack_FootMass()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var foot = human.Body.Parts.Single(p => p.Name.Equals("right foot"));

            Assert.AreEqual(757d, (int)foot.GetMass());
        }

        [TestMethod]
        public void HumanBodyAttack_FootSize()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var foot = human.Body.Parts.Single(p => p.Name.Equals("right foot"));

            Assert.AreEqual(151d, (int)foot.Size);
        }
        [TestMethod]
        public void HumanBodyAttack_Kick()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var kick = human.Body.Moves.Single(x => x.Name.Equals("kick"));
            Assert.AreEqual(StressMode.Blunt, kick.StressMode);

            var parts = kick.GetRelatedBodyParts(human.Body);
            Assert.AreEqual(1, parts.Count());
            Assert.AreEqual("right foot", parts.First().Name);

            var move = CombatMoveBuilder.BodyMove(human, human, kick, human.Body.Parts.First());
            var mom = human.GetStrikeMomentum(move);
            Assert.AreEqual(45d, (int)mom);
        }

        [TestMethod]
        public void HumanTotalBodyPartRelativeSize()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);

            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(6791, totRel);
        }
    }
}
