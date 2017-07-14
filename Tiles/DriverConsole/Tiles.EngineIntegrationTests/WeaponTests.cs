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

        [TestMethod]
        public void WoodShortSword_Stab_MaxPenetration()
        {
            var w = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            Assert.AreEqual(2000, w.Class.WeaponClass.AttackMoveClasses.Single(x => x.Name.Equals("stab")).MaxPenetration);
        }

        [TestMethod]
        public void WoodShortSword_Stab_ContactArea()
        {
            var w = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            Assert.AreEqual(50, w.Class.WeaponClass.AttackMoveClasses.Single(x => x.Name.Equals("stab")).ContactArea);
        }

        [TestMethod]
        public void WoodShortSword_Slash_MaxPenetration()
        {
            var w = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            Assert.AreEqual(4000, w.Class.WeaponClass.AttackMoveClasses.Single(x => x.Name.Equals("slash")).MaxPenetration);
        }

        [TestMethod]
        public void WoodShortSword_Slash_ContactArea()
        {
            var w = CreateMaterialTemplateWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "WOOD_TEMPLATE");
            Assert.AreEqual(20000, w.Class.WeaponClass.AttackMoveClasses.Single(x => x.Name.Equals("slash")).ContactArea);
        }
    }
}
