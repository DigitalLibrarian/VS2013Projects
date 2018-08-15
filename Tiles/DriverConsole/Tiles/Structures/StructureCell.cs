using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Structures
{
    public class StructureCell : IStructureCell
    {
        public Sprite Sprite { get; set; }
        public IStructure Structure { get; private set; }
        public StructureCellType Type { get; set; }
        public bool IsOpen { get; set; }
        public bool CanOpen { get; set; }
        public bool CanClose { get; set; }
        public bool CanPass { get; set; }

        public StructureCell(
            IStructure structure,
            StructureCellType type,
            Sprite sprite,
            bool canOpen = false,
            bool canClose = false,
            bool canPass = true,
            bool isOpen = false)
        {
            Structure = structure;
            Type = type;
            Sprite = sprite;
            IsOpen = isOpen;
            CanOpen = canOpen;
            CanClose = canClose;
            CanPass = canPass;
        }
        
        public bool Open()
        {
            if (IsOpen) return false;
            if (!CanOpen) return false;
            IsOpen = true;
            return true;
        }

        public bool Close()
        {
            if (!IsOpen) return false;
            if (!CanClose) return false;
            IsOpen = false;
            return true;
        }
    }
}
