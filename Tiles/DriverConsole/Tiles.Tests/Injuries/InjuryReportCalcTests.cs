using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Injuries;
using Tiles.Materials;

namespace Tiles.Tests.Injuries
{
    [TestClass]
    public class InjuryReportCalcTests
    {
        Mock<IInjuryFactory> InjuryFactoryMock { get; set; }
        Mock<ILayeredMaterialStrikeResultBuilder> BuilderMock { get; set; }

        InjuryReportCalc Calc { get; set; }

        Mock<ICombatMoveContext> ContextMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryFactoryMock = new Mock<IInjuryFactory>();
            BuilderMock = new Mock<ILayeredMaterialStrikeResultBuilder>();

            Calc = new InjuryReportCalc(InjuryFactoryMock.Object, BuilderMock.Object);
        }

        [Ignore]
        [TestMethod]
        public void InjuryReportCalc_MaterialStrike_ZeroTissueLayers()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void InjuryReportCalc_MaterialStrike_ArmorLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void InjuryReportCalc_MaterialStrike_NonCosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void InjuryReportCalc_MaterialStrike_CosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void InjuryReportCalc_MaterialStrike_InternalNonCosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void InjuryReportCalc_MaterialStrike_InternalCosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }
    }
}
