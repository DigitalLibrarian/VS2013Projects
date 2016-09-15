using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface ILayeredMaterialStrikeResultBuilder
    {
        void AddLayer(IMaterial mat);
        void AddLayer(IMaterial mat, double thick, double volume, object tag);

        void SetStrikerMaterial(IMaterial mat);

        void SetMomentum(double momentum);
        void SetContactArea(int contactArea);
        void SetMaxPenetration(int maxPenetration);
        void SetStressMode(StressMode mode);

        ILayeredMaterialStrikeResult Build();

        void Clear();
    }

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
        int ContactArea { get; set; }
        int MaxPenetration { get; set; }
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
            ContactArea = -1;
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

        public void SetContactArea(int contactArea)
        {
            ContactArea = contactArea;
        }

        public void SetMaxPenetration(int maxPenetration)
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
            var contactArea = (double)ContactArea;
            
            var mode = StressMode;
            var momentum = Momentum;

            int penetration = 0;
            bool done = false;
            double epsilon = 0.001d;
            foreach (var layer in Layers)
            {
                if (momentum <= epsilon) break;
                if (mode == StressMode.Edge && penetration >= MaxPenetration) mode = Materials.StressMode.Blunt;

                var layerResult = PerformSingleLayerTest(
                    StrikerMaterial,
                    momentum,
                    ContactArea,
                    mode,
                    layer);

                momentum = layerResult.ResultMomentum;
                if (layerResult.IsDefeated)
                {
                    if (mode == Materials.StressMode.Edge)
                    {
                        penetration += (int)layer.Thickness;
                    }
                }
                else if (mode != StressMode.Blunt)
                {
                    mode = StressMode.Blunt;
                }
                else
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

                if (done) break;
            }

            result.Penetration = System.Math.Min(penetration, MaxPenetration);

            return result;
        }

        IMaterialStrikeResult PerformSingleLayerTest(
            IMaterial strikerMat, double momentum, int contactArea,
            StressMode mode, MLayer layer)
        {
            // TODO - If the weapon has a smaller contact area than the layer, the layer's volume is reduced by the ratio of areas.
            // (Volume damaged by weapon) = (layer volume) x (weapon contact area) / (layer contact area)


            Builder.Clear();

            Builder.SetStressMode(mode);
            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrickenMaterial(layer.Material);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerVolume(layer.Volume);

            contactArea = (int) System.Math.Min(contactArea, layer.Thickness);
            Builder.SetContactArea(contactArea);

            return Builder.Build();
        }
    }
}
