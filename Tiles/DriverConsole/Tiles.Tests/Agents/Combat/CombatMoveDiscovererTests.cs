using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Math;

namespace Tiles.Tests.Agents.Combat
{
    [TestClass]
    public class CombatMoveDiscovererTests
    {
        Mock<ICombatMoveBuilder> BuilderMock { get; set; }
        CombatMoveDiscoverer Disco { get; set; }

        Mock<IAgent> AttackerMock { get; set; }
        Mock<IBody> AttackerBodyMock { get; set; }
        IList<IBodyPart> AttackerBodyParts { get; set; }

        Mock<IOutfit> AttackerOutfitMock { get; set; }
        
        Mock<IAgent> DefenderMock { get; set; }
        Mock<IBody> DefenderBodyMock { get; set; }
        IList<IBodyPart> DefenderBodyParts { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            BuilderMock = new Mock<ICombatMoveBuilder>();
            Disco = new CombatMoveDiscoverer(BuilderMock.Object);

            AttackerBodyParts = new List<IBodyPart>();
            AttackerBodyMock = new Mock<IBody>();
            AttackerBodyMock.Setup(x => x.Parts).Returns(AttackerBodyParts);
            AttackerOutfitMock = new Mock<IOutfit>();

            AttackerMock = new Mock<IAgent>();
            AttackerMock.Setup(x => x.Body).Returns(AttackerBodyMock.Object);
            AttackerMock.Setup(x => x.Outfit).Returns(AttackerOutfitMock.Object);

            DefenderBodyMock = new Mock<IBody>();
            DefenderBodyParts = new List<IBodyPart>();
            DefenderBodyMock.Setup(x => x.Parts).Returns(DefenderBodyParts);

            DefenderMock = new Mock<IAgent>();
            DefenderMock.Setup(x => x.Body).Returns(DefenderBodyMock.Object);
        }

        Mock<IBodyPart> AddBodyPart(IList<IBodyPart> parts, Mock<IBodyPart> partMock = null)
        {
            partMock = partMock ?? new Mock<IBodyPart>();
            parts.Add(partMock.Object);

            return partMock;
        }

        Mock<IItem> MockWeaponItem(params Mock<ICombatMoveClass>[] moveClasses)
        {
            var itemMock = new Mock<IItem>();
            var weaponClassMock = new Mock<IWeaponClass>();
            itemMock.Setup(x => x.Class.WeaponClass).Returns(weaponClassMock.Object);
            weaponClassMock.Setup(x => x.AttackMoveClasses).Returns(moveClasses.Select(x => x.Object).ToList());

            return itemMock;
        }

        void AssertNoMovesBuilt()
        {
            BuilderMock.Verify(
                x => x.AttackBodyPartWithWeapon(
                    It.IsAny<IAgent>(), 
                    It.IsAny<IAgent>(),
                    It.IsAny<ICombatMoveClass>(), 
                    It.IsAny<IBodyPart>(), 
                    It.IsAny<IItem>()
                    ), Times.Never());
        }

        [Ignore]
        [TestMethod]
        public void TODO_WriteTestsForNewDiscoveryFunctionality() { }

        [TestMethod]
        public void NoPossibleMoves_NoAttackerBodyParts()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));
            Assert.IsFalse(
                Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).Any());
            AssertNoMovesBuilt();
        }

        [TestMethod]
        public void NoPossibleMoves_NoWeapon()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));
            var partMock = AddBodyPart(AttackerBodyParts);
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(partMock.Object)).Returns((IItem)null);

            Assert.IsFalse(
                Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).Any());

            AttackerOutfitMock.Verify(x => x.GetWeaponItem(partMock.Object), Times.Once());

            AssertNoMovesBuilt();
        }

        [TestMethod]
        public void NoPossibleMoves_NoAttackMoveClasses()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));
            var partMock = AddBodyPart(AttackerBodyParts);
            var itemMock = MockWeaponItem();
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(partMock.Object)).Returns(itemMock.Object);

            Assert.IsFalse(
                Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).Any());

            AttackerOutfitMock.Verify(x => x.GetWeaponItem(partMock.Object), Times.Once());
            AssertNoMovesBuilt();
        }

        [TestMethod]
        public void NoPossibleMoves_NoDefenderBodyParts()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));
            var partMock = AddBodyPart(AttackerBodyParts);
            var itemMock = MockWeaponItem(new Mock<ICombatMoveClass>());
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(partMock.Object)).Returns(itemMock.Object);

            Assert.IsFalse(
                Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).Any());

            AttackerOutfitMock.Verify(x => x.GetWeaponItem(partMock.Object), Times.Once());
            AssertNoMovesBuilt();
        }

        [TestMethod]
        public void NoPossibleMoves_NotAdjacent()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(10, 10, 0));

            AddBodyPart(DefenderBodyParts);

            var partMock = AddBodyPart(AttackerBodyParts);
            var itemMock = MockWeaponItem(new Mock<ICombatMoveClass>());
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(partMock.Object)).Returns(itemMock.Object);

            Assert.IsFalse(
                Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).Any());
            
            AttackerOutfitMock.Verify(x => x.GetWeaponItem(partMock.Object), Times.Never()); 
            AssertNoMovesBuilt();

        }

        [TestMethod]
        public void PossibleMeleeMoves()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));
            
            var defenderBodyPartMock1 = AddBodyPart(DefenderBodyParts);
            var defenderBodyPartMock2 = AddBodyPart(DefenderBodyParts);

            var attackClassMock1 = new Mock<ICombatMoveClass>();
            var attackClassMock2 = new Mock<ICombatMoveClass>();

            var partMock = AddBodyPart(AttackerBodyParts);
            var itemMock = MockWeaponItem(attackClassMock1, attackClassMock2);
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(partMock.Object)).Returns(itemMock.Object);

            var spoofedResult = new Mock<ICombatMove>();
            BuilderMock.Setup(
                x => x.AttackBodyPartWithWeapon(
                    It.IsAny<IAgent>(),
                    It.IsAny<IAgent>(),
                    It.IsAny<ICombatMoveClass>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<IItem>()
                    )).Returns(spoofedResult.Object);

            var result = Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).ToList();

            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.All(x => x == spoofedResult.Object));


            foreach (var bp in new[] { defenderBodyPartMock1, defenderBodyPartMock2 })
            {
                foreach(var ac in new [] { attackClassMock1, attackClassMock2})
                {
                    BuilderMock.Verify(
                        x => x.AttackBodyPartWithWeapon(
                            AttackerMock.Object,
                            DefenderMock.Object,
                            ac.Object,
                            bp.Object,
                            itemMock.Object
                            ), Times.Once());
                }
            }

            BuilderMock.Verify(
                x => x.AttackBodyPartWithWeapon(
                    It.IsAny<IAgent>(),
                    It.IsAny<IAgent>(),
                    It.IsAny<ICombatMoveClass>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<IItem>()
                    ), Times.Exactly(4));

            AttackerOutfitMock.Verify(x => x.GetWeaponItem(partMock.Object), Times.Once());
        }

    }
}
