using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Health.Injuries;
using Tiles.Items;

namespace Tiles.Tests.Bodies.Health.Injuries
{
    [TestClass]
    public class InjuryResultBuilderTests
    {
        Mock<IDamageVector> DamageMock { get; set; }
        Mock<IInjuryFactory> InjuryFactoryMock { get; set; }

        InjuryResultBuilder Builder { get; set; }

        Mock<IDamageResistor> ArmorResistMock1 { get; set; }
        Mock<IDamageResistor> ArmorResistMock2 { get; set; }
        Mock<IDamageResistor> ArmorResistMock3 { get; set; }

        Mock<IBodyPart> PartMock { get; set; }

        Mock<ITissueLayer> Tissue1Mock { get; set; }
        Mock<IDamageResistor> TissueResistMock1 { get; set; }

        Mock<ITissueLayer> Tissue2Mock { get; set; }
        Mock<IDamageResistor> TissueResistMock2 { get; set; }

        Mock<ITissueLayer> Tissue3Mock { get; set; }
        Mock<IDamageResistor> TissueResistMock3 { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryFactoryMock = new Mock<IInjuryFactory>();
            InjuryFactoryMock.Setup(
                x => x.Create(
                    It.IsAny<IInjuryClass>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<ITissueLayer>()))
                .Returns((IInjuryClass ic, IBodyPart bp, ITissueLayer tl) =>
                {
                    var injuryMock = new Mock<IInjury>();
                    injuryMock.Setup(x => x.Class).Returns(ic);
                    return injuryMock.Object;
                });
            InjuryFactoryMock.Setup(
                x => x.Create(
                    It.IsAny<IInjuryClass>(),
                    It.IsAny<IBodyPart>()))
                .Returns((IInjuryClass ic, IBodyPart bp) =>
                {
                    var injuryMock = new Mock<IInjury>();
                    injuryMock.Setup(x => x.Class).Returns(ic);
                    return injuryMock.Object;
                });
            DamageMock = new Mock<IDamageVector>();

            Builder = new InjuryResultBuilder(InjuryFactoryMock.Object, DamageMock.Object);

            PartMock = new Mock<IBodyPart>();

            ArmorResistMock1 = new Mock<IDamageResistor>();
            ArmorResistMock2 = new Mock<IDamageResistor>();
            ArmorResistMock3 = new Mock<IDamageResistor>();

            Tissue1Mock = new Mock<ITissueLayer>();
            TissueResistMock1 = new Mock<IDamageResistor>();

            Tissue2Mock = new Mock<ITissueLayer>();
            TissueResistMock2 = new Mock<IDamageResistor>();

            Tissue3Mock = new Mock<ITissueLayer>();
            TissueResistMock3 = new Mock<IDamageResistor>();
        }

        void SetupDamageVector(Dictionary<DamageType, uint> damages)
        {
            DamageMock.Setup(x => x.GetComponentTypes())
                .Returns(damages.Keys);

            foreach (var key in damages.Keys)
            {
                DamageMock.Setup(x => x.GetComponent(key))
                    .Returns(damages[key]);
            }
        }

        void AddAllResistors()
        {
            Builder.SetTargetBodyPart(PartMock.Object);

            Builder.AddArmorResistor(ArmorResistMock1.Object);
            Builder.AddArmorResistor(ArmorResistMock2.Object);
            Builder.AddArmorResistor(ArmorResistMock3.Object);

            Builder.AddTissueResistor(Tissue1Mock.Object, TissueResistMock1.Object);
            Builder.AddTissueResistor(Tissue2Mock.Object, TissueResistMock2.Object);
            Builder.AddTissueResistor(Tissue3Mock.Object, TissueResistMock3.Object);
        }

        void AssertTotalTissueInjuriesMade(int total)
        {
            InjuryFactoryMock.Verify(
                x => x.Create(
                    It.IsAny<IInjuryClass>(),
                    It.IsAny<IBodyPart>(),
                    It.IsAny<ITissueLayer>()),
                Times.Exactly(total));
        }

        void AssertTotalBodyPartInjuriesMade(int total)
        {
            InjuryFactoryMock.Verify(
                x => x.Create(
                    It.IsAny<IInjuryClass>(),
                    It.IsAny<IBodyPart>()),
                Times.Exactly(total));
        }

        void AssertInjury(IInjuryClass ic, IBodyPart bp, int total)
        {
            InjuryFactoryMock.Verify(
                x => x.Create(ic, bp),
                Times.Exactly(total));

        }

        void AssertInjury(IInjuryClass ic, IBodyPart bp, ITissueLayer tl, int total)
        {
            InjuryFactoryMock.Verify(
                x => x.Create(ic, bp, tl),
                Times.Exactly(total));
        }


        [TestMethod]
        public void AddDamage() 
        {
            var newDamageMock = new Mock<IDamageVector>();
            Builder.AddDamage(newDamageMock.Object);
            DamageMock.Verify(x => x.Add(newDamageMock.Object));
        }

