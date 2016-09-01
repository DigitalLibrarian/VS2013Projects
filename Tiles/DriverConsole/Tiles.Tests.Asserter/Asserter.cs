using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Math;


namespace Tiles.Tests.Assertions
{
    static public class Asserter
    {
        static public void AreEqual(Vector2 expected, Vector2 actual)
        {
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Y, actual.Y);
        }

        static public void AreNotEqual(Vector2 expected, Vector2 actual)
        {
            Assert.AreNotEqual(expected.X, actual.X);
            Assert.AreNotEqual(expected.Y, actual.Y);
        }

        static public void AreEqual(Vector3 expected, Vector3 actual)
        {
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Y, actual.Y);
            Assert.AreEqual(expected.Z, actual.Z);
        }

        static public void AreNotEqual(Vector3 expected, Vector3 actual)
        {
            Assert.AreNotEqual(expected.X, actual.X);
            Assert.AreNotEqual(expected.Y, actual.Y);
            Assert.AreNotEqual(expected.Z, actual.Z);
        }
        static public void AssertException<TException>(Action action) 
            where TException : Exception
        {
            TException caughtException = null;
            try
            {
                action();
            }
            catch (TException e)
            {
                caughtException = e;
            }
            
            Assert.IsNotNull(caughtException);
        }

        static public void AssertException(Action action)
        {
            AssertException<Exception>(action);
        }
    }
}
