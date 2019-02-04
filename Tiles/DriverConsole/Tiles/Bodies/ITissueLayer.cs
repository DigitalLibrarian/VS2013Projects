using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Injuries;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Bodies
{
    public interface ITissueLayer
    {
        ITissueLayerClass Class { get; }
        IMaterial Material { get; }
        IDamageVector Damage { get; }

        /// <summary>
        /// Thickness in m^-5
        /// </summary>
        double Thickness { get; }
        double Volume { get; }

        double WoundAreaRatio { get; }
        double PenetrationRatio { get; }

        string Name { get; }

        bool IsPenetrated { get; }
        bool IsPristine { get; }
        bool IsPulped { get; }
        bool IsVascular { get; }

        void AddInjury(ITissueLayerInjury injury);

    }
}
