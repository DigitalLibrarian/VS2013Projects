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
            if (partMock == null)
            {
                var partClassMock = new Mock<IBodyPartClass>();
                partMock = new Mock<IBodyPart>();
                partMock.Setup(x => x.Class).Returns(partClassMock.Object);
            }
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
        public void PossibleMeleeWeaponStrikeBodyPart()
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


        [Ignore]
        [TestMethod]
        public void PossibleGrasp()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));

            var defenderBodyPartMock1 = AddBodyPart(DefenderBodyParts);
            defenderBodyPartMock1.Setup(x => x.IsWrestling).Returns(true);
            var defenderBodyPartMock2 = AddBodyPart(DefenderBodyParts);
            defenderBodyPartMock2.Setup(x => x.IsWrestling).Returns(false);
            var defenderBodyPartMock3 = AddBodyPart(DefenderBodyParts);
            defenderBodyPartMock3.Setup(x => x.IsWrestling).Returns(false);

            var attackerBodyPartMock1 = AddBodyPart(AttackerBodyParts);
            attackerBodyPartMock1.Setup(x => x.IsWrestling).Returns(false);
            attackerBodyPartMock1.Setup(x => x.CanGrasp).Returns(false);

            var itemMock = MockWeaponItem();
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(attackerBodyPartMock1.Object)).Returns(itemMock.Object);

            var attackerBodyPartMock2 = AddBodyPart(AttackerBodyParts);
            attackerBodyPartMock2.Setup(x => x.IsWrestling).Returns(false);
            attackerBodyPartMock2.Setup(x => x.CanGrasp).Returns(true);
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(attackerBodyPartMock2.Object)).Returns((IItem)null);

            var attackerBodyPartMock3 = AddBodyPart(AttackerBodyParts);
            attackerBodyPartMock3.Setup(x => x.IsWrestling).Returns(false);
            attackerBodyPartMock3.Setup(x => x.CanGrasp).Returns(true);
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(attackerBodyPartMock3.Object)).Returns((IItem)null);

            var attackerBodyPartMock4 = AddBodyPart(AttackerBodyParts);
            attackerBodyPartMock4.Setup(x => x.IsWrestling).Returns(false);
            attackerBodyPartMock4.Setup(x => x.CanGrasp).Returns(false);
            AttackerOutfitMock.Setup(x => x.GetWeaponItem(attackerBodyPartMock3.Object)).Returns((IItem)null);

            var spoofedResult = new Mock<ICombatMove>();
            BuilderMock.Setup(
                x => x.GraspOpponentBodyPart(
                    It.IsAny<IAgent>(),
                    It.IsAny<IAgent>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<IBodyPart>()
                    )).Returns(spoofedResult.Object);

            var result = Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).ToList();
            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.All(x => x == spoofedResult.Object));

            foreach (var defenderPart in new[] { defenderBodyPartMock2.Object, defenderBodyPartMock3.Object })
            {
                foreach (var attackerPart in new[] { attackerBodyPartMock2.Object, attackerBodyPartMock3.Object})
                {
                    BuilderMock.Verify(x => x.GraspOpponentBodyPart(
                        AttackerMock.Object, DefenderMock.Object, attackerPart, defenderPart), Times.Once());
                }
            }
        }

        [Ignore]
        [TestMethod]
        public void PossibleBreakGrasp()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));

            DefenderBodyMock.Setup(x => x.IsWrestling).Returns(true);

            var attackerBodyPartMock = AddBodyPart(AttackerBodyParts);
            attackerBodyPartMock.Setup(x => x.IsWrestling).Returns(true);

            var defenderBodyPartMock = AddBodyPart(DefenderBodyParts);
            defenderBodyPartMock.Setup(x => x.IsWrestling).Returns(true);
            defenderBodyPartMock.Setup(x => x.IsGrasping).Returns(true);
            defenderBodyPartMock.Setup(x => x.Grasped).Returns(attackerBodyPartMock.Object);

            var spoofedResult = new Mock<ICombatMove>();
            BuilderMock.Setup(
                x => x.BreakOpponentGrasp(
                    It.IsAny<IAgent>(),
                    It.IsAny<IAgent>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<IBodyPart>()
                    )).Returns(spoofedResult.Object);

            var result = Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).ToList();
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(x => x == spoofedResult.Object));

            BuilderMock.Verify(x => x.BreakOpponentGrasp(It.IsAny<IAgent>(), It.IsAny<IAgent>(), attackerBodyPartMock.Object, defenderBodyPartMock.Object), Times.Once);
        }

        [Ignore]
        [TestMethod]
        public void PossiblePullAndReleaseGrasp()
        {
            AttackerMock.Setup(x => x.Pos).Returns(Vector3.Zero);
            DefenderMock.Setup(x => x.Pos).Returns(new Vector3(1, 1, 0));
            
            var attackerBodyPartMock = AddBodyPart(AttackerBodyParts);
            attackerBodyPartMock.Setup(x => x.IsWrestling).Returns(true);
            attackerBodyPartMock.Setup(x => x.IsGrasping).Returns(true);

            var defenderBodyPartMock = AddBodyPart(DefenderBodyParts);
            defenderBodyPartMock.Setup(x => x.IsWrestling).Returns(true);
            defenderBodyPartMock.Setup(x => x.IsGrasping).Returns(false);

            attackerBodyPartMock.Setup(x => x.Grasped).Returns(defenderBodyPartMock.Object);


            var spoofedResult1 = new Mock<ICombatMove>();
            BuilderMock.Setup(
                x => x.PullGraspedBodyPart(
                    It.IsAny<IAgent>(),
                    It.IsAny<IAgent>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<IBodyPart>()
                    )).Returns(spoofedResult1.Object);

            var spoofedResult2 = new Mock<ICombatMove>();
            BuilderMock.Setup(
                x => x.ReleaseGraspedPart(
                    It.IsAny<IAgent>(),
                    It.IsAny<IAgent>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<IBodyPart>()
                    )).Returns(spoofedResult2.Object);


            var result = Disco.GetPossibleMoves(AttackerMock.Object, DefenderMock.Object).ToList();
            Assert.AreEqual(2, result.Count());
            Assert.AreSame(spoofedResult1.Object, result.ElementAt(0));
            Assert.AreSame(spoofedResult2.Object, result.ElementAt(1));

            BuilderMock.Verify(x => x.PullGraspedBodyPart(AttackerMock.Object, DefenderMock.Object, attackerBodyPartMock.Object, defenderBodyPartMock.Object), Times.Once());
            BuilderMock.Verify(x => x.ReleaseGraspedPart(AttackerMock.Object, DefenderMock.Object, attackerBodyPartMock.Object, defenderBodyPartMock.Object), Times.Once());
        }
    }
}
