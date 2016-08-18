using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
//[ITEM_WEAPON:ITEM_WEAPON_SWORD_SHORT]
//[NAME:short sword:short swords]
//[SIZE:300]
//[SKILL:SWORD]
//[TWO_HANDED:37500]
//[MINIMUM_SIZE:32500]
//[CAN_STONE]
//[MATERIAL_SIZE:3]
//[ATTACK:EDGE:20000:4000:slash:slashes:NO_SUB:1250]
//    [ATTACK_PREPARE_AND_RECOVER:3:3]
//[ATTACK:EDGE:50:2000:stab:stabs:NO_SUB:1000]
//    [ATTACK_PREPARE_AND_RECOVER:3:3]
//[ATTACK:BLUNT:20000:4000:slap:slaps:flat:1250]
//    [ATTACK_PREPARE_AND_RECOVER:3:3]
//[ATTACK:BLUNT:100:1000:strike:strikes:pommel:1000]
//    [ATTACK_PREPARE_AND_RECOVER:3:3]
    public class ItemWeapon
    {
        public const string TokenName = "ITEM_WEAPON";

        public string ReferenceName { get; set; }
        public string Name { get; set; }
        public string NamePlural { get; set; }

        public List<Tag> Tokens { get; set; }
        public List<WeaponAttack> Attacks { get; set; }

        public static ItemWeapon FromElement(Element ele)
        {
            var weapon = new ItemWeapon
            {
                Tokens = new List<Tag>(),
                ReferenceName = ele.Tags.First().Words[1],
                Attacks = new List<WeaponAttack>()
            };

            int i = 0;
            foreach (var tag in ele.Tags)
            {
                if (tag.Name.Equals("NAME"))
                {
                    weapon.Name = tag.Words[1];
                    weapon.NamePlural = tag.Words[2];
                } 
                if (tag.Name.Equals("ATTACK"))
                {
                    weapon.Attacks.Add(WeaponAttack.FromTags(ele.Tags.Skip(i).ToList()));
                }

                weapon.Tokens.Add(tag);
                i++;
            }

            return weapon;
        }

        //[ATTACK:BLUNT:100:1000:strike:strikes:pommel:1000]
        //    [ATTACK_PREPARE_AND_RECOVER:3:3]
        public class WeaponAttack
        {
            public string VerbSecondPerson { get; set; }
            public string VerbThirdPerson { get; set; }
            public string Noun { get; set; }
            public string AttackType { get; set; }

            public int ContactArea { get; set; }
            public int PenetrationSize { get; set; }
            public int VelocityMultiplier { get; set; }

            public int PrepTime { get; set; }
            public int RecoveryTime { get; set; }

            public List<Tag> Tokens { get; set; }

            public static WeaponAttack FromTags(IList<Tag> tags)
            {
                var firstTag = tags.First();
                var wa = new WeaponAttack
                {
                    AttackType = firstTag.Words[1],
                    ContactArea = int.Parse(firstTag.Words[2]),
                    PenetrationSize = int.Parse(firstTag.Words[3]),
                    VerbSecondPerson = firstTag.Words[4],
                    VerbThirdPerson = firstTag.Words[5],
                    Noun = firstTag.Words[6],
                    VelocityMultiplier = int.Parse(firstTag.Words[7]),
                    Tokens = new List<Tag> { firstTag}
                };

                foreach (var tag in tags.Skip(1))
                {
                    if (tag.Name.Equals("ATTACK_PREPARE_AND_RECOVER"))
                    {
                        wa.PrepTime = int.Parse(tag.Words[1]);
                        wa.RecoveryTime = int.Parse(tag.Words[2]);
                    }

                    if (tag.Name.Equals("ATTACK")) break;
                    wa.Tokens.Add(tag);
                }
                return wa;
            }
        }
    }
}
