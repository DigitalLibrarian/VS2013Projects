using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles;
using Tiles.Items;
using Tiles.Agents;
using Tiles.Bodies;
using Moq;
using Tiles.Math;
using Tiles.Tests.Assertions;
using Tiles.Structures;
using System.Collections.Generic;
using Tiles.Items.Outfits;
using Tiles.Agents.Behaviors;
using Tiles.Agents.Combat;

namespace Tiles.Tests.Agents
{
    [TestClass]
    public class AgentTests
    {
        Mock<IAtlas> AtlasMock { get; set; }
        Sprite Sprite { get; set; }
        Mock<IBody> BodyMock { get; set; }
        Mock<IInventory> InventoryMock { get; set; }
        Mock<IOutfit> OutfitMock { get; set; }
        Mock<IAgentCommandQueue> CommandQueueMock { get; set; }
        Mock<IAgentClass> AgentClassMock { get; set; }
        string Name { get; set; }

        Agent Agent { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            AtlasMock = new Mock<IAtlas>();
            Sprite = new Sprite();
            BodyMock = new Mock<IBody>();
            BodyMock.Setup(x => x.Parts).Returns(new List<IBodyPart>());
            InventoryMock = new Mock<IInventory>();
            OutfitMock = new Mock<IOutfit>();
            CommandQueueMock = new Mock<IAgentCommandQueue>();
            Name = "name";
            AgentClassMock = new Mock<IAgentClass>();
            AgentClassMock.Setup(x => x.Name).Returns(Name);
            AgentClassMock.Setup(x => x.Sprite).Returns(Sprite);

            Agent = new Agent(
                AtlasMock.Object, 
                AgentClassMock.Object,
                Vector3.Zero, 
                BodyMock.Object, 
                InventoryMock.Object, 
                OutfitMock.Object,
                CommandQueueMock.Object);
        }


        [TestMethod]
        public void Constructor()
        {
            Assert.IsNull(Agent.AgentBehavior);
            Assert.AreSame(AtlasMock.Object, Agent.Atlas);
            Assert.AreEqual(Sprite, Agent.Sprite);
            Assert.AreSame(BodyMock.Object, Agent.Body);
            Assert.AreSame(InventoryMock.Object, Agent.Inventory);
            Assert.AreSame(OutfitMock.Object, Agent.Outfit);
            Assert.AreEqual(Vector3.Zero, Agent.Pos);
            Assert.IsFalse(Agent.IsPlayer);
            Assert.IsNull(Agent.AgentBehavior);
            Assert.AreEqual(Name, Agent.Name);
            Assert.IsTrue(Agent.IsProne);
        }

        [TestMethod]
        public void VacuouslyCannotStand()
        {
            Assert.IsFalse(Agent.Body.Parts.Any());
            Assert.IsFalse(Agent.CanStand);
        }

        [TestMethod]
        public void CanMove_Wrestling()
        {
            BodyMock.Setup(x => x.IsWrestling).Returns(true);

            Assert.IsFalse(Agent.CanMove(new Vector3()));
            AtlasMock.Verify(x => x.GetTileAtPos(It.IsAny<Vector3>()), Times.Never());
        }

        [TestMethod]
        public void IsDead()
        {
            BodyMock.Setup(x => x.IsDead).Returns(false);
            Assert.IsFalse(Agent.IsDead);

            BodyMock.Setup(x => x.IsDead).Returns(true);
            Assert.IsTrue(Agent.IsDead);
        }

        [TestMethod]
        public void Update()
        {
            var gameMock = new Mock<IGame>();

            Agent.Update(gameMock.Object);

            var behaveMock = new Mock<IAgentBehavior>();
            Agent.AgentBehavior = behaveMock.Object;

            Agent.Update(gameMock.Object);

            behaveMock.Verify(x => x.Update(gameMock.Object, Agent), Times.Once());
        }

        [TestMethod]
        public void Move_ImpassableTerrain_NoStructure_NoAgent()
        {
            var delta = new Vector3(1, 1, 0);
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);

            var newTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns(newTileMock.Object);

