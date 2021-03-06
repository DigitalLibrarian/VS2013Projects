﻿using System;
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
using Tiles.Bodies.Injuries;
using Tiles.Materials;
using Tiles.Bodies;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class AgentTests
    {
        IRandom Random { get; set; }

        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IContentMapper ContentMapper { get; set; }

        IEntityManager EntityManager { get; set; }
        DfTagsFascade DfTagsFascade { get; set; }

        IAtlas Atlas { get; set; }

        ICombatMoveFactory CombatMoveFactory { get; set; }
        IInjuryReportCalc InjuryReportCalc { get; set; }

        IMaterialStressCalc MaterialStressCalc { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            var colorFactory = new DfColorFactory();
            DfAgentFactory = new DfAgentFactory(Store,
                new DfAgentBuilderFactory(),
                colorFactory,
                new DfMaterialFactory(Store, new DfMaterialBuilderFactory(), colorFactory),
                new DfTissueTemplateFactory(Store),
                new DfCombatMoveFactory(),
                new DfBodyAttackFactory()
                );

            ContentMapper = new ContentMapper();

            EntityManager = new EntityManager();
            DfTagsFascade = new DfTagsFascade(Store, EntityManager, Random);

            Atlas = new Mock<IAtlas>().Object;
            CombatMoveFactory = new CombatMoveFactory();
            InjuryReportCalc = new InjuryReportCalc(Random, new InjuryFactory(Random));

            MaterialStressCalc = new MaterialStressCalc();
        }


        [TestMethod]
        public void Dwarf_PainThreshold()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            Assert.AreEqual(100, dwarf.GetPainThreshold());
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

            Assert.AreEqual("CARMINE", dwarf.Body.Class.BloodMaterial.GetStateProperty("COLOR", "LIQUID"));
            Assert.AreEqual("frozen blood", dwarf.Body.Class.BloodMaterial.GetStateProperty("NAME", "SOLID"));
            Assert.AreEqual("blood", dwarf.Body.Class.BloodMaterial.GetStateProperty("NAME", "LIQUID"));
            Assert.AreEqual("boiling blood", dwarf.Body.Class.BloodMaterial.GetStateProperty("NAME", "GAS"));
        }

        [TestMethod]
        public void Dwarf_Moves()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            Assert.AreEqual(2, dwarf.Body.Moves.Count(x => x.Name.Equals("punch")));
            Assert.AreEqual(2, dwarf.Body.Moves.Count(x => x.Name.Equals("kick")));
            Assert.AreEqual(2, dwarf.Body.Moves.Count(x => x.Name.Equals("scratch")));
            Assert.AreEqual(1, dwarf.Body.Moves.Count(x => x.Name.Equals("bite")));
            Assert.AreEqual(7, dwarf.Body.Moves.Count());
        }

        [TestMethod]
        public void Dwarf_FeetCharacteristic()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            Assert.IsNotNull(agent);

            var leftFoot = agent.Body.Parts.Single(p => p.IsStance && p.IsLeft);
            var rightFoot = agent.Body.Parts.Single(p => p.IsStance && p.IsRight);
            Assert.AreEqual("left foot", leftFoot.Name);
            Assert.AreEqual("right foot", rightFoot.Name);
        }

        [TestMethod]
        public void Dwarf_PunchTimes()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var move = dwarf.Body.Moves.First(x => x.Name.Equals("punch"));

            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);
        }

        [TestMethod]
        public void Dwarf_PunchCannotLatch()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var move = dwarf.Body.Moves.First(x => x.Name.Equals("punch"));

            Assert.IsFalse(move.CanLatch);
        }

        [TestMethod]
        public void Dwarf_BiteCanLatch()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var move = dwarf.Body.Moves.First(x => x.Name.Equals("bite"));

            Assert.IsTrue(move.CanLatch);
        }

        [TestMethod]
        public void Dwarf_SkullAroundBrain()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var skull = dwarf.Body.Parts.First(x => x.Name.Equals("skull"));
            var brain = dwarf.Body.Parts.First(x => x.Name.Equals("brain"));

            Assert.AreEqual(1, skull.Class.BodyPartRelations.Count());
            var aroundSkull = dwarf.Body.GetRelations(skull, BodyPartRelationType.Around);
            var cleansSkull = dwarf.Body.GetRelations(skull, BodyPartRelationType.Cleans);

            Assert.AreEqual(1, aroundSkull.Count());
            Assert.AreSame(brain, aroundSkull.Single().Key);
            Assert.AreEqual(100, aroundSkull.Single().Value);

            Assert.AreEqual(0, cleansSkull.Count());

            Assert.AreEqual(0, dwarf.Body.GetRelations(brain, BodyPartRelationType.Around).Count());
            Assert.AreEqual(0, dwarf.Body.GetRelations(brain, BodyPartRelationType.Cleans).Count());
        }

        [TestMethod]
        public void Dwarf_AroundTrueRib()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var trueRibs = dwarf.Body.Parts.Where(p => p.Class.Categories.Contains("RIB_TRUE"));
            var heart = dwarf.Body.Parts.First(x => x.Name.Equals("heart"));
            var lungs = dwarf.Body.Parts.Where(x => x.Class.Categories.Contains("LUNG"));
            Assert.AreEqual(2, lungs.Count());
            Assert.AreEqual(2, trueRibs.Count());
            foreach (var rib in trueRibs)
            {
                var around = dwarf.Body.GetRelations(rib, BodyPartRelationType.Around);
                Assert.AreEqual(3, around.Count());
                Assert.IsTrue(around.ContainsKey(heart));
                Assert.AreEqual(5, around[heart]);

                foreach (var lung in lungs)
                {
                    Assert.IsTrue(around.ContainsKey(lung));
                    Assert.AreEqual(5, around[lung]);
                }

                var cleans = dwarf.Body.GetRelations(rib, BodyPartRelationType.Cleans);
                Assert.AreEqual(0, cleans.Count());
            }
        }

        [TestMethod]
        public void Dwarf_EyelidCleansEye()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var rEyeLid = dwarf.Body.Parts.Single(x => x.Class.TokenId.Equals("R_EYELID"));
            var rEye = dwarf.Body.Parts.Single(x => x.Class.TokenId.Equals("REYE"));
            var lEyeLid = dwarf.Body.Parts.Single(x => x.Class.TokenId.Equals("L_EYELID"));
            var lEye = dwarf.Body.Parts.Single(x => x.Class.TokenId.Equals("LEYE"));

            Assert.AreEqual(2, rEyeLid.Class.BodyPartRelations.Count());
            Assert.AreEqual(0, rEye.Class.BodyPartRelations.Count());
            Assert.AreEqual(2, lEyeLid.Class.BodyPartRelations.Count());
            Assert.AreEqual(0, lEye.Class.BodyPartRelations.Count());

            var relations = dwarf.Body.GetRelations(rEyeLid, BodyPartRelationType.Around);
            Assert.AreEqual(1, relations.Count());
            Assert.IsTrue(relations.ContainsKey(rEye));
            Assert.AreEqual(50, relations[rEye]);

            relations = dwarf.Body.GetRelations(lEyeLid, BodyPartRelationType.Around);
            Assert.AreEqual(1, relations.Count());
            Assert.IsTrue(relations.ContainsKey(lEye));
            Assert.AreEqual(50, relations[lEye]);

            relations = dwarf.Body.GetRelations(rEyeLid, BodyPartRelationType.Cleans);
            Assert.AreEqual(1, relations.Count());
            Assert.IsTrue(relations.ContainsKey(rEye));
            Assert.AreEqual(100, relations[rEye]);

            relations = dwarf.Body.GetRelations(lEyeLid, BodyPartRelationType.Cleans);
            Assert.AreEqual(1, relations.Count());
            Assert.IsTrue(relations.ContainsKey(lEye));
            Assert.AreEqual(100, relations[lEye]);
        }

        [TestMethod]
        public void Dwarf_LowerBodyInternalParts()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var lb = dwarf.Body.Parts.First(x => x.Name.Equals("lower body"));
            var lbInternal = dwarf.Body.GetInternalParts(lb);

            var guts = lbInternal.FirstOrDefault(x => x.Name.Equals("guts"));
            Assert.IsNotNull(guts);
            var spleen = lbInternal.FirstOrDefault(x => x.Name.Equals("spleen"));
            Assert.IsNotNull(spleen);
            var lowerSpine = lbInternal.FirstOrDefault(x => x.Name.Equals("lower spine"));
            Assert.IsNotNull(lowerSpine);
            var leftKidney = lbInternal.FirstOrDefault(x => x.Name.Equals("left kidney"));
            Assert.IsNotNull(leftKidney);
            var rightKidney = lbInternal.FirstOrDefault(x => x.Name.Equals("right kidney"));
            Assert.IsNotNull(rightKidney);
            var pancreas = lbInternal.FirstOrDefault(x => x.Name.Equals("pancreas"));
            Assert.IsNotNull(pancreas);
            var stomach = lbInternal.FirstOrDefault(x => x.Name.Equals("stomach"));
            Assert.IsNotNull(stomach);


            Assert.AreEqual(7, lbInternal.Count());
        }

        [TestMethod]
        public void DwarfRightUpperArm()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("right upper arm"));
            Assert.IsNotNull(bp);

            var ca = bp.ContactArea;
            Assert.AreEqual(35, (int)ca);
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
        public void DwarfHeadSkinHealingRate()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(100, layer.Class.HealingRate);
        }

        [TestMethod]
        public void DwarfHeadSkinPainReceptors()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(5, layer.Class.PainReceptors);
        }


        [TestMethod]
        public void DwarfHeadFatHasArteries()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.IsFalse(layer.Class.HasArteries);
        }

        [TestMethod]
        public void DwarfHeadMuscleHasArteries()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            Assert.IsTrue(layer.Class.HasArteries);
        }

        [TestMethod]
        public void DwarfHeadFatPainReceptors()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(5, layer.Class.PainReceptors);
        }
        [TestMethod]
        public void DwarfHeadContactArea()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("head"));
            Assert.AreEqual(47, (int)bp.ContactArea);
        }

        [TestMethod]
        public void DwarfSkull_ConnectionProperties()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("skull"));
            Assert.IsTrue(bp.IsConnector);
            Assert.IsTrue(bp.PreventsParentCollapse);
        }

        [TestMethod]
        public void DwarfSkull_BoneHealingRate()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("skull"));
            Assert.AreEqual(1000, bp.Tissue.TissueLayers.Single().Class.HealingRate);
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
            Assert.AreEqual(104, (int)bp.ContactArea);
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
        public void DwarfLowerBodySkinVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("lower body"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(19, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfLowerBodyFatVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("lower body"));
            var layer = bp.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.AreEqual(193, (int)layer.Volume);
        }

        [TestMethod]
        public void DwarfLowerBodyMuscleVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("lower body"));
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

            var ca = bp.ContactArea;
            Assert.AreEqual(2, (int)ca);

            var nailLayer = bp.Tissue.TissueLayers.First(x => x.Material.Name.Equals("nail"));
            Assert.AreEqual(2, nailLayer.Class.RelativeThickness);
        }


        [TestMethod]
        public void DwarfFourthToeNailVolume()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("fourth toe"));
            Assert.IsNotNull(bp);

            var ca = bp.ContactArea;
            Assert.AreEqual(2, (int)ca);

            var nailLayer = bp.Tissue.TissueLayers.First(x => x.Material.Name.Equals("nail"));
            Assert.AreEqual(1d, nailLayer.Volume);
        }
        
        [TestMethod]
        public void DwarfFourthToeNailThickness()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var bp = dwarf.Body.Parts.First(x => x.Name.Equals("fourth toe"));
            Assert.IsNotNull(bp);

            var ca = bp.ContactArea;
            Assert.AreEqual(2, (int)ca);

            var nailLayer = bp.Tissue.TissueLayers.First(x => x.Material.Name.Equals("nail"));
            Assert.AreEqual(1d, (int)nailLayer.Thickness);
        }

        [TestMethod]
        public void DwarfPunch_ContactArea()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("punch"));
            Assert.AreEqual(19, (int)moveClass.ContactArea);
        }

        [TestMethod]
        public void DwarfBite_Constraints()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("bite"));

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
            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("bite"));
            Assert.AreEqual(9, moveClass.ContactArea, 1d);
        }

        [TestMethod]
        public void DwarfBite_StrikeMaterial()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("bite"));
            var targetBodyPart = dwarf.Body.Parts.Single(x => x.Name.Equals("left foot"));
            var move = CombatMoveFactory.BodyMove(dwarf, dwarf, moveClass, targetBodyPart);
            var mat = move.GetStrikeMaterial();
            Assert.AreEqual("tooth", mat.Name);
        }

        [TestMethod]
        public void DwarfBite_StrikeMomentum()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("bite"));
            var targetBodyPart = dwarf.Body.Parts.Single(x => x.Name.Equals("left foot"));
            var move = CombatMoveFactory.BodyMove(dwarf, dwarf, moveClass, targetBodyPart);
            var mom = move.GetStrikeMomentum();
            Assert.AreEqual(19,(int) mom);
        }

        [TestMethod]
        public void DwarfBite_MaxPenetration()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("bite"));
            Assert.AreEqual(30.2d, moveClass.MaxPenetration, 0.1d);
        }

        [TestMethod]
        public void DwarfScratch_ContactArea()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("scratch"));
            Assert.AreEqual(8d, moveClass.ContactArea, 1d);
        }


        [TestMethod]
        public void DwarfScratch_StrikeMomentum()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var targetBodyPart = dwarf.Body.Parts.Single(x => x.Name.Equals("left foot"));

            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(dwarf, dwarf, moveClass, targetBodyPart);
            var mom = move.GetStrikeMomentum();
            Assert.AreEqual(17, (int) mom);
        }

        [TestMethod]
        public void DwarfScratch_Penetration()
        {
            var dwarf = DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);

            var moveClass = dwarf.Body.Moves.First(x => x.Name.Equals("scratch"));
            Assert.AreEqual(25d, moveClass.MaxPenetration, 1d);
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

            Assert.AreEqual(396, (int)part.ContactArea);
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
            Assert.AreEqual(41, (int)layer.Thickness);
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
            Assert.AreEqual(225, (int)layer.Volume);
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

            var handMass = part.Mass;
            Assert.AreEqual(504d, (int)handMass);
        }

        [TestMethod]
        public void HumanBodyAttack_Punch_Momentum()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var punch = human.Body.Moves.First(x => x.Name.Equals("punch"));
            Assert.AreEqual(StressMode.Impact, punch.StressMode);

            var parts = punch.GetRelatedBodyParts(human.Body);
            Assert.AreEqual(1, parts.Count());
            var part = parts.First();
            Assert.AreEqual("right hand", part.Name);

            var move = CombatMoveFactory.BodyMove(human, human, punch, human.Body.Parts.First());
            var punchMom = move.GetStrikeMomentum();
            Assert.AreEqual(51, (int)punchMom);
        }

        [TestMethod]
        public void HumanBodyAttack_Bite_Momentum()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var bite = human.Body.Moves.First(x => x.Name.Equals("bite"));
            Assert.AreEqual(StressMode.Shear, bite.StressMode);

            var move = CombatMoveFactory.BodyMove(human, human, bite, human.Body.Parts.First());

            var mom = move.GetStrikeMomentum();
            Assert.AreEqual(18, (int)mom);
        }

        [TestMethod]
        public void HumanBodyAttack_Scratch_Momentum()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var scratch = human.Body.Moves.First(x => x.Name.Equals("scratch"));
            Assert.AreEqual(StressMode.Shear, scratch.StressMode);

            var move = CombatMoveFactory.BodyMove(human, human, scratch, human.Body.Parts.First());
            var mom = move.GetStrikeMomentum();
            Assert.AreEqual(16, (int)mom);
        }

        [TestMethod]
        public void HumanBodyAttack_FootMass()
        {
            var human = DfTagsFascade.CreateCreatureAgent(Atlas, "HUMAN", "MALE", Vector3.Zero);
            var foot = human.Body.Parts.Single(p => p.Name.Equals("right foot"));

            Assert.AreEqual(757d, (int)foot.Mass);
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
            var kick = human.Body.Moves.First(x => x.Name.Equals("kick"));
            Assert.AreEqual(StressMode.Impact, kick.StressMode);

            var parts = kick.GetRelatedBodyParts(human.Body);
            Assert.AreEqual(1, parts.Count());
            Assert.AreEqual("right foot", parts.First().Name);

            var move = CombatMoveFactory.BodyMove(human, human, kick, human.Body.Parts.First());
            var mom = move.GetStrikeMomentum();
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
            var moveClass = agent.Body.Moves.First(x => x.Name.Equals("snatch at"));
            Assert.AreEqual(StressMode.Shear, moveClass.StressMode);
            Assert.AreEqual(1, moveClass.ContactArea);
        }

        [TestMethod]
        public void ParakeetBodyAttack_Scratch_MaxPenetration()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "BIRD_PARAKEET", "MALE", Vector3.Zero);
            var moveClass = agent.Body.Moves.First(x => x.Name.Equals("snatch at"));
            Assert.AreEqual(StressMode.Shear, moveClass.StressMode);
            Assert.AreEqual(1, moveClass.MaxPenetration);
        }

        [TestMethod]
        public void Unicorn_HornStabContactArea()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            var moveClass = agent.Body.Moves.First(x => x.Name.Equals("stab"));

            Assert.AreEqual(StressMode.Shear, moveClass.StressMode);
            Assert.AreEqual(4, (int)moveClass.ContactArea);
        }

        [TestMethod]
        public void Unicorn_Moves()
        {
            var agent = DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
            Assert.AreEqual(4, agent.Body.Moves.Count(x => x.Name.Equals("kick")));
            Assert.AreEqual(1, agent.Body.Moves.Count(x => x.Name.Equals("stab")));
            Assert.AreEqual(1, agent.Body.Moves.Count(x => x.Name.Equals("bite")));
            Assert.AreEqual(6, agent.Body.Moves.Count());
        }

        [TestMethod]
        public void Unicorn_SkinTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));

            var cost = MaterialStressCalc.BluntCost1(layer.Material.ImpactYield, layer.Volume);
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

            var cost = MaterialStressCalc.BluntCost1(layer.Material.ImpactYield, layer.Volume);
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

            var cost = MaterialStressCalc.BluntCost1(layer.Material.ImpactYield, layer.Volume);
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

            var cost = MaterialStressCalc.BluntCost1(layer.Material.ImpactYield, layer.Volume);
            Assert.AreEqual(1395d, (int)cost);
        }

        [TestMethod]
        public void GiantCaveSpider_Moves()
        {
            var attacker = GetNewGiantCaveSpider();
            var defender = GetNewDwarf();

            var move = attacker.Body.Moves.Single();
            Assert.AreEqual("bite", move.Name);
            Assert.AreEqual(18d, move.ContactArea, 1d);
            Assert.AreEqual(71, move.MaxPenetration, 1d);
        }

        [TestMethod]
        public void BandedKnifeFish_BiteStrikeMaterial()
        {
            var attacker = GetNewBandedKnifeFish();

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("bite"));
            Assert.IsNotNull(moveClass);

            var move = CombatMoveFactory.BodyMove(attacker, attacker, moveClass, attacker.Body.Parts.First());
            var mat = move.GetStrikeMaterial();
            Assert.IsNotNull(mat);
        }

        [TestMethod]
        public void BlueShark_BiteStrikeMaterial()
        {
            var attacker = GetNewBlueShark();

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("bite"));
            Assert.IsNotNull(moveClass);

            var move = CombatMoveFactory.BodyMove(attacker, attacker, moveClass, attacker.Body.Parts.First());
            var mat = move.GetStrikeMaterial();
            Assert.IsNotNull(mat);
        }

        [TestMethod]
        public void GiantGrizzlyBear_ToeSize()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);

            var part = attacker.Body.Parts.Single(p => p.Name.Equals("first right front toe"));
            Assert.AreEqual(24.95d, part.ContactArea, 0.01d);
            Assert.AreEqual(125.27d, part.Size, 0.01d);
            Assert.AreEqual(107d, part.Thickness);
        }

        [TestMethod]
        public void CrabVsCrab_SnatchMaterial()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "CRAB", "MALE", Vector3.Zero);

            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("snatch"));
            var move = CombatMoveFactory.BodyMove(attacker, attacker, moveClass, attacker.Body.Parts.First());
            var strikeMat = move.GetStrikeMaterial();
            Assert.AreEqual("chitin", strikeMat.Name);
        }

        [TestMethod]
        public void GiantGrizzlyBear_Size()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);

            Assert.AreEqual(170000d, attacker.Class.BodyClass.Size);
        }

        [TestMethod]
        public void GiantGrizzlyBear_ScratchMaterial()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);

            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, attacker, moveClass, attacker.Body.Parts.First());
            var mat = move.GetStrikeMaterial();

            Assert.AreEqual("nail", mat.Name);
        }

        [TestMethod]
        public void GiantGrizzlyBear_ScratchMomentum()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);

            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));
            var move = CombatMoveFactory.BodyMove(attacker, attacker, moveClass, attacker.Body.Parts.First());

            var mom = move.GetStrikeMomentum();
            Assert.AreEqual(201.44d, mom, 0.01d);
        }

        [TestMethod]
        public void GiantGrizzlyBear_ScratchContactArea()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));

            Assert.AreEqual(54.15d, moveClass.ContactArea, 0.01d);
        }

        [TestMethod]
        public void GiantGrizzlyBear_ScratchPenetration()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("scratch"));

            Assert.AreEqual(400.88d, moveClass.MaxPenetration, 0.01d);
        }

        [TestMethod]
        public void GiantGrizzlyBear_BiteMomentum()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);

            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));
            var move = CombatMoveFactory.BodyMove(attacker, attacker, moveClass, attacker.Body.Parts.First());

            var mom = move.GetStrikeMomentum();
            Assert.AreEqual(727.60, mom, 0.01d);
        }

        [TestMethod]
        public void GiantGrizzlyBear_BiteContactArea()
        {
            var attacker = DfTagsFascade.CreateCreatureAgent(Atlas, "GIANT_BEAR_GRIZZLY", "MALE", Vector3.Zero);
            var moveClass = attacker.Body.Moves.First(x => x.Name.Equals("bite"));

            Assert.AreEqual(127.67d, moveClass.ContactArea, 0.01d);
        }

        IAgent GetNewDwarf()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
        }

        IAgent GetNewUnicorn()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
        }

        IAgent GetNewGiantCaveSpider()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "SPIDER_CAVE_GIANT", "MALE", Vector3.Zero);
        }

        IAgent GetNewBandedKnifeFish()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "FISH_KNIFEFISH_BANDED", "MALE", Vector3.Zero);
        }
        
        IAgent GetNewBlueShark()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "SHARK_BLUE", "MALE", Vector3.Zero);
        }
    }
}
