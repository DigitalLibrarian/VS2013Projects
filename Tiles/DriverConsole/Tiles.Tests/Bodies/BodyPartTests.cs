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

        [TestInitialize]
        public void Initialize()
        {
            ClassMock = new Mock<IBodyPartClass>();
            TissueMock = new Mock<ITissue>();
            ParentMock = new Mock<IBodyPart>();
            Part = new BodyPart(ClassMock.Object, TissueMock.Object, ParentMock.Object);
        }

        [TestMethod]
        public void RootPartConstructor()
        {
            var part = new BodyPart(ClassMock.Object, TissueMock.Object);

            Assert.AreSame(ClassMock.Object, part.Class);
            Assert.AreSame(TissueMock.Object, part.Tissue);
            Assert.IsNull(part.Parent);
            Assert.IsNotNull(part.Health);
        }

        [TestMethod]
        public void Parent()
        {
            Assert.AreSame(ParentMock.Object, Part.Parent);
        }

        [TestMethod]
        public void Name_ClassProperty()
        {
            var name = "boo";
            Assert.IsNull(Part.Name);
            ClassMock.Setup(x => x.Name).Returns(name);
            Assert.AreSame(name, Part.Name);
        }

        [TestMethod]
        public void ArmorSlot_ClassProperty()
        {
            var slot = ArmorSlot.RightHand;
            Assert.AreNotEqual(slot, Part.ArmorSlot);
            ClassMock.Setup(x => x.ArmorSlot).Returns(slot);
            Assert.AreEqual(slot, Part.ArmorSlot);
        }

        [TestMethod]
        public void WeaponSlot_ClassProperty()
        {
            var slot = WeaponSlot.Main;
            Assert.AreNotEqual(slot, Part.WeaponSlot);
            ClassMock.Setup(x => x.WeaponSlot).Returns(slot);
            Assert.AreEqual(slot, Part.WeaponSlot);
        }


        [TestMethod]
        public void IsLifeCritical_ClassProperty()
        {
            Assert.IsFalse(Part.IsLifeCritical);
            ClassMock.Setup(x => x.IsLifeCritical).Returns(true);
            Assert.IsTrue(Part.IsLifeCritical);
        }

        [TestMethod]
        public void CanAmputate_ClassProperty()
        {
            Assert.IsFalse(Part.CanBeAmputated);
            ClassMock.Setup(x => x.CanBeAmputated).Returns(true);
            Assert.IsTrue(Part.CanBeAmputated);
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
