using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Tests.Bodies
{
    [TestClass]
    public class BodyPartTests
    {
        Mock<IBodyPartClass> ClassMock { get; set; }
        Mock<IBodyPart> ParentMock { get; set; }
        Mock<ITissue> TissueMock { get; set; }

        BodyPart Part { get; set; }

        int Size = 9;

        [TestInitialize]
        public void Initialize()
        {
            ClassMock = new Mock<IBodyPartClass>();
            TissueMock = new Mock<ITissue>();
            ParentMock = new Mock<IBodyPart>();
            Part = new BodyPart(ClassMock.Object, TissueMock.Object, Size, ParentMock.Object);
        }

        [TestMethod]
        public void RootPartConstructor()
        {
            var part = new BodyPart(ClassMock.Object, TissueMock.Object, Size);

            Assert.AreSame(ClassMock.Object, part.Class);
            Assert.AreSame(TissueMock.Object, part.Tissue);
            Assert.AreEqual(Size, part.Size);
            Assert.IsNull(part.Parent);
        }

        [TestMethod]
        public void Parent()
        {
            Assert.AreSame(ParentMock.Object, Part.Parent);
        }

        [TestMethod]
        public void ClassProperty_Name()
        {
            var name = "boo";
            Assert.IsNull(Part.Name);
            ClassMock.Setup(x => x.Name).Returns(name);
            Assert.AreSame(name, Part.Name);
        }

        [TestMethod]
        public void ClassProperty_ArmorSlot()
        {
            var slot = ArmorSlot.RightHand;
            Assert.AreNotEqual(slot, Part.ArmorSlot);
            ClassMock.Setup(x => x.ArmorSlot).Returns(slot);
            Assert.AreEqual(slot, Part.ArmorSlot);
        }

        [TestMethod]
        public void ClassProperty_WeaponSlot()
        {
            var slot = WeaponSlot.Main;
            Assert.AreNotEqual(slot, Part.WeaponSlot);
            ClassMock.Setup(x => x.WeaponSlot).Returns(slot);
            Assert.AreEqual(slot, Part.WeaponSlot);
        }


        [TestMethod]
        public void ClassProperty_IsLifeCritical()
        {
            Assert.IsFalse(Part.IsLifeCritical);
            ClassMock.Setup(x => x.IsLifeCritical).Returns(true);
            Assert.IsTrue(Part.IsLifeCritical);
        }

        [TestMethod]
        public void ClassProperty_CanAmputate()
        {
            Assert.IsFalse(Part.CanBeAmputated);
            ClassMock.Setup(x => x.CanBeAmputated).Returns(true);
            Assert.IsTrue(Part.CanBeAmputated);
        }

        [TestMethod]
        public void ClassProperty_IsNervous()
        {
            Assert.IsFalse(Part.IsNervous);
            ClassMock.Setup(x => x.IsNervous).Returns(true);
            Assert.IsTrue(Part.IsNervous);
        }

        [TestMethod]
        public void ClassProperty_IsCirculatory()
        {
            Assert.IsFalse(Part.IsCirculatory);
            ClassMock.Setup(x => x.IsCirculatory).Returns(true);
            Assert.IsTrue(Part.IsCirculatory);
        }

        [TestMethod]
        public void ClassProperty_IsSkeletal()
        {
            Assert.IsFalse(Part.IsSkeletal);
            ClassMock.Setup(x => x.IsSkeletal).Returns(true);
            Assert.IsTrue(Part.IsSkeletal);
        }

        [TestMethod]
        public void ClassProperty_IsDigit()
        {
            Assert.IsFalse(Part.IsDigit);
            ClassMock.Setup(x => x.IsDigit).Returns(true);
            Assert.IsTrue(Part.IsDigit);
        }

        [TestMethod]
        public void ClassProperty_IsBreathe()
        {
            Assert.IsFalse(Part.IsBreathe);
            ClassMock.Setup(x => x.IsBreathe).Returns(true);
            Assert.IsTrue(Part.IsBreathe);
        }

        [TestMethod]
        public void ClassProperty_IsSight()
        {
            Assert.IsFalse(Part.IsSight);
            ClassMock.Setup(x => x.IsSight).Returns(true);
            Assert.IsTrue(Part.IsSight);
        }

        [TestMethod]
        public void ClassProperty_IsStance()
        {
            Assert.IsFalse(Part.IsStance);
            ClassMock.Setup(x => x.IsStance).Returns(true);
            Assert.IsTrue(Part.IsStance);
        }

        [TestMethod]
        public void ClassProperty_IsInternal()
        {
            Assert.IsFalse(Part.IsInternal);
            ClassMock.Setup(x => x.IsInternal).Returns(true);
            Assert.IsTrue(Part.IsInternal);
        }

        [TestMethod]
        public void CanGrasp()
        {
            Assert.IsFalse(Part.CanGrasp);

            ClassMock.Setup(x => x.CanGrasp).Returns(true);
            Assert.IsTrue(Part.CanGrasp);

            var graspedPartMock = new Mock<IBodyPart>();
            Part.StartGrasp(graspedPartMock.Object);
            Assert.IsFalse(Part.CanGrasp);

            Part.StopGrasp(graspedPartMock.Object);
            Assert.IsTrue(Part.CanGrasp);

            var weaponItemMock = new Mock<IItem>();
            Part.Weapon = weaponItemMock.Object;
            Assert.IsFalse(Part.CanGrasp);
        }

        [TestMethod]
        public void IsGrasping()
        {
            Assert.IsFalse(Part.IsGrasping);

            var graspedPartMock = new Mock<IBodyPart>();

            Part.StartGrasp(graspedPartMock.Object);
            Assert.IsTrue(Part.IsGrasping);

            Part.StopGrasp(graspedPartMock.Object);
            Assert.IsFalse(Part.IsGrasping);
        }

        [TestMethod]
        public void IsWrestling()
        {
            Assert.IsFalse(Part.IsWrestling);

            var graspedPartMock = new Mock<IBodyPart>();

            Part.StartGrasp(graspedPartMock.Object);
            Assert.IsTrue(Part.IsWrestling);
            Part.StopGrasp(graspedPartMock.Object);
            Assert.IsFalse(Part.IsWrestling);

            Part.GraspedBy = new Mock<IBodyPart>().Object;
            Assert.IsTrue(Part.IsWrestling);
            Part.GraspedBy = null;
            Assert.IsFalse(Part.IsWrestling);
        }

        [TestMethod]
        public void IsBeingGrasped()
        {
            Assert.IsFalse(Part.IsBeingGrasped);

            Part.GraspedBy = new Mock<IBodyPart>().Object;

            Assert.IsTrue(Part.IsBeingGrasped);

            Part.GraspedBy = null;

            Assert.IsFalse(Part.IsBeingGrasped);
        }

        [TestMethod]
        public void StartGrasp()
        {
            var partMock = new Mock<IBodyPart>();
            Part.StartGrasp(partMock.Object);

            Assert.AreSame(Part.Grasped, partMock.Object);
            partMock.VerifySet(x => x.GraspedBy = Part, Times.Once());
        }

        [TestMethod]
        public void StopGrasp()
        {
            var partMock = new Mock<IBodyPart>();
            Part.StopGrasp(partMock.Object);
            Assert.IsNull(Part.Grasped);

            partMock.VerifySet(x => x.GraspedBy = null, Times.Once());
        }
    }
}
