using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class BodyClassFactory : IBodyClassFactory
    {
        ITissueClass CreateTissues()
        {
            return new TissueClass(new List<ITissueLayerClass>
            {
                new TissueLayerClass(new Material("bone"), 25),
                new TissueLayerClass(new Material("fat"), 15),
                new TissueLayerClass(new Material("skin"), 5),
            });
        }

        public IBodyClass CreateHumanoid()
        {
            var tissueClass = CreateTissues();
            var torso = new BodyPartClass("torso", tissueClass, ArmorSlot.Torso, WeaponSlot.None, new List<ICombatMoveClass>());

            var head = new BodyPartClass("head", tissueClass, ArmorSlot.Head, WeaponSlot.None, new List<ICombatMoveClass>(), 
                parent: torso,
                canAmputate: true,
                isCritical: true);

            var leftArm = new BodyPartClass("left arm", tissueClass, ArmorSlot.LeftArm, WeaponSlot.None, new List<ICombatMoveClass>(), 
                parent: torso,
                canAmputate: true);

            var leftHand = new BodyPartClass("left hand", tissueClass, ArmorSlot.LeftHand, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: leftArm,
                canGrasp: true,
                canAmputate: true);

            var rightArm = new BodyPartClass("right arm", tissueClass, ArmorSlot.RightArm, WeaponSlot.None, new List<ICombatMoveClass>(), 
                parent: torso, 
                canAmputate: true);

            var rightHand = new BodyPartClass("right hand", tissueClass, ArmorSlot.RightHand, WeaponSlot.Main, new List<ICombatMoveClass>(),
                parent: rightArm,
                canGrasp: true,
                canAmputate: true);

            var leftLeg = new BodyPartClass("left leg", tissueClass, ArmorSlot.LeftLeg, WeaponSlot.None, new List<ICombatMoveClass>(), 
                parent:torso,
                canAmputate:true);

            var leftFoot = new BodyPartClass("left foot", tissueClass, ArmorSlot.LeftFoot, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: leftLeg,
                canAmputate: true);

            var rightLeg = new BodyPartClass("right leg", tissueClass, ArmorSlot.RightLeg, WeaponSlot.None, new List<ICombatMoveClass>(), 
                parent: torso,
                canAmputate: true);

            var rightFoot = new BodyPartClass("right foot", tissueClass, ArmorSlot.RightFoot, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: rightLeg,
                canAmputate: true);

            var bodyParts = new List<IBodyPartClass>{
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
            };
            return new BodyClass(bodyParts);
        }

        public IBodyClass CreateFeralHumanoid()
        {
            var tissueClass = CreateTissues();

            var torso = new BodyPartClass("torso", tissueClass, ArmorSlot.Torso, WeaponSlot.None, new List<ICombatMoveClass>());

            var head = new BodyPartClass("head", tissueClass, ArmorSlot.Head, WeaponSlot.Teeth, new List<ICombatMoveClass>(),
                parent: torso,
                canAmputate: true,
                isCritical: true);

            var leftArm = new BodyPartClass("left arm", tissueClass, ArmorSlot.LeftArm, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canAmputate: true);

            var leftHand = new BodyPartClass("left hand", tissueClass, ArmorSlot.LeftHand, WeaponSlot.Claw, new List<ICombatMoveClass>(),
                parent: leftArm,
                canGrasp: true,
                canAmputate: true);

            var rightArm = new BodyPartClass("right arm", tissueClass, ArmorSlot.RightArm, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canAmputate: true);

            var rightHand = new BodyPartClass("right hand", tissueClass, ArmorSlot.RightHand, WeaponSlot.Claw, new List<ICombatMoveClass>(),
                parent: rightArm,
                canGrasp: true,
                canAmputate: true);

            var leftLeg = new BodyPartClass("left leg", tissueClass, ArmorSlot.LeftLeg, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canAmputate: true);

            var leftFoot = new BodyPartClass("left foot", tissueClass, ArmorSlot.LeftFoot, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: leftLeg,
                canAmputate: true);

            var rightLeg = new BodyPartClass("right leg", tissueClass, ArmorSlot.RightLeg, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: torso,
                canAmputate: true);

            var rightFoot = new BodyPartClass("right foot", tissueClass, ArmorSlot.RightFoot, WeaponSlot.None, new List<ICombatMoveClass>(),
                parent: rightLeg,
                canAmputate: true);

            var bodyParts = new List<IBodyPartClass>{
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
            };
            return new BodyClass(bodyParts);
        }
    }
}
