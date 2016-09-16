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
    public class DwarfVsUnicornStrikeTests : DfContentTestBase
    {
        IAgent Dwarf { get; set; }
        IAgent Unicorn { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            Dwarf = CreateAgent("DWARF", "MALE", Vector3.Zero);
            Unicorn = CreateAgent("UNICORN", "MALE", Vector3.Zero);
        }

        [TestMethod]
        public void DwarfVsUnicorn_SlashLegWithSteelSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();
            
            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
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

            //Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Impact_Dent, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_SlashLegWithCopperSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
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

            //Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Impact_Dent, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_StabLegWithSteelSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            Assert.IsNotNull(moveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

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

            //Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

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
        public void DwarfVsUnicorn_StabLowerBodyWithSteelSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "STEEL");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            Assert.IsNotNull(moveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var hairLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("hair"));
            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                strikeMomentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            //Assert.AreSame(BodyPartInjuryClasses.JustTissueDamage, partInjury.Class);

           // Assert.AreEqual(3, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

        }


        [TestMethod]
        public void DwarfVsUnicorn_StabLowerBodyWithCopperSword()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("lower body"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT, "COPPER");
            attacker.Outfit.Wield(sword);

            var moveClass = sword.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("stab"));
            Assert.IsNotNull(moveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, sword);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var hairLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("hair"));
            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                strikeMomentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                sword.Class.Material
                );

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            //Assert.AreSame(BodyPartInjuryClasses.JustTissueDamage, partInjury.Class);

            // Assert.AreEqual(3, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Shear_Cut, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Impact_Dent, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

        }


        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithSilverMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            Assert.IsNotNull(moveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

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
                weapon.Class.Material
                );

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            //Assert.AreSame(BodyPartInjuryClasses.JustTissueDamage, partInjury.Class);

            //Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_BashHoofWithSilverMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front hoof"));
            Assert.IsNotNull(targetBodyPart);

            var weapon = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "SILVER");
            attacker.Outfit.Wield(weapon);

            var moveClass = weapon.Class.WeaponClass.AttackMoveClasses.SingleOrDefault(mc => mc.Name.Equals("bash"));
            Assert.IsNotNull(moveClass);

            var slashMove = CombatMoveBuilder.AttackBodyPartWithWeapon(attacker, defender, moveClass, targetBodyPart, weapon);

            var strikeMomentum = attacker.GetStrikeMomentum(slashMove);

            var context = new CombatMoveContext(attacker, defender, slashMove);

            var hoofLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("hoof"));

            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
                context,
                moveClass.StressMode,
                strikeMomentum,
                moveClass.ContactArea,
                moveClass.MaxPenetration,
                targetBodyPart,
                weapon.Class.Material
                );

            Assert.AreEqual(1, injuryReport.BodyPartInjuries.Count());

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);
            
            Assert.AreEqual(1, partInjury.TissueLayerInjuries.Count());

            // glance
            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(hoofLayer, tInjury.Layer);
        }

        [TestMethod]
        public void DwarfVsUnicorn_BashLegWithCopperMace()
        {
            var attacker = GetNewDwarf();
            var defender = GetNewUnicorn();

            var targetBodyPart = defender.Body.Parts.First(x => x.Name.Equals("right front leg"));
            Assert.IsNotNull(targetBodyPart);

            var sword = CreateInorganicWeapon(DfTags.MiscTags.ITEM_WEAPON_MACE, "COPPER");
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

            //Assert.AreEqual(4, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);

            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Impact_Bypass, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(3);
            Assert.AreEqual(MaterialStressResult.None, tInjury.StrikeResult.StressResult);
            Assert.AreSame(boneLayer, tInjury.Layer);
        }

        [TestMethod]
        public void UnicornVsDwarf_StabUpperBody()
        {
            //Unicorn 1 stabs Dwarf 2 in the right lower arm, tearing the muscle!
            var attacker = GetNewUnicorn();
            var defender = GetNewDwarf();

            var targetBodyPart = defender.Body.Parts.Single(p => p.Name.Equals("upper body"));

            var moveClass = attacker.Body.Moves.Single(x => x.Name.Equals("stab"));
            var move = CombatMoveBuilder.BodyMove(attacker, defender, moveClass, targetBodyPart);

            var momentum = attacker.GetStrikeMomentum(move);
            var context = new CombatMoveContext(attacker, defender, move);

            //  TODO - most dense tissue layer
            var relatedParts = move.Class.GetRelatedBodyParts(attacker.Body);
            var weaponMat = relatedParts.First().Tissue.TissueLayers.Last().Material;


            var skinLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("skin"));
            var fatLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("fat"));
            var muscleLayer = targetBodyPart.Tissue.TissueLayers.Single(x => x.Material.Name.Equals("muscle"));


            var injuryReport = InjuryReportCalc.CalculateMaterialStrike(
               context,
               moveClass.StressMode,
               momentum,
               moveClass.ContactArea,
               moveClass.MaxPenetration,
               targetBodyPart,
               weaponMat
               );

            var partInjury = injuryReport.BodyPartInjuries.First();
            Assert.AreEqual(targetBodyPart, partInjury.BodyPart);

            Assert.AreEqual(3, partInjury.TissueLayerInjuries.Count());

            var tInjury = partInjury.TissueLayerInjuries.ElementAt(0);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(skinLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(1);

            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(fatLayer, tInjury.Layer);

            tInjury = partInjury.TissueLayerInjuries.ElementAt(2);
            Assert.AreEqual(MaterialStressResult.Shear_CutThrough, tInjury.StrikeResult.StressResult);
            Assert.AreSame(muscleLayer, tInjury.Layer);
        }

        IAgent GetNewDwarf()
        {
            return Dwarf;
        }

        IAgent GetNewUnicorn()
        {
            return Unicorn;
        }
    }
}
