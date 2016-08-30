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
    public class EntityManagerTests
    {
        EntityManager Manager { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Manager = new EntityManager();
        }
        
        [TestMethod]
        public void CreateEntity()
        {
            int id = 1;
            var entity = Manager.GetEntity(id);
            Assert.IsNull(entity);

            var compIds = new int[0];

            Assert.IsFalse(Manager.GetEntities(compIds).Any());

            var result = Manager.CreateEntity(id);
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);

            entity = Manager.GetEntity(id);
            Assert.IsNotNull(entity);
            Assert.AreEqual(1, Manager.GetEntities(compIds).Count());

            Assert.AreEqual(id, entity.Id);
        }

        [TestMethod]
        public void GetEntities_ComponentRequirements()
        {
            var entity1 = Manager.CreateEntity(1);
            var entity2 = Manager.CreateEntity(2);
            var entity3 = Manager.CreateEntity(3);
            var entity4 = Manager.CreateEntity(4);

            var compMock1 = MockComponent(1);
            var compMock2 = MockComponent(2);
            var compMock3 = MockComponent(3);
            var compMock4 = MockComponent(4);
            int unknownCompId = 5;


            entity2.AddComponent(compMock2.Object);
            entity2.AddComponent(compMock3.Object);

            entity3.AddComponent(compMock3.Object);

            entity4.AddComponent(compMock4.Object);

            var result = Manager.GetEntities(new int[] { 
                compMock1.Object.Id
            });

            Assert.AreEqual(0, result.Count());

            result = Manager.GetEntities(new int[] { 
                compMock2.Object.Id
            });

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(entity2, result.ElementAt(0));

            result = Manager.GetEntities(new int[] { 
                compMock3.Object.Id
            });

            Assert.AreEqual(2, result.Count());
            Assert.AreSame(entity2, result.ElementAt(0));
            Assert.AreSame(entity3, result.ElementAt(1));

            result = Manager.GetEntities(new int[] { 
                compMock4.Object.Id
            });

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(entity4, result.ElementAt(0));

            result = Manager.GetEntities(new int[] { 
                unknownCompId
            });

            Assert.AreEqual(0, result.Count());

            result = Manager.GetEntities(new int[] { });

            Assert.AreEqual(4, result.Count());
            Assert.AreSame(entity1, result.ElementAt(0));
            Assert.AreSame(entity2, result.ElementAt(1));
            Assert.AreSame(entity3, result.ElementAt(2));
            Assert.AreSame(entity4, result.ElementAt(3));

        }

        [TestMethod]
        public void GetEntity()
        {
            int id = 1;

            Assert.IsNull(Manager.GetEntity(id));

            var entity = Manager.CreateEntity(id);

            Assert.AreSame(entity, Manager.GetEntity(id));
        }

        [Ignore]
        [TestMethod]
        public void DuplicateEntityId() 
        { 

        }

        Mock<IComponent> MockComponent(int id) 
        {
            var m = new Mock<IComponent>();
            m.Setup(x => x.Id).Returns(id);
            return m;
        }

    }
}
