using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Random;

namespace Tiles.Bodies.Injuries
{
    public class InjuryReportCalc : IInjuryReportCalc
    {
        private IRandom Random { get; set; }
        private IInjuryFactory InjuryFactory { get; set; }
        private ILayeredMaterialStrikeResultBuilder Builder { get; set; }

        public InjuryReportCalc(IRandom random, IInjuryFactory injuryFactory)
        {
            Random = random;
            InjuryFactory = injuryFactory;
            Builder = new LayeredMaterialStrikeResultBuilder(new SingleLayerStrikeTester(new MaterialStrikeResultBuilder())); 
        }

        public InjuryReportCalc(IRandom random, IInjuryFactory injuryFactory, ILayeredMaterialStrikeResultBuilder resultBuilder)
        {
            Random = random;
            InjuryFactory = injuryFactory;
            Builder = resultBuilder;
        }

        public IInjuryReport CalculateMaterialStrike(
            IEnumerable<IItem> armorItems, StressMode stressMode, 
            double momentum, double contactArea, double maxPenetration, 
            IBody targetBody, IBodyPart targetPart, IMaterial strikerMat, double sharpness, 
            bool implementWasSmall, double implementSize, bool implementIsEdged)
        {
            // tlParts is an index of all involved tissue layers to their containing parts.  
            // It will contain this data for all parts nested inside the target
            var tlParts = new Dictionary<ITissueLayer, IBodyPart>();

            Builder.Clear();
            // setup builder and generate tlParts
            {
                Builder.SetStressMode(stressMode);
                Builder.SetMomentum(momentum);
                Builder.SetStrikerContactArea(contactArea);
                Builder.SetStrickenContactArea(targetPart.ContactArea);
                Builder.SetMaxPenetration(maxPenetration);
                Builder.SetStrikerMaterial(strikerMat);
                Builder.SetStrikerSharpness(sharpness);
                Builder.SetImplementIsSmall(implementWasSmall);
                Builder.SetImplementSize(implementSize);
                Builder.SetImplementIsEdged(implementIsEdged);

                var tissueLayers = targetPart.Tissue.TissueLayers.Reverse();

                // TODO - add armor items

                foreach (var tissueLayer in tissueLayers)
                {
                    if (IsSuitable(tissueLayer))
                    {
                        Builder.AddLayer(
                            tissueLayer.Material, 
                            tissueLayer.Thickness,
                            tissueLayer.Volume, 
                            tissueLayer);
                        tlParts.Add(tissueLayer, targetPart);
                    }
                }

                // TODO - it should not be possible to sever internal parts, but we can "spill" them

                // Internal wounds - pick one internal part at random.  if it is surrounded by anything, then that goes first.
                var allInternals = targetBody.GetInternalParts(targetPart);
                var strikePathInternals = new List<IBodyPart>();
                if (allInternals.Any())
                {
                    var targetInternal = Random.NextElement<IBodyPart>(allInternals);
                    foreach (var testInternal in allInternals)
                    {
                        if (testInternal != targetInternal)
                        {
                            var weight = testInternal.Class.GetBpRelationWeight(targetInternal.Class, BodyPartRelationType.Around);
                            if (weight > 0)
                            {
                                strikePathInternals.Add(testInternal);
                                break;
                            }
                        }
                    }
                    strikePathInternals.Add(targetInternal);

                    foreach (var strikePathInternal in strikePathInternals)
                    {
                        foreach (var tissueLayer in strikePathInternal.Tissue.TissueLayers.Reverse())
                        {
                            if (IsSuitable(tissueLayer))
                            {
                                // TODO - need to figure in the effective thickness and volume by accounting for existing wounds
                                Builder.AddLayer(tissueLayer.Material, tissueLayer.Thickness, tissueLayer.Volume, tissueLayer);
                                tlParts.Add(tissueLayer, strikePathInternal);
                            }
                        }
                    }
                }
            }

            var result = Builder.Build();

            // the injury factory translates from material strike results to injury descriptions
            var bpInjuries = InjuryFactory.Create(targetBody, targetPart, contactArea, maxPenetration, result, tlParts);

            return new InjuryReport(bpInjuries);
        }

        private bool IsSuitable(ITissueLayer layer)
        {
            return !layer.Class.IsCosmetic && !layer.IsPulped;
        }
    }
}
