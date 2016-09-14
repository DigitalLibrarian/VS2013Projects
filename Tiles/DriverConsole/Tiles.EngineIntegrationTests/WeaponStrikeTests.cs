﻿using System;
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
            //Assert.AreSame(BodyPartInjuryClasses.Severed, partInjury.Class);

            Assert.AreEqual(5, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(nailLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(4);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            //AssertTissueInjuryClass(TissueLayerInjuryClasses.TearApart, tInjury);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_SlashLegWithSteelSwordSlash()
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
            //Assert.AreSame(BodyPartInjuryClasses.JustTissueDamage, partInjury.Class);

            Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithCopperMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = GetInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "COPPER");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            Assert.IsNotNull(moveClass);

            var move = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(move);

            var context = new CombatMoveContext(attacker, defender, move);

            var hairLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("hair"));
            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));
            var boneLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                strikeMomentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            //Assert.AreSame(BodyPartInjuryClasses.JustTissueDamage, partInjury.Class);

            Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(2, (int)tInjury.StrikeResult.MomentumThreshold);
            Assert.AreEqual(MaterialStressResult.Impact_Dent, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);

            Assert.AreEqual(MaterialStressResult.Impact_Dent, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(68, (int)tInjury.StrikeResult.MomentumThreshold);
            Assert.AreEqual(MaterialStressResult.Impact_Dent, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(1376, tInjury.StrikeResult.MomentumThreshold);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }


        [TestMethod]
        public void Unicorn_SkinTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(2, (int)cost);
        }


        [TestMethod]
        public void Unicorn_FatTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(30, (int)cost);
        }



        [TestMethod]
        public void Unicorn_MuscleTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(76, (int)cost);
        }




        [TestMethod]
        public void Unicorn_BoneTissue_ImpactCostDent()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var layer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("bone"));

            var cost = MaterialStressCalc.ImpactCost1(layer.Material, layer.Volume);
            Assert.AreEqual(1520, (int)cost);
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

        void AssertTissueInjuryClass(ITissueLayerInjuryClass expected, ITissueLayerInjury tInjury)
        {
            Assert.AreSame(expected, tInjury.Class, string.Format("Expected injury class {0} for tissue layer {1}, but got {2}", expected.Adjective, tInjury.Layer.Material.Name, tInjury.Class.Adjective));
        }
    }

}