        [TestMethod]
        public void SingleInjury_ArmorStops()
        {
            SetupDamageVector(new Dictionary<DamageType, uint>
            {
                { DamageType.Blunt, 10 },
                { DamageType.Slash, 10 },
                { DamageType.Pierce, 10 }
            });

            ArmorResistMock2.Setup(X => X.Resist(DamageMock.Object))
                .Returns(true);

            AddAllResistors();
            
            var result = Builder.Build();
            Assert.IsFalse(result.Any());

            ArmorResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Never());

            TissueResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Never());
            TissueResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Never());
            TissueResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Never());

            AssertTotalTissueInjuriesMade(0);
            AssertTotalBodyPartInjuriesMade(0);
        }

        [TestMethod]
        public void SingleInjury_BruisedTissue()
        {
            SetupDamageVector(new Dictionary<DamageType, uint>{
                { DamageType.Blunt, 10 }
            });

            Tissue2Mock.Setup(x => x.CanBeBruised)
                .Returns(true);

            TissueResistMock2.Setup(x => x.Resist(DamageMock.Object))
                .Returns(true);

            AddAllResistors();

            var result = Builder.Build().ToList();
            Assert.AreEqual(1, result.Count());

            var injury = result.First();

            Assert.AreSame(
                StandardInjuryClasses.BruisedTissueLayer, 
                injury.Class);

            ArmorResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Once());

            TissueResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Never());

            AssertTotalTissueInjuriesMade(1);
            AssertTotalBodyPartInjuriesMade(0);;
            AssertInjury(
                StandardInjuryClasses.BruisedTissueLayer,
                PartMock.Object,
                Tissue2Mock.Object,
                1);
        }

        [TestMethod]
        public void SingleInjury_TornTissue()
        {
            SetupDamageVector(new Dictionary<DamageType, uint>{
                { DamageType.Slash, 10 }
            });

            Tissue2Mock.Setup(x => x.CanBeTorn)
                .Returns(true);

            TissueResistMock2.Setup(x => x.Resist(DamageMock.Object))
                .Returns(true);

            AddAllResistors();

            var result = Builder.Build().ToList();
            Assert.AreEqual(1, result.Count());

            var injury = result.First();

            Assert.AreSame(
                StandardInjuryClasses.TornTissueLayer,
                injury.Class);

            ArmorResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Once());

            TissueResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Never());

            AssertTotalTissueInjuriesMade(1);
            AssertTotalBodyPartInjuriesMade(0);
            AssertInjury(
                StandardInjuryClasses.TornTissueLayer,
                PartMock.Object,
                Tissue2Mock.Object,
                1);
        }

        [TestMethod]
        public void SingleInjury_PuncturedTissue()
        {
            SetupDamageVector(new Dictionary<DamageType, uint>{
                { DamageType.Pierce, 10 }
            });

            Tissue2Mock.Setup(x => x.CanBePunctured)
                .Returns(true);

            TissueResistMock2.Setup(x => x.Resist(DamageMock.Object))
                .Returns(true);

            AddAllResistors();

            var result = Builder.Build().ToList();
            Assert.AreEqual(1, result.Count());

            var injury = result.First();

            Assert.AreSame(
                StandardInjuryClasses.PuncturedTissueLayer,
                injury.Class);

            ArmorResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Once());

            TissueResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Never());

            AssertTotalTissueInjuriesMade(1);
            AssertTotalBodyPartInjuriesMade(0);
            AssertInjury(
                StandardInjuryClasses.PuncturedTissueLayer,
                PartMock.Object,
                Tissue2Mock.Object,
                1);
        }

        [TestMethod]
        public void SingleInjury_BodyPartRemoval() 
        {
            SetupDamageVector(new Dictionary<DamageType, uint>{
                { DamageType.Blunt, 10 },
                { DamageType.Slash, 10 },
                { DamageType.Pierce, 10 }
            });
            
            AddAllResistors();

            var result = Builder.Build().ToList();
            Assert.AreEqual(1, result.Count());

            var injury = result.First();

            Assert.AreSame(
                StandardInjuryClasses.MissingBodyPart,
                injury.Class);

            ArmorResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            ArmorResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Once());

            TissueResistMock1.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock2.Verify(x => x.Resist(DamageMock.Object), Times.Once());
            TissueResistMock3.Verify(x => x.Resist(DamageMock.Object), Times.Once());

            AssertTotalTissueInjuriesMade(0);
            AssertTotalBodyPartInjuriesMade(1);
            AssertInjury(
                StandardInjuryClasses.MissingBodyPart,
                PartMock.Object,
                1);
        }

        [TestMethod]
        public void AllPossibleInjuries()
        {
            SetupDamageVector(new Dictionary<DamageType, uint>{
                { DamageType.Blunt, 10 },
                { DamageType.Slash, 10 },
                { DamageType.Pierce, 10 }
            });

            foreach (var tm in new[] { Tissue1Mock, Tissue2Mock, Tissue3Mock })
            {
                tm.Setup(x => x.CanBeBruised)
                    .Returns(true);
                tm.Setup(x => x.CanBePunctured)
                    .Returns(true);
                tm.Setup(x => x.CanBeTorn)
                    .Returns(true);
            }

            AddAllResistors();

            var result = Builder.Build().ToList();
            Assert.AreEqual((3 * 3) + 1, result.Count());
        }

    }
}
