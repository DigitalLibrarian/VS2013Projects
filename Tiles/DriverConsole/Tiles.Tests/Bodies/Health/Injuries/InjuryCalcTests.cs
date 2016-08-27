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
using Tiles.Bodies.Health;
using Tiles.Bodies.Health.Injuries;
using Tiles.Items;
using Tiles.Items.Outfits;

namespace Tiles.Tests.Bodies.Health.Injuries
{
    [TestClass]
    public class InjuryCalcTests
    {
        Mock<IInjuryResultBuilderFactory> BuilderFactoryMock { get; set; }
        Mock<IDamageResistorFactory> ResistorFactoryMock { get; set; }
        InjuryCalc Calc { get; set; }

        Mock<IAgent> AttackerMock { get; set; }
        Mock<IAgent> DefenderMock { get; set; }
        List<Mock<IItem>> ArmorMocks { get; set; }
        Mock<IOutfit> DefenderOutfitMock { get; set; }

        Mock<IBodyPart> PartMock { get; set; }
        Mock<ITissue> PartTissueMock { get; set; }
        List<Mock<ITissueLayer>> TissueLayerMocks { get; set; }

        Mock<IItem> WeaponMock { get; set; }
        Mock<ICombatMoveClass> MoveClassMock { get; set; }
        Mock<IDamageVector> DamageMock { get; set; }

        Mock<IInjuryResultBuilder> BuilderMock { get; set; }

        IEnumerable<IInjury> BuilderResult { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            BuilderResult = new List<IInjury>();
            BuilderMock = new Mock<IInjuryResultBuilder>();
            BuilderMock.Setup(x => x.Build())
                .Returns(BuilderResult); 
            
            BuilderFactoryMock = new Mock<IInjuryResultBuilderFactory>();
            BuilderFactoryMock.Setup(x => x.Create())
                .Returns(BuilderMock.Object);

            ResistorFactoryMock = new Mock<IDamageResistorFactory>();

            Calc = new InjuryCalc(
                BuilderFactoryMock.Object,
                ResistorFactoryMock.Object);

            AttackerMock = new Mock<IAgent>();

            TissueLayerMocks = new List<Mock<ITissueLayer>>();
            PartTissueMock = new Mock<ITissue>();
            PartTissueMock.Setup(x => x.TissueLayers)
                .Returns(
                () => TissueLayerMocks.Select(x => x.Object).ToList());

            PartMock = new Mock<IBodyPart>();
            PartMock.Setup(x => x.Tissue)
                .Returns(PartTissueMock.Object);
            
            ArmorMocks = new List<Mock<IItem>>();
            DefenderOutfitMock = new Mock<IOutfit>();
            DefenderOutfitMock.Setup(x => x.GetItems(PartMock.Object))
                .Returns(() => ArmorMocks.Select(t => t.Object).ToList());

            DefenderMock = new Mock<IAgent>();
            DefenderMock.Setup(x => x.Outfit)
                .Returns(DefenderOutfitMock.Object);

            
            WeaponMock = new Mock<IItem>();
            DamageMock = new Mock<IDamageVector>();
            MoveClassMock = new Mock<ICombatMoveClass>();
            MoveClassMock.Setup(x => x.DamageVector)
                .Returns(DamageMock.Object);
        }

        Mock<IDamageResistor> Setup(Mock<IItem> armorMock)
        {
            ArmorMocks.Add(armorMock);
            var resistMock = new Mock<IDamageResistor>();
            ResistorFactoryMock.Setup(x => x.Create(armorMock.Object))
                .Returns(resistMock.Object);
            return resistMock;
        }

        Mock<IDamageResistor> Setup(Mock<ITissueLayer> layer)
        {
            TissueLayerMocks.Add(layer);
            var resistMock = new Mock<IDamageResistor>();
            ResistorFactoryMock.Setup(x => x.Create(layer.Object))
                .Returns(resistMock.Object);
            return resistMock;
        }


        [TestMethod]
        public void MeleeWeaponStrike() 
        {
            var armorMock1 = new Mock<IItem>();
            var armorMock2 = new Mock<IItem>();
            var armorResistMock1 = Setup(armorMock1);
            var armorResistMock2 = Setup(armorMock2);

            var tisMock1 = new Mock<ITissueLayer>();
            var tisMock2 = new Mock<ITissueLayer>();
            var tisResistMock1 = Setup(tisMock1);
            var tisResistMock2 = Setup(tisMock2);

            var result = Calc.MeleeWeaponStrike(
                MoveClassMock.Object,
                AttackerMock.Object,
                DefenderMock.Object,
                PartMock.Object,
                WeaponMock.Object);

            Assert.AreSame(result, BuilderResult);

            BuilderMock.Verify(x => x.SetTargetBodyPart(PartMock.Object), Times.Once());
            BuilderMock.Verify(x => x.AddDamage(DamageMock.Object), Times.Once());

            ResistorFactoryMock.Verify(x => x.Create(armorMock1.Object), Times.Once());
            ResistorFactoryMock.Verify(x => x.Create(armorMock2.Object), Times.Once());

            ResistorFactoryMock.Verify(x => x.Create(tisMock1.Object), Times.Once());
            ResistorFactoryMock.Verify(x => x.Create(tisMock2.Object), Times.Once());

            BuilderMock.Verify(x => x.AddArmorResistor(armorResistMock1.Object), Times.Once());
            BuilderMock.Verify(x => x.AddArmorResistor(armorResistMock2.Object), Times.Once());

            BuilderMock.Verify(x => x.AddTissueResistor(tisMock1.Object, tisResistMock1.Object), Times.Once());
            BuilderMock.Verify(x => x.AddTissueResistor(tisMock2.Object, tisResistMock2.Object), Times.Once());

            BuilderMock.Verify(x => x.Build(), Times.Once());
        }

        [Ignore]
        [TestMethod]
        public void UnarmedWeaponStrike() { }

    }
}
