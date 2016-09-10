using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfBodyAttack
    {
        public string ReferenceName { get; set; }
        public IEnumerable<string> ByTypes { get; set; }
        public IEnumerable<string> ByCategories { get; set; }
        public Verb Verb { get; set; }

        public int ContactPercent { get; set; }
        public int PenetrationPercent { get; set; }

        public int PrepTime { get; set; }
        public int RecoveryTime { get; set; }
    }
}
