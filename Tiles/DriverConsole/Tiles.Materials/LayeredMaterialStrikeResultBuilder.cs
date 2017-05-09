using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class LayeredMaterialStrikeResultBuilder : ILayeredMaterialStrikeResultBuilder
    {
        class MaterialLayer
        {
            public object Tag { get; set; }
            public bool IsTagged { get { return Tag != null; } }

            public double Thickness { get; set; }
            public double Volume { get; set; }
            public IMaterial Material { get; set; }
        }

        ISingleLayerStrikeTester LayerTester { get; set; }

        double Momentum { get; set; }
        double StrikerContactArea { get; set; }
        double StrikerSharpness { get; set; }
        double StrickenContactArea { get; set; }
        double MaxPenetration { get; set; }
        StressMode StressMode { get; set; }

        IMaterial StrikerMaterial { get; set; }
        List<MaterialLayer> Layers { get; set; }

        public LayeredMaterialStrikeResultBuilder(ISingleLayerStrikeTester tester)
        {
            LayerTester = tester;
            Layers = new List<MaterialLayer>();
            Clear();
        }

        #region Configuration
        public void Clear()
        {
            Layers.Clear();
            Momentum = -1;
            StrikerContactArea = -1;
            StrickenContactArea = -1;
            StrikerSharpness = 0;
            StressMode = StressMode.None;
            StrikerMaterial = null;
        }

        public void SetStrikerMaterial(IMaterial mat)
        {
            StrikerMaterial = mat;
        }

        public void SetMomentum(double momentum)
        {
            Momentum = momentum;
        }

        public void SetStrikerContactArea(double contactArea)
        {
            StrikerContactArea = contactArea;
        }

        public void SetStrikerSharpness(double sharp)
        {
            StrikerSharpness = sharp;
        }

        public void SetStrickenContactArea(double contactArea)
        {
            StrickenContactArea = contactArea;
        }

        public void SetMaxPenetration(double maxPenetration)
        {
            MaxPenetration = maxPenetration;
        }

        public void SetStressMode(StressMode mode)
        {
            StressMode = mode;
        }

        public void AddLayer(IMaterial mat)
        {
            Layers.Add(new MaterialLayer
            {
                Material = mat
            });
        }
        public void AddLayer(IMaterial mat, double thick, double volume, object tag)
        {
            Layers.Add(new MaterialLayer
            {
                Material = mat,
                Thickness = thick,
                Volume = volume,
                Tag = tag
            });
        }
        #endregion

        public ILayeredMaterialStrikeResult Build()
        {
            const double epsilon = 0.00001d;

            var result = new LayeredMaterialStrikeResult();
            var contactArea = StrikerContactArea;
            
            var mode = StressMode;
            var momentum = Momentum;

            double penetration = 0;
            bool done = false;
            int turnsToBlunt = -1;
            var strikeMaterial = StrikerMaterial;
            double lastMomentumIn;
            for (int layerIndex = 0; layerIndex < Layers.Count(); layerIndex++)
            {
                if (momentum <= epsilon) break;
                if (turnsToBlunt-- == 0  || penetration >= MaxPenetration)
                {
                    mode = Materials.StressMode.Blunt;
                }

                lastMomentumIn = momentum;
                var layer = Layers[layerIndex];
                var layerResult = PerformSingleLayerTest(
                    strikeMaterial,
                    momentum,
                    contactArea,
                    MaxPenetration - penetration,
                    mode,
                    layer);
                 
                momentum = layerResult.ResultMomentum;

                if (layerResult.IsDefeated)
                {
                    if (mode == Materials.StressMode.Edge)
                    {
                        penetration += layer.Thickness;
                    }
                    else if (layerResult.StressResult == StressResult.Impact_CompleteFracture)
                    {
                        strikeMaterial = layer.Material;
                        mode = Materials.StressMode.Edge;
                        turnsToBlunt = 1;
                    }
                }
                else if (mode != StressMode.Blunt)
                {
                    mode = StressMode.Blunt;
                    // redo as blunt
                    if (layerResult.StressResult == StressResult.None)
                    {
                        // retry this layer with the mode change
                       layerIndex--;
                       momentum = lastMomentumIn;
                       continue;
                    }
                }
                else if (layerResult.StressResult == StressResult.None)
                {
                    done = true;
                }
                if (layer.IsTagged)
                {
                    result.AddLayerResult(layerResult, layer.Tag);
                }
                else
                {
                    result.AddLayerResult(layerResult);
                }

                if (mode == Materials.StressMode.Blunt)
                {
                    strikeMaterial = layer.Material;
                }

                if (done) break;
            }

            result.Penetration = penetration;

            return result;
        }

        MaterialStrikeResult PerformSingleLayerTest(
            IMaterial strikerMat, double momentum, double contactArea, double penetrationLeft,
            StressMode mode, MaterialLayer layer)
        {
            return LayerTester.StrikeTest(
                mode,
                strikerMat,
                StrikerSharpness,
                contactArea,
                momentum, 
                penetrationLeft,
                layer.Material,
                layer.Thickness,
                layer.Volume,
                StrickenContactArea);
        }
    }
}
