using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content
{
    public class Verb
    {
        bool IsTransitive { get; set; }
        string SecondPerson { get; set; }
        string ThirdPerson { get; set; }
    }

    public class Material
    {
        public string Adjective { get; set; }
    }
    
    public class TissueLayer
    {
        Material Material { get; set; }
        int RelativeThickness { get; set; }
    }

    public class Tissue
    {
        List<TissueLayer> Layers { get; set; }
    }

    public class BodyPart
    {
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        BodyPart Parent { get; set; }

        public bool CanBeAmputated { get; set; }
        public bool CanGrasp { get; set; }
    }

    public class Body
    {
        List<BodyPart> Parts { get; set; }
    }


    public class CombatMove
    {
        string Name { get; set; }

        int PrepTime { get; set; }
        int RecoveryTime { get; set; }

        bool IsDefenderPartSpecific { get; set; }
        bool IsMartialArts { get; set; }
        bool IsStrike { get; set; }
        bool IsItem { get; set; }

        int ContactArea { get; set; }
        int MaxPenetration { get; set; }
        int VelocityMultiplier { get; set; }
    }


    public class Weapon
    {
        List<string> SlotRequirements { get; set; }
        List<CombatMove> Moves { get; set; }
    }

    public class Armor
    {
        List<string> SlotRequirements { get; set; }

    }

    public class Item
    {
        Weapon Weapon { get; set; }
        Armor Armor { get; set; }

        Material Material { get; set; }
    }

    public class Agent
    {
        Body Body { get; set; }
    }
}
