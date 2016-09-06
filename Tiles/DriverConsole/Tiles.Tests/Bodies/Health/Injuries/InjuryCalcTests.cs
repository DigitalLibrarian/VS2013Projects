using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Health.Injuries;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Materials;

namespace Tiles.Tests.Bodies.Health.Injuries
{
    [TestClass]
    public class InjuryCalcTests
    {
        //InjuryCalc InjuryCalc { get; set; }
        MostBestInjuryCalc InjuryCalc { get; set; }

        int ShortSwordSize = 300;
        int ShortSwordContactArea_Slash = 20000;

        int MaceSize = 800;
        int MaceContactArea_Bash = 20;

        double Shear_SlowVelo = 1;
        double Shear_ModerateVelo = 28;
        double Shear_FastVelo = 100;

        double Impact_SlowVelo = 1;
        double Impact_ModerateVelo = 1.2d;
        double Impact_FastVelo = 100;

        int BoneLayerThickness = 26315;
        
        Mock<IAgent> AttackerMock { get; set; }
        Mock<IAgent> DefenderMock { get; set; }
        Mock<IBodyPart> BodyPartMock_SingleBoneLayer { get; set; }
        Mock<IBodyPart> BodyPartMock_SingleSteelLayer { get; set; }

        Mock<IOutfit> OutfitMock { get; set; }
        Mock<IItem> WeaponItemMock { get; set; }
        Mock<IItemClass> WeaponItemClassMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            //InjuryCalc = new InjuryCalc(new InjuryFactory());
            InjuryCalc = new MostBestInjuryCalc(
                new InjuryFactory(),
                new LayeredMaterialStrikeResultBuilder(
                    new MaterialStrikeResultBuilder()));

            var boneLayerMock = MockTissueLayer(BoneLayerThickness, TestMaterials.Bone);
            BodyPartMock_SingleBoneLayer = MockBodyPart(boneLayerMock.Object);

            var steelLayerMock = MockTissueLayer(BoneLayerThickness, TestMaterials.Steel);
            BodyPartMock_SingleSteelLayer = MockBodyPart(steelLayerMock.Object);

            var bodyMock = new Mock<IBody>();
            bodyMock.Setup(x => x.Parts)
                .Returns(new[] { 
                    BodyPartMock_SingleBoneLayer.Object,
                    BodyPartMock_SingleSteelLayer.Object
                });
            
            OutfitMock = new Mock<IOutfit>();
            OutfitMock.Setup(x => x.GetItems(BodyPartMock_SingleBoneLayer.Object))
                .Returns(new IItem[0]);

            OutfitMock.Setup(x => x.GetItems(BodyPartMock_SingleSteelLayer.Object))
                .Returns(new IItem[0]);

            AttackerMock = new Mock<IAgent>();
            DefenderMock = new Mock<IAgent>();
            DefenderMock.Setup(x => x.Body).Returns(bodyMock.Object);
            DefenderMock.Setup(x => x.Outfit).Returns(OutfitMock.Object);

            WeaponItemClassMock = new Mock<IItemClass>();

            WeaponItemMock = new Mock<IItem>();
            WeaponItemMock.Setup(x => x.GetMass()).Returns((() =>
                WeaponItemMock.Object.Class.Material.GetMassForUniformVolume(
                    WeaponItemMock.Object.Class.Size)
            ));
            WeaponItemMock.Setup(x => x.Class).Returns(WeaponItemClassMock.Object);
        }



        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_NoInjury()
        {
            var weaponMat = TestMaterials.Steel;
            SetupWeapon(ShortSwordSize, weaponMat);
            
            int contactArea = ShortSwordContactArea_Slash;
            var cmcMock = MockCombat(StressMode.Edge, contactArea);

            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                Shear_SlowVelo,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleBoneLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(0, result.Count());
        }


        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_Cut()
        {
            var weaponMat = TestMaterials.Steel;
            SetupWeapon(ShortSwordSize, weaponMat);

            int contactArea = ShortSwordContactArea_Slash;
            var cmcMock = MockCombat(StressMode.Edge, contactArea);

            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                Shear_ModerateVelo,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleBoneLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(1, result.Count());

            var injury = result.ElementAt(0);
            AssertInjuryClass(StandardInjuryClasses.CutBodyPart, injury.Class);
        }

        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_BadlyGashed()
        {
            var weaponMat = TestMaterials.Steel;
            SetupWeapon(ShortSwordSize, weaponMat);

            int contactArea = ShortSwordContactArea_Slash;
            var cmcMock = MockCombat(StressMode.Edge, contactArea);

            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                Shear_FastVelo,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleBoneLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(1, result.Count());

            var injury = result.ElementAt(0);
            AssertInjuryClass(StandardInjuryClasses.BadlyGashedBodyPart, injury.Class);
        }

