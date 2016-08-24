using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfItemBuilder
    {
        void SetMaterial(Material material);
        void AddCombatMove(CombatMove move);

        void SetName(string singular, string plural);

        Item Build();
    }
}
