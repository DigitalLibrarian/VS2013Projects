﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class LayeredMaterialStrikeResultBuilder : ILayeredMaterialStrikeResultBuilder
    {
        class MLayer
        {
            public object Tag { get; set; }
            public bool IsTagged { get { return Tag != null; } }

            public double Thickness { get; set; }
            public double Volume { get; set; }
            public IMaterial Material { get; set; }
        }

        IMaterialStrikeResultBuilder Builder { get; set; }

        double Momentum { get; set; }
        double StrikerContactArea { get; set; }
        double StrikerSharpness { get; set; }
        double StrickenContactArea { get; set; }
        double MaxPenetration { get; set; }
        StressMode StressMode { get; set; }

        IMaterial StrikerMaterial { get; set; }
        List<MLayer> Layers { get; set; }

        public LayeredMaterialStrikeResultBuilder(IMaterialStrikeResultBuilder matStrikeBuilder)
        {
            Builder = matStrikeBuilder;
            Layers = new List<MLayer>();
            Clear();
        }

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
            Layers.Add(new MLayer
            {
                Material = mat
            });
        }
        public void AddLayer(IMaterial mat, double thick, double volume, object tag)
        {
            Layers.Add(new MLayer
            {
                Material = mat,
                Thickness = thick,
                Volume = volume,
                Tag = tag
            });
        }

        public ILayeredMaterialStrikeResult Build()
        {
            var result = new LayeredMaterialStrikeResult();
            var contactArea = StrikerContactArea;
            
            var mode = StressMode;
            var momentum = Momentum;

            double penetration = 0;
            bool done = false;
            double epsilon = 0.00001d;
            int turnsToBlunt = -1;
            var strikeMaterial = StrikerMaterial;
            for (int layerIndex = 0; layerIndex < Layers.Count(); layerIndex++)
            {
                if (momentum <= epsilon) break;
                if (turnsToBlunt-- == 0  || penetration >= MaxPenetration)
                {
                    mode = Materials.StressMode.Blunt;
                }

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
                    else
                    {
                        if (layerResult.StressResult == StressResult.Impact_CompleteFracture)
                        {
                            if (mode == Materials.StressMode.Blunt)
                            {
                                strikeMaterial = layer.Material;
                                mode = Materials.StressMode.Edge;
                                turnsToBlunt = 1;
                            }
                        }
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

            result.Penetration = (int) System.Math.Min(penetration, MaxPenetration);

            return result;
        }

        MaterialStrikeResult PerformSingleLayerTest(
            IMaterial strikerMat, double momentum, double contactArea, double penetrationLeft,
            StressMode mode, MLayer layer)
        {
            Builder.Clear();

            Builder.SetStressMode(mode);
            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrikerSharpness(StrikerSharpness);
            Builder.SetStrickenMaterial(layer.Material);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerVolume(System.Math.Max(1d, layer.Volume));
            Builder.SetLayerThickness(layer.Thickness);
            Builder.SetRemainingPenetration(penetrationLeft);

            Builder.SetStrikerContactArea(contactArea);
            Builder.SetStrickenContactArea(StrickenContactArea);

            return Builder.Build();
        }
    }
}