        [TestMethod]
        public void MeleeWeapon_SingleLayer_Blunt_NoInjury()
        {
            var weaponMat = TestMaterials.Feather;
            SetupWeapon(MaceSize, weaponMat);

            int contactArea = MaceContactArea_Bash;
            var cmcMock = MockCombat(StressMode.Blunt, contactArea);
            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                Impact_SlowVelo,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleBoneLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void MeleeWeapon_SingleLayer_Blunt_Bruised()
        {
            var weaponMat = TestMaterials.Silver;
            SetupWeapon(MaceSize, weaponMat);
            
            int contactArea = MaceContactArea_Bash;
            var cmcMock = MockCombat(StressMode.Blunt, contactArea);
            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                Impact_ModerateVelo,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleBoneLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(1, result.Count());

            var injury = result.Single();

            AssertInjuryClass(StandardInjuryClasses.BruisedBodyPart, injury.Class);
        }


        [TestMethod]
        public void MeleeWeapon_SingleLayer_Blunt_Battered()
        {
            var weaponMat = TestMaterials.Silver;
            SetupWeapon(MaceSize, weaponMat);

            int contactArea = MaceContactArea_Bash;
            var cmcMock = MockCombat(StressMode.Blunt, contactArea);

            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                Impact_FastVelo,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleBoneLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(1, result.Count());

            var injury = result.ElementAt(0);
            AssertInjuryClass(StandardInjuryClasses.BatteredBodyPart, injury.Class);
        }


        [Ignore]
        [TestMethod]
        public void MeleeWeapon_BluntPassesThroughElasticLayersToCauseInternalFracture()
        {

        }

        [TestMethod]
        [Ignore]
        public void CumulativeInjury()
        {

        }
        
        [Ignore]
        [TestMethod]
        public void MeleeWeapon_BluntPassesThroughElasticLayersToCausePlasticDeform()
        {

        }

        #region Helpers
        Mock<ICombatMoveClass> MockCombat(StressMode contactType, int contactArea)
        {
            var m = new Mock<ICombatMoveClass>();
            m.Setup(x => x.StressMode).Returns(contactType);
            m.Setup(x => x.ContactArea).Returns(contactArea);
            return m;
        }

        Mock<ITissueLayer> MockTissueLayer(int thick, IMaterial mat)
        {
            var m = new Mock<ITissueLayer>();
            m.Setup(x => x.Material).Returns(mat);
            m.Setup(x => x.Thickness).Returns(thick);
            return m;
        }

        Mock<IBodyPart> MockBodyPart(params ITissueLayer[] layers)
        {
            var tissueMock = new Mock<ITissue>();
            tissueMock.Setup(x => x.TissueLayers).Returns(layers.ToArray());
            tissueMock.Setup(x => x.TotalThickness).Returns(() =>
            {
                return tissueMock.Object.TissueLayers
                    .Select(x => x.Thickness)
                    .Sum();
            });

            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.Tissue).Returns(tissueMock.Object);

            var damage = new DamageVector();
            partMock.Setup(x => x.Damage).Returns(damage);
            return partMock;
        }

        void SetupWeapon(int size, IMaterial mat)
        {
            WeaponItemClassMock.Setup(x => x.Size).Returns(size);
            WeaponItemClassMock.Setup(x => x.Material).Returns(mat);
        }
        void AssertInjuryClass(IInjuryClass expected, IInjuryClass actual)
        {
            Assert.AreSame(expected, actual, string.Format("Expected injury class '{0}', actual '{1}'", expected.Adjective, actual.Adjective));
        }
        #endregion
    }
}
