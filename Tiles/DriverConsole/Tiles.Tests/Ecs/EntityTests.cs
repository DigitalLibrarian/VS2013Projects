using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;

namespace Tiles.Tests.Ecs
{
    [TestClass]
    public class EntityTests
    {
        public interface ISomeComponent : IComponent { }

        int EntityId = 0;
        Entity Entity { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Entity = new Entity(EntityId);
        }

        [TestMethod]
        public void Invariants()
        {
            Assert.AreEqual(EntityId, Entity.Id);
        }

        [TestMethod]
        public void GetComponent_QueryType()
        {
            var compMock = new Mock<ISomeComponent>();
            compMock.Setup(x => x.Id).Returns(1);


            var result = Entity.GetComponent<ISomeComponent>(1);
            Assert.IsNull(result);

            Entity.AddComponent(compMock.Object);

            result = Entity.GetComponent<ISomeComponent>(1);
            Assert.AreSame(compMock.Object, result);
        }

        [TestMethod]
        public void HasComponent()
        {
            var compMock = new Mock<IComponent>();
            compMock.Setup(x => x.Id).Returns(1);

            Assert.IsFalse(Entity.HasComponent(compMock.Object.Id));
            Entity.AddComponent(compMock.Object);
            Assert.IsTrue(Entity.HasComponent(compMock.Object.Id));
        }
    }
}