            // Show failure because of impassable terrain (no structure)
            newTileMock.Setup(x => x.HasStructureCell).Returns(false);
            newTileMock.Setup(x => x.StructureCell).Returns((IStructureCell)null);
            newTileMock.Setup(x => x.IsTerrainPassable).Returns(false);
            newTileMock.Setup(x => x.HasAgent).Returns(false);
            newTileMock.Setup(x => x.Agent).Returns((IAgent)null);
            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);
            newTileMock.Verify(x => x.IsTerrainPassable, Times.Once());

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Never());
            newTileMock.Verify(x => x.SetAgent(Agent), Times.Never());
        }

        [TestMethod]
        public void Move_ImpassableTerrain_PassableStructure_NoAgent()
        {
            var delta = new Vector3(1, 1, 0);
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);

            var newTileMock = new Mock<ITile>();
            var structureCellMock = new Mock<IStructureCell>();
            structureCellMock.Setup(x => x.CanPass).Returns(true);
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns(newTileMock.Object);

            newTileMock.Setup(x => x.HasStructureCell).Returns(true);
            newTileMock.Setup(x => x.StructureCell).Returns(structureCellMock.Object);
            newTileMock.Setup(x => x.IsTerrainPassable).Returns(false);
            newTileMock.Setup(x => x.HasAgent).Returns(false);
            newTileMock.Setup(x => x.Agent).Returns((IAgent)null);

            var result = Agent.Move(delta);

            Assert.IsTrue(result);
            Asserter.AreEqual(newDest, Agent.Pos);

            newTileMock.Verify(x => x.HasAgent, Times.Once());
            newTileMock.Verify(x => x.HasStructureCell, Times.Once());
            newTileMock.Verify(x => x.IsTerrainPassable, Times.Never());

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Once());
            newTileMock.Verify(x => x.SetAgent(Agent), Times.Once());
        }
        
        [TestMethod]
        public void Move_PassableTerrain_ImpassableStructure_NoAgent()
        {
            var delta = new Vector3(1, 1, 0);
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);

            var newTileMock = new Mock<ITile>();
            var structureCellMock = new Mock<IStructureCell>();
            structureCellMock.Setup(x => x.CanPass).Returns(false);
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns(newTileMock.Object);

            newTileMock.Setup(x => x.HasStructureCell).Returns(true);
            newTileMock.Setup(x => x.StructureCell).Returns(structureCellMock.Object);
            newTileMock.Setup(x => x.IsTerrainPassable).Returns(true);
            newTileMock.Setup(x => x.HasAgent).Returns(false);
            newTileMock.Setup(x => x.Agent).Returns((IAgent)null);

            var result = Agent.Move(delta);

            Assert.IsFalse(result);
            Asserter.AreEqual(originalPos, Agent.Pos);

            newTileMock.Verify(x => x.HasAgent, Times.Once());
            newTileMock.Verify(x => x.HasStructureCell, Times.Once());
            newTileMock.Verify(x => x.IsTerrainPassable, Times.Never());

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Never());
            newTileMock.Verify(x => x.SetAgent(Agent), Times.Never());
        }

        [TestMethod]
        public void Move_PassableTerrain_PassableStructure_NoAgent()
        {
            var delta = new Vector3(1, 1, 0);
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);

            var newTileMock = new Mock<ITile>();
            var structureCellMock = new Mock<IStructureCell>();
            structureCellMock.Setup(x => x.CanPass).Returns(true);
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns(newTileMock.Object);

            newTileMock.Setup(x => x.HasStructureCell).Returns(true);
            newTileMock.Setup(x => x.StructureCell).Returns(structureCellMock.Object);
            newTileMock.Setup(x => x.IsTerrainPassable).Returns(true);
            newTileMock.Setup(x => x.HasAgent).Returns(false);
            newTileMock.Setup(x => x.Agent).Returns((IAgent)null);

            var result = Agent.Move(delta);

            Assert.IsTrue(result);
            Asserter.AreEqual(newDest, Agent.Pos);

            newTileMock.Verify(x => x.HasAgent, Times.Once());
            newTileMock.Verify(x => x.HasStructureCell, Times.Once());
            newTileMock.Verify(x => x.IsTerrainPassable, Times.Never());

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Once());
            newTileMock.Verify(x => x.SetAgent(Agent), Times.Once());
        }

        
        [TestMethod]
        public void Move_PassableTerrain_NoStructure_NoAgent()
        {
            var delta = new Vector3(1, 1, 0);
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);

            var newTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns(newTileMock.Object);

            newTileMock.Setup(x => x.HasStructureCell).Returns(false);
            newTileMock.Setup(x => x.StructureCell).Returns((IStructureCell) null);
            newTileMock.Setup(x => x.IsTerrainPassable).Returns(true);
            newTileMock.Setup(x => x.HasAgent).Returns(false);
            
            var result = Agent.Move(delta);

            Assert.IsTrue(result);
            Asserter.AreEqual(newDest, Agent.Pos);

            newTileMock.Verify(x => x.HasAgent, Times.Once());
            newTileMock.Verify(x => x.HasStructureCell, Times.Once());
            newTileMock.Verify(x => x.IsTerrainPassable, Times.Once());

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Once());
            newTileMock.Verify(x => x.SetAgent(Agent), Times.Once());
        }

        [TestMethod]
        public void Move_PassableTerrain_NoStructure_HasAgent()
        {
            var delta = new Vector3(1, 1, 0);
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);

            var newTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns(newTileMock.Object);

            newTileMock.Setup(x => x.HasStructureCell).Returns(false);
            newTileMock.Setup(x => x.StructureCell).Returns((IStructureCell)null);
            newTileMock.Setup(x => x.IsTerrainPassable).Returns(true);
            newTileMock.Setup(x => x.HasAgent).Returns(true);

            var result = Agent.Move(delta);
            Assert.IsFalse(result);
            Asserter.AreEqual(originalPos, Agent.Pos);

            newTileMock.Verify(x => x.HasAgent, Times.Once());
            newTileMock.Verify(x => x.HasStructureCell, Times.Never());
            newTileMock.Verify(x => x.IsTerrainPassable, Times.Never());

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Never());
            newTileMock.Verify(x => x.SetAgent(Agent), Times.Never());
        }

        [TestMethod]
        public void Move_NewTileDoesntExist()
        {
            var delta = new Vector3();
            var originalPos = Agent.Pos;
            var newDest = originalPos + delta;

            var originalTileMock = new Mock<ITile>();
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(newDest)).Returns((ITile)null);
            AtlasMock.Setup(atlas => atlas.GetTileAtPos(originalPos)).Returns(originalTileMock.Object);

            Assert.IsFalse(Agent.Move(delta));
            Asserter.AreEqual(originalPos, Agent.Pos);
            
            var result = Agent.Move(delta);
            Assert.IsFalse(result);
            Asserter.AreEqual(originalPos, Agent.Pos);

            originalTileMock.Verify(x => x.RemoveAgent(), Times.Never());
        }

        [TestMethod]
        public void GetStrikeMomentum_SteelAxeSwordHack()
        {
            int bodySize = 6000;
            BodyMock.Setup(x => x.Size).Returns(bodySize);
            BodyMock.Setup(x => x.GetAttribute("STRENGTH"))
                .Returns(1250);

            var weaponMock = new Mock<IItem>();
            weaponMock.Setup(x => x.GetMass()).Returns(6280);

            int veloMultiply = 1250;
            var moveClassMock = new Mock<ICombatMoveClass>();
            moveClassMock.Setup(x => x.VelocityMultiplier).Returns(veloMultiply);
            moveClassMock.Setup(x => x.IsItem).Returns(true);

            var moveMock = new Mock<ICombatMove>();
            moveMock.Setup(x => x.Weapon).Returns(weaponMock.Object);
            moveMock.Setup(x => x.Class).Returns(moveClassMock.Object);

            var strikeMom = Agent.GetStrikeMomentum(moveMock.Object);
            Assert.AreEqual(86, (int) strikeMom);

            moveClassMock.Verify(x => x.GetRelatedBodyParts(It.IsAny<IBody>()), Times.Never());
        }

