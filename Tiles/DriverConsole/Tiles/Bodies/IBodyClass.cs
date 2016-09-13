using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Bodies
{
    public interface IBodyClass
    {
        IEnumerable<ICombatMoveClass> Moves { get; set; }
        IEnumerable<IBodyPartClass> Parts { get; set; }
        /// <summary>
        /// Volume in cm3
        /// </summary>
        double Size { get; set; }
    }
}
