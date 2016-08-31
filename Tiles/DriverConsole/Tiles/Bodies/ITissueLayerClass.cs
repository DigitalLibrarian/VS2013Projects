using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public interface ITissueLayerClass
    {
        IMaterial Material { get; }
        int RelativeThickness { get; }
    }
}
