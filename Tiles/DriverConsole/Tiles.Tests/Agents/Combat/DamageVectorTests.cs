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

            Assert.IsFalse(v.GetComponentTypes().Any());

            foreach (var damageType in DamageVector.AllDamageTypes())
            {
                Assert.AreEqual(0U, v.GetComponent(damageType));
            }

            v.SetComponent(DamageType.Slash, 25U);
            Assert.AreEqual(25U, v.GetComponent(DamageType.Slash));
        }

        [TestMethod]
        public void ValuedConstructor()
        {
            var v = new DamageVector(new Dictionary<DamageType, uint>
            {
                {DamageType.Blunt, 1U},
                {DamageType.Pierce, 2U}
            });

            Assert.IsTrue(v.GetComponentTypes().Contains(DamageType.Blunt));
            Assert.IsTrue(v.GetComponentTypes().Contains(DamageType.Pierce));
            Assert.IsFalse(v.GetComponentTypes().Contains(DamageType.Slash));

            Assert.AreEqual(1U, v.GetComponent(DamageType.Blunt));
            Assert.AreEqual(2U, v.GetComponent(DamageType.Pierce));
            Assert.AreEqual(0U, v.GetComponent(DamageType.Slash));

            v.SetComponent(DamageType.Blunt, 11U);
            Assert.AreEqual(11U, v.GetComponent(DamageType.Blunt));
            v.SetComponent(DamageType.Slash, 42U);
            Assert.AreEqual(42U, v.GetComponent(DamageType.Slash));
        }

        [TestMethod]
        public void ToString_Equivalence()
        {
            var v1 = new DamageVector();
            var v2 = new DamageVector();
            Assert.AreEqual(v1.ToString(), v2.ToString());

            v1 = new DamageVector(new Dictionary<DamageType, uint>
            {
                {DamageType.Blunt, 1U},
                {DamageType.Pierce, 1U},
                {DamageType.Slash, 1U}
            });
            v2 = new DamageVector(new Dictionary<DamageType, uint>
            {
                {DamageType.Blunt, 2U},
                {DamageType.Pierce, 2U},
                {DamageType.Slash, 2U}
            });

            Assert.AreNotEqual(v1.ToString(), v2.ToString());
        }
    }
}
