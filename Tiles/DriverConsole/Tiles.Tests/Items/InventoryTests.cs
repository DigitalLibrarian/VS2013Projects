using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tiles.Items;
using Tiles.Tests.Assertions;

namespace Tiles.Tests.Items
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void Invariants()
        {
            var inv = new Inventory();

            Assert.IsFalse(inv.GetItems().Any());
            Assert.IsFalse(inv.GetWorn().Any());
        }

        [TestMethod]
        public void AddItem()
        {
            var itemMock = new Mock<IItem>();
            var inv = new Inventory();

            inv.AddItem(itemMock.Object);

            Assert.AreEqual(1, inv.GetItems().Count());
            Assert.AreSame(itemMock.Object, inv.GetItems().Single());

            Assert.IsFalse(inv.GetWorn().Any());
        }

        [TestMethod]
        public void RemoveItem()
        {
            var itemMock1 = new Mock<IItem>();
            var inv = new Inventory();

            inv.RemoveItem(itemMock1.Object);
            inv.AddItem(itemMock1.Object);

            Assert.AreEqual(1, inv.GetItems().Count());
            Assert.AreSame(itemMock1.Object, inv.GetItems().Single());

            var itemMock2 = new Mock<IItem>();
            inv.RemoveItem(itemMock1.Object);
            inv.RemoveItem(itemMock2.Object);

            Assert.IsFalse(inv.GetItems().Any());
            Assert.IsFalse(inv.GetWorn().Any());
        }

        [TestMethod]
        public void AddToWorn()
        {
            var itemMock1 = new Mock<IItem>();
            var itemMock2 = new Mock<IItem>();
            var itemMock3 = new Mock<IItem>();

            var inv = new Inventory();

            inv.AddItem(itemMock1.Object);
            inv.AddItem(itemMock2.Object);
            inv.AddItem(itemMock3.Object);

            Assert.AreEqual(3, inv.GetItems().Count());
            Assert.AreEqual(0, inv.GetWorn().Count());

            object key1 = new object();
            object key2 = new object();
            object key3 = new object();
            object key4 = new object();

            Asserter.AssertException(() => inv.GetWorn(key1));
            Asserter.AssertException(() => inv.GetWorn(key2));
            Asserter.AssertException(() => inv.GetWorn(key3));
            Asserter.AssertException(() => inv.GetWorn(key4));

            inv.AddToWorn(key1, itemMock1.Object);
            Assert.AreEqual(itemMock1.Object, inv.GetWorn(key1));
            Assert.AreEqual(2, inv.GetItems().Count());
            Assert.AreEqual(1, inv.GetWorn().Count());

            inv.AddToWorn(key2, itemMock2.Object);
            Assert.AreEqual(itemMock2.Object, inv.GetWorn(key2));
            Assert.AreEqual(1, inv.GetItems().Count());
            Assert.AreEqual(2, inv.GetWorn().Count());

            inv.AddToWorn(key3, itemMock3.Object);
            Assert.AreEqual(itemMock3.Object, inv.GetWorn(key3));
            Assert.AreEqual(0, inv.GetItems().Count());
            Assert.AreEqual(3, inv.GetWorn().Count());

            Assert.AreSame(itemMock1.Object, inv.GetWorn().ElementAt(0));
            Assert.AreSame(itemMock2.Object, inv.GetWorn().ElementAt(1));
            Assert.AreSame(itemMock3.Object, inv.GetWorn().ElementAt(2));
            

            inv.RestoreFromWorn(key3);
            Assert.AreEqual(1, inv.GetItems().Count());
            Assert.AreEqual(2, inv.GetWorn().Count());

            inv.RestoreFromWorn(key2);
            Assert.AreEqual(2, inv.GetItems().Count());
            Assert.AreEqual(1, inv.GetWorn().Count());

            inv.RestoreFromWorn(key1);
            Assert.AreEqual(3, inv.GetItems().Count());
            Assert.AreEqual(0, inv.GetWorn().Count());
            
            Assert.AreSame(itemMock1.Object, inv.GetItems().ElementAt(2));
            Assert.AreSame(itemMock2.Object, inv.GetItems().ElementAt(1));
            Assert.AreSame(itemMock3.Object, inv.GetItems().ElementAt(0));

            Asserter.AssertException(() => inv.RestoreFromWorn(key4));
        }

       
    }
}
