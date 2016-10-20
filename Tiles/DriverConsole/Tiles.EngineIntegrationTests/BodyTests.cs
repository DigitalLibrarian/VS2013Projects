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
                new DfTissueTemplateFactory(Store),
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
        public void DwarfRightUpperArm()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(bp);

            var ca = bp.GetContactArea();
            Assert.AreEqual(35d, ca);
        }

        [TestMethod]
        public void DwarfHeadMuscleVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var muscle = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            Assert.AreEqual(338, (int)muscle.Volume);
        }


        [TestMethod]
        public void DwarfHeadMuscleThickness()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var muscle = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            Assert.AreEqual(153, (int)muscle.Thickness);
        }

        [TestMethod]
        public void DwarfHeadSkinVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(5, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfUpperBodySkinVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("upper body"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(19, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfUpperBodySkinThickness()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("upper body"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(3, (int)layer.Thickness);
        }


        [TestMethod]
        public void DwarfUpperBodyContactArea()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.AreEqual(104, (int)bp.GetContactArea());
        }


        [TestMethod]
        public void DwarfUpperBodySize()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("upper body"));
            Assert.AreEqual(1081, (int)bp.Size);
        }

        [TestMethod]
        public void DwarfUpperBodyMuscleVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("upper body"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            Assert.AreEqual(1207, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfLeftUpperLegMuscleVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("left upper leg"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            Assert.AreEqual(301, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfLeftUpperLegFatVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("left upper leg"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(96, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfFourthToeNailRelativeThickness()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("fourth toe"));
            Assert.IsNotNull(bp);

            var ca = bp.GetContactArea();
            Assert.AreEqual(2d, ca);

            var nailLayer = bp.Tissue.TissueLayers.First(x => x.Material.Name.Equals("nail"));
            Assert.AreEqual(2, nailLayer.Class.RelativeThickness);
        }


        [TestMethod]
        public void DwarfFourthToeNailVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("fourth toe"));
            Assert.IsNotNull(bp);

            var ca = bp.GetContactArea();
            Assert.AreEqual(2d, ca);

            var nailLayer = bp.Tissue.TissueLayers.First(x => x.Material.Name.Equals("nail"));
            Assert.AreEqual(1d, nailLayer.Volume);
        }
        
        [TestMethod]
        public void DwarfFourthToeNailThickness()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("fourth toe"));
            Assert.IsNotNull(bp);

            var ca = bp.GetContactArea();
            Assert.AreEqual(2d, ca);

            var nailLayer = bp.Tissue.TissueLayers.First(x => x.Material.Name.Equals("nail"));
            Assert.AreEqual(1d, (int)nailLayer.Thickness);
        }

        [TestMethod]
        public void DwarfBite_Constraints()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("bite"));

            var reqs = moveClass.Requirements;
            Assert.AreEqual(1, reqs.Count());
            var req = reqs.First();
            Assert.AreEqual(BodyPartRequirementType.ChildBodyPartGroup, req.Type);

            Assert.AreEqual(2, req.Constraints.Count());
            var con = req.Constraints.ElementAt(0);
            Assert.AreEqual(BprConstraintType.ByCategory, con.ConstraintType);
            Assert.IsTrue(new string[] { "HEAD" }.SequenceEqual(con.Tokens));

            con = req.Constraints.ElementAt(1);
            Assert.AreEqual(BprConstraintType.ByCategory, con.ConstraintType);
            Assert.IsTrue(new string[] { "TOOTH" }.SequenceEqual(con.Tokens));
        }

        [TestMethod]
        public void DwarfBite_ContactArea()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("bite"));
            Assert.AreEqual(3, moveClass.ContactArea);
        }

        [TestMethod]
        public void DwarfBite_StrikeMaterial()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("bite"));
            var targetBodyPart = dwarf.Body.Parts.Single(x => x.Name.Equals("left foot"));
            var move = CombatMoveBuilder.BodyMove(dwarf, dwarf, moveClass, targetBodyPart);
            var mat = dwarf.GetStrikeMaterial(move);
            Assert.AreEqual("tooth", mat.Name);
        }

        [TestMethod]
        public void DwarfBite_StrikeMomentum()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("bite"));
            var targetBodyPart = dwarf.Body.Parts.Single(x => x.Name.Equals("left foot"));
            var move = CombatMoveBuilder.BodyMove(dwarf, dwarf, moveClass, targetBodyPart);
            var mom = dwarf.GetStrikeMomentum(move);
            Assert.AreEqual(4,(int) mom);
        }

        [TestMethod]
        public void DwarfBite_MaxPenetration()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("bite"));
            Assert.AreEqual(7, moveClass.MaxPenetration);
        }

        [TestMethod]
        public void DwarfScratch_ContactArea()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("scratch"));
            Assert.AreEqual(7, moveClass.ContactArea);
        }


        [TestMethod]
        public void DwarfScratch_StrikeMomentum()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var targetBodyPart = dwarf.Body.Parts.Single(x => x.Name.Equals("left foot"));

            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("scratch"));
            var move = CombatMoveBuilder.BodyMove(dwarf, dwarf, moveClass, targetBodyPart);
            var mom = dwarf.GetStrikeMomentum(move);
            Assert.AreEqual(17, (int) mom);
        }

        [TestMethod]
        public void DwarfScratch_Penetration()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var moveClass = dwarf.Body.Moves.Single(x => x.Name.Equals("scratch"));
            Assert.AreEqual(26, moveClass.MaxPenetration);
        }
        [TestMethod]
        public void DwarfTotalBodyPartRelativeSize()
        {
            var expected = 5546;
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(expected, totRel);
        }

        [TestMethod]
        public void GiantTotalBodyPartRelativeSize()
        {
            var expected = 5546;
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT", "MALE", Vector3.Zero);

            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(expected, totRel);
        }

        [TestMethod]
        public void GiantTortoiseBodyPartRelativeSize()
        {
            var expected = 7660;
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT TORTOISE", "MALE", Vector3.Zero);

            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(expected, totRel);

        }

        [TestMethod]
        public void UnicornTotalBodyPartRelativeSize()
        {
            var expected = 6791;
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);

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
            Assert.AreEqual(74, (int)layer.Thickness);
        }

        [TestMethod]
        public void UnicornFrontLeg_FatVolume()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("right front leg"));

            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(1395, (int)layer.Volume);
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
            Assert.AreEqual(43, (int)layer.Thickness);
        }

        [TestMethod]
        public void HumanUpperBody_FatTissueProperties()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var part = agent.Body.Parts.Single(x => x.Name.Equals("upper body"));

            var layer = part.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(3, layer.Class.VascularRating);
            Assert.IsTrue(layer.Class.IsConnective);
            Assert.IsFalse(layer.Class.IsCosmetic);
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
        public void HumanBodyAttack_Punch_Momentum()
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
            Assert.AreEqual(51, (int)punchMom);
        }

        [TestMethod]
        public void HumanBodyAttack_Bite_Momentum()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var bite = human.Body.Moves.Single(x => x.Name.Equals("bite"));
            Assert.AreEqual(StressMode.Edge, bite.StressMode);

            var move = CombatMoveBuilder.BodyMove(human, human, bite, human.Body.Parts.First());

            var mom = human.GetStrikeMomentum(move);
            Assert.AreEqual(4, (int)mom);
        }


        [TestMethod]
        public void HumanBodyAttack_Scratch_Momentum()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var scratch = human.Body.Moves.Single(x => x.Name.Equals("scratch"));
            Assert.AreEqual(StressMode.Edge, scratch.StressMode);

            var move = CombatMoveBuilder.BodyMove(human, human, scratch, human.Body.Parts.First());
            var mom = human.GetStrikeMomentum(move);
            Assert.AreEqual(16, (int)mom);
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
        public void HumanBodyAttack_Kick_Momentum()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var kick = human.Body.Moves.Single(x => x.Name.Equals("kick"));
            Assert.AreEqual(StressMode.Blunt, kick.StressMode);

            var parts = kick.GetRelatedBodyParts(human.Body);
            Assert.AreEqual(1, parts.Count());
            Assert.AreEqual("right foot", parts.First().Name);

            var move = CombatMoveBuilder.BodyMove(human, human, kick, human.Body.Parts.First());
            var mom = human.GetStrikeMomentum(move);
            Assert.AreEqual(76, (int)mom);
        }

        [TestMethod]
        public void HumanTotalBodyPartRelativeSize()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var totRel = agent.Class.BodyClass.TotalBodyPartRelSize;
            Assert.AreEqual(5546, totRel);
        }


        [TestMethod]
        public void ParakeetBodyAttack_Scratch_ContactArea()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "BIRD_PARAKEET", "MALE", Vector3.Zero);
            var moveClass = agent.Body.Moves.Single(x => x.Name.Equals("snatch at"));
            Assert.AreEqual(StressMode.Edge, moveClass.StressMode);
            Assert.AreEqual(1, moveClass.ContactArea);
        }


        [TestMethod]
        public void ParakeetBodyAttack_Scratch_MaxPenetration()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "BIRD_PARAKEET", "MALE", Vector3.Zero);
            var moveClass = agent.Body.Moves.Single(x => x.Name.Equals("snatch at"));
            Assert.AreEqual(StressMode.Edge, moveClass.StressMode);
            Assert.AreEqual(1, moveClass.MaxPenetration);
        }


        [TestMethod]
        public void Unicorn_SkinTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(2, (int)cost);
        }
        [TestMethod]
        public void Unicorn_FatTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(27d, (int)cost);
        }

        [TestMethod]
        public void Unicorn_MuscleTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(69, (int)cost);
        }

        [TestMethod]
        public void Unicorn_BoneTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(1395d, (int)cost);
        }


        IAgent GetNewDwarf()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
        }

        IAgent GetNewUnicorn()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
        }
    }
}
