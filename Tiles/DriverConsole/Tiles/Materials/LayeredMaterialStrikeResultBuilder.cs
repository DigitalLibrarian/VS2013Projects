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

        void SetStrikerContactArea(double contactArea);
        void SetStrickenContactArea(double contactArea);
        void SetMaxPenetration(double maxPenetration);
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
        double StrikerContactArea { get; set; }
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
                    StrikerContactArea,
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

            result.Penetration = (int) System.Math.Min(penetration, MaxPenetration);

            return result;
        }

        IMaterialStrikeResult PerformSingleLayerTest(
            IMaterial strikerMat, double momentum, double contactArea,
            StressMode mode, MLayer layer)
        {
            // TODO - If the weapon has a smaller contact area than the layer, the layer's volume is reduced by the ratio of areas.
            // (Volume damaged by weapon) = (layer volume) x (weapon contact area) / (layer contact area)
            //var volDamaged = layer.Volume;
            //if (StrikerContactArea < StrickenContactArea)
            //{
            //    volDamaged *= (StrikerContactArea / StrickenContactArea);
            //}

            //var ca = System.Math.Min(StrikerContactArea, StrickenContactArea);
            Builder.Clear();

            Builder.SetStressMode(mode);
            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrickenMaterial(layer.Material);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerVolume(layer.Volume);
            Builder.SetLayerThickness(layer.Thickness);

            //contactArea = (int) System.Math.Min(contactArea, layer.Thickness);



            Builder.SetStrikerContactArea(StrikerContactArea);
            Builder.SetStrickenContactArea(StrickenContactArea);

            return Builder.Build();
        }
    }
}
