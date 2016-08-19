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
using Tiles.Math;
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

            while (true)
            {
                Console.Write(":");
                var referenceName = Console.ReadLine();
                if (referenceName.ToLower().Equals("q")) break;
                var c = fab.Creatures.SingleOrDefault(x => x.ReferenceName == referenceName);
                if (c == null)
                {
                    Console.WriteLine("Not found");
                }
                else
                {
                    var body = fab.CreateBody(c);
                    int br = 9;
                }
            }

        }



        static IItem CreateWeapon(Inorganic inorg, ItemWeapon weapon, ObjectDb objDb)
        {
            return DfWeaponItemBuilder.FromDefinition(inorg, weapon, objDb);
        }
    }
}
