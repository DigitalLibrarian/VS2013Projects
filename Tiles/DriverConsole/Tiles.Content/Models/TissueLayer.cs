﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class TissueLayer
    {
        public TissueLayer()
        {
            Material = new Material();
        }
        public Material Material { get; set; }
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
    }
}
