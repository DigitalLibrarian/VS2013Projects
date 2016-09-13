using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public interface ITissueLayer
    {
        ITissueLayerClass Class { get; }
        IMaterial Material { get; }
        /// <summary>
        /// Thickness in m^-5
        /// </summary>
        double Thickness { get; }
        double Volume { get; }

        bool CanBeBruised { get; }
        bool CanBeTorn { get; }
        bool CanBePunctured { get; }
    }
}
