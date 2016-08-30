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
        private static int _NextId = 1;

        public static readonly int Body = _NextId++;
        public static readonly int Inventory = _NextId++;
        public static readonly int Outfit = _NextId++;
        public static readonly int Sprite = _NextId++;
        public static readonly int Command = _NextId++;
        public static readonly int Agent = _NextId++;

        public static readonly int AtlasPosition = _NextId++;
    }
}
