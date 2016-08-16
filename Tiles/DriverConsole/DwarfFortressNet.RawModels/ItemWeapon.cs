using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class ItemWeapon
    {
        public const string TokenName = "ITEM_WEAPON";

        public string ReferenceName { get; set; }
        public string Name { get; set; }
        public string NamePlural { get; set; }

        public List<Tag> Tokens { get; set; }

        public static ItemWeapon FromElement(Element ele)
        {
            var weapon = new ItemWeapon
            {
                Tokens = new List<Tag>(),
                ReferenceName = ele.Tags.First().Words[1]
            };

            foreach (var tag in ele.Tags)
            {
                if (tag.Name.Equals("NAME"))
                {
                    weapon.Name = tag.Words[1];
                    weapon.NamePlural = tag.Words[2];
                }

                weapon.Tokens.Add(tag);
            }

            return weapon;
        }
    }
}
