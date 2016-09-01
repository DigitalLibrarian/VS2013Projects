using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;

namespace Tiles.Items
{
    public interface IItem
    {
        IItemClass Class { get; }
        bool IsWeapon { get; }
        bool IsArmor { get; }


        /// <summary>
        /// Mass in g
        /// </summary>
        /// <returns>mass in grams</returns>
        double GetMass();
    }
}
