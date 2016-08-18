using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
namespace Tiles.Tests
{
    [TestClass]
    public class ConjugatedWordTests
    {
        class TestWord : ConjugatedWord
        {
            public TestWord(IDictionary<VerbConjugation, string> conjugations) : base(conjugations) { }
        }

        [TestMethod]
        public void Conjugate_Happy()
        {
            var w = new TestWord(new Dictionary<VerbConjugation, string>
            {
                {VerbConjugation.FirstPerson, "i"},
                {VerbConjugation.SecondPerson, "me"},
            });

            Assert.AreEqual("i", w.Conjugate(VerbConjugation.FirstPerson));
            Assert.AreEqual("me", w.Conjugate(VerbConjugation.SecondPerson));
        }

        [TestMethod]
        public void Conjugate_MissingConjugation()
        {
            var w = new TestWord(new Dictionary<VerbConjugation, string>
            {
                {VerbConjugation.FirstPerson, "i"},
                {VerbConjugation.SecondPerson, "me"},
            });

            MissingVerbConjugationException caughtException = null;

            try
            {
                w.Conjugate(VerbConjugation.ThirdPerson);
            }
            catch (MissingVerbConjugationException e)
            {
                caughtException = e;
            }
            finally
            {
                Assert.IsNotNull(caughtException);
            }
        }
    }
}
