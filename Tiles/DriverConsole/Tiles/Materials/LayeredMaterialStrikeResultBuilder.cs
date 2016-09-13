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
                if (new string[] { 
                    "hair"
                }.Contains(layer.Material.Name)){
                    continue;
                }
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
                    /*
                    layerResult = PerformSingleLayerTest(
                       StrikerMaterial,
                       momentum,
                       ContactArea,
                       mode,
                       layer);
                     */
                    //layerResult.StressMode = StressMode; // only first layer maintains the original stress mode
                    
                }
                else
                {
                    done = true;
                }

                /*
                if (layerResult.BreaksThrough)
                {
                    momentum = momentum - (momentum * (20d / 100d));
                    momentum = momentum - layerResult.MomentumThreshold;
                    if (mode == Materials.StressMode.Edge)
                    {
                        penetration += (int)layer.Thickness;
                    }
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


                    layerResult.StressMode = StressMode; // only first layer maintains the original stress mode
                    if (layerResult.BreaksThrough)
                    {
                        momentum = momentum - (momentum * (20d / 100d));
                        momentum = momentum - layerResult.MomentumThreshold;

                        // allow further testing with blunt (current layer keeps original stress mode for partial cuts)
                    }
                    else
                    {
                        // TODO - If both edged and blunt momenta thresholds haven't been met, attack is permanently converted to blunt and its momentum may be greatly reduced. 
                        // Specifically, it is multiplied by SHEAR_STRAIN_AT_YIELD/50000 for edged attacks or IMPACT_STRAIN_AT_YIELD/50000 otherwise. 
                        momentum = momentum - (momentum * (60d / 100d));
                    }
                }
                else
                {
                    // blunt failed to propagate
                    done = true;
                }
                */
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
