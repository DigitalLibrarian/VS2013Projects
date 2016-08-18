using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Tests.Agents
{
    [TestClass]
    public class AgentCommandFactoryTests
    {
        AgentCommandFactory Factory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Factory = new AgentCommandFactory();
        }

        [TestMethod]
        public void Nothing()
        {
            var agentMock = new Mock<IAgent>();
            var commands = Factory.Nothing(agentMock.Object);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void Move()
        {
            var agentMock = new Mock<IAgent>();
            var dir = new Vector3(1, 1, 1);
            var commands = Factory.MoveDirection(agentMock.Object, dir);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.Move, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(dir, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void PickUpItemsOnAgentTile()
        {
            var gameMock = new Mock<IGame>();
            var atlasMock = new Mock<IAtlas>();
            gameMock.Setup(x => x.Atlas).Returns(atlasMock.Object);

            var agentMock = new Mock<IAgent>();
            var agentPos = new Vector3(2, 3, 4);
            agentMock.Setup(x => x.Pos).Returns(agentPos);

            var items = new List<IItem>{
                new Mock<IItem>().Object,
                new Mock<IItem>().Object,
                new Mock<IItem>().Object
            };
            var tileMock = new Mock<ITile>();
            tileMock.Setup(x => x.Items).Returns(items);
            atlasMock.Setup(x => x.GetTileAtPos(agentPos)).Returns(tileMock.Object);

            var commands = Factory.PickUpItemsOnAgentTile(gameMock.Object, agentMock.Object);
            Assert.AreEqual(items.Count(), commands.Count());

            for (int i = 0; i < items.Count();i++ )
            {
                IItem item = items[i];
                var command = commands.Single(x => x.Item == item);

                Assert.AreEqual(AgentCommandType.PickUpItemOnAgentTile, command.CommandType);
                Asserter.AreEqual(Vector3.Zero, command.TileOffset);
                Asserter.AreEqual(Vector3.Zero, command.Direction);
                Assert.IsNull(command.Target);
                Assert.IsNull(command.AttackMove);
                Assert.AreSame(item, command.Item);
                Assert.IsNull(command.Weapon);
                Assert.IsNull(command.Armor);
                Assert.AreNotEqual(0, command.RequiredTime);
            }
        }

        [TestMethod]
        public void MeleeAttack()
        {
            var agentMock = new Mock<IAgent>();
            var targetMock = new Mock<IAgent>();
            var attackMock = new Mock<ICombatMove>();
            var classMock = new Mock<ICombatMoveClass>();
            attackMock.Setup(x => x.Class).Returns(classMock.Object);
            int prepTime = 1;
            int recoveryTime = 2;
            classMock.Setup(x => x.PrepTime).Returns(prepTime);
            classMock.Setup(x => x.RecoveryTime).Returns(recoveryTime);

            var commands = Factory.MeleeAttack(agentMock.Object, targetMock.Object, attackMock.Object);
            Assert.AreEqual(3, commands.Count());

            var command = commands.ElementAt(0);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
            Assert.IsTrue(command.RequiredTime > 0);

            command = commands.ElementAt(1);

            Assert.AreEqual(AgentCommandType.AttackMelee, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.AreSame(targetMock.Object, command.Target);
            Assert.AreSame(attackMock.Object, command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
            Assert.IsTrue(command.RequiredTime > 0);

            command = commands.ElementAt(2);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
            Assert.IsTrue(command.RequiredTime > 0);
        }
        
        [TestMethod]
        public void WieldWeapon()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();

            var commands = Factory.WieldWeapon(agentMock.Object, itemMock.Object);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.WieldWeapon, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Weapon);
            Assert.IsNull(command.Armor);
            Assert.IsNull(command.Item);
        }
        
        [TestMethod]
        public void WearArmor()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();

            var commands = Factory.WearArmor(agentMock.Object, itemMock.Object);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.WearArmor, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Armor);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Item);
        }
        
        [TestMethod]
        public void UnwieldWeapon()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();

            var commands = Factory.UnwieldWeapon(agentMock.Object, itemMock.Object);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.UnwieldWeapon, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Weapon);
            Assert.IsNull( command.Item);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void TakeOffArmor()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();

            var commands = Factory.TakeOffArmor(agentMock.Object, itemMock.Object);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.TakeOffArmor, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Armor);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
        }

        [TestMethod]
        public void DropInventoryItem()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();

            var commands = Factory.DropInventoryItem(agentMock.Object, itemMock.Object);
            Assert.AreEqual(1, commands.Count());
            var command = commands.Single();

            Assert.AreEqual(AgentCommandType.DropInventoryItem, command.CommandType);
            Asserter.AreEqual(Vector3.Zero, command.TileOffset);
            Asserter.AreEqual(Vector3.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }
    }
}
