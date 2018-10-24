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
            ImplementWasSmall = false;
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

        public void SetImplementWasSmall(bool wasSmall)
        {
            ImplementWasSmall = wasSmall;
        }
        #endregion

        public ILayeredMaterialStrikeResult Build()
        {
            var result = new LayeredMaterialStrikeResult();
            var contactArea = StrikerContactArea;
            
            var mode = StressMode;
            var momentum = Momentum;

            double penRemaining = MaxPenetration;
            double penetration = 0;
            bool done = false;
            var strikeMaterial = StrikerMaterial;
            double lastMomentumIn;
            for (int layerIndex = 0; layerIndex < Layers.Count(); layerIndex++)
            {
                if (penetration >= MaxPenetration)
                {
                    mode = Materials.StressMode.Blunt;
                }

                lastMomentumIn = momentum;
                var layer = Layers[layerIndex];
                var layerResult = PerformSingleLayerTest(
                    strikeMaterial,
                    momentum,
                    contactArea,
                    MaxPenetration,
                    penRemaining,
                    mode,
                    layer);
                 
                momentum = layerResult.ResultMomentum;

                if (layerResult.IsDefeated)
                {
                    if (mode == Materials.StressMode.Edge && penRemaining > 0d)
                    {
                        var penComp = layer.Thickness * layerResult.PenetrationRatio;
                        penetration += penComp;
                        penRemaining -= penComp;
                        if (layerResult.PenetrationRatio < 1d
                            || penRemaining < 1d)
                        {
                            penRemaining = 0d;
                            penetration = MaxPenetration;
                        }
                    } 
                    else if (layerResult.StressResult == StressResult.Impact_CompleteFracture)
                    {
                        strikeMaterial = layer.Material;
                        mode = Materials.StressMode.Edge;
                        MaxPenetration = layer.Thickness;
                        penRemaining = MaxPenetration;
                        penetration = 0;
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
                    //if (mode == Materials.StressMode.Blunt) layerResult.StressResult = StressResult.Impact_Bypass;
                }
                if (layer.IsTagged)
                {
                    result.AddLayerResult(layerResult, layer.Tag);
                }
                else
                {
                    result.AddLayerResult(layerResult);
                }

                if (done) break;
            }


            result.Penetration = penetration;


            return result;
        }

        MaterialStrikeResult PerformSingleLayerTest(
            IMaterial strikerMat, double momentum, double contactArea, 
            double maxPenetration, double penetrationLeft,
            StressMode mode, MaterialLayer layer)
        {
            return LayerTester.StrikeTest(
                mode,
                strikerMat,
                StrikerSharpness,
                contactArea,
                momentum, 
                maxPenetration,
                penetrationLeft,
                layer.Material,
                layer.Thickness,
                layer.Volume,
                StrickenContactArea,
                ImplementWasSmall,
                ImplementSize);
        }

        public bool ImplementWasSmall { get; set; }
        public double ImplementSize { get; set; }

        public void SetImplementSize(double size)
        {
            ImplementSize = size;
        }
    }
}
