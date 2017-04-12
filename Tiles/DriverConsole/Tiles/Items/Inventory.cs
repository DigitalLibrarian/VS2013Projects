using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Items
{
    public class Inventory : IInventory
    {
        IList<IItem> Items { get; set; }
        public Inventory()
        {
            Items = new List<IItem>();
        }

        public void AddItem(IItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(IItem item)
        {
            foreach (var pair in Worn.ToList())
            {
                if (pair.Value == item)
                {
                    RestoreFromWorn(item);
                }
            }

            if (Items.Contains(item))
            {
                Items.Remove(item);
            }
            
        }

        public IEnumerable<IItem> GetItems()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }
        
        //TODO - this "Worn" construct needs to go away.  Outfit can play the role of anything "Worn"
        Dictionary<object, IItem> Worn = new Dictionary<object, IItem>();
        public void AddToWorn(object key, IItem item)
        {
            Items.Remove(item);
            Worn[key] = item;
        }
        
        public void RestoreFromWorn(object key)
        {
            var item = Worn[key];
            Items.Add(item);
            Worn.Remove(key);
        }
        
        public IEnumerable<IItem> GetWorn()
        {
            foreach (var item in Worn.Values)
            {
                yield return item;
            }
        }
        
        public IItem GetWorn(object key)
        {
            if (Worn.ContainsKey(key))
            {
                return Worn[key];
            }
            else
            {
                return null;
            }
        }
    }
}
