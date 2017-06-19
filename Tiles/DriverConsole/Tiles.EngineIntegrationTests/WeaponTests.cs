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

            var mat = w.Class.Material;
            Assert.AreEqual(542500, mat.ImpactYield);
            Assert.AreEqual(1085000, mat.ImpactFracture);
            Assert.AreEqual(319, mat.ImpactStrainAtYield);

            Assert.AreEqual(542500, mat.CompressiveYield);
            Assert.AreEqual(1085000, mat.CompressiveFracture);
            Assert.AreEqual(319, mat.CompressiveStrainAtYield);

            Assert.AreEqual(155000, mat.TensileYield);
            Assert.AreEqual(310000, mat.TensileFracture);
            Assert.AreEqual(73, mat.TensileStrainAtYield);

            Assert.AreEqual(155000, mat.TorsionYield);
            Assert.AreEqual(310000, mat.TorsionFracture);
            Assert.AreEqual(189, mat.TorsionStrainAtYield);

            Assert.AreEqual(155000, mat.ShearYield);
            Assert.AreEqual(310000, mat.ShearFracture);
            Assert.AreEqual(189, mat.ShearStrainAtYield);

            Assert.AreEqual(155000, mat.BendingYield);
            Assert.AreEqual(310000, mat.BendingFracture);
            Assert.AreEqual(73, mat.BendingStrainAtYield);
        }

    }
}
