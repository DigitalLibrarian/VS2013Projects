using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Bodies;
using Tiles.Ecs;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Math;

namespace Tiles.EntityComponents
{
    public static class ComponentTypes
    {
        public static readonly int Particle = 1;
        public static readonly int Body = 2;
        public static readonly int Inventory = 3;
        public static readonly int Outfit = 4;
        public static readonly int Sprite = 5;
        public static readonly int Command = 6;
        public static readonly int Agent = 7;
    }
}
