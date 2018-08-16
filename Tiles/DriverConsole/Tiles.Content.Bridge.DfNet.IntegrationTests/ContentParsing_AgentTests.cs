using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet.IntegrationTests
{
    [TestClass]
    public class ContentParsing_AgentTests
    {
        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }

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
        }


        [TestMethod]
        public void Dwarf_HeadTissues()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            var head = agent.Body.Parts.Single(p => p.NameSingular.Equals("head"));
            
            var tissueLayers = head.Tissue.Layers;
            Assert.AreEqual(6, tissueLayers.Count);

            var eyebrow = tissueLayers.Single(x => x.Material.Name.Equals("eyebrow"));
            Assert.IsTrue(eyebrow.IsCosmetic);
            var whisker = tissueLayers.Single(x => x.Material.Name.Equals("chin whisker"));
            Assert.IsTrue(whisker.IsCosmetic);
            var hair = tissueLayers.Single(x => x.Material.Name.Equals("hair"));
            Assert.IsTrue(hair.IsCosmetic);

            var skin = tissueLayers.Single(x => x.Material.Name.Equals("skin"));
            Assert.IsFalse(skin.IsCosmetic);
            Assert.IsTrue(skin.IsConnective);
            Assert.AreEqual(1, skin.VascularRating);
            Assert.AreEqual(100, skin.HealingRate);
            Assert.AreEqual(5, skin.PainReceptors);
            var fat = tissueLayers.Single(x => x.Material.Name.Equals("fat"));
            Assert.IsFalse(fat.IsCosmetic);
            Assert.IsFalse(fat.HasArteries);
            var muscle = tissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            Assert.IsFalse(muscle.IsCosmetic);
            Assert.IsTrue(muscle.HasArteries);
        }

        [TestMethod]
        public void Dwarf_Bite()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            var move = agent.Body.Moves.First(x => x.Name.Equals("BITE"));
            Assert.AreEqual(1, move.Requirements.Count());
            Assert.AreEqual(2, move.Requirements.First().Constraints.Count());
            var con = move.Requirements.First().Constraints.ElementAt(0);
            Assert.AreEqual(BprConstraintType.ByCategory, con.ConstraintType);
            Assert.IsTrue(new string[] { "HEAD" }.SequenceEqual(con.Tokens));

            con = move.Requirements.First().Constraints.ElementAt(1);
            Assert.AreEqual(BprConstraintType.ByCategory, con.ConstraintType);
            Assert.IsTrue(new string[] { "TOOTH" }.SequenceEqual(con.Tokens));

            Assert.AreEqual(9, move.ContactArea, 0.5);
            Assert.AreEqual(6, move.MaxPenetration, 0.5);

            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);
        }


        [TestMethod]
        public void Dwarf_Kick()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            Assert.IsNotNull(agent);

            var move = agent.Body.Moves.First(x => x.Name.Equals("KICK"));
            Assert.AreEqual(25d, move.ContactArea, 1d);
            Assert.AreEqual(129d, move.MaxPenetration, 1d);

            Assert.AreEqual(4, move.PrepTime);
            Assert.AreEqual(4, move.RecoveryTime);
        }

        [TestMethod]
        public void Dwarf_FeetCharacteristic()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            Assert.IsNotNull(agent);

            var leftFoot = agent.Body.Parts.Single(p => p.IsStance && p.IsLeft);
            var rightFoot = agent.Body.Parts.Single(p => p.IsStance && p.IsRight);
            Assert.AreEqual("left foot", leftFoot.NameSingular);
            Assert.AreEqual("right foot", rightFoot.NameSingular);
        }

        [TestMethod]
        public void Dwarf_Skull()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            var part = agent.Body.Parts.Single(x => x.NameSingular == "skull");
            Assert.IsTrue(part.IsConnector);
            Assert.IsTrue(part.PreventsParentCollapse);
        }

        [TestMethod]
        public void Dwarf_BloodAndPus()
        {
            var agent = DfAgentFactory.Create("DWARF", "MALE");
            Assert.IsNotNull(agent.Body.BloodMaterial);
            Assert.IsNotNull(agent.Body.PusMaterial);
        }

        [TestMethod]
        public void BronzeColossus_BloodAndPus()
        {
            var agent = DfAgentFactory.Create("COLOSSUS_BRONZE", "MALE");
            Assert.IsNull(agent.Body.BloodMaterial);
            Assert.IsNull(agent.Body.PusMaterial);
        }

        [TestMethod]
        public void GreenDevourer()
        {
            var agent = DfAgentFactory.Create("GREEN_DEVOURER");
            Assert.IsNotNull(agent);
            Assert.AreEqual((int)'G', agent.Sprite.Symbol);
            Assert.IsTrue(
                agent.Body.Parts
                .Where(p => p.Tissue.Layers.Any())
                .Any());

            Assert.IsFalse(
                agent.Body.Parts
                .Where(p => p.Tissue.Layers.Any(
                    layer => layer.Material.Adjective.Equals("bone")))
                .Any());

            Assert.IsTrue(
                agent.Body.Parts
                .Where(p => p.Tissue.Layers.Any(
                    layer => layer.Material.Adjective.Equals("skin")))
                .Any());
        }


        [TestMethod]
        public void HumanSkin()
        {
            var agent = DfAgentFactory.Create("HUMAN");
            Assert.IsNotNull(agent);

            var up = agent.Body.Parts.Single(x => x.NameSingular.Equals("upper body"));
            var skin = up.Tissue.Layers.Single(x => x.Material.Name.Equals("skin"));
            Assert.AreEqual(1, skin.RelativeThickness);
        }


        [TestMethod]
        public void HumanPunch()
        {
            var agent = DfAgentFactory.Create("HUMAN");
            Assert.IsNotNull(agent);

            var punch = agent.Body.Moves.First(x => x.Name.Equals("PUNCH"));
            Assert.AreEqual(21d, punch.ContactArea, 1d);
            Assert.AreEqual(100, punch.MaxPenetration, 1d);

            Assert.AreEqual(3, punch.PrepTime);
            Assert.AreEqual(3, punch.RecoveryTime);
        }

        [TestMethod]
        public void SoldierAntman()
        {
            var agent = DfAgentFactory.Create("ANT_MAN", "SOLDIER");
            Assert.IsNotNull(agent);
        }

        [TestMethod]
        public void CaveFloater()
        {
            var agent = DfAgentFactory.Create("CAVE_FLOATER");
            Assert.IsNotNull(agent);
        }

        [TestMethod]
        public void BloodWoman()
        {
            var agent = DfAgentFactory.Create("BLOOD_MAN", "FEMALE");
            Assert.IsNotNull(agent);

            var graspers = agent.Body.Parts.Where(p => p.Types.Contains("GRASP"));
            Assert.AreEqual(2, graspers.Count());
            var hand = graspers.FirstOrDefault();
            Assert.AreEqual("right hand", hand.NameSingular);

            Assert.AreEqual(1, hand.Tissue.Layers.Count());
        }

        [TestMethod]
        public void Hippo_Ivory()
        {
            var agent = DfAgentFactory.Create("HIPPO", "MALE");
            Assert.IsNotNull(agent);

            var leftEyeTooth = agent.Body.Parts.Where(p => p.NameSingular == "left eye tooth").Single();
            Assert.IsNotNull(leftEyeTooth);

            var rightEyeTooth = agent.Body.Parts.Where(p => p.NameSingular == "right eye tooth").Single();
            Assert.IsNotNull(rightEyeTooth);

            Assert.AreEqual("ivory", rightEyeTooth.Tissue.Layers.Single().Material.Name);
            Assert.AreEqual("ivory", leftEyeTooth.Tissue.Layers.Single().Material.Name);
        }

        [TestMethod]
        public void Hippo_ThroatArteries()
        {
            var agent = DfAgentFactory.Create("HIPPO", "MALE");
            Assert.IsNotNull(agent);

            var throat = agent.Body.Parts.Single(p => p.NameSingular == "throat");
            Assert.IsNotNull(throat);

            Assert.IsTrue(throat.Tissue.Layers.Single(x => x.Material.Name.Equals("skin")).HasMajorArteries);
        }

        [TestMethod]
        public void Hippo_HeartArteries()
        {
            var agent = DfAgentFactory.Create("HIPPO", "MALE");
            Assert.IsNotNull(agent);

            var heart = agent.Body.Parts.Single(p => p.NameSingular == "heart");
            Assert.IsNotNull(heart);

            Assert.IsTrue(heart.Tissue.Layers.Single().HasMajorArteries);
        }
    }
}
