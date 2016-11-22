using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tiles.Bodies;

namespace Tiles.Tests.Bodies
{
    [TestClass]
    public class HealthVectorTests
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            var hv = new HealthVector();

            Assert.IsFalse(hv.OutOfHealth);
            Assert.AreEqual(HealthVector.MaxHealth, (int)hv.Health);
        }

        [TestMethod]
        public void Constructor()
        {
            uint initialHealth = 50;
            var hv = new HealthVector(initialHealth);

            Assert.IsFalse(hv.OutOfHealth);
            Assert.AreEqual(initialHealth, hv.Health);

            initialHealth = 0;
            hv = new HealthVector(initialHealth);

            Assert.IsTrue(hv.OutOfHealth);
            Assert.AreEqual(initialHealth, hv.Health);

            initialHealth = 100;
            hv = new HealthVector(initialHealth);

            Assert.IsFalse(hv.OutOfHealth);
            Assert.AreEqual((uint)HealthVector.MaxHealth, hv.Health);
        }

        [TestMethod]
        public void Create()
        {
            var hv = HealthVector.Create();

            Assert.IsFalse(hv.OutOfHealth);
            Assert.AreEqual((uint)HealthVector.MaxHealth, hv.Health);
        }

        [TestMethod]
        public void TakeDamage()
        {
            var hv = new HealthVector(10);
            hv.TakeDamage(1);
            Assert.AreEqual((uint)9, hv.Health);

            hv = new HealthVector(1);
            hv.TakeDamage(2);
            Assert.AreEqual((uint)HealthVector.MinHealth, hv.Health);

            hv = new HealthVector(HealthVector.MaxHealth);
            hv.TakeDamage(0);
            Assert.AreEqual((uint)HealthVector.MaxHealth, hv.Health);

        }

        [TestMethod]
        public void TestToString()
        {
            var hv1 = new HealthVector(1);
            var hv2 = new HealthVector(2);
            var hv3 = new HealthVector(1);

            Assert.AreEqual(hv1.ToString(), hv3.ToString());
            Assert.AreNotEqual(hv1.ToString(), hv2.ToString());
            Assert.AreNotEqual(hv2.ToString(), hv3.ToString());
        }
    }
}
