using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Items
{
    public interface IInventory
    {
        // TODO - bounded volume and weight
        void AddItem(IItem item);
        void RemoveItem(IItem item);

        void AddToWorn(object key, IItem item);
        void RestoreFromWorn(object key);
        
        IEnumerable<IItem> GetItems();
        IEnumerable<IItem> GetWorn();
        IItem GetWorn(object key);
    }
}