#region Stance

        private void SetupStanceFixture(out Mock<IBodyPart> leftFootMock, out Mock<IBodyPart> rightFootMock, out Mock<IBodyPart> controlMock)
        {
            leftFootMock = new Mock<IBodyPart>();
            leftFootMock.Setup(x => x.IsStance).Returns(true);
            leftFootMock.Setup(x => x.IsLeft).Returns(true);

            rightFootMock = new Mock<IBodyPart>();
            rightFootMock.Setup(x => x.IsStance).Returns(true);
            rightFootMock.Setup(x => x.IsRight).Returns(true);

            controlMock = new Mock<IBodyPart>();

            var parts = new List<IBodyPart>
            {
                controlMock.Object, leftFootMock.Object, rightFootMock.Object
            };

            BodyMock.Setup(x => x.Parts).Returns(parts);
            BodyMock.Setup(x => x.Amputate(It.IsAny<IBodyPart>()))
                .Callback<IBodyPart>(p => parts.Remove(p));

            Agent = new Agent(
                AtlasMock.Object,
                AgentClassMock.Object,
                Vector3.Zero,
                BodyMock.Object,
                InventoryMock.Object,
                OutfitMock.Object,
                CommandQueueMock.Object);

            Assert.IsFalse(Agent.IsProne);
            Assert.IsTrue(Agent.CanStand);
        }

        [TestMethod]
        public void Sever_IsProne_Unaffected()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Agent.Sever(controlMock.Object);

            Assert.IsFalse(Agent.IsProne);
        }

        [TestMethod]
        public void Sever_IsProne_SeverLastLeft()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Agent.Sever(leftFootMock.Object);

            Assert.IsTrue(Agent.IsProne);
            Assert.IsFalse(Agent.CanStand);
        }

        [TestMethod]
        public void Sever_IsProne_PulpLastLeft()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            leftFootMock.Setup(x => x.IsEffectivelyPulped).Returns(true);
            Agent.Sever(controlMock.Object);

            Assert.IsTrue(Agent.IsProne);
            Assert.IsFalse(Agent.CanStand);
        }

        [TestMethod]
        public void Sever_IsProne_SeverLastRight()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Agent.Sever(rightFootMock.Object);

            Assert.IsTrue(Agent.IsProne);
            Assert.IsFalse(Agent.CanStand);
        }

        [TestMethod]
        public void Sever_IsProne_PulpLastRight()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            rightFootMock.Setup(x => x.IsEffectivelyPulped).Returns(true);
            Agent.Sever(controlMock.Object);

            Assert.IsTrue(Agent.IsProne);
            Assert.IsFalse(Agent.CanStand);
        }

        [TestMethod]
        public void StandUp_IsProne()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Assert.IsFalse(Agent.IsProne);
            Assert.IsTrue(Agent.LayDown());
            Assert.IsTrue(Agent.IsProne);
            Assert.IsTrue(Agent.CanStand);

            Assert.IsTrue(Agent.StandUp());
            Assert.IsFalse(Agent.IsProne);
        }

        [TestMethod]
        public void StandUp_Standing()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Assert.IsFalse(Agent.IsProne);
            Assert.IsTrue(Agent.LayDown());
            Assert.IsTrue(Agent.IsProne);
            Assert.IsTrue(Agent.CanStand);

            Assert.IsTrue(Agent.StandUp());
            Assert.IsFalse(Agent.IsProne);

            Assert.IsFalse(Agent.StandUp());
            Assert.IsFalse(Agent.IsProne);
        }

        [TestMethod]
        public void StandUp_Unable()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Assert.IsTrue(Agent.CanStand);
            rightFootMock.Setup(x => x.IsEffectivelyPulped).Returns(true);
            Assert.IsFalse(Agent.CanStand);

            Assert.IsFalse(Agent.StandUp());
            Assert.IsFalse(Agent.IsProne);
        }

        [TestMethod]
        public void LayDown_IsProne()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Assert.IsFalse(Agent.IsProne);
            Assert.IsTrue(Agent.LayDown());
            Assert.IsTrue(Agent.IsProne);

            Assert.IsFalse(Agent.LayDown());
            Assert.IsTrue(Agent.IsProne);
        }

        [TestMethod]
        public void LayDown_Standing()
        {
            Mock<IBodyPart> leftFootMock, rightFootMock, controlMock;
            SetupStanceFixture(out leftFootMock, out rightFootMock, out controlMock);

            Assert.IsFalse(Agent.IsProne);
            Assert.IsTrue(Agent.LayDown());
            Assert.IsTrue(Agent.IsProne);
        }
#endregion

        [TestMethod]
        public void PainThreshold()
        {
            foreach (var pair in new[] { 
                new [] { 1000, 100 }
            })
            {
                BodyMock.Setup(x => x.GetAttribute("WILLPOWER"))
                    .Returns(pair[0]);

                Assert.AreEqual(pair[1], Agent.GetPainThreshold());
            }
        }
    }
}
