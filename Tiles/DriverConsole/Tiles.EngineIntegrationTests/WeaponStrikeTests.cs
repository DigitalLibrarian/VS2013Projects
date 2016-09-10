using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DfNet.Raws;
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;
using Tiles.ScreensImpl.SiteFactories;
using Tiles.Ecs;
using Tiles.Random;
using Tiles.Agents;
using Tiles.Math;
using Moq;
using Tiles.Items;
using Tiles.Agents.Combat;
using Tiles.Injuries;
using Tiles.Materials;

namespace Tiles.EngineIntegrationTests
{
    [TestClass]
    public class WeaponStrikeTests
    {
        IRandom Random { get; set; }

        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IContentMapper ContentMapper { get; set; }

        IEntityManager EntityManager { get; set; }
        DfTagsFascade DfTagsFascade { get; set; }

        IAtlas Atlas { get; set; }

        ICombatMoveBuilder CombatMoveBuilder { get; set; }
        IInjuryReportCalc InjuryReportCalc { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            DfAgentFactory = new DfAgentFactory(Store,
                new DfAgentBuilderFactory(),
                new DfColorFactory(),
                new DfMaterialFactory(Store, new DfMaterialBuilderFactory()),
                new DfCombatMoveFactory(),
                new DfBodyAttackFactory()
                );

            ContentMapper = new ContentMapper();

            EntityManager = new EntityManager();
            DfTagsFascade = new DfTagsFascade(Store, EntityManager, Random);

            Atlas = new Mock<IAtlas>().Object;
            CombatMoveBuilder = new CombatMoveBuilder();
            InjuryReportCalc = new InjuryReportCalc(new LayeredMaterialStrikeResultBuilder(new MaterialStrikeResultBuilder()));
        }

        [TestMethod]
        public void DwarfVsDwarf_ToeWithSteelSwordSlash()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewDwarf();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("first toe"));
            Assert.IsNotNull(targetBodyPart);

            var sword = GetInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);
            Assert.AreEqual(914, (int)(strikeMomentum * 10));

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var nailLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("nail"));
            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                slashMoveClass.StressMode,
                strikeMomentum,
                slashMoveClass.ContactArea,
                slashMoveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            Assert.AreSame(BodyPartInjuryClasses.Severed, partInjury.Class);

            Assert.AreEqual(5, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(TissueLayerInjuryClasses.Tear, tInjury.Class);
            Assert.AreSame(nailLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(TissueLayerInjuryClasses.Tear, tInjury.Class);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(TissueLayerInjuryClasses.Tear, tInjury.Class);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(TissueLayerInjuryClasses.TearApart, tInjury.Class);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual(TissueLayerInjuryClasses.TearApart, tInjury.Class);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_ToeWithSteelSwordSlash()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = GetInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var slashMoveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("slash"));
            Assert.IsNotNull(slashMoveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, slashMoveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);
            Assert.AreEqual(914, (int)(strikeMomentum * 10));

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var hairLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("hair"));
            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                slashMoveClass.StressMode,
                strikeMomentum,
                slashMoveClass.ContactArea,
                slashMoveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            Assert.AreSame(BodyPartInjuryClasses.JustTissueDamage, partInjury.Class);

            Assert.AreEqual(3, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(TissueLayerInjuryClasses.Tear, tInjury.Class);
            Assert.AreSame(hairLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(TissueLayerInjuryClasses.Tear, tInjury.Class);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(TissueLayerInjuryClasses.Tear, tInjury.Class);
            Assert.AreSame(fatLayer, tInjury.Layer);

        }




        IAgent GetNewDwarf()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "DWARF", "MALE", Vector3.Zero);
        }

        IAgent GetNewUnicorn()
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, "UNICORN", "MALE", Vector3.Zero);
        }

        IItem GetInorganicWeapon(string name, string materialName)
        {
            return DfTagsFascade.CreateInorganicWeapon(name, materialName);
        }
    }

}
