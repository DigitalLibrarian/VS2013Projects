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
        private IInjuryFactory InjuryFactory { get; set; }
        private ILayeredMaterialStrikeResultBuilder Builder { get; set; }

        public InjuryReportCalc()
        {
            InjuryFactory = new InjuryFactory();
            Builder = new LayeredMaterialStrikeResultBuilder(new SingleLayerStrikeTester(new MaterialStrikeResultBuilder())); 
        }

        public InjuryReportCalc(IInjuryFactory injuryFactory, ILayeredMaterialStrikeResultBuilder resultBuilder)
        {
            InjuryFactory = injuryFactory;
            Builder = resultBuilder;
        }

        public IInjuryReport CalculateMaterialStrike(
            IEnumerable<IItem> armorItems, StressMode stressMode, 
            double momentum, double contactArea, int maxPenetration, 
            IBody targetBody, IBodyPart targetPart, IMaterial strikerMat, double sharpness)
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
                Builder.SetStrickenContactArea(targetPart.GetContactArea());
                Builder.SetMaxPenetration(maxPenetration);
                Builder.SetStrikerMaterial(strikerMat);
                Builder.SetStrikerSharpness(sharpness);

                var tissueLayers = targetPart.Tissue.TissueLayers.Reverse();

                // TODO - these armor items might need to be reversed
                foreach (var armorItem in armorItems)
                {
                    Builder.AddLayer(armorItem.Class.Material);
                }

                foreach (var tissueLayer in tissueLayers)
                {
                    if (!tissueLayer.Class.IsCosmetic)
                    {
                        Builder.AddLayer(tissueLayer.Material, tissueLayer.Thickness, tissueLayer.Volume, tissueLayer);
                        tlParts.Add(tissueLayer, targetPart);
                    }
                }

                // TODO - it should not be possible to sever internal parts, but we can "spill" them
                foreach (var internalPart in targetBody.GetInternalParts(targetPart))
                {
                    foreach (var tissueLayer in internalPart.Tissue.TissueLayers.Reverse())
                    {
                        if (!tissueLayer.Class.IsCosmetic)
                        {
                            Builder.AddLayer(tissueLayer.Material, tissueLayer.Thickness, tissueLayer.Volume, tissueLayer);
                            tlParts.Add(tissueLayer, internalPart);
                        }
                    }
                }
            }

            var result = Builder.Build();

            // the injury factory translates from material strike results to injury descriptions
            var bpInjuries = InjuryFactory.Create(targetPart, contactArea, maxPenetration, result, tlParts);

            return new InjuryReport(bpInjuries);
        }
    }
}
