using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
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
        Mock<IInjuryFactory> InjuryFactoryMock { get; set; }

        InjuryCalc InjuryCalc { get; set; }


        IMaterial Skin = new Material("skin", "skin")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

        };

        IMaterial Muscle = new Material("muscle", "muscle")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,
        };

        IMaterial Bone = new Material("bone", "bone")
        {
            ImpactYield = 200000,
            ImpactFracture = 200000,
            ImpactStrainAtYield = 100,
        };


        Mock<IAgent> DefenderMock { get; set; }
        Mock<IBodyPart> BodyPartMock_SingleSkinLayer { get; set; }

        Mock<IOutfit> OutfitMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryFactoryMock = new Mock<IInjuryFactory>();
            InjuryCalc = new InjuryCalc(InjuryFactoryMock.Object);

            InjuryFactoryMock.Setup(x => x.Create(It.IsAny<IInjuryClass>(), It.IsAny<IBodyPart>()))
                .Returns((IInjuryClass ic, IBodyPart bp) => {
                    var injuryMock = new Mock<IInjury>();
                    injuryMock.Setup(x => x.Class).Returns(ic);
                    injuryMock.Setup(x => x.BodyPart).Returns(bp);
                    return injuryMock.Object;
                });

            var tissueMock = new Mock<ITissue>();

            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.Tissue).Returns(tissueMock.Object);

            var skinThick = 4310;
            var skinLayerMock = new Mock<ITissueLayer>();
            skinLayerMock.Setup(x => x.Material).Returns(Skin);
            skinLayerMock.Setup(x => x.Thickness).Returns(skinThick);

            tissueMock.Setup(x => x.TissueLayers)
                .Returns(new[] { skinLayerMock.Object });

            var bodyMock = new Mock<IBody>();
            bodyMock.Setup(x => x.Parts)
                .Returns(new[] { partMock.Object });

            BodyPartMock_SingleSkinLayer = partMock;

            OutfitMock = new Mock<IOutfit>();
            OutfitMock.Setup(x => x.GetItems(BodyPartMock_SingleSkinLayer.Object))
                .Returns(new IItem[0]);

            DefenderMock = new Mock<IAgent>();
            DefenderMock.Setup(x => x.Body).Returns(bodyMock.Object);
            DefenderMock.Setup(x => x.Outfit).Returns(OutfitMock.Object);
            

        }


        [Ignore]
        [TestMethod]
        public void MeleeWeapon_ElasticEdgedCollsionTurnsToBlunt()
        {

        }

        [Ignore]
        [TestMethod]
        public void MeleeWeapon_SingleLayer_Edged_Elastic()
        {

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
        public void MeleeWeapon_SingleLayer_Blunt_Elastic()
        {
            int forcePerArea = 1;
            int contactArea = 10;

            var result = InjuryCalc.MaterialStrike(
                ContactType.Blunt,
                forcePerArea, contactArea,
                DefenderMock.Object, BodyPartMock_SingleSkinLayer.Object
                );

            Assert.AreEqual(1, result.Count());

            var injury = result.Single();

            Assert.AreEqual(StandardInjuryClasses.BruisedBodyPart, injury.Class);
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
    }
}
