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

            public int Thickness { get; set; }
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
            var contactArea = (double)ContactArea;
            
            var mode = StressMode;
            var momentum = Momentum;

            int penetration = 0;
            bool done = false;

            foreach (var layer in Layers)
            {
                if (momentum <= 0) break;
                if(mode == StressMode.Edge && penetration >= MaxPenetration) break;

                var layerResult = PerformSingleLayerTest(
                    StrikerMaterial,
                    momentum,
                    ContactArea,
                    mode,
                    layer);

                if (layerResult.BreaksThrough)
                {
                    momentum = momentum - (momentum * (5d / 100d));
                    penetration += layer.Thickness;
                }
                else if (mode != StressMode.Blunt)
                {
                    // fail to pierce/cut, convert to blunt and greatly reduce
                    mode = StressMode.Blunt;
                    layerResult = PerformSingleLayerTest(
                       StrikerMaterial,
                       momentum,
                       ContactArea,
                       mode,
                       layer);

                    layerResult.StressMode = StressMode;
                    if (layerResult.BreaksThrough)
                    {
                        momentum = momentum - (momentum * (5d / 100d));
                        penetration += layer.Thickness;
                        // allow furth testing with blunt
                    }
                    else
                    {
                        // TODO - If both edged and blunt momenta thresholds haven't been met, attack is permanently converted to blunt and its momentum may be greatly reduced. Specifically, it is multiplied by SHEAR_STRAIN_AT_YIELD/50000 for edged attacks or IMPACT_STRAIN_AT_YIELD/50000 otherwise. 
                        momentum = momentum - (momentum * (30d / 100d));
                        done = true;
                    }
                }
                else
                {
                    // blunt failed to propagate
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

            if(result.TaggedResults.Where(x => x.Value.BreaksThrough).Count()
                == Layers.Where(l => l.IsTagged).Count())
            {
                // if edged, and contact area and penetration spell out a big
                // enough volume (compared to the part), then we have a severed limb
                int br = 0;
            }
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
