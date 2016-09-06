using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface ILayeredMaterialStrikeResult
    {
        int Penetration { get; }
        IEnumerable<IMaterialStrikeResult> LayerResults { get; }
        IDictionary<object, IMaterialStrikeResult> TaggedResults { get; }

        void AddLayerResult(IMaterialStrikeResult result);
        void AddLayerResult(IMaterialStrikeResult result, object tag);
    }

    public class LayeredMaterialStrikeResult : ILayeredMaterialStrikeResult
    {
        List<IMaterialStrikeResult> Results { get; set; }
        Dictionary<object, IMaterialStrikeResult> Tagged { get; set; }
        public int Penetration { get; set; }
        public LayeredMaterialStrikeResult()
        {
            Results = new List<IMaterialStrikeResult>();
            Tagged = new Dictionary<object, IMaterialStrikeResult>();
        }

        public IEnumerable<IMaterialStrikeResult> LayerResults { get { return Results; } }
        public IDictionary<object, IMaterialStrikeResult> TaggedResults { get { return Tagged; } }

        public void AddLayerResult(IMaterialStrikeResult result)
        {
            Results.Add(result);
        }

        public void AddLayerResult(IMaterialStrikeResult result, object tag)
        {
            AddLayerResult(result);
            Tagged.Add(tag, result);
        }
    }

    public interface ILayeredMaterialStrikeResultBuilder
    {
        void AddLayer(IMaterial mat);
        void AddLayer(IMaterial mat, int thick, object tag);

        void SetStrikerMaterial(IMaterial mat);

        void SetMomentum(double momentum);
        void SetContactArea(int contactArea);
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

            public int Thickness { get; set; }
            public IMaterial Material { get; set; }
        }

        IMaterialStrikeResultBuilder Builder { get; set; }

        double Momentum { get; set; }
        int ContactArea { get; set; }
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
        public void AddLayer(IMaterial mat, int thick, object tag)
        {
            Layers.Add(new MLayer
            {
                Material = mat,
                Thickness = thick,
                Tag = tag
            });
        }

        public ILayeredMaterialStrikeResult Build()
        {
            var result = new LayeredMaterialStrikeResult();

            var momentum = Momentum/(double) ContactArea;
            var contactArea = ContactArea;
            var mode = StressMode;
            int penetration = 0;

            foreach (var layer in Layers)
            {
                if (momentum <= 0) break;

                var layerResult = PerformSingleLayerTest(
                    StrikerMaterial,
                    momentum,
                    contactArea,
                    mode,
                    layer);

                if (layerResult.BreaksThrough)
                {
                    momentum = momentum - (momentum * (5d / 100d));
                    penetration += layer.Thickness;
                }
                else if (mode != StressMode.Blunt)
                {
                    // fail to pierce/cut
                    layerResult = PerformSingleLayerTest(
                       StrikerMaterial,
                       momentum,
                       contactArea,
                       StressMode,
                       layer);

                    if (layerResult.BreaksThrough)
                    {
                        momentum = momentum - (momentum * (5d / 100d));
                        penetration += layer.Thickness;
                    }
                    else
                    {
                        mode = StressMode.Blunt;
                        momentum = momentum  - (momentum * (33d / 100d));
                    }
                }
                
                if (layer.IsTagged)
                {
                    result.AddLayerResult(layerResult, layer.Tag);
                }
                else
                {
                    result.AddLayerResult(layerResult);
                }
            }

            result.Penetration = penetration;

            return result;
        }

        IMaterialStrikeResult PerformSingleLayerTest(
            IMaterial strikerMat, double momentum, int contactArea,
            StressMode mode, MLayer layer)
        {
            Builder.Clear();

            Builder.SetStressMode(mode);
            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrickenMaterial(layer.Material);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            return Builder.Build();
        }
    }
}
