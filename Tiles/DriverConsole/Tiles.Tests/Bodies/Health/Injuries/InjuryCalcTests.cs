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
        InjuryCalc InjuryCalc { get; set; }

        int ShortSwordSize = 300;
        int MaceSize = 800;
        int SkinLayerThickness = 4310;

        IMaterial Brittle = new Material("brittle", "brittle")
        {
            ImpactYield = 1,
            ImpactFracture = 1,
            ImpactStrainAtYield = 5,

            ShearYield = 1,
            ShearFracture = 2,
            ShearStrainAtYield = 5,

            SolidDensity = 1
        };

        IMaterial Skin = new Material("skin", "skin")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

            ShearYield = 20000,
            ShearFracture = 20000,
            ShearStrainAtYield = 50000,

            SolidDensity = 1000
        };

        IMaterial Muscle = new Material("muscle", "muscle")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

            ShearYield = 20000,
            ShearFracture = 20000,
            ShearStrainAtYield = 50000,

            SolidDensity = 1060
        };

        IMaterial Bone = new Material("bone", "bone")
        {
            ImpactYield = 200000,
            ImpactFracture = 200000,
            ImpactStrainAtYield = 100,

            ShearYield = 115000,
            ShearFracture = 130000,
            ShearStrainAtYield = 100,

            SolidDensity = 500
        };

        IMaterial Steel = new Material("steel", "steel")
        {
            ImpactYield = 1505000,
            ImpactFracture = 2520000,
            ImpactStrainAtYield = 940,

            ShearYield = 430000,
            ShearFracture = 720000,
            ShearStrainAtYield = 215,

            SolidDensity = 7850
        };

        Mock<IAgent> AttackerMock { get; set; }
        Mock<IAgent> DefenderMock { get; set; }
        Mock<IBodyPart> BodyPartMock_SingleSkinLayer { get; set; }

        Mock<IOutfit> OutfitMock { get; set; }
        Mock<IItem> WeaponItemMock { get; set; }
        Mock<IItemClass> WeaponItemClassMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryCalc = new InjuryCalc(new InjuryFactory());

            var tissueMock = new Mock<ITissue>();

            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.Tissue).Returns(tissueMock.Object);

            var skinLayerMock = new Mock<ITissueLayer>();
            skinLayerMock.Setup(x => x.Material).Returns(Skin);
            skinLayerMock.Setup(x => x.Thickness).Returns(SkinLayerThickness);

            tissueMock.Setup(x => x.TissueLayers)
                .Returns(new[] { skinLayerMock.Object });

            var bodyMock = new Mock<IBody>();
            bodyMock.Setup(x => x.Parts)
                .Returns(new[] { partMock.Object });

            BodyPartMock_SingleSkinLayer = partMock;

            OutfitMock = new Mock<IOutfit>();
            OutfitMock.Setup(x => x.GetItems(BodyPartMock_SingleSkinLayer.Object))
                .Returns(new IItem[0]);

            AttackerMock = new Mock<IAgent>();
            DefenderMock = new Mock<IAgent>();
            DefenderMock.Setup(x => x.Body).Returns(bodyMock.Object);
            DefenderMock.Setup(x => x.Outfit).Returns(OutfitMock.Object);

            WeaponItemClassMock = new Mock<IItemClass>();

            WeaponItemMock = new Mock<IItem>();
            WeaponItemMock.Setup(x => x.Class).Returns(WeaponItemClassMock.Object);
        }


        [Ignore]
        [TestMethod]
        public void MeleeWeapon_ElasticEdgedCollsionTurnsToBlunt()
        {

        }

        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_Brittle_Weak()
        {
            int contactArea = 10;
            int force = 1;
            SetupWeapon(ShortSwordSize, Brittle);
            var cmcMock = MockCombat(ContactType.Edge, contactArea);

            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                force,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleSkinLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(1, result.Count());

            var injury = result.Single();

            AssertInjuryClass(StandardInjuryClasses.BruisedBodyPart, injury.Class);
        }

        [Ignore]
        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_Plastic()
        {

        }

        [Ignore]
        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_Fracture()
        {

        }

        [TestMethod]
        public void MeleeWeapon_SingleLayer_Blunt_Brittle_Weak()
        {
            int contactArea = 10;
            int force = 1;
            SetupWeapon(MaceSize, Brittle);
            var cmcMock = MockCombat(ContactType.Blunt, contactArea);

            var result = InjuryCalc.MeleeWeaponStrike(
                cmcMock.Object,
                force,
                AttackerMock.Object,
                DefenderMock.Object,
                BodyPartMock_SingleSkinLayer.Object,
                WeaponItemMock.Object);

            Assert.AreEqual(1, result.Count());

            var injury = result.Single();

            AssertInjuryClass(StandardInjuryClasses.BruisedBodyPart, injury.Class);
        }

        [Ignore]
        [TestMethod]
        public void MeleeWeapon_SingleLayer_Blunt_Plastic()
        {

        }

        [Ignore]
        [TestMethod]
        public void MeleeWeapon_SingleLayer_Blunt_Fracture()
        {

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

        Mock<ICombatMoveClass> MockCombat(ContactType contactType, int contactArea)
        {
            var m = new Mock<ICombatMoveClass>();
            m.Setup(x => x.ContactType).Returns(contactType);
            m.Setup(x => x.ContactArea).Returns(contactArea);
            return m;
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
    }
}
