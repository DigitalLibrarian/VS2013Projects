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
            var possCreatures = fab.Creatures;
            foreach (var creature in possCreatures)
            {
                Console.WriteLine(creature.ReferenceName);
            }
            Console.WriteLine(string.Format("Total: {0}/{1}", possCreatures.Count(), fab.Creatures.Count()));
            while (true)
            {
                Console.Write(":");
                var referenceName = Console.ReadLine();
                if (referenceName.ToLower().Equals("q")) break;
                var c = fab.Creatures.SingleOrDefault(x => x.ReferenceName == referenceName);
                int total = 0;
                if (c == null)
                {
                    if (referenceName == "ALL")
                    {
                        foreach (var possCreature in possCreatures)
                        {
                            if (!possCreature.Castes.Any())
                            {
                                //var body = fab.CreateBody(possCreature);
                                // total++;
                            }
                            foreach (var caste in possCreature.Castes)
                            {
                                var body = fab.CreateBody(possCreature, caste);
                                total++;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not found");
                    }
                }
                else
                {
                    if (!c.Castes.Any())
                    {
                        //fab.CreateBody(c);
                        //total++;
                    }
                    foreach (var caste in c.Castes)
                    {
                        fab.CreateBody(c, caste);
                        total++;
                    }
                }
                Console.WriteLine(string.Format("Created {0}", total));
            }
        }



        static IItem CreateWeapon(Inorganic inorg, ItemWeapon weapon, ObjectDb objDb)
        {
            return DfWeaponItemBuilder.FromDefinition(inorg, weapon, objDb);
        }
    }
}
