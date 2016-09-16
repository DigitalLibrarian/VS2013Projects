using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Bodies
{
    public class TissueLayer : ITissueLayer
    {
        public ITissueLayerClass Class { get; private set; }
        public IMaterial Material { get; private set; }
        public ITissueLayerStatus Status { get; private set; }
        public double Thickness { get; private set; }
        public double Volume { get; private set; }

        public TissueLayer(ITissueLayerClass layerClass, IMaterial material, double thickness, double volume)
        {
            Class = layerClass;
            Material = material;
            Thickness = thickness;
            Volume = volume;
            Status = new TissueLayerStatus();
        }
        
        public bool CanBeBruised { get; set; }
        public bool CanBeTorn { get; set; }
        public bool CanBePunctured { get; set; }
    }

    public interface ITissueLayerStatus
    {
        Fraction DentFraction { get; }
        Fraction CutFraction { get; }

        double WoundArea { get; }

        bool IsPulped();
    }

    public class TissueLayerStatus : ITissueLayerStatus
    {
        public Fraction DentFraction { get; set; }
        public Fraction CutFraction { get; set; }
        public double WoundArea { get; set; }

        public TissueLayerStatus()
        {
            const int denom = 10000;
            DentFraction = new Fraction(0, denom);
            CutFraction = new Fraction(0, denom);
        }

        public bool IsPulped()
        {
            return DentFraction.AsDouble() >= 2.5d
                || CutFraction.AsDouble() >= 1d;
        }
    }
}
