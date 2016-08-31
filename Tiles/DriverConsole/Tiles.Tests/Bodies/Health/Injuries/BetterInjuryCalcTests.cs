using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Tests.Bodies.Health.Injuries
{
    [TestClass]
    public class BetterInjuryCalcTests
    {
        Mock<IInjuryFactory> InjuryFactoryMock { get; set; }

        BetterInjuryCalc InjuryCalc { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryFactoryMock = new Mock<IInjuryFactory>();
            InjuryCalc = new BetterInjuryCalc(InjuryFactoryMock.Object);
        }

        [Ignore]
        [TestMethod]
        public void ElasticEdgedCollsionTurnsToBlunt()
        {

        }

        [Ignore]
        [TestMethod]
        public void SingleLayer_Edged_Elastic()
        {

        }

        [Ignore]
        [TestMethod]
        public void SingleLayer_Edged_Plastic()
        {

        }

        [Ignore]
        [TestMethod]
        public void SingleLayer_Edged_Fracture()
        {

        }

        [Ignore]
        [TestMethod]
        public void SingleLayer_Blunt_Elastic()
        {

        }

        [Ignore]
        [TestMethod]
        public void SingleLayer_Blunt_Plastic()
        {

        }

        [Ignore]
        [TestMethod]
        public void SingleLayer_Blunt_Fracture()
        {

        }

        [Ignore]
        [TestMethod]
        public void BluntPassesThroughElasticLayersToCauseInternalFracture()
        {

        }
        
        [Ignore]
        [TestMethod]
        public void BluntPassesThroughElasticLayersToCausePlasticDeform()
        {

        }
    }
}
