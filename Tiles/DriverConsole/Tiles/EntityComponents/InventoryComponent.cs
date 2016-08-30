﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.Items;

namespace Tiles.EntityComponents
{
    public interface IInventoryComponent : IComponent
    {
        IInventory Inventory { get; }
    }

    public class InventoryComponent : IInventoryComponent
    {
        public int Id { get { return ComponentTypes.Inventory; } }
        public IInventory Inventory { get; set; }
    }
}
