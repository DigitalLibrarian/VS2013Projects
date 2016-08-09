using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Structures
{
    public interface IStructureCell
    {
        ISprite Sprite { get; set; }
        IStructure Structure { get;}
        StructureCellType Type { get; set; }

        bool IsOpen { get; set; }
        bool CanOpen { get; set; }
        bool CanClose { get; set; }
        bool CanPass { get; set; }

        bool Open();
        bool Close();
    }
}
