﻿using System;
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
        public TissueLayerClass(IMaterial material, int relThick, bool isCosmetic)
        {
            Material = material;
            RelativeThickness = relThick;
            IsCosmetic = isCosmetic;
        }
    }
}
