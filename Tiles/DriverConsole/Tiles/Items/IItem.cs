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
        string Name { get; }
        ISprite Sprite { get; }

        bool IsWeapon { get; }
        IWeapon Weapon { get; }

        bool IsArmor { get; }
        IArmor Armor { get; }
    }
    
    //public interface IContentStore<TContent>
    //{
    //    TContent Get(long id);
    //    IEnumerable<TContent> GetAll();
    //}
        
    //public class ContentStore<TContent> : IContentStore<TContent>
    //{
    //    IDictionary<long, TContent> Data { get; set; }
    //    public ContentStore(IDictionary<long, TContent> idMap)
    //    {
    //        Data = idMap;
    //    }

    //    public TContent Get(long id)
    //    {
    //        return Data[id];
    //    }
        
    //    public IEnumerable<TContent> GetAll()
    //    {
    //        return Data.Values;
    //    }
    //}

    //public interface IConsumableClass
    //{

    //}


    //public interface IItemClass
    //{
    //    string Name { get; }
    //    ISprite Sprite { get; }
    //    bool Stacks { get; }
    //    uint StackLimit { get; }

    //    IWeaponClass WeaponClass { get; }
    //    IArmorClass ArmorClass { get; }

    //    IEnumerable<IItemClass> DisassemblyProducts { get; }
    //}

    //public interface ICraftingRecipe
    //{
    //    IReadOnlyDictionary<IItemClass, uint> Dependencies { get; }

    //    IItemClass Product { get; }
    //    uint Yield { get; }
    //}

    //public interface ICraftingRecipeManager
    //{
    //    IEnumerable<ICraftingRecipe> GetAll();

    //    IEnumerable<ICraftingRecipe> FindByProduct(IItemClass product);
    //}

}
