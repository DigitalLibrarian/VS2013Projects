using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Control
{
    static public class ConsoleKeyCompassMapping
    {
        static Dictionary<ConsoleKey, CompassDirection> _Map = new Dictionary<ConsoleKey, CompassDirection>()
            {
                {ConsoleKey.NumPad8, CompassDirection.North},
                {ConsoleKey.UpArrow, CompassDirection.North},
                {ConsoleKey.NumPad6, CompassDirection.East},
                {ConsoleKey.RightArrow, CompassDirection.East},
                {ConsoleKey.NumPad2, CompassDirection.South},
                {ConsoleKey.DownArrow, CompassDirection.South},
                {ConsoleKey.NumPad4, CompassDirection.West},
                {ConsoleKey.LeftArrow, CompassDirection.West},
                {ConsoleKey.NumPad1, CompassDirection.SouthWest},
                {ConsoleKey.NumPad3, CompassDirection.SouthEast},
                {ConsoleKey.NumPad7, CompassDirection.NorthWest},
                {ConsoleKey.NumPad9, CompassDirection.NorthEast}
            };

        static IReadOnlyDictionary<ConsoleKey, CompassDirection> Dictionary { get { return _Map; } }

        public static bool IsCompassKey(ConsoleKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public static CompassDirection ToDirection(ConsoleKey key)
        {
            return ConsoleKeyCompassMapping.Dictionary[key];
        }

        public static bool IsCompassDirection(ConsoleKey key)
        {
            return ConsoleKeyCompassMapping.Dictionary.ContainsKey(key);
        }
    }
}
