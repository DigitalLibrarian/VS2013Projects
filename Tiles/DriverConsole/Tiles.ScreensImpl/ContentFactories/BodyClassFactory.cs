using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class BodyClassFactory : IBodyClassFactory
    {
        ITissueClass CreateTissues()
        {
            return new TissueClass(new List<ITissueLayerClass>
            {
                new TissueLayerClass(new Material("bone", "bone"), 25),
                new TissueLayerClass(new Material("bone", "fat"), 15),
                new TissueLayerClass(new Material("bone", "skin"), 5),
            });
        }

        Dictionary<string, BodyPartClass> Human(ITissueClass tissueClass)
        {
            var torso = new BodyPartClass("torso", tissueClass, ArmorSlot.Torso, WeaponSlot.None, new List<ICombatMoveClass>(),
                canBeAmputated: false);

            var head = new BodyPartClass("head", tissueClass, ArmorSlot.Head, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canBeAmputated: true,
                isCritical: true);

            var leftArm = new BodyPartClass("left arm", tissueClass, ArmorSlot.LeftArm, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canBeAmputated: true);

            var leftHand = new BodyPartClass("left hand", tissueClass, ArmorSlot.LeftHand, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: leftArm,
                canGrasp: true,
                canBeAmputated: true);

            var rightArm = new BodyPartClass("right arm", tissueClass, ArmorSlot.RightArm, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canBeAmputated: true);

            var rightHand = new BodyPartClass("right hand", tissueClass, ArmorSlot.RightHand, WeaponSlot.Main, new List<ICombatMoveClass>(),
                parent: rightArm,
                canGrasp: true,
                canBeAmputated: true);

            var leftLeg = new BodyPartClass("left leg", tissueClass, ArmorSlot.LeftLeg, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canBeAmputated: true);

            var leftFoot = new BodyPartClass("left foot", tissueClass, ArmorSlot.LeftFoot, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: leftLeg,
                canBeAmputated: true);

            var rightLeg = new BodyPartClass("right leg", tissueClass, ArmorSlot.RightLeg, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canBeAmputated: true);

            var rightFoot = new BodyPartClass("right foot", tissueClass, ArmorSlot.RightFoot, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: rightLeg,
                canBeAmputated: true);

            var pD = new Dictionary<string, BodyPartClass>();

            foreach(var bc in new List<BodyPartClass>(){
                head,
                torso,
                leftArm,
                rightArm,
                leftHand,
                rightHand,
                leftLeg,
                rightLeg,
                leftFoot,
                rightFoot
            })
            {
                pD.Add(bc.Name, bc);
            }
            return pD;
        }

        public IBodyClass CreateHumanoid()
        {
            var tissueClass = CreateTissues();
            var bodyParts = Human(tissueClass).Values.ToList();
            return new BodyClass(bodyParts);
        }

        public IBodyClass CreateFeralHumanoid()
        {
            var tissueClass = CreateTissues();
            var bodyParts = Human(tissueClass);
            bodyParts["head"].WeaponSlot = WeaponSlot.Teeth;
            bodyParts["left hand"].WeaponSlot = WeaponSlot.Claw;
            bodyParts["right hand"].WeaponSlot = WeaponSlot.Claw;
            return new BodyClass(bodyParts.Values.ToList());
        }
    }
}
