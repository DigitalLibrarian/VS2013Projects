using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace Tiles.Render
{
    public interface IRenderer
    {
        void DisplayAtlasFrame(IAtlas atlas, int camX, int camY);

        void DisplayDebugLog(IEnumerable<string> lines);
    }
}
