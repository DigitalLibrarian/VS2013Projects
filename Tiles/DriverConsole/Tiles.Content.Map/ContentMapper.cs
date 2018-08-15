using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

using ContentModel = Tiles.Content.Models;
using EngineMaterials = Tiles.Materials;
using EngineCombat = Tiles.Agents.Combat;
using EngineItems = Tiles.Items;
using EngineBodies = Tiles.Bodies;
using EngineAgents = Tiles.Agents;
using Tiles.Math;

namespace Tiles.Content.Map
{
    public class ContentMapper : IContentMapper
    {
        public EngineMaterials.Material.MaterialStateProperty Map(ContentModel.MaterialStateProp p)
        {
            return new EngineMaterials.Material.MaterialStateProperty
            {
                Name = p.Name,
                State = p.State,
                Value = p.Value
            };
        }

        public EngineMaterials.IMaterial Map(ContentModel.Material m)
        {
            if (m == null) return null;

            return new EngineMaterials.Material(m.Name, m.Adjective, m.StateProps.Select(p => Map(p)))
            {
                ImpactFracture = m.ImpactFracture,
                ImpactYield = m.ImpactYield,
                ImpactStrainAtYield = m.ImpactStrainAtYield,

                ShearFracture = m.ShearFracture,
                ShearYield = m.ShearYield,
                ShearStrainAtYield = m.ShearStrainAtYield,

                CompressiveFracture = m.CompressiveFracture,
                CompressiveStrainAtYield = m.CompressiveStrainAtYield,
                CompressiveYield = m.CompressiveYield,

                TensileFracture = m.TensileFracture,
                TensileStrainAtYield = m.TensileStrainAtYield,
                TensileYield = m.TensileYield,

                TorsionFracture = m.TorsionFracture,
                TorsionStrainAtYield = m.TorsionStrainAtYield,
                TorsionYield = m.TorsionYield,

                BendingFracture = m.BendingFracture,
                BendingStrainAtYield = m.BendingStrainAtYield,
                BendingYield = m.BendingYield,

                SolidDensity = m.SolidDensity,
                SharpnessMultiplier = m.SharpnessMultiplier
            };
        }

        public EngineItems.IArmorClass Map(ContentModel.Armor armor)
        {
            if (armor == null) return null;
            return new ArmorClass(
                armor.SlotRequirements.Select(Map).ToArray());
        }

        public EngineItems.IWeaponClass Map(ContentModel.Weapon weapon)
        {
            if (weapon == null) return null;
            return new WeaponClass(
                weapon.SlotRequirements.Select(Map).ToArray(),
                weapon.Moves.Select(Map).ToArray());
        }

        public EngineItems.IItemClass Map(ContentModel.Item item)
        {
            return new ItemClass(
                string.Format("{0} {1}", item.Material.Adjective, item.NameSingular), 
                Map(item.Sprite), 
                item.Size,
                Map(item.Material), 
                Map(item.Weapon), 
                Map(item.Armor));
        }

        public EngineItems.ArmorSlot Map(ContentModel.ArmorSlot slot)
        {
            return (EngineItems.ArmorSlot)(int)slot;
        }

        public EngineItems.WeaponSlot Map(ContentModel.WeaponSlot slot)
        {
            return (EngineItems.WeaponSlot)(int)slot;
        }

        public EngineCombat.ICombatMoveClass Map(ContentModel.CombatMove move)
        {
            var cmc = new EngineCombat.CombatMoveClass(
                move.Verb.SecondPerson,
                Map(move.Verb),
                move.PrepTime,
                move.RecoveryTime)
                {
                    IsDefenderPartSpecific = move.IsDefenderPartSpecific,
                    IsMartialArts = move.IsMartialArts,
                    IsStrike = move.IsStrike,
                    IsItem = move.IsItem,
                    AttackerBodyStateChange = Map(move.BodyStateChange),
                    StressMode = Map(move.ContactType),
                    ContactArea = move.ContactArea,
                    MaxPenetration = move.MaxPenetration,
                    VelocityMultiplier = move.VelocityMultiplier,
                    Requirements = move.Requirements.Select(rq => Map(rq))
                };

            return cmc;
        }

        public EngineCombat.IBodyPartRequirement Map(ContentModel.BodyPartRequirement req)
        {
            return new EngineCombat.BodyPartRequirement
            {
                Type = Map(req.Type),
                Constraints = req.Constraints.Select(x => Map(x)).ToList()
            };
        }

        private EngineCombat.BprConstraint Map(ContentModel.BprConstraint x)
        {
            return new EngineCombat.BprConstraint(Map(x.ConstraintType))
            {
                Tokens = x.Tokens.ToList()
            };
        }

        private EngineCombat.BprConstraintType Map(ContentModel.BprConstraintType bprConstraintType)
        {
            return (EngineCombat.BprConstraintType)(int)bprConstraintType;
        }

        public EngineCombat.BodyPartRequirementType Map(ContentModel.BodyPartRequirementType t)
        {
            return (EngineCombat.BodyPartRequirementType)(int)t;
        }

