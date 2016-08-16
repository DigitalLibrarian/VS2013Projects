using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

using Df = DwarfFortressNet.RawModels;

namespace DwarfFortressNet.Bridge
{
    public class DfCreatureBodyBuilder
    {
        Dictionary<string, List<Df.BodyPart>> PartDefsByToken { get; set; }

        Dictionary<string, List<IBodyPart>> CreatedPartsByToken { get; set; }
        Dictionary<string, IBodyPart> CreatedPartsByName { get; set; }

        IList<IBodyPart> BodyParts { get; set; }

        Creature Creature { get; set; }
        ObjectDb Db { get; set; }
        public DfCreatureBodyBuilder(Creature creature, ObjectDb objDb)
        {
            Creature = creature;
            Db = objDb;

            Clear();
        }
        public void Clear()
        {
            PartDefsByToken = new Dictionary<string, List<Df.BodyPart>>();
            CreatedPartsByName = new Dictionary<string, IBodyPart>();
            CreatedPartsByToken = new Dictionary<string, List<IBodyPart>>();

            BodyParts = new List<IBodyPart>();
        }

        public IEnumerable<Df.BodyPart> AllPartDefns
        {
            get
            {
                return Creature.BodyPartSets.Select(refName => Db.Get<Df.BodyPartSet>(refName)).SelectMany(set => set.BodyParts);
            }
        }

        public IEnumerable<Df.BodyPart> RootPartDefns
        {
            get
            {
                return AllPartDefns.Where(part => part.Con == null && part.ConType == null);
            }
        }

        public IEnumerable<Df.BodyPart> SpecificallyConnectedPartDefns
        {
            get
            {
                return AllPartDefns.Where(part => part.Con != null);
            }
        }

        public IEnumerable<Df.BodyPart> TokenConnectedPartDefns
        {
            get
            {
                return AllPartDefns.Where(part => part.ConType != null);
            }
        }

        void RecordPart(Df.BodyPart dfPart, IBodyPart part)
        {
            var refName = dfPart.ReferenceName;
            CreatedPartsByName.Add(refName, part);

            foreach (var tokenName in dfPart.Tokens.Where(x => x.IsSingleWord()).Select(x => x.Words.First()))
            {
                if (CreatedPartsByToken.ContainsKey(tokenName))
                {
                    CreatedPartsByToken[tokenName].Add(part);
                }
                else
                {
                    CreatedPartsByToken[tokenName] = new List<IBodyPart> { part };
                }
            }
        }

        public void AddRootParts()
        {
            foreach (var part in RootPartDefns)
            {
                var bp = new Tiles.Bodies.BodyPart(
                    name: part.Name,
                    isCritical: part.Tokens.Any(x => x.IsSingleWord("THOUGHT")),
                    canAmputate: false,
                    canGrasp: part.Tokens.Any(token => token.IsSingleWord("GRASP")),
                    armorSlotType: Tiles.Items.ArmorSlot.None,
                    weaponSlotType: Tiles.Items.WeaponSlot.None
                    );

                RecordPart(part, bp);
                BodyParts.Add(bp);
            }
        }

        public void AddSpecificallyConnectedParts()
        {
            foreach (var part in SpecificallyConnectedPartDefns)
            {
                var parentBP = CreatedPartsByName[part.Con];

                var bp = new Tiles.Bodies.BodyPart(
                    name: part.Name,
                    isCritical: part.Tokens.Any(x => x.IsSingleWord("THOUGHT")),
                    canAmputate: false,
                    canGrasp: part.Tokens.Any(token => token.IsSingleWord("GRASP")),
                    armorSlotType: Tiles.Items.ArmorSlot.None,
                    weaponSlotType: Tiles.Items.WeaponSlot.None,
                    parent: parentBP
                    );

                RecordPart(part, bp);
                BodyParts.Add(bp);
            }
        }

        public void AddTokenConnectedParts()
        {
            foreach (var part in TokenConnectedPartDefns)
            {
                foreach (var targetParent in CreatedPartsByToken[part.ConType])
                {
                    var bp = new Tiles.Bodies.BodyPart(
                        name: part.Name,
                        isCritical: part.Tokens.Any(x => x.IsSingleWord("THOUGHT")),
                        canAmputate: false,
                        canGrasp: part.Tokens.Any(token => token.IsSingleWord("GRASP")),
                        armorSlotType: Tiles.Items.ArmorSlot.None,
                        weaponSlotType: Tiles.Items.WeaponSlot.None,
                        parent: targetParent
                        );

                    BodyParts.Add(bp);
                }
            }
        }

        public IBody Build()
        {
            return new Body(BodyParts);
        }


        public static IBody FromCreatureDefinition(Creature c, ObjectDb objDb)
        {
            var builder = new DfCreatureBodyBuilder(c, objDb);
            builder.AddRootParts();
            builder.AddSpecificallyConnectedParts();
            builder.AddTokenConnectedParts();
            return builder.Build();
        }
    }
}
