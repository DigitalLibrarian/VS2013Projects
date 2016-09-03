using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tiles.Agents.Combat;

namespace Tiles.Tests.Agents.Combat
{
    [TestClass]
    public class DamageVectorTests
    {

        [TestMethod]
        public void AllDamageTypes()
        {
            var expected = Enum.GetValues(typeof(DamageType)).Cast<DamageType>();
            var actual = DamageVector.AllDamageTypes();

            Assert.AreEqual(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        [TestMethod]
        public void DefaultConstructor()
        {
            var v = new DamageVector();

            Assert.IsFalse(v.GetTypes().Any());

            foreach (var damageType in DamageVector.AllDamageTypes())
            {
                Assert.AreEqual(0, v.Get(damageType));
            }

            v.Set(DamageType.Slash, 25);
            Assert.AreEqual(25, v.Get(DamageType.Slash));
        }

        [TestMethod]
        public void ValuedConstructor()
        {
            var v = new DamageVector(new Dictionary<DamageType, int>
            {
                {DamageType.Blunt, 1},
                {DamageType.Pierce, 2}
            });

            Assert.IsTrue(v.GetTypes().Contains(DamageType.Blunt));
            Assert.IsTrue(v.GetTypes().Contains(DamageType.Pierce));
            Assert.IsFalse(v.GetTypes().Contains(DamageType.Slash));

            Assert.AreEqual(1, v.Get(DamageType.Blunt));
            Assert.AreEqual(2, v.Get(DamageType.Pierce));
            Assert.AreEqual(0, v.Get(DamageType.Slash));

            v.Set(DamageType.Blunt, 11);
            Assert.AreEqual(11, v.Get(DamageType.Blunt));
            v.Set(DamageType.Slash, 42);
            Assert.AreEqual(42, v.Get(DamageType.Slash));
        }

        [TestMethod]
        public void ToString_Equivalence()
        {
            var v1 = new DamageVector();
            var v2 = new DamageVector();
            Assert.AreEqual(v1.ToString(), v2.ToString());

            v1 = new DamageVector(new Dictionary<DamageType, int>
            {
                {DamageType.Blunt, 1},
                {DamageType.Pierce, 1},
                {DamageType.Slash, 1}
            });
            v2 = new DamageVector(new Dictionary<DamageType, int>
            {
                {DamageType.Blunt, 2},
                {DamageType.Pierce, 2},
                {DamageType.Slash, 2}
            });

            Assert.AreNotEqual(v1.ToString(), v2.ToString());
        }
    }
}
