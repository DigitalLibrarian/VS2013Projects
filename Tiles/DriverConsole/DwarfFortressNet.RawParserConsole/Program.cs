using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using DwarfFortressNet.RawModels;
using Tiles.Bodies;
using DwarfFortressNet.Bridge;
using Tiles.Items;
namespace DwarfFortressNet.RawParserConsole
{
    class Program
    {
        private static readonly string _DirKey = @"DwarfFortressRawsDirectory";
        static void Main(string[] args)
        {
            var dirStr = System.Configuration.ConfigurationManager.AppSettings.Get(_DirKey);

            var fab = new DfFabricator();
            fab.ReadDfRawDir(dirStr);
            
            
            foreach (var inorg in fab.Inorganics)
            {
                foreach (var weapon in fab.ItemWeapons)
                {
                    fab.CreateWeapon(inorg, weapon);
                }
            }


            var c = fab.Creatures.Single(x => x.ReferenceName == "GOBLIN");
            fab.CreateBody(c);

            System.Console.ReadKey();
        }


        static IItem CreateWeapon(Inorganic inorg, ItemWeapon weapon, ObjectDb objDb)
        {
            return DfWeaponItemBuilder.FromDefinition(inorg, weapon, objDb);
        }
    }
}
