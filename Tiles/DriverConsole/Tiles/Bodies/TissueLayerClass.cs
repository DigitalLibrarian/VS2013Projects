using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class TissueLayerClass : ITissueLayerClass
    {
        public IMaterial Material { get; set; }
        public int RelativeThickness { get; set; }
        public bool IsCosmetic { get; set; }
        public bool IsConnective { get; set; }
        public int VascularRating { get; set; }
        public int PainReceptors { get; set; }
        public int HealingRate { get; set; }
        public bool ThickensOnStrength { get; set; }
        public bool ThickensOnEnergyStorage { get; set; }
        public bool HasArteries { get; set; }
        public bool HasMajorArteries { get; set; }

        public string Name { get { return Material.Name; } }

        public TissueLayerClass(IMaterial material, int relThick)
        {
            Material = material;
            RelativeThickness = relThick;
        }
    }
}
