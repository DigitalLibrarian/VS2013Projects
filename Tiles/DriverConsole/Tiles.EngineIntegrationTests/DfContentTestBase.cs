using DfNet.Raws;
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
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;
using Tiles.Ecs;
using Tiles.Bodies.Injuries;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;
using Tiles.Random;
using Tiles.ScreensImpl.SiteFactories;

namespace Tiles.EngineIntegrationTests
{
    public class DfContentTestBase
    {
        IRandom Random { get; set; }

        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IContentMapper ContentMapper { get; set; }

        IEntityManager EntityManager { get; set; }
        DfTagsFascade DfTagsFascade { get; set; }

        IAtlas Atlas { get; set; }

        protected ICombatMoveFactory CombatMoveFactory { get; set; }
        protected IInjuryReportCalc InjuryReportCalc { get; set; }

        public virtual void Initialize()
        {
            Store = TestContentStore.Get();
            DfAgentFactory = new DfAgentFactory(Store,
                new DfAgentBuilderFactory(),
                new DfColorFactory(),
                new DfMaterialFactory(Store, new DfMaterialBuilderFactory()),
                new DfTissueTemplateFactory(Store),
                new DfCombatMoveFactory(),
                new DfBodyAttackFactory()
                );

            ContentMapper = new ContentMapper();

            EntityManager = new EntityManager();
            DfTagsFascade = new DfTagsFascade(Store, EntityManager, Random);

            Atlas = new Mock<IAtlas>().Object;
            CombatMoveFactory = new CombatMoveFactory();
            InjuryReportCalc = new InjuryReportCalc();
        }


        protected IAgent CreateAgent(string name, string caste)
        {
            return CreateAgent(name, caste, Vector3.Zero);
        }
        protected IAgent CreateAgent(string name, string caste, Vector3 pos)
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, name, caste, pos);
        }

        protected IItem CreateInorganicWeapon(string name, string materialName)
        {
            return DfTagsFascade.CreateInorganicWeapon(name, materialName);
        }

        protected IItem CreateMaterialTemplateWeapon(string name, string matTempName)
        {
            return DfTagsFascade.CreateMaterialTemplateWeapon(name, matTempName);
        }

        protected IInjuryReport AssertTissueStrikeResults(
            IAgent attacker, IAgent defender, IBodyPart targetPart, 
            ICombatMove move, 
            params StressResult[] expectedLayerResults
            )
        {
            IMaterial strikerMaterial = attacker.GetStrikeMaterial(move);
            var mom = attacker.GetStrikeMomentum(move);
            var context = new CombatMoveContext(attacker, defender, move);
            var moveClass = move.Class;

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context.Defender.Outfit.GetItems(targetPart).Where(x => x.IsArmor),
                moveClass.StressMode,
                mom,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                defender.Body,
                targetPart,
                strikerMaterial,
                move.Sharpness
                );

            var partInjury = injuryReport.BodyPartInjuries.First();

            var got = partInjury.TissueLayerInjuries.Count();
            var expected = expectedLayerResults.Count();

            if (got < expected)
            {
                Assert.Fail("Got {0} layer result(s), expected {1}.", got, expected);
            }

            int i = 0;
            foreach (var exp in expectedLayerResults)
            {
                var tInjury = partInjury.TissueLayerInjuries.ElementAt(i);
                Assert.AreEqual(exp, tInjury.StrikeResult.StressResult,
                    string.Format("Expected <{0}>, got <{1}> for {2} {3}",
                        exp,
                        tInjury.StrikeResult.StressResult,
                        partInjury.BodyPart.Name,
                        tInjury.Layer.Material.Name));
                i++;
            }

            if (got > expected)
            {
                foreach (var tInjury in partInjury.TissueLayerInjuries.Skip(expected))
                {
                    if (tInjury.StrikeResult.StressResult != StressResult.None)
                    {
                        Assert.Fail(string.Format("Unexpected tissue layer result {0} for {1} layer", tInjury.StrikeResult.StressResult, tInjury.Layer.Material.Name));
                    }
                }
            }
            return injuryReport;
        }

        public IMaterial CreateInorganicMaterial(string matName)
        {
            return DfTagsFascade.GetInorganic(matName);
        }
    }
}
