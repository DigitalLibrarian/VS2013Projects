using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class WeaponTests : DfContentTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public void IronPick_MaxPenetration()
        {
            var w = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_PICK, "IRON");
            Assert.IsNotNull(w);

            Assert.AreEqual(4000, w.Class.WeaponClass.AttackMoveClasses.Single().MaxPenetration);
        }
    }
}