        public EngineBodies.BodyStateChange Map(ContentModel.BodyStateChange bodyStateChange)
        {
            return (EngineBodies.BodyStateChange)(int)bodyStateChange;
        }

        public Tiles.IVerb Map(ContentModel.Verb verb)
        {
            return new Verb(new Dictionary<VerbConjugation, string>
            {
                { VerbConjugation.SecondPerson, verb.SecondPerson },
                { VerbConjugation.ThirdPerson, verb.ThirdPerson}
            },
            verb.IsTransitive
            );
        }

        public Sprite Map(ContentModel.Sprite sprite)
        {
            return new Sprite(sprite.Symbol, 
                Map(sprite.Foreground),
                Map(sprite.Background));
        }

        public Color Map(ContentModel.Color c)
        {
            return new Color(c.R, c.B, c.G, c.A);
        }


        public EngineAgents.IAgentClass Map(ContentModel.Agent agent)
        {
            return new EngineAgents.AgentClass(
                agent.Name,
                Map(agent.Sprite),
                Map(agent.Body)
                );
        }

        public EngineBodies.IBodyClass Map(ContentModel.Body body)
        {
            var partMap = body.Parts.ToDictionary(x => x, x => Map(x) );
            foreach (var part in body.Parts)
            {
                if (part.Parent != null)
                {
                    // urgly
                    partMap[part].Parent = partMap[part.Parent];
                }
            }

            return new EngineBodies.BodyClass(
                body.Size, 
                body.Parts.Select(x => partMap[x]), 
                body.Moves.Select(x => Map(x)),
                body.Attributes.Select(x => Map(x)),
                Map(body.BloodMaterial),
                Map(body.PusMaterial)
                );
        }

        private EngineBodies.IAttributeClass Map(ContentModel.Attribute x)
        {
            return new EngineBodies.AttributeClass(x.Name, x.Median);
        }

        EngineBodies.BodyPartClass Map(ContentModel.BodyPart bodyPart)
        {
            return new EngineBodies.BodyPartClass(
                name: bodyPart.NameSingular,
                tokenId: bodyPart.TokenId,
                tissueClass: Map(bodyPart.Tissue), 
                armorSlotType: Map(bodyPart.ArmorSlot), 
                weaponSlotType: Map(bodyPart.WeaponSlot), 
                categories: bodyPart.Categories,
                types: bodyPart.Types,
                relSize: bodyPart.RelativeSize,
                isCritical: true, 
                canGrasp: bodyPart.CanGrasp,
                canBeAmputated: bodyPart.CanBeAmputated,
                isNervous: bodyPart.IsNervous,
                isCirc: bodyPart.IsCirculatory,
                isSkeletal: bodyPart.IsSkeletal,
                isDigit: bodyPart.IsDigit,
                isBreathe: bodyPart.IsBreathe,
                isSight: bodyPart.IsSight,
                isStance: bodyPart.IsStance,
                isInternal: bodyPart.IsInternal,
                isSmall: bodyPart.IsSmall,
                isEmbedded: bodyPart.IsEmbedded,
                isConnector: bodyPart.IsConnector,
                preventsParentCollapse: bodyPart.PreventsParentCollapse,
                relations: bodyPart.BodyPartRelations.Select(x => Map(x))
                );
        }

        EngineBodies.BodyPartRelationStrategy Map(ContentModel.BodyPartRelationStrategy s)
        {
            return (EngineBodies.BodyPartRelationStrategy)(int)s;
        }

        EngineBodies.BodyPartRelationType Map(ContentModel.BodyPartRelationType t)
        {
            return (EngineBodies.BodyPartRelationType)(int)t;
        }

        EngineBodies.IBodyPartRelation Map(ContentModel.BodyPartRelation rel)
        {
            return new EngineBodies.BodyPartRelation
            {
                Type = Map(rel.Type),
                Strategy = Map(rel.Strategy),
                StrategyParam = rel.StrategyParam,
                Weight = rel.Weight
            };
        }

        EngineBodies.ITissueClass Map(ContentModel.Tissue tissue)
        {
            var layers = new List<EngineBodies.ITissueLayerClass>();
            foreach (var layer in tissue.Layers)
            {
                layers.Add(new EngineBodies.TissueLayerClass( Map(layer.Material), layer.RelativeThickness)
                    {
                        IsCosmetic = layer.IsCosmetic,
                        IsConnective = layer.IsConnective,
                        VascularRating = layer.VascularRating,
                        PainReceptors = layer.PainReceptors,
                        HealingRate = layer.HealingRate,
                        ThickensOnStrength = layer.ThickensOnStrength,
                        ThickensOnEnergyStorage = layer.ThickensOnEnergyStorage,
                        HasArteries = layer.HasArteries, 
                        HasMajorArteries = layer.HasMajorArteries
                    });
            }

            return new EngineBodies.TissueClass(layers);
        }


        public EngineMaterials.StressMode Map(ContentModel.ContactType ct)
        {
            return (EngineMaterials.StressMode)(int)ct;
        }
    }

}
