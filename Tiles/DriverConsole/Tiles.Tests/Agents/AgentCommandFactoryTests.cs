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
            var command = Factory.Nothing(agentMock.Object);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
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
            var dir = new Vector2(1, 1);
            var command = Factory.MoveDirection(agentMock.Object, dir);

            Assert.AreEqual(AgentCommandType.Move, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
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
            var agentMock = new Mock<IAgent>();
            var command = Factory.PickUpItemsOnAgentTile(agentMock.Object);

            Assert.AreEqual(AgentCommandType.PickUpItemsOnAgentTile, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void MeleeAttack()
        {
            var agentMock = new Mock<IAgent>();
            var targetMock = new Mock<IAgent>();
            var attackMock = new Mock<IAttackMove>();

            var command = Factory.MeleeAttack(agentMock.Object, targetMock.Object, attackMock.Object);

            Assert.AreEqual(AgentCommandType.AttackMelee, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.AreSame(targetMock.Object, command.Target);
            Assert.AreSame(attackMock.Object, command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void WieldWeapon()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();
            var weaponMock = new Mock<IWeapon>();

            var command = Factory.WieldWeapon(agentMock.Object, itemMock.Object, weaponMock.Object);

            Assert.AreEqual(AgentCommandType.WieldWeapon, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Item);
            Assert.AreSame(weaponMock.Object, command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void WearArmor()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();
            var armorMock = new Mock<IArmor>();

            var command = Factory.WearArmor(agentMock.Object, itemMock.Object, armorMock.Object);

            Assert.AreEqual(AgentCommandType.WearArmor, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Item);
            Assert.IsNull(command.Weapon);
            Assert.AreSame(armorMock.Object, command.Armor);
        }

        [TestMethod]
        public void UnwieldWeapon()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();
            var weaponMock = new Mock<IWeapon>();

            var command = Factory.UnwieldWeapon(agentMock.Object, itemMock.Object, weaponMock.Object);

            Assert.AreEqual(AgentCommandType.UnwieldWeapon, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Item);
            Assert.AreSame(weaponMock.Object, command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void TakeOffArmor()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();
            var armorMock = new Mock<IArmor>();

            var command = Factory.TakeOffArmor(agentMock.Object, itemMock.Object, armorMock.Object);

            Assert.AreEqual(AgentCommandType.TakeOffArmor, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Item);
            Assert.IsNull(command.Weapon);
            Assert.AreSame(armorMock.Object, command.Armor);
        }

        [TestMethod]
        public void DropInventoryItem()
        {
            var agentMock = new Mock<IAgent>();
            var itemMock = new Mock<IItem>();

            var command = Factory.DropInventoryItem(agentMock.Object, itemMock.Object);

            Assert.AreEqual(AgentCommandType.DropInventoryItem, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.AreSame(itemMock.Object, command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }
    }
}
